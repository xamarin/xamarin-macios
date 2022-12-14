#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKAccessibilityImageRegion", SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKAccessibilityImageRegion : NSObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.WatchKitRemoved); } }

		public WKAccessibilityImageRegion () : base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected WKAccessibilityImageRegion (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected internal WKAccessibilityImageRegion (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual CGRect Frame {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual string Label {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
	} /* class WKAccessibilityImageRegion */
}
#endif // __IOS__ && !NET
