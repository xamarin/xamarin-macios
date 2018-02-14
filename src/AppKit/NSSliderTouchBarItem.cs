using System;
using ObjCRuntime;
using Foundation;

namespace AppKit {

	public partial class NSSliderTouchBarItem {
		// If you modify, also search for other other XM_ACTIVATED_COPY and update as well
		NSObject target;
		Selector action;

		public event EventHandler Activated {
			add {
				target = ActionDispatcher.SetupAction (Target, value);
				action = ActionDispatcher.Action;
				MarkDirty ();
				Target = target;
				Action = action;
			}

			remove {
				ActionDispatcher.RemoveAction (Target, value);
				target = null;
				action = null;
				MarkDirty ();
			}
		}


	}
}

