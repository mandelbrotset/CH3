#version 130

precision highp float;

in vec2 texCoord;
in vec3 vertexPosition;
in vec3 vertexNormal;

uniform mat4 MVP_matrix;
uniform mat4 V_matrix;

uniform vec3 lightPosition;

out vec3 lightPos;

void main() {
	gl_Position = MVP_matrix * vec4(vertexPosition, 1.0);
	lightPos = vec3(V_matrix * vec4(lightPosition, 1.0)).xyz;
}
