#if !__WATCHOS__
#nullable enable

using System;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLTileRenderPipelineDescriptorTest {

		MTLTileRenderPipelineDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
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
