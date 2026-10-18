//
// coreimage.cs: Definitions for CoreImage
//
// Copyright 2010, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc
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

using System.ComponentModel;
using System.Reflection;
using AVFoundation;
using CoreGraphics;
using CoreImage;
using CoreML;
using CoreVideo;
using ImageIO;
using IOSurface;
using Metal;
#if HAS_OPENGLES
using OpenGLES;
#endif
#if !MONOMAC
using UIKit;
#else
using AppKit;
using ImageKit;
#endif

#if !MONOMAC
using Color = UIKit.UIColor;
using NSImageRep = Foundation.NSObject;
using NSCompositingOperation = Foundation.NSObject;
using IKFilterUIView = Foundation.NSObject;
#else
using Color = AppKit.NSColor;
using UIImage = AppKit.NSImage;
#endif

namespace CoreImage {

	/// <summary>A Core Image color, including both color values and a reference to a color space.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/UIKit/Reference/UIColor_Class/index.html">Apple documentation for <c>CIColor</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIColor : NSSecureCoding, NSCopying {
		/// <param name="c">To be added.</param>
		///         <summary>Creates a <see cref="CoreImage.CIColor" /> from a <see cref="CoreGraphics.CGColor" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("colorWithCGColor:")]
		CIColor FromCGColor (CGColor c);

		/// <param name="red">To be added.</param>
		/// <param name="green">To be added.</param>
		/// <param name="blue">To be added.</param>
		/// <param name="alpha">To be added.</param>
		/// <summary>Creates a <see cref="CoreImage.CIColor" /> from the supplied <paramref name="red" />, <paramref name="green" />, <paramref name="blue" />, and <paramref name="alpha" /> values.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("colorWithRed:green:blue:alpha:")]
		CIColor FromRgba (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		/// <param name="red">To be added.</param>
		/// <param name="green">To be added.</param>
		/// <param name="blue">To be added.</param>
		/// <param name="alpha">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("colorWithRed:green:blue:alpha:colorSpace:")]
		[return: NullAllowed]
		CIColor FromRgba (nfloat red, nfloat green, nfloat blue, nfloat alpha, CGColorSpace colorSpace);

		/// <param name="red">To be added.</param>
		/// <param name="green">To be added.</param>
		/// <param name="blue">To be added.</param>
		/// <summary>Creates a <see cref="CoreImage.CIColor" /> from the supplied <paramref name="red" />, <paramref name="green" />, and <paramref name="blue" /> values.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("colorWithRed:green:blue:")]
		CIColor FromRgb (nfloat red, nfloat green, nfloat blue);

		/// <param name="red">To be added.</param>
		/// <param name="green">To be added.</param>
		/// <param name="blue">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("colorWithRed:green:blue:colorSpace:")]
		[return: NullAllowed]
		CIColor FromRgb (nfloat red, nfloat green, nfloat blue, CGColorSpace colorSpace);

		/// <param name="representation">To be added.</param>
		///         <summary>Creates a <see cref="CoreImage.CIColor" /> from a string of the format "R G B A".</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("colorWithString:")]
		CIColor FromString (string representation);

		/// <param name="c">To be added.</param>
		/// <summary>Creates a new CIColor with the specified color.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithCGColor:")]
		NativeHandle Constructor (CGColor c);

		/// <param name="red">To be added.</param>
		/// <param name="green">To be added.</param>
		/// <param name="blue">To be added.</param>
		/// <summary>Creates a new CIColor from the specified color components.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithRed:green:blue:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue);

		/// <param name="red">To be added.</param>
		/// <param name="green">To be added.</param>
		/// <param name="blue">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithRed:green:blue:colorSpace:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue, CGColorSpace colorSpace);

		/// <param name="red">To be added.</param>
		/// <param name="green">To be added.</param>
		/// <param name="blue">To be added.</param>
		/// <param name="alpha">To be added.</param>
		/// <summary>Creates a new CIColor from the specified color components.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithRed:green:blue:alpha:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		/// <param name="red">To be added.</param>
		/// <param name="green">To be added.</param>
		/// <param name="blue">To be added.</param>
		/// <param name="alpha">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithRed:green:blue:alpha:colorSpace:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha, CGColorSpace colorSpace);

		/// <summary>Gets the number of components. This is also the number of components that are returned in the <see cref="CoreImage.CIColor.Components" /> property.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("numberOfComponents")]
		nint NumberOfComponents { get; }

		[Internal, Export ("components")]
		IntPtr GetComponents ();

		/// <summary>Gets the alpha channel value.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("alpha")]
		nfloat Alpha { get; }

		/// <summary>Gets the color space for the color.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("colorSpace")]
		CGColorSpace ColorSpace { get; }

		/// <summary>Gets the red channel value.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("red")]
		nfloat Red { get; }

		/// <summary>Gets the green channel value.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("green")]
		nfloat Green { get; }

		/// <summary>Gets the blue channel value.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("blue")]
		nfloat Blue { get; }

		// the above are the component of a CIColor instance
		// while the one below are CIColor convenience helpers
		// conflicting names means we have to keep the *Color suffix on them

		/// <summary>A <see cref="CoreImage.CIColor" /> with RGBA values of [0.0, 0.0, 0.0, 1.0].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("blackColor", ArgumentSemantic.Strong)]
		CIColor BlackColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> object whose RGBA values are [1, 1, 1, 1].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("whiteColor", ArgumentSemantic.Strong)]
		CIColor WhiteColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> whose RGBA values are (0.5, 0.5, 0.5, 1.0).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("grayColor", ArgumentSemantic.Strong)]
		CIColor GrayColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> object whose RGBA values are [1, 0, 0, 1].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("redColor", ArgumentSemantic.Strong)]
		CIColor RedColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> whose RGBA values are [0, 1, 0, 1].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("greenColor", ArgumentSemantic.Strong)]
		CIColor GreenColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> whose RGBA values are [0, 0, 1, 1].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("blueColor", ArgumentSemantic.Strong)]
		CIColor BlueColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> whose RGBA values are [0, 1, 1, 1].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("cyanColor", ArgumentSemantic.Strong)]
		CIColor CyanColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> whose RGBA values are [1, 0, 1, 1].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("magentaColor", ArgumentSemantic.Strong)]
		CIColor MagentaColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> objects whose RGBA values are [1, 1, 0, 1].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("yellowColor", ArgumentSemantic.Strong)]
		CIColor YellowColor { get; }

		/// <summary>A <see cref="CoreImage.CIColor" /> whose values are [0, 0, 0, 0].</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("clearColor", ArgumentSemantic.Strong)]
		CIColor ClearColor { get; }

		/// <summary>Returns a string representation of the color, in the format "R G B [A]".</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("stringRepresentation")]
		string StringRepresentation ();

		/// <param name="color">To be added.</param>
		/// <summary>Creates a new CIColor with the specified color.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithColor:")]
		NativeHandle Constructor (Color color);
	}

	/// <summary>Orchestrates the rendering of a CIFilter pipeline.</summary>
	///     <remarks>
	///       <para>
	/// 	The CIContext class is used to orchestrate the rendering of a
	/// 	pipeline of filters into one of the supported output surfaces.
	///       </para>
	///       <para>
	/// 	The CGContext can render its results into a CoreGraphics
	/// 	CGImage, render directly into the screen with one of the
	/// 	various Draw methods, into a CoreVideo CVPixelBuffer or into a CoreGraphics context.
	///       </para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/QuartzCoreFramework/Classes/CIContext_Class/index.html">Apple documentation for <c>CIContext</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIContext {
		// marked iOS5 but it's not working in iOS8.0
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("init")]
		NativeHandle Constructor ();

		/// <param name="device">To be added.</param>
		///         <summary>Creates a new CIContext from the provided Metal device.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contextWithMTLDevice:")]
		CIContext FromMetalDevice (IMTLDevice device);

		[MacCatalyst (13, 1)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Static]
		[Export ("contextWithMTLDevice:options:")]
		CIContext FromMetalDevice (IMTLDevice device, [NullAllowed] NSDictionary options);

		/// <summary>Creates a new <see cref="CoreImage.CIContext" /> from the provided Metal <paramref name="device" />, applying the specified options.</summary>
		/// <param name="device">The source <see cref="Metal.IMTLDevice" />.</param>
		/// <param name="options">The desired options for the new <see cref="CIContext" />.</param>
		/// <returns>A new <see cref="CoreImage.CIContext" />.</returns>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contextWithMTLDevice:options:")]
		CIContext FromMetalDevice (IMTLDevice device, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[MacCatalyst (13, 1)]
		[Internal, Static]
		[Export ("contextWithCGContext:options:")]
		CIContext FromContext (CGContext ctx, [NullAllowed] NSDictionary options);

		[Static, Internal]
		[Export ("contextWithOptions:")]
		CIContext FromOptions ([NullAllowed] NSDictionary dictionary);

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("initWithOptions:")]
		NativeHandle Constructor ([NullAllowed] NSDictionary options);

		/// <summary>Creates a new <see cref="CoreImage.CIContext" /> with default options.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("context")]
		CIContext Create ();

#if HAS_OPENGLES
		/// <param name="eaglContext">To be added.</param>
		/// <summary>Creates a new <see cref="CoreImage.CIContext" /> from the provided <paramref name="eaglContext" />.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Static]
		[Export ("contextWithEAGLContext:")]
		CIContext FromContext (EAGLContext eaglContext);

		/// <param name="eaglContext">To be added.</param>
		/// <param name="dictionary">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Creates a new <see cref="CoreImage.CIContext" /> from the provided <paramref name="eaglContext" /> and by using the options that are named in <paramref name="dictionary" />.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Static]
		[Export ("contextWithEAGLContext:options:")]
		CIContext FromContext (EAGLContext eaglContext, [NullAllowed] NSDictionary dictionary);
#endif

		/// <param name="image">To be added.</param>
		///         <param name="buffer">To be added.</param>
		///         <summary>Renders <paramref name="image" /> to <paramref name="buffer" />.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("render:toCVPixelBuffer:")]
		void Render (CIImage image, CVPixelBuffer buffer);

		/// <param name="image">To be added.</param>
		///         <param name="buffer">To be added.</param>
		///         <param name="rectangle">To be added.</param>
		///         <param name="cs">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("render:toCVPixelBuffer:bounds:colorSpace:")]
		// null is not documented for CGColorSpace but it makes sense with the other overload not having this parameter (unit tested)
		void Render (CIImage image, CVPixelBuffer buffer, CGRect rectangle, [NullAllowed] CGColorSpace cs);

		/// <param name="image">To be added.</param>
		///         <param name="surface">To be added.</param>
		///         <param name="bounds">To be added.</param>
		///         <param name="colorSpace">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("render:toIOSurface:bounds:colorSpace:")]
		void Render (CIImage image, IOSurface.IOSurface surface, CGRect bounds, [NullAllowed] CGColorSpace colorSpace);

		/// <summary>Gets the maximum allowed input image size.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputImageMaximumSize")]
		CGSize InputImageMaximumSize { get; }

		/// <summary>Gets the maximum allowed output image size.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("outputImageMaximumSize")]
		CGSize OutputImageMaximumSize { get; }

		/// <param name="image">To be added.</param>
		///         <param name="texture">To be added.</param>
		///         <param name="commandBuffer">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <param name="bounds">To be added.</param>
		///         <param name="colorSpace">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("render:toMTLTexture:commandBuffer:bounds:colorSpace:")]
		void Render (CIImage image, IMTLTexture texture, [NullAllowed] IMTLCommandBuffer commandBuffer, CGRect bounds, CGColorSpace colorSpace);

		/// <param name="image">To be added.</param>
		///         <param name="atPoint">To be added.</param>
		///         <param name="fromRect">To be added.</param>
		///         <summary>Developers should not use this deprecated method. Developers should use 'DrawImage (image, CGRect, CGRect)' instead.</summary>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DrawImage (image, CGRect, CGRect)' instead.")]
		[Export ("drawImage:atPoint:fromRect:")]
		void DrawImage (CIImage image, CGPoint atPoint, CGRect fromRect);

		/// <param name="image">The image to draw.</param>
		///         <param name="inRectangle">The rectangle where to draw the image.</param>
		///         <param name="fromRectangle">The rectangle of the image to draw.</param>
		///         <summary>Draws the <paramref name="fromRectangle" /> portion of <paramref name="image" /> into the rectangle specified by <paramref name="inRectangle" />.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("drawImage:inRect:fromRect:")]
		void DrawImage (CIImage image, CGRect inRectangle, CGRect fromRectangle);

		/// <param name="image">To be added.</param>
		///         <param name="fromRectangle">To be added.</param>
		///         <summary>Creates a new <see cref="CoreGraphics.CGImage" /> from the <paramref name="fromRectangle" /> region of <paramref name="image" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("createCGImage:fromRect:")]
		[return: Release ()]
		[return: NullAllowed]
		CGImage CreateCGImage (CIImage image, CGRect fromRectangle);

		/// <param name="image">To be added.</param>
		///         <param name="fromRect">To be added.</param>
		///         <param name="ciImageFormat">To be added.</param>
		///         <param name="colorSpace">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("createCGImage:fromRect:format:colorSpace:")]
		[return: Release ()]
		[return: NullAllowed]
		CGImage CreateCGImage (CIImage image, CGRect fromRect, int /* CIFormat = int */ ciImageFormat, [NullAllowed] CGColorSpace colorSpace);

		/// <param name="image">To be added.</param>
		///         <param name="fromRect">To be added.</param>
		///         <param name="format">To be added.</param>
		///         <param name="colorSpace">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <param name="deferred">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("createCGImage:fromRect:format:colorSpace:deferred:")]
		[return: Release]
		[return: NullAllowed]
		CGImage CreateCGImage (CIImage image, CGRect fromRect, CIFormat format, [NullAllowed] CGColorSpace colorSpace, bool deferred);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Internal, Export ("createCGLayerWithSize:info:")]
		[return: NullAllowed]
		CGLayer CreateCGLayer (CGSize size, [NullAllowed] NSDictionary info);

		/// <param name="image">To be added.</param>
		/// <param name="bitmapPtr">To be added.</param>
		/// <param name="bytesPerRow">To be added.</param>
		/// <param name="bounds">To be added.</param>
		/// <param name="bitmapFormat">To be added.</param>
		/// <param name="colorSpace">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("render:toBitmap:rowBytes:bounds:format:colorSpace:")]
		void RenderToBitmap (CIImage image, IntPtr bitmapPtr, nint bytesPerRow, CGRect bounds, int /* CIFormat = int */ bitmapFormat, [NullAllowed] CGColorSpace colorSpace);

		//[Export ("render:toIOSurface:bounds:colorSpace:")]
		//void RendertoIOSurfaceboundscolorSpace (CIImage im, IOSurfaceRef surface, CGRect r, CGColorSpaceRef cs, );

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("reclaimResources")]
		void ReclaimResources ();

		/// <summary>Frees data in the cache and runs the garbage collector.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("clearCaches")]
		void ClearCaches ();

#if !XAMCORE_5_0
		[Obsolete ("Use 'CIContextOptions.MemoryLimit' instead.")]
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIContextMemoryLimit")]
		NSString MemoryLimit { get; }
