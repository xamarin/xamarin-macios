//
// Unit tests for MFMessageComposeViewController
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if HAS_MESSAGE

using System;
using System.Drawing;
using Foundation;
using UIKit;

using MessageUI;
using NUnit.Framework;

namespace MonoTouchFixtures.MessageUI {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MessageComposeViewControllerTest {

		[Test]
		public void MessageComposeDelegate ()
		{
			if (!MFMessageComposeViewController.CanSendText)
				Assert.Inconclusive ("Not configured to send text");

			using (var mail = new MFMessageComposeViewController ()) {
				Assert.Null (mail.MessageComposeDelegate, "MessageComposeDelegate");
				mail.Finished += (sender, e) => { };
				Assert.NotNull (mail.MessageComposeDelegate, "MessageComposeDelegate");
			}
		}
	}
}

#endif // HAS_MESSAGE
