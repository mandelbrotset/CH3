#version 330

precision highp float;

in vec2 texCoord;
in vec3 vertexPosition;
in vec3 vertexNormal;

// NEW
uniform mat4 modelViewProjectionMatrix;
uniform mat4 normalMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelviewMatrix;


out vec2 tex_Coord;
out vec3 normal;
out vec3 position;

void main(){
	position = (modelviewMatrix * vec4(vertexPosition, 1.0)).xyz;

	gl_Position = modelViewProjectionMatrix * vec4(vertexPosition, 1.0);
	normal 		= normalize(normalMatrix * vec4(vertexNormal, 0.0)).xyz;
	tex_Coord = texCoord;
}

/*


in vec3 position;
in vec3 normalIn;
in vec2 texCoordIn;
in vec3 tangentIn;
in vec3 bitangentIn;

out	vec3 v2f_normal;
out	vec3 v2f_tangent;
out	vec3 v2f_bitangent;
out	vec3 v2f_position;
out	vec2 v2f_texCoord;

void main()
{	
  v2f_normal = normalize((normalMatrix * vec4(normalIn, 0.0)).xyz);
  v2f_tangent = normalize((normalMatrix * vec4(tangentIn, 0.0)).xyz);
  v2f_bitangent = normalize((normalMatrix * vec4(bitangentIn, 0.0)).xyz);
  v2f_texCoord = texCoordIn;
  v2f_position = (viewMatrix * vec4(position, 1.0)).xyz;
  gl_Position = viewProjectionMatrix * vec4(position, 1.0);
} 

*/