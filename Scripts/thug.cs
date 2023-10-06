using Godot;
using System;
using System.Drawing;


public partial class thug : CharacterBody2D
{
	public enum State
	{
		IDLE = 8,
		RUN = 16,
		ATTACK = 32
	};
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = 0;

	private State state = State.RUN | State.ATTACK;

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
		return Scene.ObjectTypes.THUG;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (IsOnFloor() && !IsOnCeiling())
		{
			KinematicCollision2D col = GetLastSlideCollision();
			if (col != null)
			{
				if (col.GetCollider() != null)
				{
					string name = col.GetCollider().GetClass();
					if ((name != "player"))
					{
						Vector2 normal = col.GetNormal();
						if (normal.X != 0)
						{
							direction = (int)normal.X;
						}
					}
				}
			}

			Scene parent = (Scene)GetParent();
			{
				Vector2 target_pos = parent.Player.Position;
				Vector2 distance = target_pos - Position;
				distance = distance.Abs();
				if (distance.Y > 5)
				{
					state &= ~State.ATTACK;
				}
				else
				{
				if (distance.Length() < 80)
				{
					if (distance.Length() > 20)
					{
						state &= ~State.ATTACK;
					}
					else
					{
						state |= State.ATTACK;
						if (target_pos.X > Position.X)
						{
							if (direction != 1)
							{
								state &= ~State.ATTACK; //restart the attack
							}
					
						}
						else if (target_pos.X < Position.X)
						{
							if (direction != -1)
							{
								state &= ~State.ATTACK; //restart the attack
							}
						}
					}
					if (target_pos.X > Position.X)
					{
						direction = 1;
					}
					else if (target_pos.X < Position.X)
					{
						direction = -1;
					}
				}
				if (distance.Length() < 20)
				{
				}
				
				else
				{
					state &= ~State.ATTACK;
				}
				}
			}
		}
		else
		{
			velocity.Y += gravity * (float)delta;
		}
		if (StateHas(State.ATTACK))
		{
			velocity.X = 0;
		}
		else
		{
			velocity.X = speed * direction;
		}

		Velocity = velocity;

		MoveAndSlide();

		Update();

	}

	private void Update()
	{
		AnimatedSprite2D anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		RayCast2D sword = (RayCast2D)GetNode("sword_ray");
		anim.FlipH = (direction == -1);
		if (direction == 1)
		{
			sword.RotationDegrees = 0;
		}
		else
		{
			sword.RotationDegrees = -180;
		}

		if (StateHas(State.ATTACK))
		{
			anim.Offset = new Vector2(0, -2.5f);
			if (anim.Frame == 3)
			{
				SwordAttack();
			}
			anim.Play("attack");
		}
		else if (StateHas(State.RUN))
		{
			anim.Offset = new Vector2(0, 0);
			anim.Play("run");
		}
	}

	private void SwordAttack()
	{
		RayCast2D sword = (RayCast2D)GetNode("sword_ray");
		GodotObject collider = sword.GetCollider();
		if (collider != null)
		{
			if (collider.HasMethod("ObjectType"))
			{
				Variant typev = collider.Callv("ObjectType", null);
				int type = typev.AsInt32();
				if (type == (int)Scene.ObjectTypes.PLAYER)
				{
					{
						collider.Call("OnAttacked", 30);
					}
				}
			}
		}
	}

	public void OnDeath()
	{
		// PackedScene scene = GD.Load<PackedScene>("res://thug.tscn");

		// thug successor = (thug)scene.Instantiate();
		// successor.Position = original_pos;

		// GetParent().AddChild(successor);

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
