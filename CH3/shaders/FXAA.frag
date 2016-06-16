uniform float time;

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





}