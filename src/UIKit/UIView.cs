// 
// UIView.cs: Implements the managed UIView
//
// Authors:
//   Geoff Norton.
//     
// Copyright 2009 Novell, Inc
// Copyrigh 2014, Xamarin Inc.
//

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

#nullable enable

namespace UIKit {
	public partial class UIView : IEnumerable {

		public void Add (UIView view)
		{
			AddSubview (view);
		}

		public void AddSubviews (params UIView []? views)
		{
			if (views is null)
				return;
			foreach (var v in views)
				AddSubview (v);
		}

		public IEnumerator GetEnumerator ()
		{
			UIView [] subviews = Subviews;
			if (subviews is null)
				yield break;
			foreach (UIView uiv in subviews)
				yield return uiv;
		}

		public static void BeginAnimations (string animation)
		{
			BeginAnimations (animation, IntPtr.Zero);
		}

		[Register]
		class _UIViewStaticCallback : NSObject {
			static _UIViewStaticCallback? shared;
			public const string start = "start";
			public const string end = "end";
			public event Action? WillStart;
			public event Action? WillEnd;

			public _UIViewStaticCallback ()
			{
				IsDirectBinding = false;
			}

			[Preserve (Conditional = true)]
			[Export ("start")]
			public void OnStart ()
			{
				if (WillStart is not null)
					WillStart ();
			}

			[Preserve (Conditional = true)]
			[Export ("end")]
			public void OnEnd ()
			{
				shared = null;
				if (WillEnd is not null)
					WillEnd ();
			}

			public static _UIViewStaticCallback Prepare ()
			{
				if (shared is null) {
					shared = new _UIViewStaticCallback ();
					SetAnimationDelegate (shared);
				}
				return shared;
			}
		}

		public static event Action AnimationWillStart {
			add {
				_UIViewStaticCallback.Prepare ().WillStart += value;
			}
			remove {
				_UIViewStaticCallback.Prepare ().WillStart -= value;
			}
		}

		public static event Action AnimationWillEnd {
			add {
				_UIViewStaticCallback.Prepare ().WillEnd += value;
			}
			remove {
				_UIViewStaticCallback.Prepare ().WillEnd -= value;
			}
		}

		[Advice ("Use the *Notify method that has 'UICompletionHandler completion' parameter, the 'bool' will tell you if the operation finished.")]
		public static void Animate (double duration, Action animation, Action completion)
		{
			// animation null check will be done in AnimateNotify
			AnimateNotify (duration, animation, (x) => {
				if (completion is not null)
					completion ();
			});
		}

		[Advice ("Use the *Notify method that has 'UICompletionHandler completion' parameter, the 'bool' will tell you if the operation finished.")]
		public static void Animate (double duration, double delay, UIViewAnimationOptions options, Action animation, Action completion)
		{
			// animation null check will be done in AnimateNotify
			AnimateNotify (duration, delay, options, animation, (x) => {
				if (completion is not null)
					completion ();
			});
		}

		[Advice ("Use the *Notify method that has 'UICompletionHandler completion' parameter, the 'bool' will tell you if the operation finished.")]
		public static void Transition (UIView fromView, UIView toView, double duration, UIViewAnimationOptions options, Action completion)
		{
			TransitionNotify (fromView, toView, duration, options, (x) => {
				if (completion is not null)
					completion ();
			});
		}

		[Advice ("Use the *Notify method that has 'UICompletionHandler completion' parameter, the 'bool' will tell you if the operation finished.")]
		public static void Transition (UIView withView, double duration, UIViewAnimationOptions options, Action animation, Action completion)
		{
			// animation null check will be done in AnimateNotify
			TransitionNotify (withView, duration, options, animation, (x) => {
				if (completion is not null)
					completion ();
			});
		}

		public static Task<bool> AnimateAsync (double duration, Action animation)
		{
			return AnimateNotifyAsync (duration, animation);
		}

		public UIImage Capture (bool afterScreenUpdates = true)
		{
			UIImage snapshot;
			var bounds = Bounds; // try to access objc the smalles amount of times.
			try {
				UIGraphics.BeginImageContextWithOptions (bounds.Size, Opaque, 0.0f);
				DrawViewHierarchy (bounds, afterScreenUpdates);
				snapshot = UIGraphics.GetImageFromCurrentImageContext ();
			} finally {
				UIGraphics.EndImageContext ();
			}
			return snapshot;
		}

		#region Inlined from the UITraitChangeObservable protocol
		/// <summary>
		/// Registers a callback handler that will be executed when one of the specified traits changes.
		/// </summary>
		/// <param name="traits">The traits to observe.</param>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges (Type [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, traits, handler);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when one of the specified traits changes.
		/// </summary>
		/// <param name="traits">The traits to observe.</param>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public unsafe IUITraitChangeRegistration RegisterForTraitChanges (Action<IUITraitEnvironment, UITraitCollection> handler, params Type [] traits)
		{
			// Add an override with 'params', unfortunately this means reordering the parameters.
			return IUITraitChangeObservable._RegisterForTraitChanges (this, handler, traits);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when the specified trait changes.
		/// </summary>
		/// <typeparam name="T">The trait to observe.</typeparam>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public unsafe IUITraitChangeRegistration RegisterForTraitChanges<T> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T : IUITraitDefinition
		{
			return IUITraitChangeObservable._RegisterForTraitChanges<T> (this, handler);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when any of the specified traits changes.
		/// </summary>
		/// <typeparam name="T1">A trait to observe</typeparam>
		/// <typeparam name="T2">A trait to observe</typeparam>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public unsafe IUITraitChangeRegistration RegisterForTraitChanges<T1, T2> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
		{
			return IUITraitChangeObservable._RegisterForTraitChanges<T1, T2> (this, handler);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when any of the specified traits changes.
		/// </summary>
		/// <typeparam name="T1">A trait to observe</typeparam>
		/// <typeparam name="T2">A trait to observe</typeparam>
		/// <typeparam name="T3">A trait to observe</typeparam>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public unsafe IUITraitChangeRegistration RegisterForTraitChanges<T1, T2, T3> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
			where T3 : IUITraitDefinition
		{
			return IUITraitChangeObservable._RegisterForTraitChanges<T1, T2, T3> (this, handler);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when any of the specified traits changes.
		/// </summary>
		/// <typeparam name="T1">A trait to observe</typeparam>
		/// <typeparam name="T2">A trait to observe</typeparam>
		/// <typeparam name="T3">A trait to observe</typeparam>
		/// <typeparam name="T4">A trait to observe</typeparam>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public unsafe IUITraitChangeRegistration RegisterForTraitChanges<T1, T2, T3, T4> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
			where T3 : IUITraitDefinition
			where T4 : IUITraitDefinition
		{
			return IUITraitChangeObservable._RegisterForTraitChanges<T1, T2, T3, T4> (this, handler);
		}

		/// <summary>
		/// Registers a selector that will be called on the specified object when any of the specified traits changes.
		/// </summary>
		/// <param name="traits">The traits to observe.</param>
		/// <param name="target">The object whose specified selector will be called.</param>
		/// <param name="action">The selector to call on the specified object.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges (Type [] traits, NSObject target, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, traits, target, action);
		}

		/// <summary>
		/// Registers a selector that will be called on the current object when any of the specified traits changes.
		/// </summary>
		/// <param name="traits">The traits to observe.</param>
		/// <param name="action">The selector to call on the current object.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges (Type [] traits, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, traits, action);
		}
		#endregion
	}
}
