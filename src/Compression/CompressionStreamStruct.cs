using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Compression
{

		[iOS (9,0), Mac (10,11)]
		[StructLayout(LayoutKind.Sequential)]
		struct CompressionStreamStruct {
			public IntPtr Destination; // uint8_t * dst_ptr
			public nint DestinationSize; // size_t dst_size;
			public IntPtr Source; // const uint8_t * src_ptr;
			public nint SourceSize; // size_t src_size;
			public IntPtr State; // void * __nullable state;


			[DllImport (Constants.libcompression)]
			public static extern CompressionStatus compression_stream_init (ref CompressionStreamStruct stream, StreamOperation operation, CompressionAlgorithm algorithm);

			[DllImport (Constants.libcompression)]
			public static extern CompressionStatus compression_stream_process (ref CompressionStreamStruct stream, StreamFlag flags);

			[DllImport (Constants.libcompression)]
			public static extern CompressionStatus compression_stream_destroy (ref CompressionStreamStruct stream);
		}
}
