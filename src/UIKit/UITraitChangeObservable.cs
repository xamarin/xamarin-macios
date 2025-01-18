//
// UITraitChangeObservable.cs: support for IUITraitChangeObservable
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2023 Microsoft Corp. All rights reserved.
//

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace UIKit {
	public partial interface IUITraitChangeObservable {
#if XAMCORE_5_0
		private static Class [] ToClasses (params Type [] traits)
#else
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static Class [] ToClasses (params Type [] traits)
#endif
		{
			if (traits is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (traits));

			return Class.FromTypes (traits);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when one of the specified traits changes.
		/// </summary>
		/// <param name="traits">The traits to observe.</param>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges (Type [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return _RegisterForTraitChanges (this, traits, handler);
		}

		internal static IUITraitChangeRegistration _RegisterForTraitChanges (IUITraitChangeObservable This, Type [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return _RegisterForTraitChanges (This, ToClasses (traits), handler);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when one of the specified traits changes.
		/// </summary>
		/// <param name="traits">The traits to observe.</param>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges (Action<IUITraitEnvironment, UITraitCollection> handler, params Type [] traits)
		{
			return _RegisterForTraitChanges (this, handler, traits);
		}

		internal static IUITraitChangeRegistration _RegisterForTraitChanges (IUITraitChangeObservable This, Action<IUITraitEnvironment, UITraitCollection> handler, params Type [] traits)
		{
			// Add an override with 'params', unfortunately this means reordering the parameters.
			return _RegisterForTraitChanges (This, ToClasses (traits), handler);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when the specified trait changes.
		/// </summary>
		/// <typeparam name="T">The trait to observe.</typeparam>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges<T> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T : IUITraitDefinition
		{
			return _RegisterForTraitChanges<T> (this, handler);
		}

		internal static IUITraitChangeRegistration _RegisterForTraitChanges<T> (IUITraitChangeObservable This, Action<IUITraitEnvironment, UITraitCollection> handler)
			where T : IUITraitDefinition
		{
			return _RegisterForTraitChanges (This, ToClasses (typeof (T)), handler);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when any of the specified traits changes.
		/// </summary>
		/// <typeparam name="T1">A trait to observe</typeparam>
		/// <typeparam name="T2">A trait to observe</typeparam>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges<T1, T2> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
		{
			return _RegisterForTraitChanges<T1, T2> (this, handler);
		}

		internal static IUITraitChangeRegistration _RegisterForTraitChanges<T1, T2> (IUITraitChangeObservable This, Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
		{
			return _RegisterForTraitChanges (This, ToClasses (typeof (T1), typeof (T2)), handler);
		}

		/// <summary>
		/// Registers a callback handler that will be executed when any of the specified traits changes.
		/// </summary>
		/// <typeparam name="T1">A trait to observe</typeparam>
		/// <typeparam name="T2">A trait to observe</typeparam>
		/// <typeparam name="T3">A trait to observe</typeparam>
		/// <param name="handler">The callback to execute when any of the specified traits changes.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges<T1, T2, T3> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
			where T3 : IUITraitDefinition
		{
			return _RegisterForTraitChanges<T1, T2, T3> (this, handler);
		}

		internal static IUITraitChangeRegistration _RegisterForTraitChanges<T1, T2, T3> (IUITraitChangeObservable This, Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
			where T3 : IUITraitDefinition
		{
			return _RegisterForTraitChanges (This, ToClasses (typeof (T1), typeof (T2), typeof (T3)), handler);
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
		public IUITraitChangeRegistration RegisterForTraitChanges<T1, T2, T3, T4> (Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
			where T3 : IUITraitDefinition
			where T4 : IUITraitDefinition
		{
			return _RegisterForTraitChanges<T1, T2, T3, T3> (this, handler);
		}

		internal static IUITraitChangeRegistration _RegisterForTraitChanges<T1, T2, T3, T4> (IUITraitChangeObservable This, Action<IUITraitEnvironment, UITraitCollection> handler)
			where T1 : IUITraitDefinition
			where T2 : IUITraitDefinition
			where T3 : IUITraitDefinition
			where T4 : IUITraitDefinition
		{
			return _RegisterForTraitChanges (This, ToClasses (typeof (T1), typeof (T2), typeof (T3), typeof (T4)), handler);
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
			return _RegisterForTraitChanges (this, traits, target, action);
		}

		internal static IUITraitChangeRegistration _RegisterForTraitChanges (IUITraitChangeObservable This, Type [] traits, NSObject target, Selector action)
		{
			return _RegisterForTraitChanges (This, ToClasses (traits), target, action);
		}

		/// <summary>
		/// Registers a selector that will be called on the current object when any of the specified traits changes.
		/// </summary>
		/// <param name="traits">The traits to observe.</param>
		/// <param name="action">The selector to call on the current object.</param>
		/// <returns>A token that can be used to unregister the callback by calling <see cref="M:UnregisterForTraitChanges" />.</returns>
		public IUITraitChangeRegistration RegisterForTraitChanges (Type [] traits, Selector action)
		{
			return _RegisterForTraitChanges (this, traits, action);
		}

		internal static IUITraitChangeRegistration _RegisterForTraitChanges (IUITraitChangeObservable This, Type [] traits, Selector action)
		{
			return _RegisterForTraitChanges (This, ToClasses (traits), action);
		}

#if XAMCORE_5_0
		private static Class [] ToClasses (IUITraitDefinition [] traits)
#else
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static Class [] ToClasses (IUITraitDefinition [] traits)
#endif
		{
			if (traits is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (traits));
			var traitsClasses = new Class [traits.Length];
			for (var i = 0; i < traits.Length; i++)
				traitsClasses [i] = new Class (traits [i].GetType ());
			return traitsClasses;
		}

#if !XAMCORE_5_0
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Action<IUITraitEnvironment, UITraitCollection>)' method instead.")]
		public IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return RegisterForTraitChanges (ToClasses (traits), handler);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], NSObject, Selector)' method instead.")]
		public IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, NSObject target, Selector action)
		{
			return RegisterForTraitChanges (ToClasses (traits), target, action);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Selector)' method instead.")]
		public IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Selector action)
		{
			return RegisterForTraitChanges (ToClasses (traits), action);
		}
#endif // !XACMORE_5_0

		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
#if XAMCORE_5_0
		private extern static NativeHandle NativeHandle_objc_msgSend_NativeHandle_NativeHandle (IntPtr receiver, IntPtr selector, NativeHandle arg1, NativeHandle arg2);
#else
		[EditorBrowsable (EditorBrowsableState.Never)]
		public extern static NativeHandle NativeHandle_objc_msgSend_NativeHandle_NativeHandle (IntPtr receiver, IntPtr selector, NativeHandle arg1, NativeHandle arg2);
#endif

		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
#if XAMCORE_5_0
		private extern unsafe static NativeHandle NativeHandle_objc_msgSend_NativeHandle_BlockLiteral (IntPtr receiver, IntPtr selector, NativeHandle arg1, BlockLiteral* arg2);
#else
		[EditorBrowsable (EditorBrowsableState.Never)]
		public extern unsafe static NativeHandle NativeHandle_objc_msgSend_NativeHandle_BlockLiteral (IntPtr receiver, IntPtr selector, NativeHandle arg1, BlockLiteral* arg2);
#endif

		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
#if XAMCORE_5_0
		private extern static NativeHandle NativeHandle_objc_msgSend_NativeHandle_NativeHandle_NativeHandle (IntPtr receiver, IntPtr selector, NativeHandle arg1, NativeHandle arg2, NativeHandle arg3);
#else
		[EditorBrowsable (EditorBrowsableState.Never)]
		public extern static NativeHandle NativeHandle_objc_msgSend_NativeHandle_NativeHandle_NativeHandle (IntPtr receiver, IntPtr selector, NativeHandle arg1, NativeHandle arg2, NativeHandle arg3);
#endif
	}

#if !XAMCORE_5_0
	public partial class UIPresentationController {
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Action<IUITraitEnvironment, UITraitCollection>)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public unsafe virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), handler);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], NSObject, Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, NSObject target, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), target, action);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), action);
		}
	}

	public partial class UIView {
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Action<IUITraitEnvironment, UITraitCollection>)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public unsafe virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), handler);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], NSObject, Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, NSObject target, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), target, action);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), action);
		}
	}

	public partial class UIViewController {
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Action<IUITraitEnvironment, UITraitCollection>)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public unsafe virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), handler);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], NSObject, Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, NSObject target, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), target, action);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), action);
		}
	}

	public partial class UIWindowScene {
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Action<IUITraitEnvironment, UITraitCollection>)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public unsafe virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), handler);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], NSObject, Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, NSObject target, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), target, action);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public virtual IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (this, IUITraitChangeObservable.ToClasses (traits), action);
		}
	}

	public static partial class UITraitChangeObservable_Extensions {
		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Action<IUITraitEnvironment, UITraitCollection>)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public unsafe static IUITraitChangeRegistration RegisterForTraitChanges (this IUITraitChangeObservable This, IUITraitDefinition [] traits, global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (This, IUITraitChangeObservable.ToClasses (traits), handler);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], NSObject, Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public static IUITraitChangeRegistration RegisterForTraitChanges (this IUITraitChangeObservable This, IUITraitDefinition [] traits, NSObject target, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (This, IUITraitChangeObservable.ToClasses (traits), target, action);
		}

		[Obsolete ("Use the 'UITraitChangeObservable.RegisterForTraitChanges (Class[], Selector)' method instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		public static IUITraitChangeRegistration RegisterForTraitChanges (this IUITraitChangeObservable This, IUITraitDefinition [] traits, Selector action)
		{
			return IUITraitChangeObservable._RegisterForTraitChanges (This, IUITraitChangeObservable.ToClasses (traits), action);
		}
	}
#endif
}
