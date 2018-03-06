//
// Unit tests for MFMailComposeViewController
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;

using MessageUI;
#else
using MonoTouch.Foundation;
using MonoTouch.MessageUI;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MessageUI {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MailComposeViewControllerTest {

		[Test]
		public void TextShadowOffset_7443 ()
		{
			if (!MFMailComposeViewController.CanSendMail)
				Assert.Inconclusive ("Not configured to send emails");

			var cancelAttributes = new UITextAttributes ();
			cancelAttributes.TextShadowOffset = new UIOffset (0, -1);
			UIBarButtonItem.AppearanceWhenContainedIn (typeof(UISearchBar)).SetTitleTextAttributes (cancelAttributes, UIControlState.Disabled);
			using (var mail = new MFMailComposeViewController ()) {
				// we're happy the .ctor did not crash (only on iOS6) because the dictionary had a null key (typo)
				Assert.That (mail.Handle, Is.Not.EqualTo (IntPtr.Zero));
			}
		}

		[Test]
		public void MailComposeDelegate ()
		{
			if (!MFMailComposeViewController.CanSendMail)
				Assert.Inconclusive ("Not configured to send emails");

			using (var mail = new MFMailComposeViewController ()) {
				Assert.Null (mail.MailComposeDelegate, "MailComposeDelegate");
				mail.Finished += (sender, e) => { };
				Assert.NotNull (mail.MailComposeDelegate, "MailComposeDelegate");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
