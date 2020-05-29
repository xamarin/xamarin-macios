using System;
using NUnit.Framework;

using AppKit;
using Foundation;
using AudioUnit;
using AVFoundation;
using AUUnit = AudioUnit.AudioUnit;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class AVPlayerLayerTests
	{
		[Test]
		public void AVPlayerLayer_VideoGravity ()
		{
			AVPlayerLayer layer = new AVPlayerLayer ();
			Assert.IsNotNull (layer.VideoGravity);
		}
	}
}