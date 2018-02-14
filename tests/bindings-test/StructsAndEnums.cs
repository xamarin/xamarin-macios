using System;
using System.Runtime.InteropServices;

#if !__UNIFIED__
using nint=System.Int32;
#else
using Foundation;
using ObjCRuntime;
#endif

using MatrixFloat2x2 = global::OpenTK.NMatrix2;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using MatrixFloat4x3 = global::OpenTK.NMatrix4x3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;

namespace Bindings.Test
{
	public static class CFunctions {
		[DllImport ("__Internal")]
		public static extern int theUltimateAnswer ();

		[DllImport ("__Internal")]
		public static extern void x_call_block (ref BlockLiteral block);

		[DllImport ("__Internal")]
		public static extern void x_get_matrix_float2x2 (IntPtr self, string sel, out float r0c0, out float r0c1, out float r1c0, out float r1c1);

		[DllImport ("__Internal")]
		public static extern void x_get_matrix_float3x3 (IntPtr self, string sel, out float r0c0, out float r0c1, out float r0c2, out float r1c0, out float r1c1, out float r1c2, out float r2c0, out float r2c1, out float r2c2);

		[DllImport ("__Internal")]
		public static extern void x_get_matrix_float4x4 (IntPtr self, string sel, out float r0c0, out float r0c1, out float r0c2, out float r0c3, out float r1c0, out float r1c1, out float r1c2, out float r1c3, out float r2c0, out float r2c1, out float r2c2, out float r2c3, out float r3c0, out float r3c1, out float r3c2, out float r3c3);

		[DllImport ("__Internal")]
		public static extern void x_get_matrix_float4x3 (IntPtr self, string sel, out float r0c0, out float r0c1, out float r0c2, out float r0c3, out float r1c0, out float r1c1, out float r1c2, out float r1c3, out float r2c0, out float r2c1, out float r2c2, out float r2c3);

		public static MatrixFloat2x2 GetMatrixFloat2x2 (NSObject obj, string selector)
		{
			float r0c0, r0c1, r1c0, r1c1;
			x_get_matrix_float2x2 (obj.Handle, selector, out r0c0, out r0c1, out r1c0, out r1c1);
			return new MatrixFloat2x2 (
				r0c0, r0c1,
				r1c0, r1c1);
		}

		public static MatrixFloat3x3 GetMatrixFloat3x3 (NSObject obj, string selector)
		{
			float r0c0, r0c1, r0c2, r1c0, r1c1, r1c2, r2c0, r2c1, r2c2;
			x_get_matrix_float3x3 (obj.Handle, selector, out r0c0, out r0c1, out r0c2, out r1c0, out r1c1, out r1c2, out r2c0, out r2c1, out r2c2);
			return new MatrixFloat3x3 (
				r0c0, r0c1, r0c2,
				r1c0, r1c1, r1c2,
				r2c0, r2c1, r2c2);

		}

		public static MatrixFloat4x4 GetMatrixFloat4x4 (NSObject obj, string selector)
		{
			float r0c0, r0c1, r0c2, r0c3, r1c0, r1c1, r1c2, r1c3, r2c0, r2c1, r2c2, r2c3, r3c0, r3c1, r3c2, r3c3;
			x_get_matrix_float4x4 (obj.Handle, selector, out r0c0, out r0c1, out r0c2, out r0c3, out r1c0, out r1c1, out r1c2, out r1c3, out r2c0, out r2c1, out r2c2, out r2c3, out r3c0, out r3c1, out r3c2, out r3c3);
			return new MatrixFloat4x4 (
				r0c0, r0c1, r0c2, r0c3,
				r1c0, r1c1, r1c2, r1c3,
				r2c0, r2c1, r2c2, r2c3,
				r3c0, r3c1, r3c2, r3c3);
		}

