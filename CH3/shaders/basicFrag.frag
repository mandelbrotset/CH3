uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;


void main()
{
	float step = 0.6;
	float width = 1280;
	float height = 720;
	vec2 uv = gl_TexCoord[0].xy;


	vec4 texColor = texture2D(tex,gl_TexCoord[0].xy);
 	gl_FragColor = texColor;


}