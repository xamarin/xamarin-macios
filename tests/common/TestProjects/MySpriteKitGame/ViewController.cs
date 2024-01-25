using System;

using SpriteKit;

using UIKit;

namespace MySpriteKitGame {
	public partial class ViewController : UIViewController {
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Configure the view.
			var skView = (SKView) View;
			skView.ShowsFPS = true;
			skView.ShowsNodeCount = true;

			// Create and configure the scene.
			var scene = new MyScene (skView.Bounds.Size);
			scene.ScaleMode = SKSceneScaleMode.AspectFill;

			// Present the scene.
			skView.PresentScene (scene);
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				return UIInterfaceOrientationMask.AllButUpsideDown;

			return UIInterfaceOrientationMask.All;
		}
	}
}
