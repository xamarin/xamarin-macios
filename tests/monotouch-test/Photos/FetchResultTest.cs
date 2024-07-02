//
// Unit tests for FetchResult
//
// Authors:
//	Alex Soto  <dalexsoto@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if HAS_PHOTOS && !__TVOS__ && HAS_UIKIT

using System;
using System.Linq;
using Foundation;
using UIKit;
using ObjCRuntime;
using Photos;
using CoreGraphics;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.Photos {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FetchResultTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			if (PHPhotoLibrary.GetAuthorizationStatus (PHAccessLevel.ReadWrite) != PHAuthorizationStatus.Authorized)
				Assert.Inconclusive ("Requires access to the photo library");
		}

		[Test]
		public void FetchResultToArray ()
		{
			var collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			if (collection.Count == 0) {
				XamagramImage.Image.SaveToPhotosAlbum (null);
				collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			}

			// Actual Test
			var array = collection.ToArray ();
			Assert.That (array is not null && array.Count () > 0);
		}

		[Test]
		public void FetchResultIndex ()
		{
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
			var collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			if (collection.Count == 0) {
				XamagramImage.Image.SaveToPhotosAlbum (null);
				collection = PHAsset.FetchAssets (PHAssetMediaType.Image, null);
			}

			// Actual Test
			var obj = collection.ObjectsAt<NSObject> (NSIndexSet.FromNSRange (new NSRange (0, 1)));
			Assert.That (obj is not null && obj.Count () > 0);
		}
	}

	// This class is only used in case the Photo Library is Empty,
	// this will create a blue Xamagram logo
	static class XamagramImage {
		static UIImage imageOfXamagram;

		static void DrawXamagram ()
		{
			var context = UIGraphics.GetCurrentContext ();
			var color = UIColor.FromRGBA (0.204f, 0.600f, 0.863f, 1.000f);

			context.SaveState ();
			context.TranslateCTM (257.0f, 257.0f);
			context.RotateCTM (90.0f * (nfloat) Math.PI / 180.0f);

			var polygonPath = new UIBezierPath ();
			polygonPath.MoveTo (new CGPoint (0.0f, -250.0f));
			polygonPath.AddLineTo (new CGPoint (216.51f, -125.0f));
			polygonPath.AddLineTo (new CGPoint (216.51f, 125.0f));
			polygonPath.AddLineTo (new CGPoint (0.0f, 250.0f));
			polygonPath.AddLineTo (new CGPoint (-216.51f, 125.0f));
			polygonPath.AddLineTo (new CGPoint (-216.51f, -125.0f));
			polygonPath.ClosePath ();
			color.SetFill ();
			polygonPath.Fill ();

			context.RestoreState ();

			var textRect = new CGRect (0.0f, 0.0f, 512.0f, 512.0f);
			var textContent = "X";
			UIColor.White.SetFill ();
			var textFont = UIFont.FromName ("Helvetica", 350.0f);
			textRect.Offset (0.0f, (textRect.Height - new NSString (textContent).StringSize (textFont, textRect.Size).Height) / 2.0f);
			new NSString (textContent).DrawString (textRect, textFont, UILineBreakMode.WordWrap, UITextAlignment.Center);
		}

		public static UIImage Image {
			get {
				if (imageOfXamagram is not null)
					return imageOfXamagram;

				UIGraphics.BeginImageContextWithOptions (new CGSize (512.0f, 512.0f), false, 0);
				DrawXamagram ();
				imageOfXamagram = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();

				return imageOfXamagram;
			}
		}
	}
}

#endif // HAS_PHOTOS && !__TVOS__
