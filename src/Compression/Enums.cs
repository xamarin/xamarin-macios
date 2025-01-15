using System;

using ObjCRuntime;
#nullable enable

namespace Compression {

	// this enum as per the headers is an int NOT an NSInteger
	/// <summary>Specifies the algorithm to use to compress or decompress the underlying stream.</summary>
	public enum CompressionAlgorithm {
		/// <summary>The LZ4 compression format.</summary>
		LZ4 = 0x100,
		/// <summary>The <see cref="LZ4">LZ4</see> compression format, without block headers.</summary>
		LZ4Raw = 0x101,
		/// <summary>The Lempel–Ziv Finite State Entropy (LZFSE) compression format.</summary>
		/// <remarks>This is the format Apple recommends on Apple platforms.</remarks>
		Lzfse = 0x801,
		/// <summary>The Lempel–Ziv–Markov (LZMA) compression format.</summary>
		/// <remarks>The encoder in the Compression framework only supports level 6. The decoder supports all compression levels.</remarks>
		Lzma = 0x306,
		/// <summary>The zlib compression format. This is the compression format most compatible with other platforms.</summary>
		/// <remarks>The encoder in the Compression framework only supports level 5. The decoder supports all compression levels.</remarks>
		Zlib = 0x205,
		/// <summary>The Brotli compression format.</summary>
		/// <remarks>The encoder in the Compression framework only supports the Brotli level 2 encoder. The decoder supports all compression levels.</remarks>
		Brotli = 0xB02,
		/// <summary>The LZBitmap compression format.</summary>
		/// <remarks>This compression algorithm is only available on Apple devices.</remarks>
#if NET
		[iOS (15, 0), Mac (12, 0), TV (15, 0), MacCatalyst (15, 0)]
#else
		[iOS (15, 0), Mac (12, 0), TV (15, 0)]
#endif
		LZBitmap = 0x702,
	}

	[Internal]
	enum CompressionStatus {
		Ok = 0,
		End = 1,
		Error = -1,
	}

	[Internal]
	enum StreamFlag {
		Continue = 0, // not present in the API, but makes it nice in our case
		Finalize = 0x0001,
	}

	[Internal]
	enum StreamOperation {
		Encode = 0,
		Decode = 1,
	}
}
