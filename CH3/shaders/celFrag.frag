#version 130

uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;


void main()
{
	
	float intensity = 1.0;
	

	vec3 texColor = texture2D(tex , gl_TexCoord[0].xy).xyz;
	vec3 color = vec3(1.0);

	intensity = -dot(light_direction , gl_TexCoord[1].xyz);

	if (intensity > 0.95)
		color = texColor;
	else if (intensity > 0.5)
		color = 0.8*texColor;
	else if (intensity > 0.25)
		color = 0.5*texColor;
	else 
		color = 0.3 * texColor;

	gl_FragColor = vec4(color, 1.0);

}