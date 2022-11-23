#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKInterfaceImage", SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceImage : WKInterfaceObject, IWKImageAnimatable {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.WatchKitRemoved); } }

		protected WKInterfaceImage (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected internal WKInterfaceImage (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetImage (global::UIKit.UIImage image)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetImage (NSData imageData)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetImage (string imageName)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetTintColor (global::UIKit.UIColor color)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void StartAnimating ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void StartAnimating (NSRange imageRange, double duration, nint repeatCount)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void StopAnimating ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}
	} /* class WKInterfaceImage */
}
#endif // __IOS__ && !NET
