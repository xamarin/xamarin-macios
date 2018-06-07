// 
// CGColorSpace.cs: Implements geometry classes
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2011-2014,2016 Xamarin Inc
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
using System.ComponentModel;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace CoreGraphics {

	// untyped enum -> CGColorSpace.h
	public enum CGColorRenderingIntent {
		Default,
		AbsoluteColorimetric,
		RelativeColorimetric,
		Perceptual,
		Saturation
	};
	
	// untyped enum -> CGColorSpace.h
	public enum CGColorSpaceModel {
		Unknown = -1,
		Monochrome,
		RGB,
		CMYK,
		Lab,
		DeviceN,
		Indexed,
		Pattern
	}

	public class CGColorSpace : INativeObject
#if !COREBUILD 
		, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;

#if !XAMCORE_3_0
		[Obsolete ("Use a real 'null' value instead of this managed wrapper over a null native instance.")]
#if XAMCORE_2_0
		readonly
#endif
		public static CGColorSpace Null = new CGColorSpace (IntPtr.Zero);
#endif

		// Invoked by the marshallers, we need to take a ref
		public CGColorSpace (IntPtr handle)
		{
			this.handle = handle;
			CGColorSpaceRetain (handle);
		}

		public CGColorSpace (CFPropertyList propertyList)
		{
			if (propertyList == null)
				throw new ArgumentNullException (nameof(propertyList));
			this.handle = CGColorSpaceCreateWithPropertyList (propertyList.Handle);
		}

		[Preserve (Conditional=true)]
		internal CGColorSpace (IntPtr handle, bool owns)
		{
			if (!owns)
				CGColorSpaceRetain (handle);

			this.handle = handle;
		}

		~CGColorSpace ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorSpaceRelease (/* CGColorSpaceRef */ IntPtr space);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceRetain (/* CGColorSpaceRef */ IntPtr space);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGColorSpaceRelease (handle);
				handle = IntPtr.Zero;
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateDeviceGray ();

		public static CGColorSpace CreateDeviceGray ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceGray (), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateDeviceRGB ();

		public static CGColorSpace CreateDeviceRGB ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceRGB (), true);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorSpaceCreateDeviceCMYK ();

		public static /* CGColorSpaceRef */ CGColorSpace CreateDeviceCmyk ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceCMYK (), true);
		}

