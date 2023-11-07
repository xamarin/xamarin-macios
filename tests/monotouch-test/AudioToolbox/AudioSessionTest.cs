// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC && !XAMCORE_3_0

using System;
using Foundation;
using AudioToolbox;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioSessionTest {

		public AudioSessionTest ()
		{
			TestRuntime.RequestMicrophonePermission ();
			AudioSession.Initialize ();
		}

		[Test]
		public void Properties ()
		{
			var input = AudioSession.InputRoute;

			Assert.That (Enum.IsDefined (typeof (AudioSessionInputRouteKind), input), "InputRoute");

			var outputs = AudioSession.OutputRoutes;
			if (outputs is not null) {
				foreach (var output in outputs)
					Assert.That (Enum.IsDefined (typeof (AudioSessionOutputRouteKind), output), "Output: " + output.ToString ());
			}

			if (TestRuntime.IsDevice) {
				Assert.That (outputs is not null && outputs.Length > 0, "All known devices have at least speakers #1");
				Assert.That (outputs [0] != AudioSessionOutputRouteKind.None, "All known devices have at least speakers #2");
			}
		}
	}
}
#endif // !__TVOS__ && !__WATCHOS__ && !MONOMAC && !XAMCORE_3_0
