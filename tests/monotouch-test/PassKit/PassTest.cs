//
// Unit tests for PKPass
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using PassKit;
#else
using MonoTouch.Foundation;
using MonoTouch.PassKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PassTest {

		[Test]
		public void Defaults ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			using (PKPass pass = new PKPass ()) {
				Assert.Null (pass.AuthenticationToken, "AuthenticationToken");
#if !__WATCHOS__
				Assert.NotNull (pass.Icon, "Icon");
#endif
				Assert.Null (pass.LocalizedDescription, "LocalizedDescription");
				Assert.That (string.IsNullOrEmpty (pass.LocalizedName), Is.False, "LocalizedName");
				Assert.Null (pass.OrganizationName, "OrganizationName");
				Assert.Null (pass.PassTypeIdentifier, "PassTypeIdentifier");
				Assert.Null (pass.PassUrl, "PassUrl");
				Assert.Null (pass.RelevantDate, "RelevantDate");
				Assert.Null (pass.SerialNumber, "SerialNumber");
				Assert.Null (pass.WebServiceUrl, "WebServiceUrl");
			}
		}

		static public PKPass GetBoardingPass ()
		{
			using (NSUrl url = new NSUrl (NSBundle.MainBundle.BundleUrl + "/BoardingPass.pkpass"))
			using (NSData data = NSData.FromUrl (url)) {
				NSError error;
				PKPass pass = new PKPass (data, out error);
				Assert.Null (error, "error");
				return pass;
			}
		}

		[Test]
		public void BoardingPass ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			using (var pass = GetBoardingPass ()) {
				Assert.That (pass.AuthenticationToken, Is.EqualTo ("vxwxd7J8AlNNFPS8k0a0FfUFtq0ewzFdc"), "AuthenticationToken");
#if !__WATCHOS__
				Assert.NotNull (pass.Icon, "Icon");
#endif

				// need to ensure it's english locale
				Assert.That (string.IsNullOrEmpty (pass.LocalizedDescription), Is.False, "LocalizedName");
				Assert.That (string.IsNullOrEmpty (pass.LocalizedName), Is.False, "LocalizedName");

				Assert.That (pass.OrganizationName, Is.EqualTo ("Skyport Airways"), "OrganizationName");
				Assert.That (pass.PassTypeIdentifier, Is.EqualTo ("pass.com.apple.devpubs.example"), "PassTypeIdentifier");
				if (TestRuntime.CheckXcodeVersion (5, 0))
					Assert.That (pass.PassUrl.AbsoluteString, Is.EqualTo ("shoebox://card/1UuiGnfwxHgd0G0bIuPYPNpeRX8="), "PassUrl");
				else
					Assert.Null (pass.PassUrl, "PassUrl");
				Assert.That (pass.RelevantDate.SecondsSinceReferenceDate, Is.EqualTo (364688700), "RelevantDate");
				Assert.That (pass.SerialNumber, Is.EqualTo ("gT6zrHkaW"), "SerialNumber");
				Assert.That (pass.WebServiceUrl.AbsoluteString, Is.EqualTo ("https://example.com/passes/"), "WebServiceUrl");
			}
		}

		[Test]
		public void Fields ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			Assert.That (PKPass.ErrorDomain.ToString (), Is.EqualTo ("PKPassKitErrorDomain"), "ErrorDomain");
		}
	}
}

#endif // !__TVOS__

