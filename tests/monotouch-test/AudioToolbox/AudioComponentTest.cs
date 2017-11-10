// Copyright 2011 Xamarin Inc. All rights reserved

#if IOS || MONOMAC
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
#if !MONOMAC
			var types = new List<AudioTypeOutput> { AudioTypeOutput.Generic, AudioTypeOutput.Remote, AudioTypeOutput.VoiceProcessingIO };
#else
			var types = new List<AudioTypeOutput> { AudioTypeOutput.Generic, AudioTypeOutput.Default, AudioTypeOutput.HAL, AudioTypeOutput.System, AudioTypeOutput.VoiceProcessingIO };
#endif
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
		
		[Test]
		public void GetSetNullComponentList ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
#if !MONOMAC
			var types = new List<AudioTypeOutput> { AudioTypeOutput.Generic, AudioTypeOutput.Remote, AudioTypeOutput.VoiceProcessingIO };
#else
			var types = new List<AudioTypeOutput> { AudioTypeOutput.Generic, AudioTypeOutput.Default, AudioTypeOutput.HAL, AudioTypeOutput.System, AudioTypeOutput.VoiceProcessingIO };
#endif
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
				//monotouchtests does not have permissions to deal with the hwd.
				Assert.Throws <ArgumentNullException> (() => component.ComponentList = null);
			}
		}

		// test the diff properties of the ResourceUsageInfo since it was manually done
		
		[Test]
		public void TestResourceUsageInfoIOKitUserClient ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			var clientId = "CustomUserClient1";
			var resources = new ResourceUsageInfo ();
			resources.IOKitUserClient = new string[] { clientId };
			var userClientList = resources.IOKitUserClient;
			Assert.IsNotNull (userClientList);
			Assert.AreEqual (1, userClientList.Length, "List does not have all client ids.");
			Assert.AreEqual (clientId, userClientList [0], "Client ids are not the same.");

			// similar test but with null values.
			
			resources.IOKitUserClient = null;
			Assert.IsNull (resources.IOKitUserClient, "Value was not set to null.");
		}
		
		[Test]
		public void TestResourceUsageInfoMachLookUpGlobalName ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			var serviceName = "MachServiceName1";
			var resources = new ResourceUsageInfo ();
			resources.MachLookUpGlobalName = new string[] { serviceName };
			var serviceNames = resources.MachLookUpGlobalName;
			Assert.NotNull (serviceNames, "Returned list is null");
			Assert.AreEqual (1, serviceNames.Length, "List does not have all service names.");
			Assert.AreEqual (serviceName, serviceNames [0], "Service names are not equal.");

			// similar test but with null values
			
			resources.MachLookUpGlobalName = null;
			Assert.IsNull (resources.MachLookUpGlobalName, "Value was no set to null.");
		}
	}
}

#endif