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
		[iOS (13,0)][TV (13,0)][Watch (6,0)]
		Persistent = 2,
		[NoiOS][NoTV][NoWatch]
		Session = 3,
#if !XAMCORE_4_0
		[NoiOS][NoTV][NoWatch] // historically not available under the old name
		User = Persistent,
#endif
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
					CFObject.CFRelease (error);
			}
		}

		static NSArray EnsureNonNullArray (object [] items, string name)
		{
			if (items == null)
				throw new ArgumentNullException (name);

			foreach (var i in items)
				if (i == null)
					throw new ArgumentException ("Array contains a null entry", name);

			return NSArray.FromObjects (items);
		}

		static T[] ArrayFromHandle<T> (IntPtr handle, bool releaseAfterUse) where T : class, INativeObject
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

		[Deprecated (PlatformName.MacOSX, 10,15)]
		[Deprecated (PlatformName.iOS, 13,0)]
		[Deprecated (PlatformName.WatchOS, 6,0)]
		[Deprecated (PlatformName.TvOS, 13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerRegisterFontsForURLs(IntPtr arrayRef, CTFontManagerScope scope, ref IntPtr error_array);

		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'RegisterFonts' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'RegisterFonts' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'RegisterFonts' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'RegisterFonts' instead.")]
		public static NSError [] RegisterFontsForUrl (NSUrl [] fontUrls, CTFontManagerScope scope)
		{
			using (var arr = EnsureNonNullArray (fontUrls, nameof (fontUrls))) {
				IntPtr error_array = IntPtr.Zero;
				if (CTFontManagerRegisterFontsForURLs (arr.Handle, scope, ref error_array))
					return null;
				return ArrayFromHandle<NSError> (error_array, releaseAfterUse: true);
			}
		}

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		public delegate bool CTFontRegistrationHandler (NSError[] errors, bool done);

		internal delegate bool InnerRegistrationHandler (IntPtr block, IntPtr errors, bool done);
		static readonly InnerRegistrationHandler callback = TrampolineRegistrationHandler;

		[MonoPInvokeCallback (typeof (InnerRegistrationHandler))]
		static unsafe bool TrampolineRegistrationHandler (IntPtr block, /* NSArray */ IntPtr errors, bool done)
		{
			var del = BlockLiteral.GetTarget<CTFontRegistrationHandler> (block);
			return del != null ? del (NSArray.ArrayFromHandle<NSError> (errors), done) : true;
		}

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTFontManagerRegisterFontURLs (/* CFArrayRef */ IntPtr fontUrls, CTFontManagerScope scope, bool enabled, IntPtr registrationHandler);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTFontManagerRegisterFontURLs (/* CFArrayRef */ IntPtr fontUrls, CTFontManagerScope scope, bool enabled, ref BlockLiteral registrationHandler);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RegisterFonts (NSUrl [] fontUrls, CTFontManagerScope scope, bool enabled, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (fontUrls, nameof (fontUrls))) {
				if (registrationHandler == null) {
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
					CFObject.CFRelease (error);
			}
		}

		[Deprecated (PlatformName.MacOSX, 10,15)]
		[Deprecated (PlatformName.iOS, 13,0)]
		[Deprecated (PlatformName.WatchOS, 6,0)]
		[Deprecated (PlatformName.TvOS, 13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerUnregisterFontsForURLs(IntPtr arrayRef, CTFontManagerScope scope, ref IntPtr error_array);

		[Deprecated (PlatformName.MacOSX, 10,15, message : "Use 'UnregisterFonts' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message : "Use 'UnregisterFonts' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message : "Use 'UnregisterFonts' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message : "Use 'UnregisterFonts' instead.")]
		public static NSError [] UnregisterFontsForUrl (NSUrl [] fontUrls, CTFontManagerScope scope)
		{
			IntPtr error_array = IntPtr.Zero;
			using (var arr = EnsureNonNullArray (fontUrls, nameof (fontUrls))) {
				if (CTFontManagerUnregisterFontsForURLs (arr.Handle, scope, ref error_array))
					return null;
				return ArrayFromHandle<NSError> (error_array, releaseAfterUse: true);
			}
		}

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerUnregisterFontURLs (/* CFArrayRef */ IntPtr fontUrls, CTFontManagerScope scope, IntPtr registrationHandler);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerUnregisterFontURLs (/* CFArrayRef */ IntPtr fontUrls, CTFontManagerScope scope, ref BlockLiteral registrationHandler);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void UnregisterFonts (NSUrl [] fontUrls, CTFontManagerScope scope, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (fontUrls, nameof (fontUrls))) {
				if (registrationHandler == null) {
					CTFontManagerUnregisterFontURLs (arr.Handle, scope, IntPtr.Zero);
				} else {
					BlockLiteral block_handler = new BlockLiteral ();
					block_handler.SetupBlockUnsafe (callback, registrationHandler);
					CTFontManagerUnregisterFontURLs (arr.Handle, scope, ref block_handler);
					block_handler.CleanupBlock ();
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

		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerRegisterGraphicsFont (IntPtr cgfont, out IntPtr error);

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
					CFObject.CFRelease (h);
			}
			return ret;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontManagerUnregisterGraphicsFont (IntPtr cgfont, out IntPtr error);

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
					CFObject.CFRelease (h);
			}
			return ret;
		}
		
		static CTFontManager ()
		{
			var handle = Libraries.CoreText.Handle;
#if !XAMCORE_3_0
			ErrorDomain  = Dlfcn.GetStringConstant (handle, "kCTFontManagerErrorDomain");
#endif
#if !XAMCORE_4_0
			ErrorFontUrlsKey  = Dlfcn.GetStringConstant (handle, "kCTFontManagerErrorFontURLsKey");
#endif
		}

		static NSString _RegisteredFontsChangedNotification;

		[iOS (7,0)]
		static NSString RegisteredFontsChangedNotification {
			get {
				if (_RegisteredFontsChangedNotification == null)
					_RegisteredFontsChangedNotification = Dlfcn.GetStringConstant (Libraries.CoreText.Handle, "kCTFontManagerRegisteredFontsChangedNotification");
				return _RegisteredFontsChangedNotification;
			}
		}

#if !XAMCORE_3_0
		public readonly static NSString ErrorDomain;
#endif
#if !XAMCORE_4_0
		[Obsolete ("Use the 'CTFontManagerErrorKeys.FontUrlsKey' property instead.")]
		public readonly static NSString ErrorFontUrlsKey;
#endif

		public static partial class Notifications {
			public static NSObject ObserveRegisteredFontsChanged (EventHandler<NSNotificationEventArgs> handler)
			{
				return NSNotificationCenter.DefaultCenter.AddObserver (RegisteredFontsChangedNotification, notification => handler (null, new NSNotificationEventArgs (notification)));
			}
			
		}

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRegisterFontDescriptors (/* CFArrayRef */ IntPtr fontDescriptors, CTFontManagerScope scope, bool enabled, IntPtr registrationHandler);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRegisterFontDescriptors (/* CFArrayRef */ IntPtr fontDescriptors, CTFontManagerScope scope, bool enabled, ref BlockLiteral registrationHandler);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RegisterFontDescriptors (CTFontDescriptor[] fontDescriptors, CTFontManagerScope scope, bool enabled, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (fontDescriptors, nameof (fontDescriptors))) {
				if (registrationHandler == null) {
					CTFontManagerRegisterFontDescriptors (arr.Handle, scope, enabled, IntPtr.Zero);
				} else {
					BlockLiteral block_handler = new BlockLiteral ();
					block_handler.SetupBlockUnsafe (callback, registrationHandler);
					CTFontManagerRegisterFontDescriptors (arr.Handle, scope, enabled, ref block_handler);
					block_handler.CleanupBlock ();
				}
			}
		}

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerUnregisterFontDescriptors (/* CFArrayRef */ IntPtr fontDescriptors, CTFontManagerScope scope, IntPtr registrationHandler);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerUnregisterFontDescriptors (/* CFArrayRef */ IntPtr fontDescriptors, CTFontManagerScope scope, ref BlockLiteral registrationHandler);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void UnregisterFontDescriptors (CTFontDescriptor[] fontDescriptors, CTFontManagerScope scope, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (fontDescriptors, nameof (fontDescriptors))) {
				if (registrationHandler == null) {
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
		[iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef */ IntPtr CTFontManagerCopyRegisteredFontDescriptors (CTFontManagerScope scope, bool enabled);

		[iOS (13,0)]
		[NoWatch][NoTV][NoMac]
		public static CTFontDescriptor[] GetRegisteredFontDescriptors (CTFontManagerScope scope, bool enabled)
		{
			var p = CTFontManagerCopyRegisteredFontDescriptors (scope, enabled);
			// Copy/Create rule - we must release the CFArrayRef
			return ArrayFromHandle<CTFontDescriptor> (p, releaseAfterUse: true);
		}
#endif

		// [Watch (2,0), TV (9,0), Mac (10,7), iOS (7,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe /* CTFontDescriptorRef _Nullable */ IntPtr CTFontManagerCreateFontDescriptorFromData (/* CFDataRef */ IntPtr data);

		public static CTFontDescriptor CreateFontDescriptor (NSData data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			var p = CTFontManagerCreateFontDescriptorFromData (data.Handle);
			if (p == IntPtr.Zero)
				return null;
			// Copy/Create rule - dont retain it inside the .ctor
			return new CTFontDescriptor (p, owns: true);
		}

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe /* CFArrayRef */ IntPtr CTFontManagerCreateFontDescriptorsFromData (/* CFDataRef */ IntPtr data);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		public static CTFontDescriptor[] CreateFontDescriptors (NSData data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			var p = CTFontManagerCreateFontDescriptorsFromData (data.Handle);
			// Copy/Create rule - we must release the CFArrayRef
			return ArrayFromHandle<CTFontDescriptor> (p, releaseAfterUse: true);
		}

#if __IOS__
		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRegisterFontsWithAssetNames (/* CFArrayRef */ IntPtr fontAssetNames, /* CFBundleRef _Nullable */ IntPtr bundle, CTFontManagerScope scope, bool enabled, IntPtr registrationHandler);

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRegisterFontsWithAssetNames (/* CFArrayRef */ IntPtr fontAssetNames, /* CFBundleRef _Nullable */ IntPtr bundle, CTFontManagerScope scope, bool enabled, ref BlockLiteral registrationHandler);

		// reminder that NSBundle and CFBundle are NOT toll-free bridged :(
		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RegisterFonts (string[] assetNames, CFBundle bundle, CTFontManagerScope scope, bool enabled, CTFontRegistrationHandler registrationHandler)
		{
			using (var arr = EnsureNonNullArray (assetNames, nameof (assetNames))) {
				if (registrationHandler == null) {
					CTFontManagerRegisterFontsWithAssetNames (arr.Handle, bundle.GetHandle (), scope, enabled, IntPtr.Zero);
				} else {
					BlockLiteral block_handler = new BlockLiteral ();
					block_handler.SetupBlockUnsafe (callback, registrationHandler);
					CTFontManagerRegisterFontsWithAssetNames (arr.Handle, bundle.GetHandle (), scope, enabled, ref block_handler);
					block_handler.CleanupBlock ();
				}
			}
		}

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		public delegate void CTFontManagerRequestFontsHandler (CTFontDescriptor[] unresolvedFontDescriptors);

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe void CTFontManagerRequestFonts (/* CFArrayRef */ IntPtr fontDescriptors, ref BlockLiteral completionHandler);

		internal delegate void InnerRequestFontsHandler (IntPtr block, IntPtr fontDescriptors);
		static readonly InnerRequestFontsHandler requestCallback = TrampolineRequestFonts;

		[MonoPInvokeCallback (typeof (InnerRequestFontsHandler))]
		static unsafe void TrampolineRequestFonts (IntPtr block, /* CFArray */ IntPtr fontDescriptors)
		{
			var del = BlockLiteral.GetTarget<CTFontManagerRequestFontsHandler> (block);
			if (del != null)
				del (NSArray.ArrayFromHandle<CTFontDescriptor> (fontDescriptors));
		}

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static void RequestFonts (CTFontDescriptor[] fontDescriptors, CTFontManagerRequestFontsHandler completionHandler)
		{
			if (completionHandler == null)
				throw new ArgumentNullException (nameof (completionHandler));

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
