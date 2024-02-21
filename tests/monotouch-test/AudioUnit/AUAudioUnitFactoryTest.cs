//
// Unit tests for AUAudioUnitFactory
//
// Authors:
//	Oleg Demchenko (oleg.demchenko@xamarin.com)
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;

using NUnit.Framework;

using Foundation;
using AudioUnit;

namespace MonoTouchFixtures.AudioUnit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AUAudioUnitFactoryTest {
		[Test]
		public void CreateAudioUnit ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			const string expectedManufacturer = "Apple";
			var desc = new AudioComponentDescription {
				ComponentType = AudioComponentType.Output,
#if MONOMAC
#if NET
				ComponentSubType = AudioUnitSubType.VoiceProcessingIO,
#else
				ComponentSubType = (int)AudioUnitSubType.VoiceProcessingIO,
#endif
#else
#if NET
				ComponentSubType = (AudioUnitSubType) AudioTypeOutput.Remote,
#else
				ComponentSubType = 0x72696f63, // Remote_IO
#endif
#endif
				ComponentManufacturer = AudioComponentManufacturerType.Apple
			};

			using (var auFactory = new CustomAudioUnitFactory ()) {
				NSError error;
				using (var audioUnit = auFactory.CreateAudioUnit (desc, out error)) {
					Assert.True (audioUnit is not null, "CustomAudioUnitFactory returned null object for valid component description");
					Assert.True (audioUnit.ManufacturerName == expectedManufacturer,
						$"CustomAudioUnitFactory returned audio unit with incorrect manufacturer. Expected - {expectedManufacturer}, actual - {audioUnit.ManufacturerName}");
				}
			}
		}

		public class CustomAudioUnitFactory : NSObject, IAUAudioUnitFactory {
			public AUAudioUnit CreateAudioUnit (AudioComponentDescription desc, out NSError error)
			{
				var audioUnit = new AUAudioUnit (desc, out error);
				return audioUnit;
			}

			public void BeginRequestWithExtensionContext (NSExtensionContext context)
			{
				throw new NotImplementedException ();
			}
		}
	}
}

#endif // !__WATCHOS__
