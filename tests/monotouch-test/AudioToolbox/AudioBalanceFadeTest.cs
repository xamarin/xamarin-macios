//
// Unit tests for AudioBalanceFade
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.IO;
using Foundation;
using AudioToolbox;
using CoreFoundation;
using NUnit.Framework;
using System.Threading;

namespace MonoTouchFixtures.AudioToolbox {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioBalanceFadeTest
	{
		[Test]
		public void GetBalanceFade ()
		{
			var acl = AudioChannelLayout.FromAudioChannelLayoutTag (AudioChannelLayoutTag.AudioUnit_6_1);
			var abf = new AudioBalanceFade (acl);
			Assert.IsNotNull (abf.GetBalanceFade ());
		}
	}
}

#endif // !__WATCHOS__
