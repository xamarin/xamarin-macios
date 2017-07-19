// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
#if XAMCORE_2_0
using Foundation;
using AudioToolbox;
using AudioUnit;
using CoreFoundation;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.AudioToolbox;
using MonoTouch.CoreFoundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioComponentTest {

		[Test]
		public void GetSetComponentList ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			var types = new List<AudioTypeOutput> { AudioTypeOutput.Generic, AudioTypeOutput.Remote, AudioTypeOutput.VoiceProcessingIO };
			foreach (var t in types) {
				var resources = new ResourceUsageInfo ();
				resources.IOKitUserClient = new string[] { "CustomUserClient1" };
				resources.MachLookUpGlobalName = new string[] { "MachServiceName1" };
				resources.NetworkClient = false;
				resources.TemporaryExceptionReadWrite = false;

				var componentInfo = new AudioComponentInfo ();
				componentInfo.Type = t.ToString ();
				componentInfo.Subtype =  "XMPL";
				componentInfo.Name = "XMPL";
				componentInfo.Version = 1;
				componentInfo.ResourceUsage = resources;
				var component = AudioComponent.FindComponent (t);
				if (component == null)
					continue;
				var l = component.ComponentList;
				Assert.IsNull (l, "List is not null.");
				l = new AudioComponentInfo[] { componentInfo };
				//monotouchtests does not have permissions to deal with the hwd.
				Assert.Throws <InvalidOperationException> (() => component.ComponentList = l);
			}
		}
	}
}

#endif // !__WATCHOS__