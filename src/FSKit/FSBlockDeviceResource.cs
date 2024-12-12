#if NET
using Foundation;

#nullable enable

namespace FSKit {
	public partial class FSBlockDeviceResource {
		public unsafe void Read (byte [] buffer, long offset, nuint length, FSBlockDeviceResourceReadReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				Read ((IntPtr) bufferPtr, offset, length, reply);
			}
		}

		public unsafe void SynchronousRead (byte [] buffer, long offset, nuint length, FSBlockDeviceResourceReadReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				SynchronousRead ((IntPtr) bufferPtr, offset, length, reply);
			}
		}

		public unsafe void Write (byte [] buffer, long offset, nuint length, FSBlockDeviceResourceWriteReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				Write ((IntPtr) bufferPtr, offset, length, reply);
			}
		}

		public unsafe void SynchronousWrite (byte [] buffer, long offset, nuint length, FSBlockDeviceResourceWriteReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				SynchronousWrite ((IntPtr) bufferPtr, offset, length, reply);
			}
		}

		public unsafe void SynchronousMetadataRead (byte [] buffer, long offset, nuint length, FSBlockDeviceResourceMetadataReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				SynchronousMetadataRead ((IntPtr) bufferPtr, offset, length, reply);
			}
		}

		public unsafe void SynchronousMetadataRead (byte [] buffer, long offset, nuint length, FSMetadataReadahead[] readAheadExtents, FSBlockDeviceResourceMetadataReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				fixed (FSMetadataReadahead* readAheadExtentsPtr = readAheadExtents) {
					SynchronousMetadataRead ((IntPtr) bufferPtr, offset, length, (IntPtr) readAheadExtentsPtr, readAheadExtents.Length, reply);
				}
			}
		}

		public unsafe void MetadataWrite (byte [] buffer, long offset, nuint length, FSBlockDeviceResourceMetadataReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				MetadataWrite ((IntPtr) bufferPtr, offset, length, reply);
			}
		}

		public unsafe void SynchronousMetadataWrite (byte [] buffer, long offset, nuint length, FSBlockDeviceResourceMetadataReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				SynchronousMetadataWrite ((IntPtr) bufferPtr, offset, length, reply);
			}
		}

		public unsafe void DelayedMetadataWriteFrom (byte [] buffer, long offset, nuint length, FSBlockDeviceResourceMetadataReplyHandler reply)
		{
			fixed (byte* bufferPtr = buffer) {
				DelayedMetadataWriteFrom ((IntPtr) bufferPtr, offset, length, reply);
			}
		}
	}
}
#endif // NET
