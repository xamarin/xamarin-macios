using System;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;
using ImageIO;
using UIKit;

// TODO: rename namespace, get rid of TJ prefix
namespace UiImageViewAnimatedGIF  {
	public class TJAnimatedImage : NSObject {

		public NSData data = null;
		public NSUrl url = null;

		// add backing store field - is there a better way t do this?
		private CGSize size;
		public CGSize Size {
			get { return GetSize (); }
			set { this.size = value; }
		}

		public TJAnimatedImage () : base()
		{
			//base.Init ();
			size = CGSize.Empty;
		}

		public static TJAnimatedImage AnimatedImageWithData(NSData data)
		{
			TJAnimatedImage animatedImage = new TJAnimatedImage();
			animatedImage.data = data;
			return animatedImage;
		}

		public static TJAnimatedImage AnimatedImageWithURL(NSUrl url)
		{
			TJAnimatedImage animatedImage = new TJAnimatedImage ();
			animatedImage.url = url;
			return animatedImage;
		}

		public override bool Equals (object obj)
		{
			return obj is TJAnimatedImage image &&
			       base.Equals (obj) &&
			       EqualityComparer<NSData>.Default.Equals (data, image.data) &&
			       EqualityComparer<NSUrl>.Default.Equals (url, image.url);
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (base.GetHashCode (), data, url);
		}

		private CGSize GetSize()
		{
			if (size.Equals (CGSize.Empty))
			{
				CGImageSource imageSource = null;
				if (data != null) {
					imageSource = ImageIO.CGImageSource.FromData (data);
				} else if (url != null) {
					imageSource = ImageIO.CGImageSource.FromUrl (url);
				}
				if (imageSource != null) {
					if (imageSource.ImageCount > 0) {
						NSDictionary nullDict = null;
						NSDictionary properties = imageSource.CopyProperties (nullDict, 0);

						var w = properties ["PixelWidth"];
						var h = properties ["PixelHeight"].ToString();
						float width = float.Parse(properties["PixelWidth"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
						float height = float.Parse (properties["PixelHeight"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
						size = new CGSize (width, height);
					}
				}
			}
			return size;
		}
	}

	public class TJAnimatedImageView : CustomUIImageView {
		public TJAnimatedImageView(CGRect bounds) : base(bounds) { }

		public void SetImage(UIImage image)
		{
			this.AnimatedImage = null;
			base.Image = image;
		}

	}

	public class CustomUIImageView : UIImageView {


		static readonly NSString kUIImageViewAnimatedGIFAnimatedImageKey = new NSString ("kUIImageViewAnimatedGIFAnimatedImageKey");

		private TJAnimatedImage animatedImage;
		public TJAnimatedImage AnimatedImage {
			get { return animatedImage; /*(TJAnimatedImage)this.ValueForKey (kUIImageViewAnimatedGIFAnimatedImageKey)*/; }

			set {
				SetAnimatedImage (value);
			}
		}

		public CustomUIImageView (CGRect bounds) : base(bounds)
		{
		}

		protected void TJSetImageAnimated(UIImage image)
		{
			this.Image = image;
		}

		void MyAnimationBlock (System.nint arg0, CGImage arg1, out bool done)
		{
			// TODO: ObjC uses strongSelf + weakSelf to check whether the user has dismissed the view
			if (this.animatedImage.IsEqual (animatedImage)) {
				UIImage loadedImage = UIImage.FromImage (arg1);
				TJSetImageAnimated (loadedImage);
				animatedImage.Size = loadedImage.Size;
				done = false;
			} else {
				done = true;
			}
		}

		void SetAnimatedImage (TJAnimatedImage anImage)
		{
			if (this.AnimatedImage != null && (this.AnimatedImage == anImage || this.AnimatedImage.IsEqual(anImage))) {
				return;
			}

			// objc_setAssociatedObject(self, kUIImageViewAnimatedGIFAnimatedImageKey, animatedImage, OBJC_ASSOCIATION_RETAIN);
			//this.SetValueForKey (anImage, kUIImageViewAnimatedGIFAnimatedImageKey);
			this.animatedImage = anImage;

			this.TJSetImageAnimated (null);
			CGImageAnimation animation = new CGImageAnimation ();

			if (anImage.data != null) {
				// TODO: This sample does not test CGAnimageImageData, add test case later?
				animation.CGAnimateImageData (anImage.data, null, MyAnimationBlock);
			}
			else if (anImage.url != null){
				animation.CGAnimateImageAtUrl (anImage.url, null, MyAnimationBlock);
			}
		}
	}
}