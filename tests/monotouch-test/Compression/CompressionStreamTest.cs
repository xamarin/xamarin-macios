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
		string lzeUncompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents/Resources/uncompressed.txt");
#else
		string lzeCompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "compressed_lze");
		string lzeUncompressedFilePath = Path.Combine (NSBundle.MainBundle.BundlePath, "uncompressed.txt");
#endif

		string firstTempPath;
		string secondTempPath;
		
		[SetUp]
		public void SetUp ()
		{
			firstTempPath = Path.GetTempFileName ();
			secondTempPath = Path.GetTempFileName ();
		}
		
		[TearDown]
		public void TearDown ()
		{
			if (File.Exists (firstTempPath))
				File.Delete (firstTempPath);
			if (File.Exists (secondTempPath))
				File.Delete (secondTempPath);
		}
		
		[Test]
		public void TestDecodeRealFile ()
		{
		
			using (var inputStream = File.OpenRead (lzeCompressedFilePath))
			using (var outputStream = File.Create (firstTempPath))
			using (var stream = new CompressionStream (inputStream, outputStream, StreamOperation.Decode, CompressionAlgorithm.LZFSE)) {
				var diff = stream.Process ();
			}
			// ensure that the data is the expected one
			string[] result = File.ReadAllLines (firstTempPath);
			string [] expected = File.ReadAllLines (lzeUncompressedFilePath);

			Assert.AreEqual (expected, result);
		}

		[TestCase (CompressionAlgorithm.LZ4)]
		[TestCase (CompressionAlgorithm.LZ4Raw)]
		[TestCase (CompressionAlgorithm.Lzfse)]
		[TestCase (CompressionAlgorithm.Lzma)]
		[TestCase (CompressionAlgorithm.ZLib)]
		public void TestEncodeDecodeRoundTrip (CompressionAlgorithm algorithm)
		{
			using (var inputStream = File.OpenRead (lzeUncompressedFilePath))
			using (var outputStream = File.Create (firstTempPath))
			using (var stream = new CompressionStream (inputStream, outputStream, StreamOperation.Encode, algorithm)) {
				var diff = stream.Process ();
			}

			// we have the data in the tempPath, lets deflate and compare with the expected data
			using (var inputStream = File.OpenRead (firstTempPath))
			using (var outputStream = File.Create (secondTempPath))
			using (var stream = new CompressionStream (inputStream, outputStream, StreamOperation.Decode, algorithm)) {
				var diff = stream.Process ();
			}
			
			string[] result = File.ReadAllLines (secondTempPath);
			string [] expected = File.ReadAllLines (lzeUncompressedFilePath);
			Assert.AreEqual (expected, result);
		}
	}
}
