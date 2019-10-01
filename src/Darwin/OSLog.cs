//
// OSLog.cs: Bindings to the os_log(3) API from Apple
//
// Authors:
//  William Kent
//
// Copyright 2019, Xamarin Inc.
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

using System;
using ObjCRuntime;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Darwin {
	public class OSLog : IDisposable, INativeObject {
		static OSLog _default;

		public static OSLog Default {
			get {
				if (_default == null)
					_default = new OSLog (IntPtr.Zero);
				return _default;
			}
		}

		bool disposed;
		IntPtr handle;
		public IntPtr Handle { get { return handle; } }

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~OSLog ()
		{
			Dispose (false);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!disposed) {
				if (handle != IntPtr.Zero)
					os_release (handle);

				disposed = true;
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.SystemLibrary)]
		extern static IntPtr os_log_create (string subsystem, string category);

		[DllImport (Constants.SystemLibrary)]
		extern static void os_release (IntPtr handle);

		[DllImport ("__Internal")]
		extern static void xamarin_os_log (IntPtr logHandle, OSLogLevel level, string message);

		OSLog (IntPtr handle)
		{
			this.handle = handle;
		}

		public OSLog (string subsystem, string category)
		{
			if (subsystem == null)
				throw new ArgumentNullException (nameof (subsystem));
			if (category == null)
				throw new ArgumentNullException (nameof (category));

			handle = os_log_create (subsystem, category);
		}

		public void Log (string message)
		{
			if (message == null)
				throw new ArgumentNullException (nameof (message));

			xamarin_os_log (handle, OSLogLevel.Default, message);
		}

		public void Log (OSLogLevel level, string message)
		{
			if (message == null)
				throw new ArgumentNullException (nameof (message));

			xamarin_os_log (handle, level, message);
		}
	}

	public enum OSLogLevel
	{
		// These values must match the os_log_type_t enum in <os/log.h>.
		Default = 0x00,
		Info    = 0x01,
		Debug   = 0x02,
		Error   = 0x10,
		Fault   = 0x11
	}
}
