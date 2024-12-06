using System;
using UIKit;
using MonoTouch.Dialog;
using System.Threading;

namespace Sample {
	public partial class AppDelegate {
		public void DemoLoadMore ()
		{
			Section loadMore = new Section ();

			var s = new StyledStringElement ("Hola") {
				BackgroundUri = new Uri ("http://www.google.com/images/logos/ps_logo2.png")
				//BackgroundColor = UIColor.Red
			};
			loadMore.Add (s);
			loadMore.Add (new StringElement ("Element 1"));
			loadMore.Add (new StringElement ("Element 2"));
			loadMore.Add (new StringElement ("Element 3"));


			loadMore.Add (new LoadMoreElement ("Load More Elements...", "Loading Elements...", lme => {
				// Launch a thread to do some work
				ThreadPool.QueueUserWorkItem (delegate
				{

					// We just wait for 2 seconds.
					System.Threading.Thread.Sleep (2000);

					// Now make sure we invoke on the main thread the updates
					navigation.BeginInvokeOnMainThread (delegate
					{
						lme.Animating = false;
						loadMore.Insert (loadMore.Count - 1, new StringElement ("Element " + (loadMore.Count + 1)),
										new StringElement ("Element " + (loadMore.Count + 2)),
										new StringElement ("Element " + (loadMore.Count + 3)));

					});
				});

			}, UIFont.BoldSystemFontOfSize (14.0f), UIColor.Blue));

			var root = new RootElement ("Load More") {
				loadMore
			};

			var dvc = new DialogViewController (root, true);
			navigation.PushViewController (dvc, true);
		}
	}
}

