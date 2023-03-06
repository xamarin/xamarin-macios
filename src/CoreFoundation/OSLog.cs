//
// OSLog.cs: Bindings to the os_log(3) API from Apple
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#nullable enable

using System;

using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public sealed class OSLog : NativeObject {

		static OSLog? _default;

		public static OSLog Default {
			get {
				if (_default is null) {
					var h = Dlfcn.dlsym (Libraries.System.Handle, "_os_log_default");
					if (h == IntPtr.Zero)
						throw new NotSupportedException ("Feature not available on this OS version");
					_default = new OSLog (h, false);
				}
				return _default;
			}
		}

		protected internal override void Retain ()
		{
			if (Handle != IntPtr.Zero)
				os_retain (Handle);
		}

		protected internal override void Release ()
		{
			if (Handle != IntPtr.Zero)
				os_release (Handle);
		}

#if NET
		[DllImport (Constants.libSystemLibrary)]
		extern static IntPtr os_log_create (IntPtr subsystem, IntPtr category);
#else
		[DllImport (Constants.libSystemLibrary)]
		extern static IntPtr os_log_create (string subsystem, string category);
#endif

		[DllImport (Constants.libSystemLibrary)]
		extern static IntPtr os_retain (IntPtr handle);

		[DllImport (Constants.libSystemLibrary)]
		extern static void os_release (IntPtr handle);

		[DllImport ("__Internal")]
		extern static void xamarin_os_log (IntPtr logHandle, OSLogLevel level, IntPtr message);

		[Preserve (Conditional = true)]
		internal OSLog (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public OSLog (string subsystem, string category)
		{
			if (subsystem is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (subsystem));
			if (category is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (category));
#if NET
			using var subsystemPtr = new TransientString (subsystem);
			using var categoryPtr = new TransientString (category);
			Handle = os_log_create (subsystemPtr, categoryPtr);
#else
			Handle = os_log_create (subsystem, category);
#endif
		}

		public void Log (string message)
		{
			Log (OSLogLevel.Default, message);
		}

		public void Log (OSLogLevel level, string message)
		{
			if (message is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (message));

			using var messagePtr = new TransientString (message);
			xamarin_os_log (Handle, level, messagePtr);
		}
	}
}
