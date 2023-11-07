//
// NSSegmentedControl: Support for the NSSegmentedControl class
//
// Author:
//   Pavel Sich (pavel.sich@me.com)
//

#if !__MACCATALYST__

using System;
using ObjCRuntime;
using Foundation;

#nullable enable

namespace AppKit {

	public partial class NSSegmentedControl {
		NSActionDispatcher? dispatcher;

		public new NSSegmentedCell Cell {
			get { return (NSSegmentedCell) base.Cell; }
			set { base.Cell = value; }
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#endif
		public static NSSegmentedControl FromLabels (string [] labels, NSSegmentSwitchTracking trackingMode, Action action)
		{
			var dispatcher = new NSActionDispatcher (action);
			var control = _FromLabels (labels, trackingMode, dispatcher, NSActionDispatcher.Selector);
			control.dispatcher = dispatcher;
			return control;
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
#endif
		public static NSSegmentedControl FromImages (NSImage [] images, NSSegmentSwitchTracking trackingMode, Action action)
		{
			var dispatcher = new NSActionDispatcher (action);
			var control = _FromImages (images, trackingMode, dispatcher, NSActionDispatcher.Selector);
			control.dispatcher = dispatcher;
			return control;
		}

		public void UnselectAllSegments ()
		{
			NSSegmentSwitchTracking current = this.Cell.TrackingMode;
			this.Cell.TrackingMode = NSSegmentSwitchTracking.Momentary;

			for (nint i = 0; i < this.SegmentCount; i++)
				SetSelected (false, i);

			this.Cell.TrackingMode = current;
		}

	}
}
#endif // !__MACCATALYST__
