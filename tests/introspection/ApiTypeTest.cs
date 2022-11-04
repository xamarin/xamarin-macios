using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

using NUnit.Framework;
using Xamarin.Utils;

using Foundation;
using ObjCRuntime;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class ApiTypeTest : ApiBaseTest {

		bool Skip (Type type)
		{
			switch (type.Namespace) {
#if __IOS__ || __WATCHOS__
			// running the .cctor on the simulator works... but makes some other CoreNFC intro tests fail later
			// we'll still get the results from device tests
			case "CoreNFC":
			case "DeviceCheck":
				// we can't call the `NFCNdefReaderSession.ReadingAvailable` API on 32bits (PlatformNotSupportedException)
				// and if we call it then the .cctor is executed and we get the same failures :()
				return ((IntPtr.Size == 4) || TestRuntime.IsSimulatorOrDesktop);
#endif
			default:
				return false;
			}
		}

		[Test]
		public void StaticCtor ()
		{
			var issues = new HashSet<string> ();
			ContinueOnFailure = true;
			foreach (Type t in Assembly.GetTypes ()) {
				if (Skip (t))
					continue;

				var cctor = t.GetConstructor (BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
				if (cctor == null)
					continue;
				// we don't skip based on availability attributes since the execution of .cctor can easily happen indirectly and
				// we rather catch them all here *now* than trying to figure out how to replicate the specific conditions *later*
				try {
					RuntimeHelpers.RunClassConstructor (t.TypeHandle);
				}
				catch (TypeInitializationException e) {
					issues.Add (t.FullName);
					ReportError ($"{t.FullName} .cctor could not execute properly: {e}");
				}
			}
			AssertIfErrors ($"{Errors} execution failure(s): " + String.Join (',', issues));
		}
	}
}
