using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace TVMLKit {

	public class TVViewElementDispatchResult {

#if !COREBUILD
		public TVViewElementDispatchResult (bool isDispatched, bool isCancelled)
		{
			IsDispatched = isDispatched;
			IsCancelled = isCancelled;
		}

		public bool IsDispatched { get; set; }

		public bool IsCancelled { get; set; }
#endif
	}

	public partial class TVViewElement {
#if !COREBUILD
		[Export ("updateType")]
		public virtual TVElementUpdateType UpdateType {
			get {
				var value = _UpdateType;
				switch ((long) value) {
				case 2:
					if (UIDevice.CurrentDevice.CheckSystemVersion (12, 0)) {
						return TVElementUpdateType.Styles;
					} else {
						return TVElementUpdateType.Children;
					}
					break;
				case 3:
					if (UIDevice.CurrentDevice.CheckSystemVersion (12, 0)) {
						return TVElementUpdateType.Children;
					} else {
						return TVElementUpdateType.Self;
					}
					break;
				case 4:
					if (UIDevice.CurrentDevice.CheckSystemVersion (12, 0)) {
						return TVElementUpdateType.Self;
					} else {
						return TVElementUpdateType.Styles;
					}
					break;
				default:
					return value;
				}
			}
		}
#endif
	}
}
