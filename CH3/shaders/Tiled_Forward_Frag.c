#version 330

precision highp float;

#define NUM_POSSIBLE_LIGHTS 10
#define WIDTH  1280
#define HEIGHT  720
#define LIGHT_GRID_TILE_DIM_X 32
#define LIGHT_GRID_TILE_DIM_Y 32
#define LIGHT_GRID_MAX_DIM_X  40
//((int)WIDTH  + LIGHT_GRID_TILE_DIM_X - 1) / LIGHT_GRID_TILE_DIM_X
#define LIGHT_GRID_MAX_DIM_Y 23
//((int)HEIGHT + LIGHT_GRID_TILE_DIM_Y - 1) / LIGHT_GRID_TILE_DIM_Y

struct Light {
	vec3 	position;
	float 	range;
	vec3 	color;
	int 	type;
	vec3 	direction;
	float	angle;
};

uniform LightGrid
{
	ivec4 lightGridCountOffsets[LIGHT_GRID_MAX_DIM_X * LIGHT_GRID_MAX_DIM_Y];
};

uniform isamplerBuffer tileLightIndexListsTex;

layout(std140) uniform LightData {
	Light g_lights[NUM_POSSIBLE_LIGHTS];
};

uniform float time;

uniform sampler2D tex;
uniform vec3 light_direction;

uniform mat4 viewMatrix;

in vec3 normal;
in vec2 tex_Coord;
in vec3	position;

vec3 doLight(vec3 lightDir, vec3 normal, vec3 diffuse, vec3 specular, float shininess, vec3 viewDir, vec3 lightColor)
{
	float ndotL = max(dot(normal, lightDir),0.0);
	
	vec3 fresnelSpec = specular + (vec3(1.0) - specular) * pow(clamp(1.0 + dot(-viewDir, normal), 0.0, 1.0), 5.0);
	vec3 h = normalize(lightDir + viewDir);

	float normalizationFactor = ((shininess + 2.0) / 8.0);

	vec3 spec = fresnelSpec * pow(max(0, dot(h, normal)), shininess) * normalizationFactor;

 
	return ndotL * lightColor * (diffuse + spec);
}

float lerp(float a, float b, float w)
{
  return a + w*(b-a);
}

vec3 doLight(vec3 normal, vec3 diffuse, vec3 specular, float shininess, vec3 viewDir, int lightIndex)
{
	vec3 lightColor = g_lights[lightIndex].color;
	
	vec3 lightDir;
	float lightAttenuation = 1.0f;

	if(g_lights[lightIndex].type == 0 /*PointLight*/) {
		vec3 lightPos 	 = g_lights[lightIndex].position;
		float lightRange = g_lights[lightIndex].range;

		lightDir = vec3(lightPos) - position;
		float dist = length(lightDir);
		lightDir = normalize(lightDir);

		float inner = 0.0;
		lightAttenuation = max(1.0 - max(0.0, (dist - inner) / (lightRange - inner)), 0.0);
		
	} else if(g_lights[lightIndex].type == 1 /*SpotLight*/) {
		vec3 lightPos 	 = g_lights[lightIndex].position;
		float lightRange = g_lights[lightIndex].range;

		lightDir = vec3(lightPos) - position;
		float dist = length(lightDir);
		lightDir = normalize(lightDir);

		float inner = 0.0;
		lightAttenuation = max(1.0 - max(0.0, (dist - inner) / (lightRange - inner)), 0.0);

	    float minCos = cos( g_lights[lightIndex].angle );
	    float maxCos = lerp( minCos, 1, 0.5f );
	    float cosAngle = dot(normalize(g_lights[lightIndex].direction), -lightDir);
    	lightAttenuation *= smoothstep( minCos, maxCos, cosAngle );

	} else if(g_lights[lightIndex].type == 2 /*DirectionalLight*/) {
		lightDir = normalize(g_lights[lightIndex].direction);
	}

	return doLight(lightDir, normal, diffuse, specular, shininess, viewDir, lightColor) * lightAttenuation;
}

vec3 evalTiledLighting(in vec3 diffuse, in vec3 specular, in float shininess, in vec3 normal, in vec3 viewDir)
{
	ivec2 l = ivec2(int(gl_FragCoord.x) / LIGHT_GRID_TILE_DIM_X, int(gl_FragCoord.y) / LIGHT_GRID_TILE_DIM_Y);

	int lightOffset = lightGridCountOffsets[l.x + l.y * LIGHT_GRID_MAX_DIM_X].y;  
	int lightCount  = lightGridCountOffsets[l.x + l.y * LIGHT_GRID_MAX_DIM_X].x;

	vec3 color = vec3(0.0, 0.0, 0.0);

	for (int i = 0; i < lightCount; ++i)
	{
		int lightIndex = texelFetch(tileLightIndexListsTex, lightOffset + i).x; 
		color += doLight(normal, diffuse, specular, shininess, viewDir, lightIndex);
	}

	return color;
}


void main()
{
	vec3 viewDir = -normalize(position);


	vec3 color = vec3(1.0) ;

	vec3 diffuse  = texture(tex, tex_Coord).xyz;
	vec3 specular = vec3(0.8f);
	float material_specular_exponent = 1.0f;
	vec3 lighting = evalTiledLighting(diffuse, specular, material_specular_exponent, normalize(normal), viewDir);

	color = lighting + diffuse * 0.25;

	gl_FragColor = vec4(color, 1.0f);
}