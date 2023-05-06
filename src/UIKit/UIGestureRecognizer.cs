// 
// UIGestureRecognizer: Implements some helper methods for UIGestureRecognizer
//
// Authors:
//   Miguel de Icaza
//     
// Copyright 2010 Novell, Inc
// Copyright 2011-2013 Xamarin Inc. All rights reserved
//

#if !WATCH

using System;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;
#nullable enable

namespace UIKit {

	public partial class UIGestureRecognizer {
		//
		// Tracks the targets (NSObject, which we always enforce to be Token) to the Selector the point to, used when disposing
		//
		Dictionary<Token, IntPtr> recognizers = new Dictionary<Token, IntPtr> ();
		const string tsel = "target";
		internal const string parametrized_selector = "target:";

		[DesignatedInitializer]
		public UIGestureRecognizer (Action action) : this (Selector.GetHandle (tsel), new ParameterlessDispatch (action))
		{
		}

		// Called by the Dispose() method, because this can run from a finalizer, we need to
		// (a) reference the handle, that we will release later, and (b) to remove the targets on the
		// UI thread.
		// Note: preserving this member allows us to re-enable the `Optimizable` binding flag
		[Preserve (Conditional = true)]
		void OnDispose ()
		{
			var copyOfRecognizers = recognizers;
			var savedHandle = Handle;
			recognizers = new Dictionary<Token, IntPtr> ();

			if (copyOfRecognizers.Count == 0)
				return;

			DangerousRetain (savedHandle);
			NSRunLoop.Main.BeginInvokeOnMainThread (() => {
				foreach (var kv in copyOfRecognizers)
					RemoveTarget (kv.Key, kv.Value);
				DangerousRelease (savedHandle);
			});
		}

		//
		// Signature swapped, this is only used so we can store the "token" in recognizers
		//
		public UIGestureRecognizer (Selector sel, Token token) : this (token, sel)
		{
			recognizers [token] = sel.Handle;
			MarkDirty ();
		}

		internal UIGestureRecognizer (IntPtr sel, Token token) : this (token, sel)
		{
			recognizers [token] = sel;
			MarkDirty ();
		}

		[Register ("__UIGestureRecognizerToken")]
		public class Token : NSObject {
			public Token ()
			{
				IsDirectBinding = false;
			}
		}

		[Register ("__UIGestureRecognizerGenericCB")]
		internal class Callback<T> : Token where T : UIGestureRecognizer {
			Action<T> action;

			internal Callback (Action<T> action)
			{
				this.action = action;
			}

			[Export ("target:")]
			[Preserve (Conditional = true)]
			public void Activated (T sender) => action (sender);

		}

		[Register ("__UIGestureRecognizerParameterlessToken")]
		public class ParameterlessDispatch : Token {
			Action action;

			internal ParameterlessDispatch (Action action)
			{
				this.action = action;
			}

			[Export ("target")]
			[Preserve (Conditional = true)]
			public void Activated ()
			{
				action ();
			}
		}

		[Register ("__UIGestureRecognizerParametrizedToken")]
		public class ParametrizedDispatch : Token {
			Action<UIGestureRecognizer> action;

			internal ParametrizedDispatch (Action<UIGestureRecognizer> action)
			{
				this.action = action;
			}

			[Export ("target:")]
			[Preserve (Conditional = true)]
			public void Activated (UIGestureRecognizer sender)
			{
				action (sender);
			}
		}

		public Token AddTarget (Action action)
		{
			if (action is null)
				throw new ArgumentNullException ("action");

			var t = new ParameterlessDispatch (action);
			RegisterTarget (t, Selector.GetHandle (tsel));
			return t;
		}

		public Token AddTarget (Action<NSObject> action)
		{
			if (action is null)
				throw new ArgumentNullException ("action");

			var t = new ParametrizedDispatch (action);
			RegisterTarget (t, Selector.GetHandle (parametrized_selector));
			return t;
		}

