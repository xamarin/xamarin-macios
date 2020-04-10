using Foundation;
using System;
using UIKit;

namespace UiImageViewAnimatedGIF {

	public partial class ViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate {

		//@property (nonatomic, strong) NSMutableDictionary<NSString*, TJAnimatedImage*>* animatedImagesForFilenames;
		NSMutableDictionary<NSString, TJAnimatedImage> animatedImagesForFilenames;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			UITableView tableView = new UITableView(this.View.Bounds);
			tableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			tableView.DataSource = this;
			tableView.Delegate = this;
			tableView.ContentInset = UIEdgeInsets.Zero;
			float Epsilon = 1.192092896e-07F;
			tableView.SeparatorInset = new UIEdgeInsets (0, Epsilon, 0, 0);
			tableView.LayoutMargins = UIEdgeInsets.Zero;
			tableView.PreservesSuperviewLayoutMargins = false;
			this.View.AddSubview (tableView);

			this.animatedImagesForFilenames = new NSMutableDictionary<NSString, TJAnimatedImage> ();
		}

		static string [] names = { "earth", "elmo", "hack", "meme" };
		NSArray imageFileNames = NSArray.FromStrings(names);

		public TJAnimatedImage animatedImageForFilename(NSString filename)
		{
			TJAnimatedImage image = animatedImagesForFilenames [filename];
			if (image == null) {
				//string exePath = System.IO.Path.GetDirectoryName (System.Reflection.Assembly.GetEntryAssembly ().Location);

				image = TJAnimatedImage.AnimatedImageWithURL (NSBundle.MainBundle.GetUrlForResource(filename, "gif"));
				CoreGraphics.CGSize size = image.Size;
				this.animatedImagesForFilenames [filename] = image;
			}
			return image;
		}

		public nint NumberOfSections (UITableView tableView) // numberOfSectionsInTableView
		{
			return 1;
		}

		public nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			NSString filename = new NSString(names[indexPath.Row]);
			TJAnimatedImage image = animatedImageForFilename (filename);
			nfloat aspectRatio = image.Size.Height / image.Size.Width;
			nfloat height = aspectRatio * this.View.Bounds.Size.Width;
			return height;
		}

		public nint RowsInSection (UITableView tableView, nint section)
		{
			return names.Length;
		}

		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			const string kIdentifier = "cell";
			const int kImageViewTag = 555;
			UITableViewCell cell = tableView.DequeueReusableCell (kIdentifier);
			CustomUIImageView imageView;
			if (cell != null) {
				imageView = (CustomUIImageView)cell.ContentView.ViewWithTag (kImageViewTag);

			} else {
				cell = new UITableViewCell (UITableViewCellStyle.Default, kIdentifier);
				imageView = new CustomUIImageView (cell.ContentView.Bounds) {
					AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
					Tag = kImageViewTag
				};
				cell.ContentView.AddSubview (imageView);
			}

			// figure out how to create an NSArray w/ NSString type values
			imageView.AnimatedImage = this.animatedImageForFilename (new NSString (names [indexPath.Row]));
			return cell;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}