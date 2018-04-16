using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using CoreVideo;

namespace Compression {

	public enum CompressionAlgorithm {
		LZ4 = 0x100,
		LZ4Raw = 0x101,
		LZFSE = 0x801,
		LZMA = 0x306,
		ZLIB = 0x205,
	}

	public enum CompressionStatus {
		Ok = 0,
		End = 1,
		Error = -1,
	}

	public enum StreamFlag {
		Finalize = 0x0001,
	}

	public enum StreamOperation {
		Encode = 0,
		Decode = 1,
	}
}