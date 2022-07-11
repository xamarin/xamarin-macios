#if __IOS__

using System;

using Foundation;
using ObjCRuntime;
using UIKit;

using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class KeyCommandTest {

		[Test]
		public void Create ()
		{
			using (var key = new NSString ("a")) {
				Assert.NotNull (UIKeyCommand.Create (key, UIKeyModifierFlags.Command, new Selector ("foo")), "Create");
			}
		}
	}
}

#endif
