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

#nullable enable

using System.ComponentModel;

using CoreFoundation;

namespace CoreGraphics {

	// untyped enum -> CGColorSpace.h
	/// <summary>Determines how Quartz maps colors from the source color space to the gamut of the destination.</summary>
	///     <remarks>To be added.</remarks>
	public enum CGColorRenderingIntent {
		/// <summary>The default rendering intent.</summary>
		Default,
		/// <summary>Clamps colors outside of the gamut of the device into the space supported by the device.</summary>
		AbsoluteColorimetric,
		/// <summary>Shifts all colors to adjust for the requirements of the context.</summary>
		RelativeColorimetric,
		/// <summary>Preserves the color relationship by compressing the gamut of the graphics context to fit the output device.</summary>
		Perceptual,
		/// <summary>Preserves the relative saturation.</summary>
		Saturation,
	};

	// untyped enum -> CGColorSpace.h
	/// <summary>Color space model.</summary>
	///     <remarks>To be added.</remarks>
	public enum CGColorSpaceModel {
		/// <summary>Unknown color space model.</summary>
		Unknown = -1,
		/// <summary>Monochrome color space model.</summary>
		Monochrome,
		/// <summary>Red, Green and Blue model.</summary>
		RGB,
		/// <summary>Cyan, Magenta, Yellow and Key Black.</summary>
		CMYK,
		/// <summary>CIELAB color space (L* a* b*).</summary>
		Lab,
		/// <summary>DeviceN color space.</summary>
		DeviceN,
		/// <summary>Indexed color model, up to 256 values that are looked up.</summary>
		Indexed,
		/// <summary>Pattern color space, used when stroking or filling with a pattern.</summary>
		Pattern,
		/// <summary>To be added.</summary>
		Xyz,
	}

	/// <summary>Colorspace, determines how Quartz interprets color information.</summary>
	///     <remarks>To be added.</remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/Drawing/">Example_Drawing</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CGColorSpace : NativeObject {
#if !COREBUILD
		static IntPtr Create (CFPropertyList propertyList)
		{
			if (propertyList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (propertyList));
			IntPtr result = CGColorSpaceCreateWithPropertyList (propertyList.GetCheckedHandle ());
			GC.KeepAlive (propertyList);
			return result;
		}

		/// <param name="propertyList">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CGColorSpace (CFPropertyList propertyList)
			: base (Create (propertyList), true)
		{
		}

