using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#else
using AppKit;
using Foundation;
using ObjCRuntime;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class EveryFrameworkSmokeTests
	{
		enum LoadStatus { FailTest, Acceptable };

		LoadStatus CheckLoadFailure (string libraryName, string path)
		{
			// Easy pass if the library doesn't even exist on the system...
			if (!File.Exists (path))
				return LoadStatus.Acceptable;

			// No bindings for any of these yet
			switch (libraryName) {
			case "CryptoTokenKitLibrary":
			case "FinderSyncLibrary":
			case "HypervisorLibrary":
				return LoadStatus.Acceptable;
			}

			// These libraries only have 64-bit version
			bool is64Bit = IntPtr.Size == 8;
			if (!is64Bit) {
				switch (libraryName) {
				case "AVKitLibrary":
				case "AccountsLibrary":
				case "CloudKitLibrary":
				case "ContactsLibrary":
				case "ContactsUILibrary":
				case "CryptoTokenKitLibrary":
				case "EventKitLibrary":
				case "FinderSyncLibrary":
				case "GLKitLibrary":
				case "GameControllerLibrary":
				case "GameplayKitLibrary":
				case "HypervisorLibrary":
				case "LocalAuthenticationLibrary":
				case "MapKitLibrary":
				case "MediaLibraryLibrary":
				case "MetalKitLibrary":
				case "MetalPerformanceShadersLibrary":
				case "ModelIOLibrary":
				case "MultipeerConnectivityLibrary":
				case "NetworkExtensionLibrary":
				case "NotificationCenterLibrary":
				case "SceneKitLibrary":
				case "SocialLibrary":
				case "SpriteKitLibrary":
				case "PhotosLibrary":
				case "PhotosUILibrary":
				case "IntentsLibrary":
				case "MediaPlayerLibrary":
				case "VisionLibrary":
				case "CoreMLLibrary":
				case "ExternalAccessoryLibrary":
				case "CoreSpotlightLibrary":
					return LoadStatus.Acceptable;
				}
			}

			return LoadStatus.FailTest;
		}

		[Test]
		public void ExpectedLibrariesAreLoaded ()
		{
			List<string> failures = new List<string> ();

			// In the past, we've missed frameworks in NSObject.mac.cs and shipped brokeness
			// This test tries to verify every framework listed in Constants either is loaded or expected to not be
			foreach (FieldInfo info in typeof(Constants).GetFields ().Where (x => x.Name.EndsWith ("Library")) ) {
				string path = (string)info.GetRawConstantValue ();
				// Use RTLD_NOLOAD (0x10) so we don't load, just check to see if it is in memory
				IntPtr handle = Dlfcn.dlopen (path, 0x10);
				if (handle == IntPtr.Zero && CheckLoadFailure (info.Name, path) == LoadStatus.FailTest)
					failures.Add (string.Format ("{0} ({1}) failed to load but this was not expected", info.Name, path));
			}

			if (failures.Count > 0)
				Assert.Fail (string.Join ("\n", failures));
		}
	}
}
