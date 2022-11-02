#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
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
#endif // __IOS__ && !NET
