using Godot;
using System;

public partial class player_diamond : AnimatedSprite2D
{
	private int bob_dir = 1;

	private const float bob_speed = 0.1f;

	private Vector2 velocity = Vector2.Zero;

	private int tics_since_dir_change = 0;

	Scene scene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		velocity.Y = bob_speed;
		scene = (Scene)(GetParent().GetParent());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Visible = scene.PlayerData.has_gem;
		if (tics_since_dir_change >= 30)
		{
			tics_since_dir_change = 0;
			bob_dir *= -1;
		}

		if (bob_dir == 1)
		{
			if (velocity.Y < bob_speed)
			{
				velocity.Y += 0.001f;
			}
			if (velocity.Y >= bob_speed)
			{
				velocity.Y = bob_speed;
				tics_since_dir_change++;
			}
		}
		else
		{
			if (velocity.Y > -bob_speed)
			{
				velocity.Y -= 0.001f;
			}
			else if (velocity.Y <= -bob_speed)
			{
				velocity.Y = -bob_speed;
				tics_since_dir_change++;
			}
		}

		Position += velocity;
	}
}
