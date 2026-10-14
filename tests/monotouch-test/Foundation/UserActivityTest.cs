// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace MonoTouchFixtures.Foundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UserActivityTest {
		[Preserve (AllMembers = true)]
		class WeakUserActivityDelegate : NSObject {
			public int SaveCount { get; private set; }

			[Export ("userActivityWillSave:")]
			public void WillSave (NSUserActivity activity)
			{
				SaveCount++;
			}
		}

		[Preserve (AllMembers = true)]
		class TrackingUserActivity : NSUserActivity {
			public int GetterCount { get; private set; }
			public int SetterCount { get; private set; }

			public TrackingUserActivity () : base ("com.microsoft.macios.wrapped-protocol")
			{
			}

			public override NSObject WeakDelegate {
				get {
					GetterCount++;
					return base.WeakDelegate;
				}
				set {
					SetterCount++;
					base.WeakDelegate = value;
				}
			}
		}

		[TestCase (false)]
		[TestCase (true)]
		public void ProtocolWrapper (bool subclass)
		{
			using var activity = subclass ? new TrackingUserActivity () : new NSUserActivity ("com.microsoft.macios.wrapped-protocol");
			using var weakDelegate = new WeakUserActivityDelegate ();
			activity.WeakDelegate = weakDelegate;

			var wrapped = activity.Delegate;
			Assert.That (wrapped, Is.Not.Null, "Wrapped delegate");
			using var disposable = (IDisposable) wrapped;
			Assert.That (wrapped, Is.Not.InstanceOf<NSObject> (), "Protocol wrapper");
			Assert.That (wrapped.Handle, Is.EqualTo (weakDelegate.Handle), "Getter handle");
			wrapped.UserActivityWillSave (activity);
			Assert.That (weakDelegate.SaveCount, Is.EqualTo (1), "Protocol method");

			activity.Delegate = wrapped;
			Assert.That (activity.WeakDelegate, Is.SameAs (weakDelegate), "Setter preserves NSObject identity");
			if (activity is TrackingUserActivity tracking) {
				Assert.That (tracking.GetterCount, Is.EqualTo (2), "Weak getter override");
				Assert.That (tracking.SetterCount, Is.EqualTo (2), "Weak setter override");
			}

			activity.Delegate = null;
			Assert.That (activity.WeakDelegate, Is.Null, "Cleared weak delegate");
			Assert.That (activity.Delegate, Is.Null, "Cleared delegate");
		}

		[TestCase (false)]
		[TestCase (true)]
		public void ProtocolWrapperWithoutNSObject (bool subclass)
		{
			using var activity = subclass ? new TrackingUserActivity () : new NSUserActivity ("com.microsoft.macios.wrapped-protocol");
			using var original = new NSObject ();
			var wrapped = Runtime.GetINativeObject<INSUserActivityDelegate> (original.Handle, false);
			using var disposable = (IDisposable) wrapped;
			original.Dispose ();

			activity.Delegate = wrapped;
			using var weakDelegate = activity.WeakDelegate;
			Assert.That (weakDelegate, Is.Not.Null, "Resolved NSObject");
			Assert.That (weakDelegate, Is.Not.SameAs (original), "New NSObject");
			Assert.That (weakDelegate.Handle, Is.EqualTo (wrapped.Handle), "Setter handle");
			if (activity is TrackingUserActivity tracking) {
				Assert.That (tracking.GetterCount, Is.EqualTo (1), "Weak getter override");
				Assert.That (tracking.SetterCount, Is.EqualTo (1), "Weak setter override");
			}
			activity.Delegate = null;
		}

		[TestCase (false)]
		[TestCase (true)]
		public void ManagedDelegate (bool subclass)
		{
			using var activity = subclass ? new TrackingUserActivity () : new NSUserActivity ("com.microsoft.macios.wrapped-protocol");
			using var managedDelegate = new NSUserActivityDelegate ();
			activity.Delegate = managedDelegate;
			Assert.That (activity.Delegate, Is.SameAs (managedDelegate), "Managed delegate identity");
			if (activity is TrackingUserActivity tracking) {
				Assert.That (tracking.GetterCount, Is.EqualTo (1), "Weak getter override");
				Assert.That (tracking.SetterCount, Is.EqualTo (1), "Weak setter override");
			}
			activity.Delegate = null;
			Assert.That (activity.Delegate, Is.Null, "Cleared delegate");
		}
	}
}
