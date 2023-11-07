#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using Foundation;
using AudioUnit;
using AUUnit = AudioUnit.AudioUnit;
using AVFoundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVAudioIONodeTests {
		[Test]
		public void AVAudioIONodeTests_AudioUnitTest ()
		{
			TestRuntime.AssertNotVirtualMachine ();

			Asserts.EnsureYosemite ();

			using (AVAudioEngine eng = new AVAudioEngine ()) {
				using (AVAudioIONode node = eng.OutputNode) {
					using (AUUnit unit = node.AudioUnit)
						unit.GetElementCount (AudioUnitScopeType.Global);
					using (AUUnit unit = node.AudioUnit)
						unit.GetElementCount (AudioUnitScopeType.Global);
					using (AUUnit unit = node.AudioUnit)
						unit.GetElementCount (AudioUnitScopeType.Global);
				}
			}
			// Make sure this doens't crash.
		}
	}
}
#endif // __MACOS__
