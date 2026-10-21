// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

#nullable enable

#if __MACOS__ && !COREBUILD
namespace Foundation {
	public partial class NSObject {
		static readonly ConditionalWeakTable<NSObject, Dictionary<string, BindingRegistrationState>> bindingRegistrations = new ();

		/// <summary>Binds a property to an observable and keeps both objects alive until the binding is removed.</summary>
		/// <param name="binding">The name of the binding on this object.</param>
		/// <param name="observable">The object to observe.</param>
		/// <param name="keyPath">The key path to observe on <paramref name="observable" />.</param>
		/// <param name="options">Options for the binding, or <see langword="null" /> for the defaults.</param>
		/// <returns>A registration that removes the binding when disposed.</returns>
		/// <remarks>
		///   <para>Keep the registration for as long as the binding is needed, and dispose it on the main thread before disposing either object.</para>
		///   <para>Removing or replacing this binding through the managed API invalidates the previous registration. Disposing an invalidated registration has no effect.</para>
		///   <para>If establishing the binding fails, any binding with this name is removed before the exception is rethrown.</para>
		///   <para>An undisposed registration logs a warning and schedules unbinding on the main thread when collected. Both objects remain rooted until cleanup completes. Explicit disposal is required to break cycles where either object retains the registration.</para>
		/// </remarks>
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		public IDisposable BindWithLifetime (NSString binding, NSObject observable, string keyPath, NSDictionary? options = null)
		{
			AppKit.NSApplication.EnsureUIThread ();
			binding.GetNonNullHandle (nameof (binding));
			observable.GetNonNullHandle (nameof (observable));
			this.GetNonNullHandle ("this");
			ArgumentNullException.ThrowIfNull (keyPath);

			var state = new BindingRegistrationState (this, binding, observable);
			var registration = new BindingRegistration (state);
			try {
#if XAMCORE_5_0
				_Bind (binding, observable, keyPath, options);
#else
#pragma warning disable CS0618
				Bind (binding, observable, keyPath, options);
#pragma warning restore CS0618
#endif
				ReleaseBindingRegistration (binding);
				bindingRegistrations.GetOrCreateValue (this) [state.Name] = state;
			} catch {
				// Cocoa may install a binding before raising an exception while evaluating its key path.
				Unbind (binding);
				state.Release ();
				GC.SuppressFinalize (registration);
				throw;
			}
			return registration;
		}

		/// <summary>Binds a property using strongly typed options and keeps both objects alive until the binding is removed.</summary>
		/// <param name="binding">The name of the binding on this object.</param>
		/// <param name="observable">The object to observe.</param>
		/// <param name="keyPath">The key path to observe on <paramref name="observable" />.</param>
		/// <param name="options">Options for the binding, or <see langword="null" /> for the defaults.</param>
		/// <returns>A registration that removes the binding when disposed.</returns>
		/// <remarks>
		///   <para>Keep the registration for as long as the binding is needed, and dispose it on the main thread before disposing either object.</para>
		///   <para>The registration has the same lifetime and cleanup behavior as <see cref="BindWithLifetime(NSString, NSObject, string, NSDictionary)" />.</para>
		/// </remarks>
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		public IDisposable BindWithLifetime (NSString binding, NSObject observable, string keyPath, AppKit.NSBindingOptions? options)
		{
			return BindWithLifetime (binding, observable, keyPath, options.GetDictionary ());
		}

		void ReleaseBindingRegistration (NSString binding)
		{
			if (!bindingRegistrations.TryGetValue (this, out var registrations))
				return;
			var name = binding.ToString ();
			if (registrations.TryGetValue (name, out var state)) {
				registrations.Remove (name);
				state.Release ();
			}
		}

		sealed class BindingRegistrationState {
			NSObject? receiver;
			NSObject? observable;
			NSString? binding;
			GCHandle lifetime;

			public readonly string Name;
			public bool IsActive => Volatile.Read (ref receiver) is not null;

			public BindingRegistrationState (NSObject receiver, NSString binding, NSObject observable)
			{
				this.receiver = receiver;
				this.observable = observable;
				Name = binding.ToString ();
				this.binding = new NSString (Name);
				// Root the endpoints independently of the finalizable token until main-thread cleanup finishes.
				lifetime = GCHandle.Alloc (this);
			}

			public void Unbind ()
			{
				if (!IsActive)
					return;
				AppKit.NSApplication.EnsureUIThread ();
				var receiver = this.receiver;
				var binding = this.binding;
				var observable = this.observable;
				if (receiver is null || binding is null)
					return;

				receiver.Unbind (binding);
				if (bindingRegistrations.TryGetValue (receiver, out var registrations) &&
					registrations.TryGetValue (Name, out var current) && ReferenceEquals (current, this))
					registrations.Remove (Name);
				Release ();
				GC.KeepAlive (observable);
			}

			public void Release ()
			{
				Volatile.Write (ref receiver, null);
				observable = null;
				binding?.Dispose ();
				binding = null;
				if (lifetime.IsAllocated)
					lifetime.Free ();
			}
		}

		sealed class BindingRegistration : IDisposable {
			readonly BindingRegistrationState state;

			public BindingRegistration (BindingRegistrationState state)
			{
				this.state = state;
			}

			~BindingRegistration ()
			{
				if (!state.IsActive)
					return;
				Runtime.NSLog ("Warning: binding registration object was not disposed manually with Dispose()");
				CoreFoundation.DispatchQueue.MainQueue.DispatchAsync (state.Unbind);
			}

			public void Dispose ()
			{
				state.Unbind ();
				GC.SuppressFinalize (this);
			}
		}
	}
}
#endif
