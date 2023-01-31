//
// ModelIO/MIEnums.cs: definitions
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
#if NET
using Vector3 = global::System.Numerics.Vector3;
using Vector4 = global::System.Numerics.Vector4;
using VectorInt4 = global::CoreGraphics.NVector4i;
#else
using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;
using VectorInt4 = global::OpenTK.Vector4i;
#endif

#nullable enable

namespace ModelIO {

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class MDLVertexFormatExtensions {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.MetalKitLibrary)]
		static extern /* MTLVertexFormat */ nuint MTKMetalVertexFormatFromModelIO (/* MTLVertexFormat */ nuint vertexFormat);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static MTLVertexFormat ToMetalVertexFormat (this MDLVertexFormat vertexFormat)
		{
			nuint mtlVertexFormat = MTKMetalVertexFormatFromModelIO ((nuint) (ulong) vertexFormat);
			return (MTLVertexFormat) (ulong) mtlVertexFormat;
		}
	}
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MDLAxisAlignedBoundingBox {
		public Vector3 MaxBounds;
		public Vector3 MinBounds;

		public MDLAxisAlignedBoundingBox (Vector3 maxBounds, Vector3 minBounds)
		{
			MaxBounds = maxBounds;
			MinBounds = minBounds;
		}

	}

#if !NET
	[Obsolete ("Use 'MDLVoxelIndexExtent2' instead.")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MDLVoxelIndexExtent {
		public MDLVoxelIndexExtent (Vector4 minimumExtent, Vector4 maximumExtent)
		{
			this.MinimumExtent = minimumExtent;
			this.MaximumExtent = maximumExtent;
		}
		public Vector4 MinimumExtent, MaximumExtent;
	}
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
#if NET
	public struct MDLVoxelIndexExtent {
#else
	public struct MDLVoxelIndexExtent2 {
#endif
		public VectorInt4 MinimumExtent { get; private set; }
		public VectorInt4 MaximumExtent { get; private set; }

#if NET
		public MDLVoxelIndexExtent (VectorInt4 minimumExtent, VectorInt4 maximumExtent)
#else
		public MDLVoxelIndexExtent2 (VectorInt4 minimumExtent, VectorInt4 maximumExtent)
#endif
		{
			this.MinimumExtent = minimumExtent;
			this.MaximumExtent = maximumExtent;
		}
	}
}
