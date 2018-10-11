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
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

namespace CoreText {

#region Text Tab Constants
	public static class CTTextTabOptionKey {

		public static readonly NSString ColumnTerminators;

		static CTTextTabOptionKey ()
		{
			ColumnTerminators = Dlfcn.GetStringConstant (Libraries.CoreText.Handle, "kCTTabColumnTerminatorsAttributeName");
		}
	}

	public class CTTextTabOptions {

		public CTTextTabOptions ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTTextTabOptions (NSDictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException ("dictionary");
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary {get; private set;}

		public NSCharacterSet ColumnTerminators {
			get {return (NSCharacterSet) Dictionary [CTTextTabOptionKey.ColumnTerminators];}
			set {Adapter.SetValue (Dictionary, CTTextTabOptionKey.ColumnTerminators, value);}
		}
	}
#endregion

	public class CTTextTab : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CTTextTab (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw ConstructorError.ArgumentNull (this, "handle");

			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}
		
		public IntPtr Handle {
			get {return handle;}
		}

		~CTTextTab ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

#region Text Tab Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTextTabCreate (CTTextAlignment alignment, double location, IntPtr options);
		public CTTextTab (CTTextAlignment alignment, double location)
			: this (alignment, location, null)
		{
		}

		public CTTextTab (CTTextAlignment alignment, double location, CTTextTabOptions options)
		{
			handle = CTTextTabCreate (alignment, location, 
					options == null ? IntPtr.Zero : options.Dictionary.Handle);

			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}
#endregion

#region Text Tab Access
		[DllImport (Constants.CoreTextLibrary)]
		static extern CTTextAlignment CTTextTabGetAlignment (IntPtr tab);
		public CTTextAlignment TextAlignment {
			get {return CTTextTabGetAlignment (handle);}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTTextTabGetLocation (IntPtr tab);
		public double Location {
			get {return CTTextTabGetLocation (handle);}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTextTabGetOptions (IntPtr tab);
		public CTTextTabOptions GetOptions ()
		{
			var options = CTTextTabGetOptions (handle);
			if (options == IntPtr.Zero)
				return null;
			return new CTTextTabOptions (
					(NSDictionary) Runtime.GetNSObject (options));
		}
#endregion
	}
}

