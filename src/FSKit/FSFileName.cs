#if NET
using System;

using ObjCRuntime;

#nullable enable

namespace FSKit {
	public partial class FSFileName {
		[DesignatedInitializer]
		public FSFileName (byte [] bytes)
		{
			if (bytes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bytes));

			unsafe {
				fixed (byte* bytesPtr = bytes) {
					InitializeHandle (InitWithBytes ((IntPtr) bytesPtr, (nuint) bytes.Length));
				}
			}
		}

		public FSFileName Create (byte [] bytes)
		{
			if (bytes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bytes));

			unsafe {
				fixed (byte* bytesPtr = bytes) {
					return _Create ((IntPtr) bytesPtr, (nuint) bytes.Length);
				}
			}
		}
	}
}
#endif // NET
