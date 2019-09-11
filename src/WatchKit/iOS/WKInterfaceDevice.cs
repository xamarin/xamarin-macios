#if __IOS__
using System;
using System.Collections.Generic;
using System.ComponentModel;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace WatchKit {
	[Register ("WKInterfaceDevice", SkipRegistration = true)]
	[Introduced (PlatformName.iOS, 8,2, PlatformArchitecture.All)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("The WatchKit framework has been removed from iOS")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceDevice : NSObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS"); } }

		protected WKInterfaceDevice (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		protected internal WKInterfaceDevice (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual bool AddCachedImage (global::UIKit.UIImage image, string name)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual bool AddCachedImage (NSData imageData, string name)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void RemoveAllCachedImages ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void RemoveCachedImage (string name)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static WKInterfaceDevice CurrentDevice {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string LocalizedModel {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string Model {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string Name {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual CGRect ScreenBounds {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual nfloat ScreenScale {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string SystemName {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string SystemVersion {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual NSDictionary WeakCachedImages {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public IReadOnlyDictionary<string,long> CachedImages {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public UIContentSizeCategory PreferredContentSizeCategory {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public string PreferredContentSizeCategoryString {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public bool CheckSystemVersion (int major, int minor)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}
	} /* class WKInterfaceDevice */
}
#endif // __IOS__
