// 
// CTFont.cs: Implements the managed CTFont
//
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2011 - 2014 Xamarin Inc
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
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using Foundation;

using CGGlyph = System.UInt16;

namespace CoreText {

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontManager.h
	public enum CTFontManagerScope : uint {
		None = 0, Process = 1, User = 2, Session = 3
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontManager.h
	public enum CTFontManagerAutoActivation : uint {
		Default = 0, Disabled = 1, Enabled = 2,
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "It's now treated as 'Default'.")]
		PromptUser = 3,
	}

	public partial class CTFontManager {

#if MONOMAC
		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerIsSupportedFont (IntPtr url);

		[Deprecated (PlatformName.MacOSX, 10, 6)]
		[Unavailable (PlatformName.iOS)]
		public static bool IsFontSupported (NSUrl url)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			return CTFontManagerIsSupportedFont (url.Handle);
		}
#elif !XAMCORE_3_0
		[Obsolete ("API not available on iOS, it will always return false.")]
		[Deprecated (PlatformName.MacOSX, 10, 6)]
		[Unavailable (PlatformName.iOS)]
		public static bool IsFontSupported (NSUrl url)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			return false;
		}
#endif

		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerRegisterFontsForURL (IntPtr fontUrl, CTFontManagerScope scope, ref IntPtr error);
		public static NSError RegisterFontsForUrl (NSUrl fontUrl, CTFontManagerScope scope)
		{
			if (fontUrl == null)
				throw new ArgumentNullException ("fontUrl");
			
			IntPtr error = IntPtr.Zero;

			try {
				if (CTFontManagerRegisterFontsForURL (fontUrl.Handle, scope, ref error))
					return null;
				else
					return Runtime.GetNSObject<NSError> (error);
			} finally {
				if (error != IntPtr.Zero)
					NSObject.DangerousRelease (error);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerRegisterFontsForURLs(IntPtr arrayRef, CTFontManagerScope scope, ref IntPtr error_array);
		public static NSError [] RegisterFontsForUrl (NSUrl [] fontUrls, CTFontManagerScope scope)
		{
			if (fontUrls == null)
				throw new ArgumentNullException ("fontUrls");

			foreach (var furl in fontUrls)
				if (furl == null)
					throw new ArgumentException ("contains a null entry", "fontUrls");

			using (var arr = NSArray.FromNSObjects (fontUrls)) {
				IntPtr error_array = IntPtr.Zero;
				try {
					if (CTFontManagerRegisterFontsForURLs (arr.Handle, scope, ref error_array))
						return null;
					else
						return NSArray.ArrayFromHandle<NSError> (error_array);
				} finally {
					if (error_array != IntPtr.Zero)
						NSObject.DangerousRelease (error_array);
				}
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerUnregisterFontsForURL(IntPtr fotUrl, CTFontManagerScope scope, ref IntPtr error);
		public static NSError UnregisterFontsForUrl (NSUrl fontUrl, CTFontManagerScope scope)
		{
			if (fontUrl == null)
				throw new ArgumentNullException ("fontUrl");

			IntPtr error = IntPtr.Zero;

			try {
				if (CTFontManagerUnregisterFontsForURL (fontUrl.Handle, scope, ref error))
					return null;
				else
					return Runtime.GetNSObject<NSError> (error);
			} finally {
				if (error != IntPtr.Zero)
					NSObject.DangerousRelease (error);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerUnregisterFontsForURLs(IntPtr arrayRef, CTFontManagerScope scope, ref IntPtr error_array);
		public static NSError [] UnregisterFontsForUrl (NSUrl [] fontUrls, CTFontManagerScope scope)
		{
			if (fontUrls == null)
				throw new ArgumentNullException ("fontUrls");

			foreach (var furl in fontUrls)
				if (furl == null)
					throw new ArgumentException ("contains a null entry", "fontUrls");


			IntPtr error_array = IntPtr.Zero;
			using (var arr = NSArray.FromNSObjects (fontUrls)) {
				try {
					if (CTFontManagerUnregisterFontsForURLs (arr.Handle, scope, ref error_array))
						return null;
					else
						return NSArray.ArrayFromHandle<NSError> (error_array);
				} finally {
					if (error_array != IntPtr.Zero)
						NSObject.DangerousRelease (error_array);
				}
			}
		}

		[iOS (7,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef */ IntPtr CTFontManagerCreateFontDescriptorsFromURL (/* CFURLRef */ IntPtr fileURL);

		[iOS (7,0)]
		public static CTFontDescriptor[] GetFonts (NSUrl url)
		{
			if (url == null)
				throw new ArgumentNullException ("url");

			var arrayPtr = CTFontManagerCreateFontDescriptorsFromURL (url.Handle);
			if (arrayPtr == IntPtr.Zero)
				return new CTFontDescriptor [0];

			using (var unmanageFonts = new CFArray (arrayPtr, true)) {
				var managedFonts = new CTFontDescriptor [unmanageFonts.Count];
				for (int index = 0; index < unmanageFonts.Count; index++) {
					managedFonts [index] = new CTFontDescriptor (unmanageFonts.GetValue (index), false);
				}
				return managedFonts;
			}
		}

		[Mac (10,8)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerRegisterGraphicsFont (IntPtr cgfont, out IntPtr error);

		[Mac (10,8)]
		public static bool RegisterGraphicsFont (CGFont font, out NSError error)
		{
			if (font == null)
				throw new ArgumentNullException ("font");
			IntPtr h = IntPtr.Zero;
			bool ret;
			try {
				ret = CTFontManagerRegisterGraphicsFont (font.Handle, out h);
				if (ret)
					error = null;
				else 
					error = new NSError (h);
			} finally {
				if (h != IntPtr.Zero)
					NSObject.DangerousRelease (h);
			}
			return ret;
		}

		[Mac (10,8)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerUnregisterGraphicsFont (IntPtr cgfont, out IntPtr error);

		[Mac (10,8)]
		public static bool UnregisterGraphicsFont (CGFont font, out NSError error)
		{
			if (font == null)
				throw new ArgumentNullException ("font");
			IntPtr h = IntPtr.Zero;
			bool ret;
			try {
				ret = CTFontManagerUnregisterGraphicsFont (font.Handle, out h);
				if (ret)
					error = null;
				else 
					error = new NSError (h);
			} finally {
				if (h != IntPtr.Zero)
					NSObject.DangerousRelease (h);
			}
			return ret;
		}
		
		static CTFontManager ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreTextLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
#if !XAMCORE_3_0
				ErrorDomain  = Dlfcn.GetStringConstant (handle, "kCTFontManagerErrorDomain");
#endif
				ErrorFontUrlsKey  = Dlfcn.GetStringConstant (handle, "kCTFontManagerErrorFontURLsKey");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}

		static NSString _RegisteredFontsChangedNotification;

		[iOS (7,0)]
		static NSString RegisteredFontsChangedNotification {
			get {
				if (_RegisteredFontsChangedNotification == null){
					var handle = Dlfcn.dlopen (Constants.CoreTextLibrary, 0);
					_RegisteredFontsChangedNotification = Dlfcn.GetStringConstant (handle, "kCTFontManagerRegisteredFontsChangedNotification");
					Dlfcn.dlclose (handle);
				}
				return _RegisteredFontsChangedNotification;
			}
		}

#if !XAMCORE_3_0
		public readonly static NSString ErrorDomain;
#endif
		public readonly static NSString ErrorFontUrlsKey;

		public static partial class Notifications {
			public static NSObject ObserveRegisteredFontsChanged (EventHandler<NSNotificationEventArgs> handler)
			{
				return NSNotificationCenter.DefaultCenter.AddObserver (RegisteredFontsChangedNotification, notification => handler (null, new NSNotificationEventArgs (notification)));
			}
			
		}
	}
}
