#if __MACOS__
using System;
using NUnit.Framework;

using Foundation;
using AppKit;

namespace apitest {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSFormatterTests {
		NSNumberFormatter formatter;

		[SetUp]
		public void SetUp ()
		{
			formatter = new NSNumberFormatter ();
			formatter.NumberStyle = NSNumberFormatterStyle.Currency;
			formatter.Locale = NSLocale.FromLocaleIdentifier ("en-US");
		}

		[Test]
		public void NSFormatter_ShouldGetString ()
		{
			var str = formatter.StringFor (NSNumber.FromFloat (0.12f));

			Assert.AreEqual (str, "$0.12");
		}

		[Test]
		public void NSFormatter_ShouldGetAttributedString ()
		{
			var str = formatter.GetAttributedString (NSNumber.FromFloat (3.21f), new NSStringAttributes () { Font = NSFont.SystemFontOfSize (8) });

			Assert.AreEqual (str.Value, "$3.21");
		}

		[Test]
		public void NSFormatter_ShouldGetEditingString ()
		{
			var str = formatter.EditingStringFor (NSNumber.FromInt32 (14));

			Assert.AreEqual (str, "$14.00");
		}

		[Test]
		public void NSFormatter_IsPartialStringValid ()
		{
			string newstr;
			NSString error;
			formatter.PartialStringValidationEnabled = true;
			var valid = formatter.IsPartialStringValid ("valid string", out newstr, out error);

			Assert.IsTrue (valid);
		}
	}
}

#endif // __MACOS__
