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
using System.Runtime.Versioning;
using CoreGraphics;
using ObjCRuntime;
using Foundation;


#if NET
using Vector2 = global::System.Numerics.Vector2;
#else
using Vector2 = global::OpenTK.Vector2;
#endif

namespace Vision {

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[TV (11,0)]
	[Mac (10,13)]
	[iOS (11,0)]
#endif
	public static partial class VNUtils {
	
		// initialized only once (see tests/cecil-tests/)
		[Field ("VNNormalizedIdentityRect", Constants.VisionLibrary)]
		public static CGRect NormalizedIdentityRect { get; } = Dlfcn.GetCGRect (Libraries.Vision.Handle, "VNNormalizedIdentityRect");

		[DllImport (Constants.VisionLibrary, EntryPoint = "VNNormalizedRectIsIdentityRect")]
		[return: MarshalAs (UnmanagedType.U1)]
		public static extern bool IsIdentityRect (CGRect rect);

		[DllImport (Constants.VisionLibrary, EntryPoint = "VNImagePointForNormalizedPoint")]
		public static extern CGPoint GetImagePoint (CGPoint normalizedPoint, nuint imageWidth, nuint imageHeight);

#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[TV (14,0)]
		[Mac (11,0)]
		[iOS (14,0)]
		[MacCatalyst (14,0)]
#endif
		[DllImport (Constants.VisionLibrary, EntryPoint = "VNNormalizedPointForImagePoint")]
		public static extern CGPoint GetNormalizedPoint (CGPoint imagePoint, nuint imageWidth, nuint imageHeight);

		[DllImport (Constants.VisionLibrary, EntryPoint = "VNImageRectForNormalizedRect")]
		public static extern CGRect GetImageRect (CGRect normalizedRect, nuint imageWidth, nuint imageHeight);

		[DllImport (Constants.VisionLibrary, EntryPoint = "VNNormalizedRectForImageRect")]
		public static extern CGRect GetNormalizedRect (CGRect imageRect, nuint imageWidth, nuint imageHeight);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		[DllImport (Constants.VisionLibrary, EntryPoint="VNImagePointForNormalizedPointUsingRegionOfInterest")]
		public static extern CGPoint GetImagePoint (CGPoint normalizedPoint, nuint imageWidth, nuint imageHeight, CGRect regionOfInterest);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		[DllImport (Constants.VisionLibrary, EntryPoint="VNNormalizedPointForImagePointUsingRegionOfInterest")]
		public static extern CGPoint GetNormalizedPoint (CGPoint imagePoint, nuint imageWidth, nuint imageHeight, CGRect regionOfInterest);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		[DllImport (Constants.VisionLibrary, EntryPoint="VNImageRectForNormalizedRectUsingRegionOfInterest")]
		public static extern CGRect GetImageRect (CGRect normalizedRect, nuint imageWidth, nuint imageHeight, CGRect regionOfInterest);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[TV (15,0)]
		[Mac (12,0)]
		[iOS (15,0)]
		[MacCatalyst (15,0)]
#endif
		[DllImport (Constants.VisionLibrary, EntryPoint="VNNormalizedRectForImageRectUsingRegionOfInterest")]
		public static extern CGRect GetNormalizedRect (CGRect imageRect, nuint imageWidth, nuint imageHeight, CGRect regionOfInterest);

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

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.VisionLibrary)]
		static extern nuint VNElementTypeSize (nuint elementType);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public static nuint GetElementTypeSize (VNElementType elementType) => VNElementTypeSize ((nuint) (ulong) elementType);
	}

	public partial class VNGeometryUtils {

		public static VNCircle CreateBoundingCircle (Vector2 [] points, out NSError error)
		{
			if (points == null)
				throw new ArgumentNullException (nameof (points));
			if (points.Length == 0)
				throw new ArgumentException ($"'{nameof (points)}' array must have more than zero elements.");

			unsafe {
				fixed (Vector2* points_ptr = points)
					return CreateBoundingCircle ((IntPtr) points_ptr, points.Length, out error);
			}
		}
	}
}
