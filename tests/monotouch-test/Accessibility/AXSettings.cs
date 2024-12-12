//
// Copyright 2024 Microsoft Corp
//

using System;
using System.Threading.Tasks;

using Accessibility;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.Accessibility {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AXSettingsTest {
#if NET
		[Test]
		public void IsAssistiveAccessEnabled ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);
			Assert.That (AXSettings.IsAssistiveAccessEnabled, Is.EqualTo (true).Or.EqualTo (false), "IsAssistiveAccessEnabled");
		}

		static bool testedOnce;
		[Test]
		public void OpenSettingsFeature ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			if (!testedOnce) {
				Assert.Ignore ("This test opens the Settings app (stopping executing of monotouch-test), so it's ignored when running automatically. Run again to actually run it (you'll have to switch back to monotouch-test manually).");
				testedOnce = true;
			}

			NSError? error = null;
			var didComplete = new TaskCompletionSource<bool> ();
			AXSettings.OpenSettingsFeature (AXSettingsFeature.PersonalVoiceAllowAppsToRequestToUse, (e) => {
				e = error;
				didComplete.TrySetResult (true);
			});
			Assert.IsTrue (TestRuntime.RunAsync (TimeSpan.FromSeconds (30), didComplete.Task), "Timed out");
			Assert.IsNull (error);
		}
#endif
	}
}
