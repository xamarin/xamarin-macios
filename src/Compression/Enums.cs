using System;

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using CoreVideo;

namespace Compression {

	// this enum as per the headers is an int NOT an NSInterger
	[iOS (9,0), TV (9,0), Mac (10,11)]
	public enum CompressionAlgorithm {
		LZ4 = 0x100,
		LZ4Raw = 0x101,
		Lzfse = 0x801,
		Lzma = 0x306,
		Zlib = 0x205,
	}

	enum CompressionStatus {
		Ok = 0,
		End = 1,
		Error = -1,
	}

	enum StreamFlag {
		Continue = 0, // not present in the API, but makes it nice in our case
		Finalize = 0x0001,
	}

	enum StreamOperation {
		Encode = 0,
		Decode = 1,
	}
}
