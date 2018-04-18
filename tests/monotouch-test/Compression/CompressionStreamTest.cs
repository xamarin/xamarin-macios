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
		public void TestConstructorNullValues ()
		{
			using (var inputStream = File.OpenRead (lzeCompressedFilePath)) {
				Assert.Throws<ArgumentNullException> (() => new CompressionStream (null, inputStream, StreamOperation.Decode, CompressionAlgorithm.LZ4), "Null input stream");
				Assert.Throws<ArgumentNullException> (() => new CompressionStream (inputStream, null, StreamOperation.Decode, CompressionAlgorithm.LZ4), "Null ouput stream");
			}
		}
		
		[Test]
		public void TestConstructorWrongBufferSizes ()
		{
			using (var inputStream = File.OpenRead (lzeCompressedFilePath))
			using (var outputStream = File.Create (firstTempPath)) {
				Assert.Throws<ArgumentException> (() => new CompressionStream (inputStream, outputStream, StreamOperation.Decode, CompressionAlgorithm.Lzfse, 0, 512), "0 input size");
				Assert.Throws<ArgumentException> (() => new CompressionStream (inputStream, outputStream, StreamOperation.Decode, CompressionAlgorithm.Lzfse, -12, 512), "negative input size");
				Assert.Throws<ArgumentException> (() => new CompressionStream (inputStream, outputStream, StreamOperation.Decode, CompressionAlgorithm.Lzfse, 512, 0), "0 output size");
				Assert.Throws<ArgumentException> (() => new CompressionStream (inputStream, outputStream, StreamOperation.Decode, CompressionAlgorithm.Lzfse, 512, -20), "negative output size");
			}
		}

		[Test]
		public void TestProcessWrongFormat ()
		{
			using (var inputStream = File.OpenRead (lzeCompressedFilePath))
			using (var outputStream = File.Create (firstTempPath))
			using (var stream = new CompressionStream (inputStream, outputStream, StreamOperation.Decode, CompressionAlgorithm.LZ4)) {
				// it is async, we expect it to be aggregated
				Assert.Throws<AggregateException> (() => stream.Process ());
			}
		}
		
		
		[TestCase (512, 1024)]
		[TestCase (256, 512)]
		[TestCase (1024, 512)]
		[TestCase (512, 256)]
		public void TestDecodeRealFile (int inBufferSize, int outBufferSize)
		{

			using (var inputStream = File.OpenRead (lzeCompressedFilePath))
			using (var outputStream = File.Create (firstTempPath))
			using (var stream = new CompressionStream (inputStream, outputStream, StreamOperation.Decode, CompressionAlgorithm.Lzfse, inBufferSize, outBufferSize)) {
				stream.Process ();
			}
			// ensure that the data is the expected one
			string [] result = File.ReadAllLines (firstTempPath);
			string [] expected = File.ReadAllLines (lzeUncompressedFilePath);

			Assert.AreEqual (expected, result);
		}

#if MONOMAC
		[TestCase (CompressionAlgorithm.LZ4Raw)]
#endif
		[TestCase (CompressionAlgorithm.LZ4)]
		[TestCase (CompressionAlgorithm.Lzfse)]
		[TestCase (CompressionAlgorithm.Lzma)]
		[TestCase (CompressionAlgorithm.ZLib)]
		public void TestEncodeDecodeRoundTrip (CompressionAlgorithm algorithm)
		{
			using (var inputStream = File.OpenRead (lzeUncompressedFilePath))
			using (var outputStream = File.Create (firstTempPath))
			using (var stream = new CompressionStream (inputStream, outputStream, StreamOperation.Encode, algorithm)) {
				stream.Process ();
			}

			// we have the data in the tempPath, lets deflate and compare with the expected data
			using (var inputStream = File.OpenRead (firstTempPath))
			using (var outputStream = File.Create (secondTempPath))
			using (var stream = new CompressionStream (inputStream, outputStream, StreamOperation.Decode, algorithm)) {
				stream.Process ();
			}
			
			string [] result = File.ReadAllLines (secondTempPath);
			string [] expected = File.ReadAllLines (lzeUncompressedFilePath);
			Assert.AreEqual (expected, result);
		}
	}
}
