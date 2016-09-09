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
using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.CoreGraphics;
using XamCore.ObjCRuntime;
using Vector2i = global::OpenTK.Vector2i;
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;
using Matrix2 = global::OpenTK.Matrix2;
using Matrix3 = global::OpenTK.Matrix3;
using Matrix4 = global::OpenTK.Matrix4;
using Quaternion = global::OpenTK.Quaternion;
using MathHelper = global::OpenTK.MathHelper;

namespace XamCore.ModelIO {

	partial class MDLMesh {
		internal MDLMesh (Vector3 extent, Vector2i segments, bool inwardNormals, MDLGeometryType geometryType, IMDLMeshBufferAllocator allocator, int? hemisphereSegments, bool? cap, bool? isCone)
		{
			if (hemisphereSegments.HasValue) {
				// initCapsule
				InitializeHandle (InitCapsule (extent, segments, hemisphereSegments.Value, inwardNormals, geometryType, allocator), "initCapsuleWithExtent:cylinderSegments:hemisphereSegments:inwardNormals:geometryType:allocator:");
			} else if (cap.HasValue && isCone.HasValue) {
				// initHemisphere || initCone
				if (isCone.Value)
					InitializeHandle (InitCone (extent, segments, inwardNormals, cap.Value, geometryType, allocator), "initConeWithExtent:segments:inwardNormals:cap:geometryType:allocator:");
				else
					InitializeHandle (InitHemisphere (extent, segments, inwardNormals, cap.Value, geometryType, allocator), "initHemisphereWithExtent:segments:inwardNormals:cap:geometryType:allocator:");
			} else {
				// initSphere
				InitializeHandle (InitSphere (extent, segments, inwardNormals, geometryType, allocator), "initSphereWithExtent:segments:inwardNormals:geometryType:allocator:");
			}
		}

		internal MDLMesh (MDLMesh mesh, int submeshIndex, uint subdivisionLevels, IMDLMeshBufferAllocator allocator)
		{
			InitializeHandle (InitMesh (mesh, submeshIndex, subdivisionLevels, allocator), "initMeshBySubdividingMesh:submeshIndex:subdivisionLevels:allocator:");
		}

		public static MDLMesh CreateSphere (Vector3 dimensions, Vector2i segments, MDLGeometryType geometryType, bool inwardNormals, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (dimensions, segments, inwardNormals, geometryType, allocator, null, null, null);
		}

		public static MDLMesh CreateHemisphere (Vector3 dimensions, Vector2i segments, MDLGeometryType geometryType, bool inwardNormals, bool cap, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (dimensions, segments, inwardNormals, geometryType, allocator, null, cap, false);
		}

		public static MDLMesh CreateCapsule (Vector3 dimensions, Vector2i segments, MDLGeometryType geometryType, bool inwardNormals, int hemisphereSegments, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (dimensions, segments, inwardNormals, geometryType, allocator, hemisphereSegments, null, null);
		}

		public static MDLMesh CreateCone (Vector3 dimensions, Vector2i segments, MDLGeometryType geometryType, bool inwardNormals, bool cap, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (dimensions, segments, inwardNormals, geometryType, allocator, null, cap, true);
		}

		public static MDLMesh CreateSubdividedMesh (MDLMesh mesh, int submeshIndex, uint subdivisionLevels, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (mesh, submeshIndex, subdivisionLevels, allocator);
		}

		public MDLVertexAttributeData AnisotropyVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.Anisotropy);
			}
		}
		
		public MDLVertexAttributeData BinormalVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.Binormal);
			}
		}
		
		public MDLVertexAttributeData BitangentVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.Bitangent);
			}
		}
		
		public MDLVertexAttributeData ColorVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.Color);
			}
		}
		
		public MDLVertexAttributeData EdgeCreaseVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.EdgeCrease);
			}
		}
		
		public MDLVertexAttributeData JointIndicesVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.JointIndices);
			}
		}
		
		public MDLVertexAttributeData JointWeightsVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.JointWeights);
			}
		}
		
		public MDLVertexAttributeData NormalVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.Normal);
			}
		}
		
		public MDLVertexAttributeData OcclusionValueVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.OcclusionValue);
			}
		}
		
		public MDLVertexAttributeData PositionVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.Position);
			}
		}
		
		public MDLVertexAttributeData ShadingBasisUVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.ShadingBasisU);
			}
		}
		
		public MDLVertexAttributeData ShadingBasisVVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.ShadingBasisV);
			}
		}
		
		public MDLVertexAttributeData SubdivisionStencilVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.SubdivisionStencil);
			}
		}
		
		public MDLVertexAttributeData TangentVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.Tangent);
			}
		}
		
		public MDLVertexAttributeData TextureCoordinateVertexData {
			get {
				return GetVertexAttributeDataForAttribute (MDLVertexAttributes.TextureCoordinate);
			}
		}
	}
}
#endif