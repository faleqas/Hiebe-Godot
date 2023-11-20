using Godot;
using System;

public partial class healthbar : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private float Health = 100;

	Sprite2D bar = null;

	Vector2 original_bar_pos;

	Scene scene;
	public override void _Ready()
	{
		bar = (Sprite2D)GetNode("bar");
		original_bar_pos = bar.Position;

		scene = (Scene)(GetParent().GetParent()); //if node tree is modified modify this accordingly

		player Player = scene.Player;
		if (Player != null)
		{
			Health = Player.GetHealth();
			if (Health < 0)
			{
				Health = 0;
			}
		}
		else
		{
			Health = 0;
		}

		bar.Scale = new Vector2(1.0f, Health / 100);

		bar.Position = original_bar_pos + new Vector2(0, bar.Texture.GetHeight() * (1 - bar.Scale.Y));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bar.Scale = new Vector2(1.0f, Health / 100);

		bar.Position = original_bar_pos + new Vector2(0, bar.Texture.GetHeight() * (1 - bar.Scale.Y));

		player Player = scene.Player;
		if (Player != null)
		{
			Health = Player.GetHealth();
			GD.Print(Health);
			if (Health < 0)
			{
				Health = 0;
			}
		}
		else
		{
			Health = 0;
			GD.Print("player is null");
		}
	}
}
