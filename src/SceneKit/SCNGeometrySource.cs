//
// SCNGeometrySource.cs: extensions to provide an array-based API that
// we pass as pointers
//
// Authors:
//   MIguel de Icaza (miguel@xamarin.com)
//
// Copyright Xamarin Inc
//
using System;

using CoreGraphics;
using Foundation;
#if !WATCH
using Metal;
#endif

#nullable enable

namespace SceneKit {
	public partial class SCNGeometrySource {

		public static unsafe SCNGeometrySource FromVertices (SCNVector3 [] vertices)
		{
			if (vertices is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (vertices));

			fixed (SCNVector3* ptr = &vertices [0])
				return FromVertices ((IntPtr) ptr, vertices.Length);
		}

		public static unsafe SCNGeometrySource FromNormals (SCNVector3 [] normals)
		{
			if (normals is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (normals));

			fixed (SCNVector3* ptr = &normals [0])
				return FromNormals ((IntPtr) ptr, normals.Length);
		}

		public static unsafe SCNGeometrySource FromTextureCoordinates (CGPoint [] texcoords)
		{
			if (texcoords is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (texcoords));

			fixed (CGPoint* ptr = &texcoords [0])
				return FromTextureCoordinates ((IntPtr) ptr, texcoords.Length);
		}

		static NSString SemanticToToken (SCNGeometrySourceSemantics geometrySourceSemantic)
		{
			switch (geometrySourceSemantic) {
			case SCNGeometrySourceSemantics.Vertex:
				return SCNGeometrySourceSemantic.Vertex;
			case SCNGeometrySourceSemantics.Normal:
				return SCNGeometrySourceSemantic.Normal;
			case SCNGeometrySourceSemantics.Color:
				return SCNGeometrySourceSemantic.Color;
			case SCNGeometrySourceSemantics.Texcoord:
				return SCNGeometrySourceSemantic.Texcoord;
			case SCNGeometrySourceSemantics.VertexCrease:
				return SCNGeometrySourceSemantic.VertexCrease;
			case SCNGeometrySourceSemantics.EdgeCrease:
				return SCNGeometrySourceSemantic.EdgeCrease;
			case SCNGeometrySourceSemantics.BoneWeights:
				return SCNGeometrySourceSemantic.BoneWeights;
			case SCNGeometrySourceSemantics.BoneIndices:
				return SCNGeometrySourceSemantic.BoneIndices;
			default:
				throw new System.ArgumentException ("geometrySourceSemantic");
			}
		}

		public static SCNGeometrySource FromData (NSData data, SCNGeometrySourceSemantics semantic, nint vectorCount, bool floatComponents, nint componentsPerVector, nint bytesPerComponent, nint offset, nint stride)
		{
			return FromData (data, SemanticToToken (semantic), vectorCount, floatComponents, componentsPerVector, bytesPerComponent, offset, stride);
		}

#if !WATCH
		public static SCNGeometrySource FromMetalBuffer (IMTLBuffer mtlBuffer, MTLVertexFormat vertexFormat, SCNGeometrySourceSemantics semantic, nint vertexCount, nint offset, nint stride)
		{
			return FromMetalBuffer (mtlBuffer, vertexFormat, SemanticToToken (semantic), vertexCount, offset, stride);
		}
#endif
	}

}
