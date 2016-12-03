#version 140


in vec2 texCoord;
in vec3 vertexPosition;
in vec3 vertexNormal;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(){
	gl_Position = projection_matrix * view_matrix  * vec4(vertexPosition, 1);

}