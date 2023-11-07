//
// Unit tests for PKPassLibrary
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !MONOMAC

using System;
using System.IO;
using Foundation;
using UIKit;
using PassKit;
using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PassLibraryTest {

#if !__WATCHOS__ // hangs on watchOS 3 beta 2 simulator
		[Test]
		public void Defaults ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			// this is yet another case where Apples plays ping-ping with different versions
			//
			// newsflash: iOS 8.1 says it's available but won't let you create a PKPassLibrary instance #24747
			// but iOS 8.2 (first beta) on an iPad let's you do it
			// and final iOS 8.2 does not seems to return true anymore #28711 while 8.3 beta works !?!
			if (!PKPassLibrary.IsAvailable)
				Assert.Inconclusive ("PassKit is not available");

			var library = new PKPassLibrary ();
			// not null (but empty by default) and there's no API to add them
			var passes = library.GetPasses ();
			if (passes is null)
				TestRuntime.IgnoreInCI ("GetPasses () seems to randomly return null on our bots.");
			// If the following assert fails for you locally, please investigate! See https://github.com/xamarin/maccore/issues/2598.
			Assert.NotNull (passes, "GetPasses - if this assert fails for you locally, please investigate! See https://github.com/xamarin/maccore/issues/2598.");

			using (var url = PassTest.GetBoardingPassUrl ()) {
#if __MACCATALYST__
				// we can just open the url
				Assert.True (UIApplication.SharedApplication.OpenUrl (url), "OpenUrl");
#elif !__WATCHOS__
				// and we can't trick the OS to do it for us
				Assert.False (UIApplication.SharedApplication.OpenUrl (url), "OpenUrl");
#endif
			}

			Assert.Null (library.GetPass (String.Empty, String.Empty), "GetPass");

			using (var pass = PassTest.GetBoardingPass ()) {
				Assert.False (library.Contains (pass), "Contains");
				Assert.False (library.Replace (pass), "Replace");
				library.Remove (pass);
			}
		}
#endif

		[Test]
		public void Fields ()
		{
			TestRuntime.AssertXcodeVersion (4, 5);

			Assert.That (PKPassLibraryUserInfoKey.AddedPasses.ToString (), Is.EqualTo ("PKPassLibraryAddedPassesUserInfo"), "AddedPassesUserInfoKey");
			Assert.That (PKPassLibraryUserInfoKey.RemovedPassInfos.ToString (), Is.EqualTo ("PKPassLibraryRemovedPassInfosUserInfo"), "RemovedPassInfosUserInfoKey");
			Assert.That (PKPassLibraryUserInfoKey.ReplacementPasses.ToString (), Is.EqualTo ("PKPassLibraryReplacementPassesUserInfo"), "ReplacementPassesUserInfoKey");
			Assert.That (PKPassLibrary.DidChangeNotification.ToString (), Is.EqualTo ("PKPassLibraryDidChangeNotification"), "DidChangeNotification");
			Assert.That (PKPassLibraryUserInfoKey.PassTypeIdentifier.ToString (), Is.EqualTo ("PKPassLibraryPassTypeIdentifierUserInfo"), "PassTypeIdentifierUserInfoKey");
			Assert.That (PKPassLibraryUserInfoKey.SerialNumber.ToString (), Is.EqualTo ("PKPassLibrarySerialNumberUserInfo"), "SerialNumberUserInfoKey");
		}
	}
}

#endif // !__TVOS__
