#if XAMCORE_2_0 || !MONOMAC
//
// ModelIO/MIEnums.cs: enumerations and definitions
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2015 Xamarin, Inc.
//
//
using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using Metal;
using ObjCRuntime;
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;
using Vector4i = global::OpenTK.Vector4i;
using VectorInt4 = global::OpenTK.Vector4i;
using Matrix2 = global::OpenTK.Matrix2;
using Matrix3 = global::OpenTK.Matrix3;
using Matrix4 = global::OpenTK.Matrix4;
using Quaternion = global::OpenTK.Quaternion;
using MathHelper = global::OpenTK.MathHelper;

namespace ModelIO {
	[Native]
	public enum MDLVertexFormat : ulong {
		Invalid = 0,

		PackedBits           =  0x1000,
		UCharBits            = 0x10000,
		CharBits             = 0x20000,
		UCharNormalizedBits  = 0x30000,
		CharNormalizedBits   = 0x40000,
		UShortBits           = 0x50000,
		ShortBits            = 0x60000,
		UShortNormalizedBits = 0x70000,
		ShortNormalizedBits  = 0x80000,
		UIntBits             = 0x90000,
		IntBits              = 0xA0000,
		HalfBits             = 0xB0000,
		FloatBits            = 0xC0000,

		UChar  = UCharBits | 1,
		UChar2 = UCharBits | 2,
		UChar3 = UCharBits | 3,
		UChar4 = UCharBits | 4,

		Char =  CharBits | 1,
		Char2 = CharBits | 2,
		Char3 = CharBits | 3,
		Char4 = CharBits | 4,

		UCharNormalized  = UCharNormalizedBits | 1,
		UChar2Normalized = UCharNormalizedBits | 2,
		UChar3Normalized = UCharNormalizedBits | 3,
		UChar4Normalized = UCharNormalizedBits | 4,

		CharNormalized  = CharNormalizedBits | 1,
		Char2Normalized = CharNormalizedBits | 2,
		Char3Normalized = CharNormalizedBits | 3,
		Char4Normalized = CharNormalizedBits | 4,

		UShort  = UShortBits | 1,
		UShort2 = UShortBits | 2,
		UShort3 = UShortBits | 3,
		UShort4 = UShortBits | 4,

		Short  = ShortBits | 1,
		Short2 = ShortBits | 2,
		Short3 = ShortBits | 3,
		Short4 = ShortBits | 4,

		UShortNormalized  = UShortNormalizedBits | 1,
		UShort2Normalized = UShortNormalizedBits | 2,
		UShort3Normalized = UShortNormalizedBits | 3,
		UShort4Normalized = UShortNormalizedBits | 4,

		ShortNormalized  = ShortNormalizedBits | 1,
		Short2Normalized = ShortNormalizedBits | 2,
		Short3Normalized = ShortNormalizedBits | 3,
		Short4Normalized = ShortNormalizedBits | 4,

		UInt  = UIntBits | 1,
		UInt2 = UIntBits | 2,
		UInt3 = UIntBits | 3,
		UInt4 = UIntBits | 4,

		Int  = IntBits | 1,
		Int2 = IntBits | 2,
		Int3 = IntBits | 3,
		Int4 = IntBits | 4,

		Half  = HalfBits | 1,
		Half2 = HalfBits | 2,
		Half3 = HalfBits | 3,
		Half4 = HalfBits | 4,

		Float  = FloatBits | 1,
		Float2 = FloatBits | 2,
		Float3 = FloatBits | 3,
		Float4 = FloatBits | 4,

		Int1010102Normalized  = IntBits  | PackedBits | 4,
		UInt1010102Normalized = UIntBits | PackedBits | 4,
	}

#if !COREBUILD
	public static class MDLVertexFormatExtensions {
		
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[DllImport (Constants.MetalKitLibrary)]
		static extern /* MTLVertexFormat */ nuint MTKMetalVertexFormatFromModelIO (/* MTLVertexFormat */ nuint vertexFormat);

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		public static MTLVertexFormat ToMetalVertexFormat (this MDLVertexFormat vertexFormat)
		{
			nuint mtlVertexFormat = MTKMetalVertexFormatFromModelIO ((nuint)(ulong)vertexFormat);
			return (MTLVertexFormat)(ulong)mtlVertexFormat;
		}
	}
#endif

