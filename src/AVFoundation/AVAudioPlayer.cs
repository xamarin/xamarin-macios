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
	public partial class AVAudioPlayer {
		/// <summary>Create a new <see cref="AVAudioPlayer" /> from the specified url and hint for the file type.</summary>
		/// <param name="url">The url of a local audio file.</param>
		/// <param name="fileTypeHint">The uniform type identifier for the audio format.</param>
		/// <param name="error">An object describing the error in case an error occurs, null otherwise.</param>
		/// <returns>A new <see cref="AVAudioPlayer" /> instance if successful, null otherwise.</returns>
		public static AVAudioPlayer? FromUrl (NSUrl url, NSString? fileTypeHint, out NSError? error)
		{
			var rv = new AVAudioPlayer (NSObjectFlag.Empty);
			rv.InitializeHandle (rv._InitWithContentsOfUrl (url, fileTypeHint, out error), string.Empty, false);
			if (rv.Handle == IntPtr.Zero) {
				rv.Dispose ();
				return null;
			}
			return rv;
		}

		/// <summary>Create a new <see cref="AVAudioPlayer" /> from the specified url and hint for the file type.</summary>
		/// <param name="url">The url of a local audio file.</param>
		/// <param name="fileTypeHint">The uniform type identifier for the audio format.</param>
		/// <param name="error">An object describing the error in case an error occurs, null otherwise.</param>
		/// <returns>A new <see cref="AVAudioPlayer" /> instance if successful, null otherwise.</returns>
		public static AVAudioPlayer? FromUrl (NSUrl url, AVFileTypes fileTypeHint, out NSError? error)
		{
			return FromUrl (url, fileTypeHint.GetConstant (), out error);
		}

		/// <summary>Create a new <see cref="AVAudioPlayer" /> from the specified url.</summary>
		/// <param name="url">The url of a local audio file.</param>
		/// <param name="error">An object describing the error in case an error occurs, null otherwise.</param>
		/// <returns>A new <see cref="AVAudioPlayer" /> instance if successful, null otherwise.</returns>
		public static AVAudioPlayer? FromUrl (NSUrl url, out NSError? error)
		{
			var rv = new AVAudioPlayer (NSObjectFlag.Empty);
			rv.InitializeHandle (rv._InitWithContentsOfUrl (url, out error), string.Empty, false);
			if (rv.Handle == IntPtr.Zero) {
				rv.Dispose ();
				return null;
			}
			return rv;
		}

		/// <summary>Create a new <see cref="AVAudioPlayer" /> from the specified url.</summary>
		/// <param name="url">The url of a local audio file.</param>
		/// <returns>A new <see cref="AVAudioPlayer" /> instance if successful, null otherwise.</returns>
		public static AVAudioPlayer? FromUrl (NSUrl url)
		{
			return FromUrl (url, out _);
		}

		/// <summary>Create a new <see cref="AVAudioPlayer" /> from the specified data and hint for the file type.</summary>
		/// <param name="data">The audio data to play.</param>
		/// <param name="fileTypeHint">The uniform type identifier for the audio format.</param>
		/// <param name="error">An object describing the error in case an error occurs, null otherwise.</param>
		/// <returns>A new <see cref="AVAudioPlayer" /> instance if successful, null otherwise.</returns>
		public static AVAudioPlayer? FromData (NSData data, AVFileTypes fileTypeHint, out NSError? error)
		{
			return FromData (data, fileTypeHint.GetConstant (), out error);
		}

		/// <summary>Create a new <see cref="AVAudioPlayer" /> from the specified data and hint for the file type.</summary>
		/// <param name="data">The audio data to play.</param>
		/// <param name="fileTypeHint">The uniform type identifier for the audio format.</param>
		/// <param name="error">An object describing the error in case an error occurs, null otherwise.</param>
		/// <returns>A new <see cref="AVAudioPlayer" /> instance if successful, null otherwise.</returns>
		public static AVAudioPlayer? FromData (NSData data, NSString? fileTypeHint, out NSError? error)
		{
			var rv = new AVAudioPlayer (NSObjectFlag.Empty);
			rv.InitializeHandle (rv._InitWithData (data, fileTypeHint, out error), string.Empty, false);
			if (rv.Handle == IntPtr.Zero) {
				rv.Dispose ();
				return null;
			}
			return rv;
		}

		/// <summary>Create a new <see cref="AVAudioPlayer" /> from the specified data.</summary>
		/// <param name="data">The audio data to play.</param>
		/// <param name="error">An object describing the error in case an error occurs, null otherwise.</param>
		/// <returns>A new <see cref="AVAudioPlayer" /> instance if successful, null otherwise.</returns>
		public static AVAudioPlayer? FromData (NSData data, out NSError? error)
		{
			var rv = new AVAudioPlayer (NSObjectFlag.Empty);
			rv.InitializeHandle (rv._InitWithData (data, out error), string.Empty, false);
			if (rv.Handle == IntPtr.Zero) {
				rv.Dispose ();
				return null;
			}
			return rv;
		}

		/// <summary>Create a new <see cref="AVAudioPlayer" /> from the specified data.</summary>
		/// <param name="data">The audio data to play.</param>
		/// <returns>A new <see cref="AVAudioPlayer" /> instance if successful, null otherwise.</returns>
		public static AVAudioPlayer? FromData (NSData data)
		{
			return FromData (data, out _);
		}
	}
}
