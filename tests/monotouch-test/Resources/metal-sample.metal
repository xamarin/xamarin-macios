#include <metal_stdlib>
#include <simd/simd.h>

using namespace metal;

/* structs */

struct SomeData
{
    float4 anArray [[position]];
    float4 aValue;
};

struct SomeVertex
{
    vector_float2 position;
    vector_float2 textureCoordinate;
};

/* functions */

kernel void
grayscaleKernel(texture2d<half, access::read>  inTexture  [[texture(0)]],
                texture2d<half, access::write> outTexture [[texture(1)]],
                uint2                          gid         [[thread_position_in_grid]])
{
}

vertex SomeData
vertexShader (uint vertexID [[ vertex_id ]],
              constant SomeVertex *vertexArray [[buffer(0)]],
              constant vector_uint2 *viewportSizePointer  [[buffer(1)]])
{
    SomeData out;
    return out;
}
