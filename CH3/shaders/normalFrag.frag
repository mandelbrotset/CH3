uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;

void main()
{
	vec4 texColor2 = texture2D(tex,gl_TexCoord[0].xy);

	vec4 texColor = vec4(0.5*(gl_TexCoord[1].xyz + 1), 1.0);

	gl_FragColor = texColor;

}