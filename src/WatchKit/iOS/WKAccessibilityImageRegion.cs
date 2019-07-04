#if __IOS__
using System;
using System.ComponentModel;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKAccessibilityImageRegion", SkipRegistration = true)]
	public class WKAccessibilityImageRegion : NSObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS"); } }

		public WKAccessibilityImageRegion () : base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		protected WKAccessibilityImageRegion (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		protected internal WKAccessibilityImageRegion (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual CGRect Frame {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string Label {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
	} /* class WKAccessibilityImageRegion */
}
#endif // __IOS__
