using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

#nullable enable

namespace SensorKit {

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoWatch]
	[NoTV]
	[NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
#endif
	public static class SRAbsoluteTime {

		[DllImport (Constants.SensorKitLibrary, EntryPoint = "SRAbsoluteTimeGetCurrent")]
		public static extern /* SRAbsoluteTime */ double GetCurrent ();

		[DllImport (Constants.SensorKitLibrary, EntryPoint = "SRAbsoluteTimeFromCFAbsoluteTime")]
		public static extern /* SRAbsoluteTime */ double FromCFAbsoluteTime (/* CFAbsoluteTime */ double cfAbsoluteTime);

		[DllImport (Constants.SensorKitLibrary, EntryPoint = "SRAbsoluteTimeToCFAbsoluteTime")]
		public static extern /* CFAbsoluteTime */ double ToCFAbsoluteTime (double srAbsoluteTime);

		[DllImport (Constants.SensorKitLibrary, EntryPoint = "SRAbsoluteTimeFromContinuousTime")]
		public static extern /* SRAbsoluteTime */ double FromContinuousTime (ulong continuousTime);
	}
}
