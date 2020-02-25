using System;
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

		[Test]
		public void StaticCtor ()
		{
			ContinueOnFailure = true;
			foreach (Type t in Assembly.GetTypes ()) {
				var cctor = t.GetConstructor (BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
				if (cctor == null)
					continue;
				// we don't skip based on availability attributes since the execution of .cctor can easily happen indirectly and
				// we rather catch them all here *now* than trying to figure out how to replicate the specific conditions *later*
				try {
					RuntimeHelpers.RunClassConstructor (t.TypeHandle);
				}
				catch (TypeInitializationException e) {
					ReportError ($"{t.FullName} .cctor could not execute properly: {e}");
				}
			}
			AssertIfErrors ($"{Errors} execution failure(s)");
		}
	}
}