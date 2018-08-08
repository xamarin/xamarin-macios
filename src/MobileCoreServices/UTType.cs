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

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace MobileCoreServices {

#if XAMCORE_2_0
	static
#endif
	public partial class UTType {

		[iOS (8,0)][Mac (10,10)]
		[DllImport (Constants.CoreServicesLibrary)]
		extern static int /* Boolean */ UTTypeIsDynamic (IntPtr /* CFStringRef */ handle);
		
		[iOS (8,0)][Mac (10,10)]
		[DllImport (Constants.CoreServicesLibrary)]
		extern static int /* Boolean */ UTTypeIsDeclared (IntPtr /* CFStringRef */ handle);

		[iOS (8,0)][Mac (10,10)]
		public static bool IsDynamic (string utType)
		{
			if (utType == null)
				throw new ArgumentNullException ("utType");

			using (var x = new NSString (utType)) 
				return UTTypeIsDynamic (x.Handle) != 0;
		}

		[iOS (8,0)][Mac (10,10)]
		public static bool IsDeclared (string utType)
		{
			if (utType == null)
				throw new ArgumentNullException ("utType");

			using (var x = new NSString (utType)) 
				return UTTypeIsDeclared (x.Handle) != 0;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSString */ UTTypeCreatePreferredIdentifierForTag (IntPtr /* CFStringRef */ tagClassStr, IntPtr /* CFStringRef */ tagStr, IntPtr /* CFStringRef */ conformingToUtiStr);

		public static string CreatePreferredIdentifier (string tagClass, string tag, string conformingToUti)
		{
			var a = NSString.CreateNative (tagClass);
			var b = NSString.CreateNative (tag);
			var c = NSString.CreateNative (conformingToUti);
			var ret = NSString.FromHandle (UTTypeCreatePreferredIdentifierForTag (a, b, c));
			NSString.ReleaseNative (a);
			NSString.ReleaseNative (b);
			NSString.ReleaseNative (c);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSString Array */ UTTypeCreateAllIdentifiersForTag (IntPtr /* CFStringRef */ tagClassStr, IntPtr /* CFStringRef */ tagStr, IntPtr /* CFStringRef */ conformingToUtiStr);

#if XAMCORE_2_0
		static
#endif
		public string [] CreateAllIdentifiers (string tagClass, string tag, string conformingToUti)
		{
			if (tagClass == null)
				throw new ArgumentNullException ("tagClass");
			if (tag == null)
				throw new ArgumentNullException ("tag");

			var a = NSString.CreateNative (tagClass);
			var b = NSString.CreateNative (tag);
			var c = NSString.CreateNative (conformingToUti);
			var ret = NSArray.StringArrayFromHandle (UTTypeCreateAllIdentifiersForTag (a, b, c));
			NSString.ReleaseNative (a);
			NSString.ReleaseNative (b);
			NSString.ReleaseNative (c);
			return ret;
		}
		
		[iOS (8,0)][Mac (10,10)]
		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSString Array */ UTTypeCopyAllTagsWithClass (IntPtr /* CFStringRef */ utiStr, IntPtr /* CFStringRef */ tagClassStr);
		
		[iOS (8,0)][Mac (10,10)]
#if XAMCORE_2_0
		static
#endif
		public string [] CopyAllTags (string uti, string tagClass)
		{
			if (uti == null)
				throw new ArgumentNullException ("uti");
			if (tagClass == null)
				throw new ArgumentNullException ("tagClass");

			var a = NSString.CreateNative (uti);
			var b = NSString.CreateNative (tagClass);
			var ret = NSArray.StringArrayFromHandle (UTTypeCopyAllTagsWithClass (a, b));
			NSString.ReleaseNative (a);
			NSString.ReleaseNative (b);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static int /* Boolean */ UTTypeConformsTo (IntPtr /* CFStringRef */ utiStr, IntPtr /* CFStringRef */ conformsToUtiStr);

#if XAMCORE_2_0
		static
#endif
		public bool ConformsTo (string uti, string conformsToUti)
		{
			if (uti == null)
				throw new ArgumentNullException ("uti");
			if (conformsToUti == null)
				throw new ArgumentNullException ("conformsToUti");

			var a = NSString.CreateNative (uti);
			var b = NSString.CreateNative (conformsToUti);
			var ret = UTTypeConformsTo (a, b);
			NSString.ReleaseNative (a);
			NSString.ReleaseNative (b);
			return ret != 0;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSString */ UTTypeCopyDescription (IntPtr /* CFStringRef */ utiStr);

#if XAMCORE_2_0
		static
#endif
		public string GetDescription (string uti)
		{
			if (uti == null)
				throw new ArgumentNullException ("uti");

			var a = NSString.CreateNative (uti);
			var ret = NSString.FromHandle (UTTypeCopyDescription (a));
			NSString.ReleaseNative (a);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* CFStringRef */ UTTypeCopyPreferredTagWithClass (IntPtr /* CFStringRef */ uti, IntPtr /* CFStringRef */ tagClass);

		public static string GetPreferredTag (string uti, string tagClass)
		{
			if (uti == null)
				throw new ArgumentNullException ("uti");
			if (tagClass == null)
				throw new ArgumentNullException ("tagClass");

			var a = NSString.CreateNative (uti);
			var b = NSString.CreateNative (tagClass);
			var ret = NSString.FromHandle (UTTypeCopyPreferredTagWithClass (a, b));
			NSString.ReleaseNative (a);
			NSString.ReleaseNative (b);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSDictionary */ UTTypeCopyDeclaration (IntPtr utiStr);

		public static NSDictionary GetDeclaration (string uti)
		{
			if (uti == null)
				throw new ArgumentNullException ("uti");

			var a = NSString.CreateNative (uti);
			var ret = Runtime.GetNSObject <NSDictionary> (UTTypeCopyDeclaration (a));
			NSString.ReleaseNative (a);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		extern static IntPtr /* NSUrl */ UTTypeCopyDeclaringBundleURL (IntPtr utiStr);

		public static NSUrl GetDeclaringBundleURL (string uti)
		{
			if (uti == null)
				throw new ArgumentNullException ("uti");

			var a = NSString.CreateNative (uti);
			var ret = Runtime.GetNSObject <NSUrl> (UTTypeCopyDeclaringBundleURL (a));
			NSString.ReleaseNative (a);
			return ret;
		}

		[DllImport (Constants.CoreServicesLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern unsafe bool /* Boolean */ UTTypeEqual (/* CFStringRef */ IntPtr inUTI1, /* CFStringRef */ IntPtr inUTI2);

		[iOS (12,0)][TV (12,0)][Watch (5,0)]
		public static bool Equals (NSString uti1, NSString uti2)
		{
			if (uti1 == null)
				return uti2 == null;
			else if (uti2 == null)
				return false;
			return UTTypeEqual (uti1.Handle, uti2.Handle);
		}
	}
}
