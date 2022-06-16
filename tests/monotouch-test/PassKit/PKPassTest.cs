#if !__TVOS__ && !MONOMAC

using System;
using Foundation;
using UIKit;
using PassKit;
using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PKPassTest {

		[Test]
		public void GetLocalizedValueNull ()
		{
			using var pass = new PKPass ();
			Assert.IsNull (pass.GetLocalizedValue (new NSString ()), "'PKPass.GetLocalizedValue' is not returning a null value");
		}
	}
}

#endif
