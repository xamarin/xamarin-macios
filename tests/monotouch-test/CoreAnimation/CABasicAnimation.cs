#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CABasicAnimationTests {
		[Test]
		public void CABasicAnimation_FromToBy_INativeTests ()
		{
			CABasicAnimation test = CABasicAnimation.FromKeyPath ("bounds");
			NSNumber number = new NSNumber (10);
			test.From = number;
			Assert.AreEqual (test.From, number, "NSObject from");
			test.To = number;
			Assert.AreEqual (test.To, number, "NSObject to");
			test.By = number;
			Assert.AreEqual (test.By, number, "NSObject by");

			CGColor color = new CGColor (.5f, .5f, .5f);
			test = CABasicAnimation.FromKeyPath ("color");
			test.SetFrom (color);
			Assert.AreEqual (test.GetFromAs<CGColor> (), color, "INativeObject from");
			test.SetTo (color);
			Assert.AreEqual (test.GetToAs<CGColor> (), color, "INativeObject to");
			test.SetBy (color);
			Assert.AreEqual (test.GetByAs<CGColor> (), color, "INativeObject by");
		}
	}
}
#endif // __MACOS__