		[Preserve (Conditional = true)]
		internal CGColorSpace (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		internal static CGColorSpace? FromHandle (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				return null;
			return new CGColorSpace (handle, owns);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorSpaceRelease (/* CGColorSpaceRef */ IntPtr space);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceRetain (/* CGColorSpaceRef */ IntPtr space);

		protected internal override void Retain ()
		{
			CGColorSpaceRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGColorSpaceRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateDeviceGray ();

		/// <summary>Creates a new gray device dependent color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static CGColorSpace CreateDeviceGray ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceGray (), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateDeviceRGB ();

		/// <summary>Creates and returns a device dependent RGB color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static CGColorSpace CreateDeviceRGB ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceRGB (), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorSpaceCreateDeviceCMYK ();

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents a device dependent CMYK color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static /* CGColorSpaceRef */ CGColorSpace CreateDeviceCmyk ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceCMYK (), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern unsafe static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateCalibratedGray (/* const CGFloat[3] */ nfloat* whitepoint, /* const CGFloat[3] */ nfloat* blackpoint, /* CGFloat */ nfloat gamma);

		public static CGColorSpace? CreateCalibratedGray (nfloat [] whitepoint, nfloat []? blackpoint, nfloat gamma)
		{
			if (whitepoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint is not null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));

			unsafe {
				fixed (nfloat* whitepointPtr = whitepoint, blackpointPtr = blackpoint) {
					var ptr = CGColorSpaceCreateCalibratedGray (whitepointPtr, blackpointPtr, gamma);
					return FromHandle (ptr, true);
				}
			}
		}

		// 3, 3, 3, 9
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateCalibratedRGB (/* const CGFloat[3] */ nfloat* whitePoint, /* const CGFloat[3] */ nfloat* blackPoint, /* const CGFloat[3] */ nfloat* gamma, /* const CGFloat[9] */ nfloat* matrix);

		public static CGColorSpace? CreateCalibratedRGB (nfloat [] whitepoint, nfloat []? blackpoint, nfloat []? gamma, nfloat []? matrix)
		{
			if (whitepoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint is not null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));
			if (gamma is not null && gamma.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (gamma));
			if (matrix is not null && matrix.Length != 9)
				throw new ArgumentException ("Must be null or have exactly 9 values", nameof (matrix));

			unsafe {
				fixed (nfloat* whitepointPtr = whitepoint, blackpointPtr = blackpoint, gammaPtr = gamma, matrixPtr = matrix) {
					var ptr = CGColorSpaceCreateCalibratedRGB (whitepointPtr, blackpointPtr, gammaPtr, matrixPtr);
					return FromHandle (ptr, true);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGColorSpaceRef __nullable */ IntPtr CGColorSpaceCreateLab (nfloat* whitepoint, nfloat* blackpoint, nfloat* range);

		// Available since the beginning of time
		public static CGColorSpace? CreateLab (nfloat [] whitepoint, nfloat []? blackpoint, nfloat []? range)
		{
			if (whitepoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint is not null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));
			if (range is not null && range.Length != 4)
				throw new ArgumentException ("Must be null or have exactly 4 values", nameof (range));

			unsafe {
				fixed (nfloat* whitepointPtr = whitepoint, blackpointPtr = blackpoint, rangePtr = range) {
					var ptr = CGColorSpaceCreateLab (whitepointPtr, blackpointPtr, rangePtr);
					return FromHandle (ptr, true);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateIndexed (/* CGColorSpaceRef */ IntPtr baseSpace,
			/* size_t */ nint lastIndex, /* const unsigned char* */ byte [] colorTable);

		/// <include file="../../docs/api/CoreGraphics/CGColorSpace.xml" path="/Documentation/Docs[@DocId='M:CoreGraphics.CGColorSpace.CreateIndexed(CoreGraphics.CGColorSpace,System.Int32,System.Byte[])']/*" />
		public static CGColorSpace? CreateIndexed (CGColorSpace baseSpace, int lastIndex, byte [] colorTable)
		{
			var ptr = CGColorSpaceCreateIndexed (baseSpace.GetHandle (), lastIndex, colorTable);
			GC.KeepAlive (baseSpace);
			return FromHandle (ptr, true);
		}


		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreatePattern (/* CGColorSpaceRef */ IntPtr baseSpace);

		/// <param name="baseSpace">To be added.</param>
		///         <summary>Creates and returns a pattern color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static CGColorSpace? CreatePattern (CGColorSpace? baseSpace)
		{
			var ptr = CGColorSpaceCreatePattern (baseSpace.GetHandle ());
			GC.KeepAlive (baseSpace);
			return FromHandle (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithName (/* CFStringRef */ IntPtr name);

		/// <param name="name">To be added.</param>
		///         <summary>Creates a named color space. Valid names are available in <see cref="CoreGraphics.CGColorSpaceNames" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static CGColorSpace? CreateWithName (string name)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			using (var ns = new NSString (name)) {
				var cs = CGColorSpaceCreateWithName (ns.Handle);
				return FromHandle (cs, true);
			}
		}

		static CGColorSpace? Create (NSString? name)
		{
			if (name is null)
				return null;
			var handle = name.Handle;
			if (handle == NativeHandle.Zero)
				return null;
			var r = CGColorSpaceCreateWithName (handle);
			GC.KeepAlive (name);
			return FromHandle (r, true);
		}

		/// <summary>Creates and returns a generic Gray color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateGenericGray ()
		{
			return Create (CGColorSpaceNames.GenericGray);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents a generic RGB color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateGenericRgb ()
		{
			return Create (CGColorSpaceNames.GenericRgb);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents a generic CMYK color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateGenericCmyk ()
		{
			return Create (CGColorSpaceNames.GenericCmyk);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents a generic linear RGB color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateGenericRgbLinear ()
		{
			return Create (CGColorSpaceNames.GenericRgbLinear);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents an Adobe RGB (1998) color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateAdobeRgb1988 ()
		{
			return Create (CGColorSpaceNames.AdobeRgb1998);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents an sRGB color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateSrgb ()
		{
			return Create (CGColorSpaceNames.Srgb);
		}

		/// <summary>Creates and returns a generic Gray color space with a gamma value of 2.2.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateGenericGrayGamma2_2 ()
		{
			return Create (CGColorSpaceNames.GenericGrayGamma2_2);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents an device dependent CMYK color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateGenericXyz ()
		{
			return Create (CGColorSpaceNames.GenericXyz);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents an ACEScg color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateAcesCGLinear ()
		{
			return Create (CGColorSpaceNames.AcesCGLinear);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents an ITU-R BT.709 color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateItuR_709 ()
		{
			return Create (CGColorSpaceNames.ItuR_709);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents an ITU-R BT.2020 color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateItuR_2020 ()
		{
			return Create (CGColorSpaceNames.ItuR_2020);
		}

		/// <summary>Creates and returns a <see cref="CoreGraphics.CGColorSpace" /> that represents a ROMM RGB color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateRommRgb ()
		{
			return Create (CGColorSpaceNames.RommRgb);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceGetBaseColorSpace (/* CGColorSpaceRef */ IntPtr space);

		/// <summary>Tthe base colorspace.</summary>
		///         <returns />
		///         <remarks>To be added.</remarks>
		public CGColorSpace? GetBaseColorSpace ()
		{
			var h = CGColorSpaceGetBaseColorSpace (Handle);
			return FromHandle (h, false);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGColorSpaceModel CGColorSpaceGetModel (/* CGColorSpaceRef */ IntPtr space);

		/// <summary>The colorspace model.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CGColorSpaceModel Model {
			get {
				return CGColorSpaceGetModel (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGColorSpaceGetNumberOfComponents (/* CGColorSpaceRef */ IntPtr space);

		/// <summary>Number of components on this colorspace.</summary>
		///         <value />
		///         <remarks>To be added.</remarks>
		public nint Components {
			get {
				return CGColorSpaceGetNumberOfComponents (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGColorSpaceGetColorTableCount (/* CGColorSpaceRef */ IntPtr space);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorSpaceGetColorTable (/* CGColorSpaceRef */ IntPtr space, /* uint8_t* */ byte [] table);

		/// <summary>Return the entries of the color table used in an indexed color space</summary>
		///         <returns>An array of bytes with the same format that was provided to the CreateIndexed method</returns>
		///         <remarks>An empty array is returned if the Model is not CGColorSpaceModel.Indexed</remarks>
		public byte [] GetColorTable ()
		{
			nint n = CGColorSpaceGetColorTableCount (Handle);
			if (n == 0)
				return Array.Empty<byte> ();

			byte [] table = new byte [n * GetBaseColorSpace ()!.Components];
			CGColorSpaceGetColorTable (Handle, table);
			return table;
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.13", "Use 'CreateIDCCData' instead.")]
		[ObsoletedOSPlatform ("tvos11.0", "Use 'CreateIDCCData' instead.")]
		[ObsoletedOSPlatform ("ios11.0", "Use 'CreateIDCCData' instead.")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithICCProfile (/* CFDataRef */ IntPtr data);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithICCData (/* CFTypeRef cg_nullable */ IntPtr data);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.13", "Use 'CreateIDCCData' instead.")]
		[ObsoletedOSPlatform ("tvos11.0", "Use 'CreateIDCCData' instead.")]
		[ObsoletedOSPlatform ("ios11.0", "Use 'CreateIDCCData' instead.")]
		public static CGColorSpace? CreateIccProfile (NSData? data)
		{
			IntPtr ptr = CGColorSpaceCreateWithICCProfile (data.GetHandle ());
			GC.KeepAlive (data);
			return FromHandle (ptr, true);
		}

		/// <param name="data">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public static CGColorSpace? CreateIccData (NSData data)
		{
			CGColorSpace? result = CreateIccData (data.GetHandle ());
			GC.KeepAlive (data);
			return result;
		}

		/// <param name="provider">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public static CGColorSpace? CreateIccData (CGDataProvider provider)
		{
			CGColorSpace? result = CreateIccData (provider.GetHandle ());
			GC.KeepAlive (provider);
			return result;
		}

		static CGColorSpace? CreateIccData (IntPtr handle)
		{
			var ptr = CGColorSpaceCreateWithICCData (handle);
			return FromHandle (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateICCBased (/* size_t */ nint nComponents,
			/* const CGFloat* __nullable */ nfloat* range,
			/* CGDataProviderRef __nullable */ IntPtr profile,
			/* CGColorSpaceRef __nullable */ IntPtr alternate);

		public static CGColorSpace? CreateIccProfile (nfloat []? range, CGDataProvider profile, CGColorSpace alternate)
		{
			nint nComponents = range is null ? 0 : range.Length / 2;
			unsafe {
				fixed (nfloat* rangePtr = range) {
					var ptr = CGColorSpaceCreateICCBased (nComponents, rangePtr, profile.GetHandle (), alternate.GetHandle ());
					GC.KeepAlive (profile);
					GC.KeepAlive (alternate);
					return FromHandle (ptr, true);
				}
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.13", "Use 'GetICCData' instead.")]
		[ObsoletedOSPlatform ("tvos11.0", "Use 'GetICCData' instead.")]
		[ObsoletedOSPlatform ("ios11.0", "Use 'GetICCData' instead.")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFDataRef */ IntPtr CGColorSpaceCopyICCProfile (/* CGColorSpaceRef */ IntPtr space);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.13", "Use 'GetICCData' instead.")]
		[ObsoletedOSPlatform ("tvos11.0", "Use 'GetICCData' instead.")]
		[ObsoletedOSPlatform ("ios11.0", "Use 'GetICCData' instead.")]
		public NSData? GetIccProfile ()
		{
			IntPtr ptr = CGColorSpaceCopyICCProfile (Handle);
			return Runtime.GetNSObject<NSData> (ptr, true);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CFDataRef* */ IntPtr CGColorSpaceCopyICCData (/* CGColorSpaceRef */ IntPtr space);

		/// <summary>Gets the ICC data for the color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public NSData? GetIccData ()
		{
			IntPtr ptr = CGColorSpaceCopyICCData (Handle);
			return Runtime.GetNSObject<NSData> (ptr, true);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern unsafe /* CFStringRef* */ IntPtr CGColorSpaceCopyName (/* CGColorSpaceRef */ IntPtr space);

		/// <summary>Gets the color space name.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public string? Name {
			get {
				return CFString.FromHandle (CGColorSpaceCopyName (Handle), true);
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGColorSpaceIsWideGamutRGB (/* CGColorSpaceRef */ IntPtr space);

		/// <summary>Gets a value that tells whether the color space supports wide gamut colors.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool IsWideGamutRgb {
			get {
				return CGColorSpaceIsWideGamutRGB (Handle) != 0;
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGColorSpaceSupportsOutput (/* CGColorSpaceRef cg_nullable */ IntPtr space);

		/// <summary>Gets a Boolean value that tells whether the color space supports output.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool SupportsOutput {
			get {
				return CGColorSpaceSupportsOutput (Handle) != 0;
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCopyPropertyList (IntPtr space);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateWithPropertyList (IntPtr plist);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public CFPropertyList? ToPropertyList ()
		{
			var x = CGColorSpaceCopyPropertyList (Handle);
			if (x == IntPtr.Zero)
				return null;
			return new CFPropertyList (x, owns: true);
		}

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15.4")]
		[ObsoletedOSPlatform ("tvos13.4")]
		[ObsoletedOSPlatform ("ios13.4")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGColorSpaceIsHDR (/* CGColorSpaceRef */ IntPtr space);

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15.4")]
		[ObsoletedOSPlatform ("tvos13.4")]
		[ObsoletedOSPlatform ("ios13.4")]
		public bool IsHdr {
			get {
				return CGColorSpaceIsHDR (Handle) != 0;
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGColorSpaceUsesExtendedRange (/* CGColorSpaceRef gc_nullable */ IntPtr space);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool UsesExtendedRange {
			get {
				return CGColorSpaceUsesExtendedRange (Handle) != 0;
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGColorSpaceUsesITUR_2100TF (/* CGColorSpaceRef */ IntPtr space);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool UsesItur2100TF => CGColorSpaceUsesITUR_2100TF (Handle) != 0;

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateLinearized (/* CGColorSpaceRef gc_nullable */ IntPtr space);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		public CGColorSpace? CreateLinearized () => Runtime.GetINativeObject<CGColorSpace> (CGColorSpaceCreateLinearized (Handle), owns: true);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateExtended (/* CGColorSpaceRef gc_nullable */ IntPtr space);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		public CGColorSpace? CreateExtended () => Runtime.GetINativeObject<CGColorSpace> (CGColorSpaceCreateExtended (Handle), owns: true);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateExtendedLinearized (/* CGColorSpaceRef gc_nullable */ IntPtr space);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		public CGColorSpace? CreateExtendedLinearized () => Runtime.GetINativeObject<CGColorSpace> (CGColorSpaceCreateExtendedLinearized (Handle), owns: true);

		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateCopyWithStandardRange (/* CGColorSpaceRef cg_nullable */ IntPtr s);

		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		public CGColorSpace? CreateCopyWithStandardRange () => Runtime.GetINativeObject<CGColorSpace> (CGColorSpaceCreateCopyWithStandardRange (Handle), owns: true);

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGColorSpaceIsHLGBased (/* CGColorSpace */ IntPtr space);

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool IsHlgBased => CGColorSpaceIsHLGBased (Handle) != 0;

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern byte CGColorSpaceIsPQBased (/* CGColorSpace */ IntPtr space);

		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool IsPQBased => CGColorSpaceIsPQBased (Handle) != 0;

		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("tvos18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCopyBaseColorSpace (/* CGColorSpaceRef */ IntPtr space);

		/// <summary>Get the base color space, or a new color space without any image-specific metadata.</summary>
		/// <returns>If the current color space is a pattern or an indexed color space, get the base color space. If the current color space contains image-specific metadata associated with the gain map, get a new color space without the metadata. Otherwise returns a null value.</returns>
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("tvos18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		public CGColorSpace? CopyBaseColorSpace ()
		{
			var h = CGColorSpaceCopyBaseColorSpace (Handle);
			return FromHandle (h, true);
		}


#endif // !COREBUILD
	}
}
