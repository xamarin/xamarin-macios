//
// Unit tests for AUViewController
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !__TVOS__

using Foundation;
using CoreAudioKit;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreAudioKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AUViewControllerTest {
		[SetUp]
		public void MinimumSdkCheck ()
		{
			// AUViewController was added in iOS 9
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void Ctor ()
		{
			// this tests that mtouch properly links with CoreAudioKit
			// when not using the simlauncher (since monotouch-test 
			// uses native libraries, which prevents mtouch from building
			// monotouch-test using simlauncher).
			using (var obj = new AUViewController ()) {
			}
		}
	}
}

#endif // !__WATCHOS__ && !__TVOS__