#if !XAMCORE_3_0
		[Obsolete ("This method has been renamed 'CreateDeviceCmyk'.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static /* CGColorSpaceRef */ CGColorSpace CreateDeviceCMYK ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceCMYK (), true);
		}
#endif
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateCalibratedGray (/* const CGFloat[3] */ nfloat [] whitepoint, /* const CGFloat[3] */ nfloat [] blackpoint, /* CGFloat */ nfloat gamma);

		public static CGColorSpace CreateCalibratedGray (nfloat [] whitepoint, nfloat [] blackpoint, nfloat gamma)
		{
			if (whitepoint == null)
				throw new ArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint != null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));
			
			var ptr = CGColorSpaceCreateCalibratedGray (whitepoint, blackpoint, gamma);
			return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, true);
		}
		
		// 3, 3, 3, 9
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateCalibratedRGB (/* const CGFloat[3] */ nfloat [] whitePoint, /* const CGFloat[3] */ nfloat [] blackPoint, /* const CGFloat[3] */ nfloat [] gamma, /* const CGFloat[9] */ nfloat [] matrix);

		public static CGColorSpace CreateCalibratedRGB (nfloat [] whitepoint, nfloat [] blackpoint, nfloat [] gamma, nfloat [] matrix)
		{
			if (whitepoint == null)
				throw new ArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint != null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));
			if (gamma != null && gamma.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (gamma));
			if (matrix != null && matrix.Length != 9)
				throw new ArgumentException ("Must be null or have exactly 9 values", nameof (matrix));
			
			var ptr = CGColorSpaceCreateCalibratedRGB (whitepoint, blackpoint, gamma, matrix);
			return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef __nullable */ IntPtr CGColorSpaceCreateLab (nfloat [] whitepoint, nfloat [] blackpoint, nfloat [] range);

		// Available since the beginning of time
		public static CGColorSpace CreateLab (nfloat [] whitepoint, nfloat [] blackpoint, nfloat [] range)
		{
			if (whitepoint == null)
				throw new ArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint != null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));
			if (range != null && range.Length != 4)
				throw new ArgumentException ("Must be null or have exactly 4 values", nameof (range));

			var ptr = CGColorSpaceCreateLab (whitepoint, blackpoint, range);
			return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateIndexed (/* CGColorSpaceRef */ IntPtr baseSpace,
			/* size_t */ nint lastIndex, /* const unsigned char* */ byte[] colorTable);

		public static CGColorSpace CreateIndexed (CGColorSpace baseSpace, int lastIndex, byte[] colorTable)
		{
			var ptr = CGColorSpaceCreateIndexed (baseSpace == null ? IntPtr.Zero : baseSpace.handle, lastIndex, colorTable);
			return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, true);
		}

			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreatePattern (/* CGColorSpaceRef */ IntPtr baseSpace);

		public static CGColorSpace CreatePattern (CGColorSpace baseSpace)
		{
			var ptr = CGColorSpaceCreatePattern (baseSpace == null ? IntPtr.Zero : baseSpace.handle);
			return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, true);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithName (/* CFStringRef */ IntPtr name);

		public static CGColorSpace CreateWithName (string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			using (var ns = new NSString (name)) {
				var cs = CGColorSpaceCreateWithName (ns.Handle);
				return cs == IntPtr.Zero ? null : new CGColorSpace (cs, true);
			}
		}

		static CGColorSpace Create (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;
			var r = CGColorSpaceCreateWithName (handle);
			return r == IntPtr.Zero ? null : new CGColorSpace (r, true);
		}
			
		[iOS (9,0)]
		public static CGColorSpace CreateGenericGray ()
		{
			return Create (CGColorSpaceNames.GenericGray.Handle);
		}

		[iOS (9,0)]
		public static CGColorSpace CreateGenericRgb ()
		{
			return Create (CGColorSpaceNames.GenericRgb.Handle);
		}

		[iOS (8,0)]
		public static CGColorSpace CreateGenericCmyk ()
		{
			return Create (CGColorSpaceNames.GenericCmyk.Handle);
		}

		[iOS (9,0)]
		public static CGColorSpace CreateGenericRgbLinear ()
		{
			return Create (CGColorSpaceNames.GenericRgbLinear.Handle);
		}

		[iOS (8,0)]
		public static CGColorSpace CreateAdobeRgb1988 ()
		{
			return Create (CGColorSpaceNames.AdobeRgb1998.Handle);
		}

		[iOS (8,0)]
		public static CGColorSpace CreateSrgb ()
		{
			return Create (CGColorSpaceNames.Srgb.Handle);
		}

		[iOS (8,0)]
		public static CGColorSpace CreateGenericGrayGamma2_2 ()
		{
			return Create (CGColorSpaceNames.GenericGrayGamma2_2.Handle);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CGColorSpace CreateGenericXyz ()
		{
			return Create (CGColorSpaceNames.GenericXyz.Handle);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CGColorSpace CreateAcesCGLinear ()
		{
			return Create (CGColorSpaceNames.AcesCGLinear.Handle);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CGColorSpace CreateItuR_709 ()
		{
			return Create (CGColorSpaceNames.ItuR_709.Handle);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CGColorSpace CreateItuR_2020 ()
		{
			return Create (CGColorSpaceNames.ItuR_2020.Handle);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CGColorSpace CreateRommRgb ()
		{
			return Create (CGColorSpaceNames.RommRgb.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceGetBaseColorSpace (/* CGColorSpaceRef */ IntPtr space);

		public CGColorSpace GetBaseColorSpace ()
		{
			var h = CGColorSpaceGetBaseColorSpace (handle);
			return h == IntPtr.Zero ? null : new CGColorSpace (h, false);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGColorSpaceModel CGColorSpaceGetModel (/* CGColorSpaceRef */ IntPtr space);

		public CGColorSpaceModel Model {
			get {
				return CGColorSpaceGetModel (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGColorSpaceGetNumberOfComponents (/* CGColorSpaceRef */ IntPtr space);
		
		public nint Components {
			get {
				return CGColorSpaceGetNumberOfComponents (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGColorSpaceGetColorTableCount (/* CGColorSpaceRef */ IntPtr space);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorSpaceGetColorTable (/* CGColorSpaceRef */ IntPtr space, /* uint8_t* */ byte[] table);
		
		static byte[] Empty = new byte [0];
		
		public byte[] GetColorTable ()
		{
			nint n = CGColorSpaceGetColorTableCount (handle);
			if (n == 0)
				return Empty;
			
			byte[] table = new byte [n * GetBaseColorSpace ().Components];
			CGColorSpaceGetColorTable (handle, table);
			return table;
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreateIDCCData' instead.")]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithICCProfile (/* CFDataRef */ IntPtr data);

		[DllImport (Constants.CoreGraphicsLibrary)]
		[iOS (10,0)][Mac (10,12)][Watch (3,0)][TV (10,0)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithICCData (/* CFTypeRef cg_nullable */ IntPtr data);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreateIDCCData' instead.")]
#if XAMCORE_4_0
		public static CGColorSpace CreateIccProfile (NSData data)
#else
		public static CGColorSpace CreateICCProfile (NSData data)
#endif
		{
			IntPtr ptr = CGColorSpaceCreateWithICCProfile (data.GetHandle ());
			return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, true);
		}

		[iOS (10, 0)]
		[Mac (10, 12)]
		public static CGColorSpace CreateIccData (NSData data)
		{
			return CreateIccData (data.GetHandle ());
		}

		[iOS (10,0)][Mac (10,12)][Watch (3,0)][TV (10,0)]
		public static CGColorSpace CreateIccData (CGDataProvider provider)
		{
			return CreateIccData (provider.GetHandle ());
		}

		static CGColorSpace CreateIccData (IntPtr handle)
		{
			var ptr = CGColorSpaceCreateWithICCData (handle);
			return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateICCBased (/* size_t */ nint nComponents,
			/* const CGFloat* __nullable */ nfloat[] range, 
			/* CGDataProviderRef __nullable */ IntPtr profile,
			/* CGColorSpaceRef __nullable */ IntPtr alternate);

#if XAMCORE_4_0
		public static CGColorSpace CreateIccProfile (nfloat[] range, CGDataProvider profile, CGColorSpace alternate)
#else
		public static CGColorSpace CreateICCProfile (nfloat[] range, CGDataProvider profile, CGColorSpace alternate)
#endif
		{
			nint nComponents = range == null ? 0 : range.Length / 2;
			IntPtr p = profile == null ? IntPtr.Zero : profile.Handle;
			IntPtr a = alternate == null ? IntPtr.Zero : alternate.Handle;
			var ptr = CGColorSpaceCreateICCBased (nComponents, range, p, a);
			return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'GetICCData' instead." )]
		extern static /* CFDataRef */ IntPtr CGColorSpaceCopyICCProfile (/* CGColorSpaceRef */ IntPtr space);

		[iOS (7,0)] // note: pre-release docs/headers says iOS6 and later, available on OSX since 10.5
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'GetICCData' instead." )]
#if XAMCORE_4_0
		public NSData GetIccProfile ()
#else
		public NSData GetICCProfile ()
#endif
		{
			IntPtr ptr = CGColorSpaceCopyICCProfile (handle);
			return (ptr == IntPtr.Zero) ? null : new NSData (ptr, true);
		}

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CFDataRef* */ IntPtr CGColorSpaceCopyICCData (/* CGColorSpaceRef */ IntPtr space);

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		public NSData GetIccData ()
		{
			IntPtr ptr = CGColorSpaceCopyICCData (handle);
			return (ptr == IntPtr.Zero) ? null : new NSData (ptr, true);
		}

		[iOS (10,0)]
		[TV (10,0)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern unsafe /* CFStringRef* */ IntPtr CGColorSpaceCopyName (/* CGColorSpaceRef */ IntPtr space);

		[iOS (10,0)]
		[TV (10,0)]
		public string Name {
			get {
				return CFString.FetchString (CGColorSpaceCopyName (handle), true);
			}
		}

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern bool CGColorSpaceIsWideGamutRGB (/* CGColorSpaceRef */ IntPtr space);

		[iOS (10,0)][Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
		public bool IsWideGamutRgb {
			get {
				return CGColorSpaceIsWideGamutRGB (handle);
			}
		}

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern bool CGColorSpaceSupportsOutput (/* CGColorSpaceRef */ IntPtr space);

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		public bool SupportsOutput {
			get {
				return CGColorSpaceSupportsOutput (handle);
			}
		}

		[iOS(10,0)][Mac(10,12)][TV(10,0)][Watch(5,0)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCopyPropertyList (IntPtr space);

		[iOS(10,0)][Mac(10,12)][TV(10,0)][Watch(5,0)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateWithPropertyList (IntPtr plist);

		[iOS(10,0)][Mac(10,12)]
		[TV(10,0)][Watch(5,0)]
		public CFPropertyList ToPropertyList ()
		{
			var x = CGColorSpaceCopyPropertyList (handle);
			if (x == IntPtr.Zero)
				return null;
			return new CFPropertyList (x);
		}
		
#endif // !COREBUILD
	}
}
