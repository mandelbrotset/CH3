uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;


void main()
{
	float intensity;
	vec4 texColor = texture2D(tex,gl_TexCoord[0].xy);
	vec4 color;
	vec3 dir = vec3(0, 0.1, -1);
	intensity = -dot(light_direction, gl_TexCoord[1].xyz);

	if (intensity > 0.95)
		color = texColor;
	else if (intensity > 0.5)
		color = 0.6*texColor;
	else if (intensity > 0.25)
		color = 0.4*texColor;
	else 
		color = 0.2 * texColor;

	gl_FragColor = color;//vec4(sin(time*0.001)+0.7, 0.7, 1.0, 1.0);

}