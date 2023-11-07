//
// Unit tests for NSGridView
//


#if __MACOS__

using System;
using AppKit;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.AppKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSGridViewTest {

		[Test]
		public void CreateWithNSViewArrayOfArrayCheckNSTextView ()
		{
			NSView [] [] nSViewsArrayOfArray = new NSView [4] [];
			nSViewsArrayOfArray [0] = new NSView [1] { new NSTextView () { Value = "0" } };
			nSViewsArrayOfArray [1] = new NSView [1] { new NSTextView () { Value = "1" } };
			nSViewsArrayOfArray [2] = new NSView [1] { new NSTextView () { Value = "2" } };
			nSViewsArrayOfArray [3] = new NSView [1] { new NSTextView () { Value = "3" } };

			NSGridView nSGridViewArrayOfArray = NSGridView.Create (nSViewsArrayOfArray);

			Assert.NotNull (nSGridViewArrayOfArray);
			Assert.AreEqual ("0", ((NSTextView) (nSGridViewArrayOfArray.GetCell (0, 0)).ContentView).Value, "0,0");
			Assert.AreEqual ("1", ((NSTextView) (nSGridViewArrayOfArray.GetCell (0, 1)).ContentView).Value, "0,1");
			Assert.AreEqual ("2", ((NSTextView) (nSGridViewArrayOfArray.GetCell (0, 2)).ContentView).Value, "0,2");
			Assert.AreEqual ("3", ((NSTextView) (nSGridViewArrayOfArray.GetCell (0, 3)).ContentView).Value, "0,3");
		}

		[Test]
		public void CreateWithNSViewArrayOfArrayCheckDifferentArrayLength ()
		{
			NSView [] [] nSViewsArrayOfArray = new NSView [2] [];
			nSViewsArrayOfArray [0] = new NSView [1] { new NSTextView () { Value = "0" } };
			nSViewsArrayOfArray [1] = new NSView [2] { new NSTextView () { Value = "1" }, new NSTextView () { Value = "1bis" } };

			NSGridView nSGridViewArrayOfArray = NSGridView.Create (nSViewsArrayOfArray);

			Assert.NotNull (nSGridViewArrayOfArray);
			Assert.AreEqual ("0", ((NSTextView) (nSGridViewArrayOfArray.GetCell (0, 0)).ContentView).Value, "0,0");
			Assert.AreEqual ("1", ((NSTextView) (nSGridViewArrayOfArray.GetCell (0, 1)).ContentView).Value, "0,1");
			Assert.AreEqual ("1bis", ((NSTextView) (nSGridViewArrayOfArray.GetCell (1, 1)).ContentView).Value, "0,2");
		}

		[Test]
		public void CreateWithTwoDimensionalNSViewArrayNSTextView ()
		{
			NSView [,] nSViewsTwoDim = new NSView [2, 2];
			nSViewsTwoDim [0, 0] = new NSTextView () { Value = "0" };
			nSViewsTwoDim [0, 1] = new NSTextView () { Value = "1" };
			nSViewsTwoDim [1, 0] = new NSTextView () { Value = "2" };
			nSViewsTwoDim [1, 1] = new NSTextView () { Value = "3" };

			NSGridView nSGridViewTwoDimensionalArray = NSGridView.Create (nSViewsTwoDim);

			Assert.NotNull (nSGridViewTwoDimensionalArray);
			Assert.AreEqual ("0", ((NSTextView) (nSGridViewTwoDimensionalArray.GetCell (0, 0)).ContentView).Value, "0,0");
			Assert.AreEqual ("1", ((NSTextView) (nSGridViewTwoDimensionalArray.GetCell (0, 1)).ContentView).Value, "0,1");
			Assert.AreEqual ("2", ((NSTextView) (nSGridViewTwoDimensionalArray.GetCell (1, 0)).ContentView).Value, "1,0");
			Assert.AreEqual ("3", ((NSTextView) (nSGridViewTwoDimensionalArray.GetCell (1, 1)).ContentView).Value, "1,1");
		}

		[Test]
		public void CreateWithTwoDimensionalNSViewArrayCheckDifferentDimensionSize ()
		{
			NSView [,] nSViewsTwoDim = new NSView [1, 4];
			nSViewsTwoDim [0, 0] = new NSTextView () { Value = "0" };
			nSViewsTwoDim [0, 1] = new NSTextView () { Value = "1" };
			nSViewsTwoDim [0, 2] = new NSTextView () { Value = "2" };
			nSViewsTwoDim [0, 3] = new NSTextView () { Value = "3" };

			NSGridView nSGridViewTwoDimensionalArray = NSGridView.Create (nSViewsTwoDim);

			Assert.NotNull (nSGridViewTwoDimensionalArray);
			Assert.AreEqual ("0", ((NSTextView) (nSGridViewTwoDimensionalArray.GetCell (0, 0)).ContentView).Value, "0,0");
			Assert.AreEqual ("1", ((NSTextView) (nSGridViewTwoDimensionalArray.GetCell (0, 1)).ContentView).Value, "0,1");
			Assert.AreEqual ("2", ((NSTextView) (nSGridViewTwoDimensionalArray.GetCell (0, 2)).ContentView).Value, "0,2");
			Assert.AreEqual ("3", ((NSTextView) (nSGridViewTwoDimensionalArray.GetCell (0, 3)).ContentView).Value, "0,3");
		}

		[Test]
		public void CreateWithNSViewArrayOfArrayCheckWithNullNSView ()
		{
			NSView [] [] nSViewsArrayOfArray = new NSView [2] [];
			nSViewsArrayOfArray [0] = new NSView [1] { new NSTextView () { Value = "0" } };
			nSViewsArrayOfArray [1] = new NSView [1] { null };

			Assert.Throws<ArgumentNullException> (() => NSGridView.Create (nSViewsArrayOfArray), "Broken Array #1");
		}

		[Test]
		public void CreateWithNSViewArrayOfArrayCheckWithNullArray ()
		{
			NSView [] [] nSViewsArrayOfArray = new NSView [2] [];
			nSViewsArrayOfArray [0] = new NSView [1] { new NSTextView () { Value = "0" } };
			nSViewsArrayOfArray [1] = null;

			Assert.Throws<ArgumentNullException> (() => NSGridView.Create (nSViewsArrayOfArray), "Broken Array #2");
		}

		[Test]
		public void CreateWithNSViewArrayOfArrayCheckWithNull ()
		{
			NSView [] [] nSViewsArrayOfArray = null;

			Assert.Throws<ArgumentNullException> (() => NSGridView.Create (nSViewsArrayOfArray), "Broken Array #3");
		}

		[Test]
		public void CreateWithTwoDimensionalNSViewArrayCheckWithNull ()
		{
			NSView [,] nSViewsTwoDim = null;

			Assert.Throws<ArgumentNullException> (() => NSGridView.Create (nSViewsTwoDim), "Broken Array #4");
		}

		[Test]
		public void CreateWithTwoDimensionalNSViewArrayWithNullCell ()
		{
			NSView [,] nSViewsTwoDim = new NSView [2, 2];
			nSViewsTwoDim [0, 0] = new NSTextView () { Value = "0" };
			nSViewsTwoDim [0, 1] = new NSTextView () { Value = "1" };
			nSViewsTwoDim [1, 0] = new NSTextView () { Value = "2" };
			nSViewsTwoDim [1, 1] = null;

			Assert.Throws<ArgumentNullException> (() => NSGridView.Create (nSViewsTwoDim), "Broken Array #5");
		}
	}
}

#endif // __MACOS__

