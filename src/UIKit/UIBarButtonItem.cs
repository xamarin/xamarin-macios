using Foundation;
using ObjCRuntime;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UIKit {

	[Category (typeof (UIBarButtonItem))]
	static class UIBarButtonItem_Extensions {
		[Export (UIBarButtonItem.actionSelector)]
		static void Call (this UIBarButtonItem item, NSObject sender)
		{
			item.OnClicked (sender);
		}
	}

	public partial class UIBarButtonItem {
		internal const string actionSelector = "xamarinInvokeCallback:";

		[DynamicDependencyAttribute ("Call(UIKit.UIBarButtonItem,Foundation.NSObject)", typeof (UIBarButtonItem_Extensions))]
		static Selector actionSel = new Selector (actionSelector);

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

		[DynamicDependencyAttribute ("Call(UIKit.UIBarButtonItem,Foundation.NSObject)", typeof (UIBarButtonItem_Extensions))]
		EventHandler? clicked;

		internal void OnClicked (NSObject sender)
		{
			if (clicked is not null)
				clicked (sender, EventArgs.Empty);
		}

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
