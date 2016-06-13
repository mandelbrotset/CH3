uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;


float intensity(in vec4 color){
	return sqrt((color.x*color.x)+(color.y*color.y)+(color.z*color.z));
}

vec3 sobel(float stepx, float stepy, vec2 center)
{

	
	float tleft = intensity(texture2D(tex,center + vec2(-stepx,stepy)));
	float left = intensity(texture2D(tex,center + vec2(-stepx,0)));
	float bleft = intensity(texture2D(tex,center + vec2(-stepx, -stepy)));

	float top = intensity(texture2D(tex,center + vec2(0, stepy)));
	float bottom = intensity(texture2D(tex,center + vec2(0, -stepy)));

	float tright = intensity(texture2D(tex,center + vec2(stepx,stepy)));
	float right = intensity(texture2D(tex,center + vec2(stepx,0)));
	float bright = intensity(texture2D(tex,center + vec2(stepx, -stepy)));

    float x = tleft + 2.0*left + bleft - tright - 2.0*right - bright;
    float y = -tleft - 2.0*top - tright + bleft + 2.0 * bottom + bright;
    float color = sqrt((x*x) + (y*y));


 //    float color = 0;


    return vec3(color);
}


void main()
{
	float step = 1.7;
	float width = 1280;
	float height = 720;
	vec2 uv = gl_TexCoord[0].xy;


	vec4 texColor = texture2D(tex,gl_TexCoord[0].xy);
	texColor = vec4(1-sobel(step/width, step/height, uv), 1.0);
	if(texColor.x > 0.9999)
	 	gl_FragColor = vec4(0.0);
	 else
	 	gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0);
}