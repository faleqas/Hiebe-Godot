using Godot;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;


public partial class player : CharacterBody2D
{
	public enum State
	{
		IDLE = 0,
		RUNNING = 2,
		JUMPING = 4,
		LANDED = 64,
		FALLING = 8,
		SHOOTING = 16,
		DEATH = 32 //Allah Yr7amoh
	};

	private State state = State.IDLE;
	private const float Speed = 70.0f;
	private const float JumpVelocity = -120.0f;

	private const float acceleration = 7.0f;
	private const float friction = 11.0f;

	private int tics_since_fall = 0; //used for coyote jumping
	private int last_jump_press_tic = -1;

	private int health = 100;

		private const int LANDED_COUNTDOWN_TICS = 10;

	private int landed_countdown = LANDED_COUNTDOWN_TICS; //landed squash lasts for landed_countdown tics

	
	private float gravity = 0;
	public Vector2 direction = Vector2.Right;

	private Vector2 velocity = Vector2.Zero;

	private CpuParticles2D particles = null;

	Scene parent = null;

	bool on_fire_tile = false;

	Vector2 spawn_pos = Vector2.Zero;

	private int damage_countdown = 0; //if countdown reaches 0 the player's modulate returns to white.
	//while damage_countdown is not 0 the player cannot be damaged

	public override void _Ready()
	{
		base._Ready();
		parent = (Scene)GetParent();
		gravity = ((Scene)GetParent()).Gravity;
		((Scene)(GetParent())).Player = this;
		particles = (CpuParticles2D)GetNode("run_particles");
		spawn_pos = Position;
	}

	public Scene.ObjectTypes ObjectType()
	{
		return Scene.ObjectTypes.PLAYER;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (StateHas(State.DEATH))
		{

		}
		else
		{
			velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				particles.Emitting = false;
				velocity.Y += gravity * (float)delta;
				if (StateHas(State.SHOOTING))
				{
					velocity.Y = 0;
				}
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("jump"))
			{
				last_jump_press_tic = 0;
				if (IsOnFloor() || (tics_since_fall < 10 && tics_since_fall != 0))
				{
					velocity.Y = JumpVelocity;
				}
			}
			else if (last_jump_press_tic < 10 && last_jump_press_tic > 0)
			{
				if (IsOnFloor() || (tics_since_fall < 10 && tics_since_fall != 0))
				{
					velocity.Y = JumpVelocity;
					GD.Print("coyote");
				}
			}

			if (velocity.Y > 0)
			{
				state |= State.FALLING;
				state &= ~State.JUMPING;
			}
			else if (velocity.Y < 0)
			{
				state |= State.JUMPING;
				state &= ~State.FALLING;
			}
			else
			{
				state &= ~State.FALLING;
				state &= ~State.JUMPING;
			}

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			
			if (direction.X == 1)
			{
				particles.Position = new Vector2(-6, 9);
			}
			else
			{
				particles.Position = new Vector2(6, 9);
			}
			Vector2 move_direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			if ((move_direction != Vector2.Zero) && (!StateHas(State.SHOOTING)))
			{
				velocity.X += direction.X * acceleration;
				
				if (IsOnFloor())
				{
					particles.Emitting = true;
				}
				if (velocity.X > Speed)
				{
					velocity.X = Speed;
					particles.Emitting = false;
				}
				else if (velocity.X < -Speed)
				{
					velocity.X = -Speed;
					particles.Emitting = false;
				}
				if (on_fire_tile)
				{
					velocity.X *= 0.8f;
				}
				direction = move_direction;
				state |= State.RUNNING;
			}
			else
			{
				particles.Emitting = false;
				if (velocity.X < 0)
				{
					velocity.X += friction;
					if (velocity.X > 0)
					{
						velocity.X = 0;
					}
				}
				if (velocity.X > 0)
				{
					velocity.X -= friction;
					if (velocity.X < 0)
					{
						velocity.X = 0;
					}
				}
				state &= ~State.RUNNING;
			}

			if (Input.IsActionJustPressed("attack") && !StateHas(State.FALLING))
			{
				state |= State.SHOOTING;
			}
			if (Input.IsActionJustPressed("die"))
			{
				health = 0;
				OnDeath();
			}

			for (int i = 0; i < GetSlideCollisionCount(); i++)
			{
				KinematicCollision2D col = GetSlideCollision(i);
				if (col.GetCollider() != null)
				{
					string col_type = col.GetCollider().GetType().ToString();
					if (col_type == "stoner" || col_type == "stoner_bullet"
					|| col_type == "thug")
					{
						velocity = new Vector2(0, JumpVelocity);
						OnAttacked(15);
						particles.Emitting = false;
					}
				}
			}

			Velocity = velocity;
			bool was_on_floor = IsOnFloor();
			MoveAndSlide();
			if (IsOnFloor() && !was_on_floor)
			{
				state |= State.LANDED;
			}
			else if (!IsOnFloor() && was_on_floor && velocity.Y >= 0)
			{
				tics_since_fall++;
			}
			else if (IsOnFloor())
			{
				tics_since_fall = 0;
			}
			else if (!IsOnFloor() && tics_since_fall != 0)
			{
				tics_since_fall++;
			}
			//GD.Print(tics_since_fall);

			if (last_jump_press_tic >= 0)
			{
				last_jump_press_tic++;
			}
		}

