// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TableViewCellTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UITableViewCell tvc = new UITableViewCell (frame)) {
				Assert.That (tvc.Frame.X, Is.EqualTo ((nfloat) 0.0f), "X");
				Assert.That (tvc.Frame.Y, Is.EqualTo ((nfloat) 0.0f), "Y");
				// whatever supplied value X and Y are set to 0.0 by the cell
				// Width and Height are set by the cell (e.g. 320 x 44 for the iPhone)
			}
		}
	}
}

#endif // !__WATCHOS__