#endif // !XAMCORE_5_0

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("offlineGPUCount")]
		[Static]
		int OfflineGPUCount { get; }

		/// <param name="gpuIndex">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Export ("contextForOfflineGPUAtIndex:")]
		[Static]
		[return: NullAllowed]
		CIContext FromOfflineGpu (int gpuIndex);

		/// <summary>The working <see cref="CoreGraphics.CGColorSpace" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("workingColorSpace")]
		[NullAllowed]
		CGColorSpace WorkingColorSpace { get; }

		/// <summary>The working pixel format.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("workingFormat")]
		CIFormat WorkingFormat { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contextWithMTLCommandQueue:")]
		CIContext Create (IMTLCommandQueue commandQueue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contextWithMTLCommandQueue:options:")]
		CIContext Create (IMTLCommandQueue commandQueue, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("writeOpenEXRRepresentationOfImage:toURL:options:error:")]
		bool WriteOpenExrRepresentation (CIImage image, NSUrl url, NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError errorPtr);

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("OpenEXRRepresentationOfImage:options:error:")]
		[return: NullAllowed]
		NSData GetOpenEXRRepresentation (CIImage image, NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError errorPtr);

		// From the createCGImage (CIContext) category
		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Export ("createCGImage:fromRect:format:colorSpace:deferred:calculateHDRStats:")]
		[return: NullAllowed]
		CGImage CreateCGImage (CIImage image, CGRect fromRect, int format, [NullAllowed] CGColorSpace colorSpace, bool deferred, bool calculateHdrStats);

		// From the CalculateHDRStats (CIContext) category
		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Export ("calculateHDRStatsForIOSurface:")]
		void CalculateHdrStats (IOSurface.IOSurface surface);

		// From the CalculateHDRStats (CIContext) category
		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Export ("calculateHDRStatsForCVPixelBuffer:")]
		void CalculateHdrStats (CVPixelBuffer buffer);

		// From the CalculateHDRStats (CIContext) category
		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Export ("calculateHDRStatsForCGImage:")]
		CGImage CalculateHdrStats (CGImage cgimage);

		// From the CalculateHDRStats (CIContext) category
		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Export ("calculateHDRStatsForImage:")]
		[return: NullAllowed]
		CIImage CalculateHdrStats (CIImage image);
	}

	[Internal]
	[Static]
	interface CIContextOptionKeys {
		[Field ("kCIContextOutputColorSpace", "+CoreImage")]
		NSString OutputColorSpace { get; }

		[Field ("kCIContextWorkingColorSpace", "+CoreImage")]
		NSString WorkingColorSpace { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIContextWorkingFormat", "+CoreImage")]
		NSString WorkingFormatField { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIContextHighQualityDownsample", "+CoreImage")]
		NSString HighQualityDownsample { get; }

		[Field ("kCIContextOutputPremultiplied", "+CoreImage")]
		NSString OutputPremultiplied { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIContextCacheIntermediates", "+CoreImage")]
		NSString CacheIntermediates { get; }

		[Field ("kCIContextUseSoftwareRenderer", "+CoreImage")]
		NSString UseSoftwareRenderer { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIContextPriorityRequestLow", "+CoreImage")]
		NSString PriorityRequestLow { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIContextAllowLowPower")]
		NSString AllowLowPower { get; }

		[iOS (14, 0)]
		[TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("kCIContextName")]
		NSString Name { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIContextMemoryLimit")]
		NSString MemoryLimit { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIContextCVMetalTextureCache")]
		NSString CVMetalTextureCache { get; }
	}

	[StrongDictionary ("CIContextOptionKeys", Suffix = "")]
	interface CIContextOptions {
		/// <summary>The desired CIColorSpace to be used for the CIContext rendering operation.</summary>
		/// <remarks>This color space is used before the image is rendered into the output.</remarks>
		CGColorSpace OutputColorSpace { get; set; }

		/// <summary>The colorspace used by image processing operations, this is different than the colorspace used for the final rendering.</summary>
		CGColorSpace WorkingColorSpace { get; set; }

		/// <summary>Gets or sets the image format to use for storing intermediate rendering results.</summary>
		CIFormat WorkingFormatField { get; set; }

		/// <summary><see langword="true" /> if downsampling should be higher quality at the expense of performance.</summary>
		bool HighQualityDownsample { get; set; }

		/// <summary>If <see langword="true" />, the output should premultiply pixel values by their alpha values.</summary>
		bool OutputPremultiplied { get; set; }

		/// <summary>If not <see langword="null" />, <see langword="true" /> indicates that intermediate images should be cached.</summary>
		bool CacheIntermediates { get; set; }

#if !XAMCORE_6_0
#if __MACOS__
		/// <include file="../docs/api/CoreImage/CIContextOptions.xml" path="/Documentation/Docs[@DocId='macOS:P:CoreImage.CIContextOptions.UseSoftwareRenderer']/*" />
#else
		/// <include file="../docs/api/CoreImage/CIContextOptions.xml" path="/Documentation/Docs[@DocId='P:CoreImage.CIContextOptions.UseSoftwareRenderer']/*" />
#endif
		[Export ("UseSoftwareRenderer")]
#if XAMCORE_5_0
		[Obsolete ("Use 'UseSoftwareRenderer' instead.")]
#endif
		bool NullableUseSoftwareRenderer { get; }
#endif // !XAMCORE_6_0

#if XAMCORE_5_0
#if __MACOS__
		/// <include file="../docs/api/CoreImage/CIContextOptions.xml" path="/Documentation/Docs[@DocId='macOS:P:CoreImage.CIContextOptions.UseSoftwareRenderer']/*" />
#else
		/// <include file="../docs/api/CoreImage/CIContextOptions.xml" path="/Documentation/Docs[@DocId='P:CoreImage.CIContextOptions.UseSoftwareRenderer']/*" />
#endif
		[Export ("UseSoftwareRenderer")]
		bool UseSoftwareRenderer { get; }
#endif // XAMCORE_5_0

		/// <summary>Gets or sets whether to request low priority from the GPU.</summary>
		bool PriorityRequestLow { get; set; }

		[MacCatalyst (13, 1)]
		bool AllowLowPower { get; set; }

		[iOS (14, 0), TV (14, 0), MacCatalyst (14, 0)]
		string Name { get; set; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIContextMemoryLimit")]
		NSString MemoryLimit { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIContextCVMetalTextureCache")]
		NSString CVMetalTextureCache { get; }
	}
	/// <summary>Extension methods for <see cref="CoreImage.CIContext" /> that can generate common image formats.</summary>
	[Category]
	[BaseType (typeof (CIContext))]
	interface CIContext_ImageRepresentation {

		/// <param name="image">The image input to be processed.</param>
		/// <param name="format">The desired pixel format.</param>
		/// <param name="colorSpace">The color space to be used.</param>
		/// <param name="options">Processing arguments.</param>
		/// <summary>Applies the processing of this context to the <paramref name="image" /> and returns a TIFF image of the result.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("TIFFRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetTiffRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		/// <param name="image">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("GetTiffRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetTiffRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		/// <param name="image">The image input to be processed.</param>
		/// <param name="colorSpace">The color space to be used.</param>
		/// <param name="options">Processing arguments.</param>
		/// <summary>Applies the processing of this context to the <paramref name="image" /> and returns a JPEG image of the result.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("JPEGRepresentationOfImage:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetJpegRepresentation (CIImage image, CGColorSpace colorSpace, NSDictionary options);

		/// <param name="image">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("GetJpegRepresentation (This, image, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetJpegRepresentation (CIImage image, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		/// <param name="image">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("HEIFRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetHeifRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		/// <param name="image">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("GetHeifRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetHeifRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("HEIF10RepresentationOfImage:colorSpace:options:error:")]
		[return: NullAllowed]
		NSData GetHeif10Representation (CIImage image, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Wrap ("GetHeif10Representation (This, image, colorSpace, options.GetDictionary ()!, out error)")]
		[return: NullAllowed]
		NSData GetHeif10Representation (CIImage image, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("PNGRepresentationOfImage:format:colorSpace:options:")]
		[return: NullAllowed]
		NSData GetPngRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, NSDictionary options);

		/// <param name="image">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("GetPngRepresentation (This, image, format, colorSpace, options.GetDictionary ()!)")]
		[return: NullAllowed]
		NSData GetPngRepresentation (CIImage image, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options);

		/// <param name="image">The image input to be processed.</param>
		/// <param name="url">To be added.</param>
		/// <param name="format">The desired pixel format.</param>
		/// <param name="colorSpace">The color space to be used.</param>
		/// <param name="options">Processing arguments.</param>
		/// <param name="error">If not <see langword="null" />, error that occurred during processing.</param>
		/// <summary>Applies the processing of this context to the <paramref name="image" /> and writes a TIFF image of the result to <paramref name="url" />.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("writeTIFFRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WriteTiffRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="url">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("WriteTiffRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteTiffRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, out NSError error);

		/// <param name="image">The image input to be processed.</param>
		/// <param name="url">The file URL to which the image should be written.</param>
		/// <param name="colorSpace">The color space to be used.</param>
		/// <param name="options">Processing arguments.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Applies the processing of this context to the <paramref name="image" /> and writes a JPEG image of the result to <paramref name="url" />.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("writeJPEGRepresentationOfImage:toURL:colorSpace:options:error:")]
		bool WriteJpegRepresentation (CIImage image, NSUrl url, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="url">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("WriteJpegRepresentation (This, image, url, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteJpegRepresentation (CIImage image, NSUrl url, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="url">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("writeHEIFRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WriteHeifRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="url">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("WriteHeifRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteHeifRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("writeHEIF10RepresentationOfImage:toURL:colorSpace:options:error:")]
		bool WriteHeif10Representation (CIImage image, NSUrl url, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Wrap ("WriteHeif10Representation (This, image, url, colorSpace, options.GetDictionary ()!, out error)")]
		bool WriteHeif10Representation (CIImage image, NSUrl url, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="url">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("writePNGRepresentationOfImage:toURL:format:colorSpace:options:error:")]
		bool WritePngRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, NSDictionary options, [NullAllowed] out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="url">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("WritePngRepresentation (This, image, url, format, colorSpace, options.GetDictionary ()!, out error)")]
		bool WritePngRepresentation (CIImage image, NSUrl url, CIFormat format, CGColorSpace colorSpace, CIImageRepresentationOptions options, [NullAllowed] out NSError error);
	}


	[Category]
	[BaseType (typeof (CIContext))]
	interface CIContext_CIDepthBlurEffect {
		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		/// <param name="url">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("depthBlurEffectFilterForImageURL:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (NSUrl url, [NullAllowed] NSDictionary options);

		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		/// <param name="data">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("depthBlurEffectFilterForImageData:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (NSData data, [NullAllowed] NSDictionary options);

		// as per the docs: The 'options' parameter is a key value/pair reserved for future use.
		/// <param name="image">To be added.</param>
		/// <param name="disparityImage">To be added.</param>
		/// <param name="portraitEffectsMatte">To be added.</param>
		/// <param name="orientation">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("depthBlurEffectFilterForImage:disparityImage:portraitEffectsMatte:orientation:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (CIImage image, CIImage disparityImage, [NullAllowed] CIImage portraitEffectsMatte, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Export ("depthBlurEffectFilterForImage:disparityImage:portraitEffectsMatte:hairSemanticSegmentation:orientation:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (CIImage image, CIImage disparityImage, [NullAllowed] CIImage portraitEffectsMatte, [NullAllowed] CIImage hairSemanticSegmentation, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary options);

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Export ("depthBlurEffectFilterForImage:disparityImage:portraitEffectsMatte:hairSemanticSegmentation:glassesMatte:gainMap:orientation:options:")]
		[return: NullAllowed]
		CIFilter GetDepthBlurEffectFilter (CIImage image, CIImage disparityImage, [NullAllowed] CIImage portraitEffectsMatte, [NullAllowed] CIImage hairSemanticSegmentation, [NullAllowed] CIImage glassesMatte, [NullAllowed] CIImage gainMap, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary options);
	}

	/// <include file="../docs/api/CoreImage/CIFilter.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIFilter']/*" />
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] //  In iOS8 they expose custom filters, we expose a protected one in CIFilter.cs
	interface CIFilter : NSSecureCoding, NSCopying {
		/// <summary>Gets an array that contains the names of the input keys for the filter.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("inputKeys")]
		string [] InputKeys { get; }

		/// <summary>Gets an array that contains the names of the output keys for the filter.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("outputKeys")]
		string [] OutputKeys { get; }

		/// <summary>Sets all input values to their defaults.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("setDefaults")]
		void SetDefaults ();

		/// <summary>Configurable attributes of the filter.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>
		///             Each filter has a set of attributes that control it.  You can
		///             get this list by requesting the Attributes dictionary from the
		///             filter. Each filter, in addition to having a series of
		///             configurable properties always includes its name
		///             ("CIAttributeFilterName" key), a user friendly name
		///             ("CIAttributeFilterDisplayName" key) and a list of categories
		///             that this filter belongs to ("CIAttributeFilterCategories").   
		///
		///           </para>
		///         </remarks>
		[Export ("attributes")]
		NSDictionary Attributes { get; }

		/// <summary>Gets the filter name.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("name")]
		string Name {
			get;
			[MacCatalyst (13, 1)]
			set;
		}

		/// <include file="../docs/api/CoreImage/CIFilter.xml" path="/Documentation/Docs[@DocId='M:CoreImage.CIFilter.FromName(System.String)']/*" />
		[Static]
		[Export ("filterWithName:")]
		[return: NullAllowed]
		CIFilter FromName (string name);

		/// <param name="name">To be added.</param>
		///         <param name="inputParameters">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Returns a <see cref="CoreImage.CIFilter" /> that corresponds to <paramref name="name" /> and is initialized with the parameters that are named in <paramref name="inputParameters" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("filterWithName:withInputParameters:")]
		[return: NullAllowed]
		CIFilter GetFilter (string name, [NullAllowed] NSDictionary inputParameters);

		/// <param name="category">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Returns an array of strings that specifies the filters that the system provides for the specified <paramref name="category" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("filterNamesInCategory:")]
		string [] FilterNamesInCategory ([NullAllowed] string category);

		[Static]
		[Export ("filterNamesInCategories:"), Internal]
		string [] _FilterNamesInCategories ([NullAllowed] string [] categories);

		/// <param name="filterName">To be added.</param>
		///         <summary>Gets the localized name for the specified filter name.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedNameForFilterName:")]
		[return: NullAllowed]
		string FilterLocalizedName (string filterName);

		/// <param name="category">To be added.</param>
		///         <summary>Returns the localized name for the specified category.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedNameForCategory:")]
		string CategoryLocalizedName (string category);

		/// <param name="filterName">To be added.</param>
		///         <summary>Gets the localized description for the specified filter name.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedDescriptionForFilterName:")]
		[return: NullAllowed]
		string FilterLocalizedDescription (string filterName);

		/// <param name="filterName">To be added.</param>
		///         <summary>Gets the localized reference documentation for the specified filter name.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedReferenceDocumentationForFilterName:")]
		[return: NullAllowed]
		NSUrl FilterLocalizedReferenceDocumentation (string filterName);

		/// <param name="name">An identifier for the filter type.</param>
		/// <param name="constructorObject">The <see cref="CoreImage.ICIFilterConstructor" /> factory.</param>
		/// <param name="classAttributes">The filter attributes.</param>
		/// <summary>Registers the filter generated by the <paramref name="constructorObject" /> factory.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("registerFilterName:constructor:classAttributes:")]
		void RegisterFilterName (string name, ICIFilterConstructor constructorObject, NSDictionary<NSString, NSObject> classAttributes);

		/// <param name="k">To be added.</param>
		///         <param name="args">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("apply:arguments:options:")]
		[return: NullAllowed]
		CIImage Apply (CIKernel k, [NullAllowed] NSArray args, [NullAllowed] NSDictionary options);

		/// <param name="configurationOptions">To be added.</param>
		///         <param name="excludedKeys">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("viewForUIConfiguration:excludedKeys:")]
		IKFilterUIView GetFilterUIView (NSDictionary configurationOptions, [NullAllowed] NSArray excludedKeys);

		/// <summary>Gets the image that results from applying the filter.</summary>
		[MacCatalyst (13, 1)]
		[Export ("outputImage")]
		[NullAllowed]
		CIImage OutputImage { get; }

		/// <param name="filters">To be added.</param>
		///         <param name="extent">To be added.</param>
		///         <summary>Returns a list of filters for an extent as serialized XMP data.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("serializedXMPFromFilters:inputImageExtent:"), Static]
		[return: NullAllowed]
		NSData SerializedXMP (CIFilter [] filters, CGRect extent);

		/// <param name="xmpData">To be added.</param>
		///         <param name="extent">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("filterArrayFromSerializedXMP:inputImageExtent:error:"), Static]
		CIFilter [] FromSerializedXMP (NSData xmpData, CGRect extent, out NSError error);

		[Export ("setValue:forKey:"), Internal]
		void SetValueForKey ([NullAllowed] NSObject value, IntPtr key);

		[Export ("valueForKey:"), Internal]
		[return: NullAllowed]
		NSObject ValueForKey (IntPtr key);

		// CIRAWFilter (CIFilter)

		/// <param name="url">The URL from which the RAW image data can be read.</param>
		///         <param name="options">The RAW processing options.</param>
		///         <summary>Creates a <see cref="CoreImage.CIFilter" /> that applies the <paramref name="options" /> to the RAW data read from <paramref name="url" />.</summary>
		///         <returns>A RAW processing filter.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Export ("filterWithImageURL:options:")]
		CIFilter CreateRawFilter (NSUrl url, NSDictionary options);

		/// <param name="url">The URL from which the RAW image data can be read.</param>
		///         <param name="options">The RAW processing options.</param>
		///         <summary>Creates a <see cref="CoreImage.CIFilter" /> that applies the <paramref name="options" /> to the RAW data read from <paramref name="url" />.</summary>
		///         <returns>A RAW processing filter.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Wrap ("CreateRawFilter (url, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (NSUrl url, CIRawFilterOptions options);

		/// <param name="data">The RAW image data.</param>
		///         <param name="options">The RAW processing options.</param>
		///         <summary>Creates a <see cref="CoreImage.CIFilter" /> that applies the <paramref name="options" /> to the RAW data in <paramref name="data" />.</summary>
		///         <returns>A RAW processing filter.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Export ("filterWithImageData:options:")]
		CIFilter CreateRawFilter (NSData data, NSDictionary options);

		/// <param name="data">The RAW image data.</param>
		///         <param name="options">The RAW processing options.</param>
		///         <summary>Creates a <see cref="CoreImage.CIFilter" /> that applies the <paramref name="options" /> to the RAW data in <paramref name="data" />.</summary>
		///         <returns>A RAW processing filter.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Wrap ("CreateRawFilter (data, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (NSData data, CIRawFilterOptions options);

		/// <param name="pixelBuffer">The <see cref="CoreVideo.CVPixelBuffer" /> containing RAW data.</param>
		///         <param name="properties">A dictionary of image data.</param>
		///         <param name="options">The set of RAW processing options to be applied to the input image(s).</param>
		///         <summary>Creates a RAW processing filter for converting the data in <paramref name="pixelBuffer" /> by applying the settings in <paramref name="options" />.</summary>
		///         <returns>A RAW processing filter.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Export ("filterWithCVPixelBuffer:properties:options:")]
		CIFilter CreateRawFilter (CVPixelBuffer pixelBuffer, NSDictionary properties, NSDictionary options);

		/// <param name="pixelBuffer">The <see cref="CoreVideo.CVPixelBuffer" /> containing RAW data.</param>
		///         <param name="properties">A dictionary of image data.</param>
		///         <param name="options">The set of RAW processing options to be applied to the input image(s).</param>
		///         <summary>Creates a RAW processing filter for converting the data in <paramref name="pixelBuffer" /> by applying the settings in <paramref name="options" />.</summary>
		///         <returns>A RAW processing filter.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
		[Static]
		[Wrap ("CreateRawFilter (pixelBuffer, properties, options.GetDictionary ()!)")]
		CIFilter CreateRawFilter (CVPixelBuffer pixelBuffer, NSDictionary properties, CIRawFilterOptions options);

		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("blurredRectangleGeneratorFilter")]
		CIBlurredRectangleGenerator BlurredRectangleGeneratorFilter { get; }

		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("cannyEdgeDetectorFilter")]
		CICannyEdgeDetector CannyEdgeDetectorFilter { get; }

		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("roundedRectangleStrokeGeneratorFilter")]
		CIRoundedRectangleStrokeGenerator RoundedRectangleStrokeGeneratorFilter { get; }

		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("sobelGradientsFilter")]
		CISobelGradients SobelGradientsFilter { get; }
	}

	[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
	[NativeName ("CIRAWDecoderVersion")]
	public enum CIRawDecoderVersion : long {

		[Field ("CIRAWDecoderVersionNone")]
		VersionNone = 0,

		[iOS (26, 0), MacCatalyst (26, 0), TV (26, 0), Mac (26, 0)]
		[Field ("CIRAWDecoderVersion9")]
		Version9 = 7,

		[iOS (26, 0), MacCatalyst (26, 0), TV (26, 0), Mac (26, 0)]
		[Field ("CIRAWDecoderVersion9DNG")]
		Version9Dng = 8,

		[Field ("CIRAWDecoderVersion8")]
		Version8 = 1,

		[Field ("CIRAWDecoderVersion8DNG")]
		Version8Dng = 2,

		[Field ("CIRAWDecoderVersion7")]
		Version7 = 3,

		[Field ("CIRAWDecoderVersion7DNG")]
		Version7Dng = 4,

		[Field ("CIRAWDecoderVersion6")]
		Version6 = 5,

		[Field ("CIRAWDecoderVersion6DNG")]
		Version6Dng = 6,
	}

	/// <summary>Handler invoked when Core Image finishes downloading resources for a RAW filter.</summary>
	/// <param name="error">The error if the download failed or timed out; otherwise, <see langword="null" />.</param>
	delegate void CIRawFilterDownloadResourcesCompletionHandler ([NullAllowed] NSError error);

	[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
	[BaseType (typeof (CIFilter), Name = "CIRAWFilter")]
	interface CIRawFilter : CIFilterProtocol {
		[Static]
		[Export ("supportedCameraModels")]
		string [] SupportedCameraModels { get; }

		/// <param name="version">The RAW decoder version.</param>
		/// <summary>Gets the camera models supported by the specified RAW decoder version.</summary>
		/// <returns>The names of the supported camera models.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Static]
		[Export ("supportedCameraModelsWithVersion:")]
		string [] GetSupportedCameraModels ([BindAs (typeof (CIRawDecoderVersion))] NSString version);

		/// <summary>Downloads on-demand resources required to decode the filter's image.</summary>
		/// <param name="timeout">The maximum time, in seconds, to wait for the download.</param>
		/// <param name="completionHandler">The handler to invoke when the download finishes.</param>
		/// <returns>An object that tracks and can cancel the download.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Async (XmlDocs = """
			<summary>Asynchronously downloads on-demand resources required to decode the filter's image.</summary>
			<returns>A task that represents the asynchronous download operation.</returns>
			""")]
		[Export ("downloadResourcesWithTimeout:completionHandler:")]
		NSProgress DownloadResources (double timeout, CIRawFilterDownloadResourcesCompletionHandler completionHandler);

		/// <summary>Downloads all on-demand RAW demosaicing and denoising models that aren't already installed.</summary>
		/// <param name="timeout">The maximum time, in seconds, to wait for the download.</param>
		/// <param name="completionHandler">The handler called on a queue chosen by Core Image when the download finishes or fails.</param>
		/// <returns>An object that tracks and can cancel the download.</returns>
		[TV (27, 2), Mac (27, 2), iOS (27, 2), MacCatalyst (27, 2)]
		[Static]
		[Async (XmlDocs = """
			<summary>Asynchronously downloads all on-demand RAW demosaicing and denoising models that aren't already installed.</summary>
			<param name="timeout">The maximum time, in seconds, to wait for the download.</param>
			<returns>A task that represents the asynchronous download operation.</returns>
			""",
			XmlDocsWithOutParameter = """
			<summary>Asynchronously downloads all on-demand RAW demosaicing and denoising models that aren't already installed and provides download progress.</summary>
			<param name="timeout">The maximum time, in seconds, to wait for the download.</param>
			<param name="result">An object that tracks and can cancel the download.</param>
			<returns>A task that represents the asynchronous download operation.</returns>
			""")]
		[Export ("downloadAllResourcesWithTimeout:completionHandler:")]
		NSProgress DownloadAllResources (double timeout, CIRawFilterDownloadResourcesCompletionHandler completionHandler);

		[Export ("supportedDecoderVersions")]
		string [] SupportedDecoderVersions { get; }

		[Export ("nativeSize")]
		CGSize NativeSize { get; }

		[Export ("properties")]
		NSDictionary Properties { get; }

		[Export ("orientation", ArgumentSemantic.Assign)]
		CGImagePropertyOrientation Orientation { get; set; }

		[Export ("draftModeEnabled")]
		bool DraftModeEnabled { [Bind ("isDraftModeEnabled")] get; set; }

		[Export ("decoderVersion", ArgumentSemantic.Retain)]
		string DecoderVersion { get; set; }

		[Export ("scaleFactor")]
		float ScaleFactor { get; set; }

		[Export ("exposure")]
		float Exposure { get; set; }

		[Export ("baselineExposure")]
		float BaselineExposure { get; set; }

		[Export ("shadowBias")]
		float ShadowBias { get; set; }

		[Export ("boostAmount")]
		float BoostAmount { get; set; }

		[Export ("boostShadowAmount")]
		float BoostShadowAmount { get; set; }

		[Export ("gamutMappingEnabled")]
		bool GamutMappingEnabled { [Bind ("isGamutMappingEnabled")] get; set; }

		[Export ("lensCorrectionSupported")]
		bool LensCorrectionSupported { [Bind ("isLensCorrectionSupported")] get; }

		[Export ("lensCorrectionEnabled")]
		bool LensCorrectionEnabled { [Bind ("isLensCorrectionEnabled")] get; set; }

		[Export ("luminanceNoiseReductionSupported")]
		bool LuminanceNoiseReductionSupported { [Bind ("isLuminanceNoiseReductionSupported")] get; }

		[Export ("luminanceNoiseReductionAmount")]
		float LuminanceNoiseReductionAmount { get; set; }

		[Export ("colorNoiseReductionSupported")]
		bool ColorNoiseReductionSupported { [Bind ("isColorNoiseReductionSupported")] get; }

		[Export ("colorNoiseReductionAmount")]
		float ColorNoiseReductionAmount { get; set; }

		[Export ("sharpnessSupported")]
		bool SharpnessSupported { [Bind ("isSharpnessSupported")] get; }

		[Export ("sharpnessAmount")]
		float SharpnessAmount { get; set; }

		[Export ("contrastSupported")]
		bool ContrastSupported { [Bind ("isContrastSupported")] get; }

		[Export ("contrastAmount")]
		float ContrastAmount { get; set; }

		[Export ("detailSupported")]
		bool DetailSupported { [Bind ("isDetailSupported")] get; }

		[Export ("detailAmount")]
		float DetailAmount { get; set; }

		[Export ("moireReductionSupported")]
		bool MoireReductionSupported { [Bind ("isMoireReductionSupported")] get; }

		[Export ("moireReductionAmount")]
		float MoireReductionAmount { get; set; }

		/// <summary>Gets whether the RAW image supports despeckle correction.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("despeckleSupported")]
		bool DespeckleSupported { [Bind ("isDespeckleSupported")] get; }

		/// <summary>Gets or sets the amount of despeckle correction applied to the RAW image.</summary>
		/// <value>A value from 0 for no correction to 1 for maximum correction.</value>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("despeckleAmount")]
		float DespeckleAmount { get; set; }

		[Export ("localToneMapSupported")]
		bool LocalToneMapSupported { [Bind ("isLocalToneMapSupported")] get; }

		[Export ("localToneMapAmount")]
		float LocalToneMapAmount { get; set; }

		[Export ("extendedDynamicRangeAmount")]
		float ExtendedDynamicRangeAmount { get; set; }

		[Export ("neutralChromaticity", ArgumentSemantic.Assign)]
		CGPoint NeutralChromaticity { get; set; }

		[Export ("neutralLocation", ArgumentSemantic.Assign)]
		CGPoint NeutralLocation { get; set; }

		[Export ("neutralTemperature")]
		float NeutralTemperature { get; set; }

		[Export ("neutralTint")]
		float NeutralTint { get; set; }

		[NullAllowed, Export ("linearSpaceFilter", ArgumentSemantic.Retain)]
		CIFilter LinearSpaceFilter { get; set; }

		[NullAllowed, Export ("previewImage")]
		CIImage PreviewImage { get; }

		[NullAllowed, Export ("portraitEffectsMatte")]
		CIImage PortraitEffectsMatte { get; }

		[NullAllowed, Export ("semanticSegmentationSkinMatte")]
		CIImage SemanticSegmentationSkinMatte { get; }

		[NullAllowed, Export ("semanticSegmentationHairMatte")]
		CIImage SemanticSegmentationHairMatte { get; }

		[NullAllowed, Export ("semanticSegmentationGlassesMatte")]
		CIImage SemanticSegmentationGlassesMatte { get; }

		[NullAllowed, Export ("semanticSegmentationSkyMatte")]
		CIImage SemanticSegmentationSkyMatte { get; }

		[NullAllowed, Export ("semanticSegmentationTeethMatte")]
		CIImage SemanticSegmentationTeethMatte { get; }

		[Static]
		[Export ("filterWithImageURL:")]
		[return: NullAllowed]
		CIRawFilter Create (NSUrl url);

		[Static]
		[Export ("filterWithImageData:identifierHint:")]
		[return: NullAllowed]
		CIRawFilter Create (NSData data, [NullAllowed] string identifierHint);

		[Static]
		[Export ("filterWithCVPixelBuffer:properties:")]
		[return: NullAllowed]
		CIRawFilter Create (CVPixelBuffer buffer, NSDictionary properties);

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Export ("highlightRecoverySupported")]
		bool HighlightRecoverySupported { [Bind ("isHighlightRecoverySupported")] get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Export ("highlightRecoveryEnabled")]
		bool HighlightRecoveryEnabled { [Bind ("isHighlightRecoveryEnabled")] get; set; }
	}

	[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
	[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[Static]
	[Internal]
	interface CIRawFilterKeys {
		[MacCatalyst (13, 1)]
		[Field ("kCIInputAllowDraftModeKey")]
		NSString AllowDraftModeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputDecoderVersionKey")]
		NSString VersionKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCISupportedDecoderVersionsKey")]
		NSString SupportedDecoderVersionsKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputBaselineExposureKey")]
		NSString BaselineExposureKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputBoostKey")]
		NSString BoostKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputBoostShadowAmountKey")]
		NSString BoostShadowAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputDisableGamutMapKey")]
		NSString DisableGamutMapKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralChromaticityXKey")]
		NSString NeutralChromaticityXKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralChromaticityYKey")]
		NSString NeutralChromaticityYKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralTemperatureKey")]
		NSString NeutralTemperatureKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralTintKey")]
		NSString NeutralTintKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNeutralLocationKey")]
		NSString NeutralLocationKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputScaleFactorKey")]
		NSString ScaleFactorKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputIgnoreImageOrientationKey")]
		NSString IgnoreImageOrientationKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputImageOrientationKey")]
		NSString ImageOrientationKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputEnableSharpeningKey")]
		NSString EnableSharpeningKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputEnableChromaticNoiseTrackingKey")]
		NSString EnableChromaticNoiseTrackingKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionAmountKey")]
		NSString NoiseReductionAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputMoireAmountKey")]
		NSString MoireAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputEnableVendorLensCorrectionKey")]
		NSString EnableVendorLensCorrectionKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputLuminanceNoiseReductionAmountKey")]
		NSString LuminanceNoiseReductionAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputColorNoiseReductionAmountKey")]
		NSString ColorNoiseReductionAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionSharpnessAmountKey")]
		NSString NoiseReductionSharpnessAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionContrastAmountKey")]
		NSString NoiseReductionContrastAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionDetailAmountKey")]
		NSString NoiseReductionDetailAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputLinearSpaceFilter")]
		NSString LinearSpaceFilterKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIInputEnableEDRModeKey")]
		NSString EnableEdrModeKey { get; }

		[iOS (14, 3), TV (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("kCIInputLocalToneMapAmountKey")]
		NSString InputLocalToneMapAmountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIOutputNativeSizeKey")]
		NSString OutputNativeSizeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIActiveKeys")]
		NSString ActiveKeysKey { get; }
	}

	/// <summary>Settings for use with <see cref="CoreImage.CIFilter.CreateRawFilter(CoreVideo.CVPixelBuffer,Foundation.NSDictionary,Foundation.NSDictionary)" />.</summary>
	[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'CIRawFilter' instead.")]
	[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'CIRawFilter' instead.")]
	[StrongDictionary ("CIRawFilterKeys")]
	interface CIRawFilterOptions {

		/// <summary>
		///           <see langword="true" /> if draft mode should be allowed. (Switching this key is an expensive operation.)</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool AllowDraftMode { get; set; }

		/// <summary>The key of the current decoder (see <see cref="CoreImage.CIRawFilterOptions.SupportedDecoderVersions" />).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		string Version { get; set; }

		/// <summary>A dictionary whose keys are version identifiers of valid decoders.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		NSDictionary [] SupportedDecoderVersions { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		float BaselineExposure { get; set; }

		/// <summary>Amount of boost (contrast enhancement), ranging from 0.0 (no boost) to 1.0 (full boost).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		float Boost { get; set; }

		/// <summary>Amount of boost (contrast enhancement), ranging from 0.0 (no boost) to 1.0 (full boost) to be applied in shadow regions.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		float BoostShadowAmount { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool DisableGamutMap { get; set; }

		/// <summary>Current neutral X value of the chromaticity.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		float NeutralChromaticityX { get; set; }

		/// <summary>Current neutral Y value of the chromaticity.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		float NeutralChromaticityY { get; set; }

		/// <summary>The neutral color temperature. (Set using <see cref="CoreImage.CIRawFilterOptions.NeutralTint" />.)</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		float NeutralTemperature { get; set; }

		/// <summary>The neutral tint. Setting this value also modifies <see cref="CoreImage.CIRawFilterOptions.NeutralTemperature" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		float NeutralTint { get; set; }

		/// <summary>Used to set the neutral (X,Y) position in the unrotated output image.</summary>
		///         <value>To be added.</value>
		///         <remarks>
		///           <para>Developers should not rely on reading this value: it is specified as "undefined" for reading.</para>
		///         </remarks>
		[MacCatalyst (13, 1)]
		CIVector NeutralLocation { get; set; }

		/// <summary>The desired scale factor for drawing the image.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		float ScaleFactor { get; set; }

		/// <summary>If <see langword="true" />, the image's embedded orientation data will be ignored.</summary>
		///         <value>The default value is <see langword="false" />.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool IgnoreImageOrientation { get; set; }

		/// <summary>The EXIF image orientation value (in the range 1..8).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		int ImageOrientation { get; set; }

		/// <summary>
		///           <see langword="true" /> if sharpening should be applied.</summary>
		///         <value>The default value is <see langword="true" />.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool EnableSharpening { get; set; }

		/// <summary>If <see langword="true" />, chromatic noise tracking using ISO and exposure is active.</summary>
		///         <value>The default value is <see langword="true" />.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool EnableChromaticNoiseTracking { get; set; }

		/// <summary>Amount of noise reduction to apply, ranging from 0.0 (no reduction) to 1.0 (maximum).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		double NoiseReductionAmount { get; set; }

		/// <summary>If <see langword="true" />, correction will be applied for known lenses.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool EnableVendorLensCorrection { get; set; }

		/// <summary>Amount of noise reduction to apply to luminance data, ranging from 0.0 (no reduction) to 1.0 (maximum).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		double LuminanceNoiseReductionAmount { get; set; }

		/// <summary>Amount of noise reduction to apply to color data, ranging from 0.0 (no reduction) to 1.0 (maximum).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		double ColorNoiseReductionAmount { get; set; }

		/// <summary>Amount of sharpening to apply during noise reduction, in the range 0.0 (no sharpening) to 1.0 (maximum).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		double NoiseReductionSharpnessAmount { get; set; }

		/// <summary>Amount of contrast enhancement to apply during noise reduction, in the range 0.0 (no contrast enhancement) to 1.0 (maximum).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		double NoiseReductionContrastAmount { get; set; }

		/// <summary>Amount of detail enhancement to apply during noise reduction, in the range 0.0 (no detail enhancement) to 1.0 (maximum).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputNoiseReductionDetailAmountKey")]
		double NoiseReductionDetailAmount { get; set; }

		/// <summary>The <see cref="CoreImage.CIFilter" /> applied to the image when, during RAW processing, it is in the linear color space.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		CIFilter LinearSpaceFilter { get; set; }

		/// <summary>The full native size of the original image.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		CIVector OutputNativeSize { get; set; }

		/// <summary>The set of input keys that are available for use on the input image.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		NSSet ActiveKeys { get; }
	}

	/// <summary>Keys that can be used to get output results out of a CIFilter.</summary>
	///     <remarks>At this point, this only includes the CIFilterOutputKey</remarks>
	[Static]
	interface CIFilterOutputKey {
		/// <summary>Represents the value associated with the constant kCIOutputImageKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIOutputImageKey", "+CoreImage")]
		NSString Image { get; }
	}

	/// <summary>Keys that can be used to configure the CIFilter input values.</summary>
	[Static]
	interface CIFilterInputKey {
		/// <summary>Represents the value associated with the constant kCIInputBackgroundImageKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputBackgroundImageKey", "+CoreImage")]
		NSString BackgroundImage { get; }

		/// <summary>Represents the value associated with the constant kCIInputImageKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputImageKey", "+CoreImage")]
		NSString Image { get; }

		/// <summary>Represents the value associated with the constant kCIInputVersionKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputVersionKey", "+CoreImage")]
		NSString Version { get; }

		/// <summary>Represents the value associated with the constant kCIInputRefractionKey.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputRefractionKey", "+CoreImage")]
		NSString Refraction { get; }

		/// <summary>Represents the value associated with the constant kCIInputGradientImageKey.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputGradientImageKey", "+CoreImage")]
		NSString GradientImage { get; }

		/// <summary>Represents the value associated with the constant kCIInputShadingImageKey.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputShadingImageKey", "+CoreImage")]
		NSString ShadingImage { get; }

		/// <summary>Represents the value associated with the constant kCIInputTimeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputTimeKey", "+CoreImage")]
		NSString Time { get; }

		/// <summary>Represents the value associated with the constant kCIInputTransformKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputTransformKey", "+CoreImage")]
		NSString Transform { get; }

		/// <summary>Represents the value associated with the constant kCIInputScaleKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputScaleKey", "+CoreImage")]
		NSString Scale { get; }

		/// <summary>Represents the value associated with the constant kCIInputAspectRatioKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputAspectRatioKey", "+CoreImage")]
		NSString AspectRatio { get; }

		/// <summary>Represents the value associated with the constant kCIInputCenterKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputCenterKey", "+CoreImage")]
		NSString Center { get; }

		/// <summary>Represents the value associated with the constant kCIInputRadiusKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputRadiusKey", "+CoreImage")]
		NSString Radius { get; }

		/// <summary>Represents the value associated with the constant kCIInputAngleKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputAngleKey", "+CoreImage")]
		NSString Angle { get; }

		/// <summary>Represents the value associated with the constant kCIInputWidthKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputWidthKey", "+CoreImage")]
		NSString Width { get; }

		/// <summary>Represents the value associated with the constant kCIInputSharpnessKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputSharpnessKey", "+CoreImage")]
		NSString Sharpness { get; }

		/// <summary>Represents the value associated with the constant kCIInputIntensityKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputIntensityKey", "+CoreImage")]
		NSString Intensity { get; }

		/// <summary>Represents the value associated with the constant kCIInputEVKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputEVKey", "+CoreImage")]
		NSString EV { get; }

		/// <summary>Represents the value associated with the constant kCIInputSaturationKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputSaturationKey", "+CoreImage")]
		NSString Saturation { get; }

		/// <summary>Represents the value associated with the constant kCIInputColorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputColorKey", "+CoreImage")]
		NSString Color { get; }

		/// <summary>Represents the value associated with the constant kCIInputBrightnessKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputBrightnessKey", "+CoreImage")]
		NSString Brightness { get; }

		/// <summary>Represents the value associated with the constant kCIInputContrastKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputContrastKey", "+CoreImage")]
		NSString Contrast { get; }

		/// <summary>Represents the value associated with the constant kCIInputBiasKey.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputBiasKey", "+CoreImage")]
		NSString BiasKey { get; }

		/// <summary>A key indicating a value that is a weight matrix for a convolution filter.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputWeightsKey", "+CoreImage")]
		NSString WeightsKey { get; }

		/// <summary>Represents the value associated with the constant kCIInputMaskImageKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputMaskImageKey", "+CoreImage")]
		NSString MaskImage { get; }

		/// <summary>Represents the value associated with the constant kCIInputTargetImageKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputTargetImageKey", "+CoreImage")]
		NSString TargetImage { get; }

		/// <summary>Represents the value associated with the constant kCIInputExtentKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIInputExtentKey", "+CoreImage")]
		NSString Extent { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputDepthImageKey", "+CoreImage")]
		NSString DepthImage { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputDisparityImageKey", "+CoreImage")]
		NSString DisparityImage { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputMatteImageKey", "+CoreImage")]
		NSString MatteImage { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIInputAmountKey", "+CoreImage")]
		NSString Amount { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputCountKey", "+CoreImage")]
		NSString Count { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputThresholdKey", "+CoreImage")]
		NSString Threshold { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputRadius0Key", "+CoreImage")]
		NSString Radius0 { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputRadius1Key", "+CoreImage")]
		NSString Radius1 { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputColor0Key", "+CoreImage")]
		NSString Color0 { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputColor1Key", "+CoreImage")]
		NSString Color1 { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputColorSpaceKey", "+CoreImage")]
		NSString ColorSpace { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputExtrapolateKey", "+CoreImage")]
		NSString Extrapolate { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputPerceptualKey", "+CoreImage")]
		NSString Perceptual { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputBiasVectorKey", "+CoreImage")]
		NSString BiasVector { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputBacksideImageKey", "+CoreImage")]
		NSString BacksideImage { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputPaletteImageKey", "+CoreImage")]
		NSString PaletteImage { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputPoint0Key", "+CoreImage")]
		NSString Point0 { get; }

		[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
		[Field ("kCIInputPoint1Key", "+CoreImage")]
		NSString Point1 { get; }
	}

	[MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0), TV (26, 0)]
	enum CIDynamicRangeOption {
		[Field ("kCIDynamicRangeStandard")]
		Standard,

		[Field ("kCIDynamicRangeConstrainedHigh")]
		ConstrainedHigh,

		[Field ("kCIDynamicRangeHigh")]
		High,
	}

	/// <summary>Constants used for CIFilter's attributes</summary>
	[Static]
	interface CIFilterAttributes {
		/// <summary>Represents the value associated with the constant kCIAttributeFilterName</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeFilterName", "+CoreImage")]
		NSString FilterName { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeFilterDisplayName</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeFilterDisplayName", "+CoreImage")]
		NSString FilterDisplayName { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeDescription</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeDescription", "+CoreImage")]
		NSString Description { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeReferenceDocumentation</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeReferenceDocumentation", "+CoreImage")]
		NSString ReferenceDocumentation { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeFilterCategories</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeFilterCategories", "+CoreImage")]
		NSString FilterCategories { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeClass</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeClass", "+CoreImage")]
		NSString Class { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeType</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeType", "+CoreImage")]
		NSString Type { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeMin</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeMin", "+CoreImage")]
		NSString Min { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeMax</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeMax", "+CoreImage")]
		NSString Max { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeSliderMin</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeSliderMin", "+CoreImage")]
		NSString SliderMin { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeSliderMax</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeSliderMax", "+CoreImage")]
		NSString SliderMax { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeDefault</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeDefault", "+CoreImage")]
		NSString Default { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeIdentity</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeIdentity", "+CoreImage")]
		NSString Identity { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeName</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeName", "+CoreImage")]
		NSString Name { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeDisplayName</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeDisplayName", "+CoreImage")]
		NSString DisplayName { get; }

		/// <summary>Represents the value associated with the constant kCIUIParameterSet</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIUIParameterSet", "+CoreImage")]
		NSString UIParameterSet { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeTime</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeTime", "+CoreImage")]
		NSString TypeTime { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeScalar</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeScalar", "+CoreImage")]
		NSString TypeScalar { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeDistance</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeDistance", "+CoreImage")]
		NSString TypeDistance { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeAngle</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeAngle", "+CoreImage")]
		NSString TypeAngle { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeBoolean</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeBoolean", "+CoreImage")]
		NSString TypeBoolean { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeInteger</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeInteger", "+CoreImage")]
		NSString TypeInteger { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeCount</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeCount", "+CoreImage")]
		NSString TypeCount { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypePosition</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypePosition", "+CoreImage")]
		NSString TypePosition { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeOffset</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeOffset", "+CoreImage")]
		NSString TypeOffset { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypePosition3</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypePosition3", "+CoreImage")]
		NSString TypePosition3 { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeRectangle</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIAttributeTypeRectangle", "+CoreImage")]
		NSString TypeRectangle { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeOpaqueColor</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeOpaqueColor", "+CoreImage")]
		NSString TypeOpaqueColor { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeGradient</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeGradient", "+CoreImage")]
		NSString TypeGradient { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeImage</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeImage", "+CoreImage")]
		NSString TypeImage { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeTransform</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeTransform", "+CoreImage")]
		NSString TypeTransform { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeTypeColor</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeTypeColor", "+CoreImage")]
		NSString TypeColor { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeFilterAvailable_Mac</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeFilterAvailable_Mac", "+CoreImage")]
		NSString Available_Mac { get; }

		/// <summary>Represents the value associated with the constant kCIAttributeFilterAvailable_iOS</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIAttributeFilterAvailable_iOS", "+CoreImage")]
		NSString Available_iOS { get; }
	}

	/// <summary>Constants used for CIFilter filtering facilities to find filters by category.</summary>
	[Static]
	interface CIFilterCategory {
		/// <summary>Represents the value associated with the constant kCICategoryDistortionEffect</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryDistortionEffect", "+CoreImage")]
		NSString DistortionEffect { get; }

		/// <summary>Represents the value associated with the constant kCICategoryGeometryAdjustment</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryGeometryAdjustment", "+CoreImage")]
		NSString GeometryAdjustment { get; }

		/// <summary>Represents the value associated with the constant kCICategoryCompositeOperation</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryCompositeOperation", "+CoreImage")]
		NSString CompositeOperation { get; }

		/// <summary>Represents the value associated with the constant kCICategoryHalftoneEffect</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryHalftoneEffect", "+CoreImage")]
		NSString HalftoneEffect { get; }

		/// <summary>Represents the value associated with the constant kCICategoryColorAdjustment</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryColorAdjustment", "+CoreImage")]
		NSString ColorAdjustment { get; }

		/// <summary>Represents the value associated with the constant kCICategoryColorEffect</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryColorEffect", "+CoreImage")]
		NSString ColorEffect { get; }

		/// <summary>Represents the value associated with the constant kCICategoryTransition</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryTransition", "+CoreImage")]
		NSString Transition { get; }

		/// <summary>Represents the value associated with the constant kCICategoryTileEffect</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryTileEffect", "+CoreImage")]
		NSString TileEffect { get; }

		/// <summary>Represents the value associated with the constant kCICategoryGenerator</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryGenerator", "+CoreImage")]
		NSString Generator { get; }

		/// <summary>Represents the value associated with the constant kCICategoryReduction</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryReduction", "+CoreImage")]
		NSString Reduction { get; }

		/// <summary>Represents the value associated with the constant kCICategoryGradient</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryGradient", "+CoreImage")]
		NSString Gradient { get; }

		/// <summary>Represents the value associated with the constant kCICategoryStylize</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryStylize", "+CoreImage")]
		NSString Stylize { get; }

		/// <summary>Represents the value associated with the constant kCICategorySharpen</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategorySharpen", "+CoreImage")]
		NSString Sharpen { get; }

		/// <summary>Represents the value associated with the constant kCICategoryBlur</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryBlur", "+CoreImage")]
		NSString Blur { get; }

		/// <summary>Represents the value associated with the constant kCICategoryVideo</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryVideo", "+CoreImage")]
		NSString Video { get; }

		/// <summary>Represents the value associated with the constant kCICategoryStillImage</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryStillImage", "+CoreImage")]
		NSString StillImage { get; }

		/// <summary>Represents the value associated with the constant kCICategoryInterlaced</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryInterlaced", "+CoreImage")]
		NSString Interlaced { get; }

		/// <summary>Represents the value associated with the constant kCICategoryNonSquarePixels</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryNonSquarePixels", "+CoreImage")]
		NSString NonSquarePixels { get; }

		/// <summary>Represents the value associated with the constant kCICategoryHighDynamicRange</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryHighDynamicRange", "+CoreImage")]
		NSString HighDynamicRange { get; }

		/// <summary>Represents the value associated with the constant kCICategoryBuiltIn</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCICategoryBuiltIn", "+CoreImage")]
		NSString BuiltIn { get; }

		/// <summary>Represents the value associated with the constant kCICategoryFilterGenerator</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCICategoryFilterGenerator", "+CoreImage")]
		NSString FilterGenerator { get; }
	}

	/// <summary>Interface for classes that create named filters.</summary>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface CIFilterConstructor {
		/// <param name="name">To be added.</param>
		/// <summary>Creates a new filter from the provided name.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("filterWithName:")]
		[return: NullAllowed]
		CIFilter FilterWithName (string name);
	}

	interface ICIFilterConstructor { }

	/// <summary>Contains options for core image filter user interfaces.</summary>
	[Static]
	[MacCatalyst (13, 1)]
	interface CIUIParameterSet {
		/// <summary>Basic  user interface set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIUISetBasic", "+CoreImage")]
		NSString Basic { get; }

		/// <summary>Intermediate user interface set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIUISetIntermediate", "+CoreImage")]
		NSString Intermediate { get; }

		/// <summary>Advanced user interface set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIUISetAdvanced", "+CoreImage")]
		NSString Advanced { get; }

		/// <summary>Development user interface set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIUISetDevelopment", "+CoreImage")]
		NSString Development { get; }
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoTV]
	[Static]
	interface CIFilterApply {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIApplyOptionExtent", "+CoreImage")]
		NSString OptionExtent { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIApplyOptionDefinition", "+CoreImage")]
		NSString OptionDefinition { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIApplyOptionUserInfo", "+CoreImage")]
		NSString OptionUserInfo { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIApplyOptionColorSpace", "+CoreImage")]
		NSString OptionColorSpace { get; }
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIFilterGenerator : CIFilterConstructor, NSSecureCoding, NSCopying {
		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static, Export ("filterGenerator")]
		CIFilterGenerator Create ();

		/// <param name="aURL">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("filterGeneratorWithContentsOfURL:")]
		[return: NullAllowed]
		CIFilterGenerator FromUrl (NSUrl aURL);

		/// <param name="aURL">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl aURL);

		/// <param name="sourceObject">To be added.</param>
		///         <param name="withSourceKey">To be added.</param>
		///         <param name="targetObject">To be added.</param>
		///         <param name="targetKey">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("connectObject:withKey:toObject:withKey:")]
		void ConnectObject (NSObject sourceObject, [NullAllowed] string withSourceKey, NSObject targetObject, string targetKey);

		/// <param name="sourceObject">To be added.</param>
		///         <param name="sourceKey">To be added.</param>
		///         <param name="targetObject">To be added.</param>
		///         <param name="targetKey">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("disconnectObject:withKey:toObject:withKey:")]
		void DisconnectObject (NSObject sourceObject, string sourceKey, NSObject targetObject, string targetKey);

		/// <param name="key">To be added.</param>
		///         <param name="targetObject">To be added.</param>
		///         <param name="exportedKeyName">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("exportKey:fromObject:withName:")]
		void ExportKey (string key, NSObject targetObject, [NullAllowed] string exportedKeyName);

		/// <param name="exportedKeyName">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("removeExportedKey:")]
		void RemoveExportedKey (string exportedKeyName);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("exportedKeys")]
		NSDictionary ExportedKeys { get; }

		/// <param name="attributes">To be added.</param>
		///         <param name="exportedKey">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("setAttributes:forExportedKey:")]
#if XAMCORE_5_0
		void SetAttributes (NSDictionary attributes, NSString exportedKey);
#else
		[Obsolete ("Use 'SetAttributes' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		void SetAttributesforExportedKey (NSDictionary attributes, NSString exportedKey);

		[Wrap ("SetAttributesforExportedKey (attributes, exportedKey)")]
		void SetAttributes (NSDictionary attributes, NSString exportedKey);
#endif

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("filter")]
		CIFilter CreateFilter ();

		/// <param name="name">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("registerFilterName:")]
		void RegisterFilterName (string name);

		/// <param name="toUrl">To be added.</param>
		///         <param name="atomically">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("writeToURL:atomically:")]
		bool Save (NSUrl toUrl, bool atomically);

		//Detected properties
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("classAttributes")]
		NSDictionary ClassAttributes { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFilterGeneratorExportedKey", "+CoreImage")]
		NSString ExportedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFilterGeneratorExportedKeyTargetObject", "+CoreImage")]
		NSString ExportedKeyTargetObject { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFilterGeneratorExportedKeyName", "+CoreImage")]
		NSString ExportedKeyName { get; }
	}

	/// <summary>Masks filter operations to areas composed of the intersections, unions, and affine transformations of rectangles and other similarly constructed filter shapes.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/QuartzCoreFramework/Classes/CIFilterShape_Class/index.html">Apple documentation for <c>CIFilterShape</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[MacCatalyst (13, 1)]
	interface CIFilterShape : NSCopying {
		/// <param name="rect">To be added.</param>
		///         <summary>Creates a new CIFilterShape that limits filter operations to the specified rectangle.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("shapeWithRect:")]
		CIFilterShape FromRect (CGRect rect);

		/// <param name="rect">To be added.</param>
		/// <summary>Creates a new CIFilterShape that limits filter operations to the specified rectangle.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithRect:")]
		NativeHandle Constructor (CGRect rect);

		/// <param name="transformation">To be added.</param>
		///         <param name="interiorFlag">Developers should pass <see langword="true" /> to indicate that the resulting transformed filter shape should definitely exclude the boundary points. Developers should pass <see langword="false" /> to indicate that the result should definitely include the boundary points.</param>
		///         <summary>Creates a new CIFilterShape by applying the specified transformation to the current filter shape.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>
		///           <para>App developers should realize that neither setting of <paramref name="interiorFlag" /> results in an exact operation. Points may be excluded or included along the boundary to guarantee inclusivity or exclusivity of the result.</para>
		///         </remarks>
		[Export ("transformBy:interior:")]
		CIFilterShape Transform (CGAffineTransform transformation, bool interiorFlag);

		/// <param name="dx">To be added.</param>
		///         <param name="dy">To be added.</param>
		///         <summary>Moves the filter region by the specified X and Y directions.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("insetByX:Y:")]
		CIFilterShape Inset (int /* int, not NSInteger */ dx, int /* int, not NSInteger */  dy);

		/// <param name="other">To be added.</param>
		///         <summary>Creates a new CIFilterShape from the union of the current filter shape with <paramref name="other" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("unionWith:")]
		CIFilterShape Union (CIFilterShape other);

		/// <param name="rectangle">To be added.</param>
		///         <summary>Creates a new CIFilterShape from the union of the current filter shape with the specified rectangle.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("unionWithRect:")]
		CIFilterShape Union (CGRect rectangle);

		/// <param name="other">To be added.</param>
		///         <summary>Creates a new CIFilterShape from the intersection of the current filter shape with <paramref name="other" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("intersectWith:")]
		CIFilterShape Intersect (CIFilterShape other);

		/// <param name="rectangle">The rectangle with which to calculate the intersection.</param>
		///         <summary>Returns a new <see cref="CoreImage.CIFilterShape" /> whose shape is defined by the overlap of <c>this</c> and the specified <paramref name="rectangle" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("intersectWithRect:")]
		CIFilterShape Intersect (CGRect rectangle);

		/// <summary>The rectangle that encloses the filter, in the working coordinate space.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("extent")]
		CGRect Extent { get; }
	}

	/// <summary>Options that can be used when initializing a new <see cref="CoreImage.CIImage" />.</summary>
	[StrongDictionary ("CIImageInitializationOptionsKeys")]
	interface CIImageInitializationOptions {
		// Bug #60726: [Generator] Support INativeObject in StrongDictionary
		// (https://bugzilla.xamarin.com/show_bug.cgi?id=60726)
		// CGColorSpace ColorSpace { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		CoreGraphics.CGImageProperties Properties { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool ApplyOrientationProperty { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool NearestSampling { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool AuxiliaryDepth { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool AuxiliaryDisparity { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		bool AuxiliaryPortraitEffectsMatte { get; set; }

		[MacCatalyst (13, 1)]
		bool AuxiliarySemanticSegmentationSkinMatte { get; set; }

		[MacCatalyst (13, 1)]
		bool AuxiliarySemanticSegmentationHairMatte { get; set; }

		[MacCatalyst (13, 1)]
		bool AuxiliarySemanticSegmentationTeethMatte { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		bool AuxiliarySemanticSegmentationGlassesMatte { get; set; }

		[iOS (14, 1), TV (14, 1), MacCatalyst (14, 1)]
		bool AuxiliaryHdrGainMap { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		bool ToneMapHdrToSdr { get; set; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		bool CacheImmediately { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		bool ExpandToHdr { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		float ContentHeadroom { get; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		float ContentAverageLightLevel { get; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		bool ApplyCleanAperture { get; }

		/// <summary>Gets or sets the factor by which image data is downsampled while decoding.</summary>
		/// <value>The integer value 2, 4, or 8.</value>
		/// <remarks>Downsampling is supported for JPEG, HEIF, TIFF, PNG, and RAW image formats.</remarks>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		int SubsampleFactor { get; set; }

		/// <summary>Gets or sets the uniform type identifier used when the image format cannot be determined from its contents.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		string TypeIdentifierHint { get; set; }

		/// <summary>Gets or sets whether hardware-accelerated image decoding is preferred.</summary>
		/// <remarks>If this option is not specified, hardware-accelerated decoding is preferred.</remarks>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		bool UseHardwareAcceleration { get; set; }
	}

	[Internal]
	[Static]
	interface CIImageInitializationOptionsKeys {
		[Field ("kCIImageColorSpace")]
		NSString ColorSpaceKey { get; }

		[Field ("kCIImageProperties")]
		NSString PropertiesKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageNearestSampling")]
		NSString NearestSamplingKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageApplyOrientationProperty")]
		NSString ApplyOrientationPropertyKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliaryDepth")]
		NSString AuxiliaryDepthKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliaryDisparity")]
		NSString AuxiliaryDisparityKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliaryPortraitEffectsMatte")]
		NSString AuxiliaryPortraitEffectsMatteKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliarySemanticSegmentationSkinMatte")]
		NSString AuxiliarySemanticSegmentationSkinMatteKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliarySemanticSegmentationHairMatte")]
		NSString AuxiliarySemanticSegmentationHairMatteKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAuxiliarySemanticSegmentationTeethMatte")]
		NSString AuxiliarySemanticSegmentationTeethMatteKey { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Field ("kCIImageAuxiliarySemanticSegmentationGlassesMatte")]
		NSString AuxiliarySemanticSegmentationGlassesMatteKey { get; }

		[iOS (14, 1), TV (14, 1), MacCatalyst (14, 1)]
		[Field ("kCIImageAuxiliaryHDRGainMap")]
		NSString AuxiliaryHdrGainMapKey { get; }

		[iOS (14, 3), TV (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("kCIImageAuxiliarySemanticSegmentationSkyMatte")]
		NSString AuxiliarySemanticSegmentationSkyMatteKey { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Field ("kCIImageToneMapHDRtoSDR")]
		NSString ToneMapHdrToSdrKey { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIImageCacheImmediately")]
		NSString CacheImmediatelyKey { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIImageExpandToHDR")]
		NSString ExpandToHdrKey { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("kCIImageContentHeadroom")]
		NSString ContentHeadroomKey { get; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("kCIImageContentAverageLightLevel")]
		NSString ContentAverageLightLevelKey { get; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("kCIImageApplyCleanAperture")]
		NSString ApplyCleanApertureKey { get; }

		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Field ("kCIImageSubsampleFactor")]
		NSString SubsampleFactorKey { get; }

		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Field ("kCIImageTypeIdentifierHint")]
		NSString TypeIdentifierHintKey { get; }

		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Field ("kCIImageUseHardwareAcceleration")]
		NSString UseHardwareAccelerationKey { get; }
	}

	/// <include file="../docs/api/CoreImage/CIImage.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIImage']/*" />
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIImage : NSSecureCoding, NSCopying {
		/// <param name="image">CoreGraphics image.</param>
		///         <summary>Creates an <see cref="CoreImage.CIImage" /> from a <see cref="CoreGraphics.CGImage" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("imageWithCGImage:")]
		CIImage FromCGImage (CGImage image);

		/// <param name="image">CoreGraphics image.</param>
		///         <param name="d">
		///           <para>Extra metadata, as an NSDictionary.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Creates a <see cref="CoreImage.CIImage" />  from a <see cref="CoreGraphics.CGImage" /> with the specified metadata,  <paramref name="d" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCGImage:options:")]
		CIImage FromCGImage (CGImage image, [NullAllowed] NSDictionary d);

		/// <param name="image">CoreGraphics image.</param>
		///         <param name="options">Options to initialize the image with.</param>
		///         <summary>Creates a <see cref="CoreImage.CIImage" />  from a <see cref="CoreGraphics.CGImage" /> with the specified <paramref name="options" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Wrap ("FromCGImage (image, options.GetDictionary ())")]
		CIImage FromCGImage (CGImage image, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCGImageSource:index:options:")]
		CIImage FromCGImageSource (CGImageSource source, nuint index, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("FromCGImageSource (source, index, options.GetDictionary ())")]
		CIImage FromCGImageSource (CGImageSource source, nuint index, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		/// <param name="layer">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Static]
		[Export ("imageWithCGLayer:")]
		CIImage FromLayer (CGLayer layer);

		/// <param name="layer">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Static]
		[Export ("imageWithCGLayer:options:")]
		CIImage FromLayer (CGLayer layer, [NullAllowed] NSDictionary options);

		[Static]
		[Export ("imageWithBitmapData:bytesPerRow:size:format:colorSpace:")]
		[Internal] // there's a CIFormat enum that maps to the kCIFormatARGB8, kCIFormatRGBA16, kCIFormatRGBAf, kCIFormatRGBAh constants
		CIImage FromData (NSData bitmapData, nint bytesPerRow, CGSize size, int /* CIFormat = int */ pixelFormat, [NullAllowed] CGColorSpace colorSpace);

		/// <param name="glTextureName">To be added.</param>
		///         <param name="size">To be added.</param>
		///         <param name="flipped">To be added.</param>
		///         <param name="colorspace">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("imageWithTexture:size:flipped:colorSpace:")]
		CIImage ImageWithTexture (uint /* unsigned int */ glTextureName, CGSize size, bool flipped, [NullAllowed] CGColorSpace colorspace);

		/// <param name="url">To be added.</param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> from <paramref name="url" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("imageWithContentsOfURL:")]
		[return: NullAllowed]
		CIImage FromUrl (NSUrl url);

		/// <param name="url">To be added.</param>
		///         <param name="d">
		///           <para>Extra configuration options, as an NSDictionary.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> from <paramref name="url" /> by using the options that are specified in <paramref name="d" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithContentsOfURL:options:")]
		[return: NullAllowed]
		CIImage FromUrl (NSUrl url, [NullAllowed] NSDictionary d);

		/// <param name="url">To be added.</param>
		///         <param name="options">Options to initialize the image with.</param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> from <paramref name="url" /> by using the the specified <paramref name="options" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Wrap ("FromUrl (url, options.GetDictionary ())")]
		[return: NullAllowed]
		CIImage FromUrl (NSUrl url, [NullAllowed] CIImageInitializationOptions options);

		/// <param name="data">Image data, in a format supported by the system.</param>
		///         <summary>Creates a new image from the specified <paramref name="data" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("imageWithData:")]
		[return: NullAllowed]
		CIImage FromData (NSData data);

		/// <param name="data">Image data, in a format supported by the system.</param>
		///         <param name="d">
		///           <para>Extra configuration options, as an NSDictionary.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Creates a new image from the specified <paramref name="data" /> and <paramref name="d" /> options dictionary.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithData:options:")]
		[return: NullAllowed]
		CIImage FromData (NSData data, [NullAllowed] NSDictionary d);

		/// <param name="data">Image data, in a format supported by the system.</param>
		///         <param name="options">Options to initialize the image with.</param>
		///         <summary>Creates a new image from the specified <paramref name="data" /> and <paramref name="options" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Wrap ("FromData (data, options.GetDictionary ())")]
		[return: NullAllowed]
		CIImage FromData (NSData data, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		/// <param name="imageBuffer">The source of the image.</param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> based on the data in the <paramref name="imageBuffer" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[MacCatalyst (13, 1)]
		[Export ("imageWithCVImageBuffer:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer);

		/// <param name="imageBuffer">To be added.</param>
		///         <param name="dict">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[MacCatalyst (13, 1)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Export ("imageWithCVImageBuffer:options:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);

		/// <param name="imageBuffer">Source of the data for the image.</param>
		///         <param name="dict">
		///           <para>A dictionary of strings to objects, holding the options for image creation.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> based on the data in <paramref name="imageBuffer" /> and applying the options in <paramref name="dict" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[MacCatalyst (13, 1)]
		[Export ("imageWithCVImageBuffer:options:")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary<NSString, NSObject> dict);

		/// <param name="imageBuffer">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> based on the data in the <paramref name="imageBuffer" /> and with the specified <paramref name="options" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[MacCatalyst (13, 1)]
		[Wrap ("FromImageBuffer (imageBuffer, options.GetDictionary ())")]
		CIImage FromImageBuffer (CVImageBuffer imageBuffer, CIImageInitializationOptions options);

		/// <param name="buffer">To be added.</param>
		///         <summary>Creates a new image from the data that is contained in <paramref name="buffer" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithCVPixelBuffer:")]
		CIImage FromImageBuffer (CVPixelBuffer buffer);

		/// <param name="buffer">To be added.</param>
		///         <param name="dict">
		///           <para>Extra configuration options, as an NSDictionary.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Creates a new image from the data that is contained in <paramref name="buffer" /> by using the options that are specified in <paramref name="dict" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithCVPixelBuffer:options:")]
		CIImage FromImageBuffer (CVPixelBuffer buffer, [NullAllowed] NSDictionary dict);

		/// <param name="buffer">To be added.</param>
		///         <param name="options">Options to initialize the image with.</param>
		///         <summary>Creates a new image from the data that is contained in <paramref name="buffer" /> by using the specified <paramref name="options" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("FromImageBuffer (buffer, options.GetDictionary ())")]
		CIImage FromImageBuffer (CVPixelBuffer buffer, [NullAllowed] CIImageInitializationOptions options);

		/// <param name="surface">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithIOSurface:")]
		CIImage FromSurface (IOSurface.IOSurface surface);

		/// <param name="surface">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("imageWithIOSurface:options:")]
		CIImage FromSurface (IOSurface.IOSurface surface, [NullAllowed] NSDictionary options);

		/// <param name="surface">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("FromSurface (surface, options.GetDictionary ())")]
		CIImage FromSurface (IOSurface.IOSurface surface, CIImageInitializationOptions options);

		/// <param name="color">To be added.</param>
		///         <summary>Creates a new single-color image.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("imageWithColor:")]
		CIImage ImageWithColor (CIColor color);

		/// <summary>The empty image.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[Static]
		[Export ("emptyImage")]
		CIImage EmptyImage { get; }

		/// <param name="image">CoreGraphics image.</param>
		/// <summary>Initializes a CoreImage Image from a CoreGraphics bitmap representation</summary>
		/// <remarks>
		///         </remarks>
		[Export ("initWithCGImage:")]
		NativeHandle Constructor (CGImage image);

		/// <param name="image">CoreGraphics image.</param>
		/// <param name="d">
		///           <para>Metadata to initialize with, as an NSDictionary.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Initializes a CoreImage Image from a CoreGraphics bitmap representation</summary>
		/// <remarks>
		///         </remarks>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGImage:options:")]
		NativeHandle Constructor (CGImage image, [NullAllowed] NSDictionary d);

		/// <param name="image">CoreGraphics image.</param>
		/// <param name="options">Options to initialize the image with.</param>
		/// <summary>Initializes a CoreImage Image from a CoreGraphics bitmap representation</summary>
		/// <remarks>
		///         </remarks>
		[Wrap ("this (image, options.GetDictionary ())")]
		NativeHandle Constructor (CGImage image, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGImageSource:index:options:")]
		NativeHandle Constructor (CGImageSource source, nuint index, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("this (source, index, options.GetDictionary ())")]
		NativeHandle Constructor (CGImageSource source, nuint index, CIImageInitializationOptionsWithMetadata options);

		/// <param name="layer">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CIImage (CGImage)' instead.")]
		[Export ("initWithCGLayer:")]
		NativeHandle Constructor (CGLayer layer);

		/// <param name="layer">To be added.</param>
		/// <param name="d">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CIImage (CGImage)' instead.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithCGLayer:options:")]
		NativeHandle Constructor (CGLayer layer, [NullAllowed] NSDictionary d);

		/// <param name="layer">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Wrap ("this (layer, options.GetDictionary ())")]
		NativeHandle Constructor (CGLayer layer, [NullAllowed] CIImageInitializationOptions options);

		/// <param name="data">Image data, in a format supported by the system.</param>
		/// <summary>Creates a new CIImage from the specified data.   The image data must be premultiplied.</summary>
		/// <remarks>
		///         </remarks>
		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);

		/// <param name="data">Image data, in a format supported by the system.</param>
		/// <param name="d">
		///           <para>Extra configuration options, as an NSDictionary.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Creates a new CIImage from the specified data.   The image data must be premultiplied.</summary>
		/// <remarks>
		///         </remarks>
		[Export ("initWithData:options:")]
		NativeHandle Constructor (NSData data, [NullAllowed] NSDictionary d);

		/// <param name="data">Image data, in a format supported by the system.</param>
		/// <param name="options">Options to initialize the image with.</param>
		/// <summary>Creates a new CIImage from the specified data.   The image data must be premultiplied.</summary>
		/// <remarks>
		///         </remarks>
		[Wrap ("this (data, options.GetDictionary ())")]
		NativeHandle Constructor (NSData data, [NullAllowed] CIImageInitializationOptionsWithMetadata options);

		/// <param name="d">To be added.</param>
		/// <param name="bytesPerRow">To be added.</param>
		/// <param name="size">To be added.</param>
		/// <param name="pixelFormat">To be added.</param>
		/// <param name="colorSpace">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithBitmapData:bytesPerRow:size:format:colorSpace:")]
		NativeHandle Constructor (NSData d, nint bytesPerRow, CGSize size, int /* CIFormat = int */ pixelFormat, [NullAllowed] CGColorSpace colorSpace);

		/// <param name="glTextureName">To be added.</param>
		/// <param name="size">To be added.</param>
		/// <param name="flipped">To be added.</param>
		/// <param name="colorSpace">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.TvOS, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("initWithTexture:size:flipped:colorSpace:")]
		NativeHandle Constructor (int /* unsigned int */ glTextureName, CGSize size, bool flipped, [NullAllowed] CGColorSpace colorSpace);

		/// <param name="url">Location of the image data.</param>
		/// <summary>Initializes a CoreImage image from the contents of the file pointed by the specified url.</summary>
		/// <remarks>
		///         </remarks>
		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl url);

		/// <param name="url">Location of the image data.</param>
		/// <param name="d">
		///           <para>Extra configuration options, as an NSDictionary.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Initializes a CoreImage image from the contents of the file pointed by the specified url.</summary>
		/// <remarks>
		///         </remarks>
		[Export ("initWithContentsOfURL:options:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] NSDictionary d);

		/// <param name="url">Location of the image data.</param>
		/// <param name="options">Options to initialize the image with.</param>
		/// <summary>Initializes a CoreImage image from the contents of the file pointed by the specified url.</summary>
		/// <remarks>
		///         </remarks>
		[Wrap ("this (url, options.GetDictionary ())")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] CIImageInitializationOptions options);

		/// <param name="surface">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithIOSurface:")]
		NativeHandle Constructor (IOSurface.IOSurface surface);

		/// <param name="surface">To be added.</param>
		/// <param name="options">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithIOSurface:options:")]
		NativeHandle Constructor (IOSurface.IOSurface surface, [NullAllowed] NSDictionary options);

		/// <param name="surface">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("this (surface, options.GetDictionary ())")]
		NativeHandle Constructor (IOSurface.IOSurface surface, [NullAllowed] CIImageInitializationOptions options);

		/// <param name="imageBuffer">CoreVideo image buffer.</param>
		/// <summary>Initializes a CoreImage image from the contents of the specified CoreVideo image buffer.</summary>
		/// <remarks>
		///         </remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithCVImageBuffer:")]
		NativeHandle Constructor (CVImageBuffer imageBuffer);

		/// <param name="imageBuffer">Holds the data that is the basis of the image.</param>
		/// <param name="dict">Dictionary of strings to objects, holding the options to be applied during construction. (See <see cref="CoreImage.CIImageInitializationOptions" />)<para tool="nullallowed">This parameter can be <see langword="null" />.</para></param>
		/// <summary>Constructs a <see cref="CoreImage.CIImage" /> using the options in <paramref name="dict" />.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithCVImageBuffer:options:")]
		NativeHandle Constructor (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary<NSString, NSObject> dict);

		/// <param name="imageBuffer">To be added.</param>
		/// <param name="dict">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Internal] // This overload is needed for our strong dictionary support (but only for Unified, since for Classic the generic version is transformed to this signature)
		[Sealed]
		[Export ("initWithCVImageBuffer:options:")]
		NativeHandle Constructor (CVImageBuffer imageBuffer, [NullAllowed] NSDictionary dict);

		/// <param name="imageBuffer">CoreVideo image buffer.</param>
		/// <param name="options">Options to initialize the image with.</param>
		/// <summary>Initializes a CoreImage image from the contents of the specified CoreVideo image buffer.</summary>
		/// <remarks>
		///         </remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("this (imageBuffer, options.GetDictionary ())")]
		NativeHandle Constructor (CVImageBuffer imageBuffer, [NullAllowed] CIImageInitializationOptions options);

		/// <param name="buffer">The pixel buffer that supplies the data for the image.</param>
		/// <summary>Constructs a <see cref="CoreImage.CIImage" /> with the supplied <paramref name="buffer" /> data.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithCVPixelBuffer:")]
		NativeHandle Constructor (CVPixelBuffer buffer);

		/// <param name="buffer">The data that forms the basis of the image.</param>
		/// <param name="dict">
		///           <para>A dictionary of strings to objects defining the options to be applied during construction. (See <see cref="CoreImage.CIImageInitializationOptions" />).</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Constructs a <see cref="CoreImage.CIImage" /> from the data in <paramref name="buffer" />, applying the options specified in <paramref name="dict" />.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithCVPixelBuffer:options:")]
		NativeHandle Constructor (CVPixelBuffer buffer, [NullAllowed] NSDictionary dict);

		/// <param name="buffer">Holds the data that is the basis of the image.</param>
		/// <param name="options">The options to be applied during initialization.</param>
		/// <summary>Constructs a <see cref="CoreImage.CIImage" /> using <paramref name="options" />.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("this (buffer, options.GetDictionary ())")]
		NativeHandle Constructor (CVPixelBuffer buffer, [NullAllowed] CIImageInitializationOptions options);

		/// <param name="color">Color to use for the image.</param>
		/// <summary>Creates an image with infinite dimensions that is filled with the specified color.</summary>
		/// <remarks>
		///         </remarks>
		[Export ("initWithColor:")]
		NativeHandle Constructor (CIColor color);

		/// <param name="texture">The <see cref="Metal.IMTLTexture" /> that is the basis for the <see cref="CoreImage.CIImage" />.</param>
		/// <param name="options">
		///           <para>A dictionary of strings to objects that hold the configuration options.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Constructs a <see cref="CoreImage.CIImage" /> using the <paramref name="options" />.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithMTLTexture:options:")]
		NativeHandle Constructor (IMTLTexture texture, [NullAllowed] NSDictionary options);

		/// <param name="imageRep">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithBitmapImageRep:")]
		NativeHandle Constructor (NSImageRep imageRep);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("drawAtPoint:fromRect:operation:fraction:")]
		void Draw (CGPoint point, CGRect srcRect, NSCompositingOperation op, nfloat delta);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("drawInRect:fromRect:operation:fraction:")]
		void Draw (CGRect dstRect, CGRect srcRect, NSCompositingOperation op, nfloat delta);

		/// <param name="matrix">To be added.</param>
		///         <summary>Returns a new image that results from applying the affine transform <paramref name="matrix" /> to this <see cref="CoreImage.CIImage" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("imageByApplyingTransform:")]
		CIImage ImageByApplyingTransform (CGAffineTransform matrix);

		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingTransform:highQualityDownsample:")]
		CIImage ImageByApplyingTransform (CGAffineTransform matrix, bool highQualityDownsample);

		/// <param name="r">To be added.</param>
		///         <summary>Creates a new image by cropping this <see cref="CoreImage.CIImage" /> to the rectangle <paramref name="r" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("imageByCroppingToRect:")]
		CIImage ImageByCroppingToRect (CGRect r);

		/// <summary>Returns the extent of the image.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[Export ("extent")]
		CGRect Extent { get; }

		[Export ("properties"), Internal]
		NSDictionary WeakProperties { get; }

		/// <summary>Gets a collection of metadata for this <see cref="CoreImage.CIImage" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakProperties")]
		CoreGraphics.CGImageProperties Properties { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("definition")]
		CIFilterShape Definition { get; }

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatRGBA16")]
		[Obsolete ("Use 'CIFormat.Rgba16.GetConstant ()' instead.")]
		int FormatRGBA16 { get; } /* CIFormat = int */

		/// <summary>Represents the value associated with the constant kCIFormatARGB8</summary>
		///         <value>
		///         </value>
		///         <remarks>32-bit color in ARGB format.</remarks>
		[Field ("kCIFormatARGB8")]
		[Obsolete ("Use 'CIFormat.Argb8.GetConstant ()' instead.")]
		int FormatARGB8 { get; } /* CIFormat = int */

		/// <summary>Represents the value associated with the constant kCIFormatRGBAh</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[Field ("kCIFormatRGBAh")]
		[Obsolete ("Use 'CIFormat.Rgbah.GetConstant ()' instead.")]
		int FormatRGBAh { get; } /* CIFormat = int */

		/// <summary>Represents the value associated with the constant kCIFormatRGBAf</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatRGBAf")]
		[Obsolete ("Use 'CIFormat.Rgbaf.GetConstant ()' instead.")]
		int FormatRGBAf { get; } /* CIFormat = int */

		/// <summary>Represents the value associated with the constant kCIFormatBGRA8</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[Field ("kCIFormatBGRA8")]
		[Obsolete ("Use 'CIFormat.Bgra8.GetConstant ()' instead.")]
		int FormatBGRA8 { get; } /* CIFormat = int */

		/// <summary>Represents the value associated with the constant kCIFormatRGBA8</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[Field ("kCIFormatRGBA8")]
		[Obsolete ("Use 'CIFormat.Rgba8.GetConstant ()' instead.")]
		int FormatRGBA8 { get; } /* CIFormat = int */

		/// <summary>Represents the value associated with the constant kCIFormatABGR8.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatABGR8")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.Abgr8.GetConstant ()' instead.")]
		int FormatABGR8 { get; }

		/// <summary>Represents the value associated with the constant kCIFormatA8.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatA8")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.A8.GetConstant ()' instead.")]
		int FormatA8 { get; }

		/// <summary>Represents the value associated with the constant kCIFormatA16.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatA16")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.A16.GetConstant ()' instead.")]
		int FormatA16 { get; }

		/// <summary>Represents the value associated with the constant kCIFormatAh.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatAh")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.Ah.GetConstant ()' instead.")]
		int FormatAh { get; }

		/// <summary>Represents the value associated with the constant kCIFormatAf.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatAf")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.Af.GetConstant ()' instead.")]
		int FormatAf { get; }

		/// <summary>Represents the value associated with the constant kCIFormatR8.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatR8")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.R8.GetConstant ()' instead.")]
		int FormatR8 { get; }

		/// <summary>Represents the value associated with the constant kCIFormatR16.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatR16")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.R16.GetConstant ()' instead.")]
		int FormatR16 { get; }

		/// <summary>Represents the value associated with the constant kCIFormatRh.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatRh")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.Rh.GetConstant ()' instead.")]
		int FormatRh { get; }

		/// <summary>Represents the value associated with the constant kCIFormatRf.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatRf")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.Rf.GetConstant ()' instead.")]
		int FormatRf { get; }

		/// <summary>Represents the value associated with the constant kCIFormatRG8.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatRG8")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.RG8.GetConstant ()' instead.")]
		int FormatRG8 { get; }

		/// <summary>Represents the value associated with the constant kCIFormatRG16.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatRG16")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.RG16.GetConstant ()' instead.")]
		int FormatRG16 { get; }

		/// <summary>Represents the value associated with the constant kCIFormatRGh.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatRGh")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.RGh.GetConstant ()' instead.")]
		int FormatRGh { get; }

		/// <summary>Represents the value associated with the constant kCIFormatRGf.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCIFormatRGf")]
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'CIFormat.RGf.GetConstant ()' instead.")]
		int FormatRGf { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatL8")]
		[Obsolete ("Use 'CIFormat.L8.GetConstant ()' instead.")]
		int FormatL8 { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatL16")]
		[Obsolete ("Use 'CIFormat.L16.GetConstant ()' instead.")]
		int FormatL16 { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLh")]
		[Obsolete ("Use 'CIFormat.Lh.GetConstant ()' instead.")]
		int FormatLh { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLf")]
		[Obsolete ("Use 'CIFormat.Lf.GetConstant ()' instead.")]
		int FormatLf { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLA8")]
		[Obsolete ("Use 'CIFormat.LA8.GetConstant ()' instead.")]
		int FormatLA8 { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLA16")]
		[Obsolete ("Use 'CIFormat.LA16.GetConstant ()' instead.")]
		int FormatLA16 { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLAh")]
		[Obsolete ("Use 'CIFormat.LAh.GetConstant ()' instead.")]
		int FormatLAh { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLAf")]
		[Obsolete ("Use 'CIFormat.LAf.GetConstant ()' instead.")]
		int FormatLAf { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIFormatRGB10")]
		[Obsolete ("Use 'CIFormat.Rgb10.GetConstant ()' instead.")]
		int FormatRgb10 { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIFormatRGBX16")]
		[Obsolete ("Use 'CIFormat.RgbX16.GetConstant ()' instead.")]
		int FormatRgbX16 { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIFormatRGBXf")]
		[Obsolete ("Use 'CIFormat.RgbXf.GetConstant ()' instead.")]
		int FormatRgbXf { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIFormatRGBXh")]
		[Obsolete ("Use 'CIFormat.RgbXh.GetConstant ()' instead.")]
		int FormatRgbXh { get; }
#endif // XAMCORE_5_0

		// UIKit extensions
		/// <param name="image">UIKit image.</param>
		/// <summary>Initializes a CoreImage image from a UIKit image.</summary>
		/// <remarks>
		///         </remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("initWithImage:")]
		NativeHandle Constructor (UIImage image);

		/// <param name="image">UIKit image.</param>
		/// <param name="options">
		///           <para>Extra configuration options, as an NSDictionary.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Initializes a CoreImage image from a UIKit image.</summary>
		/// <remarks>
		///         </remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("initWithImage:options:")]
		NativeHandle Constructor (UIImage image, [NullAllowed] NSDictionary options);

		/// <param name="image">UIKit image.</param>
		/// <param name="options">Options to initialize the image with.</param>
		/// <summary>Initializes a CoreImage image from a UIKit image.</summary>
		/// <remarks>
		///         </remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("this (image, options.GetDictionary ())")]
		NativeHandle Constructor (UIImage image, [NullAllowed] CIImageInitializationOptions options);

		[Field ("kCIImageAutoAdjustFeatures"), Internal]
		NSString AutoAdjustFeaturesKey { get; }

		[Field ("kCIImageAutoAdjustRedEye"), Internal]
		NSString AutoAdjustRedEyeKey { get; }

		[Field ("kCIImageAutoAdjustEnhance"), Internal]
		NSString AutoAdjustEnhanceKey { get; }

		[Export ("autoAdjustmentFiltersWithOptions:"), Internal]
		NSArray _GetAutoAdjustmentFilters ([NullAllowed] NSDictionary opts);

		/// <param name="im">To be added.</param>
		///         <param name="r">To be added.</param>
		///         <summary>Gets a rectangle that describes the region in <paramref name="im" />, an image in the transformation list, that corresponds to <paramref name="r" /> in this <see cref="CoreImage.CIImage" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("regionOfInterestForImage:inRect:")]
		CGRect GetRegionOfInterest (CIImage im, CGRect r);

		//
		// iOS 8.0
		//
		/// <param name="orientation">To be added.</param>
		///         <summary>Creates a new image by applying the <paramref name="orientation" /> to this <see cref="CoreImage.CIImage" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingOrientation:")]
		CIImage CreateWithOrientation (CIImageOrientation orientation);

		/// <param name="orientation">To be added.</param>
		///         <summary>Gets a transformation that results in <paramref name="orientation" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageTransformForOrientation:")]
		CGAffineTransform GetImageTransform (CIImageOrientation orientation);

		/// <summary>Creates a new image by clamping the current image to the rectangle that is defined by its <see cref="CoreImage.CIImage.Extent" /> property.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByClampingToExtent")]
		CIImage CreateByClampingToExtent ();

		/// <param name="dest">To be added.</param>
		///         <summary>Creates a new image by compositing this <see cref="CoreImage.CIImage" /> over <paramref name="dest" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByCompositingOverImage:")]
		CIImage CreateByCompositingOverImage (CIImage dest);

		/// <param name="filterName">To be added.</param>
		///         <param name="inputParameters">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Creates a new image by applying <paramref name="filterName" /> to this <see cref="CoreImage.CIImage" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingFilter:withInputParameters:")]
		CIImage CreateByFiltering (string filterName, [NullAllowed] NSDictionary inputParameters);

		/// <param name="filterName">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingFilter:")]
		CIImage CreateByFiltering (string filterName);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageBySamplingLinear")]
		CIImage CreateBySamplingLinear ();

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageBySamplingNearest")]
		CIImage CreateBySamplingNearest ();

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAutoAdjustCrop"), Internal]
		NSString AutoAdjustCrop { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageAutoAdjustLevel"), Internal]
		NSString AutoAdjustLevel { get; }

		/// <summary>If not <see langword="null" />, the URL from which the image was loaded.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("url")]
		NSUrl Url { get; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorSpace")]
		CGColorSpace ColorSpace { get; }

		[MacCatalyst (13, 1)]
		[Static, Internal]
		[Export ("imageWithImageProvider:size::format:colorSpace:options:")]
		CIImage FromProvider (ICIImageProvider provider, nuint width, nuint height, int format, [NullAllowed] CGColorSpace colorSpace, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("initWithImageProvider:size::format:colorSpace:options:")]
		NativeHandle Constructor (ICIImageProvider provider, nuint width, nuint height, int f, [NullAllowed] CGColorSpace colorSpace, [NullAllowed] NSDictionary options);

		/// <param name="texture">The texture providing the basis of hte <see cref="CoreImage.CIImage" />.</param>
		///         <param name="options">
		///           <para>A dictionary of strings to objects, holding the creation options.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> from <paramref name="texture" />, applying the creation options specified in <paramref name="options" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithMTLTexture:options:")]
		[return: NullAllowed]
		CIImage FromMetalTexture (IMTLTexture texture, [NullAllowed] NSDictionary<NSString, NSObject> options);

		/// <param name="rect">The clipping rectangle.</param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> of infinite extent by cropping this <see cref="CoreImage.CIImage" /> to the <paramref name="rect" /> and then extending the pixels at the edges to infinity.</summary>
		///         <returns>A <see cref="CoreImage.CIImage" /> of infinite extent.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByClampingToRect:")]
		CIImage CreateByClamping (CGRect rect);

		/// <param name="colorSpace">The <see cref="CoreGraphics.CGColorSpace" /> to be matched from.</param>
		///         <summary>Creates a new <see cref="CoreGraphics.CGImage" /> by matching colors from <paramref name="colorSpace" /> into the working color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByColorMatchingColorSpaceToWorkingSpace:")]
		[return: NullAllowed]
		CIImage CreateByColorMatchingColorSpaceToWorkingSpace (CGColorSpace colorSpace);

		/// <param name="colorSpace">The <see cref="CoreGraphics.CGColorSpace" /> to be matched.</param>
		///         <summary>Creates a new <see cref="CoreGraphics.CGImage" /> by matching colors from the working space into colors in the specified <paramref name="colorSpace" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByColorMatchingWorkingSpaceToColorSpace:")]
		[return: NullAllowed]
		CIImage CreateByColorMatchingWorkingSpaceToColorSpace (CGColorSpace colorSpace);

		/// <summary>Creates a new image whose RGB values are created by multiplying this image's RGB values by this image's alpha value.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByPremultiplyingAlpha")]
		CIImage CreateByPremultiplyingAlpha ();

		/// <summary>Creates a new image whose RGB values are created by dividing this image's RGB values by this image's alpha value.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByUnpremultiplyingAlpha")]
		CIImage CreateByUnpremultiplyingAlpha ();

		/// <param name="extent">The area within the image to have alpha 1.0.</param>
		///         <summary>Creates a new image by copying <c>this</c>, setting the alpha of pixels within <paramref name="extent" /> to 1.0 and setting those outside to 0.0.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageBySettingAlphaOneInExtent:")]
		CIImage CreateBySettingAlphaOne (CGRect extent);

		/// <param name="sigma">The standard deviation defining the 2D Gaussian.</param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> by applying a Gaussian blur with the provided <paramref name="sigma" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>
		///           <para>The 2D Gaussian is defined as:</para>
		///           <para>
		///             <img href="~/xml/CoreImage/_images/CoreImage.GaussianEq.png" alt="Result of applying the filter." />
		///           </para>
		///           <para>Where zeta (<c>z</c>) is a vector holding the pixel coordinates and mu (μ) is a vector holding the mean of the Gaussian in either direction.</para>
		///           <para>The <paramref name="sigma" /> defines the rate of falloff of the Gaussian. Smaller <paramref name="sigma" /> values blur over fewer pixels.</para>
		///         </remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingGaussianBlurWithSigma:")]
		CIImage CreateByApplyingGaussianBlur (double sigma);

		/// <param name="properties">To be added.</param>
		///         <summary>Creates a new <see cref="CoreImage.CIImage" /> by copying <c>this</c>, and applying the <paramref name="properties" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageBySettingProperties:")]
		CIImage CreateBySettingProperties (NSDictionary properties);

		/// <summary>The image data, as a <see cref="CoreVideo.CVPixelBuffer" />.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		/// <summary>Copies the <see cref="CoreImage.CIImage" /> into a new <see cref="CoreGraphics.CGImage" />.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("CGImage")]
		CGImage CGImage { get; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("depthData")]
		AVDepthData DepthData { get; }

		/// <param name="orientation">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingCGOrientation:")]
		CIImage CreateByApplyingOrientation (CGImagePropertyOrientation orientation);

		/// <param name="orientation">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageTransformForCGOrientation:")]
		CGAffineTransform GetImageTransform (CGImagePropertyOrientation orientation);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByInsertingIntermediate")]
		CIImage CreateByInsertingIntermediate ();

		/// <param name="cache">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("imageByInsertingIntermediate:")]
		CIImage CreateByInsertingIntermediate (bool cache);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("imageByApplyingGainMap:")]
		CIImage CreateByApplyingGainMap (CIImage gainMap);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("imageByApplyingGainMap:headroom:")]
		CIImage CreateByApplyingGainMap (CIImage gainMap, float headroom);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("opaque")]
		bool Opaque { [Bind ("isOpaque")] get; }
		// CIImage_AVPortraitEffectsMatte category

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("portraitEffectsMatte")]
		AVPortraitEffectsMatte PortraitEffectsMatte { get; }

		/// <param name="matte">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithPortaitEffectsMatte:options:")] // selector typo, rdar filled 42894821
		NativeHandle Constructor (AVPortraitEffectsMatte matte, [NullAllowed] NSDictionary options);

		/// <param name="matte">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithPortaitEffectsMatte:")] // selector typo, rdar filled 42894821
		NativeHandle Constructor (AVPortraitEffectsMatte matte);

		/// <param name="matte">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithPortaitEffectsMatte:options:")] // selector typo, rdar filled 42894821
		[return: NullAllowed]
		CIImage FromPortraitEffectsMatte (AVPortraitEffectsMatte matte, [NullAllowed] NSDictionary options);

		/// <param name="matte">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithPortaitEffectsMatte:")] // selector typo, rdar filled 42894821
		[return: NullAllowed]
		CIImage FromPortraitEffectsMatte (AVPortraitEffectsMatte matte);

		// CIImage_AVSemanticSegmentationMatte

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("semanticSegmentationMatte")]
		AVSemanticSegmentationMatte SemanticSegmentationMatte { get; }

		[MacCatalyst (13, 1)]
		[Export ("initWithSemanticSegmentationMatte:options:")]
		NativeHandle Constructor (AVSemanticSegmentationMatte matte, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Export ("initWithSemanticSegmentationMatte:")]
		NativeHandle Constructor (AVSemanticSegmentationMatte matte);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithSemanticSegmentationMatte:options:")]
		[return: NullAllowed]
		CIImage FromSemanticSegmentationMatte (AVSemanticSegmentationMatte matte, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithSemanticSegmentationMatte:")]
		[return: NullAllowed]
		CIImage FromSemanticSegmentationMatte (AVSemanticSegmentationMatte matte);

		// CIImage_AVDepthData category

		/// <param name="data">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithDepthData:options:")]
		NativeHandle Constructor (AVDepthData data, [NullAllowed] NSDictionary options);

		/// <param name="data">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithDepthData:")]
		NativeHandle Constructor (AVDepthData data);

		/// <param name="data">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithDepthData:options:")]
		[return: NullAllowed]
		CIImage FromDepthData (AVDepthData data, [NullAllowed] NSDictionary options);

		/// <param name="data">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithDepthData:")]
		[return: NullAllowed]
		CIImage FromDepthData (AVDepthData data);

		// colors

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("blackImage", ArgumentSemantic.Strong)]
		CIImage BlackImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("whiteImage", ArgumentSemantic.Strong)]
		CIImage WhiteImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("grayImage", ArgumentSemantic.Strong)]
		CIImage GrayImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("redImage", ArgumentSemantic.Strong)]
		CIImage RedImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("greenImage", ArgumentSemantic.Strong)]
		CIImage GreenImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("blueImage", ArgumentSemantic.Strong)]
		CIImage BlueImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("cyanImage", ArgumentSemantic.Strong)]
		CIImage CyanImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("magentaImage", ArgumentSemantic.Strong)]
		CIImage MagentaImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("yellowImage", ArgumentSemantic.Strong)]
		CIImage YellowImage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("clearImage", ArgumentSemantic.Strong)]
		CIImage ClearImage { get; }

		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("imageByConvertingWorkingSpaceToLab")]
		CIImage ConvertWorkingSpaceToLab ();

		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("imageByConvertingLabToWorkingSpace")]
		CIImage ConvertLabToWorkingSpace ();

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("contentHeadroom")]
		float ContentHeadroom { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("metalTexture"), NullAllowed]
		IMTLTexture MetalTexture { get; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("imageByInsertingTiledIntermediate")]
		CIImage CreateByInsertingTiledIntermediate ();

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("imageBySettingContentHeadroom:")]
		CIImage CreateBySettingContentHeadroom (float headroom);

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("imageBySettingContentAverageLightLevel:")]
		CIImage CreateBySettingContentAverageLightLevel (float average);

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("contentAverageLightLevel")]
		float ContentAverageLightLevel { get; }
	}

	/// <summary>An enumeration whose values specify pixel formats.</summary>
	[BackingFieldType (typeof (int))]
	public enum CIFormat {
#if !XAMCORE_5_0
		/// <summary>Value that represents the kCIFormatARGB8 pixel format.</summary>
		[Obsolete ("Use 'Argb8' instead.")]
		ARGB8 = Argb8,
		/// <summary>Value that represents the kCIFormatRGBAh pixel format.</summary>
		[Obsolete ("Use 'Rgbah' instead.")]
		RGBAh = Rgbah,
		/// <summary>Value that represents the kCIFormatkRGBA16 pixel format.</summary>
		[Obsolete ("Use 'Rgba16' instead.")]
		[NoiOS, NoTV, NoMacCatalyst]
		RGBA16 = Rgba16,
		/// <summary>Value that represents the kCIFormatkRGBAf pixel format.</summary>
		[Obsolete ("Use 'Rgbaf' instead.")]
		[NoiOS, NoTV, NoMacCatalyst]
		RGBAf = 3,
		/// <summary>Value that represents the kCIFormatkRGBAf pixel format.</summary>
		[Obsolete ("Use 'Rgbaf' instead.")]
		kRGBAf = Rgbaf,
		/// <summary>Value that represents the kCIFormatkBGRA8 pixel format.</summary>
		[Obsolete ("Use 'Bgra8' instead.")]
		kBGRA8 = Bgra8,
		/// <summary>Value that represents the kCIFormatkRGBA8 pixel format.</summary>
		[Obsolete ("Use 'Rgba8' instead.")]
		kRGBA8 = Rgba8,
		/// <summary>Value that represents the kCIFormatABGR8 pixel format.</summary>
		[Obsolete ("Use 'Abgr8' instead.")]
		ABGR8 = Abgr8,
#endif // !XAMCORE_5_0

		/// <summary>Represents the value associated with the constant kCIFormatRGBA16.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatRGBA16")]
		Rgba16 = 2, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatARGB8</summary>
		[Field ("kCIFormatARGB8")]
		Argb8 = 0, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRGBAh</summary>
		[Field ("kCIFormatRGBAh")]
		Rgbah = 1, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRGBAf</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatRGBAf")]
		Rgbaf = 4, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatBGRA8</summary>
		[Field ("kCIFormatBGRA8")]
		Bgra8 = 5, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRGBA8</summary>
		[Field ("kCIFormatRGBA8")]
		Rgba8 = 6, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatABGR8.</summary>
		[Field ("kCIFormatABGR8")]
		[MacCatalyst (13, 1)]
		Abgr8 = 7, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatA8.</summary>
		[Field ("kCIFormatA8")]
		[MacCatalyst (13, 1)]
		A8 = 11, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatA16.</summary>
		[Field ("kCIFormatA16")]
		[MacCatalyst (13, 1)]
		A16 = 12, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatAh.</summary>
		[Field ("kCIFormatAh")]
		[MacCatalyst (13, 1)]
		Ah = 13, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatAf.</summary>
		[Field ("kCIFormatAf")]
		[MacCatalyst (13, 1)]
		Af = 14, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatR8.</summary>
		[Field ("kCIFormatR8")]
		[MacCatalyst (13, 1)]
		R8 = 15, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatR16.</summary>
		[Field ("kCIFormatR16")]
		[MacCatalyst (13, 1)]
		R16 = 16, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRh.</summary>
		[Field ("kCIFormatRh")]
		[MacCatalyst (13, 1)]
		Rh = 17, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRf.</summary>
		[Field ("kCIFormatRf")]
		[MacCatalyst (13, 1)]
		Rf = 18, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRG8.</summary>
		[Field ("kCIFormatRG8")]
		[MacCatalyst (13, 1)]
		RG8 = 19, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRG16.</summary>
		[Field ("kCIFormatRG16")]
		[MacCatalyst (13, 1)]
		RG16 = 20, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRGh.</summary>
		[Field ("kCIFormatRGh")]
		[MacCatalyst (13, 1)]
		RGh = 21, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatRGf.</summary>
		[Field ("kCIFormatRGf")]
		[MacCatalyst (13, 1)]
		RGf = 22, // same as value in old enum

		/// <summary>Represents the value associated with the constant kCIFormatL8.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatL8")]
		L8 = 23,

		/// <summary>Represents the value associated with the constant kCIFormatL16.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatL16")]
		L16 = 24,

		/// <summary>Represents the value associated with the constant kCIFormatLh.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLh")]
		Lh = 25,

		/// <summary>Represents the value associated with the constant kCIFormatLf.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLf")]
		Lf = 26,

		/// <summary>Represents the value associated with the constant kCIFormatLA8.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLA8")]
		LA8 = 27,

		/// <summary>Represents the value associated with the constant kCIFormatLA16.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLA16")]
		LA16 = 28,

		/// <summary>Represents the value associated with the constant kCIFormatLAh.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLAh")]
		LAh = 29,

		/// <summary>Represents the value associated with the constant kCIFormatLAf.</summary>
		[MacCatalyst (13, 1)]
		[Field ("kCIFormatLAf")]
		LAf = 30,

		/// <summary>Represents the value associated with the constant kCIFormatRGB10.</summary>
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIFormatRGB10")]
		Rgb10 = 31,

		/// <summary>Represents the value associated with the constant kCIFormatRGBX16.</summary>
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIFormatRGBX16")]
		RgbX16 = 32,

		/// <summary>Represents the value associated with the constant kCIFormatRGBXf.</summary>
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIFormatRGBXf")]
		RgbXf = 33,

		/// <summary>Represents the value associated with the constant kCIFormatRGBXh.</summary>
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Field ("kCIFormatRGBXh")]
		RgbXh = 34,

		/// <summary>Represents the value associated with the constant kCIFormatRGBX8</summary>
		[Field ("kCIFormatRGBX8")]
		Rgbx8 = 35,
	}

	interface ICIImageProcessorInput { }

	/// <summary>The input to a <see cref="CoreImage.CIImageProcessorKernel" />.</summary>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface CIImageProcessorInput {
		/// <summary>The region of interest in the input image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("region")]
		CGRect Region { get; }

		/// <summary>The number of bytes in a single row of the input image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("bytesPerRow")]
		nuint BytesPerRow { get; }

		/// <summary>The pixel format of the input image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("format")]
		CIFormat Format { get; }

		/// <summary>The memory address of the data buffer.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("baseAddress")]
		IntPtr BaseAddress { get; }

		/// <summary>The input <see cref="CoreVideo.CVPixelBuffer" />.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		/// <summary>The input <see cref="Metal.IMTLTexture" />.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("metalTexture")]
		IMTLTexture MetalTexture { get; }

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("surface")]
		IOSurface.IOSurface Surface { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("digest")]
		ulong Digest { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("roiTileCount")]
		nuint RoiTileCount { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("roiTileIndex")]
		nuint RoiTileIndex { get; }
	}

	interface ICIImageProcessorOutput { }

	/// <summary>The output of a <see cref="CoreImage.CIImageProcessorKernel" />.</summary>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface CIImageProcessorOutput {
		/// <summary>The <see cref="CoreGraphics.CGRect" /> to which the processing was applied.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("region")]
		CGRect Region { get; }

		/// <summary>The number of bytes in a single row of the output image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("bytesPerRow")]
		nuint BytesPerRow { get; }

		/// <summary>The colorspace of the output image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("format")]
		CIFormat Format { get; }

		/// <summary>The memory address of the data buffer.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("baseAddress")]
		IntPtr BaseAddress { get; }

		/// <summary>The output image, as a <see cref="CoreVideo.CVPixelBuffer" />.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		/// <summary>The Metal <see cref="Metal.IMTLTexture" /> of the output image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("metalTexture")]
		IMTLTexture MetalTexture { get; }

		/// <summary>The Metal command buffer for the output image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("metalCommandBuffer")]
		IMTLCommandBuffer MetalCommandBuffer { get; }

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("surface")]
		IOSurface.IOSurface Surface { get; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("digest")]
		ulong Digest { get; }

		/// <param name="identifier">A name that uniquely identifies the temporary surface during the processor invocation.</param>
		/// <param name="format">The pixel format for the surface.</param>
		/// <param name="width">The width of the surface, in pixels.</param>
		/// <param name="height">The height of the surface, in pixels.</param>
		/// <summary>Gets a temporary surface for use as scratch storage during image processing.</summary>
		/// <returns>A temporary surface, or <see langword="null" /> if one could not be created.</returns>
		/// <remarks>The returned surface is valid only for the duration of the current image processor invocation.</remarks>
		[Abstract]
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("temporarySurfaceWithIdentifier:format:width:height:")]
		[return: NullAllowed]
		IOSurface.IOSurface GetTemporarySurface (string identifier, CVPixelFormatType format, nuint width, nuint height);

		/// <param name="identifier">A name that uniquely identifies the temporary pixel buffer during the processor invocation.</param>
		/// <param name="format">The pixel format for the pixel buffer.</param>
		/// <param name="width">The width of the pixel buffer, in pixels.</param>
		/// <param name="height">The height of the pixel buffer, in pixels.</param>
		/// <param name="attributes">Optional pixel buffer creation attributes.</param>
		/// <summary>Gets a temporary pixel buffer for use as scratch storage during image processing.</summary>
		/// <returns>A temporary pixel buffer, or <see langword="null" /> if one could not be created.</returns>
		/// <remarks>The returned pixel buffer is valid only for the duration of the current image processor invocation.</remarks>
		[Abstract]
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("temporaryPixelBufferWithIdentifier:format:width:height:attributes:")]
		[return: NullAllowed]
		CVPixelBuffer GetTemporaryPixelBuffer (string identifier, CVPixelFormatType format, nuint width, nuint height, [NullAllowed] NSDictionary attributes);
	}

	/// <summary>Options used in various calls to <see cref="CoreImage.CIImage" /> involving <see cref="CoreImage.ICIImageProvider" /> objects.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("CIImageProviderKeys")]
	interface CIImageProviderOptions {
		/// <summary>A <see cref="Foundation.NSNumber" /> that holds an int that is the length of a tile square.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSObject TileSize { get; set; }
		/// <summary>User-supplied data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSObject UserInfo { get; set; }
	}

	[Internal]
	[Static]
	[MacCatalyst (13, 1)]
	interface CIImageProviderKeys {
		[Field ("kCIImageProviderTileSize")]
		NSString TileSizeKey { get; }

		[Field ("kCIImageProviderUserInfo")]
		NSString UserInfoKey { get; }
	}

	interface ICIImageProvider { }

#if !XAMCORE_5_0
	// Informal protocol
	/// <summary>Interface defining an image provider.</summary>
	[Protocol (IsInformal = true)]
	interface CIImageProvider {
		[Abstract]
		[Export ("provideImageData:bytesPerRow:origin::size::userInfo:")]
		unsafe void ProvideImageData (IntPtr data, nuint rowbytes, nuint x, nuint y, nuint width, nuint height, [NullAllowed] NSObject info);

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("provideImageToMTLTexture:commandBuffer:originx:originy:width:height:userInfo:")]
		void ProvideImageToMTLTexture (IMTLTexture texture, IMTLCommandBuffer commandBuffer, nuint originx, nuint originy, nuint width, nuint height, [NullAllowed] NSObject info);
	}
#endif

	/// <summary>Interface defining an image provider.</summary>
	[Category]
	[BaseType (typeof (NSObject))]
	interface NSObject_CIImageProvider {
		[Abstract]
		[Export ("provideImageData:bytesPerRow:origin::size::userInfo:")]
		void ProvideImageData (IntPtr data, nuint rowbytes, nuint x, nuint y, nuint width, nuint height, [NullAllowed] NSObject info);

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("provideImageToMTLTexture:commandBuffer:originx:originy:width:height:userInfo:")]
		void ProvideImageToMTLTexture (IMTLTexture texture, IMTLCommandBuffer commandBuffer, nuint originx, nuint originy, nuint width, nuint height, [NullAllowed] NSObject info);
	}

	/// <summary>Completion handler for deterimining a region of interest in the source image.</summary>
	delegate CGRect CIKernelRoiCallback (int /* int, not NSInteger */ index, CGRect rect);

	/// <summary>A kernel for filters that use custom GPU steps.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/QuartzCoreFramework/Classes/CIKernel_Class/index.html">Apple documentation for <c>CIKernel</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // avoid crashes
	interface CIKernel {

		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Static]
		[Export ("kernelsWithMetalString:error:")]
		[return: NullAllowed]
		CIKernel [] FromMetalSource (string source, [NullAllowed] out NSError error);

		/// <param name="coreImageShaderProgram">To be added.</param>
		///         <summary>Creates an array of new <see cref="CoreImage.CIKernel" /> from a list of named system kernel routines.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static, Export ("kernelsWithString:")]
		[return: NullAllowed]
		CIKernel [] FromProgramMultiple (string coreImageShaderProgram);

		/// <param name="coreImageShaderProgram">To be added.</param>
		///         <summary>Creates a new <see cref="CoreImage.CIKernel" /> from a list of named system kernel routines.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static, Export ("kernelWithString:")]
		[return: NullAllowed]
		CIKernel FromProgramSingle (string coreImageShaderProgram);

		/// <param name="name">To be added.</param>
		///         <param name="data">To be added.</param>
		///         <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("kernelWithFunctionName:fromMetalLibraryData:error:")]
		[return: NullAllowed]
		CIKernel FromFunction (string name, NSData data, [NullAllowed] out NSError error);

		/// <param name="name">To be added.</param>
		///         <param name="data">To be added.</param>
		///         <param name="format">To be added.</param>
		///         <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("kernelWithFunctionName:fromMetalLibraryData:outputPixelFormat:error:")]
		[return: NullAllowed]
		CIKernel FromFunction (string name, NSData data, CIFormat format, [NullAllowed] out NSError error);

		[iOS (14, 0)]
		[TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("kernelNamesFromMetalLibraryData:")]
		string [] GetKernelNamesFromMetalLibrary (NSData data);

		/// <summary>Gets the name of the <see cref="CoreImage.CIKernel" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("name")]
		string Name { get; }

		/// <param name="aMethod">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setROISelector:")]
		void SetRegionOfInterestSelector (Selector aMethod);

		/// <param name="extent">To be added.</param>
		///         <param name="callback">To be added.</param>
		///         <param name="args">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("applyWithExtent:roiCallback:arguments:")]
		[return: NullAllowed]
		CIImage ApplyWithExtent (CGRect extent, CIKernelRoiCallback callback, [NullAllowed] NSObject [] args);
	}

	/// <summary>Kernel for a GPU process that only operates on color data.</summary>
	///     
	///     <!-- 2014-09-08 Apple undocumented. Relates to https://developer.apple.com/library/prerelease/ios/documentation/GraphicsImaging/Conceptual/CoreImaging/ci_custom_filters/ci_custom_filters.html#//apple_ref/doc/uid/TP30001185-CH6-CJBFHGGC but no real route forward. -->
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreImage/Reference/CIColorKernel_Class/index.html">Apple documentation for <c>CIColorKernel</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIKernel))]
	[DisableDefaultCtor] // returns a nil handle -> instances of this type are returned from `kernel[s]WithString:`
	interface CIColorKernel {
		/// <param name="extent">To be added.</param>
		///         <param name="args">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("applyWithExtent:arguments:")]
		[return: NullAllowed]
		CIImage ApplyWithExtent (CGRect extent, [NullAllowed] NSObject [] args);

		// Note: the API is supported in iOS 8, but with iOS 9, they guarantee
		// a more derived result
		/// <param name="coreImageShaderProgram">To be added.</param>
		///         <summary>Creates a new CIColorKernel from the provided image shader program code.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[New, Static, Export ("kernelWithString:")]
		[return: NullAllowed]
		CIColorKernel FromProgramSingle (string coreImageShaderProgram);
	}

	/// <summary>A kernel for GPU transforms that operate only on geometry.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreImage/Reference/CIWarpKernel_Class/index.html">Apple documentation for <c>CIWarpKernel</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIKernel))]
	[DisableDefaultCtor] // returns a nil handle -> instances of this type are returned from `kernel[s]WithString:`
	interface CIWarpKernel {
		/// <param name="extent">To be added.</param>
		///         <param name="callback">To be added.</param>
		///         <param name="image">To be added.</param>
		///         <param name="args">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("applyWithExtent:roiCallback:inputImage:arguments:")]
		[return: NullAllowed]
		CIImage ApplyWithExtent (CGRect extent, CIKernelRoiCallback callback, CIImage image, [NullAllowed] NSObject [] args);

		// Note: the API is supported in iOS 8, but with iOS 9, they guarantee
		// a more derived result
		/// <param name="coreImageShaderProgram">To be added.</param>
		///         <summary>Creates a new CIWarpKernel from the supplied shader code.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[New, Static, Export ("kernelWithString:")]
		[return: NullAllowed]
		CIWarpKernel FromProgramSingle (string coreImageShaderProgram);
	}

	/// <summary>A feedback image processor.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/QuartzCoreFramework/Classes/CIImageAccumulator_Class/index.html">Apple documentation for <c>CIImageAccumulator</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // does not work in iOS 11 beta 4
	interface CIImageAccumulator {
		/// <param name="rect">The rectangle from which to create the accumulator.</param>
		///         <param name="format">The pixel format for the accumulator.</param>
		///         <summary>Creates and returns a new image accumulator from the specified rectangle and using the specified format.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("imageAccumulatorWithExtent:format:")]
		[return: NullAllowed]
		CIImageAccumulator FromRectangle (CGRect rect, CIFormat format);

		/// <param name="extent">To be added.</param>
		///         <param name="format">The pixel format for the accumulator.</param>
		///         <param name="colorSpace">The color space for the accumulator.</param>
		///         <summary>Creates and returns a new image accumulator from the specified rectangle and using the specified format and color space.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("imageAccumulatorWithExtent:format:colorSpace:")]
		[return: NullAllowed]
		CIImageAccumulator FromRectangle (CGRect extent, CIFormat format, CGColorSpace colorSpace);

		/// <param name="rectangle">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <summary>Creates a new image accumulator from the specified rectangle and using the specified format.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithExtent:format:")]
		NativeHandle Constructor (CGRect rectangle, CIFormat format);

		/// <param name="extent">To be added.</param>
		/// <param name="format">To be added.</param>
		/// <param name="colorSpace">To be added.</param>
		/// <summary>Creates a new image accumulator from the specified rectangle and using the specified format and color space.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithExtent:format:colorSpace:")]
		NativeHandle Constructor (CGRect extent, CIFormat format, CGColorSpace colorSpace);

		/// <summary>Gets the extent of the image that is replaced.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("extent")]
		CGRect Extent { get; }

		/// <summary>Gets the image format of the accumulator.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("format")]
		int CIImageFormat { get; } /* CIFormat = int */

		/// <param name="image">To be added.</param>
		///         <param name="dirtyRect">To be added.</param>
		///         <summary>Places the <paramref name="dirtyRect" /> portion of <paramref name="image" /> into the accumulator.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("setImage:dirtyRect:")]
		void SetImageDirty (CIImage image, CGRect dirtyRect);

		/// <summary>Clears the accumulator.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("clear")]
		void Clear ();

		//Detected properties
		/// <summary>Gets or sets the accumulator contents.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("image")]
		CIImage Image { get; set; }
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface CIPlugIn {
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'LoadNonExecutablePlugIns' for non-executable plugins instead.")]
		[Static]
		[Export ("loadAllPlugIns")]
		void LoadAllPlugIns ();

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("loadNonExecutablePlugIns")]
		void LoadNonExecutablePlugIns ();

		[Static]
		[Export ("loadNonExecutablePlugIn:")]
		void LoadNonExecutablePlugIn (NSUrl url);

		/// <param name="pluginUrl">To be added.</param>
		///         <param name="allowNonExecutable">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Static]
		[Export ("loadPlugIn:allowNonExecutable:")]
		void LoadPlugIn (NSUrl pluginUrl, bool allowNonExecutable);
	}

	/// <summary>Samples images for processing by a <see cref="CoreImage.CISampler" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/QuartzCoreFramework/Classes/CISampler_Class/index.html">Apple documentation for <c>CISampler</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CISampler : NSCopying {
		/// <param name="sourceImage">To be added.</param>
		///         <summary>Creates a new <see cref="CoreImage.CISampler" /> from the <paramref name="sourceImage" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static, Export ("samplerWithImage:")]
		CISampler FromImage (CIImage sourceImage);

		[Internal, Static]
		[Export ("samplerWithImage:options:")]
		CISampler FromImage (CIImage sourceImag, [NullAllowed] NSDictionary options);

		/// <param name="sourceImage">The image from which to sample.</param>
		/// <summary>Creates a new sampler from a source image.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithImage:")]
		NativeHandle Constructor (CIImage sourceImage);

		[DesignatedInitializer]
		[Internal, Export ("initWithImage:options:")]
		NSObject Constructor (CIImage image, [NullAllowed] NSDictionary options);

		/// <summary>The bounding shape and domain of definition of this <see cref="CoreImage.CISampler" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("definition")]
		CIFilterShape Definition { get; }

		/// <summary>The rectangle to which this <see cref="CoreImage.CISampler" /> applies.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("extent")]
		CGRect Extent { get; }

		[Field ("kCISamplerAffineMatrix", "+CoreImage"), Internal]
		NSString AffineMatrix { get; }

		[Field ("kCISamplerWrapMode", "+CoreImage"), Internal]
		NSString WrapMode { get; }

		[Field ("kCISamplerFilterMode", "+CoreImage"), Internal]
		NSString FilterMode { get; }

		[Field ("kCISamplerWrapBlack", "+CoreImage"), Internal]
		NSString WrapBlack { get; }

		[Field ("kCISamplerWrapClamp", "+CoreImage"), Internal]
		NSString WrapClamp { get; }

		[Field ("kCISamplerFilterNearest", "+CoreImage"), Internal]
		NSString FilterNearest { get; }

		[Field ("kCISamplerFilterLinear", "+CoreImage"), Internal]
		NSString FilterLinear { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCISamplerColorSpace", "+CoreImage"), Internal]
		NSString ColorSpace { get; }
	}

	/// <summary>A vector for use with Core Image objects such as <see cref="CoreImage.CIFilter" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/QuartzCoreFramework/Classes/CIVector_Class/index.html">Apple documentation for <c>CIVector</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIVector : NSSecureCoding, NSCopying {
		[Static, Internal, Export ("vectorWithValues:count:")]
		CIVector _FromValues (IntPtr values, nint count);

		/// <param name="x">To be added.</param>
		/// <summary>Creates a vector with the specified <paramref name="x" /> value.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("vectorWithX:")]
		CIVector Create (nfloat x);

		/// <param name="x">To be added.</param>
		/// <param name="y">To be added.</param>
		/// <summary>Creates a vector with the specified <paramref name="x" /> and <paramref name="y" /> values.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("vectorWithX:Y:")]
		CIVector Create (nfloat x, nfloat y);

		/// <param name="x">To be added.</param>
		/// <param name="y">To be added.</param>
		/// <param name="z">To be added.</param>
		/// <summary>Creates a vector with the specified <paramref name="x" />, <paramref name="y" />, and <paramref name="z" /> values.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("vectorWithX:Y:Z:")]
		CIVector Create (nfloat x, nfloat y, nfloat z);

		/// <param name="x">To be added.</param>
		/// <param name="y">To be added.</param>
		/// <param name="z">To be added.</param>
		/// <param name="w">To be added.</param>
		/// <summary>Creates a vector with the specified <paramref name="x" />, <paramref name="y" />, and <paramref name="z" />, and <paramref name="w" /> values.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("vectorWithX:Y:Z:W:")]
		CIVector Create (nfloat x, nfloat y, nfloat z, nfloat w);

		/// <param name="point">To be added.</param>
		///         <summary>Creates a <see cref="CoreGraphics.CGAffineTransform" /> that represents a directed distance from the origin to <paramref name="point" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("vectorWithCGPoint:")]
		CIVector Create (CGPoint point);

		/// <param name="point">To be added.</param>
		///         <summary>Creates a <see cref="CoreGraphics.CGAffineTransform" /> that stores the X-coordinate, Y-coordinate, height, and width in the <see cref="CoreImage.CIVector.X" />, <see cref="CoreImage.CIVector.Y" />. <see cref="CoreImage.CIVector.Z" />, and <see cref="CoreImage.CIVector.W" /> properties, respectively.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("vectorWithCGRect:")]
		CIVector Create (CGRect point);

		/// <param name="affineTransform">To be added.</param>
		///         <summary>Creates a vector from a <see cref="CoreGraphics.CGAffineTransform" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("vectorWithCGAffineTransform:")]
		CIVector Create (CGAffineTransform affineTransform);

		/// <param name="representation">To be added.</param>
		///         <summary>Creates a vector from a string, such as "[1.0, 2.0, 3.0]".</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("vectorWithString:")]
		CIVector FromString (string representation);

		/// <param name="p">To be added.</param>
		/// <summary>Creates a new CIVector for the specified point.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithCGPoint:")]
		NativeHandle Constructor (CGPoint p);

		/// <param name="r">To be added.</param>
		/// <summary>Creates a new CIVector and fills it with the X, Y, height, and width values.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithCGRect:")]
		NativeHandle Constructor (CGRect r);

		/// <param name="r">To be added.</param>
		/// <summary>Creates a new CIVector by flattening the six values in an affine transform into the first six positions in the new CIVector.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithCGAffineTransform:")]
		NativeHandle Constructor (CGAffineTransform r);


		/// <param name="x">To be added.</param>
		/// <summary>Creates a new one-dimensional vector.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithX:")]
		NativeHandle Constructor (nfloat x);

		/// <param name="x">To be added.</param>
		/// <param name="y">To be added.</param>
		/// <summary>Creates a new CIVector with the specified X and Y coordinates.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithX:Y:")]
		NativeHandle Constructor (nfloat x, nfloat y);

		/// <param name="x">To be added.</param>
		/// <param name="y">To be added.</param>
		/// <param name="z">To be added.</param>
		/// <summary>Creates a new CIVector with the specified X, Y and Z coordinates.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithX:Y:Z:")]
		NativeHandle Constructor (nfloat x, nfloat y, nfloat z);

		/// <param name="x">To be added.</param>
		/// <param name="y">To be added.</param>
		/// <param name="z">To be added.</param>
		/// <param name="w">To be added.</param>
		/// <summary>Creates a new CIVector with the specified X, Y, Z, and W coordinates.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithX:Y:Z:W:")]
		NativeHandle Constructor (nfloat x, nfloat y, nfloat z, nfloat w);

		/// <param name="representation">To be added.</param>
		/// <summary>Creates a new CIVector from the specified string representation.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithString:")]
		NativeHandle Constructor (string representation);

		[Export ("valueAtIndex:"), Internal]
		nfloat ValueAtIndex (nint index);

		/// <summary>Gets the number of items in this <see cref="CoreImage.CIVector" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("count")]
		nint Count { get; }

		/// <summary>Gets the first value in the vector.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("X")]
		nfloat X { get; }

		/// <summary>Gets the second value in the vector.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("Y")]
		nfloat Y { get; }

		/// <summary>Gets the third value in the vector.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("Z")]
		nfloat Z { get; }

		/// <summary>Gets the fourth value in the vector.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("W")]
		nfloat W { get; }

		/// <summary>Gets the point that is represented by (<see cref="CoreImage.CIVector.X" />,<see cref="CoreImage.CIVector.Y" />).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("CGPointValue")]
		CGPoint Point { get; }

		/// <summary>Gets a rectangle whose X-coordinate, Y-coordinate, height, and width are given by the <see cref="CoreImage.CIVector.X" />, <see cref="CoreImage.CIVector.Y" />. <see cref="CoreImage.CIVector.Z" />, and <see cref="CoreImage.CIVector.W" /> properties, respectively. .</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("CGRectValue")]
		CGRect Rectangle { get; }

		/// <summary>Gets the affine transform that is represented by the first 6 elements of this <see cref="CoreImage.CIVector" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("CGAffineTransformValue")]
		CGAffineTransform AffineTransform { get; }

		[Export ("stringRepresentation"), Internal]
		string StringRepresentation ();

	}

	/// <include file="../docs/api/CoreImage/CIDetector.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIDetector']/*" />
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIDetector {
		[Static, Export ("detectorOfType:context:options:"), Internal]
		[return: NullAllowed]
		CIDetector FromType (NSString detectorType, [NullAllowed] CIContext context, [NullAllowed] NSDictionary options);

		/// <param name="image">Image to analyze.</param>
		///         <summary>Analyzes the image and returns a list of features discovered in the image (faces, QR codes, rectangles).</summary>
		///         <returns>Array of discovered features.</returns>
		///         <remarks>
		///         </remarks>
		[Export ("featuresInImage:")]
		CIFeature [] FeaturesInImage (CIImage image);

		/// <param name="image">Image to analyze.</param>
		///         <param name="options">
		///           <para>Set of options to configure the search for features in the image.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Analyzes the image and returns a list of features discovered in the image (faces, QR codes, rectangles).</summary>
		///         <returns>Array of discovered features.</returns>
		///         <remarks>
		///         </remarks>
		[Export ("featuresInImage:options:")]
		CIFeature [] FeaturesInImage (CIImage image, [NullAllowed] NSDictionary options);

		[Field ("CIDetectorTypeFace"), Internal]
		NSString TypeFace { get; }

		[Field ("CIDetectorImageOrientation"), Internal]
		NSString ImageOrientation { get; }

		[Field ("CIDetectorAccuracy"), Internal]
		NSString Accuracy { get; }

		[Field ("CIDetectorAccuracyLow"), Internal]
		NSString AccuracyLow { get; }

		[Field ("CIDetectorAccuracyHigh"), Internal]
		NSString AccuracyHigh { get; }

		[Field ("CIDetectorTracking"), Internal]
		NSString Tracking { get; }

		[Field ("CIDetectorMinFeatureSize"), Internal]
		NSString MinFeatureSize { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorMaxFeatureCount"), Internal]
		NSString MaxFeatureCount { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorEyeBlink"), Internal]
		NSString EyeBlink { get; }

		[MacCatalyst (13, 1)]
		[Field ("CIDetectorSmile"), Internal]
		NSString Smile { get; }

		/// <summary>Represents the value associated with the constant CIDetectorAspectRatio</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIDetectorAspectRatio")]
		NSString AspectRatio { get; }

		/// <summary>Represents the value associated with the constant CIDetectorFocalLength</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIDetectorFocalLength")]
		NSString FocalLength { get; }

		/// <summary>Represents the value associated with the constant CIDetectorTypeQRCode</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIDetectorTypeQRCode")]
		NSString TypeQRCode { get; }

		/// <summary>Represents the value associated with the constant CIDetectorTypeRectangle</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIDetectorTypeRectangle")]
		NSString TypeRectangle { get; }

		/// <summary>Gets the number of angles from which to sample when detecting features.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIDetectorNumberOfAngles")]
		NSString NumberOfAngles { get; }

		/// <summary>Gets a value that tells whether subfeatures are detected.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIDetectorReturnSubFeatures")]
		NSString ReturnSubFeatures { get; }

		/// <summary>Gets a value that tells whether the detector detects text.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIDetectorTypeText")]
		NSString TypeText { get; }
	}

	/// <summary>An area of an image in which a <see cref="CoreImage.CIDetector" /> has detected a match.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreImage/Reference/CIFeature_Ref/index.html">Apple documentation for <c>CIFeature</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CIFeature {
		/// <summary>Gets the type of the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("type", ArgumentSemantic.Retain)]
		NSString Type { get; }

		/// <summary>Gets the rectangle that surrounds the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; }

		/// <summary>Represents the value associated with the constant CIFeatureTypeFace</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("CIFeatureTypeFace")]
		NSString TypeFace { get; }

		/// <summary>Represents the value associated with the constant CIFeatureTypeRectangle.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIFeatureTypeRectangle")]
		NSString TypeRectangle { get; }

		/// <summary>Represents the value associated with the constant CIFeatureTypeQRCode.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIFeatureTypeQRCode")]
		NSString TypeQRCode { get; }

		/// <summary>Represents the value associated with the constant CIFeatureTypeText.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CIFeatureTypeText")]
		NSString TypeText { get; }
	}

	/// <summary>Locations of the eyes and mouths in a detected face. In video sequences, attempts to maintain a consistent <see cref="CoreImage.CIFaceFeature.TrackingId" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreImage/Reference/CIFaceFeature/index.html">Apple documentation for <c>CIFaceFeature</c></related>
	[BaseType (typeof (CIFeature))]
	[DisableDefaultCtor]
	interface CIFaceFeature {
		/// <summary>Gets a value that indicates whether the <see cref="CoreImage.CIFaceFeature.LeftEyePosition" /> property is set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("hasLeftEyePosition", ArgumentSemantic.Assign)]
		bool HasLeftEyePosition { get; }

		/// <summary>Gets the position of the left eye.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("leftEyePosition", ArgumentSemantic.Assign)]
		CGPoint LeftEyePosition { get; }

		/// <summary>Gets a value that indicates whether the <see cref="CoreImage.CIFaceFeature.RightEyePosition" /> property is set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("hasRightEyePosition", ArgumentSemantic.Assign)]
		bool HasRightEyePosition { get; }

		/// <summary>Gets the position of the right eye.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("rightEyePosition", ArgumentSemantic.Assign)]
		CGPoint RightEyePosition { get; }

		/// <summary>Gets a value that indicates whether the <see cref="CoreImage.CIFaceFeature.MouthPosition" /> property is set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("hasMouthPosition", ArgumentSemantic.Assign)]
		bool HasMouthPosition { get; }

		/// <summary>Gets the position of the mouth.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("mouthPosition", ArgumentSemantic.Assign)]
		CGPoint MouthPosition { get; }

		/// <summary>Gets a value that indicates whether the <see cref="CoreImage.CIFaceFeature.TrackingId" /> property is set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("hasTrackingID", ArgumentSemantic.Assign)]
		bool HasTrackingId { get; }

		/// <summary>Gets the tracking ID.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("trackingID", ArgumentSemantic.Assign)]
		int TrackingId { get; } /* int, not NSInteger */

		/// <summary>Gets a value that indicates whether the <see cref="CoreImage.CIFaceFeature.TrackingFrameCount" /> property is set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("hasTrackingFrameCount", ArgumentSemantic.Assign)]
		bool HasTrackingFrameCount { get; }

		/// <summary>Gets the tracking frame count.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("trackingFrameCount", ArgumentSemantic.Assign)]
		int TrackingFrameCount { get; } /* int, not NSInteger */

		/// <summary>Gets the bounding image-space rectangle of the face.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; }

		/// <summary>Gets the counterclockwise in-plane rotation of the face, in radians. "0" represents a conventionally upright face.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("faceAngle", ArgumentSemantic.Assign)]
		float FaceAngle { get; } /* float, not CGFloat */

		/// <summary>Gets a value that indicates whether the <see cref="CoreImage.CIFaceFeature.FaceAngle" /> property is set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("hasFaceAngle", ArgumentSemantic.Assign)]
		bool HasFaceAngle { get; }

		/// <summary>Gets a value that indicates whether the <see cref="CoreImage.CIFaceFeature.HasSmile" /> property is set.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("hasSmile", ArgumentSemantic.Assign)]
		bool HasSmile { get; }

		/// <summary>Gets a value that indicates whether the left eye is closed.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("leftEyeClosed", ArgumentSemantic.Assign)]
		bool LeftEyeClosed { get; }

		/// <summary>Gets a value that indicates whether the left eye is closed.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("rightEyeClosed", ArgumentSemantic.Assign)]
		bool RightEyeClosed { get; }
	}

	/// <summary>A <see cref="CoreImage.CIFeature" /> describing an image-space rectangle that contains a detected physical rectangle.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/QuartzCore/Reference/CIRectangleFeature/index.html">Apple documentation for <c>CIRectangleFeature</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFeature))]
	interface CIRectangleFeature {
		/// <summary>Gets the rectangle, in image space, that bounds the detected rectangle.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bounds", ArgumentSemantic.UnsafeUnretained)]
		CGRect Bounds { get; }

		/// <summary>Gets the top left corner of the possibly skewed and rotated rectangle in the image.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("topLeft", ArgumentSemantic.UnsafeUnretained)]
		CGPoint TopLeft { get; }

		/// <summary>Gets the top right corner of the possibly skewed and rotated rectangle in the image..</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("topRight", ArgumentSemantic.UnsafeUnretained)]
		CGPoint TopRight { get; }

		/// <summary>Gets the bottom left corner of the possibly skewed and rotated rectangle in the image.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bottomLeft", ArgumentSemantic.UnsafeUnretained)]
		CGPoint BottomLeft { get; }

		/// <summary>Gets the bottom right corner of the possibly skewed and rotated rectangle in the image.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bottomRight", ArgumentSemantic.UnsafeUnretained)]
		CGPoint BottomRight { get; }
	}

	/// <summary>A detected QR code.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/QuartzCore/Reference/CIQRCodeFeature/index.html">Apple documentation for <c>CIQRCodeFeature</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFeature))]
	partial interface CIQRCodeFeature : NSSecureCoding, NSCopying {

		/// <summary>Gets the bounds of the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; }

		/// <summary>Gets the top left corner of the rectangle that contains the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("topLeft", ArgumentSemantic.Assign)]
		CGPoint TopLeft { get; }

		/// <summary>Gets the top right corner of the rectangle that contains the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("topRight", ArgumentSemantic.Assign)]
		CGPoint TopRight { get; }

		/// <summary>Gets the bottom left corner of the rectangle that contains the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bottomLeft", ArgumentSemantic.Assign)]
		CGPoint BottomLeft { get; }

		/// <summary>Gets the bottom right corner of the rectangle that contains the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bottomRight", ArgumentSemantic.Assign)]
		CGPoint BottomRight { get; }

		/// <summary>Gets the message contained in the detected QR code.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("messageString")]
		[NullAllowed]
		string MessageString { get; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("symbolDescriptor")]
		CIQRCodeDescriptor SymbolDescriptor { get; }
	}

	/// <summary>A detected text feature.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreImage/Reference/CITextFeature_Class/index.html">Apple documentation for <c>CITextFeature</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFeature))]
	interface CITextFeature {
		/// <summary>Gets the bounds of the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bounds")]
		CGRect Bounds { get; }

		/// <summary>Gets the top left corner of the rectangle that contains the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("topLeft")]
		CGPoint TopLeft { get; }

		/// <summary>Gets the top right corner of the rectangle that contains the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("topRight")]
		CGPoint TopRight { get; }

		/// <summary>Gets the bottom left corner of the rectangle that contains the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bottomLeft")]
		CGPoint BottomLeft { get; }

		/// <summary>Gets the bottom right corner of the rectangle that contains the feature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("bottomRight")]
		CGPoint BottomRight { get; }

		/// <summary>Gets an array that contains the subfeatures.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("subFeatures")]
		[NullAllowed]
		CIFeature [] SubFeatures { get; }
	}

	/// <summary>Base class for image-processing classes whose processing can be inserted into the CoreImage processing pipeline.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CoreImage/CIImageProcessorKernel">Apple documentation for <c>CIImageProcessorKernel</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CIImageProcessorKernel {
		/// <param name="inputs">
		///           <para>The input images.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <param name="arguments">
		///           <para>Additional arguments for the processing.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <param name="output">The results of the processing</param>
		///         <param name="error">Developers should set this <see cref="Foundation.NSError" /> as necessary.</param>
		///         <summary>Developers should override this method to perform custom processing on the <paramref name="inputs" />.</summary>
		///         <returns>
		///           <see langword="true" /> if the processing completed successfully.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("processWithInputs:arguments:output:error:")]
		bool Process ([NullAllowed] ICIImageProcessorInput [] inputs, [NullAllowed] NSDictionary<NSString, NSObject> arguments, ICIImageProcessorOutput output, out NSError error);

		/// <include file="../docs/api/CoreImage/CIImageProcessorKernel.xml" path="/Documentation/Docs[@DocId='M:CoreImage.CIImageProcessorKernel.GetRegionOfInterest(System.Int32,Foundation.NSDictionary{Foundation.NSString,Foundation.NSObject},CoreGraphics.CGRect)']/*" />
		[Static]
		[Export ("roiForInput:arguments:outputRect:")]
		CGRect GetRegionOfInterest (int input, [NullAllowed] NSDictionary<NSString, NSObject> arguments, CGRect outputRect);

		/// <param name="input">An index into the array of <see cref="CoreImage.ICIImageProcessorInput" /> objects passed to <see cref="CoreImage.CIImageProcessorKernel.Apply(CoreGraphics.CGRect,CoreImage.CIImage[],Foundation.NSDictionary{Foundation.NSString,Foundation.NSObject},out Foundation.NSError)" /> or <see cref="CoreImage.CIImageProcessorKernel.Process(CoreImage.ICIImageProcessorInput[],Foundation.NSDictionary{Foundation.NSString,Foundation.NSObject},CoreImage.ICIImageProcessorOutput,out Foundation.NSError)" />.</param>
		///         <summary>The color space of the <see cref="CoreImage.ICIImageProcessorInput" /> at index <paramref name="input" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("formatForInputAtIndex:")]
		CIFormat GetFormat (int input);

		/// <summary>The colorspace of the output image(s).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("outputFormat")]
		CIFormat OutputFormat { get; }

		/// <summary>If <see langword="true" />, the system must synchronize inputs prior to calling the processing function.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("synchronizeInputs")]
		bool SynchronizeInputs { get; }

		/// <param name="extent">The region to apply the custom processing.</param>
		///         <param name="inputs">
		///           <para>The input images.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <param name="args">
		///           <para>Additional arguments for the processing.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <param name="error">Developers should set this <see cref="Foundation.NSError" /> as necessary.</param>
		///         <summary>Developers should override this method to perform custom processing on the <paramref name="inputs" /> in the <paramref name="extent" /> rectangle.</summary>
		///         <returns>The image after custom processing.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("applyWithExtent:inputs:arguments:error:")]
		[return: NullAllowed]
		CIImage Apply (CGRect extent, [NullAllowed] CIImage [] inputs, [NullAllowed] NSDictionary<NSString, NSObject> args, out NSError error);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("outputIsOpaque")]
		bool OutputIsOpaque { get; }

		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0)]
		[Static]
		[Export ("roiTileArrayForInput:arguments:outputRect:")]
		CIVector [] GetRoiTileArray (int input, [NullAllowed] NSDictionary<NSString, NSObject> arguments, CGRect outputRect);

		// From the MultipleOutputSupport (CIImageProcessorKernel) category
		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Static]
		[Export ("processWithInputs:arguments:outputs:error:")]
		bool Process ([NullAllowed] ICIImageProcessorInput [] inputs, [NullAllowed] NSDictionary<NSString, NSObject> arguments, ICIImageProcessorOutput [] outputs, [NullAllowed] out NSError error);

		// From the MultipleOutputSupport (CIImageProcessorKernel) category
		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Static]
		[Export ("outputFormatAtIndex:arguments:")]
		CIFormat GetOutputFormat (int outputIndex, [NullAllowed] NSDictionary<NSString, NSObject> arguments);

		// From the MultipleOutputSupport (CIImageProcessorKernel) category
		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Static]
		[Export ("applyWithExtents:inputs:arguments:error:")]
		[return: NullAllowed]
		CIImage [] Apply (CIVector [] extents, [NullAllowed] CIImage [] inputs, [NullAllowed] NSDictionary<NSString, NSObject> arguments, [NullAllowed] out NSError error);

		// From the TiledOutputSupport (CIImageProcessorKernel) category
		/// <param name="tileExtents">The non-overlapping tile extents produced by the image processor.</param>
		/// <param name="inputs">The input images.</param>
		/// <param name="arguments">Additional arguments for the image processor.</param>
		/// <param name="error">The error if the image could not be created.</param>
		/// <summary>Creates an image by applying the image processor to the specified tile extents.</summary>
		/// <returns>The processed image, or <see langword="null" /> if processing failed.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Static]
		[Export ("applyWithTiledExtent:inputs:arguments:error:")]
		[return: NullAllowed]
		CIImage ApplyWithTiledExtent (CIVector [] tileExtents, [NullAllowed] CIImage [] inputs, [NullAllowed] NSDictionary<NSString, NSObject> arguments, [NullAllowed] out NSError error);
	}

	/// <summary>Animates a transition by creating an accordion-fold effect on the source image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIAccordionFoldTransition : CIAccordionFoldTransitionProtocol {
	}

	/// <summary>An abstract <see cref="CoreImage.CIFilter" /> that composites two images.</summary>
	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CICompositingFilter {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>The background image to use.</summary>
		///         <value>
		///         </value>
		///         <remarks>This is the background image used by the filter.</remarks>
		[CoreImageFilterProperty ("inputBackgroundImage")]
		CIImage BackgroundImage { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIAdditionCompositing.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIAdditionCompositing']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CIAdditionCompositing {
	}

	/// <summary>An abstract class that defines a <see cref="CoreImage.CIFilter" /> that performs an affine transform on an image and then performs a filtering operation on the transformed image.</summary>
	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIAffineFilter : CIFilterProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIAffineClamp.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIAffineClamp']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIAffineFilter))]
	interface CIAffineClamp : CIAffineClampProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIAffineTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIAffineTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIAffineFilter))]
	interface CIAffineTile : CIAffineTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIAffineTransform.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIAffineTransform']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIAffineFilter))]
	interface CIAffineTransform {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputTransform")]
		CGAffineTransform Transform { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaAverage")]
	interface CIAreaAverageProtocol : CIFilterProtocol {
	}

	/// <summary>Produces a single-pixel image. The pixel is the average color of the input extent.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaAverage {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }
#endif

		[iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaReductionFilter")]
	interface CIAreaReductionFilterProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaHistogram")]
	interface CIAreaHistogramProtocol : CIAreaReductionFilterProtocol {

		/// <summary>Gets or sets the scale of the area.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		/// <summary>Gets or sets the number of buckets in the histogram.</summary>
		[Abstract]
		[Export ("count")]
		nint InputCount { get; set; }
	}

	/// <summary>Creates histogram data for a given image and rectangle of interest.</summary>
	/// <remarks>
	///   <para>The <see cref="CoreImage.CIFilter.OutputImage" /> of this filter is a single-pixel high image of width <see cref="CIAreaHistogram.InputCount" />. </para>
	/// </remarks>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIAreaHistogram : CIAreaHistogramProtocol {
		/// <summary>Gets or sets the region in the source image for which to produce a histogram.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }

#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }

		[CoreImageFilterProperty ("outputData")]
		NSData OutputData { get; }
#endif
	}

	[CoreImageFilter]
	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIReductionFilter {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMaximum")]
	interface CIAreaMaximumProtocol : CIAreaReductionFilterProtocol {
	}

	/// <summary>Produces a single-pixel image. The pixel's components are set to the maximum of that component within the input extent.</summary>
	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMaximum : CIAreaMaximumProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMaximumAlpha")]
	interface CIAreaMaximumAlphaProtocol : CIAreaReductionFilterProtocol {
	}

	/// <summary>Returns a single-pixel image. The color is that of the pixel in the input extent with the greatest alpha value.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMaximumAlpha : CIAreaMaximumAlphaProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMinimum")]
	interface CIAreaMinimumProtocol : CIAreaReductionFilterProtocol {
	}

	/// <summary>Returns a single-pixel image. The pixel's components are set to the minimum of that component within the input extent.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinimum : CIAreaReductionFilterProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMinimumAlpha")]
	interface CIAreaMinimumAlphaProtocol : CIAreaReductionFilterProtocol {
	}

	/// <summary>Returns a single-pixel image. The color is that of the pixel in the input extent with the lowest alpha value.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinimumAlpha : CIAreaReductionFilterProtocol {
	}

	/// <summary>Base class for code generators.</summary>
	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CICodeGenerator {
		/// <summary>Gets or sets the message to encode.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputMessage")]
		NSData Message { get; set; }
	}

	/// <summary>Generates an Aztec code for the specified <see cref="CoreImage.CIAztecCodeGenerator.Message" />.</summary>
	///     <remarks>
	///       <para>The Aztec code is defined at <format type="text/html"><a href="https://www.iso.org/iso/iso_catalogue/catalogue_tc/catalogue_detail.htm?csnumber=41548">www.iso.org/iso/iso_catalogue/catalogue_tc/catalogue_detail.htm?csnumber=41548</a></format>.</para>
	///     </remarks>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CICodeGenerator))]
	interface CIAztecCodeGenerator : CIAztecCodeGeneratorProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
#endif
	}

	/// <summary>A <see cref="CoreImage.CITransitionFilter" /> that animates a transition between two images.</summary>
	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CITransitionFilter : CITransitionFilterProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIBarsSwipeTransition.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIBarsSwipeTransition']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIBarsSwipeTransition {

		/// <summary>Gets or sets the width of the bars.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }

		/// <summary>Gets or sets the angle, in radians, of the bar swipe transition.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		/// <summary>Gets or sets the offset from one bar to the next.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputBarOffset")]
		float BarOffset { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIBlendWithAlphaMask.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIBlendWithAlphaMask']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithAlphaMask {
	}

	/// <include file="../docs/api/CoreImage/CIBlendWithMask.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIBlendWithMask']/*" />
	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIBlendWithMask : CIBlendWithMaskProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIBloom.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIBloom']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIBloom : CIBloomProtocol {
	}

	/// <summary>Applies a blur created with a box-shaped convolution kernel.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIBoxBlur : CIBoxBlurProtocol {
	}

	/// <summary>An abstract <see cref="CoreImage.CIFilter" /> for distortions.</summary>
	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIDistortionFilter {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the radius of the effect.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIBumpDistortion")]
	interface CIBumpDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the center of the effect.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the level of the bump distortion.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIBumpDistortion.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIBumpDistortion']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIBumpDistortion : CIBumpDistortionProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIBumpDistortionLinear")]
	interface CIBumpDistortionLinearProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the angle of the line about which to distort the image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		/// <summary>Gets or sets the amount of distortion to create.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIBumpDistortionLinear.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIBumpDistortionLinear']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIBumpDistortionLinear : CIBumpDistortionLinearProtocol {
	}

	/// <include file="../docs/api/CoreImage/CICheckerboardGenerator.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CICheckerboardGenerator']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CICheckerboardGenerator : CICheckerboardGeneratorProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CICircleSplashDistortion")]
	interface CICircleSplashDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CICircleSplashDistortion.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CICircleSplashDistortion']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CICircleSplashDistortion : CICircleSplashDistortionProtocol {
	}

	/// <summary>A <see cref="CoreImage.CIFilter" /> that mimics halftone screens.</summary>
	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIScreenFilter {

		/// <summary>Gets or sets the sharpness of the halftone pattern. 1 is sharp. 0 is maximally blurry.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputSharpness")]
		float Sharpness { get; set; }

		/// <summary>Gets or sets the center of the halftone pattern.</summary>
		[CoreImageFilterProperty ("inputCenter")]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the size of the halftone pattern elements.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CICircularScreen.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CICircularScreen']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CICircularScreen : CICircularScreenProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CICircularWrap")]
	interface CICircularWrapProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the center of the distortion.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the radius of the distortion.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the angle of the image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	/// <summary>Distorts the input image so that it appears as if wrapped around a transparent circle.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CICircularWrap : CICircularWrapProtocol {
	}

	/// <summary>Produce a color half-tone image built from cyan, magenta, yellow, and black 'inks'.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter), Name = "CICMYKHalftone")]
	interface CICmykHalftone : CICmykHalftoneProtocol {
	}

	/// <summary>Generates a Code 128 barcode.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CICodeGenerator))]
	interface CICode128BarcodeGenerator : CICode128BarcodeGeneratorProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputCGImage")]
		CIImage OutputCGImage { get; }
#endif
	}

	/// <summary>An abstract <see cref="CoreImage.CIFilter" /> that combines a background and foreground image.</summary>
	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIBlendFilter {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>The background image to use.</summary>
		///         <value>
		///         </value>
		///         <remarks>This is the background image used by the filter.</remarks>
		[CoreImageFilterProperty ("inputBackgroundImage")]
		CIImage BackgroundImage { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIColorBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIColorBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIColorBurnBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorBurnBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIColorBurnBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIColorClamp.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorClamp']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIColorClamp : CIColorClampProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorControls.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorControls']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorControls : CIColorControlsProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorCrossPolynomial.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorCrossPolynomial']/*" />
	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIColorCrossPolynomial : CIColorCrossPolynomialProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorCube.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorCube']/*" />
	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIFilter))]
	interface CIColorCube : CIColorCubeProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorCubeWithColorSpace.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorCubeWithColorSpace']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIColorCube))]
	interface CIColorCubeWithColorSpace : CIColorCubeWithColorSpaceProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorDodgeBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorDodgeBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIColorDodgeBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIColorInvert.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorInvert']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorInvert : CIColorInvertProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorMap.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorMap']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorMap : CIColorMapProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorMatrix.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorMatrix']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorMatrix : CIColorMatrixProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorMonochrome.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorMonochrome']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorMonochrome : CIColorMonochromeProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorPolynomial.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorPolynomial']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIColorCrossPolynomial))]
	interface CIColorPolynomial : CIColorPolynomialProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIColorPosterize.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIColorPosterize']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIColorPosterize : CIColorPosterizeProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIColumnAverage")]
	interface CIColumnAverageProtocol : CIAreaReductionFilterProtocol {
	}

	/// <summary>Returns a 1-pixel high image with the same width as the input image. Every pixel is the average color of the corresponding column in the input image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIColumnAverage : CIColumnAverageProtocol {
	}

	/// <summary>Produces an image with outlined edges and a color halftone effect.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIComicEffect : CIComicEffectProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIConstantColorGenerator.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIConstantColorGenerator']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIConstantColorGenerator {

		/// <summary>Gets or sets the color to use.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputColor")]
		CIColor Color { get; set; }
	}

	/// <summary>An abstract class that is the base for convolution filters.</summary>
	///     <remarks>
	///       <para>Convolution filters generate a pixel value by summing the pixel values in a neighborhood (the size of the neighborhood may be 3x3, 5x5, or 9 pixels horizontally or vertically) and then by adding a bias. To maintain brightness, the sum of values in the convolution kernel must sum to 1.0.</para>
	///     </remarks>
	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionCore {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the weights for the convolution.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		/// <summary>Gets or sets the bias of the convolution.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIConvolution3X3.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIConvolution3X3']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution3X3 {
	}

	/// <include file="../docs/api/CoreImage/CIConvolution5X5.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIConvolution5X5']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution5X5 {
	}

	/// <summary>A filter that performs a custom 7x7 matrix convolution.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution7X7 {
	}

	/// <include file="../docs/api/CoreImage/CIConvolution9Horizontal.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIConvolution9Horizontal']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution9Horizontal {
	}

	/// <include file="../docs/api/CoreImage/CIConvolution9Vertical.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIConvolution9Vertical']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIConvolutionCore))]
	interface CIConvolution9Vertical {
	}

	/// <include file="../docs/api/CoreImage/CICopyMachineTransition.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CICopyMachineTransition']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CICopyMachineTransition {

		/// <summary>Gets or sets the color of the swipe.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputColor")]
		CIColor Color { get; set; }

		/// <summary>Gets or sets the width of the swipe.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }

		/// <summary>Gets or sets the opacity of the swipe.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputOpacity")]
		float Opacity { get; set; }

		/// <summary>Gets or sets the angle of the swipe.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		/// <summary>Gets or sets the extent of the image to transform.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CICrop.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CICrop']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CICrop {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the region that will remain after the image is cropped.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputRectangle")]
		CIVector Rectangle { get; set; }
	}

	/// <summary>Creates irregular blocks of average color from the source image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CICrystallize : CICrystallizeProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIDarkenBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIDarkenBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIDarkenBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIDifferenceBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIDifferenceBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIDifferenceBlendMode {
	}

	/// <summary>Applies a blur created with a disc-shaped convolution kernel.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDiscBlur : CIDiscBlurProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIDisintegrateWithMaskTransition.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIDisintegrateWithMaskTransition']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIDisintegrateWithMaskTransition : CIDisintegrateWithMaskTransitionProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIDisplacementDistortion")]
	interface CIDisplacementDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the image that defines the texture displacement.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("displacementImage", ArgumentSemantic.Retain)]
		CIImage DisplacementImage { get; set; }

		/// <summary>Gets or sets a value that determines how much distortion to apply. Default is 50.0.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	/// <summary>Textures the input image by the grayscale value of the corresponding pixels in the displacement image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDisplacementDistortion : CIDisplacementDistortionProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIDissolveTransition.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIDissolveTransition']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIDissolveTransition {
	}

	/// <summary>A <see cref="CIBlendFilter" /> that divides the color values of its <see cref="CIBlendFilter.InputImage" /> and <see cref="CIBlendFilter.BackgroundImage" />.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIDivideBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIDotScreen.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIDotScreen']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CIDotScreen : CIDotScreenProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIDroste")]
	interface CIDrosteProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets a vector that represents the first corner of the rectangular inset region.</summary>
		[Abstract]
		[Export ("insetPoint0", ArgumentSemantic.Assign)]
		CGPoint InputInsetPoint0 { get; set; }

		/// <summary>Gets or sets a vector that represents the second corner of the rectangular inset region.</summary>
		[Abstract]
		[Export ("insetPoint1", ArgumentSemantic.Assign)]
		CGPoint InputInsetPoint1 { get; set; }

		/// <summary>Gets or sets the number of droste strands. Default is 1.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("strands")]
		float Strands { get; set; }

		/// <summary>Gets or sets the number of times that the image is repeated in each spiral of a stranded Droste image. Default is 1.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("periodicity")]
		float Periodicity { get; set; }

		/// <summary>Gets or sets the amount by which to rotate the inset image. Default is 0.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("rotation")]
		float Rotation { get; set; }

		/// <summary>Gets or sets a value that controls by how much to zoom the inset image. Default is 1.0.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("zoom")]
		float Zoom { get; set; }
	}

	/// <summary>Creates a series of increasingly smaller boxes repeating a portion of the image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDroste : CIDrosteProtocol {
	}

	/// <summary>Creates a colorized edge-detection effect.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIEdges : CIEdgesProtocol {
	}

	/// <summary>Creates a black and white woodblock effect.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIEdgeWork : CIEdgeWorkProtocol {
	}

	/// <summary>A <see cref="CoreImage.CIFilter" /> that applies a filter and then tiles the results.</summary>
	[CoreImageFilter (IntPtrCtorVisibility = MethodAttributes.Family)] // was already protected in classic
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CITileFilter {

		/// <summary>Gets or sets the angle, in radians, of the tile pattern.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		/// <summary>Gets or sets the center of the tile pattern.</summary>
		[CoreImageFilterProperty ("inputCenter")]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the length of the sides of the tiles in the pattern.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIEightfoldReflectedTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIEightfoldReflectedTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIEightfoldReflectedTile : CIEightfoldReflectedTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIExclusionBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIExclusionBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIExclusionBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIExposureAdjust.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIExposureAdjust']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIExposureAdjust : CIExposureAdjustProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIFalseColor.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIFalseColor']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIFalseColor : CIFalseColorProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIFlashTransition.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIFlashTransition']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIFlashTransition : CIFlashTransitionProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIFourfoldReflectedTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIFourfoldReflectedTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIFourfoldReflectedTile : CIFourfoldReflectedTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIFourfoldRotatedTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIFourfoldRotatedTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIFourfoldRotatedTile : CIFourfoldRotatedTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIFourfoldTranslatedTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIFourfoldTranslatedTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIFourfoldTranslatedTile : CIFourfoldTranslatedTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIGammaAdjust.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIGammaAdjust']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIGammaAdjust : CIGammaAdjustProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIGaussianBlur.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIGaussianBlur']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIGaussianBlur : CIGaussianBlurProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIGaussianGradient.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIGaussianGradient']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIGaussianGradient : CIGaussianGradientProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIGlassDistortion")]
	interface CIGlassDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the texture map to use for the glass distortion effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("textureImage", ArgumentSemantic.Retain)]
		CIImage Texture { get; set; }

		/// <summary>Gets or sets the center of the input texture.</summary>
		/// <value>The default value is 200.0.</value>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the scale of the input texture.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	/// <summary>Distorts the input image so that it appears viewed through glass blocks whose geometry corresponds to the <see cref="CoreImage.CIGlassDistortion.Texture" /> image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIGlassDistortion : CIGlassDistortionProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIGlassLozenge")]
	interface CIGlassLozengeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the first point that defines the long axis of the lozenge.</summary>
		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		/// <summary>Gets or sets the second point that defines the long axis of the lozenge.</summary>
		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		/// <summary>Gets or sets the radius, and therefore the half-width, of the lozenge.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the amount simulated refraction for the lozenge. Default is 1.7.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("refraction")]
		float Refraction { get; set; }
	}

	/// <summary>Creates an effect as if a lozenge-shaped lens were placed on the input image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIGlassLozenge : CIGlassLozengeProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIGlideReflectedTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIGlideReflectedTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CIGlideReflectedTile : CIGlideReflectedTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIGloom.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIGloom']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIGloom : CIGloomProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIHardLightBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIHardLightBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIHardLightBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIHatchedScreen.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIHatchedScreen']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CIHatchedScreen : CIHatchedScreenProtocol {
	}

	/// <summary>Creates a continuous height field from a grayscale mask. Use with <see cref="CoreImage.CIShadedMaterial" /> to create realistic dropshadows.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIHeightFieldFromMask : CIHeightFieldFromMaskProtocol {
	}

	/// <summary>Pixellates the image with hexagonals based on the average color of pixels within the hexagon.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIHexagonalPixellate : CIHexagonalPixellateProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIHighlightShadowAdjust.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIHighlightShadowAdjust']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIHighlightShadowAdjust : CIHighlightShadowAdjustProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIHistogramDisplay")]
	interface CIHistogramDisplayProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the display height of the histogram.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("height")]
		float Height { get; set; }

		/// <summary>Gets or sets the height of the tallest histogram bar.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("highLimit")]
		float HighLimit { get; set; }

		/// <summary>Gets or sets the height of the shortest histogram bar.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("lowLimit")]
		float LowLimit { get; set; }
	}

	/// <summary>Renders a one-dimensional <see cref="InputImage" /> histogram as a typical two-dimensional histogram.</summary>
	/// <remarks>
	///   <para>This filter can be used in conjunction with <see cref="CoreImage.CIAreaHistogram" /> to create a user-meaningful histogram, as shown in the following example.</para>
	///   <example>
	///     <code lang="csharp lang-csharp"><![CDATA[
	/// var histogram = new CIAreaHistogram () {
	/// 	Image = heron,
	/// 	Extent = new CIVector(new RectangleF(0, 0, 567, 267)),
	/// 	Count = 256,
	/// 	Scale = 25.0f
	///
	/// };
	///
	/// //Pass 1-D histogram to CIHistogramDisplayFilter for display
	/// var histogramDisplay = new CIHistogramDisplayFilter () {
	/// 	Image = histogram.OutputImage,
	/// };
	///           ]]></code>
	///   </example>
	///   <para>Which results in the following histogram: <img href="~/xml/CoreImage/_images/CoreImage.CIHistogramDisplayFilter.png" alt="Result of applying the filter." />.</para>
	/// </remarks>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	// incorrect version string for OSX: '10.?' Double-check documentation
	[BaseType (typeof (CIFilter))]
	interface CIHistogramDisplayFilter : CIHistogramDisplayProtocol {

		// [CoreImageFilterProperty ("inputImage")]
		// CIImage InputImage { get; set; }

		// [CoreImageFilterProperty ("inputHeight")]
		// float Height { get; set; }

		// [CoreImageFilterProperty ("inputHighLimit")]
		// float HighLimit { get; set; }

		// [CoreImageFilterProperty ("inputLowLimit")]
		// float LowLimit { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIHoleDistortion")]
	interface CIHoleDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIHoleDistortion.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIHoleDistortion']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIHoleDistortion : CIHoleDistortionProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIHueAdjust.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIHueAdjust']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIHueAdjust : CIHueAdjustProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIHueBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIHueBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIHueBlendMode {
	}

	/// <summary>Produces a kaleidoscopic, 12-way-symmetrical, image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIKaleidoscope : CIKaleidoscopeProtocol {
	}

	/// <include file="../docs/api/CoreImage/CILanczosScaleTransform.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CILanczosScaleTransform']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CILanczosScaleTransform : CILanczosScaleTransformProtocol {
	}

	/// <summary>Creates an artificial lens-flare.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILenticularHaloGenerator : CILenticularHaloGeneratorProtocol {
	}

	/// <include file="../docs/api/CoreImage/CILightenBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CILightenBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CILightenBlendMode {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CILightTunnel")]
	interface CILightTunnelProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the center of the image portion to rotate.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the initial angle of the image sample.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("rotation")]
		float Rotation { get; set; }

		/// <summary>Gets or sets the distance about the center to sample.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CILightTunnel.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CILightTunnel']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILightTunnel : CILightTunnelProtocol {
	}

	/// <summary>Darkens the image based on the colors of the background image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CILinearBurnBlendMode {
	}

	/// <summary>Darkens the image based on the colors of the background image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CILinearDodgeBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CILinearGradient.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CILinearGradient']/*" />
	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIFilter))]
	interface CILinearGradient : CILinearGradientProtocol {
	}

	/// <include file="../docs/api/CoreImage/CILinearToSRGBToneCurve.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CILinearToSRGBToneCurve']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILinearToSRGBToneCurve : CILinearToSrgbToneCurveProtocol {
	}

	/// <summary>Produces a black-and-white sketch-like image via edge detection.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILineOverlay : CILineOverlayProtocol {
	}

	/// <include file="../docs/api/CoreImage/CILineScreen.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CILineScreen']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIScreenFilter))]
	interface CILineScreen : CILineScreenProtocol {
	}

	/// <include file="../docs/api/CoreImage/CILuminosityBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CILuminosityBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CILuminosityBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIMaskToAlpha.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIMaskToAlpha']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMaskToAlpha : CIMaskToAlphaProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIMaximumComponent.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIMaximumComponent']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMaximumComponent : CIMaximumComponentProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIMaximumCompositing.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIMaximumCompositing']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CIMaximumCompositing {
	}

	/// <summary>Replaces pixel values with the median of neighboring pixel values, thus reducing noise in the input image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIMedianFilter : CIMedianProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIMinimumComponent.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIMinimumComponent']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMinimumComponent : CIMinimumComponentProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIMinimumCompositing.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIMinimumCompositing']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CIMinimumCompositing {
	}

	/// <include file="../docs/api/CoreImage/CIModTransition.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIModTransition']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CIModTransition : CIModTransitionProtocol {
	}

	/// <summary>Applies a blur that simulates the motion of a camera during capture.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CILinearBlur))]
	interface CIMotionBlur : CIMotionBlurProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIMultiplyBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIMultiplyBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIMultiplyBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIMultiplyCompositing.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIMultiplyCompositing']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CIMultiplyCompositing {
	}

	/// <summary>Reduces noise by detecting luminance variations. Small luminance variations are locally blurred, large variations are locally sharpened.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CINoiseReduction : CINoiseReductionProtocol {
	}

	/// <summary>Creates an 'op art' effect by tiling, scaling, and rotating regions of the image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITileFilter))]
	interface CIOpTile : CIOpTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIOverlayBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIOverlayBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIOverlayBlendMode {
	}

	/// <summary>Animates a page curl transition between images.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITransitionFilter))]
	interface CIPageCurlTransition : CIPageCurlTransitionProtocol {
	}

	/// <summary>Animates a page curl transition, with a shadow, between images.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPageCurlWithShadowTransition : CIPageCurlWithShadowTransitionProtocol {
	}

	/// <summary>Warps an image into a parallelogram and then tiles the result.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITileFilter))]
	interface CIParallelogramTile : CIParallelogramTileProtocol {
	}

	/// <summary>Generator for 2-dimensional PDF417 bar codes.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CICodeGenerator), Name = "CIPDF417BarcodeGenerator")]
	interface CIPdf417BarcodeGenerator : CIPdf417BarcodeGeneratorProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
#endif
	}

	/// <summary>A <see cref="CoreImage.CIPerspectiveTransform" /> that is cropped according to the perspective control points, but whose pixels are not transformed.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPerspectiveTransform))]
	interface CIPerspectiveCorrection : CIPerspectiveCorrectionProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIPerspectiveTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPerspectiveTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIPerspectiveTile : CIPerspectiveTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIPerspectiveTransform.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPerspectiveTransform']/*" />
	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.Public, StringCtorVisibility = MethodAttributes.Public)]
	[BaseType (typeof (CIFilter))]
	interface CIPerspectiveTransform : CIPerspectiveTransformProtocol {
		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
	}

	/// <include file="../docs/api/CoreImage/CIPerspectiveTransformWithExtent.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPerspectiveTransformWithExtent']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPerspectiveTransform))]
	interface CIPerspectiveTransformWithExtent : CIPerspectiveTransformWithExtentProtocol {
	}

	/// <summary>The base class for photo effect filters.</summary>
	[CoreImageFilter (StringCtorVisibility = MethodAttributes.Public)]
	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CIPhotoEffect : CIPhotoEffectProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIPhotoEffectChrome.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPhotoEffectChrome']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectChrome {
	}

	/// <include file="../docs/api/CoreImage/CIPhotoEffectFade.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPhotoEffectFade']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectFade {
	}

	/// <include file="../docs/api/CoreImage/CIPhotoEffectInstant.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPhotoEffectInstant']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectInstant {
	}

	/// <include file="../docs/api/CoreImage/CIPhotoEffectMono.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPhotoEffectMono']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectMono {
	}

	/// <include file="../docs/api/CoreImage/CIPhotoEffectNoir.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPhotoEffectNoir']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectNoir {
	}

	/// <include file="../docs/api/CoreImage/CIPhotoEffectProcess.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPhotoEffectProcess']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectProcess {
	}

	/// <include file="../docs/api/CoreImage/CIPhotoEffectTonal.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPhotoEffectTonal']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectTonal {
	}

	/// <include file="../docs/api/CoreImage/CIPhotoEffectTransfer.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPhotoEffectTransfer']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIPhotoEffect))]
	interface CIPhotoEffectTransfer {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIPinchDistortion")]
	interface CIPinchDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets a value that controls by how much the image will be pinched.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIPinchDistortion.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPinchDistortion']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIPinchDistortion : CIPinchDistortionProtocol {

		// [CoreImageFilterProperty ("inputScale")]
		// float Scale { get; set; }
	}

	/// <summary>Replaces image colors with the bright colors from the background image.</summary>
	/// <remarks>
	///   <para>
	///     If the <see cref="CIBlendFilter.BackgroundImage" /> pixel is more than half of the maximum brightness,
	///     that pixel is substituted for the pixel in the <see cref="CIBlendFilter.InputImage" />. Also, even if the background
	///     pixel is less than 50% brightness, it will be substituted if the image pixel is lighter than the background pixel.
	///   </para>
	///   <para>This filter can be used to make an image look as if it were drawn with powder pastels.</para>
	/// </remarks>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CIPinLightBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIPixellate.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIPixellate']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIPixellate : CIPixellateProtocol {
	}

	/// <summary>Creates an effect mimicking artistic pointillization.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPointillize : CIPointillizeProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIQRCodeGenerator.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIQRCodeGenerator']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CICodeGenerator))]
	interface CIQRCodeGenerator : CIQRCodeGeneratorProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
#endif
	}

	/// <include file="../docs/api/CoreImage/CIRadialGradient.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIRadialGradient']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIRadialGradient : CIRadialGradientProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIRandomGenerator.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIRandomGenerator']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIRandomGenerator : CIRandomGeneratorProtocol {
	}

	/// <summary>Animates a ripple transition between images.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITransitionFilter))]
	interface CIRippleTransition : CIRippleTransitionProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIRowAverage")]
	interface CIRowAverageProtocol : CIAreaReductionFilterProtocol {
	}

	/// <summary>Returns a 1-pixel-high image, the pixels of which are the average color for the corresponding row in the input image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIRowAverage : CIRowAverageProtocol {
	}

	/// <include file="../docs/api/CoreImage/CISaturationBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISaturationBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CISaturationBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CIScreenBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIScreenBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CIScreenBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CISepiaTone.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISepiaTone']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CISepiaTone : CISepiaToneProtocol {
	}

	/// <summary>Produces a shaded image from a height field, such as that produced by <see cref="CoreImage.CIHeightFieldFromMask" />.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIShadedMaterial : CIShadedMaterialProtocol {
	}

	/// <include file="../docs/api/CoreImage/CISharpenLuminance.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISharpenLuminance']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CISharpenLuminance : CISharpenLuminanceProtocol {
	}

	/// <include file="../docs/api/CoreImage/CISixfoldReflectedTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISixfoldReflectedTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CISixfoldReflectedTile : CISixfoldReflectedTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CISixfoldRotatedTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISixfoldRotatedTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CISixfoldRotatedTile : CISixfoldRotatedTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CISmoothLinearGradient.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISmoothLinearGradient']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CILinearGradient))]
	interface CISmoothLinearGradient : CISmoothLinearGradientProtocol {
	}

	/// <include file="../docs/api/CoreImage/CISoftLightBlendMode.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISoftLightBlendMode']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIBlendFilter))]
	interface CISoftLightBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CISourceAtopCompositing.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISourceAtopCompositing']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CISourceAtopCompositing {
	}

	/// <include file="../docs/api/CoreImage/CISourceInCompositing.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISourceInCompositing']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CISourceInCompositing {
	}

	/// <include file="../docs/api/CoreImage/CISourceOutCompositing.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISourceOutCompositing']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CISourceOutCompositing {
	}

	/// <include file="../docs/api/CoreImage/CISourceOverCompositing.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISourceOverCompositing']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CICompositingFilter))]
	interface CISourceOverCompositing {
	}

	/// <summary>Replaces color ranges with specified replacement colors.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISpotColor : CISpotColorProtocol {
	}

	/// <summary>Produces an effect mimicking that of a directional spotlight.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISpotLight : CISpotLightProtocol {
	}

	/// <include file="../docs/api/CoreImage/CISRGBToneCurveToLinear.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISRGBToneCurveToLinear']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISRGBToneCurveToLinear : CISrgbToneCurveToLinearProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIStarShineGenerator.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIStarShineGenerator']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStarShineGenerator : CIStarShineGeneratorProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIStraightenFilter.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIStraightenFilter']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStraightenFilter : CIStraightenProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIStretchCrop")]
	interface CIStretchCropProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the size of the output image in pixels.</summary>
		[Abstract]
		[Export ("size", ArgumentSemantic.Assign)]
		CGPoint InputSize { get; set; }

		/// <summary>Gets or sets a value that controls the ratio of stretching to cropping. 0 causes the image to be stretched so that no cropping is necessary. 1 causes the image to be cropped so that no stretching is necessary. Intermediate values combine these effects.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("cropAmount")]
		float CropAmount { get; set; }

		/// <summary>Gets or sets a value that controls by how much more to stretch the center of the image, with a value of 0 indicating uniform stretching with no distortion.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("centerStretchAmount")]
		float CenterStretchAmount { get; set; }
	}

	/// <summary>Stretches and crops an image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIStretchCrop : CIStretchCropProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIStripesGenerator.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIStripesGenerator']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIStripesGenerator : CIStripesGeneratorProtocol {
	}

	/// <summary>Subtracts the background image pixels from those in the <see cref="CIBlendFilter.InputImage" />.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendFilter))]
	interface CISubtractBlendMode {
	}

	/// <include file="../docs/api/CoreImage/CISwipeTransition.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CISwipeTransition']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITransitionFilter))]
	interface CISwipeTransition {

		/// <summary>Gets or sets the color of the swipe boundary.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputColor")]
		CIColor Color { get; set; }

		/// <summary>Gets or sets the width of the swipe boundary.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputWidth")]
		float Width { get; set; }

		/// <summary>Gets or sets the opacity of the swipe.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputOpacity")]
		float Opacity { get; set; }

		/// <summary>Gets or sets the angle of the swipe.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputAngle")]
		float Angle { get; set; }

		/// <summary>Gets or sets the extent of the image to transform.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CITemperatureAndTint.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CITemperatureAndTint']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CITemperatureAndTint : CITemperatureAndTintProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIToneCurve.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIToneCurve']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIToneCurve : CIToneCurveProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CITorusLensDistortion")]
	interface CITorusLensDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the center of the toroidal lens.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the radius of the toroidal lens, the distance from the center of the torus to the center of its ring.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the width of the ring of the torus.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		/// <summary>Gets or sets the index of refraction.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("refraction")]
		float Refraction { get; set; }
	}

	/// <summary>Produces an effect as if a toroidal lens were placed on the input image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CITorusLensDistortion : CITorusLensDistortionProtocol {
	}

	/// <include file="../docs/api/CoreImage/CITriangleKaleidoscope.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CITriangleKaleidoscope']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CITriangleKaleidoscope : CITriangleKaleidoscopeProtocol {
	}

	/// <summary>Tiles the image with a triangular region of the input image.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CITileFilter))]
	interface CITriangleTile : CITriangleTileProtocol {
	}

	/// <include file="../docs/api/CoreImage/CITwelvefoldReflectedTile.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CITwelvefoldReflectedTile']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CITileFilter))]
	interface CITwelvefoldReflectedTile : CITwelvefoldReflectedTileProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CITwirlDistortion")]
	interface CITwirlDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the angle of the twirl effect, in radians.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CITwirlDistortion.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CITwirlDistortion']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CITwirlDistortion : CITwirlDistortionProtocol {

		// [CoreImageFilterProperty ("inputAngle")]
		// float Angle { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIUnsharpMask.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIUnsharpMask']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIUnsharpMask : CIUnsharpMaskProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIVibrance.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIVibrance']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIVibrance : CIVibranceProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIVignette.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIVignette']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIVignette : CIVignetteProtocol {
	}

	/// <include file="../docs/api/CoreImage/CIVignetteEffect.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIVignetteEffect']/*" />
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIVignetteEffect : CIVignetteEffectProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIVortexDistortion")]
	interface CIVortexDistortionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>The angle, in degrees, through which to rotate.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIVortexDistortion.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIVortexDistortion']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIDistortionFilter))]
	interface CIVortexDistortion : CIVortexDistortionProtocol {

		// [CoreImageFilterProperty ("inputAngle")]
		// float Angle { get; set; }
	}

	/// <include file="../docs/api/CoreImage/CIWhitePointAdjust.xml" path="/Documentation/Docs[@DocId='T:CoreImage.CIWhitePointAdjust']/*" />
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIWhitePointAdjust : CIWhitePointAdjustProtocol {
	}

	/// <summary>Applies a blur that simulates a camera zoom during capture.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIZoomBlur : CIZoomBlurProtocol {
	}

	/// <summary>Simulates a blurred depth of field and applies a saturation effect to the 'in-focus' region.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDepthOfField : CIDepthOfFieldProtocol {
	}

	/// <summary>Creates an artificial sun and light-ray effect.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISunbeamsGenerator : CISunbeamsGeneratorProtocol {
	}

	/// <summary>The CIFaceBalance CoreImage filter</summary>
	[CoreImageFilter (DefaultCtorVisibility = MethodAttributes.PrivateScope)]
	[BaseType (typeof (CIFilter))]
	interface CIFaceBalance {
	}

	/// <summary>A <see cref="CoreImage.CIFilter" /> that blurs based on a black-and-white mask.</summary>
	///     <remarks>
	///       <para>The blur radius varies from 0 to <see cref="CoreImage.CIMaskedVariableBlur.Radius" />. In the mask image, pure black is 0 radius (no blur) and white is maximum radius (blurriest).</para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[CoreImageFilter]
	[BaseType (typeof (CIFilter))]
	interface CIMaskedVariableBlur : CIMaskedVariableBlurProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIClamp {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>A vector whose values define the clamped rectangle within the <see cref="InputImage" />.</summary>
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	/// <summary>Creates a gradient specified using HSV.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIHueSaturationValueGradient : CIHueSaturationValueGradientProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CINinePartStretched")]
	interface CINinePartStretchedProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("breakpoint0", ArgumentSemantic.Assign)]
		CGPoint InputBreakpoint0 { get; set; }

		[Abstract]
		[Export ("breakpoint1", ArgumentSemantic.Assign)]
		CGPoint InputBreakpoint1 { get; set; }

		[Abstract]
		[Export ("growAmount", ArgumentSemantic.Assign)]
		CGPoint InputGrowAmount { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CINinePartStretched : CINinePartStretchedProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CINinePartTiled")]
	interface CINinePartTiledProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }


		[Abstract]
		[Export ("breakpoint0", ArgumentSemantic.Assign)]
		CGPoint InputBreakpoint0 { get; set; }


		[Abstract]
		[Export ("breakpoint1", ArgumentSemantic.Assign)]
		CGPoint InputBreakpoint1 { get; set; }


		[Abstract]
		[Export ("growAmount", ArgumentSemantic.Assign)]
		CGPoint InputGrowAmount { get; set; }

		[Abstract]
		[Export ("flipYTiles")]
		bool FlipYTiles { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CINinePartTiled : CINinePartTiledProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIThermal : CIThermalProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIXRay : CIXRayProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMinMaxRed")]
	interface CIAreaMinMaxRedProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIAreaMaximum))]
	interface CIAreaMinMaxRed : CIAreaMinMaxRedProtocol {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputExtent")]
		CIVector Extent { get; set; }
	}

	/// <summary>A <see cref="CoreImage.CIFilter" /> that generates images.</summary>
	[CoreImageFilter]
	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIImageGenerator {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputScaleFactor")]
		float ScaleFactor { get; set; }
	}

	/// <summary>A <see cref="CoreImage.CIImageGenerator" /> that generates an image from attributed text.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIImageGenerator))]
	interface CIAttributedTextImageGenerator : CIAttributedTextImageGeneratorProtocol {
	}

	/// <summary>A <see cref="CoreImage.CIFilter" /> that generates barcodes.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIBarcodeGenerator : CIBarcodeGeneratorProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputCGImageForQRCodeDescriptor")]
		CGImage OutputCGImageForQRCodeDescriptor { get; }

		[CoreImageFilterProperty ("outputCGImageForPDF417CodeDescriptor")]
		CGImage OutputCGImageForPdf417CodeDescriptor { get; }

		[CoreImageFilterProperty ("outputCGImageForDataMatrixCodeDescriptor")]
		CGImage OutputCGImageForDataMatrixCodeDescriptor { get; }

		[CoreImageFilterProperty ("outputCGImageForAztecCodeDescriptor")]
		CGImage OutputCGImageForAztecCodeDescriptor { get; }

		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	// Maybe 'typeof (CIScaleTransform)' (shared 'Scale' and 'AspectRatio' property).
	// It's possible to add ours but it can bite us back in the future if Apple introduce the same with different properties.
	[BaseType (typeof (CIFilter))]
	interface CIBicubicScaleTransform : CIBicubicScaleTransformProtocol {
	}

	[CoreImageFilter]
	[Abstract]
	[BaseType (typeof (CIFilter))]
	interface CILinearBlur {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }
	}

	/// <summary>A blur simulating the the unfocused background of a larger-aperture camera with an iris shutter.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CILinearBlur))]
	interface CIBokehBlur : CIBokehBlurProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))] // Could almost be typeof 'CIColorCube' but property is 'inputCube0Data' not 'inputCubeData'
	interface CIColorCubesMixedWithMask : CIColorCubesMixedWithMaskProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIColorCurves : CIColorCurvesProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDepthBlurEffect {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputAperture")]
		float Aperture { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputCalibrationData")]
		AVCameraCalibrationData CalibrationData { get; set; }

		// Radar: https://trello.com/c/9eA2BA2o
		// Don't know how to test this as I don't know which keys are valid.
		// [CoreImageFilterProperty ("inputTuningParameters")]
		// NSDictionary WeakTuningParameters { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputNosePositions")]
		CIVector NosePositions { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputLumaNoiseScale")]
		float LumaNoiseScale { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputChinPositions")]
		CIVector ChinPositions { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputDisparityImage")]
		CIImage DisparityImage { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputScaleFactor")]
		float ScaleFactor { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputRightEyePositions")]
		CIVector RightEyePositions { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputLeftEyePositions")]
		CIVector LeftEyePositions { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputFocusRect")]
		CIVector FocusRect { get; set; }

		[CoreImageFilterProperty ("inputMatteImage")]
		CIImage MatteImage { get; set; }

		[CoreImageFilterProperty ("inputHairImage")]
		CIImage HairImage { get; set; }

		[CoreImageFilterProperty ("inputShape")]
		string Shape { get; set; }

		[CoreImageFilterProperty ("inputAuxDataMetadata")]
		CGImageMetadata AuxDataMetadata { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[CoreImageFilterProperty ("inputGainMap")]
		CIImage GainMap { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[CoreImageFilterProperty ("inputGlassesImage")]
		CIImage GlassesImage { get; set; }
	}

	[CoreImageFilter]
	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDepthDisparityConverter { }

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIDepthDisparityConverter))]
	interface CIDepthToDisparity : CIDepthToDisparityProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIDepthDisparityConverter))]
	interface CIDisparityToDepth : CIDisparityToDepthProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIEdgePreserveUpsampleFilter : CIEdgePreserveUpsampleProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CILabDeltaE : CILabDeltaEProtocol {
	}

	/// <summary>A <see cref="CoreImage.CIImageGenerator" /> that generates an image from text.</summary>
	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIImageGenerator))]
	interface CITextImageGenerator : CITextImageGeneratorProtocol {
	}

	[CoreImageFilter]
	[Abstract]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIMorphology {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyGradient : CIMorphologyGradientProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyMaximum : CIMorphologyMaximumProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphology))]
	interface CIMorphologyMinimum : CIMorphologyMinimumProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithBlueMask { }

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBlendWithMask))]
	interface CIBlendWithRedMask { }

	/// <summary>Base class for barcode descriptions. Subclasses for specific schemes define relevant properties.</summary>
	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface CIBarcodeDescriptor : NSSecureCoding, NSCopying {
		// empty
	}

	/// <summary>A <see cref="CoreImage.CIBarcodeDescriptor" /> for QR codes.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBarcodeDescriptor))]
	interface CIQRCodeDescriptor {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("errorCorrectedPayload")]
		NSData ErrorCorrectedPayload { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("symbolVersion")]
		nint SymbolVersion { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("maskPattern")]
		byte /* uint8_t */ MaskPattern { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("errorCorrectionLevel")]
		CIQRCodeErrorCorrectionLevel ErrorCorrectionLevel { get; }

		/// <param name="errorCorrectedPayload">To be added.</param>
		/// <param name="symbolVersion">To be added.</param>
		/// <param name="maskPattern">To be added.</param>
		/// <param name="errorCorrectionLevel">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithPayload:symbolVersion:maskPattern:errorCorrectionLevel:")]
		NativeHandle Constructor (NSData errorCorrectedPayload, nint symbolVersion, byte maskPattern, CIQRCodeErrorCorrectionLevel errorCorrectionLevel);

		/// <param name="errorCorrectedPayload">To be added.</param>
		/// <param name="symbolVersion">To be added.</param>
		/// <param name="maskPattern">To be added.</param>
		/// <param name="errorCorrectionLevel">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("descriptorWithPayload:symbolVersion:maskPattern:errorCorrectionLevel:")]
		[return: NullAllowed]
		CIQRCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, nint symbolVersion, byte maskPattern, CIQRCodeErrorCorrectionLevel errorCorrectionLevel);
	}

	/// <summary>A <see cref="CoreImage.CIBarcodeDescriptor" /> for the Aztec barcode scheme.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBarcodeDescriptor))]
	interface CIAztecCodeDescriptor {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("errorCorrectedPayload")]
		NSData ErrorCorrectedPayload { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isCompact")]
		bool IsCompact { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("layerCount")]
		nint LayerCount { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("dataCodewordCount")]
		nint DataCodewordCount { get; }

		/// <param name="errorCorrectedPayload">To be added.</param>
		/// <param name="isCompact">To be added.</param>
		/// <param name="layerCount">To be added.</param>
		/// <param name="dataCodewordCount">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithPayload:isCompact:layerCount:dataCodewordCount:")]
		NativeHandle Constructor (NSData errorCorrectedPayload, bool isCompact, nint layerCount, nint dataCodewordCount);

		/// <param name="errorCorrectedPayload">To be added.</param>
		/// <param name="isCompact">To be added.</param>
		/// <param name="layerCount">To be added.</param>
		/// <param name="dataCodewordCount">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("descriptorWithPayload:isCompact:layerCount:dataCodewordCount:")]
		[return: NullAllowed]
		CIAztecCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, bool isCompact, nint layerCount, nint dataCodewordCount);
	}

	/// <summary>A <see cref="CoreImage.CIBarcodeDescriptor" /> for the CIPDF417 barcode scheme.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBarcodeDescriptor), Name = "CIPDF417CodeDescriptor")]
	interface CIPdf417CodeDescriptor {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("errorCorrectedPayload")]
		NSData ErrorCorrectedPayload { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isCompact")]
		bool IsCompact { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("rowCount")]
		nint RowCount { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("columnCount")]
		nint ColumnCount { get; }

		/// <param name="errorCorrectedPayload">To be added.</param>
		/// <param name="isCompact">To be added.</param>
		/// <param name="rowCount">To be added.</param>
		/// <param name="columnCount">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithPayload:isCompact:rowCount:columnCount:")]
		NativeHandle Constructor (NSData errorCorrectedPayload, bool isCompact, nint rowCount, nint columnCount);

		/// <param name="errorCorrectedPayload">To be added.</param>
		/// <param name="isCompact">To be added.</param>
		/// <param name="rowCount">To be added.</param>
		/// <param name="columnCount">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("descriptorWithPayload:isCompact:rowCount:columnCount:")]
		[return: NullAllowed]
		CIPdf417CodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, bool isCompact, nint rowCount, nint columnCount);
	}

	/// <summary>A <see cref="CoreImage.CIBarcodeDescriptor" /> for the Data Matrix barcode scheme.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIBarcodeDescriptor))]
	interface CIDataMatrixCodeDescriptor {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("errorCorrectedPayload")]
		NSData ErrorCorrectedPayload { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("rowCount")]
		nint RowCount { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("columnCount")]
		nint ColumnCount { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("eccVersion")]
		CIDataMatrixCodeEccVersion EccVersion { get; }

		/// <param name="errorCorrectedPayload">To be added.</param>
		/// <param name="rowCount">To be added.</param>
		/// <param name="columnCount">To be added.</param>
		/// <param name="eccVersion">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithPayload:rowCount:columnCount:eccVersion:")]
		NativeHandle Constructor (NSData errorCorrectedPayload, nint rowCount, nint columnCount, CIDataMatrixCodeEccVersion eccVersion);

		/// <param name="errorCorrectedPayload">To be added.</param>
		/// <param name="rowCount">To be added.</param>
		/// <param name="columnCount">To be added.</param>
		/// <param name="eccVersion">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("descriptorWithPayload:rowCount:columnCount:eccVersion:")]
		[return: NullAllowed]
		CIDataMatrixCodeDescriptor CreateDescriptor (NSData errorCorrectedPayload, nint rowCount, nint columnCount, CIDataMatrixCodeEccVersion eccVersion);
	}

	/// <summary>A <see cref="CoreImage.CIColorKernel" /> that blends images using the GPU.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIColorKernel))]
	[DisableDefaultCtor] // Handle is nil for `init`
	interface CIBlendKernel {

		/// <param name="string">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.TvOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("kernelWithString:")]
		[return: NullAllowed]
		CIBlendKernel CreateKernel (string @string);

		/// <param name="foreground">To be added.</param>
		///         <param name="background">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("applyWithForeground:background:")]
		[return: NullAllowed]
		CIImage Apply (CIImage foreground, CIImage background);

		[MacCatalyst (13, 1)]
		[Export ("applyWithForeground:background:colorSpace:")]
		[return: NullAllowed]
		CIImage Apply (CIImage foreground, CIImage background, CGColorSpace colorSpace);

		// @interface BuiltIn (CIBlendKernel)

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("componentAdd", ArgumentSemantic.Strong)]
		CIBlendKernel ComponentAdd { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("componentMultiply", ArgumentSemantic.Strong)]
		CIBlendKernel ComponentMultiply { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("componentMin", ArgumentSemantic.Strong)]
		CIBlendKernel ComponentMin { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("componentMax", ArgumentSemantic.Strong)]
		CIBlendKernel ComponentMax { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("clear", ArgumentSemantic.Strong)]
		CIBlendKernel Clear { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("source", ArgumentSemantic.Strong)]
		CIBlendKernel Source { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("destination", ArgumentSemantic.Strong)]
		CIBlendKernel Destination { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("sourceOver", ArgumentSemantic.Strong)]
		CIBlendKernel SourceOver { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("destinationOver", ArgumentSemantic.Strong)]
		CIBlendKernel DestinationOver { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("sourceIn", ArgumentSemantic.Strong)]
		CIBlendKernel SourceIn { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("destinationIn", ArgumentSemantic.Strong)]
		CIBlendKernel DestinationIn { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("sourceOut", ArgumentSemantic.Strong)]
		CIBlendKernel SourceOut { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("destinationOut", ArgumentSemantic.Strong)]
		CIBlendKernel DestinationOut { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("sourceAtop", ArgumentSemantic.Strong)]
		CIBlendKernel SourceAtop { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("destinationAtop", ArgumentSemantic.Strong)]
		CIBlendKernel DestinationAtop { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("exclusiveOr", ArgumentSemantic.Strong)]
		CIBlendKernel ExclusiveOr { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("multiply", ArgumentSemantic.Strong)]
		CIBlendKernel Multiply { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("screen", ArgumentSemantic.Strong)]
		CIBlendKernel Screen { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("overlay", ArgumentSemantic.Strong)]
		CIBlendKernel Overlay { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("darken", ArgumentSemantic.Strong)]
		CIBlendKernel Darken { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("lighten", ArgumentSemantic.Strong)]
		CIBlendKernel Lighten { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("colorDodge", ArgumentSemantic.Strong)]
		CIBlendKernel ColorDodge { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("colorBurn", ArgumentSemantic.Strong)]
		CIBlendKernel ColorBurn { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("hardLight", ArgumentSemantic.Strong)]
		CIBlendKernel HardLight { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("softLight", ArgumentSemantic.Strong)]
		CIBlendKernel SoftLight { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("difference", ArgumentSemantic.Strong)]
		CIBlendKernel Difference { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("exclusion", ArgumentSemantic.Strong)]
		CIBlendKernel Exclusion { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("hue", ArgumentSemantic.Strong)]
		CIBlendKernel Hue { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("saturation", ArgumentSemantic.Strong)]
		CIBlendKernel Saturation { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("color", ArgumentSemantic.Strong)]
		CIBlendKernel Color { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("luminosity", ArgumentSemantic.Strong)]
		CIBlendKernel Luminosity { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("subtract", ArgumentSemantic.Strong)]
		CIBlendKernel Subtract { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("divide", ArgumentSemantic.Strong)]
		CIBlendKernel Divide { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("linearBurn", ArgumentSemantic.Strong)]
		CIBlendKernel LinearBurn { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("linearDodge", ArgumentSemantic.Strong)]
		CIBlendKernel LinearDodge { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("vividLight", ArgumentSemantic.Strong)]
		CIBlendKernel VividLight { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("linearLight", ArgumentSemantic.Strong)]
		CIBlendKernel LinearLight { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("pinLight", ArgumentSemantic.Strong)]
		CIBlendKernel PinLight { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("hardMix", ArgumentSemantic.Strong)]
		CIBlendKernel HardMix { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("darkerColor", ArgumentSemantic.Strong)]
		CIBlendKernel DarkerColor { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("lighterColor", ArgumentSemantic.Strong)]
		CIBlendKernel LighterColor { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Handle is null if created thru `init`
	interface CIRenderDestination {

		/// <param name="pixelBuffer">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithPixelBuffer:")]
		NativeHandle Constructor (CVPixelBuffer pixelBuffer);

		/// <param name="surface">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithIOSurface:")]
		NativeHandle Constructor (IOSurface.IOSurface surface);

		/// <param name="texture">To be added.</param>
		/// <param name="commandBuffer">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithMTLTexture:commandBuffer:")]
		NativeHandle Constructor (IMTLTexture texture, [NullAllowed] IMTLCommandBuffer commandBuffer);

		/// <param name="width">To be added.</param>
		/// <param name="height">To be added.</param>
		/// <param name="pixelFormat">To be added.</param>
		/// <param name="commandBuffer">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="block">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithWidth:height:pixelFormat:commandBuffer:mtlTextureProvider:")]
		NativeHandle Constructor (nuint width, nuint height, MTLPixelFormat pixelFormat, [NullAllowed] IMTLCommandBuffer commandBuffer, [NullAllowed] Func<IMTLTexture> block);

		/// <param name="texture">To be added.</param>
		/// <param name="target">To be added.</param>
		/// <param name="width">To be added.</param>
		/// <param name="height">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithGLTexture:target:width:height:")]
		NativeHandle Constructor (uint texture, uint target, nuint width, nuint height);

		[Export ("initWithBitmapData:width:height:bytesPerRow:format:")]
		NativeHandle Constructor (IntPtr data, nuint width, nuint height, nuint bytesPerRow, CIFormat format);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("width")]
		nuint Width { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("height")]
		nuint Height { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("alphaMode", ArgumentSemantic.Assign)]
		CIRenderDestinationAlphaMode AlphaMode { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("flipped")]
		bool Flipped { [Bind ("isFlipped")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("dithered")]
		bool Dithered { [Bind ("isDithered")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("clamped")]
		bool Clamped { [Bind ("isClamped")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("blendKernel", ArgumentSemantic.Retain)]
		CIBlendKernel BlendKernel { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("blendsInDestinationColorSpace")]
		bool BlendsInDestinationColorSpace { get; set; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("captureTraceURL", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSUrl CaptureTraceUrl { get; set; }
	}

	/// <summary>Holds information about a render pass, including timing information and number of pixels processed.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no docs, but only returned from CIRenderTask.WaitUntilCompleted. Handle is null if created thru `init`
	interface CIRenderInfo {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("kernelExecutionTime")]
		double KernelExecutionTime { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("passCount")]
		nint PassCount { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("pixelsProcessed")]
		nint PixelsProcessed { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("kernelCompileTime")]
		double KernelCompileTime { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no docs, but only returned from CIContext.StartTaskToRender. Handle is null if created thru `init`
	interface CIRenderTask {
		/// <summary>Gets the estimated number of pixels that the render will process.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("plannedPixelsProcessed")]
		nint PlannedPixelsProcessed { get; }

		/// <summary>Gets the estimated number of pixels that will be overdrawn during the render.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("plannedPixelsOverdrawn")]
		nint PlannedPixelsOverdrawn { get; }

		/// <summary>Gets the estimated number of passes required by the render.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("plannedPassCount")]
		nint PlannedPassCount { get; }

		/// <summary>Gets the estimated peak memory required for intermediate buffers, in megabytes.</summary>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("plannedPeakMemory")]
		nint PlannedPeakMemory { get; }

		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("waitUntilCompletedAndReturnError:")]
		[return: NullAllowed]
		CIRenderInfo WaitUntilCompleted ([NullAllowed] out NSError error);
	}

	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (CIContext))]
	interface CIContext_CIRenderDestination {

		/// <param name="image">To be added.</param>
		/// <param name="fromRect">To be added.</param>
		/// <param name="destination">To be added.</param>
		/// <param name="atPoint">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("prepareRender:fromRect:toDestination:atPoint:error:")]
		bool PrepareRender (CIImage image, CGRect fromRect, CIRenderDestination destination, CGPoint atPoint, [NullAllowed] out NSError error);

		/// <param name="image">The image to estimate rendering for.</param>
		/// <param name="fromRect">The region of the image to render.</param>
		/// <param name="destination">The render destination.</param>
		/// <param name="atPoint">The point in the destination where the origin of <paramref name="fromRect" /> is placed.</param>
		/// <param name="error">The error if the render could not be estimated.</param>
		/// <summary>Estimates the resources required to render an image without executing the render.</summary>
		/// <returns>A render task containing the estimated statistics, or <see langword="null" /> if estimation failed.</returns>
		[TV (27, 0), Mac (27, 0), iOS (27, 0), MacCatalyst (27, 0)]
		[Export ("estimateRender:fromRect:toDestination:atPoint:error:")]
		[return: NullAllowed]
		CIRenderTask EstimateRender (CIImage image, CGRect fromRect, CIRenderDestination destination, CGPoint atPoint, [NullAllowed] out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="fromRect">To be added.</param>
		/// <param name="destination">To be added.</param>
		/// <param name="atPoint">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("startTaskToRender:fromRect:toDestination:atPoint:error:")]
		[return: NullAllowed]
		CIRenderTask StartTaskToRender (CIImage image, CGRect fromRect, CIRenderDestination destination, CGPoint atPoint, [NullAllowed] out NSError error);

		/// <param name="image">To be added.</param>
		/// <param name="destination">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("startTaskToRender:toDestination:error:")]
		[return: NullAllowed]
		CIRenderTask StartTaskToRender (CIImage image, CIRenderDestination destination, [NullAllowed] out NSError error);

		/// <param name="destination">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("startTaskToClear:error:")]
		[return: NullAllowed]
		CIRenderTask StartTaskToClear (CIRenderDestination destination, [NullAllowed] out NSError error);
	}

	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	partial interface CIImageRepresentationKeys {

		[Field ("kCGImageDestinationLossyCompressionQuality", "ImageIO")]
		NSString LossyCompressionQualityKey { get; }

		[Field ("kCIImageRepresentationAVDepthData")]
		NSString AVDepthDataKey { get; }

		[Field ("kCIImageRepresentationDepthImage")]
		NSString DepthImageKey { get; }

		[Field ("kCIImageRepresentationDisparityImage")]
		NSString DisparityImageKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationAVPortraitEffectsMatte")]
		NSString AVPortraitEffectsMatteKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationPortraitEffectsMatteImage")]
		NSString PortraitEffectsMatteImageKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationAVSemanticSegmentationMattes")]
		NSString SemanticSegmentationMattesKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationSemanticSegmentationSkinMatteImage")]
		NSString SemanticSegmentationSkinMatteImageKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationSemanticSegmentationHairMatteImage")]
		NSString SemanticSegmentationHairMatteImageKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCIImageRepresentationSemanticSegmentationTeethMatteImage")]
		NSString SemanticSegmentationTeethMatteImageKey { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Field ("kCIImageRepresentationSemanticSegmentationGlassesMatteImage")]
		NSString SemanticSegmentationGlassesMatteImageKey { get; }

		[iOS (14, 3), TV (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("kCIImageRepresentationSemanticSegmentationSkyMatteImage")]
		NSString SemanticSegmentationSkyMatteImageKey { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("kCIImageRepresentationHDRImage")]
		NSString HdrImageKey { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("kCIImageRepresentationHDRGainMapImage")]
		NSString HdrGainMapImageKey { get; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("kCIImageRepresentationHDRGainMapAsRGB")]
		NSString RepresentationHdrGainMapAsRgbKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("CIImageRepresentationKeys")]
	interface CIImageRepresentationOptions {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		float LossyCompressionQuality { get; set; }

		AVDepthData AVDepthData { get; set; }

		CIImage DepthImage { get; set; }

		CIImage DisparityImage { get; set; }

		[MacCatalyst (13, 1)]
		CIImage PortraitEffectsMatteImage { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		AVPortraitEffectsMatte AVPortraitEffectsMatte { get; set; }

		[MacCatalyst (13, 1)]
		AVSemanticSegmentationMatte [] SemanticSegmentationMattes { get; set; }

		[MacCatalyst (13, 1)]
		CIImage SemanticSegmentationSkinMatteImage { get; set; }

		[MacCatalyst (13, 1)]
		CIImage SemanticSegmentationHairMatteImage { get; set; }

		[MacCatalyst (13, 1)]
		CIImage SemanticSegmentationTeethMatteImage { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		CIImage SemanticSegmentationGlassesMatteImage { get; }

		[iOS (14, 3), TV (14, 3)]
		[MacCatalyst (14, 3)]
		CIImage SemanticSegmentationSkyMatteImage { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		CIImage HdrImage { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		CIImage HdrGainMapImage { get; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		bool RepresentationHdrGainMapAsRgb { get; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIAreaMinMax")]
	interface CIAreaMinMaxProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIAreaMinMax : CIAreaReductionFilterProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputImageNonMPS")]
		CIImage OutputImageNonMps { get; }

		[iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[CoreImageFilterProperty ("outputImageMPS")]
		CIImage OutputImageMps { get; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDither : CIDitherProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIGuidedFilter {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputGuideImage")]
		CIImage GuideImage { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputEpsilon")]
		float Epsilon { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputRadius")]
		float Radius { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIMeshGenerator : CIMeshGeneratorProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIMix : CIMixProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISampleNearest {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CICameraCalibrationLensCorrection {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputAVCameraCalibrationData")]
		AVCameraCalibrationData AVCameraCalibrationData { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputUseInverseLookUpTable")]
		bool UseInverseLookUpTable { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CICoreMLModelFilter {

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[CoreImageFilterProperty ("inputModel")]
		MLModel Model { get; set; }

		[CoreImageFilterProperty ("inputHeadIndex")]
		int HeadIndex { get; set; }

		[CoreImageFilterProperty ("inputSoftmaxNormalization")]
		bool SoftmaxNormalization { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CISaliencyMapFilter : CISaliencyMapProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDocumentEnhancer : CIDocumentEnhancerProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIKMeans")]
	interface CIKMeansProtocol : CIAreaReductionFilterProtocol {

		[Abstract]
		[NullAllowed, Export ("inputMeans", ArgumentSemantic.Retain)]
		CIImage Means { get; set; }

		[Abstract]
		[Export ("count")]
		nint InputCount { get; set; }

		[Abstract]
		[Export ("passes")]
		float InputPasses { get; set; }

		[Abstract]
		[Export ("perceptual")]
		bool Perceptual { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIReductionFilter))]
	interface CIKMeans : CIKMeansProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	[Abstract]
	interface CIMorphologyRectangle {
		[CoreImageFilterProperty ("inputHeight")]
		float InputHeight { get; set; }

		[CoreImageFilterProperty ("inputWidth")]
		float InputWidth { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphologyRectangle))]
	interface CIMorphologyRectangleMaximum : CIMorphologyRectangleMaximumProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIMorphologyRectangle))]
	interface CIMorphologyRectangleMinimum : CIMorphologyRectangleMinimumProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPaletteCentroid : CIPaletteCentroidProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPalettize : CIPalettizeProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	[Abstract]
	interface CIKeystoneCorrection {

		[CoreImageFilterProperty ("inputFocalLength")]
		float FocalLength { get; set; }

		[CoreImageFilterProperty ("inputTopRight")]
		CGPoint InputTopRight { get; set; }

		[CoreImageFilterProperty ("inputBottomRight")]
		CGPoint InputBottomRight { get; set; }

		[CoreImageFilterProperty ("inputTopLeft")]
		CGPoint InputTopLeft { get; set; }

		[CoreImageFilterProperty ("inputBottomLeft")]
		CGPoint InputBottomLeft { get; set; }
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIKeystoneCorrection))]
	interface CIKeystoneCorrectionCombined : CIKeystoneCorrectionCombinedProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIKeystoneCorrection))]
	interface CIKeystoneCorrectionHorizontal : CIKeystoneCorrectionHorizontalProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIKeystoneCorrection))]
	interface CIKeystoneCorrectionVertical : CIKeystoneCorrectionVerticalProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIPerspectiveRotate : CIPerspectiveRotateProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputTransform")]
		CGAffineTransform OutputTransform { get; }
#endif
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIGaborGradients : CIGaborGradientsProtocol {
	}

	[CoreImageFilter]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIRoundedRectangleGenerator : CIRoundedRectangleGeneratorProtocol {
	}

	#region Protocols

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIFilter")]
	// CIFilter already exists so we're using the Swift name
	interface CIFilterProtocol {

		[Abstract]
		[CoreImageFilterProperty ("outputImage")]
		[NullAllowed, Export ("outputImage")]
		CIImage OutputImage { get; }

		[Static]
		[NullAllowed, Export ("customAttributes")]
		NSDictionary<NSString, NSObject> CustomAttributes { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CITransitionFilter")]
	interface CITransitionFilterProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>The image that will be displayed at the end of the transition.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("targetImage", ArgumentSemantic.Retain)]
		CIImage TargetImage { get; set; }

		/// <summary>Gets or sets the current time in the transition.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("time")]
		float Time { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIAccordionFoldTransition")]
	interface CIAccordionFoldTransitionProtocol : CITransitionFilterProtocol {

		/// <summary>Gets or sets the the position from which to start the accordion transition.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("bottomHeight")]
		float BottomHeight { get; set; }

		/// <summary>Gets or sets the number of folds to use in the transition.</summary>
		[Abstract]
		[Export ("numberOfFolds")]
		// renamed for compatibility (originally bound as an integer)
		float FoldCount { get; set; }

		/// <summary>Gets or sets the fold shadow amount.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("foldShadowAmount")]
		float FoldShadowAmount { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIAffineClamp")]
	interface CIAffineClampProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIAffineTile")]
	interface CIAffineTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIAttributedTextImageGenerator")]
	interface CIAttributedTextImageGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("text", ArgumentSemantic.Retain)]
		NSAttributedString Text { get; set; }

		[Abstract]
		[Export ("scaleFactor")]
		float ScaleFactor { get; set; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("padding")]
		float Padding { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIAztecCodeGenerator")]
	interface CIAztecCodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		/// <summary>Gets or sets the percentage of redundancy of the code.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("correctionLevel")]
		float CorrectionLevel { get; set; }

		/// <summary>Gets or sets the number of concentric squares to use when encoding data for the code.</summary>
		[Abstract]
		[Export ("layers")]
		float InputLayers { get; set; }

		/// <summary>Gets or sets whether to create compact or full-size code.</summary>
		[Abstract]
		[Export ("compactStyle")]
		float InputCompactStyle { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBarcodeGenerator")]
	interface CIBarcodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("barcodeDescriptor", ArgumentSemantic.Retain)]
		CIBarcodeDescriptor BarcodeDescriptor { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBarsSwipeTransition")]
	interface CIBarsSwipeTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("barOffset")]
		float BarOffset { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBicubicScaleTransform")]
	interface CIBicubicScaleTransformProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("aspectRatio")]
		float AspectRatio { get; set; }

		[Abstract]
		[CoreImageFilterProperty ("inputB")] // name differs from the export
		[Export ("parameterB")]
		float ParameterB { get; set; }

		[Abstract]
		[CoreImageFilterProperty ("inputC")] // name differs from the export
		[Export ("parameterC")]
		float ParameterC { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBlendWithMask")]
	interface CIBlendWithMaskProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Retain)]
		CIImage BackgroundImage { get; set; }

		/// <summary>Gets or sets the mask to use for blending.</summary>
		[Abstract]
		[NullAllowed, Export ("maskImage", ArgumentSemantic.Retain)]
		CIImage MaskImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBloom")]
	interface CIBloomProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the radius of the center of the bloom.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the intensity of the bloom.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBokehBlur")]
	interface CIBokehBlurProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("ringAmount")]
		float RingAmount { get; set; }

		[Abstract]
		[Export ("ringSize")]
		float RingSize { get; set; }

		[Abstract]
		[Export ("softness")]
		float Softness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIBoxBlur")]
	interface CIBoxBlurProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>The circular extent of the filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICheckerboardGenerator")]
	interface CICheckerboardGeneratorProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the center of the checkerboard pattern.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets first square color</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		/// <summary>Gets or sets second square color</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }

		/// <summary>Gets or sets the length of the sides of the squares.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		/// <summary>Gets or sets the sharpness of the stripe pattern. 1 is sharp. 0 is maximally blurry.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICircularScreen")]
	interface CICircularScreenProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICMYKHalftone")]
	interface CICmykHalftoneProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }

		[Abstract]
		[CoreImageFilterProperty ("inputGCR")]
		[Export ("grayComponentReplacement")]
		float GrayComponentReplacement { get; set; }

		[Abstract]
		[CoreImageFilterProperty ("inputUCR")]
		[Export ("underColorRemoval")]
		float UnderColorRemoval { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICode128BarcodeGenerator")]
	interface CICode128BarcodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		/// <summary>Gets or sets the width of the quiet space in the code.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("quietSpace")]
		float QuietSpace { get; set; }

		// The availability attributes here look redundant because they're already on the type,
		// but it makes a difference when this member is inlined into another type, in which case
		// these attributes are copied as well (while the type's attributes aren't).
		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("barcodeHeight")]
		float BarcodeHeight { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorClamp")]
	interface CIColorClampProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the minimum component values.</summary>
		[Abstract]
		[Export ("minComponents", ArgumentSemantic.Retain)]
		CIVector MinComponents { get; set; }

		/// <summary>Gets or sets the maximum component values.</summary>
		[Abstract]
		[Export ("maxComponents", ArgumentSemantic.Retain)]
		CIVector MaxComponents { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorControls")]
	interface CIColorControlsProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the saturation of the resulting image. Values greater than 1 are more saturated than the original image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("saturation")]
		float Saturation { get; set; }

		/// <summary>Gets or sets the brightness bias. </summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("brightness")]
		float Brightness { get; set; }

		/// <summary>Gets or sets the contrast in the resulting image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("contrast")]
		float Contrast { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorCrossPolynomial")]
	interface CIColorCrossPolynomialProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>A ten-element vector where the first nine values are cross product weights, and the final value in a bias.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("redCoefficients", ArgumentSemantic.Retain)]
		CIVector RedCoefficients { get; set; }

		/// <summary>A ten-element vector where the first nine values are cross product weights, and the final value in a bias.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("greenCoefficients", ArgumentSemantic.Retain)]
		CIVector GreenCoefficients { get; set; }

		/// <summary>A ten-element vector where the first nine values are cross product weights, and the final value in a bias.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("blueCoefficients", ArgumentSemantic.Retain)]
		CIVector BlueCoefficients { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorCube")]
	interface CIColorCubeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the dimension of the cube data.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("cubeDimension")]
		float CubeDimension { get; set; }

		/// <summary>Gets or sets the cube data.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("cubeData", ArgumentSemantic.Retain)]
		NSData CubeData { get; set; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorCubesMixedWithMask")]
	interface CIColorCubesMixedWithMaskProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("maskImage", ArgumentSemantic.Retain)]
		CIImage MaskImage { get; set; }

		[Abstract]
		[Export ("cubeDimension")]
		float CubeDimension { get; set; }

		[Abstract]
		[Export ("cube0Data", ArgumentSemantic.Retain)]
		NSData Cube0Data { get; set; }

		[Abstract]
		[Export ("cube1Data", ArgumentSemantic.Retain)]
		NSData Cube1Data { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorCubeWithColorSpace")]
	interface CIColorCubeWithColorSpaceProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("cubeDimension")]
		float CubeDimension { get; set; }

		[Abstract]
		[Export ("cubeData", ArgumentSemantic.Retain)]
		NSData CubeData { get; set; }

		/// <summary>Gets or sets the color space to use.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorCurves")]
	interface CIColorCurvesProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("curvesData", ArgumentSemantic.Retain)]
		NSData CurvesData { get; set; }

		[Abstract]
		[Export ("curvesDomain", ArgumentSemantic.Retain)]
		CIVector CurvesDomain { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorInvert")]
	interface CIColorInvertProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorMap")]
	interface CIColorMapProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the gradient mapping image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("gradientImage", ArgumentSemantic.Retain)]
		CIImage GradientImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorMatrix")]
	interface CIColorMatrixProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Weights to use to calculate the red value.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("RVector", ArgumentSemantic.Retain)]
		CIVector RVector { get; set; }

		/// <summary>Weights to use to calculate the green value.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("GVector", ArgumentSemantic.Retain)]
		CIVector GVector { get; set; }

		/// <summary>Weights to use to calculate the blue value.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("BVector", ArgumentSemantic.Retain)]
		CIVector BVector { get; set; }

		/// <summary>Weights to use to calculate the alpha value.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("AVector", ArgumentSemantic.Retain)]
		CIVector AVector { get; set; }

		/// <summary>Values to add to each component.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("biasVector", ArgumentSemantic.Retain)]
		CIVector BiasVector { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorMonochrome")]
	interface CIColorMonochromeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the monochrome color.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		/// <summary>Gets or sets the intensity of the effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorPolynomial")]
	interface CIColorPolynomialProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("redCoefficients", ArgumentSemantic.Retain)]
		CIVector RedCoefficients { get; set; }

		[Abstract]
		[Export ("greenCoefficients", ArgumentSemantic.Retain)]
		CIVector GreenCoefficients { get; set; }

		[Abstract]
		[Export ("blueCoefficients", ArgumentSemantic.Retain)]
		CIVector BlueCoefficients { get; set; }

		/// <summary>Gets or sets the coefficients for the cubic polynomial that will be used to calculate the new alpha channel value.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("alphaCoefficients", ArgumentSemantic.Retain)]
		CIVector AlphaCoefficients { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIColorPosterize")]
	interface CIColorPosterizeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the single brightness level to which to shift all the color components in the image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("levels")]
		float Levels { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIComicEffect")]
	interface CIComicEffectProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICompositeOperation")]
	interface CICompositeOperationProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Retain)]
		CIImage BackgroundImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIConvolution")]
	interface CIConvolutionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("weights", ArgumentSemantic.Retain)]
		CIVector Weights { get; set; }

		[Abstract]
		[Export ("bias")]
		float Bias { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICopyMachineTransition")]
	interface CICopyMachineTransitionProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect Extent { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("opacity")]
		float Opacity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICoreMLModel")]
	interface CICoreMLModelProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("model", ArgumentSemantic.Retain)]
		MLModel Model { get; set; }

		[Abstract]
		[Export ("headIndex")]
		float HeadIndex { get; set; }

		[Abstract]
		[Export ("softmaxNormalization")]
		bool SoftmaxNormalization { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CICrystallize")]
	interface CICrystallizeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the radius of the crystallization effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the center of the crystallization pattern.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDepthOfField")]
	interface CIDepthOfFieldProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the first endpoint of the line segment that represents the in-focus region.</summary>
		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		/// <summary>Gets or sets the second endpoint of the line segment that represents the in-focus region.</summary>
		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		/// <summary>Gets or sets a value that controls by how much to adjust the saturation of the in-focus region.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("saturation")]
		float Saturation { get; set; }

		/// <summary>Gets or sets the radius of the unsharp mask for the in-focus region.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("unsharpMaskRadius")]
		float UnsharpMaskRadius { get; set; }

		/// <summary>Gets or sets the intensity of the unsharp mask for the in-focus region.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("unsharpMaskIntensity")]
		float UnsharpMaskIntensity { get; set; }

		/// <summary>Gets or sets the unsharp mask radius for the out-of-focus region.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDepthToDisparity")]
	interface CIDepthToDisparityProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDiscBlur")]
	interface CIDiscBlurProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>The circular extent of the filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDisintegrateWithMaskTransition")]
	interface CIDisintegrateWithMaskTransitionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the mask to use for the transition.</summary>
		[Abstract]
		[NullAllowed, Export ("maskImage", ArgumentSemantic.Retain)]
		CIImage MaskImage { get; set; }

		/// <summary>Gets or set the shadow radius.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("shadowRadius")]
		float ShadowRadius { get; set; }

		/// <summary>Gets or sets the density of the mask shadows.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("shadowDensity")]
		float ShadowDensity { get; set; }

		/// <summary>Gets or sets the offset of the mask shadows.</summary>
		[Abstract]
		[Export ("shadowOffset", ArgumentSemantic.Assign)]
		CGPoint InputShadowOffset { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDisparityToDepth")]
	interface CIDisparityToDepthProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDissolveTransition")]
	interface CIDissolveTransitionProtocol : CIFilterProtocol {
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDither")]
	interface CIDitherProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDocumentEnhancer")]
	interface CIDocumentEnhancerProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIDotScreen")]
	interface CIDotScreenProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the angle, in degrees, of the dot screen.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIEdgePreserveUpsample")]
	interface CIEdgePreserveUpsampleProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("smallImage", ArgumentSemantic.Retain)]
		CIImage SmallImage { get; set; }

		[Abstract]
		[Export ("spatialSigma")]
		float SpatialSigma { get; set; }

		[Abstract]
		[Export ("lumaSigma")]
		float LumaSigma { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIEdges")]
	interface CIEdgesProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>The input intensity. Higher values detect more edges.</summary>
		/// <value>The default value is 1.0                                                                                                 .</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIEdgeWork")]
	interface CIEdgeWorkProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>The circular extent of the filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIEightfoldReflectedTile")]
	interface CIEightfoldReflectedTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIExposureAdjust")]
	interface CIExposureAdjustProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the desired exposure.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("EV")]
		float EV { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIFalseColor")]
	interface CIFalseColorProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the color that will be used for pixels of low luminance.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		/// <summary>Gets or sets the color that will be used for pixels of high luminance</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIFlashTransition")]
	interface CIFlashTransitionProtocol : CITransitionFilterProtocol {

		/// <summary>Gets or set the center of the flash.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets a vector of the form (x,y,w,h) that represents the rectangular extent of the flash.</summary>
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		/// <summary>Gets or sets the color of the flash.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		/// <summary>Gets or sets the maximum radius of the flash effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("maxStriationRadius")]
		float MaxStriationRadius { get; set; }

		/// <summary>Gets or sets the strength of the striations in the flash.</summary>
		[Abstract]
		[Export ("striationStrength")]
		float StriationStrength { get; set; }

		/// <summary>Gets or sets the striation contrast.</summary>
		[Abstract]
		[Export ("striationContrast")]
		float StriationContrast { get; set; }

		/// <summary>Gets or sets the threshold at which the flash starts or stops appearing.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("fadeThreshold")]
		float FadeThreshold { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIFourCoordinateGeometryFilter")]
	interface CIFourCoordinateGeometryFilterProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("topLeft", ArgumentSemantic.Assign)]
		CGPoint InputTopLeft { get; set; }

		[Abstract]
		[Export ("topRight", ArgumentSemantic.Assign)]
		CGPoint InputTopRight { get; set; }

		[Abstract]
		[Export ("bottomRight", ArgumentSemantic.Assign)]
		CGPoint InputBottomRight { get; set; }

		[Abstract]
		[Export ("bottomLeft", ArgumentSemantic.Assign)]
		CGPoint InputBottomLeft { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIFourfoldReflectedTile")]
	interface CIFourfoldReflectedTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		/// <summary>Gets or sets the skew, in degrees, of the tiles in the pattern.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("acuteAngle")]
		float AcuteAngle { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIFourfoldRotatedTile")]
	interface CIFourfoldRotatedTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIFourfoldTranslatedTile")]
	interface CIFourfoldTranslatedTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		/// <summary>Gets or sets the skew, in degrees, of the tiles in the pattern.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("acuteAngle")]
		float AcuteAngle { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGaborGradients")]
	interface CIGaborGradientsProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGammaAdjust")]
	interface CIGammaAdjustProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the exponent for the gamma curve.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("power")]
		float Power { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGaussianBlur")]
	interface CIGaussianBlurProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the radius of the blurring effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGaussianGradient")]
	interface CIGaussianGradientProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the center of the gradient.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the color at the center of the gradient.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		/// <summary>Gets or sets the color at the edge and beyond of the gradient.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }

		/// <summary>Gets or sets the radius of the gradient.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGlideReflectedTile")]
	interface CIGlideReflectedTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIGloom")]
	interface CIGloomProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the radius of highlights to darken.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the severity of the gloom.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIHatchedScreen")]
	interface CIHatchedScreenProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the angle of the hatch pattern.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIHeightFieldFromMask")]
	interface CIHeightFieldFromMaskProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>The circular extent of the filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIHexagonalPixellate")]
	interface CIHexagonalPixellateProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the center of the hexagonal pixel pattern.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the size of the individual pixel cells.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIHighlightShadowAdjust")]
	interface CIHighlightShadowAdjustProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>The circular extent of the filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets a value in the range [0,1] that controls how much to brighten shaded areas.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("shadowAmount")]
		float ShadowAmount { get; set; }

		/// <summary>Gets or sets a value in the range [0,1] that controls how much to dampen highlights.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("highlightAmount")]
		float HighlightAmount { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIHueAdjust")]
	interface CIHueAdjustProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>The amount, in degrees, by which to rotate the color cube about the neutral axis.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIHueSaturationValueGradient")]
	interface CIHueSaturationValueGradientProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("value")]
		float Value { get; set; }

		/// <summary>The circular extent of the filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("softness")]
		float Softness { get; set; }

		[Abstract]
		[Export ("dither")]
		float Dither { get; set; }

		[Abstract]
		[NullAllowed, Export ("colorSpace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIKaleidoscope")]
	interface CIKaleidoscopeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the number of sections in the kaleidoscope.</summary>
		[Abstract]
		[Export ("count")]
		nint InputCount { get; set; }

		/// <summary>Gets or sets the center of the kaleidoscope.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the kaleidoscope angle.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIKeystoneCorrectionCombined")]
	interface CIKeystoneCorrectionCombinedProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIKeystoneCorrectionHorizontal")]
	interface CIKeystoneCorrectionHorizontalProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIKeystoneCorrectionVertical")]
	interface CIKeystoneCorrectionVerticalProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILabDeltaE")]
	interface CILabDeltaEProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("image2", ArgumentSemantic.Retain)]
		CIImage Image2 { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILanczosScaleTransform")]
	interface CILanczosScaleTransformProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the size of the transformed image, relative to the source.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		/// <summary>Gets or sets the aspect ratio of the transformed image, relative to the source.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("aspectRatio")]
		float AspectRatio { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILenticularHaloGenerator")]
	interface CILenticularHaloGeneratorProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the center of the flare effect.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the color of the red, green, and blue halos in  the flare effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		/// <summary>Gets or sets the radius to the middle of the flare band.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("haloRadius")]
		float HaloRadius { get; set; }

		/// <summary>Gets or sets the distance between the inner and outer bands of the flare.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("haloWidth")]
		float HaloWidth { get; set; }

		/// <summary>Gets or sets a value that controls by how much the red, green, and blue halos overlap. 1 overlaps completely. The default is 0.77.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("haloOverlap")]
		float HaloOverlap { get; set; }

		/// <summary>Gets or sets the brightness of the striations of the flare.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("striationStrength")]
		float StriationStrength { get; set; }

		/// <summary>Gets or sets the contrast of the striations of the flare.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("striationContrast")]
		float StriationContrast { get; set; }

		/// <summary>Gets or sets a value that controls the shimmer of the flare over time. Default is 0.0.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("time")]
		float Time { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILinearGradient")]
	interface CILinearGradientProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the first point.</summary>
		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		/// <summary>Gets or sets the second point.</summary>
		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		/// <summary>Gets or sets the color at <see cref="CILinearGradient.InputPoint0" />.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		/// <summary>Gets or sets the color at <see cref="CILinearGradient.InputPoint1" />.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILinearToSRGBToneCurve")]
	interface CILinearToSrgbToneCurveProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILineOverlay")]
	interface CILineOverlayProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the input noise level to use when reducing noise before applying the line overlay filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("NRNoiseLevel")]
		float NRNoiseLevel { get; set; }

		/// <summary>Gets or sets the input sharpnsee to use when applying the line overlay filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("NRSharpness")]
		float NRSharpness { get; set; }

		/// <summary>Gets or sets the edge intensity to use when drawing the overlay.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("edgeIntensity")]
		float EdgeIntensity { get; set; }

		/// <summary>Gets or sets the line overlay threshold..</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("threshold")]
		float Threshold { get; set; }

		/// <summary>Gets or sets the contrast of the line overlay.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("contrast")]
		float Contrast { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CILineScreen")]
	interface CILineScreenProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the angle, in degrees, of the line pattern. 0 is vertical.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMaskedVariableBlur")]
	interface CIMaskedVariableBlurProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("mask", ArgumentSemantic.Retain)]
		CIImage Mask { get; set; }

		/// <summary>The circular extent of the filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMaskToAlpha")]
	interface CIMaskToAlphaProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMaximumComponent")]
	interface CIMaximumComponentProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMedian")]
	interface CIMedianProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMeshGenerator")]
	interface CIMeshGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("mesh", ArgumentSemantic.Retain)]
		CIVector [] Mesh { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMinimumComponent")]
	interface CIMinimumComponentProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMix")]
	interface CIMixProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Retain)]
		CIImage BackgroundImage { get; set; }

		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIModTransition")]
	interface CIModTransitionProtocol : CITransitionFilterProtocol {

		/// <summary>Gets or sets the center of the mod transition.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the angle of the mod transition.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		/// <summary>Gets or sets the radius of the mod transition.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the compression.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("compression")]
		float Compression { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMorphologyGradient")]
	interface CIMorphologyGradientProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMorphologyMaximum")]
	interface CIMorphologyMaximumProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMorphologyMinimum")]
	interface CIMorphologyMinimumProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMorphologyRectangleMaximum")]
	interface CIMorphologyRectangleMaximumProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("width")]
		float InputWidth { get; set; }

		[Abstract]
		[Export ("height")]
		float InputHeight { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMorphologyRectangleMinimum")]
	interface CIMorphologyRectangleMinimumProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("width")]
		float InputWidth { get; set; }

		[Abstract]
		[Export ("height")]
		float InputHeight { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIMotionBlur")]
	interface CIMotionBlurProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the angle of the motion blur.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CINoiseReduction")]
	interface CINoiseReductionProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the input noise level.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("noiseLevel")]
		float NoiseLevel { get; set; }

		/// <summary>Gets or sets the input sharpness.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIOpTile")]
	interface CIOpTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the scale factor for the tile effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPageCurlTransition")]
	interface CIPageCurlTransitionProtocol : CITransitionFilterProtocol {

		/// <summary>Gets or sets the image that appears on the back side of the peeled page.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("backsideImage", ArgumentSemantic.Retain)]
		CIImage BacksideImage { get; set; }

		/// <summary>Gets or sets the image to use for shading.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("shadingImage", ArgumentSemantic.Retain)]
		CIImage ShadingImage { get; set; }

		/// <summary>Gets or sets the extent of the target image to display.</summary>
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		/// <summary>Gets or sets the angle of the page transition.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		/// <summary>Gets or sets a value that controls the radius of the curl.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPageCurlWithShadowTransition")]
	interface CIPageCurlWithShadowTransitionProtocol : CITransitionFilterProtocol {

		/// <summary>Gets or sets the image that appears on the back side of the peeled page</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("backsideImage", ArgumentSemantic.Retain)]
		CIImage BacksideImage { get; set; }

		/// <summary>Gets or sets the extent of the target image to display.</summary>
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		/// <summary>Gets or sets the angle of the page transition.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		/// <summary>Gets or sets a value that controls the radius of the curl.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the size of the shadow effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("shadowSize")]
		float ShadowSize { get; set; }

		/// <summary>Gets or sets the darkness of the shadow effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("shadowAmount")]
		float ShadowAmount { get; set; }

		/// <summary>Gets or sets the extent of the target image to shade.</summary>
		[Abstract]
		[Export ("shadowExtent", ArgumentSemantic.Assign)]
		CGRect InputShadowExtent { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPaletteCentroid")]
	interface CIPaletteCentroidProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("paletteImage", ArgumentSemantic.Retain)]
		CIImage PaletteImage { get; set; }

		[Abstract]
		[Export ("perceptual")]
		bool Perceptual { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPalettize")]
	interface CIPalettizeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[NullAllowed, Export ("paletteImage", ArgumentSemantic.Retain)]
		CIImage PaletteImage { get; set; }

		[Abstract]
		[Export ("perceptual")]
		bool Perceptual { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIParallelogramTile")]
	interface CIParallelogramTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		/// <summary>Gets or sets the angle of the most acute corners of the parallelograms in the tile pattern.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("acuteAngle")]
		float AcuteAngle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPDF417BarcodeGenerator")]
	interface CIPdf417BarcodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		/// <summary>Gets or sets the minimum width of the data area, in pixels.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("minWidth")]
		float MinWidth { get; set; }

		/// <summary>Gets or sets the maximum width of the data area, in pixels.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("maxWidth")]
		float MaxWidth { get; set; }

		/// <summary>Gets or sets the minimum height of the data area, in pixels.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("minHeight")]
		float MinHeight { get; set; }

		/// <summary>Gets or sets the maximum height of the data area, in pixels.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("maxHeight")]
		float MaxHeight { get; set; }

		/// <summary>Gets or sets an integer value in the range [0,8] that controls how much error correction data to include in the code.</summary>
		[Abstract]
		[Export ("dataColumns")]
		float InputDataColumns { get; set; }

		/// <summary>Gets or sets the number of rows in the code. <c>0</c> causes the number of rows to be chosen based on the barcode extents.</summary>
		[Abstract]
		[Export ("rows")]
		float InputRows { get; set; }

		/// <summary>Gets or sets the preferred aspect ratio of the generated code.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("preferredAspectRatio")]
		float PreferredAspectRatio { get; set; }


		/// <summary>Gets or sets a value that controls how the data are compressed in the resulting code.</summary>
		/// <remarks>
		///   <c>0</c> indicates that the compression mode should be determined by the data type. <c>1</c> indicates that the data represent ASCII digits. <c>2</c> indicates that the data is ASCII text, numbers, and punctuation. <c>3</c> indicates that the data are in an unspecified format; The least compact compression scheme will be used.
		/// </remarks>
		[Abstract]
		[Export ("compactionMode")]
		float InputCompactionMode { get; set; }

		/// <summary>Gets or sets an integer with Boolean semantics that controls whether redundant elements should be omitted to save space. <c>1</c> is true. <c>0</c> is false.</summary>
		[Abstract]
		[Export ("compactStyle")]
		float InputCompactStyle { get; set; }

		/// <summary>Gets or sets an integer value in the range [0,8] that controls how much error correction data to include in the code.</summary>
		[Abstract]
		[Export ("correctionLevel")]
		float InputCorrectionLevel { get; set; }

		/// <summary>If <see langword="true" />, the barcode will contain compaction information, even if that information is redundant.</summary>
		[Abstract]
		[Export ("alwaysSpecifyCompaction")]
		float InputAlwaysSpecifyCompaction { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPerspectiveCorrection")]
	interface CIPerspectiveCorrectionProtocol : CIFourCoordinateGeometryFilterProtocol {

		[Abstract]
		[Export ("crop")]
		[MacCatalyst (13, 1)]
		bool Crop { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPerspectiveRotate")]
	interface CIPerspectiveRotateProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("focalLength")]
		float FocalLength { get; set; }

		[Abstract]
		[Export ("pitch")]
		float Pitch { get; set; }

		[Abstract]
		[Export ("yaw")]
		float Yaw { get; set; }

		[Abstract]
		[Export ("roll")]
		float Roll { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPerspectiveTile")]
	interface CIPerspectiveTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets location of the top left corner of the source image in the output.</summary>
		[Abstract]
		[Export ("topLeft", ArgumentSemantic.Assign)]
		CGPoint InputTopLeft { get; set; }

		/// <summary>Gets or sets location of the top right corner of the source image in the output.</summary>
		[Abstract]
		[Export ("topRight", ArgumentSemantic.Assign)]
		CGPoint InputTopRight { get; set; }

		/// <summary>Gets or sets location of the bottom right corner of the source image in the output.</summary>
		[Abstract]
		[Export ("bottomRight", ArgumentSemantic.Assign)]
		CGPoint InputBottomRight { get; set; }

		/// <summary>Gets or sets location of the bottom left corner of the source image in the output.</summary>
		[Abstract]
		[Export ("bottomLeft", ArgumentSemantic.Assign)]
		CGPoint InputBottomLeft { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPerspectiveTransform")]
	interface CIPerspectiveTransformProtocol : CIFourCoordinateGeometryFilterProtocol {
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPerspectiveTransformWithExtent")]
	interface CIPerspectiveTransformWithExtentProtocol : CIFourCoordinateGeometryFilterProtocol {

		/// <summary>Gets or sets the region in the source image to transform into the target image.</summary>
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPhotoEffect")]
	interface CIPhotoEffectProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPixellate")]
	interface CIPixellateProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the center of the pixellation.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the size of the pixels to create.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIPointillize")]
	interface CIPointillizeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the radius of the points in the pattern.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the center of the pointillization.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIQRCodeGenerator")]
	interface CIQRCodeGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		/// <summary>Gets or sets the error correction level.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("correctionLevel", ArgumentSemantic.Retain)]
		string CorrectionLevel { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIRadialGradient")]
	interface CIRadialGradientProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the center of the gradient.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the first radius of the gradient.</summary>
		/// <value>To be added.</value>
		/// <remarks>Application developers can set either of the two radii to be the larger. Swapping radii is equivalent to swapping colors.</remarks>
		[Abstract]
		[Export ("radius0")]
		float Radius0 { get; set; }

		/// <summary>Gets or sets the second radius of the gradient.</summary>
		/// <value>To be added.</value>
		/// <remarks>Application developers can set either of the two radii to be the larger. Swapping radii is equivalent to swapping colors.</remarks>
		[Abstract]
		[Export ("radius1")]
		float Radius1 { get; set; }

		/// <summary>Gets or sets the color at the <see cref="CIRadialGradient.Radius0" /> location in the gradient.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		/// <summary>Gets or sets the color at the <see cref="CIRadialGradient.Radius1" /> location in the gradient.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIRandomGenerator")]
	interface CIRandomGeneratorProtocol : CIFilterProtocol {
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIRippleTransition")]
	interface CIRippleTransitionProtocol : CITransitionFilterProtocol {

		/// <summary>Gets or sets the shading map to use for shading the transition effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("shadingImage", ArgumentSemantic.Retain)]
		CIImage ShadingImage { get; set; }

		/// <summary>Gets or sets the center of the transition.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the extent over which to apply the transition.</summary>
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		/// <summary>Gets or sets the width of the ripple wave.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		/// <summary>Gets or sets the intensity of the ripple effect. Default is 100</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIRoundedRectangleGenerator")]
	interface CIRoundedRectangleGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Abstract]
		[Export ("smoothness", ArgumentSemantic.Retain)]
		float Smoothness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISaliencyMap")]
	interface CISaliencyMapProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISepiaTone")]
	interface CISepiaToneProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets a value in the range (0,...) that controls the intensity of the sepia filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIShadedMaterial")]
	interface CIShadedMaterialProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the height field to use for shading.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[NullAllowed, Export ("shadingImage", ArgumentSemantic.Retain)]
		CIImage ShadingImage { get; set; }

		/// <summary>Gets or sets a multiplier for the height field.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISharpenLuminance")]
	interface CISharpenLuminanceProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the amount by which to sharpen.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }

		[Abstract]
		[Export ("radius")]
		[MacCatalyst (13, 1)]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISixfoldReflectedTile")]
	interface CISixfoldReflectedTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISixfoldRotatedTile")]
	interface CISixfoldRotatedTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISmoothLinearGradient")]
	interface CISmoothLinearGradientProtocol : CIFilterProtocol {
		/// <summary>The point associated with <see cref="CISmoothLinearGradient.Color0" /></summary>
		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		/// <summary>The point associated with <see cref="CISmoothLinearGradient.Color1" /></summary>
		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		/// <summary>The color associated with <see cref="CISmoothLinearGradient.InputPoint0" />.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		/// <summary>The color associated with <see cref="CISmoothLinearGradient.InputPoint1" /></summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISpotColor")]
	interface CISpotColorProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the center of the first color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("centerColor1", ArgumentSemantic.Retain)]
		CIColor CenterColor1 { get; set; }

		/// <summary>Gets or sets the first replacement color.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("replacementColor1", ArgumentSemantic.Retain)]
		CIColor ReplacementColor1 { get; set; }

		/// <summary>Gets or sets the width about the center of the first color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("closeness1")]
		float Closeness1 { get; set; }

		/// <summary>Gets or sets the contrast of the first color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("contrast1")]
		float Contrast1 { get; set; }

		/// <summary>Gets or sets the center of the second color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("centerColor2", ArgumentSemantic.Retain)]
		CIColor CenterColor2 { get; set; }

		/// <summary>Gets or sets the second replacement color.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("replacementColor2", ArgumentSemantic.Retain)]
		CIColor ReplacementColor2 { get; set; }

		/// <summary>Gets or sets the width about the center of the second color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("closeness2")]
		float Closeness2 { get; set; }

		/// <summary>Gets or sets the contrast of the second color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("contrast2")]
		float Contrast2 { get; set; }

		/// <summary>Gets or sets the center of the third color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("centerColor3", ArgumentSemantic.Retain)]
		CIColor CenterColor3 { get; set; }

		/// <summary>Gets or sets the third replacement color.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("replacementColor3", ArgumentSemantic.Retain)]
		CIColor ReplacementColor3 { get; set; }

		/// <summary>Gets or sets the width about the center of the third color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("closeness3")]
		float Closeness3 { get; set; }

		/// <summary>Gets or sets the contrast of the third color range.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("contrast3")]
		float Contrast3 { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISpotLight")]
	interface CISpotLightProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the 3-dimensional point in image coordinates from which the spotlight shines.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("lightPosition", ArgumentSemantic.Retain)]
		CIVector LightPosition { get; set; }

		/// <summary>Gets or sets the 3-dimensional point in image coordinates at which the spotlight points.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("lightPointsAt", ArgumentSemantic.Retain)]
		CIVector LightPointsAt { get; set; }

		/// <summary>Gets or sets the brightness of the spotlight.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("brightness")]
		float Brightness { get; set; }

		/// <summary>Gets or sets a value that controls how tightly the spotlight beam is focused.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("concentration")]
		float Concentration { get; set; }

		/// <summary>Gets or sets the color of the spotlight.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISRGBToneCurveToLinear")]
	interface CISrgbToneCurveToLinearProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIStarShineGenerator")]
	interface CIStarShineGeneratorProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the center of the star shine effect.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the color of the star shine filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		/// <summary>Gets or sets the radius of the entire flare.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the ratio of the cross spike lengths to the radius of the center.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("crossScale")]
		float CrossScale { get; set; }

		/// <summary>Gets or sets the angle that the cross of the star shine makes with the horizontal, in radians.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("crossAngle")]
		float CrossAngle { get; set; }

		/// <summary>Gets or sets a value that controls the thickness of the radial spikes of the star shine.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("crossOpacity")]
		float CrossOpacity { get; set; }

		/// <summary>Gets or sets the thickness of the radial spikes of the star shine.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("crossWidth")]
		float CrossWidth { get; set; }

		/// <summary>Gets or sets the epsilon value for the star shine generator.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("epsilon")]
		float Epsilon { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIStraighten")]
	interface CIStraightenProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the angle, in degrees, by which to rotate the image. (The image will be scaled to fit its original size.)</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIStripesGenerator")]
	interface CIStripesGeneratorProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the center of the stripes.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets first stripe color.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		/// <summary>Gets or sets the second stripe color.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }

		/// <summary>Gets or sets the width of the stripes.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		/// <summary>Gets or sets the sharpness of the stripe pattern. 1 is sharp. 0 is maximally blurry.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("sharpness")]
		float Sharpness { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISunbeamsGenerator")]
	interface CISunbeamsGeneratorProtocol : CIFilterProtocol {

		/// <summary>Gets or sets the center of the sunbeam effect.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the color of the sunbeam effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		/// <summary>Gets or sets the radius of the solid portion of the effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("sunRadius")]
		float SunRadius { get; set; }

		/// <summary>Gets or sets the maximum length of sunbeam rays.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("maxStriationRadius")]
		float MaxStriationRadius { get; set; }

		/// <summary>Gets or sets the intensity of the rays.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("striationStrength")]
		float StriationStrength { get; set; }

		/// <summary>Gets or sets the contrast of the rays.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("striationContrast")]
		float StriationContrast { get; set; }

		/// <summary>Gets or sets the time for the effect. Application developers can use this property to cause the effect to shimmer</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("time")]
		float Time { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CISwipeTransition")]
	interface CISwipeTransitionProtocol : CITransitionFilterProtocol {

		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[Export ("opacity")]
		float Opacity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CITemperatureAndTint")]
	interface CITemperatureAndTintProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>A vector that represents tne temperature and tint of the source image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("neutral", ArgumentSemantic.Retain)]
		CIVector Neutral { get; set; }

		/// <summary>A vector that represents tne temperature and tint of the target image.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("targetNeutral", ArgumentSemantic.Retain)]
		CIVector TargetNeutral { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CITextImageGenerator")]
	interface CITextImageGeneratorProtocol : CIFilterProtocol {

		[Abstract]
		[Export ("text", ArgumentSemantic.Retain)]
		string Text { get; set; }

		[Abstract]
		[Export ("fontName", ArgumentSemantic.Retain)]
		string FontName { get; set; }

		[Abstract]
		[Export ("fontSize")]
		float FontSize { get; set; }

		[Abstract]
		[Export ("scaleFactor")]
		float ScaleFactor { get; set; }

#if XAMCORE_5_0
		[Abstract]
#endif
		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("padding")]
		float Padding { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIThermal")]
	interface CIThermalProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIToneCurve")]
	interface CIToneCurveProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets tthe point in the zeroth index position through which to interpolate the tone curve.</summary>
		[Abstract]
		[Export ("point0", ArgumentSemantic.Assign)]
		CGPoint InputPoint0 { get; set; }

		/// <summary>Gets or sets the point in the first index position through which to interpolate the tone curve.</summary>
		[Abstract]
		[Export ("point1", ArgumentSemantic.Assign)]
		CGPoint InputPoint1 { get; set; }

		/// <summary>Gets or sets the point in the second index position through which to interpolate the tone curve.</summary>
		[Abstract]
		[Export ("point2", ArgumentSemantic.Assign)]
		CGPoint InputPoint2 { get; set; }

		/// <summary>Gets or sets the point in the third index position through which to interpolate the tone curve.</summary>
		[Abstract]
		[Export ("point3", ArgumentSemantic.Assign)]
		CGPoint InputPoint3 { get; set; }

		/// <summary>Gets or sets the point in the fourth index position through which to interpolate the tone curve.</summary>
		[Abstract]
		[Export ("point4", ArgumentSemantic.Assign)]
		CGPoint InputPoint4 { get; set; }

		[Abstract]
		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("extrapolate")]
		bool Extrapolate { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CITriangleKaleidoscope")]
	interface CITriangleKaleidoscopeProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the point in the source image about which to sample for the kaleidoscopic effect.</summary>
		[Abstract]
		[Export ("point", ArgumentSemantic.Assign)]
		CGPoint InputPoint { get; set; }

		/// <summary>Gets or sets the characteristic size of the region to sample.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("size")]
		float Size { get; set; }

		/// <summary>Gets or sets the rotation of the kaleidoscopic effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("rotation")]
		float Rotation { get; set; }

		/// <summary>Gets or sets the decay.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("decay")]
		float Decay { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CITriangleTile")]
	interface CITriangleTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CITwelvefoldReflectedTile")]
	interface CITwelvefoldReflectedTileProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		[Abstract]
		[Export ("angle")]
		float Angle { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIUnsharpMask")]
	interface CIUnsharpMaskProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the radius of the smallest feature to detect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the intensity of the enhanced contrast.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIVibrance")]
	interface CIVibranceProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets a value in the range [-1,1] that controls the vibrance filter.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIVignette")]
	interface CIVignetteProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the intensity of the vignette effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }

		/// <summary>Gets or sets the radius of the vignette effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIVignetteEffect")]
	interface CIVignetteEffectProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the point about which the image will be vignetted.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the radius of the area that will not be obscured.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		/// <summary>Gets or sets the intensity of the vignette.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("intensity")]
		float Intensity { get; set; }

		/// <summary>The rate of decay of the effect.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("falloff")]
		float Falloff { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIWhitePointAdjust")]
	interface CIWhitePointAdjustProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the new white point color.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIXRay")]
	interface CIXRayProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol (Name = "CIZoomBlur")]
	interface CIZoomBlurProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		/// <summary>Gets or sets the center of the effect.</summary>
		[Abstract]
		[Export ("center", ArgumentSemantic.Assign)]
		CGPoint InputCenter { get; set; }

		/// <summary>Gets or sets the amount of blur.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("amount")]
		float Amount { get; set; }
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIColorAbsoluteDifference")]
	interface CIColorAbsoluteDifferenceProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("inputImage2")]
		CIImage Image2 { get; set; }
	}

	[CoreImageFilter]
	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIColorAbsoluteDifference : CIColorAbsoluteDifferenceProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIColorThreshold")]
	interface CIColorThresholdProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("threshold")]
		float Threshold { get; set; }
	}

	[CoreImageFilter]
	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIColorThreshold : CIColorThresholdProtocol {
	}

	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol (Name = "CIColorThresholdOtsu")]
	interface CIColorThresholdOtsuProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	[CoreImageFilter]
	[iOS (14, 0)]
	[TV (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIColorThresholdOtsu : CIColorThresholdOtsuProtocol {
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB3X3 : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB5X5 : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB7X7 : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB9Horizontal : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIConvolutionRGB9Vertical : CIFilterProtocol {

		[CoreImageFilterProperty ("inputWeights")]
		CIVector Weights { get; set; }

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }

		[CoreImageFilterProperty ("inputBias")]
		float Bias { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CILinearLightBlendMode : CIFilterProtocol {

		[CoreImageFilterProperty ("inputBackgroundImage")]
		CIImage BackgroundImage { get; set; }

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIPersonSegmentation : CIPersonSegmentationProtocol {
	}

	[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
	[Protocol (Name = "CIPersonSegmentation")]
	interface CIPersonSegmentationProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("qualityLevel")]
		nuint QualityLevel { get; set; }
	}

	[CoreImageFilter]
	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIVividLightBlendMode : CIFilterProtocol {

		[CoreImageFilterProperty ("inputBackgroundImage")]
		CIImage BackgroundImage { get; set; }

		/// <summary>Gets or sets an image to filter.</summary>
		[CoreImageFilterProperty ("inputImage")]
		CIImage InputImage { get; set; }
	}

	[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol (Name = "CIAreaLogarithmicHistogram")]
	interface CIAreaLogarithmicHistogramProtocol : CIAreaReductionFilterProtocol {
		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("count")]
		nint Count { get; set; }

		[Abstract]
		[Export ("minimumStop")]
		float MinimumStop { get; set; }

		[Abstract]
		[Export ("maximumStop")]
		float MaximumStop { get; set; }
	}

	[CoreImageFilter]
	[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIAreaLogarithmicHistogram : CIAreaLogarithmicHistogramProtocol {
	}

	[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
	[Protocol (Name = "CIConvertLab")]
	interface CIConvertLabProtocol : CIFilterProtocol {
		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("normalize")]
		bool Normalize { get; set; }
	}

	[CoreImageFilter]
	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIBlurredRectangleGenerator : CIBlurredRectangleGeneratorProtocol {
	}

	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[Protocol (Name = "CIBlurredRectangleGenerator")]
	interface CIBlurredRectangleGeneratorProtocol : CIFilterProtocol {
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("sigma")]
		float Sigma { get; set; }

		[Abstract]
		[NullAllowed, Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[CoreImageFilter]
	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CIFilter))]
	interface CICannyEdgeDetector : CICannyEdgeDetectorProtocol {
	}

	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[Protocol (Name = "CICannyEdgeDetector")]
	interface CICannyEdgeDetectorProtocol : CIFilterProtocol {
		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("gaussianSigma")]
		float GaussianSigma { get; set; }

		[Abstract]
		[Export ("perceptual")]
		bool Perceptual { get; set; }

		[Abstract]
		[Export ("thresholdHigh")]
		float ThresholdHigh { get; set; }

		[Abstract]
		[Export ("thresholdLow")]
		float ThresholdLow { get; set; }

		[Abstract]
		[Export ("hysteresisPasses")]
		nint HysteresisPasses { get; set; }
	}

	[CoreImageFilter]
	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIRoundedRectangleStrokeGenerator : CIRoundedRectangleStrokeGeneratorProtocol {
	}

	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[Protocol (Name = "CIRoundedRectangleStrokeGenerator")]
	interface CIRoundedRectangleStrokeGeneratorProtocol : CIFilterProtocol {
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect InputExtent { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("width")]
		float Width { get; set; }

		[Abstract]
		[NullAllowed, Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }

		[iOS (26, 0), TV (26, 0), MacCatalyst (26, 0), Mac (26, 0)]
		[Abstract]
		[Export ("smoothness", ArgumentSemantic.Retain)]
		float Smoothness { get; set; }
	}

	[CoreImageFilter]
	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (CIFilter))]
	interface CISobelGradients : CISobelGradientsProtocol {
	}

	[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0)]
	[Protocol (Name = "CISobelGradients")]
	interface CISobelGradientsProtocol : CIFilterProtocol {

		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }
	}

	// LabToRGBFilter and RGBtoLabFilter are names for filter API but return the same type CIConvertLab
	// Enabling these cause a large number of introspection errors
	// https://github.com/dotnet/macios/issues/15662
#if false
	[CoreImageFilter]
	[iOS (16,0), TV (16,0), MacCatalyst (16,0)]
	[BaseType (typeof (CIFilter), Name = "CIConvertLab")]
	interface LabToRGBFilter : CIConvertLabProtocol
	{

	}

	[CoreImageFilter]
	[iOS (16,0), TV (16,0), MacCatalyst (16,0)]
	[BaseType (typeof (CIFilter), Name = "CIConvertLab")]
	interface RGBtoLabFilter : CIConvertLabProtocol
	{

	}
#endif

	[CoreImageFilter]
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIMaximumScaleTransform : CIMaximumScaleTransformProtocol {
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Protocol (Name = "CIMaximumScaleTransform", BackwardsCompatibleCodeGeneration = false)]
	interface CIMaximumScaleTransformProtocol : CIFilterProtocol {
		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("aspectRatio")]
		float AspectRatio { get; set; }
	}

	[CoreImageFilter]
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIToneMapHeadroom : CIToneMapHeadroomProtocol {
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Protocol (Name = "CIToneMapHeadroom", BackwardsCompatibleCodeGeneration = false)]
	interface CIToneMapHeadroomProtocol : CIFilterProtocol {
		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("sourceHeadroom")]
		float SourceHeadroom { get; set; }

		[Abstract]
		[Export ("targetHeadroom")]
		float TargetHeadroom { get; set; }
	}

	[CoreImageFilter]
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIAreaBoundsRed : CIAreaBoundsRedProtocol {
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Protocol (Name = "CIAreaBoundsRed")]
	interface CIAreaBoundsRedProtocol : CIAreaReductionFilterProtocol {
	}

	[TV (18, 1), Mac (15, 1), iOS (18, 1), MacCatalyst (18, 1)]
	[Protocol (Name = "CIDistanceGradientFromRedMask", BackwardsCompatibleCodeGeneration = false)]
	interface CIDistanceGradientFromRedMaskProtocol : CIFilterProtocol {
		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[Export ("inputImage")]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("maximumDistance")]
		int MaximumDistance { get; set; }
	}

	[CoreImageFilter]
	[TV (18, 1), Mac (15, 1), iOS (18, 1), MacCatalyst (18, 1)]
	[BaseType (typeof (CIFilter))]
	interface CIDistanceGradientFromRedMask : CIDistanceGradientFromRedMaskProtocol {
	}

	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Protocol (Name = "CISignedDistanceGradientFromRedMask", BackwardsCompatibleCodeGeneration = false)]
	interface CISignedDistanceGradientFromRedMaskProtocol : CIFilterProtocol {
		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[Export ("inputImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("maximumDistance")]
		float MaximumDistance { get; set; }
	}

	[CoreImageFilter]
	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[BaseType (typeof (CIFilter))]
	interface CISignedDistanceGradientFromRedMask : CISignedDistanceGradientFromRedMaskProtocol {
	}

	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Protocol (Name = "CISystemToneMap", BackwardsCompatibleCodeGeneration = false)]
	interface CISystemToneMapProtocol : CIFilterProtocol {
		/// <summary>Gets or sets an image to filter.</summary>
		[Abstract]
		[NullAllowed, Export ("inputImage", ArgumentSemantic.Retain)]
		CIImage InputImage { get; set; }

		[Abstract]
		[Export ("displayHeadroom")]
		float DisplayHeadroom { get; set; }

		[Abstract]
		[NullAllowed, Export ("preferredDynamicRange", ArgumentSemantic.Retain)]
		NSString WeakPreferredDynamicRange { get; set; }
	}

	[CoreImageFilter]
	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[BaseType (typeof (CIFilter))]
	interface CISystemToneMap : CISystemToneMapProtocol {
	}

	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Protocol (Name = "CIBlurredRoundedRectangleGenerator", BackwardsCompatibleCodeGeneration = false)]
	interface CIBlurredRoundedRectangleGeneratorProtocol : CIFilterProtocol {
		[Abstract]
		[Export ("extent", ArgumentSemantic.Assign)]
		CGRect Extent { get; set; }

		[Abstract]
		[Export ("radius")]
		float Radius { get; set; }

		[Abstract]
		[Export ("smoothness")]
		float Smoothness { get; set; }

		[Abstract]
		[Export ("sigma")]
		float Sigma { get; set; }

		[Abstract]
		[Export ("color", ArgumentSemantic.Retain)]
		CIColor Color { get; set; }
	}

	[CoreImageFilter]
	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIBlurredRoundedRectangleGenerator : CIBlurredRoundedRectangleGeneratorProtocol {
	}

	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Protocol (Name = "CIRoundedQRCodeGenerator", BackwardsCompatibleCodeGeneration = false)]
	interface CIRoundedQRCodeGeneratorProtocol : CIFilterProtocol {
		[Abstract]
		[Export ("message", ArgumentSemantic.Retain)]
		NSData Message { get; set; }

		[Abstract]
		[Export ("correctionLevel", ArgumentSemantic.Retain)]
		string CorrectionLevel { get; set; }

		[Abstract]
		[Export ("scale")]
		float Scale { get; set; }

		[Abstract]
		[Export ("roundedMarkers")]
		nint RoundedMarkers { get; set; }

		[Abstract]
		[Export ("roundedData")]
		bool RoundedData { get; set; }

		[Abstract]
		[Export ("centerSpaceSize")]
		float CenterSpaceSize { get; set; }

		[Abstract]
		[Export ("color0", ArgumentSemantic.Retain)]
		CIColor Color0 { get; set; }

		[Abstract]
		[Export ("color1", ArgumentSemantic.Retain)]
		CIColor Color1 { get; set; }
	}

	[CoreImageFilter]
	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIRoundedQRCodeGenerator : CIRoundedQRCodeGeneratorProtocol {
#if !XAMCORE_5_0
		[CoreImageFilterProperty ("outputCGImage")]
		CGImage OutputCGImage { get; }
#endif
	}

	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Protocol (Name = "CIAreaAverageMaximumRed", BackwardsCompatibleCodeGeneration = false)]
	interface CIAreaAverageMaximumRedProtocol : CIAreaReductionFilterProtocol {
	}

	[CoreImageFilter]
	[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[BaseType (typeof (CIFilter))]
	interface CIAreaAverageMaximumRed : CIAreaAverageMaximumRedProtocol {
	}

	#endregion

}
