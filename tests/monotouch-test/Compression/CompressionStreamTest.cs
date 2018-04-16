using System;
using System.IO;
using System.Threading.Tasks;

using Compression;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.Compression {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CompressionStreamTest {

#if MONOMAC
                string lzeCompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents/Resources/compressed_lze");
                string lzeUncompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents/Resources/uncompressed.json");
                
#else
                string lzeCompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "compressed_lze");
                string lzeUncompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "uncompressed.json");

#endif

		string tempPath;
		
		[SetUp]
		public void SetUp ()
		{
			tempPath = Path.GetTempFileName ();
		}
		
		[TearDown]
		public void TearDown ()
		{
			if (File.Exists (tempPath))
				File.Delete (tempPath);
		}
		
		[Test]
		public async Task TestDecode ()
		{
		
			using (var inputStream = File.OpenRead (lzeCompressedFilePath))
			using (var outputStream = File.Create (tempPath))
			using (var stream = new CompressionStream (inputStream, outputStream, StreamOperation.Decode, CompressionAlgorithm.LZ4)) {
				await stream.Process ();
			}
			// ensure that the data is the expected one
			string[] result = File.ReadAllLines (tempPath);
			string [] expected = File.ReadAllLines (lzeUncompressedFilePath);

			Assert.AreEqual (expected, result);
		}

	}
}
