
uniform float time;

void main()
{
	gl_FragColor = vec4(sin(time*0.001)+0.7, 0.7, 1.0, 1.0);

}