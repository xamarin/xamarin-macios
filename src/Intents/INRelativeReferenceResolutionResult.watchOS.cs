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
	[Register("INRelativeReferenceResolutionResult", true)]
	[Obsolete (Constants.UnavailableOnWatchOS)]
	public unsafe partial class INRelativeReferenceResolutionResult : INIntentResolutionResult {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS); } }

		protected INRelativeReferenceResolutionResult (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		protected internal INRelativeReferenceResolutionResult (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		public new static INRelativeReferenceResolutionResult NeedsValue {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}

		public new static INRelativeReferenceResolutionResult NotRequired {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}

		public new static INRelativeReferenceResolutionResult Unsupported {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}
	} /* class INRelativeReferenceResolutionResult */
}
#endif // __WATCHOS__ && !NET
