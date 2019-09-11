#if __IOS__
using System;
using System.ComponentModel;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("The WatchKit framework has been removed from iOS")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public interface IWKImageAnimatable : INativeObject, IDisposable
	{
		[Preserve (Conditional = true)]
		void StartAnimating ();

		[Preserve (Conditional = true)]
		void StartAnimating (NSRange imageRange, double duration, nint repeatCount);

		[Preserve (Conditional = true)]
		void StopAnimating ();
	}
}
#endif // __IOS__
