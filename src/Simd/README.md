# Numerics types in .NET

## Type mapping between legacy Xamarin and .NET


| Xamarin Legacy Type |                         .NET Type                         |
| ------------------- | --------------------------------------------------------- |
| OpenTK.Matrix3      | [CoreGraphics.RMatrix3](#coregraphics.rmatrix3)           |
| OpenTK.Matrix4      | [System.Numerics.Matrix4x4](#system.numerics.matrix4x4)   |
| OpenTK.NMatrix2     | [CoreGraphics.NMatrix2](#coregraphics.nmatrix2)           |
| OpenTK.NMatrix3     | [CoreGraphics.NMatrix3](#coregraphics.nmatrix3)           |
| OpenTK.NMatrix4     | [CoreGraphics.NMatrix4](#coregraphics.nmatrix4)           |
| OpenTK.NMatrix4d    | [CoreGraphics.NMatrix4d](#coregraphics.nmatrix4d)         |
| OpenTK.NMatrix4x3   | [CoreGraphics.NMatrix4x3](#coregraphics.nmatrix4x3)       |
| OpenTK.NVector3     | [CoreGraphics.NVector3](#coregraphics.nvector3)           |
| OpenTK.NVector3d    | [CoreGraphics.NVector3d](#coregraphics.nvector3d)         |
| OpenTK.Quaternion   | [System.Numerics.Quaternion](#system.numerics.quaternion) |
| OpenTK.Quaterniond  | [CoreGraphics.NQuaterniond](#coregraphics.nquaterniond)   |
| OpenTK.Vector2      | [System.Numerics.Vector2](#system.numerics.vector2)       |
| OpenTK.Vector2d     | [CoreGraphics.NVector2d](#coregraphics.nvector2d)         |
| OpenTK.Vector2i     | [CoreGraphics.NVector2i](#coregraphics.nvector2i)         |
| OpenTK.Vector3      | [System.Numerics.Vector3](#system.numerics.vector3)       |
| OpenTK.Vector3d     | [CoreGraphics.NVector3d](#coregraphics.nvector3d)          |
| OpenTK.Vector3i     | [CoreGraphics.NVector3i](#coregraphics.nvector3i)         |
| OpenTK.Vector4      | [System.Numerics.Vector4](#system.numerics.vector4)       |
| OpenTK.Vector4d     | [CoreGraphics.NVector4d](#coregraphics.nvector4d)         |
| OpenTK.Vector4i     | [CoreGraphics.NVector4i](#coregraphics.nvector4i)         |

## Removed types in .NET

The following types aren't used by any API, so they've been removed

|         Removed           |
| ------------------------- |
| OpenTK.BezierCurve        |
| OpenTK.BezierCurveCubic   |
| OpenTK.BezierCurveQuadric |
| OpenTK.Box2               |
| OpenTK.Functions          |
| OpenTK.Half               |
| OpenTK.MathHelper         |
| OpenTK.Matrix2            |
| OpenTK.Matrix4d           |
| OpenTK.Vector2h           |
| OpenTK.Vector3h           |
| OpenTK.Vector4h           |

## Current Types in .NET

### CoreGraphics.NMatrix2

A 2x2 column-major matrix of floats.

Corresponds with the following native types:

* [matrix_float2x2](https://developer.apple.com/documentation/accelerate/matrix_float2x2)
* [simd_float2x2](https://developer.apple.com/documentation/accelerate/simd_float2x2)

### CoreGraphics.NMatrix3

A 3x3 column-major matrix of floats.

For memory alignment purposes, this is represented as 3 vectors of length 4,
which means that the size of this struct is 48 bytes (and not 36 bytes).

See top of /usr/include/simd/matrix_types.h for more information.

Corresponds with the following native types:

* [matrix_float3x3](https://developer.apple.com/documentation/accelerate/matrix_float3x3)
* [simd_float3x3](https://developer.apple.com/documentation/accelerate/simd_float3x3)

### CoreGraphics.NMatrix4

A 4x4 column-major matrix of floats.

Corresponds with the following native types:

* [matrix_float4x4](https://developer.apple.com/documentation/accelerate/matrix_float4x4)
* [simd_float4x4](https://developer.apple.com/documentation/accelerate/simd_float4x4)

### CoreGraphics.NMatrix4d

A 4x4 column-major matrix of doubles.

Corresponds with the following native types:

* [matrix_double4x4](https://developer.apple.com/documentation/accelerate/matrix_double4x4)
* [simd_double4x4](https://developer.apple.com/documentation/accelerate/simd_double4x4)

### CoreGraphics.NMatrix4x3

A 4x3 column-major matrix of floats (3 rows and 4 columns).

Corresponds with the following native types:

* [matrix_float4x3](https://developer.apple.com/documentation/accelerate/matrix_float4x4)
* [simd_float4x3](https://developer.apple.com/documentation/accelerate/simd_float4x4)

### CoreGraphics.NQuaterniond

A quaternion of doubles.

Corresponds with the following native types:

* [simd_quatd](https://developer.apple.com/documentation/accelerate/simd_quatd)

### CoreGraphics.NVector2d

A vector of 2 64-bit doubles.

Corresponds with the following native types:

* [vector_double2](https://developer.apple.com/documentation/accelerate/vector_double2)
* [simd_double2](https://developer.apple.com/documentation/accelerate/simd_double2)

### CoreGraphics.NVector2i

A vector of 2 32-bit ints.

Corresponds with the following native types:

* [vector_int2](https://developer.apple.com/documentation/accelerate/vector_int2)
* [simd_int2](https://developer.apple.com/documentation/accelerate/simd_int2)

### CoreGraphics.NVector3

A vector of 3 32-bit singles.

For memory alignment purposes, this vector has a length of 4 singles, which
means that the size of this struct is 16 bytes (and not 12 bytes).

See top of /usr/include/simd/vector_types.h for more information.

Corresponds with the following native types:

* [vector_float3](https://developer.apple.com/documentation/accelerate/vector_float3)
* [simd_float3](https://developer.apple.com/documentation/accelerate/simd_float3)

### CoreGraphics.NVector3d

A vector of 3 64-bit doubles.

For memory alignment purposes, this vector has a length of 4 doubles, which
means that the size of this struct is 32 bytes (and not 24 bytes).

See top of /usr/include/simd/vector_types.h for more information.

Corresponds with the following native types:

* [vector_double3](https://developer.apple.com/documentation/accelerate/vector_double3)
* [simd_double3](https://developer.apple.com/documentation/accelerate/simd_double3)

### CoreGraphics.NVector3i

A vector of 3 43-bit ints.

For memory alignment purposes, this vector has a length of 4 ints, which
means that the size of this struct is 16 bytes (and not 12 bytes).

See top of /usr/include/simd/vector_types.h for more information.

Corresponds with the following native types:

* [vector_int3](https://developer.apple.com/documentation/accelerate/vector_int3)
* [simd_int3](https://developer.apple.com/documentation/accelerate/simd_int3)

### CoreGraphics.NVector4d

A vector of 4 64-bit doubles.

Corresponds with the following native types:

* [vector_double4](https://developer.apple.com/documentation/accelerate/vector_double4)
* [simd_double4](https://developer.apple.com/documentation/accelerate/simd_double4)

### CoreGraphics.NVector4i

A vector of 4 32-bit ints.

Corresponds with the following native types:

* [vector_int4](https://developer.apple.com/documentation/accelerate/vector_int4)
* [simd_int4](https://developer.apple.com/documentation/accelerate/simd_int4)

### CoreGraphics.RMatrix3

A 3x3 row-major matrix of floats (as opposed to `CoreGraphics.NMatrix3`, which is column-major).

Corresponds with the following native types:

* [GLKMatrix3](https://developer.apple.com/documentation/glkit/glkmatrix3)

### System.Numerics.Matrix4x4

A 4x4 row-major matrix of floats (as opposed to `CoreGraphics.NMatrix4`, which is column-major).

Corresponds with the following native types:

* [GLKMatrix4](https://developer.apple.com/documentation/glkit/glkmatrix4)

### System.Numerics.Quaternion

A quaternion of floats.

Corresponds with the following native types:

* [simd_quatf](https://developer.apple.com/documentation/accelerate/simd_quatf)

### System.Numerics.Vector2

A vector of 2 32-bit floats.

Corresponds with the following native types:

* [vector_float2](https://developer.apple.com/documentation/accelerate/vector_float2)
* [simd_float2](https://developer.apple.com/documentation/accelerate/simd_float2)

### System.Numerics.Vector3

A vector of 3 32-bit floats.

As opposed to the `CoreGraphics.NVector3` struct, this struct is 12 bytes,
because it does not have any special alignment/padding.

Corresponds with the following native types:

* [vector_float3](https://developer.apple.com/documentation/accelerate/vector_float3)
* [simd_float3](https://developer.apple.com/documentation/accelerate/simd_float3)

### System.Numerics.Vector4

A vector of 4 32-bit floats.

Corresponds with the following native types:

* [vector_float4](https://developer.apple.com/documentation/accelerate/vector_float4)
* [simd_float4](https://developer.apple.com/documentation/accelerate/simd_float4)
