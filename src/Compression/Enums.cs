using System;

using ObjCRuntime;
#nullable enable

namespace Compression {

	// this enum as per the headers is an int NOT an NSInteger
	public enum CompressionAlgorithm {
		LZ4 = 0x100,
		LZ4Raw = 0x101,
		Lzfse = 0x801,
		Lzma = 0x306,
		Zlib = 0x205,
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
