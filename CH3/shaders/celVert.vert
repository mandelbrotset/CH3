#version 130


in vec2 texCoord;
in vec3 vertexPosition;
in vec3 vertexNormal;

out float z;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;
uniform mat4 rotation_matrix;

uniform float time;


void main(){
	z = (projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1)).z;
	gl_TexCoord[0] = vec4(texCoord,0,0);
	gl_TexCoord[1] = rotation_matrix * vec4(vertexNormal, 1);
	gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);

}