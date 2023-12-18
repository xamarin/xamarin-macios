#if !__WATCHOS__
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	[Preserve (AllMembers = true)]
	public class MTLIOCompressionContextTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);
		}


		[Test]
		public void DefaultChunkSize ()
		{
			TestRuntime.AssertNotSimulator (); // metal api not supported on the sim
			Assert.AreNotEqual (-1, MTLIOCompressionContext.DefaultChunkSize);
		}

		[Test]
		public void CreateAndFlushTest ()
		{
			TestRuntime.AssertNotSimulator (); // metal api not supported on the sim
			var outputPath = Path.GetTempFileName ();
			try {
				// create and flush, test should simple pass, no need to asserts
				var compressIO = MTLIOCompressionContext.Create (outputPath, MTLIOCompressionMethod.Lzfse,
					MTLIOCompressionContext.DefaultChunkSize);
				Assert.NotNull (compressIO, "Null compress IO");
				// add data
				var data = Enumerable.Repeat ((byte) 0x20, 20).ToArray ();
				compressIO!.AppendData (data);
				compressIO!.FlushAndDestroy ();
				// ensure we do not have issues with a second flush and destroy
				compressIO.Dispose ();
			} finally {
				File.Delete (outputPath);
			}
		}
	}
}
#endif
