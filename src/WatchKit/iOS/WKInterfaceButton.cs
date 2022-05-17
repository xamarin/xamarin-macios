#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKInterfaceButton", SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceButton : WKInterfaceObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.WatchKitRemoved); } }

		protected WKInterfaceButton (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected internal WKInterfaceButton (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetBackgroundColor (global::UIKit.UIColor color)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetBackgroundImage (global::UIKit.UIImage image)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetBackgroundImage (NSData imageData)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetBackgroundImage (string imageName)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetEnabled (bool enabled)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetTitle (string title)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetTitle (NSAttributedString attributedTitle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

	} /* class WKInterfaceButton */
}
#endif // __IOS__ && !NET
