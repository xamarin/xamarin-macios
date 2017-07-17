//
// VNUtils.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using System.Runtime.InteropServices;
using XamCore.CoreGraphics;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

using Vector2 = global::OpenTK.Vector2;

namespace XamCore.Vision {
	[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	public static partial class VNUtils {
	
		[Field ("VNNormalizedIdentityRect", Constants.VisionLibrary)]
		public static CGRect NormalizedIdentityRect { get; } = Dlfcn.GetCGRect (Libraries.Vision.Handle, "VNNormalizedIdentityRect");

		[DllImport (Constants.VisionLibrary)]
		static extern bool VNNormalizedRectIsIdentityRect (CGRect rect);

		public static bool IsIdentityRect (CGRect normalizedRect) => VNNormalizedRectIsIdentityRect (normalizedRect);

		[DllImport (Constants.VisionLibrary)]
		static extern CGPoint VNImagePointForNormalizedPoint (CGPoint normalizedPoint, nuint imageWidth, nuint imageHeight);

		public static CGPoint GetImagePoint (CGPoint normalizedPoint, nuint imageWidth, nuint imageHeight) => VNImagePointForNormalizedPoint (normalizedPoint, imageWidth, imageHeight);

		[DllImport (Constants.VisionLibrary)]
		static extern CGRect VNImageRectForNormalizedRect (CGRect normalizedRect, nuint imageWidth, nuint imageHeight);

		public static CGRect GetImageRect (CGRect normalizedRect, nuint imageWidth, nuint imageHeight) => VNImageRectForNormalizedRect (normalizedRect, imageWidth, imageHeight);

		[DllImport (Constants.VisionLibrary)]
		static extern CGRect VNNormalizedRectForImageRect (CGRect imageRect, nuint imageWidth, nuint imageHeight);

		public static CGRect GetNormalizedRect (CGRect imageRect, nuint imageWidth, nuint imageHeight) => VNNormalizedRectForImageRect (imageRect, imageWidth, imageHeight);

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
	}
}
#endif
