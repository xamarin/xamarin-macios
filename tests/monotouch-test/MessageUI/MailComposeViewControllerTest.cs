//
// Unit tests for MFMailComposeViewController
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if HAS_MESSAGEUI

using System;
using System.Drawing;
using Foundation;
using UIKit;

using MessageUI;
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

#if XAMCORE_3_0
			var cancelAttributes = new UIStringAttributes ();
#else
			var cancelAttributes = new UITextAttributes ();
			cancelAttributes.TextShadowOffset = new UIOffset (0, -1);
#endif
			UIBarButtonItem.AppearanceWhenContainedIn (typeof (UISearchBar)).SetTitleTextAttributes (cancelAttributes, UIControlState.Disabled);
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

#endif // HAS_MESSAGEUI
