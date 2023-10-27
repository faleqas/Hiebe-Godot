using Godot;
using System;

public partial class player_data : Node
{
	// Called when the node enters the scene tree for the first time.
	public bool has_gem = false;
	public int health = 75;
	public Vector2 position = Vector2.Zero;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
