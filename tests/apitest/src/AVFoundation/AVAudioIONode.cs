using System;
using NUnit.Framework;

using AppKit;
using Foundation;
using AudioUnit;
using AUUnit = AudioUnit.AudioUnit;
using AVFoundation;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class AVAudioIONodeTests
	{
		[Test]
		public void AVAudioIONodeTests_AudioUnitTest ()
		{
			Asserts.EnsureYosemite ();

			AVAudioEngine eng = new AVAudioEngine();
			AVAudioIONode node = eng.OutputNode;
			AUUnit unit = node.AudioUnit;
			unit.GetElementCount (AudioUnitScopeType.Global);
			// Make sure this doens't crash.
		}
	}
}