		public static MatrixFloat4x3 GetMatrixFloat4x3 (NSObject obj, string selector)
		{
			float r0c0, r0c1, r0c2, r0c3, r1c0, r1c1, r1c2, r1c3, r2c0, r2c1, r2c2, r2c3;
			x_get_matrix_float4x3 (obj.Handle, selector, out r0c0, out r0c1, out r0c2, out r0c3, out r1c0, out r1c1, out r1c2, out r1c3, out r2c0, out r2c1, out r2c2, out r2c3);
			return new MatrixFloat4x3 (
				r0c0, r0c1, r0c2, r0c3,
				r1c0, r1c1, r1c2, r1c3,
				r2c0, r2c1, r2c2, r2c3);
		}

#if !__WATCHOS__
		[DllImport ("__Internal")]
		public static extern void x_mdltransformcomponent_get_local_transform (IntPtr self, double time, out float r0c0, out float r0c1, out float r0c2, out float r0c3, out float r1c0, out float r1c1, out float r1c2, out float r1c3, out float r2c0, out float r2c1, out float r2c2, out float r2c3, out float r3c0, out float r3c1, out float r3c2, out float r3c3);

		public static MatrixFloat4x4 MDLTransformComponent_GetLocalTransform (INativeObject obj, double time)
		{
			float r0c0, r0c1, r0c2, r0c3, r1c0, r1c1, r1c2, r1c3, r2c0, r2c1, r2c2, r2c3, r3c0, r3c1, r3c2, r3c3;
			x_mdltransformcomponent_get_local_transform (obj.Handle, time, out r0c0, out r0c1, out r0c2, out r0c3, out r1c0, out r1c1, out r1c2, out r1c3, out r2c0, out r2c1, out r2c2, out r2c3, out r3c0, out r3c1, out r3c2, out r3c3);
			return new MatrixFloat4x4 (
				r0c0, r0c1, r0c2, r0c3,
				r1c0, r1c1, r1c2, r1c3,
				r2c0, r2c1, r2c2, r2c3,
				r3c0, r3c1, r3c2, r3c3);
		}

		[DllImport ("__Internal")]
		public static extern void x_mdltransform_create_global_transform (IntPtr obj, double time, out float r0c0, out float r0c1, out float r0c2, out float r0c3, out float r1c0, out float r1c1, out float r1c2, out float r1c3, out float r2c0, out float r2c1, out float r2c2, out float r2c3, out float r3c0, out float r3c1, out float r3c2, out float r3c3);

		public static MatrixFloat4x4 MDLTransform_CreateGlobalTransform (INativeObject obj, double time)
		{
			float r0c0, r0c1, r0c2, r0c3, r1c0, r1c1, r1c2, r1c3, r2c0, r2c1, r2c2, r2c3, r3c0, r3c1, r3c2, r3c3;
			x_mdltransform_create_global_transform (obj.Handle, time, out r0c0, out r0c1, out r0c2, out r0c3, out r1c0, out r1c1, out r1c2, out r1c3, out r2c0, out r2c1, out r2c2, out r2c3, out r3c0, out r3c1, out r3c2, out r3c3);
			return new MatrixFloat4x4 (
				r0c0, r0c1, r0c2, r0c3,
				r1c0, r1c1, r1c2, r1c3,
				r2c0, r2c1, r2c2, r2c3,
				r3c0, r3c1, r3c2, r3c3);
		}

		[DllImport ("__Internal")]
		public static extern void x_mdltransform_get_rotation_matrix (IntPtr obj, double time, out float r0c0, out float r0c1, out float r0c2, out float r0c3, out float r1c0, out float r1c1, out float r1c2, out float r1c3, out float r2c0, out float r2c1, out float r2c2, out float r2c3, out float r3c0, out float r3c1, out float r3c2, out float r3c3);

		public static MatrixFloat4x4 MDLTransform_GetRotationMatrix (INativeObject obj, double time)
		{
			float r0c0, r0c1, r0c2, r0c3, r1c0, r1c1, r1c2, r1c3, r2c0, r2c1, r2c2, r2c3, r3c0, r3c1, r3c2, r3c3;
			x_mdltransform_get_rotation_matrix (obj.Handle, time, out r0c0, out r0c1, out r0c2, out r0c3, out r1c0, out r1c1, out r1c2, out r1c3, out r2c0, out r2c1, out r2c2, out r2c3, out r3c0, out r3c1, out r3c2, out r3c3);
			return new MatrixFloat4x4 (
				r0c0, r0c1, r0c2, r0c3,
				r1c0, r1c1, r1c2, r1c3,
				r2c0, r2c1, r2c2, r2c3,
				r3c0, r3c1, r3c2, r3c3);
		}
#endif
	}
}

