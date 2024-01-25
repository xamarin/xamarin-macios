using System;

using Foundation;

using ObjCRuntime;

namespace NS {
	[Introduced (PlatformName.iOS, 9, 0)]
	[Introduced (PlatformName.MacOSX, 10, 11)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface ISomething {
		[Introduced (PlatformName.MacCatalyst, 13, 0)] // needed since it's not in iOS
		[Introduced (PlatformName.MacOSX, 10, 15)]
		[NoWatch, NoTV, NoiOS]
		[Export ("isLoadedInProcess")]
		bool IsLoadedInProcess { get; }
	}
}
