uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;


float intensity(in vec4 color){
	return sqrt((color.x*color.x)+(color.y*color.y)+(color.z*color.z));
}

float hue(in vec4 color){
	float hue;
	float red = color.r;
	float green = color.g;
	float blue = color.b;

	if(red > green && red > blue){
		float max = red; 
		float min = min(blue, green);

		hue = (green - blue) / (max - min);

	} else if(green > red && green > blue){
		float max = green; 
		float min = min(blue, red);

		hue = 2.0 + (blue - red) / (max - min);
	}else{
		float max = blue;
		float min = min(green, red);

		hue = 4.0 + (red - green) / (max - min);
	}

	hue *= 60;
	if(hue < 0)
		hue += 360;

	return hue;

}

vec4 edgeDetect2(float stepx, float stepy, vec2 center, vec4 color)
{
	float sense = 60;
	float i = hue(color);

	float tleft = hue(texture2D(tex,center + vec2(-stepx,stepy)));

	if(abs(tleft - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float left = hue(texture2D(tex,center + vec2(-stepx,0)));
	if(abs(left - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float bleft = hue(texture2D(tex,center + vec2(-stepx, -stepy)));
	if(abs(bleft - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float top = hue(texture2D(tex,center + vec2(0, stepy)));
	if(abs(top - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float bottom = hue(texture2D(tex,center + vec2(0, -stepy)));
	if(abs(bottom - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float tright = hue(texture2D(tex,center + vec2(stepx,stepy)));
	if(abs(tright - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float right = hue(texture2D(tex,center + vec2(stepx,0)));
	if(abs(right - i) > sense)
		return vec4(vec3(0.0), 1.0);

	float bright = hue(texture2D(tex,center + vec2(stepx, -stepy)));
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
	vec4 tleft = texture2D(tex,center + vec2(-stepx,stepy));
	if(tleft != color)
		return vec4(vec3(0.0), 1.0);

		vec4 left = texture2D(tex,center + vec2(-stepx,0));
	if(left != color)
			return vec4(vec3(0.0), 1.0);
	
	vec4 bleft = texture2D(tex,center + vec2(-stepx, -stepy));
	if(bleft != color)
		return vec4(vec3(0.0), 1.0);
	
	vec4 top = texture2D(tex,center + vec2(0, stepy));
	if(top != color)
			return vec4(vec3(0.0), 1.0);
	
	vec4 bottom = texture2D(tex,center + vec2(0, -stepy));
	if(bottom != color)		
		return vec4(vec3(0.0), 1.0);

	vec4 tright = texture2D(tex,center + vec2(stepx,stepy));
	if(tright != color)
		return vec4(vec3(0.0), 1.0);

	vec4 right = texture2D(tex,center + vec2(stepx,0));
	if(right != color)
		return vec4(vec3(0.0), 1.0);

	vec4 bright = texture2D(tex,center + vec2(stepx, -stepy));
	if(bright != color)
		return vec4(vec3(0.0), 1.0);


	return vec4(0.0);
}


void main()
{
	float step = 2.0;
	float width = 1280;
	float height = 720;
	vec2 uv = gl_TexCoord[0].xy;


	vec4 texColor = texture(tex,gl_TexCoord[0].xy);

	vec4 color;
 	color = edgeDetect(step/width, step/height, uv, texColor);

 	gl_FragColor = color;


}