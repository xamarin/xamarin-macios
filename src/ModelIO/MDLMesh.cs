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