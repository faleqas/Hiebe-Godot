using Godot;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

public partial class diamond : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.

	private int bob_dir = 1;

	private const float bob_speed = 0.05f;

	private Vector2 velocity = Vector2.Zero;

	Scene scene;
	Area2D area;

	private int tics_since_dir_change = 0;
	public override void _Ready()
	{
		scene = (Scene)GetParent();
		area = (Area2D)GetNode("Area2D");
		velocity.Y = bob_speed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (scene.Player != null)
		{
			Godot.Collections.Array<Node2D> bodies = area.GetOverlappingBodies();

			if (bodies.Contains(scene.Player))
			{
				scene.SetObjective("Current Objective: Exit the cave");
				QueueFree();
			}
		}

		if (tics_since_dir_change >= 60)
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
