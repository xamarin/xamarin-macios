using System;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MessagingTest {
		[Test]
		public void NullReceiver ()
		{
			// null receiver and valid selector
			global::ObjCRuntime.Messaging.void_objc_msgSend (IntPtr.Zero, global::ObjCRuntime.Selector.GetHandle ("release"));

			// null receiver and null selector - that null-selector only works because there's not receiver
			global::ObjCRuntime.Messaging.void_objc_msgSend (IntPtr.Zero, IntPtr.Zero);

#if false
			// this disabled code would crash because selector is null (and there's a receiver)
			var obj = new NSData ();
			global::ObjCRuntime.Messaging.void_objc_msgSend (obj.Handle, IntPtr.Zero);
#endif
		}

		[Test]
		public void SuperNullReceiver ()
		{
			var obj = new NSData ();
			// notes:
			// * `IntPtr.Zero` would crash since it's used (without check) to get both `self` and `super`
			// * unlike `objc_msgSend` a null selector would crash in this case
			global::ObjCRuntime.Messaging.void_objc_msgSendSuper (obj.SuperHandle, global::ObjCRuntime.Selector.GetHandle ("retain"));

			obj.Handle = IntPtr.Zero;
			global::ObjCRuntime.Messaging.void_objc_msgSendSuper (obj.SuperHandle, global::ObjCRuntime.Selector.GetHandle ("release"));
			// a null `Handle` just skip sending the message (as `objc_msgSend` does)

#if false
			// old bindings were using this structure (and tests still has a copy of it)
			// this allows us to set `super` to `IntPtr.Zero` (defaut), which is not (easily) possible using our NSObject
			var old = new Messaging.objc_super ();
			// here both `self/Handle` and `super/SuperHandle` are `null` and this would crash, so `super` is super important
			global::ObjCRuntime.Messaging.void_objc_msgSendSuper (ref old, global::ObjCRuntime.Selector.GetHandle ("retain"));
#endif
		}
	}
}
