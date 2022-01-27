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
using System.Runtime.Versioning;
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

#nullable enable

namespace ModelIO {

#if !COREBUILD
	public static class MDLVertexFormatExtensions {
		
#if !NET
		[iOS (9,0)][Mac (10,11)]
#endif
		[DllImport (Constants.MetalKitLibrary)]
		static extern /* MTLVertexFormat */ nuint MTKMetalVertexFormatFromModelIO (/* MTLVertexFormat */ nuint vertexFormat);

#if !NET
		[iOS (9,0)][Mac (10,11)]
#endif
		public static MTLVertexFormat ToMetalVertexFormat (this MDLVertexFormat vertexFormat)
		{
			nuint mtlVertexFormat = MTKMetalVertexFormatFromModelIO ((nuint)(ulong)vertexFormat);
			return (MTLVertexFormat)(ulong)mtlVertexFormat;
		}
	}
#endif

	[StructLayout(LayoutKind.Sequential)]
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
