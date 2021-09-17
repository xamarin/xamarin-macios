#if __IOS__
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
#if !NET
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
#endif
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public interface IWKImageAnimatable : INativeObject, IDisposable
	{
#if NET
		[UnsupportedOSPlatform ("ios")]
#endif
		[Preserve (Conditional = true)]
		void StartAnimating ();

#if NET
		[UnsupportedOSPlatform ("ios")]
#endif
		[Preserve (Conditional = true)]
		void StartAnimating (NSRange imageRange, double duration, nint repeatCount);

#if NET
		[UnsupportedOSPlatform ("ios")]
#endif
		[Preserve (Conditional = true)]
		void StopAnimating ();
	}
}
#endif // __IOS__
