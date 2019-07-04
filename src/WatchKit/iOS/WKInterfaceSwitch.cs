#if __IOS__
using System;
using System.ComponentModel;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKInterfaceSwitch", SkipRegistration = true)]
	[Introduced (PlatformName.iOS, 8,2, PlatformArchitecture.All)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("The WatchKit framework has been removed from iOS")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceSwitch : WKInterfaceObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS"); } }

		protected WKInterfaceSwitch (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		protected internal WKInterfaceSwitch (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetColor (global::UIKit.UIColor color)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetEnabled (bool enabled)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetOn (bool on)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetTitle (string title)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetTitle (NSAttributedString attributedTitle)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}
	} /* class WKInterfaceSwitch */
}
#endif // __IOS__
