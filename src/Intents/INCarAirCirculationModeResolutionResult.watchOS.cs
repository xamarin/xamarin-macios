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
	[Register("INCarAirCirculationModeResolutionResult", true)]
	[Obsolete (Constants.UnavailableOnWatchOS)]
	public unsafe partial class INCarAirCirculationModeResolutionResult : INIntentResolutionResult {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS); } }

		protected INCarAirCirculationModeResolutionResult (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		protected internal INCarAirCirculationModeResolutionResult (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		public static INCarAirCirculationModeResolutionResult ConfirmationRequiredWithCarAirCirculationModeToConfirm (INCarAirCirculationMode carAirCirculationModeToConfirm)
		{
			throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
		}

		public new static INCarAirCirculationModeResolutionResult NeedsValue {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}

		public new static INCarAirCirculationModeResolutionResult NotRequired {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}

		public new static INCarAirCirculationModeResolutionResult Unsupported {
			get {
				throw new PlatformNotSupportedException (Constants.UnavailableOnWatchOS);
			}
		}
	} /* class INCarAirCirculationModeResolutionResult */
}
#endif // __WATCHOS__ && !NET
