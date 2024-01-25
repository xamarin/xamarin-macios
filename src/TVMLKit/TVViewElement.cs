#nullable enable

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
		public virtual TVElementUpdateType UpdateType {
			[Export ("updateType")]
			get {
				var value = _UpdateType;
				switch ((long) value) {
				case 2:
					if (SystemVersion.CheckiOS (12, 0)) {
						return TVElementUpdateType.Styles;
					} else {
						return TVElementUpdateType.Children;
					}
				case 3:
					if (SystemVersion.CheckiOS (12, 0)) {
						return TVElementUpdateType.Children;
					} else {
						return TVElementUpdateType.Self;
					}
				case 4:
					if (SystemVersion.CheckiOS (12, 0)) {
						return TVElementUpdateType.Self;
					} else {
						return TVElementUpdateType.Styles;
					}
				default:
					return value;
				}
			}
		}
#endif
	}
}