		void RegisterTarget (Token target, IntPtr sel)
		{
			AddTarget (target, sel);
			MarkDirty ();
			recognizers [target] = sel;
		}

		public void RemoveTarget (Token token)
		{
			if (token is null)
				throw new ArgumentNullException ("token");
			if (recognizers is null)
				return;
			if (recognizers.Remove (token, out var sel))
				RemoveTarget (token, sel);
		}

		//
		// Used to enumerate all the registered handlers for this UIGestureRecognizer
		//
		public IEnumerable<Token> GetTargets ()
		{
			var keys = recognizers?.Keys;
			if (keys is null)
				return Array.Empty<Token> ();
			return (IEnumerable<Token>) keys;
		}
	}

#if !TVOS
	public partial class UIRotationGestureRecognizer : UIGestureRecognizer {
		public UIRotationGestureRecognizer (Action action) : base (action) { }
		public UIRotationGestureRecognizer (Action<UIRotationGestureRecognizer> action) : base (Selector.GetHandle (UIGestureRecognizer.parametrized_selector), new Callback<UIRotationGestureRecognizer> (action)) { }

	}
#endif

	public partial class UILongPressGestureRecognizer : UIGestureRecognizer {
		public UILongPressGestureRecognizer (Action action) : base (action) { }
		public UILongPressGestureRecognizer (Action<UILongPressGestureRecognizer> action) : base (Selector.GetHandle (UIGestureRecognizer.parametrized_selector), new Callback<UILongPressGestureRecognizer> (action)) { }

	}

	public partial class UITapGestureRecognizer : UIGestureRecognizer {
		public UITapGestureRecognizer (Action action) : base (action) { }
		public UITapGestureRecognizer (Action<UITapGestureRecognizer> action) : base (Selector.GetHandle (UIGestureRecognizer.parametrized_selector), new Callback<UITapGestureRecognizer> (action)) { }

	}

	public partial class UIPanGestureRecognizer : UIGestureRecognizer {
		public UIPanGestureRecognizer (Action action) : base (action) { }
		public UIPanGestureRecognizer (Action<UIPanGestureRecognizer> action) : base (Selector.GetHandle (UIGestureRecognizer.parametrized_selector), new Callback<UIPanGestureRecognizer> (action)) { }

		internal UIPanGestureRecognizer (IntPtr sel, Token token) : base (token, sel) { }

	}

#if !TVOS
	public partial class UIPinchGestureRecognizer : UIGestureRecognizer {
		public UIPinchGestureRecognizer (Action action) : base (action) { }
		public UIPinchGestureRecognizer (Action<UIPinchGestureRecognizer> action) : base (Selector.GetHandle (UIGestureRecognizer.parametrized_selector), new Callback<UIPinchGestureRecognizer> (action)) { }

	}
#endif

	public partial class UISwipeGestureRecognizer : UIGestureRecognizer {
		public UISwipeGestureRecognizer (Action action) : base (action) { }
		public UISwipeGestureRecognizer (Action<UISwipeGestureRecognizer> action) : base (Selector.GetHandle (UIGestureRecognizer.parametrized_selector), new Callback<UISwipeGestureRecognizer> (action)) { }

	}

#if !TVOS
	public partial class UIScreenEdgePanGestureRecognizer : UIPanGestureRecognizer {
		public UIScreenEdgePanGestureRecognizer (Action action) : base (action) { }
		public UIScreenEdgePanGestureRecognizer (Action<UIScreenEdgePanGestureRecognizer> action) : base (Selector.GetHandle (UIGestureRecognizer.parametrized_selector), new Callback<UIScreenEdgePanGestureRecognizer> (action)) { }

	}

	public partial class UIHoverGestureRecognizer : UIGestureRecognizer {
		public UIHoverGestureRecognizer (Action<UIHoverGestureRecognizer> action) : base (Selector.GetHandle (UIGestureRecognizer.parametrized_selector), new Callback<UIHoverGestureRecognizer> (action)) { }

	}
#endif
}

#endif // !WATCH
