// Copyright 2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AlertViewDelegateTest {

#if false
		// this occurs only on the simulator where we use a different code path to register classes
		[Test]
		public void Default ()
		{
			using (var d = new UIAlertViewDelegate ()) {
				Assert.That (d.Handle, Is.EqualTo (IntPtr.Zero), "Handle");
			}
		}
#endif

		class MyAlertViewDelegate2 : UIAlertViewDelegate {
		}

		[Test]
		public void MyDefault ()
		{
			using (var d = new MyAlertViewDelegate2 ()) {
				Assert.That (d.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