	[Native]
	public enum MDLMeshBufferType : ulong
	{
		Vertex = 1,
		Index = 2
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct  MDLAxisAlignedBoundingBox {
		public Vector3 MaxBounds;
		public Vector3 MinBounds;

		public MDLAxisAlignedBoundingBox (Vector3 maxBounds, Vector3 minBounds)
		{
			MaxBounds = maxBounds;
			MinBounds = minBounds;
		}

	}

	[Native]
	public enum MDLGeometryType : long
	{
		Points = 0,
		Lines,
		Triangles,
		TriangleStrips,
		Quads,
		VariableTopology,
	}
	
	[Native]
	public enum MDLIndexBitDepth : ulong {
		Invalid,
		UInt8 = 8,
		UInt16 = 16,
		UInt32 = 32,
	}

	[Native]
	public enum MDLMaterialSemantic : ulong {
		BaseColor = 0,
		Subsurface,
		Metallic,
		Specular,
		SpecularExponent,
		SpecularTint,
		Roughness,
		Anisotropic,
		AnisotropicRotation,
		Sheen,
		SheenTint,
		Clearcoat,
		ClearcoatGloss,
		Emission,
		Bump,
		Opacity,
		InterfaceIndexOfRefraction,
		MaterialIndexOfRefraction,
		ObjectSpaceNormal,
		TangentSpaceNormal,
		Displacement,
		DisplacementScale,
		AmbientOcclusion,
		AmbientOcclusionScale,
		None = 0x8000,
		UserDefined = 0x8001
	}

	[Native]
	public enum MDLMaterialPropertyType : ulong {
		None,
		String,
		Url,
		Texture,
		Color,
		Float,
		Float2,
		Float3,
		Float4,
		Matrix44
	}

	[Native]
	public enum MDLMaterialTextureWrapMode : ulong {
		Clamp, Repeat, Mirror
	}

	[Native]
	public enum MDLMaterialTextureFilterMode : ulong {
		Nearest,
		Linear
	}

	[Native]
	public enum MDLMaterialMipMapFilterMode : ulong {
		Nearest,
		Linear
	}

	[Native]
	public enum MDLTextureChannelEncoding : long {
		UInt8 = 1,
		UInt16 = 2,
		UInt24 = 3,
		UInt32 = 4,
		Float16 = 258,
		Float16SR = 770,
		Float32 = 260
	}

	[Native]
	public enum MDLLightType : ulong
	{
		Unknown = 0,
		Ambient,
		Directional,
		Spot,
		Point,
		Linear,
		DiscArea,
		RectangularArea,
		SuperElliptical,
		Photometric,
		Probe,
		Environment
	}

#if !XAMCORE_4_0
	[Obsolete ("Use 'MDLVoxelIndexExtent2' instead.")]
	[StructLayout(LayoutKind.Sequential)]
	public struct MDLVoxelIndexExtent {
		public MDLVoxelIndexExtent (Vector4 minimumExtent, Vector4 maximumExtent)
		{
			this.MinimumExtent = minimumExtent;
			this.MaximumExtent = maximumExtent;
		}
		public Vector4 MinimumExtent, MaximumExtent;
	}
#endif

	[StructLayout(LayoutKind.Sequential)]
#if XAMCORE_4_0
	public struct MDLVoxelIndexExtent {
#else
	public struct MDLVoxelIndexExtent2 {
#endif
		public VectorInt4 MinimumExtent { get; private set; }
		public VectorInt4 MaximumExtent { get; private set; }

#if XAMCORE_4_0
		public MDLVoxelIndexExtent (VectorInt4 minimumExtent, VectorInt4 maximumExtent)
#else
		public MDLVoxelIndexExtent2 (VectorInt4 minimumExtent, VectorInt4 maximumExtent)
#endif
		{
			this.MinimumExtent = minimumExtent;
			this.MaximumExtent = maximumExtent;
		}
	}

	[Native]
	public enum MDLCameraProjection : ulong
	{
		Perspective = 0,
		Orthographic = 1,
	}

	[Native]
	public enum MDLMaterialFace : ulong
	{
		Front = 0,
		Back,
		DoubleSided,
	}

	[Native]
	public enum MDLProbePlacement : long
	{
		UniformGrid = 0,
		IrradianceDistribution,
	}
}
#endif
