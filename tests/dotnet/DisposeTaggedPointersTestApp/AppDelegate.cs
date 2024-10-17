using System;
using System.Runtime.InteropServices;

using Foundation;

namespace DisposeTaggedPointersTestApp {
	public class Program {
		static int Main (string [] args)
		{
			var testCaseString = Environment.GetEnvironmentVariable ("TEST_CASE");
			if (string.IsNullOrEmpty (testCaseString)) {
				Console.WriteLine ($"The environment variable TEST_CASE wasn't set.");
				return 2;
			}
#if NET10_0_OR_GREATER
			var taggedPointersDisposedByDefault = false;
#else
			var taggedPointersDisposedByDefault = true;
#endif
			switch (testCaseString) {
			case "DisableWithAppContext":
				AppContext.SetSwitch ("Foundation.NSObject.DisposeTaggedPointers", false);
				if (ThrowsObjectDisposedExceptions ()) {
					Console.WriteLine ($"❌ {testCaseString}: Failure: unexpected ObjectDisposedException when DisposeTaggedPointers=false");
					return 1;
				}

				Console.WriteLine ($"✅ {testCaseString}: Success");
				return 0;
			case "EnableWithAppContext":
				AppContext.SetSwitch ("Foundation.NSObject.DisposeTaggedPointers", true);
				if (!ThrowsObjectDisposedExceptions ()) {
					Console.WriteLine ($"❌ {testCaseString}: Failure: unexpected lack of ObjectDisposedException when DisposeTaggedPointers=true");
					return 1;
				}

				Console.WriteLine ($"✅ {testCaseString}: Success");
				return 0;
			case "DisableWithMSBuildPropertyTrimmed":
				if (ThrowsObjectDisposedExceptions ()) {
					Console.WriteLine ($"❌ {testCaseString}: Failure: unexpected ObjectDisposedException when DisposeTaggedPointers=false");
					return 1;
				}

				Console.WriteLine ($"✅ {testCaseString}: Success");
				return 0;
			case "EnableWithMSBuildPropertyTrimmed":
				if (!ThrowsObjectDisposedExceptions ()) {
					Console.WriteLine ($"❌ {testCaseString}: Failure: unexpected lack of ObjectDisposedException when DisposeTaggedPointers=true");
					return 1;
				}

				Console.WriteLine ($"✅ {testCaseString}: Success");
				return 0;
			case "DisableWithMSBuildPropertyUntrimmed": {
				var throws = ThrowsObjectDisposedExceptions ();
				if (throws == taggedPointersDisposedByDefault) {
					Console.WriteLine ($"❌ {testCaseString}: Failure: unexpected ObjectDisposedException when DisposeTaggedPointers=false");
					return 1;
				}

				Console.WriteLine ($"✅ {testCaseString}: Success");
				return 0;
			}
			case "EnableWithMSBuildPropertyUntrimmed": {
				var throws = ThrowsObjectDisposedExceptions ();
				if (throws != taggedPointersDisposedByDefault) {
					Console.WriteLine ($"❌ {testCaseString}: Failure: unexpected lack of ObjectDisposedException when DisposeTaggedPointers=true");
					return 1;
				}

				Console.WriteLine ($"✅ {testCaseString}: Success");
				return 0;
			}
			case "Default": {
				var throws = ThrowsObjectDisposedExceptions ();
				if (throws != taggedPointersDisposedByDefault) {
					Console.WriteLine ($"❌ {testCaseString}: Failure: Expected ObjectDisposedException: {taggedPointersDisposedByDefault} Threw ObjectDisposedException: {throws}");
					return 1;
				}

				Console.WriteLine ($"✅ {testCaseString}: Success");
				return 0;
			}
			default:
				Console.WriteLine ($"❌ Unknown test case: {testCaseString}");
				return 2;
			}
		}

		static bool ThrowsObjectDisposedExceptions () => MonoTouchFixtures.ObjCRuntime.TaggedPointerTest.ThrowsObjectDisposedExceptions ();
	}
}
