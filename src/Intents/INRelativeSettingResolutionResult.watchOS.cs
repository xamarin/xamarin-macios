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
	[Register("INRelativeSettingResolutionResult", true)]
	[Obsolete ("This class is not available on watchOS")]
	public unsafe partial class INRelativeSettingResolutionResult : INIntentResolutionResult {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException ("This class is not supported on watchOS"); } }

		protected INRelativeSettingResolutionResult (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
		}

		protected internal INRelativeSettingResolutionResult (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
		}

		public new static INRelativeSettingResolutionResult NeedsValue {
			get {
				throw new PlatformNotSupportedException ("This class is not supported on watchOS");
			}
		}

		public new static INRelativeSettingResolutionResult NotRequired {
			get {
				throw new PlatformNotSupportedException ("This class is not supported on watchOS");
			}
		}

		public new static INRelativeSettingResolutionResult Unsupported {
			get {
				throw new PlatformNotSupportedException ("This class is not supported on watchOS");
			}
		}
	} /* class INRelativeSettingResolutionResult */
}
#endif // __WATCHOS__ && !XAMCORE_4_0
