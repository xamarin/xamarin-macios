using System;
using Foundation;
using ObjCRuntime;

namespace NS {
	[Introduced (PlatformName.iOS, 9, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Introduced (PlatformName.MacOSX, 13, 0)]
	[Introduced (PlatformName.MacCatalyst, 15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[Sealed]
	interface ISomething {
		[NoTV]
		[Introduced (PlatformName.MacCatalyst, 15, 0)]
		[Export ("microphoneEnabled", ArgumentSemantic.Assign)]
		bool MicrophoneEnabled {
			[Bind ("isMicrophoneEnabled")]
			get;
			[Introduced (PlatformName.iOS, 10, 0)]
			[Introduced (PlatformName.MacCatalyst, 15, 0)]
			set;
		}
	}
}
