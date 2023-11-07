// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2014 Xamarin Inc
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
//
using Foundation;
using ObjCRuntime;
using System;
using System.Runtime.InteropServices;

#nullable enable

namespace AVFoundation {
#if !WATCH
	public partial class AVAudioPlayer {

		[DllImport (Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
		unsafe static extern IntPtr objc_msgSend (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr* arg2);

		public static AVAudioPlayer? FromUrl (NSUrl url, out NSError? error)
		{
			error = null;
			IntPtr url__handle__ = url!.GetNonNullHandle (nameof (url));
			IntPtr handle = Messaging.IntPtr_objc_msgSend (class_ptr, Selector.GetHandle ("alloc"));
			IntPtr errorptr = IntPtr.Zero;
			if (handle == IntPtr.Zero)
				return null;
			unsafe {
				handle = objc_msgSend (handle, Selector.GetHandle ("initWithContentsOfURL:error:"), url__handle__, &errorptr);
			}
			error = Runtime.GetNSObject<NSError> (errorptr);
			return Runtime.GetNSObject<AVAudioPlayer> (handle, owns: true);
		}

		public static AVAudioPlayer? FromUrl (NSUrl url)
		{
			return FromUrl (url, out _);
		}

		public static AVAudioPlayer? FromData (NSData data, out NSError? error)
		{
			error = null;
			IntPtr data__handle__ = data!.GetNonNullHandle (nameof (data));
			IntPtr errorptr = IntPtr.Zero;
			IntPtr handle = Messaging.IntPtr_objc_msgSend (class_ptr, Selector.GetHandle ("alloc"));

			if (handle == IntPtr.Zero)
				return null;
			unsafe {
				handle = objc_msgSend (handle, Selector.GetHandle ("initWithData:error:"), data__handle__, &errorptr);
			}
			error = Runtime.GetNSObject<NSError> (errorptr);
			return Runtime.GetNSObject<AVAudioPlayer> (handle, owns: true);
		}

		public static AVAudioPlayer? FromData (NSData data)
		{
			return FromData (data, out _);
		}
	}
#endif // !WATCH
}
