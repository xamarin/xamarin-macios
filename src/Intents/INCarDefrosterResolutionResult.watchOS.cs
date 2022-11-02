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
	[Register("INCarDefrosterResolutionResult", true)]
	[Obsolete (Constants.UnavailableOnWatchOS)]
	public unsafe partial class INCarDefrosterResolutionResult : INIntentResolutionResult {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS); } }

		protected INCarDefrosterResolutionResult (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		protected internal INCarDefrosterResolutionResult (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		public new static INCarDefrosterResolutionResult NeedsValue {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}

		public new static INCarDefrosterResolutionResult NotRequired {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}

		public new static INCarDefrosterResolutionResult Unsupported {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}
	} /* class INCarDefrosterResolutionResult */
}
#endif // __WATCHOS__ && !NET
