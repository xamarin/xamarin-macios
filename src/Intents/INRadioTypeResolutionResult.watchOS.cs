#if __WATCHOS__ && !XAMCORE_4_0
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
	[Obsolete ("This class is not available on watchOS")]
	public unsafe partial class INRadioTypeResolutionResult : INIntentResolutionResult {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException ("This class is not supported on watchOS"); } }

		protected INRadioTypeResolutionResult (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
		}

		protected internal INRadioTypeResolutionResult (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
		}

		public new static INRadioTypeResolutionResult NeedsValue {
			get {
				throw new PlatformNotSupportedException ("This class is not supported on watchOS");
			}
		}

		public new static INRadioTypeResolutionResult NotRequired {
			get {
				throw new PlatformNotSupportedException ("This class is not supported on watchOS");
			}
		}

		public new static INRadioTypeResolutionResult Unsupported {
			get {
				throw new PlatformNotSupportedException ("This class is not supported on watchOS");
			}
		}
	} /* class INRadioTypeResolutionResult */
}
#endif // __WATCHOS__ && !XAMCORE_4_0
