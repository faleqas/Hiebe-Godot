using Godot;
using System;
using System.Drawing;


public partial class stoner : CharacterBody2D
{
	public enum State
	{
		IDLE = 8,
		RUN = 16,
		SHOOT = 32
	};
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = 0;

	private State state = State.RUN;

	private int direction = 1;

	private const float speed = 50.0f;

	private Vector2 original_pos = Vector2.Zero;

	public override void _Ready()
	{
		base._Ready();
		gravity = ((Scene)GetParent()).Gravity;
		original_pos = Position;
	}

	public Scene.ObjectTypes ObjectType()
	{
		return Scene.ObjectTypes.STONER;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (IsOnFloor())
		{
			KinematicCollision2D col = GetLastSlideCollision();
			if (col != null)
			{
				Vector2 normal = col.GetNormal();
				if (normal.X != 0)
				{
					direction *= -1;
				}
			}
			if (StateHas(State.SHOOT))
			{
				velocity.X = 0;
			}
			else if (StateHas(State.RUN))
			{
				velocity.X = speed * direction;
			}
			velocity.Y = 0;
		}
		else
		{
			velocity.Y += gravity * (float)delta;
		}

		Scene parent = (Scene)GetParent();
		{
			Vector2 target_pos = parent.Player.Position;
			Vector2 distance = target_pos - Position;
			distance = distance.Abs();
			if (distance.Length() < 100)
			{
				AnimatedSprite2D anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
				if (anim.Frame > 4)
				{
					anim.Frame = 4;
				}
				state |= State.SHOOT;
				if (target_pos.X > Position.X)
				{
					direction = 1;
				}
				else if (target_pos.X < Position.X)
				{
					direction = -1;
				}
				if ((parent.Gametic() % 45) == 0)
				{
					if (anim.Frame == 4)
					{
						Shoot();
					}
				}
			}
		}

		Velocity = velocity;

		MoveAndSlide();

		Update();

	}

	private void Update()
	{
		AnimatedSprite2D anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		anim.FlipH = (direction == -1);

		if (StateHas(State.SHOOT))
		{
			if (!anim.IsPlaying())
			{
				state &= ~State.SHOOT;
			}
			anim.Play("shoot");
		}
		else if (StateHas(State.RUN))
		{
			anim.Play("run");
		}
	}

	public void OnDeath()
	{
		PackedScene scene = GD.Load<PackedScene>("res://stoner.tscn");

		stoner successor = (stoner)scene.Instantiate();
		successor.Position = original_pos;

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

	private void Shoot()
	{
		PackedScene bullet_scene = GD.Load<PackedScene>("res://stoner_bullet.tscn");
		stoner_bullet bullet = (stoner_bullet)bullet_scene.Instantiate();

		Vector2 offset = new Vector2(0, 0);

		AnimatedSprite2D anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		Vector2 size = anim.SpriteFrames.GetFrameTexture("shoot", anim.Frame).GetSize();

		offset.X = (size.X) * direction;
		offset.Y += 3;

		GD.Print(size);

		bullet.Set("global_position", GlobalPosition + offset);

		bullet.direction.X = direction;

		GetParent().AddChild(bullet);
	}
}
