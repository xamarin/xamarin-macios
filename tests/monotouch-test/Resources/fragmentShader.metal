#include <metal_stdlib>
#include <simd/simd.h>

using namespace metal;

/* structs */

struct SomeData2
{
    float4 position [[ position ]];
    float2 texcoord;
};

struct SomeInputs {
    texture2d<float> texture;
    texture2d<float> anotherTexture;
    sampler textureSampler;
    float time;
};

// The following function needs min iOS version 10.0+, so means we can't just include it in monotouch-test and have our build compile this file.
// Instead we pre-compile this file manually, with a specific iOS version, and we embed that in the app instead.
fragment float4
fragmentShader2 (SomeData2 in [[stage_in]],
                 constant SomeInputs &inputs [[buffer(0)]])
{
    float4 color = { 0, 0, 0, 0 };
    return color;
}
