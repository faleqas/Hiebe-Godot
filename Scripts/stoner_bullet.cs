using Godot;
using System;

public partial class stoner_bullet : CharacterBody2D
{
	public const float Speed = 120.0f;


	public Vector2 direction = Vector2.Right;

	public override void _Ready()
	{
		base._Ready();
		AnimatedSprite2D anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		anim.FlipH = (direction.X == -1);
	}
	
	public Scene.ObjectTypes ObjectType()
	{
		return Scene.ObjectTypes.STONER_BULLET;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		velocity.X = direction.X * Speed;

		

		Velocity = velocity;

		if (MoveAndSlide())
		{
			KinematicCollision2D collision = GetLastSlideCollision();
			if (collision != null)
			{
				GodotObject collider = collision.GetCollider();
				if (collider != null)
				{
					if (collider.HasMethod("ObjectType"))
					{
						Variant typev = collider.Callv("ObjectType", null);
						int type = typev.AsInt32();
						if (type == (int)Scene.ObjectTypes.PLAYER)
						{
							collider.Call("OnAttacked", 30);
						}
					}
				}
			}
			QueueFree();
		}	

		AnimatedSprite2D anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		anim.FlipH = (direction.X == -1);
	}
}
