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

#if !WATCH

using Foundation;
using CoreFoundation;
using AudioToolbox;
using ObjCRuntime;
using System;

#nullable enable

namespace AVFoundation {
#if !TVOS
	public partial class AVAudioRecorder {
		AVAudioRecorder (NSUrl url, AudioSettings settings, out NSError error)
		{
			// We use this method because it allows us to out NSError but, as a side effect, it is possible for the handle to be null and we will need to check this manually (on the Create method).
			Handle = InitWithUrl (url, settings.Dictionary, out error);
		}

		AVAudioRecorder (NSUrl url, AVAudioFormat format, out NSError error)
		{
			// We use this method because it allows us to out NSError but, as a side effect, it is possible for the handle to be null and we will need to check this manually (on the Create method).
			Handle = InitWithUrl (url, format, out error);
		}

		public static AVAudioRecorder? Create (NSUrl url, AudioSettings settings, out NSError? error)
		{
			if (settings is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (settings));
			error = null;
			try {
				AVAudioRecorder r = new AVAudioRecorder (url, settings, out error);
				if (r.Handle == IntPtr.Zero)
					return null;

				return r;
			} catch {
				return null;
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public static AVAudioRecorder? Create (NSUrl url, AVAudioFormat? format, out NSError? error)
		{
			if (format is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (format));
			error = null;
			try {
				AVAudioRecorder r = new AVAudioRecorder (url, format, out error);
				if (r.Handle == IntPtr.Zero)
					return null;

				return r;
			} catch {
				return null;
			}
		}

		internal static AVAudioRecorder? ToUrl (NSUrl url, NSDictionary settings, out NSError? error)
		{
			return Create (url, new AudioSettings (settings), out error);
		}
	}
#endif // !TVOS
}

#endif // !WATCH
