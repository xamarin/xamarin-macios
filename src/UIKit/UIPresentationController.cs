using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace UIKit {
	public partial class UIPresentationController {
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
