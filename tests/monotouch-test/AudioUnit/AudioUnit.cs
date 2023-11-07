#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using AudioUnit;
using theUnit = AudioUnit.AudioUnit;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioUnitTests {
		theUnit GetAudioUnitForTest ()
		{
			AudioComponentDescription desc = new AudioComponentDescription ();
			desc.ComponentType = AudioComponentType.Output;
#if NET
			desc.ComponentSubType = AudioUnitSubType.HALOutput; // 'ahal'
#else
			desc.ComponentSubType = 1634230636; // 'ahal'
#endif
			desc.ComponentFlags = 0;
			desc.ComponentFlagsMask = 0;
			desc.ComponentManufacturer = AudioComponentManufacturerType.Apple;

			AudioComponent comp = AudioComponent.FindNextComponent (null, ref desc);
			theUnit unit = new theUnit (comp);
			return unit;
		}

		[Test]
		public void GetCurrentDevice_Test ()
		{
			TestRuntime.AssertNotVirtualMachine ();

			theUnit unit = GetAudioUnitForTest ();

			uint device = unit.GetCurrentDevice (AudioUnitScopeType.Global);
			Assert.IsTrue (device != 0);
		}

		[Test]
		public void AudioObjectPropertySelector4CCTest ()
		{
			Assert.That (FourCC ((int) AudioObjectPropertySelector.Devices), Is.EqualTo ("dev#"), "dev#");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.DefaultInputDevice), Is.EqualTo ("dIn "), "dIn ");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.DefaultOutputDevice), Is.EqualTo ("dOut"), "dOut");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.DefaultSystemOutputDevice), Is.EqualTo ("sOut"), "sOut");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.TranslateUIDToDevice), Is.EqualTo ("uidd"), "uidd");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.MixStereoToMono), Is.EqualTo ("stmo"), "stmo");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.PlugInList), Is.EqualTo ("plg#"), "plg#");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.TranslateBundleIDToPlugIn), Is.EqualTo ("bidp"), "bidp");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.TransportManagerList), Is.EqualTo ("tmg#"), "tmg#");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.TranslateBundleIDToTransportManager), Is.EqualTo ("tmbi"), "tmbi");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.BoxList), Is.EqualTo ("box#"), "box#");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.TranslateUIDToBox), Is.EqualTo ("uidb"), "uidb");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.ProcessIsMaster), Is.EqualTo ("mast"), "mast");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.IsInitingOrExiting), Is.EqualTo ("inot"), "inot");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.UserIDChanged), Is.EqualTo ("euid"), "euid");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.ProcessIsAudible), Is.EqualTo ("pmut"), "pmut");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.SleepingIsAllowed), Is.EqualTo ("slep"), "slep");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.UnloadingIsAllowed), Is.EqualTo ("unld"), "unld");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.HogModeIsAllowed), Is.EqualTo ("hogr"), "hogr");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.UserSessionIsActiveOrHeadless), Is.EqualTo ("user"), "user");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.ServiceRestarted), Is.EqualTo ("srst"), "srst");
			Assert.That (FourCC ((int) AudioObjectPropertySelector.PowerHint), Is.EqualTo ("powh"), "powh");
		}

		[Test]
		public void AudioObjectPropertyScope4CCTest ()
		{
			Assert.That (FourCC ((int) AudioObjectPropertyScope.Global), Is.EqualTo ("glob"), "glob");
			Assert.That (FourCC ((int) AudioObjectPropertyScope.Input), Is.EqualTo ("inpt"), "inpt");
			Assert.That (FourCC ((int) AudioObjectPropertyScope.Output), Is.EqualTo ("outp"), "outp");
			Assert.That (FourCC ((int) AudioObjectPropertyScope.PlayThrough), Is.EqualTo ("ptru"), "ptru");
		}

		string FourCC (int value)
		{
			return new string (new char [] {
				(char) (byte) (value >> 24),
				(char) (byte) (value >> 16),
				(char) (byte) (value >> 8),
				(char) (byte) value });
		}
	}
}

#endif // __MACOS__
