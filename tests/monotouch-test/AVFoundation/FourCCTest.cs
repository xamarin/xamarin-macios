using System;
using Foundation;
using AVFoundation;
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVFoundationEnumTest {
		public static string FourCC (int value)
		{
			return new string (new char [] {
				(char) (byte) (value >> 24),
				(char) (byte) (value >> 16),
				(char) (byte) (value >> 8),
				(char) (byte) value });
		}

		public static string FourCC (uint value)
		{
			return new string (new char [] {
				(char) (byte) (value >> 24),
				(char) (byte) (value >> 16),
				(char) (byte) (value >> 8),
				(char) (byte) value });
		}

		public static uint FourCC (string value)
		{
			if (value.Length != 4)
				throw new ArgumentOutOfRangeException ();
			return (((uint) value [0]) << 24) +
				   (((uint) value [1]) << 16) +
				   (((uint) value [2]) << 8) +
				   (((uint) value [3]) << 0);
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
