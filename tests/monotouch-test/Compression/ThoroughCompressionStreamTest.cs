using System.IO;
using System.IO.Compression;

using Foundation;
using Compression;

using NUnit.Framework;


namespace MonoTouchFixtures.Compression {

	// Special test case that tests the compression class against files that
	// have been compressed by apple tools. This ensures that we can read
	// the data from a file created by an outside tool.
	// The tests have been moves outside the CompressionStreamTest because
	// those tests are exactly the same ones as the DeflateStream ones while
	// this are just for the CompressionStream implementation.

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ThoroughCompressionStreamTest {

		string uncompressedFilePath = Path.Combine (NSBundle.MainBundle.ResourcePath, "uncompressed.txt");

		string firstTempPath;
		string secondTempPath;

		private static void CopyStream (Stream src, Stream dest)
		{
			byte [] array = new byte [1024];
			int bytes_read;
			bytes_read = src.Read (array, 0, 1024);
			while (bytes_read != 0) {
				dest.Write (array, 0, bytes_read);
				bytes_read = src.Read (array, 0, 1024);
			}
		}

		private static bool compare_buffers (byte [] first, byte [] second, int length)
		{
			if (first.Length < length || second.Length < length) {
				return false;
			}
			for (int i = 0; i < length; i++) {
				if (first [i] != second [i]) {
					return false;
				}
			}
			return true;
		}

		[SetUp]
		public void SetUp ()
		{
			firstTempPath = Path.GetTempFileName ();
			secondTempPath = Path.GetTempFileName ();
		}

		void DecodeRealFile (CompressionAlgorithm algorithm, string compressedFile, string uncompressedFile)
		{
			var dataStream = File.OpenRead (compressedFile);
			var backing = File.Create (firstTempPath);
			backing.Seek (0, SeekOrigin.Begin);
			CompressionStream decompressing = new CompressionStream (dataStream, CompressionMode.Decompress, algorithm, true);
			MemoryStream output = new MemoryStream ();
			CopyStream (decompressing, output);
			output.Seek (0, SeekOrigin.Begin);
			StreamReader reader = new StreamReader (output);
			output.Seek (0, SeekOrigin.Begin);
			Assert.AreNotEqual (0, output.Length, "Stream length should not be 0,");
			Assert.IsTrue (compare_buffers (File.ReadAllBytes (uncompressedFile), output.GetBuffer (), (int) output.Length), "Streams are not equal.");
			decompressing.Close ();
			output.Close ();
		}

		[TestCase (CompressionAlgorithm.LZ4, "compressed_lz4")]
		[TestCase (CompressionAlgorithm.Lzfse, "compressed_lze")]
		[TestCase (CompressionAlgorithm.Lzma, "compressed_lzma")]
		[TestCase (CompressionAlgorithm.Zlib, "compressed_zip")]
		public void TestDecodeRealFile (CompressionAlgorithm algorithm, string compressedFile)
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS 9.0+ or macOS 10.11+");
			string compressedFilePath = Path.Combine (NSBundle.MainBundle.ResourcePath, compressedFile);
			DecodeRealFile (algorithm, compressedFilePath, uncompressedFilePath);
		}
	}
}
