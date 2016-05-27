using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.AudioUnit;
using MonoMac.AVFoundation;
using AUUnit = MonoMac.AudioUnit.AudioUnit;
#else
using AppKit;
using Foundation;
using AudioUnit;
using AUUnit = AudioUnit.AudioUnit;
using AVFoundation;
#endif

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