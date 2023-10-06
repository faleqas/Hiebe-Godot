using Godot;
using System;

public partial class map_border : Area2D
{
	// Called when the node enters the scene tree for the first time.
	Control rect = null;

	bool show = false;
	float a = 0;
	public override void _Ready()
	{
		rect = (Control)GetNode("ColorRect");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (rect != null)
		{
			rect.Visible = true;
			rect.Modulate = new Color(1, 0, 0, a);
			if (show)
			{
				if (a < 0.7f)
				{
					a += 0.1f;
				}
			}
			else
			{
				if (a > 0)
				{
					a -= 0.1f;
				}
			}
		}
	}


	private void _on_body_entered(Node2D body)
	{
		if (body != GetParent())
		{
			{
				show = true;
				GD.Print("ldaqidwjm");
			}
		}
	}

	private void _on_body_exited(Node2D body)
	{
		show = false;
	}

}
