// CoreServicesLibrary.UTType
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2012 Xamarin Inc
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
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace MobileCoreServices {

	public static partial class UTType {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("macos11.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
#endif
		[DllImport (Constants.CoreServicesLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool /* Boolean */ UTTypeIsDynamic (IntPtr /* CFStringRef */ handle);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("macos11.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
#endif
		[DllImport (Constants.CoreServicesLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool /* Boolean */ UTTypeIsDeclared (IntPtr /* CFStringRef */ handle);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("macos11.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
#endif
		public static bool IsDynamic (string utType)
		{
			if (utType is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (utType));

			var ptr = CFString.CreateNative (utType);
			var result = UTTypeIsDynamic (ptr);
			CFString.ReleaseNative (ptr);
			return result;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("macos11.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
#endif
		public static bool IsDeclared (string utType)
		{
			if (utType is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (utType));

			var ptr = CFString.CreateNative (utType);
			var result = UTTypeIsDeclared (ptr);
			CFString.ReleaseNative (ptr);
			return result;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSString */ UTTypeCreatePreferredIdentifierForTag (IntPtr /* CFStringRef */ tagClassStr, IntPtr /* CFStringRef */ tagStr, IntPtr /* CFStringRef */ conformingToUtiStr);

		public static string? CreatePreferredIdentifier (string tagClass, string tag, string conformingToUti)
		{
			var a = CFString.CreateNative (tagClass);
			var b = CFString.CreateNative (tag);
			var c = CFString.CreateNative (conformingToUti);
			var ret = CFString.FromHandle (UTTypeCreatePreferredIdentifierForTag (a, b, c));
			CFString.ReleaseNative (a);
			CFString.ReleaseNative (b);
			CFString.ReleaseNative (c);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSString Array */ UTTypeCreateAllIdentifiersForTag (IntPtr /* CFStringRef */ tagClassStr, IntPtr /* CFStringRef */ tagStr, IntPtr /* CFStringRef */ conformingToUtiStr);

		public static string? []? CreateAllIdentifiers (string tagClass, string tag, string conformingToUti)
		{
			if (tagClass is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (tagClass));
			if (tag is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (tag));

			var a = CFString.CreateNative (tagClass);
			var b = CFString.CreateNative (tag);
			var c = CFString.CreateNative (conformingToUti);
			var ret = CFArray.StringArrayFromHandle (UTTypeCreateAllIdentifiersForTag (a, b, c));
			CFString.ReleaseNative (a);
			CFString.ReleaseNative (b);
			CFString.ReleaseNative (c);
			return ret;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("macos11.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
#endif
		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSString Array */ UTTypeCopyAllTagsWithClass (IntPtr /* CFStringRef */ utiStr, IntPtr /* CFStringRef */ tagClassStr);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("tvos14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("macos11.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
#endif
		public static string? []? CopyAllTags (string uti, string tagClass)
		{
			if (uti is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (uti));
			if (tagClass is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (tagClass));

			var a = CFString.CreateNative (uti);
			var b = CFString.CreateNative (tagClass);
			var ret = CFArray.StringArrayFromHandle (UTTypeCopyAllTagsWithClass (a, b));
			CFString.ReleaseNative (a);
			CFString.ReleaseNative (b);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool /* Boolean */ UTTypeConformsTo (IntPtr /* CFStringRef */ utiStr, IntPtr /* CFStringRef */ conformsToUtiStr);

		public static bool ConformsTo (string uti, string conformsToUti)
		{
			if (uti is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (uti));
			if (conformsToUti is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (conformsToUti));

			var a = CFString.CreateNative (uti);
			var b = CFString.CreateNative (conformsToUti);
			var ret = UTTypeConformsTo (a, b);
			CFString.ReleaseNative (a);
			CFString.ReleaseNative (b);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSString */ UTTypeCopyDescription (IntPtr /* CFStringRef */ utiStr);

		public static string? GetDescription (string uti)
		{
			if (uti is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (uti));

			var a = CFString.CreateNative (uti);
			var ret = CFString.FromHandle (UTTypeCopyDescription (a));
			CFString.ReleaseNative (a);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* CFStringRef */ UTTypeCopyPreferredTagWithClass (IntPtr /* CFStringRef */ uti, IntPtr /* CFStringRef */ tagClass);

		public static string? GetPreferredTag (string uti, string tagClass)
		{
			if (uti is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (uti));
			if (tagClass is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (tagClass));

			var a = CFString.CreateNative (uti);
			var b = CFString.CreateNative (tagClass);
			var ret = CFString.FromHandle (UTTypeCopyPreferredTagWithClass (a, b));
			CFString.ReleaseNative (a);
			CFString.ReleaseNative (b);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSDictionary */ UTTypeCopyDeclaration (IntPtr utiStr);

		public static NSDictionary? GetDeclaration (string uti)
		{
			if (uti is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (uti));

			var a = CFString.CreateNative (uti);
			var ret = Runtime.GetNSObject<NSDictionary> (UTTypeCopyDeclaration (a));
			CFString.ReleaseNative (a);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSUrl */ UTTypeCopyDeclaringBundleURL (IntPtr utiStr);

#if NET
		public static NSUrl? GetDeclaringBundleUrl (string uti)
#else
		public static NSUrl? GetDeclaringBundleURL (string uti)
#endif
		{
			if (uti is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (uti));

			var a = CFString.CreateNative (uti);
			var ret = Runtime.GetNSObject<NSUrl> (UTTypeCopyDeclaringBundleURL (a));
			CFString.ReleaseNative (a);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern unsafe bool /* Boolean */ UTTypeEqual (/* CFStringRef */ IntPtr inUTI1, /* CFStringRef */ IntPtr inUTI2);

#if NET
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("tvos14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("macos11.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
		[ObsoletedOSPlatform ("ios14.0", "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
#else
		[iOS (12, 0)]
		[TV (12, 0)]
		[Watch (5, 0)]
#endif
		public static bool Equals (NSString uti1, NSString uti2)
		{
			if (uti1 is null)
				return uti2 is null;
			else if (uti2 is null)
				return false;
			return UTTypeEqual (uti1.Handle, uti2.Handle);
		}
	}
}
