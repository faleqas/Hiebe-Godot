shader_type canvas_item;

void fragment()
{
	vec2 shift = UV;

	shift.x += TIME * 0.4;

	vec4 color = texture(TEXTURE, shift);
	COLOR = color;
}
