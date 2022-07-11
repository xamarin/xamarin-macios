// 
// CTFont.cs: Implements the managed CTFont
//
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2011 - 2014 Xamarin Inc
// Copyright 2019 Microsoft Corporation
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
		None = 0,
		Process = 1,
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
#endif
		Persistent = 2,
#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoiOS]
		[NoTV]
		[NoWatch]
#endif
		Session = 3,
#if !NET
		[NoiOS][NoTV][NoWatch] // historically not available under the old name
		User = Persistent,
#endif
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontManager.h
	public enum CTFontManagerAutoActivation : uint {
		Default = 0, Disabled = 1, Enabled = 2,
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.13")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13 it's now treated as 'Default'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "It's now treated as 'Default'.")]
#endif
		PromptUser = 3,
	}

	public partial class CTFontManager {

#if MONOMAC
		[DllImport (Constants.CoreTextLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CTFontManagerIsSupportedFont (IntPtr url);

#if NET
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("macos10.6")]
#if MONOMAC
		[Obsolete ("Starting with macos10.6.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10, 6)]
		[Unavailable (PlatformName.iOS)]
#endif
		public static bool IsFontSupported (NSUrl url)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			return CTFontManagerIsSupportedFont (url.Handle);
		}
#elif !XAMCORE_3_0
#if NET
		[UnsupportedOSPlatform ("macos10.6")]
#if MONOMAC
		[Obsolete ("Starting with macos10.6.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("ios")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 6)]
		[Unavailable (PlatformName.iOS)]
#endif
		[Obsolete ("API not available on iOS, it will always return false.")]
		public static bool IsFontSupported (NSUrl url)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			return false;
		}
#endif

		[DllImport (Constants.CoreTextLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CTFontManagerRegisterFontsForURL (IntPtr fontUrl, CTFontManagerScope scope, ref IntPtr error);
		public static NSError? RegisterFontsForUrl (NSUrl fontUrl, CTFontManagerScope scope)
		{
			if (fontUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (fontUrl));
			
			IntPtr error = IntPtr.Zero;

			try {
				if (CTFontManagerRegisterFontsForURL (fontUrl.Handle, scope, ref error))
					return null;
				else
					return Runtime.GetNSObject<NSError> (error);
			} finally {
				if (error != IntPtr.Zero)
					CFObject.CFRelease (error);
			}
		}

		static NSArray EnsureNonNullArray (object [] items, string name)
		{
			if (items is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));

			foreach (var i in items)
				if (i is null)
					throw new ArgumentException ("Array contains a null entry", name);

			return NSArray.FromObjects (items);
		}

		static T[]? ArrayFromHandle<T> (IntPtr handle, bool releaseAfterUse) where T : class, INativeObject
		{
			if (handle == IntPtr.Zero)
				return null;
			try {
				return NSArray.ArrayFromHandle<T> (handle);
			}
			finally {
				if (releaseAfterUse)
					CFObject.CFRetain (handle);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15)]
		[Deprecated (PlatformName.iOS, 13,0)]
		[Deprecated (PlatformName.WatchOS, 6,0)]
		[Deprecated (PlatformName.TvOS, 13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CTFontManagerRegisterFontsForURLs(IntPtr arrayRef, CTFontManagerScope scope, ref IntPtr error_array);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'RegisterFonts' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'RegisterFonts' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'RegisterFonts' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'RegisterFonts' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'RegisterFonts' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'RegisterFonts' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'RegisterFonts' instead.")]
#endif
		public static NSError []? RegisterFontsForUrl (NSUrl [] fontUrls, CTFontManagerScope scope)
		{
			using (var arr = EnsureNonNullArray (fontUrls, nameof (fontUrls))) {
				IntPtr error_array = IntPtr.Zero;
				if (CTFontManagerRegisterFontsForURLs (arr.Handle, scope, ref error_array))
					return null;
				return ArrayFromHandle<NSError> (error_array, releaseAfterUse: true);
			}
		}

#if NET
		// [SupportedOSPlatform ("tvos13.0")] - Not valid on delegate declaration
		// [SupportedOSPlatform ("macos10.15")]
		// [SupportedOSPlatform ("ios13.0")]
		// [SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public delegate bool CTFontRegistrationHandler (NSError[] errors, bool done);

		internal delegate bool InnerRegistrationHandler (IntPtr block, IntPtr errors, bool done);
		static readonly InnerRegistrationHandler callback = TrampolineRegistrationHandler;

		[MonoPInvokeCallback (typeof (InnerRegistrationHandler))]
		static unsafe bool TrampolineRegistrationHandler (IntPtr block, /* NSArray */ IntPtr errors, bool done)
		{
			var del = BlockLiteral.GetTarget<CTFontRegistrationHandler> (block);
			return del is not null ? del (NSArray.ArrayFromHandle<NSError> (errors), done) : true;
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTFontManagerRegisterFontURLs (/* CFArrayRef */ IntPtr fontUrls, CTFontManagerScope scope, [MarshalAs (UnmanagedType.I1)] bool enabled, IntPtr registrationHandler);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTFontManagerRegisterFontURLs (/* CFArrayRef */ IntPtr fontUrls, CTFontManagerScope scope, [MarshalAs (UnmanagedType.I1)] bool enabled, ref BlockLiteral registrationHandler);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RegisterFonts (NSUrl [] fontUrls, CTFontManagerScope scope, bool enabled, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (fontUrls, nameof (fontUrls))) {
				if (registrationHandler is null) {
					CTFontManagerRegisterFontURLs (arr.Handle, scope, enabled, IntPtr.Zero);
				} else {
					BlockLiteral block_handler = new BlockLiteral ();
					block_handler.SetupBlockUnsafe (callback, registrationHandler);
					CTFontManagerRegisterFontURLs (arr.Handle, scope, enabled, ref block_handler);
					block_handler.CleanupBlock ();
				}
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CTFontManagerUnregisterFontsForURL(IntPtr fotUrl, CTFontManagerScope scope, ref IntPtr error);

		public static NSError? UnregisterFontsForUrl (NSUrl fontUrl, CTFontManagerScope scope)
		{
			if (fontUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (fontUrl));

			IntPtr error = IntPtr.Zero;

			try {
				if (CTFontManagerUnregisterFontsForURL (fontUrl.Handle, scope, ref error))
					return null;
				else
					return Runtime.GetNSObject<NSError> (error);
			} finally {
				if (error != IntPtr.Zero)
					CFObject.CFRelease (error);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15)]
		[Deprecated (PlatformName.iOS, 13,0)]
		[Deprecated (PlatformName.WatchOS, 6,0)]
		[Deprecated (PlatformName.TvOS, 13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CTFontManagerUnregisterFontsForURLs(IntPtr arrayRef, CTFontManagerScope scope, ref IntPtr error_array);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'UnregisterFonts' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'UnregisterFonts' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'UnregisterFonts' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message : "Use 'UnregisterFonts' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message : "Use 'UnregisterFonts' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message : "Use 'UnregisterFonts' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message : "Use 'UnregisterFonts' instead.")]
#endif
		public static NSError []? UnregisterFontsForUrl (NSUrl [] fontUrls, CTFontManagerScope scope)
		{
			IntPtr error_array = IntPtr.Zero;
			using (var arr = EnsureNonNullArray (fontUrls, nameof (fontUrls))) {
				if (CTFontManagerUnregisterFontsForURLs (arr.Handle, scope, ref error_array))
					return null;
				return ArrayFromHandle<NSError> (error_array, releaseAfterUse: true);
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerUnregisterFontURLs (/* CFArrayRef */ IntPtr fontUrls, CTFontManagerScope scope, IntPtr registrationHandler);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerUnregisterFontURLs (/* CFArrayRef */ IntPtr fontUrls, CTFontManagerScope scope, ref BlockLiteral registrationHandler);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void UnregisterFonts (NSUrl [] fontUrls, CTFontManagerScope scope, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (fontUrls, nameof (fontUrls))) {
				if (registrationHandler is null) {
					CTFontManagerUnregisterFontURLs (arr.Handle, scope, IntPtr.Zero);
				} else {
					BlockLiteral block_handler = new BlockLiteral ();
					block_handler.SetupBlockUnsafe (callback, registrationHandler);
					CTFontManagerUnregisterFontURLs (arr.Handle, scope, ref block_handler);
					block_handler.CleanupBlock ();
				}
			}
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (7,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef */ IntPtr CTFontManagerCreateFontDescriptorsFromURL (/* CFURLRef */ IntPtr fileURL);

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (7,0)]
#endif
		public static CTFontDescriptor[] GetFonts (NSUrl url)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

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

		[DllImport (Constants.CoreTextLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CTFontManagerRegisterGraphicsFont (IntPtr cgfont, out IntPtr error);

		public static bool RegisterGraphicsFont (CGFont font, out NSError? error)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
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
					CFObject.CFRelease (h);
			}
			return ret;
		}

		[DllImport (Constants.CoreTextLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CTFontManagerUnregisterGraphicsFont (IntPtr cgfont, out IntPtr error);

		public static bool UnregisterGraphicsFont (CGFont font, out NSError? error)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
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
					CFObject.CFRelease (h);
			}
			return ret;
		}
		
#if !NET
		static CTFontManager ()
		{
			var handle = Libraries.CoreText.Handle;
#if !XAMCORE_3_0
			ErrorDomain  = Dlfcn.GetStringConstant (handle, "kCTFontManagerErrorDomain");
#endif
#pragma warning disable CS0618 // Type or member is obsolete
			ErrorFontUrlsKey  = Dlfcn.GetStringConstant (handle, "kCTFontManagerErrorFontURLsKey");
#pragma warning restore CS0618 // Type or member is obsolete
		}
#endif // !NET

		static NSString? _RegisteredFontsChangedNotification;

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (7,0)]
#endif
		static NSString? RegisteredFontsChangedNotification {
			get {
				if (_RegisteredFontsChangedNotification is null)
					_RegisteredFontsChangedNotification = Dlfcn.GetStringConstant (Libraries.CoreText.Handle, "kCTFontManagerRegisteredFontsChangedNotification");
				return _RegisteredFontsChangedNotification;
			}
		}

#if !XAMCORE_3_0
		public readonly static NSString? ErrorDomain;
#endif
#if !NET
		[Obsolete ("Use the 'CTFontManagerErrorKeys.FontUrlsKey' property instead.")]
		public readonly static NSString? ErrorFontUrlsKey;
#endif

		public static partial class Notifications {
			public static NSObject ObserveRegisteredFontsChanged (EventHandler<NSNotificationEventArgs> handler)
			{
				return NSNotificationCenter.DefaultCenter.AddObserver (RegisteredFontsChangedNotification, notification => handler (null, new NSNotificationEventArgs (notification)));
			}
			
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRegisterFontDescriptors (/* CFArrayRef */ IntPtr fontDescriptors, CTFontManagerScope scope, [MarshalAs (UnmanagedType.I1)] bool enabled, IntPtr registrationHandler);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRegisterFontDescriptors (/* CFArrayRef */ IntPtr fontDescriptors, CTFontManagerScope scope, [MarshalAs (UnmanagedType.I1)] bool enabled, ref BlockLiteral registrationHandler);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RegisterFontDescriptors (CTFontDescriptor[] fontDescriptors, CTFontManagerScope scope, bool enabled, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (fontDescriptors, nameof (fontDescriptors))) {
				if (registrationHandler is null) {
					CTFontManagerRegisterFontDescriptors (arr.Handle, scope, enabled, IntPtr.Zero);
				} else {
					BlockLiteral block_handler = new BlockLiteral ();
					block_handler.SetupBlockUnsafe (callback, registrationHandler);
					CTFontManagerRegisterFontDescriptors (arr.Handle, scope, enabled, ref block_handler);
					block_handler.CleanupBlock ();
				}
			}
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerUnregisterFontDescriptors (/* CFArrayRef */ IntPtr fontDescriptors, CTFontManagerScope scope, IntPtr registrationHandler);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerUnregisterFontDescriptors (/* CFArrayRef */ IntPtr fontDescriptors, CTFontManagerScope scope, ref BlockLiteral registrationHandler);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void UnregisterFontDescriptors (CTFontDescriptor[] fontDescriptors, CTFontManagerScope scope, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (fontDescriptors, nameof (fontDescriptors))) {
				if (registrationHandler is null) {
					CTFontManagerUnregisterFontDescriptors (arr.Handle, scope, IntPtr.Zero);
				} else {
					BlockLiteral block_handler = new BlockLiteral ();
					block_handler.SetupBlockUnsafe (callback, registrationHandler);
					CTFontManagerUnregisterFontDescriptors (arr.Handle, scope, ref block_handler);
					block_handler.CleanupBlock ();
				}
			}
		}

#if __IOS__
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef */ IntPtr CTFontManagerCopyRegisteredFontDescriptors (CTFontManagerScope scope, [MarshalAs (UnmanagedType.I1)] bool enabled);

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[iOS (13,0)]
		[NoWatch]
		[NoTV]
		[NoMac]
#endif
		public static CTFontDescriptor[]? GetRegisteredFontDescriptors (CTFontManagerScope scope, bool enabled)
		{
			var p = CTFontManagerCopyRegisteredFontDescriptors (scope, enabled);
			// Copy/Create rule - we must release the CFArrayRef
			return ArrayFromHandle<CTFontDescriptor> (p, releaseAfterUse: true);
		}
#endif

		// [Watch (2,0), TV (9,0), Mac (10,7), iOS (7,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe /* CTFontDescriptorRef _Nullable */ IntPtr CTFontManagerCreateFontDescriptorFromData (/* CFDataRef */ IntPtr data);

		public static CTFontDescriptor? CreateFontDescriptor (NSData data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			var p = CTFontManagerCreateFontDescriptorFromData (data.Handle);
			if (p == IntPtr.Zero)
				return null;
			// Copy/Create rule - dont retain it inside the .ctor
			return new CTFontDescriptor (p, owns: true);
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe /* CFArrayRef */ IntPtr CTFontManagerCreateFontDescriptorsFromData (/* CFDataRef */ IntPtr data);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public static CTFontDescriptor[]? CreateFontDescriptors (NSData data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			var p = CTFontManagerCreateFontDescriptorsFromData (data.Handle);
			// Copy/Create rule - we must release the CFArrayRef
			return ArrayFromHandle<CTFontDescriptor> (p, releaseAfterUse: true);
		}

#if __IOS__
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[NoWatch]
		[NoTV]
		[NoMac]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRegisterFontsWithAssetNames (/* CFArrayRef */ IntPtr fontAssetNames, /* CFBundleRef _Nullable */ IntPtr bundle, CTFontManagerScope scope, [MarshalAs (UnmanagedType.I1)] bool enabled, IntPtr registrationHandler);

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[NoWatch]
		[NoTV]
		[NoMac]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRegisterFontsWithAssetNames (/* CFArrayRef */ IntPtr fontAssetNames, /* CFBundleRef _Nullable */ IntPtr bundle, CTFontManagerScope scope, [MarshalAs (UnmanagedType.I1)] bool enabled, ref BlockLiteral registrationHandler);

		// reminder that NSBundle and CFBundle are NOT toll-free bridged :(
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[NoWatch]
		[NoTV]
		[NoMac]
		[iOS (13,0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RegisterFonts (string[] assetNames, CFBundle bundle, CTFontManagerScope scope, bool enabled, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (assetNames, nameof (assetNames))) {
				if (registrationHandler is null) {
					CTFontManagerRegisterFontsWithAssetNames (arr.Handle, bundle.GetHandle (), scope, enabled, IntPtr.Zero);
				} else {
					BlockLiteral block_handler = new BlockLiteral ();
					block_handler.SetupBlockUnsafe (callback, registrationHandler);
					CTFontManagerRegisterFontsWithAssetNames (arr.Handle, bundle.GetHandle (), scope, enabled, ref block_handler);
					block_handler.CleanupBlock ();
				}
			}
		}

#if NET
		// [SupportedOSPlatform ("ios13.0")] - Not valid on delegate declaration
		// [SupportedOSPlatform ("maccatalyst")]
		// [UnsupportedOSPlatform ("tvos")]
		// [UnsupportedOSPlatform ("macos")]
#else
		[NoWatch]
		[NoTV]
		[NoMac]
		[iOS (13,0)]
#endif
		public delegate void CTFontManagerRequestFontsHandler (CTFontDescriptor[] unresolvedFontDescriptors);

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[NoWatch]
		[NoTV]
		[NoMac]
		[iOS (13,0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRequestFonts (/* CFArrayRef */ IntPtr fontDescriptors, ref BlockLiteral completionHandler);

		internal delegate void InnerRequestFontsHandler (IntPtr block, IntPtr fontDescriptors);
		static readonly InnerRequestFontsHandler requestCallback = TrampolineRequestFonts;

		[MonoPInvokeCallback (typeof (InnerRequestFontsHandler))]
		static unsafe void TrampolineRequestFonts (IntPtr block, /* CFArray */ IntPtr fontDescriptors)
		{
			var del = BlockLiteral.GetTarget<CTFontManagerRequestFontsHandler> (block);
			if (del is not null)
				del (NSArray.ArrayFromHandle<CTFontDescriptor> (fontDescriptors));
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[NoWatch]
		[NoTV]
		[NoMac]
		[iOS (13,0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RequestFonts (CTFontDescriptor[] fontDescriptors, CTFontManagerRequestFontsHandler completionHandler)
		{
			if (completionHandler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (completionHandler));

			using (var arr = EnsureNonNullArray (fontDescriptors, nameof (fontDescriptors))) {
				BlockLiteral block_handler = new BlockLiteral ();
				block_handler.SetupBlockUnsafe (requestCallback, completionHandler);
				CTFontManagerRequestFonts (arr.Handle, ref block_handler);
				block_handler.CleanupBlock ();
			}
		}
#endif
	}
}
