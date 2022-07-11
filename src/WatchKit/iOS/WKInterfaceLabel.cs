#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKInterfaceLabel", SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceLabel : WKInterfaceObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.WatchKitRemoved); } }

		protected WKInterfaceLabel (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected internal WKInterfaceLabel (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetText (string text)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetText (NSAttributedString attributedText)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetTextColor (global::UIKit.UIColor color)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}
	} /* class WKInterfaceLabel */
}
#endif // __IOS__ && !NET
