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
}
