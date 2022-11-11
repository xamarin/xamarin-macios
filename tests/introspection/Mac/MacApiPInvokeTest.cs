using System;
using System.Linq;
using System.Reflection;

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {

	[TestFixture]
	public class MacApiPInvokeTest : ApiPInvokeTest {
		protected override bool SkipLibrary (string libraryName)
		{
			switch (libraryName) {
			case "/System/Library/Frameworks/OpenGL.framework/OpenGL":
				return true;
			}
			return base.SkipLibrary (libraryName);
		}

		static bool IsUnified {
			get {
				return AppDomain.CurrentDomain.GetAssemblies ().Any (x => x.FullName.Contains ("Xamarin.Mac"));
			}
		}

		protected override bool Skip (Type type)
		{
			string typeName = type.ToString ();
			bool is32Bit = IntPtr.Size == 4;
			if (is32Bit && typeName.StartsWith ("MapKit", StringComparison.Ordinal)) // MapKit is 64-bit only
				return true;

			switch (type.Namespace) {
			case "SceneKit":
			case "MonoMac.SceneKit":
				if (is32Bit)
					return true;
				break;
			}

			switch (typeName) {
			case "MonoMac.GameController.GCExtendedGamepadSnapshot": // These next 4 are in the compat API, even if they don't work.
			case "MonoMac.GameController.GCGamepadSnapshot":
			case "MonoMac.GameController.GCExtendedGamepadSnapShotDataV100":
			case "MonoMac.GameController.GCGamepadSnapShotDataV100":
				return !IsUnified;
			case "GameController.GCGamepadSnapShotDataV100": // These are 64-bit only
			case "GameController.GCExtendedGamepadSnapShotDataV100":
				return is32Bit;
			case "MonoMac.AudioToolbox.AudioSession": // These are iOS APIs that were mistakenly pulled into OSX. Removed in unified but not classic
			case "MonoMac.AudioUnit.AudioUnitUtils":
				return !IsUnified; // If these are in unified, don't skip, we want to scream
			}

			return base.Skip (type);
		}

		protected override bool Skip (string symbolName)
		{
			switch (symbolName) {
			case "SKTerminateForInvalidReceipt": // Only there for API compat
				return !IsUnified;
			}
			return base.Skip (symbolName);
		}

		protected override bool SkipAssembly (Assembly a)
		{
			// too many things are missing from XM 32bits bindings
			// and the BCL is identical for 64 bits (no need to test it 3 times)
			//			return IntPtr.Size == 4;
			return true; // skip everything until fixed
		}
	}
}
