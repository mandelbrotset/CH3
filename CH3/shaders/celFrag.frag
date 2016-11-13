#version 130

uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;

uniform mat4 view_matrix;

float round(float f) {
	//float i = (floor(f*3)/4)+0.2;
	return clamp(f,0.0,1.0);
	if (f > 0.8)
		return 1.0;
	else if (f > 0.8)
		return 0.9;
	else if (f > 0.7)
		return 0.8;
	else if (f > 0.6)
		return 0.7;
	else if (f > 0.5)
		return 0.6;
	else if (f > 0.4)
		return 0.5;
	else if (f > 0.3)
		return 0.4;
	else if (f > 0.2)
		return 0.3;
	else if (f > 0.1)
		return 0.2;
	else
		return 0.1;
}

vec3 cartoonify(vec3 color) {
	vec3 rounded = vec3(1.0);
	float max = 1.0;
	float min = 0.3;
	if (color.x > color.y && color.x > color.z) {
		return vec3(clamp(color.x, min, max), 0.3, 0.1);
	}
	if (color.y > color.x && color.y > color.z) {
		return vec3(0, clamp(color.x, min+0.1, max), 0);
	}
	if (color.z > color.x && color.z > color.y) {
		return vec3(0.3, 0.4, clamp(color.z, min, max));
	}
	rounded.x = round(color.x);
	rounded.y = round(color.y);
	rounded.z = round(color.z);
	return rounded;
}


void main()
{
	
	float intensity = 1.0;
	
//vec3 texColor = texture2D(tex , gl_TexCoord[0].xy).xyz;
	vec3 texColor = texture2D(tex , gl_TexCoord[0].xy).xyz;
	
	//texColor = cartoonify(texColor);
	
	vec3 color = vec3(1.0) ;

	intensity = -dot( normalize((view_matrix * vec4(light_direction,0)).xyz), gl_TexCoord[1].xyz) ;
	
	color = texColor * clamp(intensity, 0.25, 1.0);
	/*
	if (intensity > 0.95)
		color = texColor;
	else if (intensity > 0.5)
		color = 0.8*texColor;
	else if (intensity > 0.25)
		color = 0.5*texColor;
	else 
		color = 0.3 * texColor;
		*/

		
	gl_FragColor = vec4(color, 1.0);

}