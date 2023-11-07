using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace MyWebViewApp {
	public partial class HybridViewController : UIViewController {
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public HybridViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Intercept URL loading to handle native calls from browser
			webView.ShouldStartLoad += HandleShouldStartLoad;

			// Render the view from the type generated from RazorView.cshtml
			var model = new Model1 () { Text = "Text goes here" };
			var template = new RazorView () { Model = model };
			var page = template.GenerateString ();

			// Load the rendered HTML into the view with a base URL 
			// that points to the root of the bundled Resources folder
			webView.LoadHtmlString (page, NSBundle.MainBundle.BundleUrl);

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

		bool HandleShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{

			// If the URL is not our own custom scheme, just let the webView load the URL as usual
			var scheme = "hybrid:";

			if (request.Url.Scheme != scheme.Replace (":", ""))
				return true;

			// This handler will treat everything between the protocol and "?"
			// as the method name.  The querystring has all of the parameters.
			var resources = request.Url.ResourceSpecifier.Split ('?');
			var method = resources [0];
			var parameters = System.Web.HttpUtility.ParseQueryString (resources [1]);

			if (method == "UpdateLabel") {
				var textbox = parameters ["textbox"];

				// Add some text to our string here so that we know something
				// happened on the native part of the round trip.
				var prepended = string.Format ("C# says \"{0}\"", textbox);

				// Build some javascript using the C#-modified result
				var js = string.Format ("SetLabelText('{0}');", prepended);

				webView.EvaluateJavascript (js);
			}

			return false;
		}
	}
}