		TileMap tilemap = (TileMap)parent.GetNode("TileMap");
		if (tilemap != null)
		{
			Vector2I cell_pos = tilemap.LocalToMap(Position);
			cell_pos.Y += 1; //tile right below player

			Vector2I atlas_coords = tilemap.GetCellAtlasCoords(0, cell_pos);
			if (atlas_coords == new Vector2I(1, 2)) //use ENUMs later
			{
				OnAttacked(5);
				on_fire_tile = true;
			}
			else
			{
				on_fire_tile = false;
			}
		}
		Update();
	}

	private void Update()
	{
		AnimatedSprite2D anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		switch (direction.X)
		{
			case 1:
			{
				anim.FlipH = false;
			} break;

			case -1:
			{
				anim.FlipH = true;
			} break;
		}


		if (StateHas(State.SHOOTING))
		{
			if (!anim.IsPlaying())
			{
				state &= ~State.SHOOTING;
				ShootLaser();
			}
			anim.Play("shoot");
		}
		else if (StateHas(State.JUMPING))
		{
			anim.Play("jump");
		}
		else if (StateHas(State.FALLING))
		{
			anim.Scale = anim.Scale.Lerp(new Vector2(0.85f, 1.15f), 0.1f);
			anim.Play("fall");
		}
		else if (StateHas(State.RUNNING))
		{
			anim.Play("run");
		}
		else if (StateHas(State.DEATH))
		{
			if (!anim.IsPlaying())
			{
				RandomNumberGenerator gen = new RandomNumberGenerator();

				PackedScene scene = GD.Load<PackedScene>("res://player.tscn");

				player successor = (player)scene.Instantiate();
				successor.Position = spawn_pos;

				GetParent().AddChild(successor);
				QueueFree();
			}
			if (anim.Frame == 0)
			{
				anim.Modulate = new Godot.Color(1.0f, 0, 0);
			}
			else if (anim.Frame < 4)
			{
				anim.Modulate = new Godot.Color(1, 1, 1);
			}
			if (anim.Frame > 4)
			{
				anim.Offset = new Vector2(0, 5);
				anim.Modulate = anim.Modulate.Lerp(new Godot.Color(0, 0, 0), 0.06f);
			}
			anim.Play("death");
		}
		else
		{
			anim.Play("idle");
		}

		if (StateHas(State.LANDED))
		{
			anim.Scale = anim.Scale.Lerp(new Vector2(1.17f, 0.85f), 0.4f);
			landed_countdown--;
			if (landed_countdown <= 0)
			{
				state &= ~State.LANDED;
				landed_countdown = LANDED_COUNTDOWN_TICS;
			}
		}

		if (!StateHas(State.FALLING) && !StateHas(State.LANDED))
		{
			anim.Scale = anim.Scale.Lerp(new Vector2(1, 1), 0.2f);
		}

		if (damage_countdown > 0)
		{
			anim.Modulate = new Godot.Color(1, 0, 0, 1);
			damage_countdown--;
		}
		else if (damage_countdown == 0)
		{
			if (!StateHas(State.DEATH))
			{
				anim.Modulate = new Godot.Color(1, 1, 1, 1);
			}
		}
	}

	private void ShootLaser()
	{
		CollisionShape2D col = (CollisionShape2D)GetNode("CollisionShape2D");
		RectangleShape2D shape = (RectangleShape2D)col.Shape;
		Vector2 size = shape.Size;

		PackedScene bullet_scene = GD.Load<PackedScene>("res://player_laser.tscn");
		player_laser bullet = (player_laser)bullet_scene.Instantiate();

		Vector2 offset = new Vector2(0, 0);

		offset.X = (size.X) * direction.X;
		offset.Y = -6; //to reach eye level

		GD.Print(size);

		bullet.Set("global_position", GlobalPosition + offset);

		bullet.direction = direction;

		GetParent().AddChild(bullet);
	}

	public void OnDeath()
	{
		state = State.DEATH;
		Velocity = Vector2.Zero; //so enemy doesn't slide

		CollisionShape2D collision_shape = (CollisionShape2D)GetNode("CollisionShape2D");
		collision_shape.Disabled = true;
	}

	public void OnAttacked(int damage)
	{
		if (damage_countdown == 0)
		{
			health -= damage;
			if (health <= 0)
			{
				OnDeath();
			}
			damage_countdown = 40;
		}
	}

	public int GetHealth()
	{
		return health;
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
