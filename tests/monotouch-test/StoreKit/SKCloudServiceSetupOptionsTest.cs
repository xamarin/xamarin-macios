//
// Unit tests for SKCloudServiceSetupOptionsTest
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft. All rights reserved.
//

#if __IOS__

using System;
using Foundation;
using NUnit.Framework;
using StoreKit;

namespace MonoTouchFixtures.StoreKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SKCloudServiceSetupOptionsTest {

		[Test]
		public void ActionTest ()
		{
			TestRuntime.AssertXcodeVersion (8, 1);

			var optionsObject = new SKCloudServiceSetupOptions {
				Action = SKCloudServiceSetupAction.Subscribe
			};
			Assert.AreEqual ("sdkSubscribe", optionsObject.Dictionary ["SKCloudServiceSetupOptionsActionKey"].ToString (), "SKCloudServiceSetupOptionsActionKey");
			Assert.AreEqual (SKCloudServiceSetupAction.Subscribe, optionsObject.Action, "SKCloudServiceSetupOptions.Action");
		}
	}
}

#endif // __IOS__
