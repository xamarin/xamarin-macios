#if !__WATCHOS__
#nullable enable

using System;

using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLCounterSampleBufferDescriptorTest {
		MTLCounterSampleBufferDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
			descriptor = new MTLCounterSampleBufferDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor?.Dispose ();
			descriptor = null;
		}

		[Test]
		public void CounterSetTest ()
		{
			Assert.DoesNotThrow (() => {
				descriptor.CounterSet = null; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				using var buffer = descriptor.CounterSet;
			}, "Getter");
		}

		[Test]
		public void LabelTest ()
		{
			string newLabel = "MyLabel";
			string objLabel = null;

			Assert.DoesNotThrow (() => {
				descriptor.Label = newLabel;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objLabel = descriptor.Label;
			}, "Getter");
			Assert.AreEqual (newLabel, objLabel, "Label");
		}

		[Test]
		public void StorageModeTest ()
		{
			var newMode = MTLStorageMode.Private;
			var objMode = MTLStorageMode.Memoryless;
			Assert.DoesNotThrow (() => {
				descriptor.StorageMode = newMode;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objMode = descriptor.StorageMode;
			}, "Getter");
			Assert.AreEqual (newMode, objMode, "Mode");
		}

		[Test]
		public void SampleCountTest ()
		{
			nuint newCount = 10;
			nuint objCount = 0;

			Assert.DoesNotThrow (() => {
				descriptor.SampleCount = newCount;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objCount = descriptor.SampleCount;
			}, "Getter");
			Assert.AreEqual (newCount, objCount, "Count");
		}
	}
}

#endif // !__WATCHOS__
