#version 130

uniform sampler2D tex;
uniform vec2 screenSize;
uniform vec3 light_direction;
out vec3 fragmentColor;

void main(void) {
	vec2 fragPos = gl_FragCoord.xy / screenSize;

	float FXAA_SPAN_MAX = 8.0;
	float FXAA_REDUCE_MUL = 1.0 / 8.0;
	float FXAA_REDUCE_MIN = 1.0 / 128.0;

	vec3 rgbNW = texture2D(tex, fragPos + (vec2(-1.0, -1.0) / screenSize)).xyz;
	vec3 rgbNE = texture2D(tex, fragPos + (vec2(1.0, -1.0) / screenSize)).xyz;
	vec3 rgbSW = texture2D(tex, fragPos + (vec2(-1.0, 1.0) / screenSize)).xyz;
	vec3 rgbSE = texture2D(tex, fragPos + (vec2(1.0, 1.0) / screenSize)).xyz;
	vec3 rgbM = texture2D(tex, fragPos).xyz;

	vec3 luma = vec3(0.299, 0.587, 0.114);
	float lumaNW = dot(rgbNW, luma);
	float lumaNE = dot(rgbNE, luma);
	float lumaSW = dot(rgbSW, luma);
	float lumaSE = dot(rgbSE, luma);
	float lumaM = dot(rgbM, luma);

	float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
	float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

	vec2 dir;
	dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
	dir.y = ((lumaNW + lumaSW) - (lumaNE + lumaSE));

	float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) * (0.25 * FXAA_REDUCE_MUL), FXAA_REDUCE_MIN);

	float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);

	dir = min(vec2(FXAA_SPAN_MAX, FXAA_SPAN_MAX), max(vec2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX), dir * rcpDirMin)) / screenSize;

	vec3 rgbA = (1.0 / 2.0) * (texture2D(tex, fragPos.xy + dir * (1.0 / 3.0 - 0.5)).xyz + texture2D(tex, fragPos.xy + dir * (2.0 / 3.0 - 0.5)).xyz);
	vec3 rgbB = rgbA * (1.0 / 2.0)
			+ (1.0 / 4.0) * (texture2D(tex, fragPos.xy + dir * (0.0 / 3.0 - 0.5)).xyz + texture2D(tex, fragPos.xy + dir * (3.0 / 3.0 - 0.5)).xyz);
	float lumaB = dot(rgbB, luma);

	if ((lumaB < lumaMin) || (lumaB > lumaMax)) {
		fragmentColor = rgbA;
	} else {
		fragmentColor = rgbB;
	}
}

/*uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;


void main()
{
	
	float FXAA_SPAN_MAX = 8.0;
	float FXAA_REDUCE_MIN = 1.0/128.0;
	float FXAA_REDUCE_MUL = 1.0/64.0;


	float width = 1.0/1280.0;
	float height = 1.0/720.0;

	vec3 R_inverseFilterTextureSize = vec3(width, height, 0);


	vec3 luma = vec3(0.299, 0.587, 0.114);
	vec2 uv = gl_TexCoord[0].xy;
	vec2 texCoordOffset = R_inverseFilterTextureSize.xy;

	float lumaTL = dot(luma, texture2D(tex, uv + (vec2(-1, -1) * texCoordOffset)).xyz);
	float lumaTR = dot(luma, texture2D(tex,  uv + (vec2(1,-1) * texCoordOffset)).xyz);
	float lumaBL = dot(luma, texture2D(tex,  uv + (vec2(-1,1) * texCoordOffset)).xyz);
	float lumaBR = dot(luma, texture2D(tex,  uv + (vec2(1,1) * texCoordOffset)).xyz);
	float lumaM = dot(luma, texture2D(tex, uv).xyz);

	vec2 blurDir;
	blurDir.x = -((lumaTL + lumaTR) - (lumaBL + lumaBR));
	blurDir.y = ((lumaTL + lumaBL) - (lumaTR + lumaBR));

	float dirReduce = max((lumaTL + lumaTR + lumaBL + lumaBR) * (FXAA_REDUCE_MUL * 0.25), FXAA_REDUCE_MIN);
	float inverseDirAdjustment = 1.0/(min(abs(blurDir.x), abs(blurDir.y)) + dirReduce);

	blurDir = min(vec2(FXAA_SPAN_MAX, FXAA_SPAN_MAX), max(vec2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX), blurDir * inverseDirAdjustment)) * texCoordOffset;

	vec3 result1 = 0.5 * (
		texture2D(tex, uv + (blurDir * vec2(1.0/3.0 - 0.5))).xyz +
		texture2D(tex, uv + (blurDir * vec2(2.0/3.0 - 0.5))).xyz
	);

	vec3 result2 = result1 * 0.5 + 0.5 * (
		texture2D(tex, uv + (blurDir * vec2(-0.5))).xyz +
		texture2D(tex, uv + (blurDir * vec2(0.5))).xyz
	);

	float lumaMin = min(lumaM, min(min(lumaTL, lumaTR), min(lumaBL, lumaBR)));
	float lumaMax = min(lumaM, max(max(lumaTL, lumaTR), max(lumaBL, lumaBR)));

	float lumaResult2 = dot(luma, result2);

	float alpha = texture2D(tex, uv).a;

	if(lumaResult2 < lumaMin || lumaResult2 > lumaMax)
		gl_FragColor = vec4(result1, alpha);
	else
 		gl_FragColor = vec4(result2, alpha);
	
	//gl_FragColor = vec4(texture(tex, gl_FragCoord.xy * vec2(width, height)).xyz, 1.0 );
}
*/