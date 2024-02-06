using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace CoreAnimation {

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[Watch (8, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CAFrameRateRange {
		public float Minimum;

		public float Maximum;

		public float Preferred;

		[DllImport (Constants.QuartzLibrary, EntryPoint = "CAFrameRateRangeIsEqualToRange")]
		static extern byte IsEqualTo (CAFrameRateRange range, CAFrameRateRange other);

		[DllImport (Constants.QuartzLibrary, EntryPoint = "CAFrameRateRangeMake")]
		public static extern CAFrameRateRange Create (float minimum, float maximum, float preferred);

		public bool IsEqualTo (CAFrameRateRange other)
			=> IsEqualTo (this, other) != 0;

#if !COREBUILD
		[Field ("CAFrameRateRangeDefault", "CoreAnimation")]
		public static CAFrameRateRange Default => Marshal.PtrToStructure<CAFrameRateRange> (Dlfcn.GetIndirect (Libraries.CoreAnimation.Handle, "CAFrameRateRangeDefault"))!;
#endif

	}

}
