using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSCharacterSetTest {
		static void RequiresIos8 ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
		}

		[Test]
		public void NSMutableCharacterSet_TestStaticSets ()
		{
			RequiresIos8 ();

			TestSet (NSMutableCharacterSet.Alphanumerics, "Alphanumerics", 'a');
			TestSet (NSMutableCharacterSet.Capitalized, "Capitalized", '\u01C5');
			TestSet (NSMutableCharacterSet.Controls, "Controls", '\u0000');
			TestSet (NSMutableCharacterSet.DecimalDigits, "DecimalDigits", '1');
			TestSet (NSMutableCharacterSet.Decomposables, "Decomposables", '\u00e9');
			TestSet (NSMutableCharacterSet.Illegals, "Illegals", '\uFFFE');
			TestSet (NSMutableCharacterSet.Letters, "Letters", 'a');
			TestSet (NSMutableCharacterSet.LowercaseLetters, "LowercaseLetters", 'a');
			TestSet (NSMutableCharacterSet.Newlines, "Newlines", '\n');
			TestSet (NSMutableCharacterSet.Marks, "Marks", '\u20DD');
			TestSet (NSMutableCharacterSet.Punctuation, "Punctuation", '.');
			TestSet (NSMutableCharacterSet.Symbols, "Symbols", '~');
			TestSet (NSMutableCharacterSet.UppercaseLetters, "UppercaseLetters", 'A');
			TestSet (NSMutableCharacterSet.WhitespaceAndNewlines, "WhitespaceAndNewlines", ' ');
			TestSet (NSMutableCharacterSet.Whitespaces, "Whitespaces", ' ');
		}

		void TestSet (NSCharacterSet s, string setName, char characterThatShouldBeInSet)
		{
			RequiresIos8 ();

			Assert.IsNotNull (s, setName + " was null");
			Assert.IsTrue (s.Contains (characterThatShouldBeInSet), setName + " did not contain: " + characterThatShouldBeInSet);
		}
	}
}
