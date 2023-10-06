using Godot;
using System;

public partial class cave_entrance : Node2D
{

	Scene scene;
	Area2D area;

	bool switching_scenes = false;
	int switch_countdown = 100;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scene = (Scene)GetParent();
		area = (Area2D)GetNode("Area2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (scene.Player != null)
		{
			Godot.Collections.Array<Node2D> bodies = area.GetOverlappingBodies();

			if (bodies.Contains(scene.Player))
			{
				if (Input.IsActionJustPressed("enter"))
				{
					Scene scene = (Scene)GetParent();
					if (scene != null)
					{
						if (scene.get_scene_id() == Scene.Scenes.INTRO_FOREST)
						{
							scene.SwitchSceneTo("res://Cave.tscn");
						}
						else
						{
							scene.SwitchSceneTo("res://Scene.tscn");
						}
					}
				}
			}
		}
	}
}
