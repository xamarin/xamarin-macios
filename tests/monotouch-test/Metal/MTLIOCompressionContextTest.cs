using System;
using System.IO;
using System.Linq;
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	public class MTLIOCompressionContextTest {

		string outputPath = string.Empty;
		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);
			var paths = NSSearchPath.GetDirectories (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
			
			// Convert once
			outputPath = Path.Combine (paths [0], "output1");
		}
		

		[TearDown]
		public void TearDown ()
		{
			File.Delete (outputPath);
		}

		[Test]
		public void DefaultChunkSize ()
		{
			TestRuntime.AssertNotSimulator (); // metal api no supported on the sim
			Assert.AreNotEqual (-1, MTLIOCompressionContext.DefaultChunkSize);
		}

		[Test]
		public void CreateAndFlushTest ()
		{
			TestRuntime.AssertNotSimulator (); // metal api no supported on the sim
			// create and flush, test should simple pass, no need to asserts
			using var compressIO = MTLIOCompressionContext.Create (outputPath, MTLIOCompressionMethod.Lzfse,
				MTLIOCompressionContext.DefaultChunkSize);
			Assert.NotNull (compressIO, "Null compress IO");
			// add data
			var data =  Enumerable.Repeat((byte)0x20, 20).ToArray();
			compressIO!.AppendData (data);
			compressIO!.FlushAndDestroy ();
			// ensure we do not have issues with a second flush and destroy
			compressIO.Dispose ();
		}
	}
}
