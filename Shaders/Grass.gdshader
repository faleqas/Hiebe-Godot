shader_type canvas_item;
uniform vec2 player_pos;

uniform vec2 pos;
uniform vec2 size;

uniform vec2 player_size;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
}

void vertex()
{
	float dist = distance(pos, player_pos);
	if (dist < size.x * 0.9f)
	{
		vec2 player_center = player_pos + (player_size / 2.0f);
		vec2 vertex_pos = pos + (VERTEX);
		vec2 offset = vec2(0.0f, 0.0f);

		vec2 center_dist = player_center - vertex_pos;
		offset.x = -(center_dist.x / size.x);
		float dir = offset.x;
		if (dir < 0.0f)
		{
			offset.x = -2.0f * abs(offset.x + 1.0f);
		}
		else
		{
			offset.x = 2.0f * abs(offset.x - 1.0f);
		}

		offset.x *= abs(UV.y - 1.0f);
		VERTEX += offset;
	}
	else
	{
		
		{
			float offset_x = cos(TIME) * 1.5f;
			VERTEX += vec2(offset_x * abs(UV.y - 1.0f), 0);
		}
	}
}