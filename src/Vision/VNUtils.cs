//
// VNUtils.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using ObjCRuntime;
using Foundation;

using Vector2 = global::OpenTK.Vector2;

namespace Vision {
	[TV (11,0), Mac (10,13), iOS (11,0)]
	public static partial class VNUtils {
	
		// initialized only once (see tests/cecil-tests/)
		[Field ("VNNormalizedIdentityRect", Constants.VisionLibrary)]
		public static CGRect NormalizedIdentityRect { get; } = Dlfcn.GetCGRect (Libraries.Vision.Handle, "VNNormalizedIdentityRect");

		[DllImport (Constants.VisionLibrary, EntryPoint = "VNNormalizedRectIsIdentityRect")]
		public static extern bool IsIdentityRect (CGRect rect);

		[DllImport (Constants.VisionLibrary, EntryPoint = "VNImagePointForNormalizedPoint")]
		public static extern CGPoint GetImagePoint (CGPoint normalizedPoint, nuint imageWidth, nuint imageHeight);

		[TV (14,0), Mac (11,0), iOS (14,0)]
		[DllImport (Constants.VisionLibrary, EntryPoint = "VNNormalizedPointForImagePoint")]
		public static extern CGPoint GetNormalizedPoint (CGPoint imagePoint, nuint imageWidth, nuint imageHeight);

		[DllImport (Constants.VisionLibrary, EntryPoint = "VNImageRectForNormalizedRect")]
		public static extern CGRect GetImageRect (CGRect normalizedRect, nuint imageWidth, nuint imageHeight);

		[DllImport (Constants.VisionLibrary, EntryPoint = "VNNormalizedRectForImageRect")]
		public static extern CGRect GetNormalizedRect (CGRect imageRect, nuint imageWidth, nuint imageHeight);

		[DllImport ("__Internal", EntryPoint = "xamarin_CGPoint__VNNormalizedFaceBoundingBoxPointForLandmarkPoint_Vector2_CGRect_nuint_nuint_string")]
		static extern CGPoint VNNormalizedFaceBoundingBoxPointForLandmarkPoint (Vector2 faceLandmarkPoint, CGRect faceBoundingBox, nuint imageWidth, nuint imageHeight, out IntPtr error);

		public static CGPoint GetNormalizedFaceBoundingBoxPoint (Vector2 faceLandmarkPoint, CGRect faceBoundingBox, nuint imageWidth, nuint imageHeight)
		{
			IntPtr error;
			var result = VNNormalizedFaceBoundingBoxPointForLandmarkPoint (faceLandmarkPoint, faceBoundingBox, imageWidth, imageHeight, out error);
			if (error != IntPtr.Zero)
				throw new InvalidOperationException (Marshal.PtrToStringAuto (error));

			return result;
		}

		[DllImport ("__Internal", EntryPoint = "xamarin_CGPoint__VNImagePointForFaceLandmarkPoint_Vector2_CGRect_nuint_nuint_string")]
		static extern CGPoint VNImagePointForFaceLandmarkPoint (Vector2 faceLandmarkPoint, CGRect faceBoundingBox, nuint imageWidth, nuint imageHeight, out IntPtr error);

		public static CGPoint GetImagePoint (Vector2 faceLandmarkPoint, CGRect faceBoundingBox, nuint imageWidth, nuint imageHeight)
		{
			IntPtr error;
			var result = VNImagePointForFaceLandmarkPoint (faceLandmarkPoint, faceBoundingBox, imageWidth, imageHeight, out error);
			if (error != IntPtr.Zero)
				throw new InvalidOperationException (Marshal.PtrToStringAuto (error));

			return result;
		}

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.VisionLibrary)]
		static extern nuint VNElementTypeSize (nuint elementType);

		[TV (13,0), Mac (10,15), iOS (13,0)]
		public static nuint GetElementTypeSize (VNElementType elementType) => VNElementTypeSize ((nuint) (ulong) elementType);
	}

	public partial class VNGeometryUtils {

		public static VNCircle CreateBoundingCircle (Vector2 [] points, out NSError error)
		{
			if (points == null)
				throw new ArgumentNullException (nameof (points));
			if (points.Length == 0)
				throw new ArgumentException ($"{points} array must have more than zero elements.");

			unsafe {
				fixed (Vector2* points_ptr = points)
					return CreateBoundingCircle ((IntPtr) points_ptr, points.Length, out error);
			}
		}
	}
}
