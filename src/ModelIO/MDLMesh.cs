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
using ObjCRuntime;
using Vector2i = global::OpenTK.Vector2i;
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Vector3i = global::OpenTK.Vector3i;
using Vector4 = global::OpenTK.Vector4;
using Matrix2 = global::OpenTK.Matrix2;
using Matrix3 = global::OpenTK.Matrix3;
using Matrix4 = global::OpenTK.Matrix4;
using Quaternion = global::OpenTK.Quaternion;
using MathHelper = global::OpenTK.MathHelper;

namespace ModelIO {

	partial class MDLMesh {

		public enum  MDLMeshVectorType {
			Dimensions,
			Extent,
		}

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

		internal MDLMesh (Vector3 extent, Vector3i segments, bool inwardNormals, MDLGeometryType geometryType, IMDLMeshBufferAllocator allocator)
		{
			InitializeHandle (InitBox (extent, segments, inwardNormals, geometryType, allocator), "initBoxWithExtent:segments:inwardNormals:geometryType:allocator:");
		}

		internal MDLMesh (Vector3 extent, Vector2i segments, bool inwardNormals, bool topCap, bool bottomCap, MDLGeometryType geometryType, IMDLMeshBufferAllocator allocator)
		{
			InitializeHandle (InitCylinder (extent, segments, inwardNormals, topCap, bottomCap, geometryType, allocator), "initCylinderWithExtent:segments:inwardNormals:topCap:bottomCap:geometryType:allocator:");
		}

		internal MDLMesh (Vector3 extent, Vector2i segments, MDLGeometryType geometryType, IMDLMeshBufferAllocator allocator)
		{
			InitializeHandle (InitPlane (extent, segments, geometryType, allocator), "initPlaneWithExtent:segments:geometryType:allocator:");
		}

		internal MDLMesh (Vector3 extent, bool inwardNormals, MDLGeometryType geometryType, IMDLMeshBufferAllocator allocator)
		{
			InitializeHandle (InitIcosahedron (extent, inwardNormals, geometryType, allocator), "initIcosahedronWithExtent:inwardNormals:geometryType:allocator:");
		}

		internal MDLMesh (MDLMesh mesh, int submeshIndex, uint subdivisionLevels, IMDLMeshBufferAllocator allocator)
		{
			InitializeHandle (InitMesh (mesh, submeshIndex, subdivisionLevels, allocator), "initMeshBySubdividingMesh:submeshIndex:subdivisionLevels:allocator:");
		}

		// Note: we turn these constructors into static constructors because we don't want to lose the shape name. Also, the signatures of these constructors differ so it would not be possible to use an enum to differentiate the shapes.

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreateBox (Vector3 dimensions, Vector3i segments, MDLGeometryType geometryType, bool inwardNormals, IMDLMeshBufferAllocator allocator)
		{
			return CreateBox (dimensions, segments, geometryType, inwardNormals, allocator, MDLMeshVectorType.Dimensions);
		}

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreateBox (Vector3 vector, Vector3i segments, MDLGeometryType geometryType, bool inwardNormals, IMDLMeshBufferAllocator allocator, MDLMeshVectorType type = MDLMeshVectorType.Dimensions)
		{
			switch (type) {
			case MDLMeshVectorType.Dimensions:
				return NewBoxWithDimensions (vector, segments, geometryType, inwardNormals, allocator);
			case MDLMeshVectorType.Extent:
				return new MDLMesh (vector, segments, inwardNormals, geometryType, allocator);
			default:
				throw new ArgumentException ("The 'MDLMeshVectorType type' argument needs a value.");
			}
		}

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreateSphere (Vector3 dimensions, Vector2i segments, MDLGeometryType geometryType, bool inwardNormals, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (dimensions, segments, inwardNormals, geometryType, allocator, null, null, null);
		}

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreateHemisphere (Vector3 dimensions, Vector2i segments, MDLGeometryType geometryType, bool inwardNormals, bool cap, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (dimensions, segments, inwardNormals, geometryType, allocator, null, cap, false);
		}

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreateCylinder (Vector3 extent, Vector2i segments, bool inwardNormals, bool topCap, bool bottomCap, MDLGeometryType geometryType, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (extent, segments, inwardNormals, topCap, bottomCap, geometryType, allocator);
		}

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreateCapsule (Vector3 dimensions, Vector2i segments, MDLGeometryType geometryType, bool inwardNormals, int hemisphereSegments, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (dimensions, segments, inwardNormals, geometryType, allocator, hemisphereSegments, null, null);
		}

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreateCone (Vector3 dimensions, Vector2i segments, MDLGeometryType geometryType, bool inwardNormals, bool cap, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (dimensions, segments, inwardNormals, geometryType, allocator, null, cap, true);
		}

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreatePlane (Vector3 extent, Vector2i segments, MDLGeometryType geometryType, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (extent, segments, geometryType, allocator);
		}

		[iOS (10,0)][TV (10,0)][Mac (10,12)]
		public static MDLMesh CreateIcosahedron (Vector3 extent, bool inwardNormals, MDLGeometryType geometryType, IMDLMeshBufferAllocator allocator)
		{
			return new MDLMesh (extent, inwardNormals, geometryType, allocator);
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