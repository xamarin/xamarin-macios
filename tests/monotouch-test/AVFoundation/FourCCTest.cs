using Foundation;
using AVFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVFoundationEnumTest {
		string FourCC (int value)
		{
			return new string (new char [] {
				(char) (byte) (value >> 24),
				(char) (byte) (value >> 16),
				(char) (byte) (value >> 8),
				(char) (byte) value });
		}

		[Test]
		public void PromptStyle ()
		{
			Assert.That (FourCC ((int) AVAudioSessionPromptStyle.None), Is.EqualTo ("none"), "None");
			Assert.That (FourCC ((int) AVAudioSessionPromptStyle.Short), Is.EqualTo ("shrt"), "Short");
			Assert.That (FourCC ((int) AVAudioSessionPromptStyle.Normal), Is.EqualTo ("nrml"), "Normal");
		}
	}
}
