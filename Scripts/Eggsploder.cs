using Godot;
using System;
using System.Collections;
using System.Diagnostics;


public partial class Eggsploder : CharacterBody2D
{
	public enum State
	{
		IDLE = 8,
		FALLING = 16,
		LANDED = 32,
		EXPLODE = 64,
		EXPLOSION = 128
	};
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = 0;

	private State state = State.IDLE;

	public override void _Ready()
	{
		base._Ready();
		gravity = ((Scene)GetParent()).Gravity;
	}

	public Scene.ObjectTypes ObjectType()
	{
		return Scene.ObjectTypes.EGGPSLODER;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (StateHas(State.EXPLOSION))
		{
			Area2D explosion_area = (Area2D)GetNode("explosion_area");

			Godot.Collections.Array<Node2D> bodies = explosion_area.GetOverlappingBodies();

			for (int i = 0; i < bodies.Count; i++)
			{
				if (bodies[i].HasMethod("OnDeath"))
				{
					bodies[i].Call("OnDeath");
				}
			}
		}
		else
		{
			for (int i = 0; i < GetSlideCollisionCount(); i++)
			{
				KinematicCollision2D col = GetSlideCollision(i);
				if (col.GetCollider().GetType().ToString() == "player")
				{
					player obj = (player)col.GetCollider();
					obj.OnDeath();
				}
				if (col.GetCollider() is not TileMap)
				{
					state |= State.EXPLOSION;
				}
			}
			// Add the gravity.
			if (!IsOnFloor())
			{
				velocity.Y += gravity * (float)delta;
				state |= State.FALLING;
			}
			else
			{
				state &= ~State.FALLING;
				velocity.Y = 0;
				if (!StateHas(State.LANDED) && !StateHas(State.EXPLOSION))
				{
					state |= State.EXPLODE;
				}
			}

			Velocity = velocity;
			bool was_on_floor = IsOnFloor();
			MoveAndSlide();
			Velocity = velocity; //Don't need the sliding
			if (IsOnFloor() && !was_on_floor)
			{
				state |= State.LANDED;
				
			}

			if (!IsOnFloor())
			{
				state = State.FALLING;
			}
		}

		Update();
	}

	private void Update()
	{
		AnimatedSprite2D anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");

		if (StateHas(State.LANDED))
		{
			if (!anim.IsPlaying())
			{
				state &= ~State.LANDED;
			}
			anim.Play("land");
		}
		else if (StateHas(State.EXPLOSION))
		{
			CollisionShape2D collision_shape = (CollisionShape2D)GetNode("CollisionShape2D");
			collision_shape.Disabled = true;
			if (!anim.IsPlaying())
			{
				OnDeath();
			}
			anim.Play("explosion");
		}
		else if (StateHas(State.FALLING))
		{
			anim.Play("fall");
		}
		else if (StateHas(State.EXPLODE))
		{
			if (!anim.IsPlaying())
			{
				state &= ~State.EXPLODE;
				state |= State.EXPLOSION;

				CollisionShape2D collision_shape = (CollisionShape2D)GetNode("CollisionShape2D");
				collision_shape.Disabled = true;
			}
			anim.Play("explode");
		}
		else
		{
			anim.Play("idle");
		}
	}

	public void OnDeath()
	{
		RandomNumberGenerator gen = new RandomNumberGenerator();

		PackedScene scene = GD.Load<PackedScene>("res://eggsploder.tscn");

		Eggsploder successor = (Eggsploder)scene.Instantiate();
		successor.Position = new Vector2(gen.RandiRange(0, 180), 100);

		GetParent().AddChild(successor);
		QueueFree();
	}

	public bool StateHas(State a)
	{
		int state_int = (int)state;

		if ((state_int & (int)a) != 0)
		{
			return true;
		}
		return false;
	}
}
