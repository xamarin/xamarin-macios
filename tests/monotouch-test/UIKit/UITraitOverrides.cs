//
// Unit tests for UITraitOverrides
//
// Authors:
//	Rolf Bjarne Kvinge  <rolf@xamarin.com>
//
// Copyright 2023 Microsoft Corp. All rights reserved.
//

#if !__MACOS__

using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

using UIKit;

using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UITraitOverridesTest {

		[Test]
		public void InlinedInUIViewController ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			IUITraitChangeRegistration token;
			using var obj = new NSObject ();
			using var observable = new UIViewController ();

			// RegisterForTraitChanges (Type [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass) }, (a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Action<IUITraitEnvironment, UITraitCollection> handler, params Type [] traits)
			token = observable.RegisterForTraitChanges ((a, b) => { }, typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2, T3> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass, UITraitHorizontalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2, T3, T4> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass, UITraitHorizontalSizeClass, UITraitVerticalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Type [] traits, NSObject target, Selector action)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass) }, obj, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Type [] traits, Selector action)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass) }, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, [BlockProxy (typeof (ObjCRuntime.Trampolines.IUITraitChangeObservable_RegisterForTraitChanges_NIDAction))] global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, (a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, NSObject target, Selector action)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, obj, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, Selector action)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");
		}

		[Test]
		public void InlinedInUIView ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			IUITraitChangeRegistration token;
			using var obj = new NSObject ();
			using var observable = new UIView ();

			// RegisterForTraitChanges (Type [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass) }, (a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Action<IUITraitEnvironment, UITraitCollection> handler, params Type [] traits)
			token = observable.RegisterForTraitChanges ((a, b) => { }, typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2, T3> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass, UITraitHorizontalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2, T3, T4> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass, UITraitHorizontalSizeClass, UITraitVerticalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Type [] traits, NSObject target, Selector action)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass) }, obj, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Type [] traits, Selector action)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass) }, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, [BlockProxy (typeof (ObjCRuntime.Trampolines.IUITraitChangeObservable_RegisterForTraitChanges_NIDAction))] global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, (a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, NSObject target, Selector action)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, obj, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, Selector action)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");
		}

		[Test]
		public void InlinedInUIWindowScene ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			IUITraitChangeRegistration token;
			using var obj = new NSObject ();
			UIWindowScene observable = UIApplication.SharedApplication.Windows [0].WindowScene;

			// RegisterForTraitChanges (Type [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass) }, (a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Action<IUITraitEnvironment, UITraitCollection> handler, params Type [] traits)
			token = observable.RegisterForTraitChanges ((a, b) => { }, typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2, T3> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass, UITraitHorizontalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2, T3, T4> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass, UITraitHorizontalSizeClass, UITraitVerticalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Type [] traits, NSObject target, Selector action)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass) }, obj, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Type [] traits, Selector action)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass) }, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, [BlockProxy (typeof (ObjCRuntime.Trampolines.IUITraitChangeObservable_RegisterForTraitChanges_NIDAction))] global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, (a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, NSObject target, Selector action)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, obj, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, Selector action)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");
		}

		[Test]
		public void InlinedInUIPresentationController ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			IUITraitChangeRegistration token;
			using var obj = new NSObject ();
			using var vc = new UIViewController ();
			using var observable = new UIPresentationController (vc, null);

			// RegisterForTraitChanges (Type [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass) }, (a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Action<IUITraitEnvironment, UITraitCollection> handler, params Type [] traits)
			token = observable.RegisterForTraitChanges ((a, b) => { }, typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2, T3> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass, UITraitHorizontalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges<T1, T2, T3, T4> (Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges<UITraitVerticalSizeClass, UITraitHorizontalSizeClass, UITraitHorizontalSizeClass, UITraitVerticalSizeClass> ((a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Type [] traits, NSObject target, Selector action)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass) }, obj, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Type [] traits, Selector action)
			token = observable.RegisterForTraitChanges (new [] { typeof (UITraitVerticalSizeClass), typeof (UITraitHorizontalSizeClass) }, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, [BlockProxy (typeof (ObjCRuntime.Trampolines.IUITraitChangeObservable_RegisterForTraitChanges_NIDAction))] global::System.Action<IUITraitEnvironment, UITraitCollection> handler)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, (a, b) => { });
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, NSObject target, Selector action)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, obj, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			// RegisterForTraitChanges (Class [] traits, Selector action)
			token = observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) }, new Selector ("notifyTraitChange:collection:"));
			observable.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");
		}

		[TestCase (false)]
		[TestCase (true)]
		public void RegisterForTraitChanges_ClassArray (bool disposeRegistration)
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			CallbackTest ((vc, callback) => {
				return vc.RegisterForTraitChanges (new [] {
						new Class (typeof (UITraitVerticalSizeClass)),
						new Class (typeof (UITraitHorizontalSizeClass)),
						}, callback);
			}, "ClassArray ", disposeRegistration);
		}

		[TestCase (false)]
		[TestCase (true)]
		public void RegisterForTraitChanges_InterfaceAndExtensions (bool useExtensionMethods)
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var view = new TraitObservableView ();
			IUITraitChangeObservable observable = view;
			var traits = new [] { new Class (typeof (UITraitVerticalSizeClass)) };
			var action = new Selector ("notifyTraitChange:collection:");
			using var callbackRegistration = useExtensionMethods ?
				UITraitChangeObservable_Extensions.RegisterForTraitChanges (observable, traits, (a, b) => { }) :
				observable.RegisterForTraitChanges (traits, (a, b) => { });
			using var targetRegistration = useExtensionMethods ?
				UITraitChangeObservable_Extensions.RegisterForTraitChanges (observable, traits, view, action) :
				observable.RegisterForTraitChanges (traits, view, action);
			using var actionRegistration = useExtensionMethods ?
				UITraitChangeObservable_Extensions.RegisterForTraitChanges (observable, traits, action) :
				observable.RegisterForTraitChanges (traits, action);
			Assert.That (view.RegistrationCount, Is.EqualTo (3), "Registration overrides");

			foreach (var registration in new [] { callbackRegistration, targetRegistration, actionRegistration }) {
				Assert.That (registration.Handle, Is.Not.EqualTo (NativeHandle.Zero), "Registered");
				observable.UnregisterForTraitChanges (registration);
				Assert.That (registration.Handle, Is.EqualTo (NativeHandle.Zero), "Unregistered");
			}
			Assert.That (view.UnregistrationCount, Is.EqualTo (3), "Unregistration overrides");
		}

		[Test]
		public void RegisterForTraitChanges_Overrides ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var view = new TraitObservableView ();
			var traits = new [] { new Class (typeof (UITraitVerticalSizeClass)) };
			var action = new Selector ("notifyTraitChange:collection:");
			using var callbackRegistration = view.RegisterForTraitChanges (traits, (a, b) => { });
			using var targetRegistration = view.RegisterForTraitChanges (traits, view, action);
			using var actionRegistration = view.RegisterForTraitChanges (traits, action);
			Assert.That (view.RegistrationCount, Is.EqualTo (3), "Managed registration overrides");

			Messaging.void_objc_msgSend_IntPtr (view.Handle, new Selector ("unregisterForTraitChanges:").Handle, actionRegistration.Handle);
			Assert.That (view.UnregistrationCount, Is.EqualTo (1), "Native unregistration override");

			view.UnregisterForTraitChanges (callbackRegistration);
			Assert.That (view.UnregistrationCount, Is.EqualTo (2), "Managed unregistration override");
			Assert.That (callbackRegistration.Handle, Is.EqualTo (NativeHandle.Zero), "Managed unregistration disposed token");

			targetRegistration.Dispose ();
			Assert.That (view.UnregistrationCount, Is.EqualTo (3), "Dispose unregistration override");

			using var array = NSArray.FromNSObjects (traits);
			var handle = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (view.Handle,
				new Selector ("registerForTraitChanges:withAction:").Handle, array.Handle, action.Handle);
			Assert.That (handle, Is.Not.EqualTo (IntPtr.Zero), "Native registration");
			Assert.That (view.RegistrationCount, Is.EqualTo (4), "Native registration override");
			view.LastRegistration?.Dispose ();
			GC.KeepAlive (view);
			GC.KeepAlive (action);
		}

		[Register ("TraitObservableView")]
		class TraitObservableView : UIView {
			public int RegistrationCount;
			public int UnregistrationCount;
			public IUITraitChangeRegistration? LastRegistration;

			public override IUITraitChangeRegistration RegisterForTraitChanges (Class [] traits, Action<IUITraitEnvironment, UITraitCollection> handler)
			{
				RegistrationCount++;
				return LastRegistration = base.RegisterForTraitChanges (traits, handler);
			}

			public override IUITraitChangeRegistration RegisterForTraitChanges (Class [] traits, NSObject target, Selector action)
			{
				RegistrationCount++;
				return LastRegistration = base.RegisterForTraitChanges (traits, target, action);
			}

			public override IUITraitChangeRegistration RegisterForTraitChanges (Class [] traits, Selector action)
			{
				RegistrationCount++;
				return LastRegistration = base.RegisterForTraitChanges (traits, action);
			}

			public override void UnregisterForTraitChanges (IUITraitChangeRegistration registration)
			{
				UnregistrationCount++;
				base.UnregisterForTraitChanges (registration);
			}
		}

		[Test]
		public void RegisterForTraitChanges_Copy ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var view = new UIView ();
			using var registration = view.RegisterForTraitChanges<UITraitVerticalSizeClass> ((a, b) => { });
			Assert.Throws<NotSupportedException> (() => registration.Copy (null), "Copy");
			Assert.That (registration.Handle, Is.Not.EqualTo (NativeHandle.Zero), "Original remains registered");

			registration.Dispose ();
			Assert.Throws<ObjectDisposedException> (() => registration.Copy (null), "Copy after disposal");
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static IUITraitChangeRegistration CreateRegistration (out WeakReference observable)
		{
			var view = new UIView ();
			observable = new WeakReference (view);
			return view.RegisterForTraitChanges<UITraitVerticalSizeClass> ((a, b) => { });
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static WeakReference AbandonRegistration (out WeakReference observable)
		{
			return new WeakReference (CreateRegistration (out observable), trackResurrection: true);
		}

		[Register ("TraitChangeTarget")]
		class TraitChangeTarget : NSObject {
			readonly Action callback;

			public TraitChangeTarget (Action callback)
			{
				this.callback = callback;
			}

			[Export ("traitChanged:previousCollection:")]
			void TraitChanged (IUITraitEnvironment environment, UITraitCollection previousCollection)
			{
				callback ();
			}
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static IUITraitChangeRegistration CreateTargetRegistration (UIViewController observable, Action callback, out WeakReference target)
		{
			var receiver = new TraitChangeTarget (callback);
			target = new WeakReference (receiver);
			return observable.RegisterForTraitChanges (new [] { new Class (typeof (UITraitVerticalSizeClass)) },
				receiver, new Selector ("traitChanged:previousCollection:"));
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static WeakReference AbandonTargetRegistration (out WeakReference observable, out WeakReference target)
		{
			var controller = new UIViewController ();
			observable = new WeakReference (controller);
			return new WeakReference (CreateTargetRegistration (controller, () => { }, out target), trackResurrection: true);
		}

		[TestCase (false)]
		[TestCase (true)]
		public void RegisterForTraitChanges_RootsTarget (bool disposeRegistration)
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var observable = new UIViewController ();
			var callbacks = 0;
			using var registration = CreateTargetRegistration (observable, () => callbacks++, out var target);
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Assert.That (target.IsAlive, Is.True, "Target rooted");

			var initial = observable.TraitCollection.VerticalSizeClass;
			observable.TraitOverrides.VerticalSizeClass = initial == UIUserInterfaceSizeClass.Regular ?
				UIUserInterfaceSizeClass.Compact : UIUserInterfaceSizeClass.Regular;
			Assert.That (callbacks, Is.EqualTo (1), "Callback after collection");

			if (disposeRegistration)
				registration.Dispose ();
			else
				observable.UnregisterForTraitChanges (registration);

			Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				return !target.IsAlive;
			}), Is.True, "Target released");
			observable.TraitOverrides.VerticalSizeClass = initial;
			Assert.That (callbacks, Is.EqualTo (1), "Callback unregistered");
			GC.KeepAlive (registration);
		}

		[Test]
		public void RegisterForTraitChanges_TargetFinalizer ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var registration = AbandonTargetRegistration (out var observable, out var target);
			Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				return !registration.IsAlive && !observable.IsAlive && !target.IsAlive;
			}), Is.True, "Registration finalized and observable and target released");
		}

		[Test]
		public void RegisterForTraitChanges_RootsObservable ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var registration = CreateRegistration (out var observable);
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Assert.That (observable.IsAlive, Is.True, "Observable rooted");

			registration.Dispose ();
			Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				return !observable.IsAlive;
			}), Is.True, "Observable released");
			GC.KeepAlive (registration);
		}

		[Test]
		public void RegisterForTraitChanges_Finalizer ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var registration = AbandonRegistration (out var observable);
			Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				return !registration.IsAlive && !observable.IsAlive;
			}), Is.True, "Registration finalized and observable released");
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static WeakReference AbandonNativeRetainedRegistration (UIView parent, Action callback, bool useTarget, out WeakReference observable, out WeakReference? target)
		{
			var view = new UIView ();
			parent.AddSubview (view);
			observable = new WeakReference (view);
			var traits = new [] { new Class (typeof (UITraitVerticalSizeClass)) };
			IUITraitChangeRegistration registration;
			if (useTarget) {
				var receiver = new TraitChangeTarget (callback);
				target = new WeakReference (receiver);
				registration = view.RegisterForTraitChanges (traits, receiver, new Selector ("traitChanged:previousCollection:"));
			} else {
				target = null;
				registration = view.RegisterForTraitChanges (traits, (environment, previousCollection) => callback ());
			}
			view.TraitOverrides.VerticalSizeClass = UIUserInterfaceSizeClass.Compact;
			view.UpdateTraitsIfNeeded ();
			view.TraitOverrides.VerticalSizeClass = UIUserInterfaceSizeClass.Regular;
			view.UpdateTraitsIfNeeded ();
			return new WeakReference (registration, trackResurrection: true);
		}

		[TestCase (false)]
		[TestCase (true)]
		public void RegisterForTraitChanges_FinalizerWithNativeRetainedObservable (bool useTarget)
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var parent = new UIView ();
			var callbacks = 0;
			var registration = AbandonNativeRetainedRegistration (parent, () => callbacks++, useTarget, out var observable, out var target);
			Assert.That (callbacks, Is.GreaterThan (0), "Callback registered");

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Assert.That (registration.IsAlive, Is.True, "Token rooted until main queue cleanup");
			Assert.That (observable.IsAlive, Is.True, "Observable rooted until native unregister");
			if (target is not null)
				Assert.That (target.IsAlive, Is.True, "Target rooted until native unregister");

			Assert.That (TestRuntime.RunAsync (TimeSpan.FromSeconds (10), () => {
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				return !registration.IsAlive && (target is null || !target.IsAlive);
			}), Is.True, "Deferred cleanup completed");

			var previousCallbacks = callbacks;
			using var view = parent.Subviews [0];
			view.TraitOverrides.VerticalSizeClass = UIUserInterfaceSizeClass.Compact;
			view.UpdateTraitsIfNeeded ();
			view.TraitOverrides.VerticalSizeClass = UIUserInterfaceSizeClass.Regular;
			view.UpdateTraitsIfNeeded ();
			Assert.That (callbacks, Is.EqualTo (previousCallbacks), "No callback after deferred unregister");
			view.RemoveFromSuperview ();
		}

		[Test]
		public void RegisterForTraitChanges_TypeArray ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			CallbackTest ((vc, callback) => {
				return vc.RegisterForTraitChanges (new [] {
						typeof (UITraitVerticalSizeClass),
						typeof (UITraitHorizontalSizeClass),
						}, callback);
			}, "TypeArray ");
		}

		[Test]
		public void RegisterForTraitChanges_ParamsTypeArray ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			CallbackTest ((vc, callback) => {
				return vc.RegisterForTraitChanges (callback,
						typeof (UITraitVerticalSizeClass),
						typeof (UITraitHorizontalSizeClass));
			}, "ParamsTypeArray ");
		}

		[Test]
		public void RegisterForTraitChanges_Generic ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			CallbackTest ((vc, callback) => {
				// return vc.RegisterForTraitChanges<NSObject> (callback);
				return vc.RegisterForTraitChanges<UITraitHorizontalSizeClass, UITraitVerticalSizeClass> (callback);
			}, "Generic ");
		}

		void CallbackTest (Func<IUITraitChangeObservable, Action<IUITraitEnvironment, UITraitCollection>, IUITraitChangeRegistration> registerFunc, string prefix, bool disposeRegistration = false)
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			using var vc = new UIViewController ();
			using var label = new UILabel () {
				TextAlignment = UITextAlignment.Center,
			};
			vc.View!.AddSubview (label);

			UIUserInterfaceSizeClass? horizontal = null;
			UIUserInterfaceSizeClass? vertical = null;
			var msgPrefix = "?";
			var callbackCounter = 0;
			Exception? ex = null;

			Action<IUITraitEnvironment, UITraitCollection> callback = (env, coll) => {
				try {
					Assert.That (vc.TraitOverrides.ContainsTrait<UITraitHorizontalSizeClass> (), Is.EqualTo (horizontal is not null), $"{prefix}{msgPrefix} Horizontal A");
					Assert.That (vc.TraitOverrides.ContainsTrait (typeof (UITraitHorizontalSizeClass)), Is.EqualTo (horizontal is not null), $"{prefix}{msgPrefix} Horizontal B");
					Assert.That (vc.TraitOverrides.ContainsTrait (new Class (typeof (UITraitHorizontalSizeClass))), Is.EqualTo (horizontal is not null), $"{prefix}{msgPrefix} Horizontal C");
					Assert.That (vc.TraitOverrides.ContainsTrait<UITraitVerticalSizeClass> (), Is.EqualTo (vertical is not null), $"{prefix}{msgPrefix} Vertical A");
					Assert.That (vc.TraitOverrides.ContainsTrait (typeof (UITraitVerticalSizeClass)), Is.EqualTo (vertical is not null), $"{prefix}{msgPrefix} Vertical B");
					Assert.That (vc.TraitOverrides.ContainsTrait (new Class (typeof (UITraitVerticalSizeClass))), Is.EqualTo (vertical is not null), $"{prefix}{msgPrefix} Vertical C");
					if (horizontal is not null) {
						Assert.That (vc.TraitOverrides.HorizontalSizeClass, Is.EqualTo (horizontal.Value), $"{prefix}{msgPrefix} Horizontal Value");
					}
					if (vertical is not null) {
						Assert.That (vc.TraitOverrides.VerticalSizeClass, Is.EqualTo (vertical.Value), $"{prefix}{msgPrefix} Vertical Value");
					}
					callbackCounter++;
				} catch (Exception e) {
					ex = e;
				}
			};

			// Figure out the initial values so that we can choose a different value when setting them.
			var tc = UITraitCollection.CurrentTraitCollection;
			UIUserInterfaceSizeClass? initialHorizonal = null;
			UIUserInterfaceSizeClass? initialVertical = null;
			tc.PerformAsCurrentTraitCollection (() => {
				initialHorizonal = tc.HorizontalSizeClass;
				initialVertical = tc.VerticalSizeClass;
			});

			var firstHorizontal = (initialHorizonal == UIUserInterfaceSizeClass.Regular) ? UIUserInterfaceSizeClass.Compact : UIUserInterfaceSizeClass.Regular;
			var firstVertical = (initialVertical == UIUserInterfaceSizeClass.Regular) ? UIUserInterfaceSizeClass.Compact : UIUserInterfaceSizeClass.Regular;
			var secondHorizontal = (initialHorizonal == UIUserInterfaceSizeClass.Regular) ? UIUserInterfaceSizeClass.Regular : UIUserInterfaceSizeClass.Compact;
			var secondVertical = (initialVertical == UIUserInterfaceSizeClass.Regular) ? UIUserInterfaceSizeClass.Regular : UIUserInterfaceSizeClass.Compact;

			using var token = registerFunc (vc, callback);

			horizontal = firstHorizontal;
			vc.TraitOverrides.HorizontalSizeClass = horizontal.Value;
			Assert.That (callbackCounter, Is.EqualTo (1), $"{prefix}CallbackCounter 1");
			Assert.That (ex, Is.Null, $"{prefix}Exception 1");

			horizontal = secondHorizontal;
			vc.TraitOverrides.HorizontalSizeClass = horizontal.Value;
			Assert.That (callbackCounter, Is.EqualTo (2), $"{prefix}CallbackCounter 2");
			Assert.That (ex, Is.Null, $"{prefix}Exception 2");

			vertical = firstVertical;
			vc.TraitOverrides.VerticalSizeClass = vertical.Value;
			Assert.That (callbackCounter, Is.EqualTo (3), $"{prefix}CallbackCounter 3");
			Assert.That (ex, Is.Null, $"{prefix}Exception 3");

			vertical = secondVertical;
			vc.TraitOverrides.VerticalSizeClass = vertical.Value;
			Assert.That (callbackCounter, Is.EqualTo (4), $"{prefix}CallbackCounter 4");
			Assert.That (ex, Is.Null, $"{prefix}Exception 4");

			vertical = null;
			vc.TraitOverrides.RemoveTrait (typeof (UITraitVerticalSizeClass));
			Assert.That (callbackCounter, Is.EqualTo (4), $"{prefix}CallbackCounter 5");
			Assert.That (ex, Is.Null, $"{prefix}Exception 5");

			horizontal = null;
			vc.TraitOverrides.RemoveTrait<UITraitHorizontalSizeClass> ();
			Assert.That (callbackCounter, Is.EqualTo (4), $"{prefix}CallbackCounter 6");
			Assert.That (ex, Is.Null, $"{prefix}Exception 6");

			horizontal = firstHorizontal;
			vc.TraitOverrides.HorizontalSizeClass = horizontal.Value;
			Assert.That (callbackCounter, Is.EqualTo (5), $"{prefix}CallbackCounter 7");
			Assert.That (ex, Is.Null, $"{prefix}Exception 7");

			horizontal = null;
			vc.TraitOverrides.RemoveTrait (new Class (typeof (UITraitHorizontalSizeClass)));
			Assert.That (callbackCounter, Is.EqualTo (5), $"{prefix}CallbackCounter 8");
			Assert.That (ex, Is.Null, $"{prefix}Exception 8");

			if (disposeRegistration)
				token.Dispose ();
			else
				vc.UnregisterForTraitChanges (token);
			Assert.That (token.Handle, Is.EqualTo (NativeHandle.Zero), "Registration disposed");

			vc.TraitOverrides.HorizontalSizeClass = firstHorizontal;
			Assert.That (callbackCounter, Is.EqualTo (5), $"{prefix}CallbackCounter after unregister");
			Assert.That (ex, Is.Null, $"{prefix}Exception after unregister");
		}
	}
}

#endif // !__MACOS__
