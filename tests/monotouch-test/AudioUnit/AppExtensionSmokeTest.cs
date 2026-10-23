// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;

using Foundation;

namespace MonoTouchFixtures.AudioUnit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AppExtensionSmokeTest {
		[Test]
		public void RunsInsideAudioUnitAppExtension ()
		{
			var bundlePath = NSBundle.MainBundle.BundlePath;
			if (!bundlePath.EndsWith (".appex", StringComparison.OrdinalIgnoreCase))
				Assert.Ignore ("This test only applies when monotouch-test is hosted from an app extension.");

			Assert.That (NSBundle.MainBundle.InfoDictionary? ["NSExtension"], Is.Not.Null, "NSExtension");
			Assert.That (TestLoader.GetTestAssemblies ().Count (), Is.GreaterThanOrEqualTo (3), "Test assembly count");
		}
	}
}
