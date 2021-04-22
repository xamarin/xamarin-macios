#if __IOS__
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKAccessibilityImageRegion", SkipRegistration = true)]
#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
#endif
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
#endif // __IOS__
