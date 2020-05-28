using System;
using NUnit.Framework;

using AppKit;
using CoreGraphics;
using Foundation;

namespace apitest
{
	[TestFixture]
	public class NSTextInputClient
	{
		NSTextView textView;

		[SetUp]
		public void SetUp ()
		{
			textView = new NSTextView (new CGRect (0, 0, 37, 120));
			textView.Value = "This is a new string";
			Assert.AreEqual (textView.Value, "This is a new string", "NSTextInputClientSetup - Failed to set value");
		}

		[TearDown]
		public void TearDown ()
		{
			textView.Dispose ();
		}

		[Test]
		public void NSTextInputClient_ShouldInsertText ()
		{
			textView.InsertText ((NSString)"Test", new NSRange (5, 4));

			Assert.AreEqual (textView.Value, "This Test new string", "NSTextInputClient_ShouldInsertText - Failed to insert text");
		}

		[Test]
		public void NSTextInputClient_ShouldMarkText ()
		{
			textView.SetMarkedText ((NSString)"Testing", new NSRange (0, 10), new NSRange (5, 4));

			Assert.IsTrue (textView.HasMarkedText, "NSTextInputClient_ShouldMarkText - Failed to mark text");
			Assert.AreEqual (textView.MarkedRange, new NSRange (5, 7));

			textView.UnmarkText ();
		}

		[Test]
		public void NSTextInputClient_ShouldGetValidAttributesForMarkedText ()
		{
			Assert.IsTrue (textView.ValidAttributesForMarkedText.Length > 0, "NSTextInputClient_ShouldGetValidAttributesForMarkedTExt - No valid attributes");
		}

		[Test]
		public void NSTextInputClient_ShouldUnmarkText ()
		{
			textView.SetMarkedText ((NSString)"Testing", new NSRange (0, 10), new NSRange (5, 4));

			Assert.IsTrue (textView.HasMarkedText, "NSTextInputClient_ShouldUnMarkText - Failed to mark text");

			textView.UnmarkText ();

			Assert.IsFalse (textView.HasMarkedText, "NSTextInputClient_ShouldUnmarkText - Failed to Unmark text");
			Assert.IsTrue (textView.MarkedRange.Length == 0, "NSTextInputClient_ShouldUnmarkText - MarkedRange is not 0");
		}

		[Test]
		public void NSTextInputClient_ShouldGetAttributedSubstring ()
		{
			NSRange range;
			var attributedString = textView.GetAttributedSubstring (new NSRange (10, 15), out range);

			Assert.AreEqual (attributedString.Value, "new string", "NSTextInputClient_ShouldGetAttributedSubstring - Failed to get the correct string");
			Assert.AreEqual (range, new NSRange (10, 10), "NSTextInputClient_ShouldGetAttributedSubstring - Wrong range value returned");
		}

		[Test]
		public void NSTextInputClient_ShouldGetFirstRect ()
		{
			NSRange range;
			var rect = textView.GetFirstRect (new NSRange (12, 18), out range);

			Assert.AreEqual (rect, new CGRect (0, 0, 12, 14), "NSTextInputClient_ShouldGetFirstRect - Returned wrong rect");
			Assert.AreEqual (range, new NSRange (10, 4), "NSTextInputClient_ShouldGetFirstRect - Returned wrong Range");
		}

		[Test]
		public void NSTextInputClient_ShouldGetAttributedString ()
		{
			Assert.AreEqual (textView.AttributedString.Value, "This is a new string", "NSTextInputClient_ShouldGetAttributedString - Returned the wrong attributed string");
		}

		[Test]
		public void NSTextInputClient_ShouldGetFractionofDistanceThroughGlyph ()
		{
			Assert.IsTrue (textView.GetFractionOfDistanceThroughGlyph (new CGPoint (1, 2)) == 0, "NSTextInputClient_ShouldGetFractionofDistanceThroughGlyph - Returned wrong fraaction value");
		}

		[Test]
		public void NSTextInputClient_ShouldGetBaselineDelta ()
		{
			Assert.IsTrue (textView.GetBaselineDelta (4) == 11, "NSTextInputClient_ShouldGetBaselineDelta - Returned wrong baseline delta value");
		}

		[Test]
		public void NSTextInputClient_ShouldGetDrawsVertically ()
		{
			Assert.IsFalse (textView.DrawsVertically (4), "NSTextInputClient_ShouldGetDrawsVertically - Returned wrong value");
		}

		[Test]
		public void NSTextInputClient_ShouldGetWindowLevel ()
		{
			Assert.AreEqual (textView.WindowLevel, NSWindowLevel.Normal, "NSTextInputClient_ShouldGetWindowLevel - WindowLevel returned the wrong value");
		}
	}
}

