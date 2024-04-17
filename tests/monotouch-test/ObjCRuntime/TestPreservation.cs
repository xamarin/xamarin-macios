using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VerifyAllTestsArePreserved {
		[Test]
		public void Test ()
		{
			if (TestRuntime.IsLinkAll) {
				// The linker will remove the Preserve attribute, so this test doesn't make sense when the linker is enabled.
				// Also it wouldn't find tests without the Preserve attribute reliably anyway, since the linker would link those tests away.
				Assert.Ignore ("This test does not make sense to run with the linker enabled for all assemblies.");
			}
			TestAssembly (typeof (TestRuntime).Assembly);
		}

#if NET
		[UnconditionalSuppressMessage ("Trimming", "IL2026", Justification = "This test verifies that all tests have been correctly preserved, and fails if that's not the case. It'll thus fail if the trimmer removed anything it didn't expect - and is then technically trimmer safe in that it's aware and react accordingly if there's any behavioral change when the trimmer is enabled.")]
		[UnconditionalSuppressMessage ("Trimming", "IL2045", Justification = "This test verifies that all tests have been correctly preserved, and fails if that's not the case. It'll thus fail if the trimmer removed anything it didn't expect - and is then technically trimmer safe in that it's aware and react accordingly if there's any behavioral change when the trimmer is enabled.")]
#endif
		void TestAssembly (Assembly asm)
		{
			var failedTypes = new List<string> ();
			foreach (var type in asm.GetTypes ()) {
				if (!IsTestFixture (type))
					continue;

				if (type.IsDefined (typeof (PreserveAttribute)))
					continue;

				if (Skip (type))
					continue;

				failedTypes.Add (type.FullName);
			}

			Assert.That (failedTypes, Is.Empty, "Failed types");
		}

		bool Skip (Type type)
		{
			switch (type.FullName) {
			// We get the System.Drawing tests from source from mono, so we can't fix them (easily).
			case "MonoTests.System.Drawing.PointTest":
			case "MonoTests.System.Drawing.PointFTest":
			case "MonoTests.System.Drawing.TestRectangle":
			case "MonoTests.System.Drawing.TestRectangleF":
			case "MonoTests.System.Drawing.SizeTest":
			case "MonoTests.System.Drawing.SizeFTest":
				return true;
			}

			return false;
		}

#if NET
		[UnconditionalSuppressMessage ("Trimming", "IL2070", Justification = "This test verifies that all tests have been correctly preserved, and fails if that's not the case. It'll thus fail if the trimmer removed anything it didn't expect - and is then technically trimmer safe in that it's aware and react accordingly if there's any behavioral change when the trimmer is enabled.")]
#endif
		bool IsTestFixture (Type type)
		{
			// what's a test fixture: https://docs.nunit.org/articles/nunit/writing-tests/attributes/testfixture.html
			if (type is null || type == typeof (object))
				return false;

			if (type.IsDefined (typeof (TestFixtureAttribute)))
				return true;

			if (IsTestFixture (type.BaseType))
				return true;

			// "So long as the class contains at least one method marked with the Test, TestCase or TestCaseSource attribute, it will be treated as a test fixture."
			foreach (var method in type.GetMethods (BindingFlags.Public | BindingFlags.Instance)) {
				if (method.IsDefined (typeof (TestAttribute)))
					return true;

				if (method.IsDefined (typeof (TestCaseAttribute)))
					return true;

				if (method.IsDefined (typeof (TestCaseSourceAttribute)))
					return true;
			}

			return false;
		}
	}
}
