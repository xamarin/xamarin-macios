using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Metal {

#if NET
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst16.0")]
	[SupportedOSPlatform ("macos13.0")]
	[SupportedOSPlatform ("tvos16.0")]
#else
	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#endif
	public class MTLIOCompressionContext : DisposableObject {

		[Preserve (Conditional = true)]
		MTLIOCompressionContext (NativeHandle handle, bool owns) : base (handle, owns) { }

		[DllImport (Constants.MetalLibrary)]
		static extern unsafe void MTLIOCompressionContextAppendData (void* context, void* data, nuint size);

		unsafe void AppendData (void* data, nuint size)
			=> MTLIOCompressionContextAppendData ((void*) GetCheckedHandle (), data, size);

		public void AppendData (byte [] data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			AppendData (new ReadOnlySpan<byte> (data, 0, data.Length));
		}

		public void AppendData (ReadOnlySpan<byte> data)
		{
			unsafe {
				// Pass new bytes through deflater and write them too:
				fixed (void* bufferPtr = &MemoryMarshal.GetReference (data)) {
					AppendData (bufferPtr, (nuint) data.Length);
				}
			}
		}

		public void AppendData (NSData data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			unsafe {
				AppendData ((void*) data.Bytes, data.Length);
			}
		}

		[DllImport (Constants.MetalLibrary)]
		// [return: NullAllowed]
		static extern IntPtr MTLIOCreateCompressionContext (IntPtr path, long type, long chunkSize);

		public static MTLIOCompressionContext? Create (string path, MTLIOCompressionMethod type, long chunkSize)
		{
			if (path is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (path));

			using var pathPtr = new TransientString (path);
			var handle = MTLIOCreateCompressionContext (pathPtr, (long) type, chunkSize);
			if (handle == NativeHandle.Zero) {
				return null;
			}
			return new MTLIOCompressionContext (handle, owns: true);
		}

		protected override void Dispose (bool disposing)
		{
			// only call the parent if the user did not call FlushAndDestroy
			if (disposing && Handle != NativeHandle.Zero && Owns) {
				FlushAndDestroy ();
			}
			base.Dispose (false);
		}

		[DllImport (Constants.MetalLibrary)]
		static extern long MTLIOFlushAndDestroyCompressionContext (IntPtr context);

		public MTLIOCompressionStatus FlushAndDestroy ()
		{
			var result = (MTLIOCompressionStatus) MTLIOFlushAndDestroyCompressionContext (GetCheckedHandle ());

			ClearHandle ();
			return result;
		}

		[DllImport (Constants.MetalLibrary)]
		static extern nint MTLIOCompressionContextDefaultChunkSize ();

		public static nint DefaultChunkSize => MTLIOCompressionContextDefaultChunkSize ();

	}
}
