#version 130

in vec2 texCoord;
in vec3 vertexPosition;
in vec3 vertexNormal;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;
uniform mat4 rotation_matrix;

uniform mat4 modelViewProjectionMatrix;
uniform mat4 normalMatrix;

out vec2 tex_Coord;
out vec3 normal;

void main() {
	gl_Position = modelViewProjectionMatrix *  vec4(vertexPosition, 1.0);
	normal 		= normalize(normalMatrix * vec4(vertexNormal,0)).xyz;
	tex_Coord = texCoord;
}
 