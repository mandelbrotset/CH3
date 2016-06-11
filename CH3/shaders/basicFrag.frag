uniform float time;
uniform sampler2D tex;

void main()
{

	vec4 color = texture2D(tex,gl_TexCoord[0].xy);

	gl_FragColor = color;//vec4(sin(time*0.001)+0.7, 0.7, 1.0, 1.0);

}