#if !__WATCHOS__
#nullable enable

using System;

using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLLinkedFunctionsTest {
		MTLLinkedFunctions functions;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			functions = MTLLinkedFunctions.Create ();
		}

		[TearDown]
		public void TearDown ()
		{
			functions?.Dispose ();
			functions = null;
		}

		[Test]
		public void FunctionsTest ()
		{
			Assert.DoesNotThrow (() => {
				functions.Functions = null; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				var f = functions.Functions;
			}, "Getter");
		}

#if !__TVOS__  // Not present on tvos
		[Test]
		public void BinaryFunctions ()
		{
			Assert.DoesNotThrow (() => {
				functions.BinaryFunctions = null; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				var f = functions.BinaryFunctions;
			}, "Getter");
		}
#endif

		[Test]
		public void Groupstest ()
		{
			Assert.DoesNotThrow (() => {
				functions.Groups = null; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				var g = functions.Groups;
			}, "Getter");
		}
	}
}

#endif
