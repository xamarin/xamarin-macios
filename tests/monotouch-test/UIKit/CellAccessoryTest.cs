#if __IOS__ || __TVOS__

using System;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CellAccessoryTest {

		[Test]
		public void GetPositionBeforeAccessory ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			var cap = UICellAccessory.GetPositionBeforeAccessory (new Class ("UIButton"));
			Assert.NotNull (cap, "Class/cap");
			Assert.That (cap (new UICellAccessory [0]), Is.EqualTo ((nuint) 0), "Class/Invoke");

			cap = UICellAccessory.GetPositionBeforeAccessory (typeof (UICellAccessory));
			Assert.NotNull (cap, "Type/cap");
			Assert.That (cap (new UICellAccessory [0]), Is.EqualTo ((nuint) 0), "Type/Invoke");
		}

		[Test]
		public void GetPositionAfterAccessory ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			var cap = UICellAccessory.GetPositionAfterAccessory (new Class ("UIButton"));
			Assert.NotNull (cap, "Class/cap");
			Assert.That (cap (new UICellAccessory [0]), Is.EqualTo ((nuint) 0), "Class/Invoke");

			cap = UICellAccessory.GetPositionAfterAccessory (typeof (UICellAccessory));
			Assert.NotNull (cap, "Type/cap");
			Assert.That (cap (new UICellAccessory [0]), Is.EqualTo ((nuint) 0), "Type/Invoke");
		}
	}
}

#endif
