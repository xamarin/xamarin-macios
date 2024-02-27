#if __MACOS__
using System;
using System.Threading;
using NUnit.Framework;

using AppKit;
using AudioUnit;
using AudioToolbox;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AUGraphTests {
		int graphRenderCallbackCount = 0;
		int mixerRenderCallbackCount = 0;
		AUGraph graph;
		AudioUnit.AudioUnit mMixer;

		void SetupAUGraph ()
		{
			graph = new AUGraph ();

			AudioComponentDescription mixerDescription = new AudioComponentDescription ();
			mixerDescription.ComponentType = AudioComponentType.Mixer;
#if NET
			mixerDescription.ComponentSubType = (AudioUnitSubType) AudioTypeMixer.MultiChannel;
#else
			mixerDescription.ComponentSubType = (int)AudioTypeMixer.MultiChannel;
#endif
			mixerDescription.ComponentFlags = 0;
			mixerDescription.ComponentFlagsMask = 0;
			mixerDescription.ComponentManufacturer = AudioComponentManufacturerType.Apple;

			AudioComponentDescription outputDesciption = new AudioComponentDescription ();
			outputDesciption.ComponentType = AudioComponentType.Output;
#if NET
			outputDesciption.ComponentSubType = (AudioUnitSubType) AudioTypeOutput.System;
#else
			outputDesciption.ComponentSubType = (int)AudioTypeOutput.System;
#endif
			outputDesciption.ComponentFlags = 0;
			outputDesciption.ComponentFlagsMask = 0;
			outputDesciption.ComponentManufacturer = AudioComponentManufacturerType.Apple;

			int mixerNode = graph.AddNode (mixerDescription);
			int outputNode = graph.AddNode (outputDesciption);

			AUGraphError error = graph.ConnnectNodeInput (mixerNode, 0, outputNode, 0);
			Assert.AreEqual (AUGraphError.OK, error);

			graph.Open ();

			mMixer = graph.GetNodeInfo (mixerNode);

			AudioUnitStatus status = mMixer.SetElementCount (AudioUnitScopeType.Input, 0);
			Assert.AreEqual (AudioUnitStatus.OK, status);
		}

		[Test]
		public void DoTest ()
		{
			TestRuntime.AssertNotVirtualMachine ();

			SetupAUGraph ();

			// One of these has to be commented out depending on old\new build
			graph.AddRenderNotify (GraphRenderCallback);
			//graph.RenderCallback += HandleRenderCallback;

			AudioUnitStatus status = mMixer.SetRenderCallback (MixerRenderCallback);
			Assert.AreEqual (AudioUnitStatus.OK, status);

			WaitOnGraphAndMixerCallbacks ();
		}

		AudioUnitStatus GraphRenderCallback (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			graphRenderCallbackCount++;
			return AudioUnitStatus.NoError;
		}

		AudioUnitStatus MixerRenderCallback (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			mixerRenderCallbackCount++;
			return AudioUnitStatus.NoError;
		}

		void WaitOnGraphAndMixerCallbacks ()
		{
			graph.Initialize ();
			graph.Start ();

			// Wait for 1 second, then give up
			try {
				for (int i = 0; i < 100; ++i) {
					if (graphRenderCallbackCount > 0 && mixerRenderCallbackCount > 0)
						return;
					Thread.Sleep (10);
				}
				Assert.Fail ("Did not see events after 1 second");
			} finally {
				graph.Stop ();
			}
		}
	}
}

#endif // __MACOS__
