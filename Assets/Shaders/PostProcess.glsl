// === post_process.glsl ===
#[compute]
#version 450
#extension GL_EXT_shader_explicit_arithmetic_types_int64 : require

layout( local_size_x = 8, local_size_y = 8, local_size_z = 1 ) in;

layout( set = 0, binding = 0, rgba16f ) uniform readonly image2D inputImage;
layout( set = 0, binding = 1, rgba8 ) uniform writeonly image2D outputImage;
layout( set = 0, binding = 2 ) uniform PostProcessUniforms {
	vec2 inputSize;
	vec2 outputSize;
	float brightness;
	float contrast;
	float saturation;
	float vignetteIntensity;
	float vignetteSoftness;
	float filmGrainIntensity;
	float time;
	int toneMappingMode;
	float exposure;
	float gamma;
};
layout( set = 0, binding = 3 ) uniform sampler2D noiseTexture;

// Tone mapping operators
vec3 reinhard(vec3 color) {
	return color / (1.0 + color);
}

vec3 aces(vec3 color) {
	const float a = 2.51;
	const float b = 0.03;
	const float c = 2.43;
	const float d = 0.59;
	const float e = 0.14;
	return clamp((color * (a * color + b)) / (color * (c * color + d) + e), 0.0, 1.0);
}

vec3 uncharted2(vec3 color) {
	const float A = 0.15;
	const float B = 0.50;
	const float C = 0.10;
	const float D = 0.20;
	const float E = 0.02;
	const float F = 0.30;

	color *= exposure;
	return ((color * (A * color + C * B) + D * E) / (color * (A * color + B) + D * F)) - E / F;
}

// Color space conversions
float luminance(vec3 color) {
	return dot(color, vec3(0.2126, 0.7152, 0.0722));
}

void main() {
	ivec2 coord = ivec2(gl_GlobalInvocationID.xy);
	if (coord.x >= int(outputSize.x) || coord.y >= int(outputSize.y)) return;

	vec2 uv = (vec2(coord) + 0.5) / outputSize;
	vec3 color = imageLoad(inputImage, coord).rgb;

	// === 1. Brightness & Contrast ===
	color *= brightness;
	color = (color - 0.5) * contrast + 0.5;

	// === 2. Saturation ===
	float lum = luminance(color);
	color = mix(vec3(lum), color, saturation);

	// === 3. Tone Mapping (HDR to LDR) ===
	if (toneMappingMode == 1) {
		color = reinhard(color);
	} else if (toneMappingMode == 2) {
		color = aces(color);
	} else if (toneMappingMode == 3) {
		color = uncharted2(color);
		color /= uncharted2(vec3(11.2)); // White point
	}

	// === 4. Vignette ===
	vec2 vignetteUV = uv * (1.0 - uv);
	float vignette = pow(vignetteUV.x * vignetteUV.y * 15.0, vignetteSoftness);
	vignette = clamp(vignette, 0.0, 1.0);
	color *= mix(1.0, vignette, vignetteIntensity);

	// === 5. Film Grain ===
	if (filmGrainIntensity > 0.0) {
		vec3 noise = texture(noiseTexture, uv * 10.0 + time * 0.1).rgb;
		float grain = noise.r * 2.0 - 1.0;
		color += grain * filmGrainIntensity * 0.1;
	}

	// === 6. Gamma Correction ===
	color = pow(color, vec3(1.0 / gamma));

	// Clamp and output
	color = clamp(color, 0.0, 1.0);
	imageStore(outputImage, coord, vec4(color, 1.0));
}
