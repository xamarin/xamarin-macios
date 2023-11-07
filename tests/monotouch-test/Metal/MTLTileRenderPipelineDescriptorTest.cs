#if !__WATCHOS__ && !__TVOS__
#nullable enable

using System;

using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLTileRenderPipelineDescriptorTest {

		MTLTileRenderPipelineDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			descriptor = new MTLTileRenderPipelineDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor?.Dispose ();
			descriptor = null;
		}

		[Test]
		public void BinaryArchivesTest ()
		{
			// we want to make sure we do not crash because intro fails 
			Assert.DoesNotThrow (() => {
				descriptor.BinaryArchives = null; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				var bin = descriptor.BinaryArchives;
			}, "Getter");
		}
	}
}
#endif
