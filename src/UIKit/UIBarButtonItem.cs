using Foundation;
using ObjCRuntime;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UIKit {

	public partial class UIBarButtonItem {
		const string actionSelector = "xamarinInvokeCallback:";

		[DynamicDependencyAttribute ("Call(Foundation.NSObject)")]
		static Selector actionSel = new Selector (actionSelector);

		[Export (actionSelector)]
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

		[DynamicDependencyAttribute ("Call(Foundation.NSObject)")]
		EventHandler? clicked;

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
