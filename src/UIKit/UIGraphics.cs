//
// UIGraphics.cs: Bindings to some P/Invoke APIs to interface with CoreGraphics
//
// Authors:
//   Miguel de Icaza (miguel@novell.com)
//
// Copyright 2009 Novell, inc.
// Copyright 2012-2014 Xamarin Inc
//
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System;
using System.ComponentModel;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {

#if XAMCORE_5_0
#error All PDF public name instances in this file need to be turned into Pdf. e.g. EndPDFContext into EndPdfContext.
#endif

	public static class UIGraphics {
		[DllImport (Constants.UIKitLibrary)]
		extern static IntPtr UIGraphicsGetCurrentContext ();

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsPushContext (IntPtr ctx);

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsPopContext ();

		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIRectFillUsingBlendMode")]
		public extern static void RectFillUsingBlendMode (CGRect rect, CGBlendMode blendMode);

		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIRectFill")]
		public extern static void RectFill (CGRect rect);

		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIRectFrameUsingBlendMode")]
		public extern static void RectFrameUsingBlendMode (CGRect rect, CGBlendMode blendMode);

		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIRectFrame")]
		public extern static void RectFrame (CGRect rect);

		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIRectClip")]
		public extern static void RectClip (CGRect rect);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos17.0")]
		[UnsupportedOSPlatform ("maccatalyst17.0")]
#else
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
#endif
		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIGraphicsBeginImageContext")]
		public extern static void BeginImageContext (CGSize size);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos17.0")]
		[UnsupportedOSPlatform ("maccatalyst17.0")]
#else
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
#endif
		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIGraphicsBeginImageContextWithOptions")]
		extern static void BeginImageContextWithOptions (CGSize size, byte opaque, nfloat scale);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos17.0")]
		[UnsupportedOSPlatform ("maccatalyst17.0")]
#else
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
#endif
		public static void BeginImageContextWithOptions (CGSize size, bool opaque, nfloat scale)
		{
			BeginImageContextWithOptions (size, opaque ? (byte) 1 : (byte) 0, scale);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos17.0")]
		[UnsupportedOSPlatform ("maccatalyst17.0")]
#else
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
#endif
		[DllImport (Constants.UIKitLibrary)]
		static extern IntPtr UIGraphicsGetImageFromCurrentImageContext ();

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos17.0")]
		[UnsupportedOSPlatform ("maccatalyst17.0")]
#else
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
#endif
		[DllImport (Constants.UIKitLibrary, EntryPoint = "UIGraphicsEndImageContext")]
		public extern static void EndImageContext ();

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsAddPDFContextDestinationAtPoint (IntPtr str, CGPoint point);

		public static void AddPDFContextDestination (string str, CGPoint point)
		{
			using (var nsstr = new NSString (str))
				UIGraphicsAddPDFContextDestinationAtPoint (nsstr.Handle, point);
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsSetPDFContextDestinationForRect (IntPtr str, CGRect rect);

		public static void SetPDFContextDestination (string str, CGRect rect)
		{
			using (var nsstr = new NSString (str))
				UIGraphicsSetPDFContextDestinationForRect (nsstr.Handle, rect);
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static CGRect UIGraphicsGetPDFContextBounds ();

		public static CGRect PDFContextBounds {
			get {
				return UIGraphicsGetPDFContextBounds ();
			}
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsSetPDFContextURLForRect (IntPtr url, CGRect rect);

		public static void SetPDFContextURL (NSUrl url, CGRect rect)
		{
			UIGraphicsSetPDFContextURLForRect (url.Handle, rect);
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsBeginPDFContextToFile (/* NSString* */ IntPtr path, CGRect bounds,
			/* NSDictionary * __nullable */ IntPtr documentInfo);

		public static void BeginPDFContext (string file, CGRect bounds, NSDictionary documentInfo)
		{
			using (var nsstr = new NSString (file))
				UIGraphicsBeginPDFContextToFile (nsstr.Handle, bounds, documentInfo is null ? IntPtr.Zero : documentInfo.Handle);
		}

		public static void BeginPDFContext (string file, CGRect bounds, CGPDFInfo documentInfo)
		{
			using (var dict = documentInfo is null ? null : documentInfo.ToDictionary ())
			using (var nsstr = new NSString (file))
				UIGraphicsBeginPDFContextToFile (nsstr.Handle, bounds, dict is null ? IntPtr.Zero : dict.Handle);
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsBeginPDFContextToData (/* NSMutableData* */ IntPtr data, CGRect bounds,
			/* NSDictionary * __nullable */ IntPtr documentInfo);

		public static void BeginPDFContext (NSMutableData data, CGRect bounds, NSDictionary documentInfo)
		{
			UIGraphicsBeginPDFContextToData (data.Handle, bounds, documentInfo is null ? IntPtr.Zero : documentInfo.Handle);
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsBeginPDFPage ();

		public static void BeginPDFPage ()
		{
			UIGraphicsBeginPDFPage ();
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsBeginPDFPageWithInfo (CGRect bounds, IntPtr info);

		public static void BeginPDFPage (CGRect bounds, NSDictionary pageInfo)
		{
			UIGraphicsBeginPDFPageWithInfo (bounds, pageInfo.Handle);
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIGraphicsEndPDFContext ();

		public static void EndPDFContext ()
		{
			UIGraphicsEndPDFContext ();
		}

#if !XAMCORE_5_0
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'EndPDFContext' instead.")]
		public static void EndPDFContent () => EndPDFContext ();
#endif

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios17.0")]
		[UnsupportedOSPlatform ("tvos17.0")]
		[UnsupportedOSPlatform ("maccatalyst17.0")]
#else
		[Deprecated (PlatformName.iOS, 17, 0)]
		[Deprecated (PlatformName.TvOS, 17, 0)]
#endif
		public static UIImage GetImageFromCurrentImageContext ()
		{
			using (var pool = new NSAutoreleasePool ()) {
				var handle = UIGraphicsGetImageFromCurrentImageContext ();
				return (UIImage) Runtime.GetNSObject (handle);
			}
		}

		public static CGContext GetCurrentContext ()
		{
			var ctx = UIGraphicsGetCurrentContext ();

			if (ctx == IntPtr.Zero)
				return null;

			return new CGContext (ctx, false);
		}

		public static void PushContext (CGContext ctx)
		{
			UIGraphicsPushContext (ctx.Handle);
		}

		public static void PopContext ()
		{
			UIGraphicsPopContext ();
		}
	}
}
