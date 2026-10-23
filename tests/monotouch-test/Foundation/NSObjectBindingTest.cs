// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

#if __MACOS__
using System.Runtime.CompilerServices;

using AppKit;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSObjectBindingTest {
		[Register]
		[Preserve (AllMembers = true)]
		class BindingObject : NSObject {
			bool enabled = true;

			[Export ("enabled")]
			public bool Enabled {
				get => enabled;
				set {
					WillChangeValue ("enabled");
					enabled = value;
					DidChangeValue ("enabled");
				}
			}

			[Export ("selected")]
			public bool Selected { get; set; }

			public int UnbindCount;
			public bool UnboundOnMainThread;

			public override void Unbind (NSString binding)
			{
				UnbindCount++;
				UnboundOnMainThread = NSThread.IsMain;
				base.Unbind (binding);
			}
		}

#if !XAMCORE_5_0
		[Register]
		[Preserve (AllMembers = true)]
		class BindingOverride : BindingObject {
			public int BindCount;

			[Obsolete ("Tests the legacy Bind override.")]
			public override void Bind (NSString binding, NSObject observable, string keyPath, NSDictionary? options)
			{
				BindCount++;
				base.Bind (binding, observable, keyPath, options);
			}
		}
#endif

		[TestCase (false)]
		[TestCase (true)]
		public void DisposeUnbinds (bool typedOptions)
		{
			using var receiver = new BindingObject ();
			using var observable = new BindingObject ();
			using var binding = new NSString ("enabled");
			using var registration = typedOptions
				? receiver.BindWithLifetime (binding, observable, "enabled", new NSBindingOptions ())
				: receiver.BindWithLifetime (binding, observable, "enabled");

			observable.Enabled = false;
			Assert.That (receiver.Enabled, Is.False, "Binding updates");
			registration.Dispose ();
			observable.Enabled = true;
			Assert.That (receiver.Enabled, Is.False, "Binding removed");
			registration.Dispose ();
			Assert.That (receiver.UnbindCount, Is.EqualTo (1), "Idempotent disposal");
			Assert.That (receiver.UnboundOnMainThread, Is.True, "Main-thread unbinding");
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static (IDisposable Token, WeakReference Receiver, WeakReference Observable) CreateRegistration ()
		{
			var receiver = new BindingObject ();
			var observable = new BindingObject ();
			using var binding = new NSString ("enabled");
			var token = receiver.BindWithLifetime (binding, observable, "enabled");
			return (token, new WeakReference (receiver), new WeakReference (observable));
		}

		static void Collect ()
		{
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			GC.Collect ();
		}

		static void WaitForCollection (params WeakReference [] references)
		{
			Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => {
				Collect ();
				foreach (var reference in references) {
					if (reference.IsAlive)
						return false;
				}
				return true;
			}), Is.True, "Objects were not released");
		}

		[Test]
		public void RetainsBothEndpointsUntilDisposed ()
		{
			var (token, receiver, observable) = CreateRegistration ();
			try {
				Collect ();
				Assert.That (receiver.IsAlive, Is.True, "Receiver rooted");
				Assert.That (observable.IsAlive, Is.True, "Observable rooted");
			} finally {
				token.Dispose ();
			}
			WaitForCollection (receiver, observable);
			GC.KeepAlive (token);
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static (WeakReference Token, WeakReference Receiver) ForgetRegistration (BindingObject observable)
		{
			var receiver = new BindingObject ();
			using var binding = new NSString ("enabled");
			var token = receiver.BindWithLifetime (binding, observable, "enabled");
			return (new WeakReference (token), new WeakReference (receiver));
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static void AssertReceiverState (WeakReference reference, int unbindCount)
		{
			var receiver = reference.Target as BindingObject;
			Assert.That (receiver, Is.Not.Null, "Receiver survives until main-thread cleanup");
			if (receiver is null)
				return;
			Assert.That (receiver.UnbindCount, Is.EqualTo (unbindCount), "Unbind count");
			if (unbindCount > 0)
				Assert.That (receiver.UnboundOnMainThread, Is.True, "Finalizer cleanup runs on main thread");
		}

		[Test]
		public void ForgottenRegistrationUnbindsOnMainThread ()
		{
			using var observable = new BindingObject ();
			var (token, receiver) = ForgetRegistration (observable);
			Collect ();
			Assert.That (token.IsAlive, Is.False, "Token collected despite rooted endpoints");
			AssertReceiverState (receiver, 0);
			observable.Enabled = false;

			Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => {
				var target = receiver.Target as BindingObject;
				return target is not null && target.UnbindCount == 1;
			}), Is.True, "Queued unbinding completed");
			AssertReceiverState (receiver, 1);
			WaitForCollection (receiver);
			observable.Enabled = true;
		}

		[Test]
		public void RebindingInvalidatesPreviousToken ()
		{
			using var receiver = new BindingObject ();
			using var first = new BindingObject ();
			using var second = new BindingObject ();
			using var binding = new NSString ("enabled");
			using var oldToken = receiver.BindWithLifetime (binding, first, "enabled");
			using var newToken = receiver.BindWithLifetime (binding, second, "enabled");
			oldToken.Dispose ();
			second.Enabled = false;
			Assert.That (receiver.Enabled, Is.False, "Old token did not remove replacement");
			first.Enabled = false;
			second.Enabled = true;
			Assert.That (receiver.Enabled, Is.True, "Replacement still active");
		}

		[Test]
		public void QueuedCleanupDoesNotUnbindReplacement ()
		{
			using var first = new BindingObject ();
			using var second = new BindingObject ();
			var (oldToken, weakReceiver) = ForgetRegistration (first);
			Collect ();
			Assert.That (oldToken.IsAlive, Is.False, "Old token finalized");
			var receiver = weakReceiver.Target as BindingObject;
			Assert.That (receiver, Is.Not.Null, "Receiver retained for queued cleanup");
			if (receiver is null)
				return;
			using (receiver) {
				using var binding = new NSString ("enabled");
				using var replacement = receiver.BindWithLifetime (binding, second, "enabled");
				bool dispatched = false;
				CoreFoundation.DispatchQueue.MainQueue.DispatchAsync (() => dispatched = true);
				Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => dispatched), Is.True, "Main queue drained");
				second.Enabled = false;
				Assert.That (receiver.Enabled, Is.False, "Queued old cleanup did not unbind replacement");
			}
		}

		[Test]
		public void ManualUnbindingInvalidatesToken ()
		{
			using var receiver = new BindingObject ();
			using var observable = new BindingObject ();
			using var binding = new NSString ("enabled");
			using var token = receiver.BindWithLifetime (binding, observable, "enabled");
			receiver.Unbind (binding);
			token.Dispose ();
			Assert.That (receiver.UnbindCount, Is.EqualTo (1), "No duplicate unbinding");
			observable.Enabled = false;
			Assert.That (receiver.Enabled, Is.True, "Manual unbinding stopped updates");
		}

		[Test]
		public void BindingsAreIndependent ()
		{
			using var receiver = new BindingObject ();
			using var observable = new BindingObject ();
			using var enabled = new NSString ("enabled");
			using var selected = new NSString ("selected");
			using var enabledToken = receiver.BindWithLifetime (enabled, observable, "enabled");
			using var selectedToken = receiver.BindWithLifetime (selected, observable, "enabled", (NSBindingOptions?) null);
			enabledToken.Dispose ();
			observable.Enabled = false;
			Assert.That (receiver.Enabled, Is.True, "Disposed binding");
			Assert.That (receiver.Selected, Is.False, "Other binding");
			observable.Enabled = true;
			Assert.That (receiver.Selected, Is.True, "Other binding still updates");
		}

		[Test]
		public void BindingNameCanBeDisposed ()
		{
			using var receiver = new BindingObject ();
			using var observable = new BindingObject ();
			var binding = new NSString ("enabled");
			using var token = receiver.BindWithLifetime (binding, observable, "enabled");
			binding.Dispose ();
			token.Dispose ();
			observable.Enabled = false;
			Assert.That (receiver.Enabled, Is.True, "Token owns its binding name");
		}

		[Test]
		public void InvalidArguments ()
		{
			using var receiver = new BindingObject ();
			using var observable = new BindingObject ();
			using var binding = new NSString ("enabled");
			Assert.Throws<ArgumentNullException> (() => receiver.BindWithLifetime (null, observable, "enabled"), "binding");
			Assert.Throws<ArgumentNullException> (() => receiver.BindWithLifetime (binding, null, "enabled"), "observable");
			Assert.Throws<ArgumentNullException> (() => receiver.BindWithLifetime (binding, observable, null), "keyPath");
		}

		[Test]
		public void FailedBindingDoesNotKeepEndpointsAlive ()
		{
			var (receiver, observable) = FailBinding ();
			WaitForCollection (receiver, observable);
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static (WeakReference Receiver, WeakReference Observable) FailBinding ()
		{
			using var pool = new NSAutoreleasePool ();
			var receiver = new BindingObject ();
			var observable = new BindingObject ();
			using var binding = new NSString ("enabled");
			Assert.Throws<ObjCException> (() => receiver.BindWithLifetime (binding, observable, "missingKey", new NSBindingOptions { RaisesForNotApplicableKeys = true }));
			using var info = receiver.GetBindingInfo (binding);
			Assert.That (info, Is.Null, "Failed native binding was not installed");
			return (new WeakReference (receiver), new WeakReference (observable));
		}

		[Test]
		public void FailedRebindingInvalidatesPreviousToken ()
		{
			using var receiver = new BindingObject ();
			using var first = new BindingObject ();
			using var second = new BindingObject ();
			using var binding = new NSString ("enabled");
			using var token = receiver.BindWithLifetime (binding, first, "enabled");
			Assert.Throws<ObjCException> (() => receiver.BindWithLifetime (binding, second, "missingKey", new NSBindingOptions { RaisesForNotApplicableKeys = true }));
			var unbindCount = receiver.UnbindCount;
			token.Dispose ();
			Assert.That (receiver.UnbindCount, Is.EqualTo (unbindCount), "Failed replacement invalidated old token");
			using var info = receiver.GetBindingInfo (binding);
			Assert.That (info, Is.Null, "Partial replacement removed");
		}

#if !XAMCORE_5_0
		[Test]
		public void PreservesBindOverride ()
		{
			using var receiver = new BindingOverride ();
			using var observable = new BindingObject ();
			using var binding = new NSString ("enabled");
			using var token = receiver.BindWithLifetime (binding, observable, "enabled");
			Assert.That (receiver.BindCount, Is.EqualTo (1), "Existing override called");
			observable.Enabled = false;
			Assert.That (receiver.Enabled, Is.False, "Native binding established");
		}

		[Test]
		public void PreservesNativeBindOverride ()
		{
			using var receiver = new BindingOverride ();
			using var observable = new BindingObject ();
			using var binding = new NSString ("enabled");
			using var keyPath = new NSString ("enabled");
			Messaging.void_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr (
				receiver.Handle, Selector.GetHandle ("bind:toObject:withKeyPath:options:"),
				binding.Handle, observable.Handle, keyPath.Handle, NativeHandle.Zero);
			Assert.That (receiver.BindCount, Is.EqualTo (1), "Objective-C dispatch reaches override");
			receiver.Unbind (binding);
		}

		[TestCase (false)]
		[TestCase (true)]
		public void LegacyRebindingInvalidatesToken (bool typedOptions)
		{
			using var receiver = new BindingObject ();
			using var first = new BindingObject ();
			using var second = new BindingObject ();
			using var binding = new NSString ("enabled");
			using var token = receiver.BindWithLifetime (binding, first, "enabled");
			if (typedOptions)
				receiver.Bind (binding, second, "enabled", new NSBindingOptions ());
			else
				receiver.Bind (binding, second, "enabled", (NSDictionary?) null);
			token.Dispose ();
			second.Enabled = false;
			Assert.That (receiver.Enabled, Is.False, "Legacy replacement still active");
			receiver.Unbind (binding);
		}
#endif
	}
}
#endif
