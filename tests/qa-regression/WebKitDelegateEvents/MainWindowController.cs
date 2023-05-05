using System;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.WebKit;

namespace WebKitDelegateEvents {
	public partial class MainWindowController : NSWindowController {
		string currentLocation;

		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}

		void Navigate ()
		{
			var location = locationTextField.StringValue;
			if (currentLocation != location) {
				currentLocation = location;
				webView.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (location)));
				Window.MakeFirstResponder (webView);
			}
		}

		class DomOutlineViewDelegate : NSOutlineViewDelegate {
			public override void SelectionDidChange (NSNotification notification)
			{
				Console.WriteLine ("SELECTION CHANGE VIA DELEGATE");
			}
		}

		class DomOutlineViewDataSource : NSOutlineViewDataSource {
			WebView webView;

			DomNodeList GetChildren (NSObject item)
			{
				var node = item as DomNode;
				if (node is not null && node.HasChildNodes ())
					return node.ChildNodes;

				return null;
			}

			public DomOutlineViewDataSource (WebView webView)
			{
				this.webView = webView;
			}

			public override int GetChildrenCount (NSOutlineView outlineView, NSObject item)
			{
				if (item is null)
					return 1;

				var children = GetChildren (item);
				if (children is not null)
					return children.Count;

				return 0;
			}

			public override NSObject GetChild (NSOutlineView outlineView, int childIndex, NSObject item)
			{
				if (item is null)
					return webView.MainFrameDocument;

				var children = GetChildren (item);
				if (children is not null)
					return children.GetItem (childIndex);

				return null;
			}

			public override NSObject GetObjectValue (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				return item;
			}

			public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
			{
				return GetChildren (item) is not null;
			}
		}

		void PopulateOutlineView ()
		{
			outlineView.DataSource = new DomOutlineViewDataSource (webView);
			outlineView.ReloadData ();
			outlineView.ExpandItem (null, true);
		}

		public override void AwakeFromNib ()
		{
			navigationButtons.SetEnabled (false, 0);
			navigationButtons.SetEnabled (false, 1);
			navigationButtons.Activated += (sender, e) => {
				switch (navigationButtons.SelectedSegment) {
				case 0:
					if (webView.CanGoBack ())
						webView.GoBack ();
					break;
				case 1:
					if (webView.CanGoForward ())
						webView.GoForward ();
					break;
				}
			};

			populateOutlineViewButton.Activated += (sender, e) => PopulateOutlineView ();

			locationTextField.Activated += (sender, e) => Navigate ();

			webView.ReceivedTitle += (sender, e) => {
				Console.WriteLine ("TITLE RECEIVED: {0}", e.Title);
				if (e.ForFrame == webView.MainFrame)
					Window.Title = e.Title;
			};

			webView.FinishedLoad += (sender, e) => {
				Console.WriteLine ("FINISHED LOAD");
				navigationButtons.SetEnabled (webView.CanGoBack (), 0);
				navigationButtons.SetEnabled (webView.CanGoForward (), 1);
				PopulateOutlineView ();
			};

			//outlineView.Delegate = new DomOutlineViewDelegate ();
			outlineView.SelectionDidChange += (sender, e) => { Console.WriteLine ("SELECTION CHANGE VIA EVENT"); };

			locationTextField.StringValue = "http://catoverflow.com";
			Navigate ();
		}
	}
}
