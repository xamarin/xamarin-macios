// Copyright 2015 Xamarin Inc.

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using CoreFoundation;
using Foundation;
using PushKit;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.PushKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PushRegistryTest {

		[Test]
		public void CtorDispatchQueue ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			if (!TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 8, 2, throwIfOtherPlatform: false) && IntPtr.Size == 4)
				Assert.Inconclusive ("Requires iOS 8.2 or later in 32-bit mode.");

			using (var dq = new DispatchQueue ("pk-test-queue"))
			using (var pr = new PKPushRegistry (dq)) {
				Assert.Null (pr.Delegate, "Delegate");
				Assert.Null (pr.DesiredPushTypes, "DesiredPushTypes");
				Assert.Null (pr.WeakDelegate, "WeakDelegate");

				// it's nullable (setting a value needs more app setup or ObjC exceptions will occurs later)
				pr.DesiredPushTypes = null;
				Assert.Null (pr.DesiredPushTypes, "DesiredPushTypes-2");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
