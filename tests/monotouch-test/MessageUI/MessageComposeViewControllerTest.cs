//
// Unit tests for MFMessageComposeViewController
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
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

#endif // !__TVOS__ && !__WATCHOS__

