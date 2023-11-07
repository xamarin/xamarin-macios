//
// UIAccessibilityCustomAction.cs: Helpers for actions
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2014 Xamarin Inc
//

#if !WATCH

using System;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace UIKit {

	public partial class UIAccessibilityCustomAction {
		object action;

#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public UIAccessibilityCustomAction ()
		{
		}
#endif

		public UIAccessibilityCustomAction (string name, Func<UIAccessibilityCustomAction, bool> probe) : this (name, FuncBoolDispatcher.Selector, new FuncBoolDispatcher (probe))
		{

		}

		internal UIAccessibilityCustomAction (string name, Selector sel, FuncBoolDispatcher disp) : this (name, disp, sel)
		{
			action = disp;
			MarkDirty ();
		}

		// Use this for synchronous operations
		[Register ("__MonoMac_FuncBoolDispatcher")]
		internal sealed class FuncBoolDispatcher : NSObject {
			public const string SelectorName = "xamarinApplySelectorFunc:";
			public static readonly Selector Selector = new Selector (SelectorName);

			readonly Func<UIAccessibilityCustomAction, bool> probe;

			public FuncBoolDispatcher (Func<UIAccessibilityCustomAction, bool> probe)
			{
				if (probe is null)
					throw new ArgumentNullException ("probe");

				this.probe = probe;
				IsDirectBinding = false;
			}

			[Export (SelectorName)]
			[Preserve (Conditional = true)]
			public bool Probe (UIAccessibilityCustomAction customAction)
			{
				return probe (customAction);
			}
		}

	}
}

#endif // !WATCH
