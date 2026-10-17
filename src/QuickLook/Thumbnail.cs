//
// MacOS Quicklook Thumbnail support
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012, Xamarin Inc
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

#nullable enable

#if MONOMAC

using CoreFoundation;
using CoreGraphics;

namespace QuickLook {
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	public static partial class QLThumbnailImage {

		// QuickLook.framework/Versions/A/Headers/QLThumbnailImage.h
		// note: documented as Thread-Safe
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos15.0", "Use the QuickLookThumbnailing framework instead for creating thumbnails for files.")]
		[DllImport (Constants.QuickLookLibrary)]
		extern static /* CGImageRef */ IntPtr QLThumbnailImageCreate (/* CFAllocatorRef */ IntPtr allocator, /* CFUrlRef */ IntPtr url, CGSize maxThumbnailSize, /* CFDictionaryRef */ IntPtr options);

		/// <param name="url">To be added.</param>
		///         <param name="maxThumbnailSize">To be added.</param>
		///         <param name="scaleFactor">To be added.</param>
		///         <param name="iconMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos15.0", "Use the QuickLookThumbnailing framework instead for creating thumbnails for files.")]
		public static CGImage? Create (NSUrl url, CGSize maxThumbnailSize, float scaleFactor = 1, bool iconMode = false)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			NSMutableDictionary? dictionary = null;

			if (scaleFactor != 1 && iconMode != false) {
				dictionary = new NSMutableDictionary ();
				var scaleFactorKey = OptionScaleFactorKey;
				dictionary.LowlevelSetObject ((NSNumber) scaleFactor, scaleFactorKey);
				var iconModeKey = OptionIconModeKey;
				dictionary.LowlevelSetObject (iconMode ? CFBoolean.TrueHandle : CFBoolean.FalseHandle, iconModeKey);
			}

			var handle = QLThumbnailImageCreate (IntPtr.Zero, url.Handle, maxThumbnailSize, dictionary.GetHandle ());
			GC.KeepAlive (url);
			GC.KeepAlive (dictionary);
			if (handle != IntPtr.Zero)
				return new CGImage (handle, true);

			return null;
		}
	}
}
#endif
