
uniform float time;
uniform vec2 resolution;

void main( void ) {

	vec2 position = ( gl_FragCoord.xy / resolution.x ) + 1.0 / 4.0;
	float t = 0.001*time;

	float color = 0.0;
	color += sin( position.x * cos( t / 15.0 ) * 80.0 ) + cos( position.y * cos( t / 15.0 ) * 10.0 );
	color += sin( position.y * sin( t / 10.0 ) * 40.0 ) + cos( position.x * sin( t / 25.0 ) * 40.0 );
	color += sin( position.x * sin( t / 5.0 ) * 10.0 ) + sin( position.y * sin( t / 35.0 ) * 80.0 );
	color *= sin( t / 10.0 ) * 0.5;
	gl_FragColor = vec4( vec3( color, color * 0.5, sin( color + t / 3.0 ) * 0.75 ), 1.0 );

}