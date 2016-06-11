in vec2 texCoord;
in vec3 vertexPosition;


uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

uniform float time;


void main(){

	gl_TexCoord[0] = vec4(texCoord,0,0);
	gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);

}