#if __WATCHOS__ && !NET
using System;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UIKit;
using SceneKit;
using Contacts;
using CoreVideo;
using SpriteKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreLocation;
using AVFoundation;
using CoreFoundation;

namespace Intents {
	[Register("INRadioTypeResolutionResult", true)]
	[Obsolete (Constants.UnavailableOnWatchOS)]
	public unsafe partial class INRadioTypeResolutionResult : INIntentResolutionResult {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS); } }

		protected INRadioTypeResolutionResult (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		protected internal INRadioTypeResolutionResult (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		public new static INRadioTypeResolutionResult NeedsValue {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}

		public new static INRadioTypeResolutionResult NotRequired {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}

		public new static INRadioTypeResolutionResult Unsupported {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}
	} /* class INRadioTypeResolutionResult */
}
#endif // __WATCHOS__ && !NET
