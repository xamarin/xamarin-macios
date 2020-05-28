//
// Unit tests for AudioUnit
//

#if !__WATCHOS__

using System;
using NUnit.Framework;
using System.Runtime.InteropServices;

using Foundation;
using AudioUnit;
using AudioToolbox;
using ObjCRuntime;

namespace MonoTouchFixtures.AudioUnit
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioUnitTest
	{
		[Test]
		public void DisposeMethodTest ()
		{
			// Test case from bxc #5410

			// Create instance of AudioUnit object
			AudioComponentDescription cd = new AudioComponentDescription () {
				ComponentType = AudioComponentType.Output,
#if MONOMAC
				ComponentSubType = (int)AudioUnitSubType.VoiceProcessingIO,
#else
				ComponentSubType = 0x72696f63, // Remote_IO
#endif
				ComponentManufacturer = AudioComponentManufacturerType.Apple
			};
			AudioComponent component = AudioComponent.FindComponent (ref cd);
			var audioUnit = component.CreateAudioUnit ();

			audioUnit.Dispose ();
		}

		[Test]
		public void GetElementCount ()
		{
			var graph = new AUGraph ();
			var mixerNode = graph.AddNode (AudioComponentDescription.CreateMixer (AudioTypeMixer.MultiChannel));
			graph.Open ();
			var mixer = graph.GetNodeInfo (mixerNode);
			Assert.AreEqual (1, mixer.GetElementCount (AudioUnitScopeType.Global));
		}
	}
}

#endif // !__WATCHOS__
