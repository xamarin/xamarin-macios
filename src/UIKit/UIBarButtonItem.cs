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
		static Selector actionSel = new Selector ("InvokeAction:");

		[Register]
		internal class Callback : NSObject {
			internal UIBarButtonItem container;

			public Callback ()
			{
				IsDirectBinding = false;
			}

			[Export ("InvokeAction:")]
			[Preserve (Conditional = true)]
			public void Call (NSObject sender)
			{
				if (container.clicked is not null)
					container.clicked (sender, EventArgs.Empty);
			}
		}

		public UIBarButtonItem (UIImage image, UIBarButtonItemStyle style, EventHandler handler)
		: this (image, style, new Callback (), actionSel)
		{
			callback = (Callback) Target;
			callback.container = this;
			clicked += handler;
			MarkDirty ();
		}


		public UIBarButtonItem (string title, UIBarButtonItemStyle style, EventHandler handler)
		: this (title, style, new Callback (), actionSel)
		{
			callback = (Callback) Target;
			callback.container = this;
			clicked += handler;
			MarkDirty ();
		}


		public UIBarButtonItem (UIBarButtonSystemItem systemItem, EventHandler handler)
		: this (systemItem, new Callback (), actionSel)
		{
			callback = (Callback) Target;
			callback.container = this;
			clicked += handler;
			MarkDirty ();
		}

		public UIBarButtonItem (UIBarButtonSystemItem systemItem) : this (systemItem: systemItem, target: null, action: null)
		{
		}

		internal EventHandler clicked;
		internal Callback callback;

		public event EventHandler Clicked {
			add {
				if (clicked is null) {
					callback = new Callback ();
					callback.container = this;
					this.Target = callback;
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
