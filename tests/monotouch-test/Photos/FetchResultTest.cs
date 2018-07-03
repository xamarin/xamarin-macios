//
// Unit tests for FetchResult
//
// Authors:
//	Alex Soto  <dalexsoto@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Linq;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
using Photos;
using CoreGraphics;
using AssetsLibrary;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouch.Photos;
using System.Drawing;
using MonoTouch.AssetsLibrary;
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Photos {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FetchResultTest {

		[Test]
		public void FetchResultToArray ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);

			if (ALAssetsLibrary.AuthorizationStatus != ALAuthorizationStatus.Authorized)
				Assert.Inconclusive ("Requires access to the photo library");

			var collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			if (collection.Count == 0) {
				XamagramImage.Image.SaveToPhotosAlbum (null);
				collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			}

			// Actual Test
			var array = collection.ToArray ();
			Assert.That (array != null && array.Count() > 0);
		}

		[Test]
		public void FetchResultIndex ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);
				
			if (ALAssetsLibrary.AuthorizationStatus != ALAuthorizationStatus.Authorized)
				Assert.Inconclusive ("Requires access to the photo library");

			var collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			if (collection.Count == 0) {
				XamagramImage.Image.SaveToPhotosAlbum (null);
				collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			}

			// Actual Test
			var obj = collection [0];
			Assert.IsNotNull (obj);
		}

		[Test]
		public void FetchResultObjectsAt ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);

			if (ALAssetsLibrary.AuthorizationStatus != ALAuthorizationStatus.Authorized)
				Assert.Inconclusive ("Requires access to the photo library");

			var collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			if (collection.Count == 0) {
				XamagramImage.Image.SaveToPhotosAlbum (null);
				collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			}

			// Actual Test
			var obj = collection.ObjectsAt<NSObject> (NSIndexSet.FromNSRange (new NSRange (0, 1)));
			Assert.That (obj != null && obj.Count() > 0);
		}
	}

	// This class is only used in case the Photo Library is Empty,
	// this will create a blue Xamagram logo
	static class XamagramImage
	{
		static UIImage imageOfXamagram;

		static void DrawXamagram()
		{
			var context = UIGraphics.GetCurrentContext ();
			var color = UIColor.FromRGBA (0.204f, 0.600f, 0.863f, 1.000f);

			context.SaveState ();
			context.TranslateCTM (257.0f, 257.0f);
			context.RotateCTM (90.0f * (nfloat)Math.PI / 180.0f);

			var polygonPath = new UIBezierPath();
			polygonPath.MoveTo (new PointF (0.0f, -250.0f));
			polygonPath.AddLineTo (new PointF (216.51f, -125.0f));
			polygonPath.AddLineTo (new PointF (216.51f, 125.0f));
			polygonPath.AddLineTo (new PointF (0.0f, 250.0f));
			polygonPath.AddLineTo (new PointF (-216.51f, 125.0f));
			polygonPath.AddLineTo (new PointF (-216.51f, -125.0f));
			polygonPath.ClosePath ();
			color.SetFill ();
			polygonPath.Fill ();

			context.RestoreState();

			var textRect = new RectangleF (0.0f, 0.0f, 512.0f, 512.0f);
			var textContent = "X";
			UIColor.White.SetFill ();
			var textFont = UIFont.FromName ("Helvetica", 350.0f);
			textRect.Offset (0.0f, (textRect.Height - new NSString (textContent).StringSize (textFont, textRect.Size).Height) / 2.0f);
			new NSString (textContent).DrawString (textRect, textFont, UILineBreakMode.WordWrap, UITextAlignment.Center);
		}

		public static UIImage Image
		{
			get
			{
				if (imageOfXamagram != null)
					return imageOfXamagram;

				UIGraphics.BeginImageContextWithOptions (new SizeF (512.0f, 512.0f), false, 0);
				DrawXamagram ();
				imageOfXamagram = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();

				return imageOfXamagram;
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
