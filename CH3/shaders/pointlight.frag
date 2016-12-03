#version 130

precision highp float;

uniform vec3 lightColor;
uniform float lightRadius;

uniform sampler2D colorTexture;
uniform sampler2D specularTexture;
uniform sampler2D normalTexture;
uniform sampler2D depthTexture;

uniform mat4 inverseProjectionMatrix;
uniform vec2 screenSize;

in vec3 lightPos;
out vec4 fragmentColor;

#define ATTCONST 0.1
#define ATTLINER 0.5
#define ATTQUADA 0.9

vec3 getPosition(vec2 fragPos, float depth) {
	vec4 position = inverseProjectionMatrix * (vec4(fragPos.xy, depth, 1.0) * 2 - 1);
	return (position / position.w).xyz;
}

vec3 calculateDiffuse(vec3 diffuseLight, vec3 materialDiffuse, vec3 normal, vec3 directionToLight) {
	//I_{D} = (L dot N)*C*I_{L})
	// L = directionToLight
	// N = normal
	// I_{L} = diffuseLight
	// C = materialDiffuse
	return diffuseLight * materialDiffuse * max(0, dot(normal, directionToLight));
}

vec3 calculateSpecular(vec3 specularLight, vec3 materialSpecular, float materialShininess, vec3 normal, vec3 directionToLight, vec3 directionFromEye) {
	// (n dot h)^m
	// h = (L + V) / |L + V |

	// n = normal
	//  L = directionToLight
	//  V = directionFromEye
	//  m = materialShininess

	float normalizationFactor = ((materialShininess + 2.0) / 8.0);

	vec3 h = normalize(directionToLight - directionFromEye);
	return specularLight * materialSpecular * pow(max(0, dot(h, normal)), materialShininess) * normalizationFactor;
}

vec3 calculateFresnel(vec3 materialSpecular, vec3 normal, vec3 directionFromEye) {
	// R0 + (1- R0)(H * V)
	//
	return materialSpecular + (vec3(1.0) - materialSpecular) * pow(clamp(1.0 + dot(directionFromEye, normal), 0.0, 1.0), 5.0);
}

void main() {
vec3 lightPosition = vec3(0);

	vec2 fragPos = gl_FragCoord.xy / screenSize;
	vec3 position = getPosition(fragPos, texture(depthTexture, fragPos).x);
	float distance = length(lightPos.xyz - position);

	if(distance < lightRadius ) {
		vec3 normal = normalize(((texture(normalTexture, fragPos) * 2.0 - 1.0).xyz));
		vec3 directionFromEye = normalize(position);
		vec3 directionToLight2 = normalize(vec4(lightPosition.xyz, 1).xyz - position);

		vec3 materialColor = texture(colorTexture, fragPos).xyz;
		float spec = texture(specularTexture, fragPos).x;
		vec4 specularColor = vec4(spec) * 0.2;

		float r = max(lightRadius - distance, 0);
		float attenuation = 1 / (ATTCONST + distance * ATTLINER / r + distance * ATTQUADA / (r * r));

		//attenuation = r / (distance);

		vec3 fresnelSpecular = calculateFresnel(specularColor.rgb, normalize(normal), directionFromEye);
		
		fragmentColor = vec4(
				calculateDiffuse(lightColor, materialColor, normal, directionToLight2) + calculateSpecular(vec3(specularColor.x), fresnelSpecular, specularColor.a * 50 , normal, directionToLight2, directionFromEye), 1) * attenuation;
			
	} else {
		discard;
	}
	//fragmentColor = vec4(1,0,0,1);
}

