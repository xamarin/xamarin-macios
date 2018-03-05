// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
using AudioToolbox;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.AudioToolbox;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioSessionTest {
		
		public AudioSessionTest ()
		{
			AudioSession.Initialize ();
		}
		
		[Test]
		public void Properties ()
		{
			var input = AudioSession.InputRoute;
			
			Assert.That (Enum.IsDefined (typeof (AudioSessionInputRouteKind), input), "InputRoute");
			if (Runtime.Arch == Arch.DEVICE) {
				// Apparently my iPad2 doesn't have microphone ?!?
				//Assert.That (input != AudioSessionInputRouteKind.None, "All known devices has microphones");
			}
			
			var outputs = AudioSession.OutputRoutes;
			if (outputs != null) {
				foreach (var output in outputs)
					Assert.That (Enum.IsDefined (typeof (AudioSessionOutputRouteKind), output), "Output: " + output.ToString ());
			}
			
			if (Runtime.Arch == Arch.DEVICE) {
				Assert.That (outputs != null && outputs.Length > 0, "All known devices have at least speakers #1");
				Assert.That (outputs [0] != AudioSessionOutputRouteKind.None, "All known devices have at least speakers #2");
			}
		}
	}
}
#endif // !__TVOS__ && !__WATCHOS__
