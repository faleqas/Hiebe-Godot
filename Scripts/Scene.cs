using Godot;
using System;

public partial class Scene : Node2D
{
	// Called when the node enters the scene tree for the first time.

	public enum ObjectTypes
	{
		NONE,
		PLAYER,
		PLAYER_LASER,
		EGGPSLODER,
		STONER,
		STONER_BULLET,
		THUG
	};

	public enum Scenes
	{
		INTRO_FOREST,
		CAVE_FIRST
	};

	public float Gravity = 200.0f;
	private int tic = 0;
	public player Player = null;

	private Scenes scene_id = Scenes.INTRO_FOREST;

	private bool switching_scenes = false;

	private int switch_countdown = 60;

	private PackedScene next_scene = null;

	player_data PlayerData = null;

	public Scenes get_scene_id()
	{
		return scene_id;
	}
	public override void _Ready()
	{
		PlayerData = (player_data)GetNode("/root/PlayerData");
		if (SceneFilePath == "res://Cave.tscn")
		{
			scene_id = Scenes.CAVE_FIRST;
		}
		else if (SceneFilePath == "res://Scene.tscn")
		{
			scene_id = Scenes.INTRO_FOREST;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		tic++;
		UpdateCamera();
		CanvasModulate modulate = (CanvasModulate)(GetNode("scene_modulate"));
		if (switching_scenes)
		{
			if (modulate != null)
			{
				modulate.Color = modulate.Color.Lerp(new Color(0, 0, 0), 0.3f);
				GD.Print(modulate.Color);
			}
			
			if (switch_countdown <= 0)
			{
				PlayerData.position = Vector2.Zero;
				GetTree().ChangeSceneToPacked(next_scene);	
			}
			switch_countdown--;
		}
		else
		{
			if (modulate != null)
			{
				modulate.Color = modulate.Color.Lightened(0.3f);
			}
		}
	}

	private void UpdateCamera()
	{
		if (scene_id == Scenes.CAVE_FIRST)
		{
			return;
		}
		Camera2D camera = (Camera2D)GetNode("Camera2D");
		if (camera == null)
		{
			return;
		}
		if (Player == null)
		{
			return;
		}

		camera.Position = new Vector2(Player.Position.X, camera.Position.Y);
		
		Sprite2D bg = (Sprite2D)GetNode("Background");
		if (bg == null)
		{
			return;
		}
		bg.Position = camera.GetTargetPosition();
	}

	public int Gametic()
	{
		return tic;
	}

	public void SetObjective(string str)
	{
		CanvasLayer layer = (CanvasLayer)GetNode("UILayer");
		if (layer != null)
		{
			Control text = (Control)layer.GetNode("objective");
			
			if (text != null)
			{
				GD.Print("HAHHA");
				text.Set("text", str);
			}
		}
	}

	public void SwitchSceneTo(String scene)
	{
		PackedScene packed = GD.Load<PackedScene>(scene);
		if (packed != null)
		{
			switching_scenes = true;
			next_scene = packed;
		}
	}
}
