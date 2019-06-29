//
// Unit tests for NSGridView
//


#if XAMCORE_2_0 && __IOS__

using AppKit;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.AppKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSGridViewTest {

		[Test]
		public void CreateWithNSViewArrayOfNSViewArray ()
		{
			NSView [][] nSViewsArrayOfArray = new NSView [4][];
			nSViewsArrayOfArray [0] = new NSView [1] { new NSView() };
			nSViewsArrayOfArray [1] = new NSView [1] { new NSView() };
			nSViewsArrayOfArray [2] = new NSView [1] { new NSView() };
			nSViewsArrayOfArray [3] = new NSView [1] { new NSView() };
            
			NSGridView nSGridViewArrayOfArray = NSGridView.Create (nSViewsArrayOfArray);
			
			Assert.NotNull(nSGridViewArrayOfArray);
		}

		[Test]
		public void CreateWithTwoDimensionalNSViewArray()
		{
			NSView [,] nSViewsTwoDim = new NSView [2, 2];
			nSViewsTwoDim [0, 0] = new NSView ();
			nSViewsTwoDim [0, 1] = new NSView ();
			nSViewsTwoDim [1, 0] = new NSView ();
			nSViewsTwoDim [1, 1] = new NSView ();

			NSGridView nSGridViewTwoDimensionalArray = NSGridView.Create (nSViewsTwoDim);
			
			Assert.NotNull(nSGridViewTwoDimensionalArray);
		}
	}
}

#endif // XAMCORE_2_0 && __IOS__
