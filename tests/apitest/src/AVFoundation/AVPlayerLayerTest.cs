using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.AudioUnit;
using MonoMac.AVFoundation;
#else
using AppKit;
using Foundation;
using AudioUnit;
using AVFoundation;
using AUUnit = AudioUnit.AudioUnit;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class AVPlayerLayerTests
	{
		[Test]
		public void AVPlayerLayer_VideoGravity ()
		{
			AVPlayerLayer layer = new AVPlayerLayer ();
#if XAMCORE_2_0
			Assert.IsNotNull (layer.VideoGravity);
#else
			Assert.IsNotNull (layer.LayerVideoGravity);
#endif
		}
	}
}