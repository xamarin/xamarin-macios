//
// NSSegmentedControl: Support for the NSSegmentedControl class
//
// Author:
//   Pavel Sich (pavel.sich@me.com)
//

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.AppKit {

	public partial class NSSegmentedControl {
		
		public new NSSegmentedCell Cell {
			get { return (NSSegmentedCell) base.Cell; }
			set { base.Cell = value; }
		}

		public void UnselectAllSegments()
		{
			NSSegmentSwitchTracking current = this.Cell.TrackingMode;
			this.Cell.TrackingMode = NSSegmentSwitchTracking.Momentary;
			
			for (nint i = 0; i < this.SegmentCount; i ++)
				SetSelected (false, i);

			this.Cell.TrackingMode = current;
		}

	}
}
