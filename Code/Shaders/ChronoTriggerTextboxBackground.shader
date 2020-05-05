shader_type canvas_item;

//This shader is used to control the opening and closing animation of the textbox, as well as properly shading the textbox (the texture has no shading)

uniform float ysquish = 0f; //amount of squish, with 0 being completely squished and 1 being completely open
uniform float size_y; //the value of size.y on the rect for the ninepatchrect
varying vec2 nine_uv; //this is basically the uv without the ninepatchrect processing going on with it, used to correctly apply the colouring

void vertex() {
	nine_uv = UV; //store the UV in the fragment shader (presumably this is before the ninepatchrect processing)
	//this block uses the s
	VERTEX.y -= size_y / 2f;
	VERTEX.y = VERTEX.y * ysquish;
	VERTEX.y += size_y / 2f;
}

void fragment()
{
	//this block handles drawing the colour and shading the textbox
	COLOR = texture(TEXTURE, vec2(UV.x, UV.y));
	COLOR.r *= 1f - nine_uv.y * 2f + 1f;
	COLOR.g *= 1f - nine_uv.y * 2f + 1f;
	COLOR.b *= 1f - nine_uv.y * 2f + 1f;
}