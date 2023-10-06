using Godot;
using System;

public partial class Grass : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HandleGrassShader();
	}
	private void HandleGrassShader()
	{
		Scene parent = (Scene)(GetParent());
		player Player = parent.Player;
		if ((Player != null))
		{
			AnimatedSprite2D player_sprite = (AnimatedSprite2D)Player.GetNode("AnimatedSprite2D");
			Texture2D player_texture = null;

			if (player_sprite != null)
			{				
				{
					player_texture = player_sprite.SpriteFrames.GetFrameTexture("idle", player_sprite.Frame);
				}
			}

			ShaderMaterial material = (ShaderMaterial)Material;

			if (material != null)
			{
				material.SetShaderParameter("player_pos", Player.Position);
				
				material.SetShaderParameter("pos", Position);

				if (Texture != null)
				{
					material.SetShaderParameter("size", Texture.GetSize() * Scale);
				}

				if ((player_texture != null) && (player_sprite != null))
				{
					material.SetShaderParameter("player_size", player_texture.GetSize() * player_sprite.Scale);
				}
			}
		}
	}
}
