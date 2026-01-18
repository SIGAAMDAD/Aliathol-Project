#[compute]
#version 450
#extension GL_EXT_shader_explicit_arithmetic_types_int64 : require

layout( local_size_x = 8, local_size_y = 8, local_size_z = 1 ) in;

layout( set = 0, binding = 0, rgba8 ) uniform readonly image2D inputImage;
layout( set = 0, binding = 1, rgba16f ) uniform writeonly image2D outputImage;
layout( set = 0, binding = 2 ) uniform UpscaleUniforms {
	vec2 inputSize;
	vec2 outputSize;
	float sharpness;
	int upscaleMode;
};

vec3 fsrEasu( vec2 uv ) {
	vec2 pixelPos = uv * inputSize - 0.5;
	vec2 f = fract( pixelPos );
	vec2 pixelCenter = floor( pixelPos ) + 0.5;

	// Sample 4x4 neighborhood
	vec3 samples[16];
	for ( int y = -1; y <= 2; y++ ) {
		for ( int x = -1; x <= 2; x++ ) {
			vec2 sampleUV = ( pixelCenter + vec2( x, y ) ) / inputSize;
			samples[ ( y+1 )*4 + ( x+1 ) ] = imageLoad( inputImage, ivec2( sampleUV * inputSize ) ).rgb;
		}
	}

	// FSR1 weighting
	vec2 w0 = ( 1.0 - f ) * ( 1.0 - f ) * ( 1.0 - f );
	vec2 w1 = f * f * ( 3.0 - 2.0 * f );

	// Bilinear blend
	vec3 a = mix( mix( samples[ 5 ], samples[ 6 ], w1.x ), mix( samples[ 9 ], samples[ 10 ], w1.x ), w1.y );
	vec3 b = mix( mix( samples[ 6 ], samples[ 7 ], w1.x ), mix( samples[ 10 ], samples[ 11 ], w1.x ), w1.y );

	return mix( a, b, w0.x );
}

void main() {
	ivec2 outputCoord = ivec2( gl_GlobalInvocationID.xy );
	if ( outputCoord.x >= int( outputSize.x ) || outputCoord.y >= int( outputSize.y ) ) {
		return;
	}

	vec2 uv = ( vec2( outputCoord ) + 0.5 ) / outputSize;
	vec3 color;

	if ( upscaleMode == 3) { // FSR1
		color = fsrEasu( uv );
	} else { // Simple bilinear
		vec2 inputCoord = uv * inputSize;
		ivec2 base = ivec2( floor( inputCoord - 0.5 ) );
		vec2 f = fract( inputCoord - 0.5 );

		vec3 c00 = imageLoad( inputImage, base ).rgb;
		vec3 c10 = imageLoad( inputImage, base + ivec2( 1, 0 ) ).rgb;
		vec3 c01 = imageLoad( inputImage, base + ivec2( 0, 1 ) ).rgb;
		vec3 c11 = imageLoad( inputImage, base + ivec2( 1, 1 ) ).rgb;

		color = mix( mix( c00, c10, f.x ), mix( c01, c11, f.x ), f.y );
	}

	// Apply sharpening
	if ( upscaleMode == 3 ) {
		vec2 texelSize = 1.0 / inputSize;
		vec3 neighborSum = vec3( 0.0 );
		for ( int y = -1; y <= 1; y++ ) {
			for ( int x = -1; x <= 1; x++ ) {
				vec2 offset = vec2( x, y ) * texelSize;
				vec2 sampleUV = clamp( uv + offset, 0.0, 1.0 );
				ivec2 sampleCoord = ivec2( sampleUV * inputSize );
				neighborSum += imageLoad( inputImage, sampleCoord ).rgb;
			}
		}
		vec3 blur = neighborSum / 9.0;
		color = color + ( color - blur ) * sharpness;
	}

	imageStore( outputImage, outputCoord, vec4( color, 1.0 ) );
}
