using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Runtime.Versioning;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace CoreAnimation {

#if !NET
	[Watch (8,0), TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
#else
	[SupportedOSPlatform ("ios15.0"), SupportedOSPlatform ("tvos15.0"), SupportedOSPlatform ("macos12.0"), SupportedOSPlatform ("maccatalyst15.0")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CAFrameRateRange
	{

		public float Minimum;
		public float Maximum;
		public float Preferred;

		public CAFrameRateRange (float min, float max, float preferred) {
			Minimum = min;
			Maximum = max;
			Preferred = preferred;
		}

#if !COREBUILD
		[Field ("CAFrameRateRangeDefault", "CoreAnimation")]
		public static CAFrameRateRange Default
			=> (CAFrameRateRange) Marshal.PtrToStructure (Dlfcn.GetIndirect (Libraries.CoreAnimation.Handle, "CAFrameRateRangeDefault"), typeof (CAFrameRateRange))!;
#endif

		[DllImport (Constants.CoreAnimationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CAFrameRateRangeIsEqualToRange (CAFrameRateRange range, CAFrameRateRange other);

		public bool IsEqualTo (CAFrameRateRange other)
			=> CAFrameRateRangeIsEqualToRange  (this, other);

	}
}
