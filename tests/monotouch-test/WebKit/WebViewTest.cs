// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

#if __MACOS__

using WebKit;

namespace MonoTouchFixtures.WebKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WebViewTest {
		[Test]
		public void PolicyDelegateSelectors ()
		{
			using var view = new WebView ();
			view.UnableToImplementPolicy += (sender, e) => { };

			var policyDelegate = view.WeakPolicyDelegate ?? throw new AssertionException ("Policy delegate was not created.");

			var navigation = new Selector ("webView:decidePolicyForNavigationAction:request:frame:decisionListener:");
			var newWindow = new Selector ("webView:decidePolicyForNewWindowAction:request:newFrameName:decisionListener:");
			var mimeType = new Selector ("webView:decidePolicyForMIMEType:request:frame:decisionListener:");
			var failure = new Selector ("webView:unableToImplementPolicyWithError:frame:");

			void AssertSelectors (bool handlesNavigation, bool handlesNewWindow, bool handlesMimeType)
			{
				Assert.That (policyDelegate.RespondsToSelector (navigation), Is.EqualTo (handlesNavigation), "Navigation");
				Assert.That (policyDelegate.RespondsToSelector (newWindow), Is.EqualTo (handlesNewWindow), "New window");
				Assert.That (policyDelegate.RespondsToSelector (mimeType), Is.EqualTo (handlesMimeType), "MIME type");
				Assert.That (policyDelegate.RespondsToSelector (failure), Is.True, "Failure notification");
			}

			EventHandler<WebNavigationPolicyEventArgs> navigationHandler = (sender, e) => { };
			EventHandler<WebNewWindowPolicyEventArgs> newWindowHandler = (sender, e) => { };
			EventHandler<WebMimeTypePolicyEventArgs> mimeTypeHandler = (sender, e) => { };

			AssertSelectors (false, false, false);
			view.DecidePolicyForNavigation += navigationHandler;
			AssertSelectors (true, false, false);
			view.DecidePolicyForNewWindow += newWindowHandler;
			AssertSelectors (true, true, false);
			view.DecidePolicyForMimeType += mimeTypeHandler;
			AssertSelectors (true, true, true);
			view.DecidePolicyForNavigation -= navigationHandler;
			AssertSelectors (false, true, true);
			view.DecidePolicyForNewWindow -= newWindowHandler;
			AssertSelectors (false, false, true);
			view.DecidePolicyForMimeType -= mimeTypeHandler;
			AssertSelectors (false, false, false);
		}
	}
}

#endif
