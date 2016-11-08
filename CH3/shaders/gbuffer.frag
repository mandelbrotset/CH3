#version 130

precision highp float;

uniform sampler2D tex;
uniform vec3 light_direction;
uniform float time;

in vec3 normal;

void main() {
	float ambience = 0.1;
	float sun_intensity = 1.0;
	vec3 sun = vec3(1.0, 0.98, 0.9) * sun_intensity;
	vec3 color = texture2D(tex,gl_TexCoord[0].xy).rgb;

	gl_FragData[0] 	=  vec4(
	clamp(color * sun *
	clamp(-dot(light_direction , gl_TexCoord[1].xyz), 0.2, 1.0) + ambience * color,0.0,1.0), 1.0);


    gl_FragData[1]	=  vec4(0.5*(gl_TexCoord[1].xyz + 1), 1.0);
}