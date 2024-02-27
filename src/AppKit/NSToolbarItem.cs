//
// NSToolbarItem.cs: Support for the NSToolbarItem class
//
// Author:
//   Johan Hammar
//
using System;
using ObjCRuntime;
using Foundation;

#nullable enable

namespace AppKit {

	public partial class NSToolbarItem {
		NSObject? target;
		Selector? action;

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
