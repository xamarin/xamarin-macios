//
// Sanitize callbacks
//

#if !WATCH

using Foundation;
using ObjCRuntime;
using System;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {

	public partial class UIBarButtonItem {
		const string actionSelector = "InvokeAction:";
		static Selector actionSel = new Selector (actionSelector);

		[Export ("InvokeAction:")]
		[Preserve (Conditional = true)]
		void Call (NSObject sender)
		{
			if (clicked is not null)
				clicked (sender, EventArgs.Empty);
		}

		public UIBarButtonItem (UIImage image, UIBarButtonItemStyle style, EventHandler handler)
		: this (image, style, null, actionSel)
		{
			Target = this;
			clicked += handler;
			MarkDirty ();
		}


		public UIBarButtonItem (string title, UIBarButtonItemStyle style, EventHandler handler)
		: this (title, style, null, actionSel)
		{
			Target = this;
			clicked += handler;
			MarkDirty ();
		}


		public UIBarButtonItem (UIBarButtonSystemItem systemItem, EventHandler handler)
		: this (systemItem, null, actionSel)
		{
			Target = this;
			clicked += handler;
			MarkDirty ();
		}

		public UIBarButtonItem (UIBarButtonSystemItem systemItem) : this (systemItem: systemItem, target: null, action: null)
		{
		}

		EventHandler clicked;

		public event EventHandler Clicked {
			add {
				if (clicked is null) {
					Target = this;
					this.Action = actionSel;
					MarkDirty ();
				}

				clicked += value;
			}

			remove {
				clicked -= value;
			}
		}
	}
}

#endif // !WATCH
