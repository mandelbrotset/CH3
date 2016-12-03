#version 130

precision highp float;

uniform sampler2D tex;
uniform vec3 light_direction;
uniform float time;

uniform float material_shininess;
in vec3 normal;

in vec2 tex_Coord;
void main() {
	gl_FragData[0] 			=  vec4(texture(tex, tex_Coord).xyz * 0.25, 1);
	gl_FragData[1].xyz 	=  normalize(normal) * 0.5 + 0.5;
	
	gl_FragData[1].w = 1.0;
	gl_FragData[2] = vec4(material_shininess);
	
	/*gl_FragData[1].xyz =
			(has_normal_map == 1) ?
					(mat3(normalize(tangent), normalize(bitangent), normalize(normal)) * normalize(texture(normal_map, texCoord).xyz)) * 0.5 + 0.5 :
					normalize(normal) * 0.5 + 0.5;*/
}