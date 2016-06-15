uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;
uniform float model_id;

void main()
{

	vec4 color = vec4(model_id, model_id+0.1, model_id+0.1, 1.0);
	gl_FragColor = color;

}