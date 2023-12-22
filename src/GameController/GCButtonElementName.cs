#if !WATCH

using Foundation;
using ObjCRuntime;
using System;
using System.Runtime.InteropServices;

#nullable enable

namespace GameController {

	public class GCButtonElementName {

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[TV (16,0), Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
#endif
		[DllImport (Constants.GameControllerLibrary)]
		static extern /* GCButtonElementName */ IntPtr GCInputArcadeButtonName (nint row, nint column);

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[TV (16,0), Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
#endif
		public static NSString? GetArcadeButtonName (nint row, nint column)
			=> Runtime.GetNSObject<NSString> (GCInputArcadeButtonName (row, column));

	}

}
#endif
