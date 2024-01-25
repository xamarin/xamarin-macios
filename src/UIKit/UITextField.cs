//
// UITextField.cs: Extensions to UITextField
//
// Authors:
//   Geoff Norton
//
// Copyright 2009, Novell, Inc.
//

#if !WATCH && !COREBUILD

using System;

using Foundation;

using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {

	public partial class UITextFieldEditingEndedEventArgs : EventArgs {
		public UITextFieldEditingEndedEventArgs (UITextFieldDidEndEditingReason reason)
		{
			this.Reason = reason;
		}
		public UITextFieldDidEndEditingReason Reason { get; set; }
	}

	public delegate bool UITextFieldChange (UITextField textField, NSRange range, string replacementString);

	public delegate bool UITextFieldCondition (UITextField textField);

	public partial class UITextField : IUITextInputTraits {

		internal virtual Type GetInternalEventDelegateType {
			get { return typeof (_UITextFieldDelegate); }
		}

		internal virtual _UITextFieldDelegate CreateInternalEventDelegateType ()
		{
			return (_UITextFieldDelegate) (new _UITextFieldDelegate ());
		}

		internal _UITextFieldDelegate EnsureUITextFieldDelegate ()
		{
			if (Delegate is not null)
				UIApplication.EnsureEventAndDelegateAreNotMismatched (Delegate, GetInternalEventDelegateType);
			_UITextFieldDelegate del = Delegate as _UITextFieldDelegate;
			if (del is null) {
				del = (_UITextFieldDelegate) CreateInternalEventDelegateType ();
				Delegate = (IUITextFieldDelegate) del;
			}
			return del;
		}

#pragma warning disable 672
		[Register]
		internal class _UITextFieldDelegate : NSObject, IUITextFieldDelegate {
			public _UITextFieldDelegate () { IsDirectBinding = false; }

			internal EventHandler editingEnded;
			[Preserve (Conditional = true)]
			[Export ("textFieldDidEndEditing:")]
			public void EditingEnded (UITextField textField)
			{
				EventHandler handler = editingEnded;
				if (handler is not null) {
					handler (textField, EventArgs.Empty);
				} else {
					// if this is executed before iOS10 and only the new API is used we'll raise the new event (if set)
					EventHandler<UITextFieldEditingEndedEventArgs> handler2 = editingEnded1;
					if (handler2 is not null) {
						var args = new UITextFieldEditingEndedEventArgs (UITextFieldDidEndEditingReason.Unknown);
						handler2 (textField, args);
					}
				}
			}

			internal EventHandler<UITextFieldEditingEndedEventArgs> editingEnded1;
			[Preserve (Conditional = true)]
			[Export ("textFieldDidEndEditing:reason:")]
			public void EditingEnded (UITextField textField, UITextFieldDidEndEditingReason reason)
			{
				EventHandler<UITextFieldEditingEndedEventArgs> handler = editingEnded1;
				if (handler is not null) {
					var args = new UITextFieldEditingEndedEventArgs (reason);
					handler (textField, args);
				} else {
					// if this is executed on iOS10 (or late) and only the old API is used then we'll raise the old event (if set)
					EventHandler handler2 = editingEnded;
					if (handler2 is not null)
						handler2 (textField, EventArgs.Empty);
				}
			}

			internal EventHandler editingStarted;
			[Preserve (Conditional = true)]
			[Export ("textFieldDidBeginEditing:")]
			public void EditingStarted (UITextField textField)
			{
				EventHandler handler = editingStarted;
				if (handler is not null) {
					handler (textField, EventArgs.Empty);
				}
			}

			internal UITextFieldCondition shouldBeginEditing;
			[Preserve (Conditional = true)]
			[Export ("textFieldShouldBeginEditing:")]
			public bool ShouldBeginEditing (UITextField textField)
			{
				UITextFieldCondition handler = shouldBeginEditing;
				if (handler is not null)
					return handler (textField);
				return true;
			}

			internal UITextFieldChange shouldChangeCharacters;
			[Preserve (Conditional = true)]
			[Export ("textField:shouldChangeCharactersInRange:replacementString:")]
			public bool ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString)
			{
				UITextFieldChange handler = shouldChangeCharacters;
				if (handler is not null)
					return handler (textField, range, replacementString);
				return true;
			}

			internal UITextFieldCondition shouldClear;
			[Preserve (Conditional = true)]
			[Export ("textFieldShouldClear:")]
			public bool ShouldClear (UITextField textField)
			{
				UITextFieldCondition handler = shouldClear;
				if (handler is not null)
					return handler (textField);
				return true;
			}

			internal UITextFieldCondition shouldEndEditing;
			[Preserve (Conditional = true)]
			[Export ("textFieldShouldEndEditing:")]
			public bool ShouldEndEditing (UITextField textField)
			{
				UITextFieldCondition handler = shouldEndEditing;
				if (handler is not null)
					return handler (textField);
				return true;
			}

			internal UITextFieldCondition shouldReturn;
			[Preserve (Conditional = true)]
			[Export ("textFieldShouldReturn:")]
			public bool ShouldReturn (UITextField textField)
			{
				UITextFieldCondition handler = shouldReturn;
				if (handler is not null)
					return handler (textField);
				return true;
			}
		}
#pragma warning restore 672

		public event EventHandler Ended {
			add { EnsureUITextFieldDelegate ().editingEnded += value; }
			remove { EnsureUITextFieldDelegate ().editingEnded -= value; }
		}

		public event EventHandler<UITextFieldEditingEndedEventArgs> EndedWithReason {
			add { EnsureUITextFieldDelegate ().editingEnded1 += value; }
			remove { EnsureUITextFieldDelegate ().editingEnded1 -= value; }
		}

		public event EventHandler Started {
			add { EnsureUITextFieldDelegate ().editingStarted += value; }
			remove { EnsureUITextFieldDelegate ().editingStarted -= value; }
		}

		public UITextFieldCondition ShouldBeginEditing {
			get { return EnsureUITextFieldDelegate ().shouldBeginEditing; }
			set { EnsureUITextFieldDelegate ().shouldBeginEditing = value; }
		}

		public UITextFieldChange ShouldChangeCharacters {
			get { return EnsureUITextFieldDelegate ().shouldChangeCharacters; }
			set { EnsureUITextFieldDelegate ().shouldChangeCharacters = value; }
		}

		public UITextFieldCondition ShouldClear {
			get { return EnsureUITextFieldDelegate ().shouldClear; }
			set { EnsureUITextFieldDelegate ().shouldClear = value; }
		}

		public UITextFieldCondition ShouldEndEditing {
			get { return EnsureUITextFieldDelegate ().shouldEndEditing; }
			set { EnsureUITextFieldDelegate ().shouldEndEditing = value; }
		}

		public UITextFieldCondition ShouldReturn {
			get { return EnsureUITextFieldDelegate ().shouldReturn; }
			set { EnsureUITextFieldDelegate ().shouldReturn = value; }
		}
	}
}

#endif // !WATCH
