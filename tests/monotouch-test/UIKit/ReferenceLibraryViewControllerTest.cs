// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ReferenceLibraryViewControllerTest {

		[Test]
#if __MACCATALYST__
		[Ignore ("https://github.com/xamarin/maccore/issues/2348")] // The native class doesn't exist
#endif

		public void InitWithTerm ()
		{
#if !__MACCATALYST__
			if (Runtime.Arch == Arch.DEVICE && TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 9, 0))
				Assert.Ignore ("crash on iOS9 devices");
#endif
			using (UIReferenceLibraryViewController rlvc = new UIReferenceLibraryViewController ("Mono")) {
			}
		}

		[Test]
		[Ignore ("ios6 beta issues")]
		public void DictionaryHasDefinitionForTerm ()
		{
			// note: iOS 6 beta 3 fails with: +[_UIDictionaryWrapper _availableDictionaryAssets] returned failed - retrying. Error: Error Domain=ASError Code=4 "The operation couldn’t be completed. (ASError error 4 - Unable to copy asset information)" UserInfo=0x16ac81a0 {NSDescription=Unable to copy asset information}
			// beta 3 always return true, beta 4 false ...
			Assert.True (UIReferenceLibraryViewController.DictionaryHasDefinitionForTerm ("Mono"), "Mono");
			Assert.False (UIReferenceLibraryViewController.DictionaryHasDefinitionForTerm ("zozo"), "zozo");
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
