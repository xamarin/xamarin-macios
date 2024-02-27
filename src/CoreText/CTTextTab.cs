// 
// CTTextTab.cs: Implements the managed CTTextTab
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
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
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

	#region Text Tab Constants
#if !NET
	public static class CTTextTabOptionKey {

		public static readonly NSString ColumnTerminators;

		static CTTextTabOptionKey ()
		{
			ColumnTerminators = Dlfcn.GetStringConstant (Libraries.CoreText.Handle, "kCTTabColumnTerminatorsAttributeName")!;
		}
	}
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTTextTabOptions {

		public CTTextTabOptions ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTTextTabOptions (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary { get; private set; }

		public NSCharacterSet ColumnTerminators {
			get { return (NSCharacterSet) Dictionary [CTTextTabOptionKey.ColumnTerminators]; }
			set { Adapter.SetValue (Dictionary, CTTextTabOptionKey.ColumnTerminators, value); }
		}
	}

	static class CTTextTabOptionsExtensions {
		public static IntPtr GetHandle (this CTTextTabOptions? self)
		{
			if (self is null)
				return IntPtr.Zero;
			return self.Dictionary.GetHandle ();
		}
	}
	#endregion

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTTextTab : NativeObject {
		[Preserve (Conditional = true)]
		internal CTTextTab (NativeHandle handle, bool owns)
			: base (handle, owns, verify: true)
		{
		}

		#region Text Tab Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTextTabCreate (CTTextAlignment alignment, double location, IntPtr options);
		public CTTextTab (CTTextAlignment alignment, double location)
			: this (alignment, location, null)
		{
		}

		public CTTextTab (CTTextAlignment alignment, double location, CTTextTabOptions? options)
			: base (CTTextTabCreate (alignment, location, options.GetHandle ()), true, true)
		{
		}
		#endregion

		#region Text Tab Access
		[DllImport (Constants.CoreTextLibrary)]
		static extern CTTextAlignment CTTextTabGetAlignment (IntPtr tab);
		public CTTextAlignment TextAlignment {
			get { return CTTextTabGetAlignment (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTTextTabGetLocation (IntPtr tab);
		public double Location {
			get { return CTTextTabGetLocation (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTextTabGetOptions (IntPtr tab);
		public CTTextTabOptions? GetOptions ()
		{
			var options = CTTextTabGetOptions (Handle);
			if (options == IntPtr.Zero)
				return null;
			return new CTTextTabOptions (Runtime.GetNSObject<NSDictionary> (options)!);
		}
		#endregion
	}
}
