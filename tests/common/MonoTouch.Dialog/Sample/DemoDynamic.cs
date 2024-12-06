//
// This sample shows how to create data dynamically based on the
// responses from a remote server.
//
// It uses the BindingContext API for the login screen (we might
// as well) and the Element tree API for the actual tweet contents
//
using System;
using System.Net;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using UIKit;
using Foundation;

using MonoTouch.Dialog;

namespace Sample {
	public partial class AppDelegate {
		DialogViewController dynamic;
		BindingContext context;
		AccountInfo account;

		class AccountInfo {
			[Section ("Twitter credentials", "This sample loads various information\nsources dynamically from your twitter\naccount.")]

			[Entry ("Enter your login name")]
			public string Username;

			[Password ("Enter your password")]
			public string Password;

			[Section ("Tap to fetch the timeline")]
			[OnTap ("FetchTweets")]
			[Preserve]
			public string Login;
		}

		bool Busy {
			get {
#if __TVOS__
				return false;
#else
				return UIApplication.SharedApplication.NetworkActivityIndicatorVisible;
#endif // __TVOS__
			}
			set {
#if !__TVOS__
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
#endif // !__TVOS__
			}
		}

		public void FetchTweets ()
		{
			if (Busy)
				return;

			Busy = true;

			// Fetch the edited values.
			context.Fetch ();

			var request = (HttpWebRequest) WebRequest.Create ("http://twitter.com/statuses/friends_timeline.xml");
			request.Credentials = new NetworkCredential (account.Username, account.Password);
			request.BeginGetResponse (TimeLineLoaded, request);
		}

		// Creates the dynamic content from the twitter results
		RootElement CreateDynamicContent (XDocument doc)
		{
			var users = doc.XPathSelectElements ("./statuses/status/user").ToArray ();
			var texts = doc.XPathSelectElements ("./statuses/status/text").Select (x => x.Value).ToArray ();
			var people = doc.XPathSelectElements ("./statuses/status/user/name").Select (x => x.Value).ToArray ();

			var section = new Section ();
			var root = new RootElement ("Tweets") { section };

			for (int i = 0; i < people.Length; i++) {
				var line = new RootElement (people [i]) {
					new Section ("Profile"){
						new StringElement ("Screen name", users [i].XPathSelectElement ("./screen_name").Value),
						new StringElement ("Name", people [i]),
						new StringElement ("oFllowers:", users [i].XPathSelectElement ("./followers_count").Value)
					},
					new Section ("Tweet"){
						new StringElement (texts [i])
					}
				};
				section.Add (line);
			}

			return root;
		}

		void TimeLineLoaded (IAsyncResult result)
		{
			var request = result.AsyncState as HttpWebRequest;
			Busy = false;

			try {
				var response = request.EndGetResponse (result);
				var stream = response.GetResponseStream ();


				var root = CreateDynamicContent (XDocument.Load (new XmlTextReader (stream)));
				InvokeOnMainThread (delegate
				{
					var tweetVC = new DialogViewController (root, true);
					navigation.PushViewController (tweetVC, true);
				});
			} catch (WebException e) {

				InvokeOnMainThread (delegate
				{
					var alertController = UIAlertController.Create ("Error", "Code: " + e.Status, UIAlertControllerStyle.Alert);
					alertController.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, (obj) => { }));
					window.RootViewController.PresentViewController (alertController, true, null);
				});
			}
		}

		public void DemoDynamic ()
		{
			account = new AccountInfo ();

			context = new BindingContext (this, account, "Settings");

			if (dynamic is not null)
				dynamic.Dispose ();

			dynamic = new DialogViewController (context.Root, true);
			navigation.PushViewController (dynamic, true);
		}
	}
}
