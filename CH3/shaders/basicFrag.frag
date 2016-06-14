uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;


float intensity(in vec4 color){
	return sqrt((color.x*color.x)+(color.y*color.y)+(color.z*color.z));
}


vec4 edgeDetect2(float stepx, float stepy, vec2 center, vec4 color)
{
	float sense = 0.0001;
	float i = intensity(color);

	float tleft = intensity(texture2D(tex,center + vec2(-stepx,stepy)));

	if(abs(tleft - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float left = intensity(texture2D(tex,center + vec2(-stepx,0)));
	if(abs(left - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float bleft = intensity(texture2D(tex,center + vec2(-stepx, -stepy)));
	if(abs(bleft - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float top = intensity(texture2D(tex,center + vec2(0, stepy)));
	if(abs(top - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float bottom = intensity(texture2D(tex,center + vec2(0, -stepy)));
	if(abs(bottom - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float tright = intensity(texture2D(tex,center + vec2(stepx,stepy)));
	if(abs(tright - i) > sense)
		return vec4(vec3(0.0), 1.0);
	float right = intensity(texture2D(tex,center + vec2(stepx,0)));
	if(abs(right - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float bright = intensity(texture2D(tex,center + vec2(stepx, -stepy)));
	if(abs(bright - i) > sense)
		return vec4(vec3(0.0), 1.0);

	return vec4(0.0);

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



vec4 edgeDetect(float stepx, float stepy, vec2 center, vec4 color)
{
	float diff = 0;
	float sense = 0.14;

	vec4 tleft = texture2D(tex,center + vec2(-stepx,stepy));
	diff = abs(tleft - color);
	if(diff > sense || (diff == 0.0 && tleft != color))
		return vec4(vec3(0.0), 1.0);
	
	vec4 left = texture2D(tex,center + vec2(-stepx,0));
	diff = abs(left - color);
	if(diff > sense || (diff == 0.0 && left != color))
			return vec4(vec3(0.0), 1.0);
	
	vec4 bleft = texture2D(tex,center + vec2(-stepx, -stepy));
	diff = abs(bleft - color);
	if(diff > sense || (diff == 0.0 && bleft != color))
		return vec4(vec3(0.0), 1.0);
	
	vec4 top = texture2D(tex,center + vec2(0, stepy));
	diff = abs(top - color);
	if(diff > sense || (diff == 0.0 && top != color))
			return vec4(vec3(0.0), 1.0);
	
	vec4 bottom = texture2D(tex,center + vec2(0, -stepy));
	diff = abs(bottom - color);
	if(diff > sense || (diff == 0.0 && bottom != color))		
		return vec4(vec3(0.0), 1.0);

	vec4 tright = texture2D(tex,center + vec2(stepx,stepy));
		diff = abs(tright - color);
	if(diff > sense || (diff == 0.0 && tright != color))
		return vec4(vec3(0.0), 1.0);

	vec4 right = texture2D(tex,center + vec2(stepx,0));
	diff = abs(right - color);
	if(diff > sense || (diff == 0.0 && right != color))
		return vec4(vec3(0.0), 1.0);

	vec4 bright = texture2D(tex,center + vec2(stepx, -stepy));
	diff = abs(bright - color);
	if(diff > sense || (diff == 0.0 && bright != color))
		return vec4(vec3(0.0), 1.0);

	return vec4(0.0);
}


void main()
{
	float step = 2;
	float width = 1280;
	float height = 720;
	vec2 uv = gl_TexCoord[0].xy;


	vec4 texColor = texture2D(tex,gl_TexCoord[0].xy);
 	gl_FragColor = edgeDetect(step/width, step/height, uv, texColor);


}