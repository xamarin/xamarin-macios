//
// vImageTypes.cs
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2013-2014, Xamarin, Inc.
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

using System.Runtime.CompilerServices;

using Pixel8 = System.Byte;
using PixelF = System.Single;
using Pixel16U = System.UInt16;
using Pixel16S = System.Int16;
using ResamplingFilter = System.IntPtr;
using GammaFunction = System.IntPtr;

using vImagePixelCount = System.IntPtr;

#nullable enable

namespace Accelerate {
	// vImage_Buffer - vImage_Types.h
	/// <summary>Structure used to represent image data.</summary>
	///     <remarks>
	///       <para>This structure is used to describe a block of image data.   The image data is stored in the Data property with the Width and Height properties describing how many pixels the image has on each dimension.   </para>
	///       <para>
	///       </para>
	///       <para>The BytesPerRow property describes how many bytes are used on each row of pixels.   This is often referred to as the stride of the image.   It does not necessarily have to match the width in pixels, it can often be set to a different value to ensure that each image row starts in an aligned memory address (this is typically done to improve performance as CPUs are able to perform aligned fetches from memory faster than unaligned ones).</para>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct vImageBuffer {
		/// <summary>Points to the image data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public IntPtr Data { get; set; }

		// There is no way a row in the image will have more than 2^32 pixels
		// for their dimensions or row size.   If they ever do, we expose a
		// long property.
		// so expose these as ints as a convenience.
		vImagePixelCount HeightIntPtr;
		vImagePixelCount WidthIntPtr;
		nint RowBytesCountIntPtr; // size_t = nint

		/// <summary>Bytes per row in the image.   This is the stride of the row.   </summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int BytesPerRow {
			get { return (int) RowBytesCountIntPtr; }
			set { RowBytesCountIntPtr = value; }
		}

		/// <summary>The width of the image in pixels.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int Width {
			get { return (int) WidthIntPtr; }
			set { WidthIntPtr = (vImagePixelCount) value; }
		}

		/// <summary>The height of the image in pixels.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int Height {
			get { return (int) HeightIntPtr; }
			set { HeightIntPtr = (vImagePixelCount) value; }
		}
	}

	// vImage_AffineTransform - vImage_Types.h
	/// <summary>Struct that represents an affine transformation as a vector of six single-precision values.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct vImageAffineTransformFloat {
		// all defined as 'float'
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public float a;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public float b;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public float c;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public float d;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public float tx;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public float ty;

		// TODO: constructor from CGAffineTransform, vImageAffineTransformDouble
	}

	// vImage_AffineTransform_Double - vImage_Types.h
	/// <summary>Struct that represents an affine transformation as a vector of six double-precision values.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct vImageAffineTransformDouble {
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double a;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double b;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double c;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double d;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double tx;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double ty;

		// TODO: constructor from CGAffineTransform, vImageAffineTransformFloat
	}

	// vImage_Error (ssize_t) - vImageTypes.h
	/// <summary>Error codes returned by the various vImage manipulation APIs.</summary>
	///     <remarks>To be added.</remarks>
	[Native]
	public enum vImageError : long {
		/// <summary>To be added.</summary>
		NoError = 0,
		/// <summary>To be added.</summary>
		RoiLargerThanInputBuffer = -21766,
		/// <summary>To be added.</summary>
		InvalidKernelSize = -21767,
		/// <summary>To be added.</summary>
		InvalidEdgeStyle = -21768,
		/// <summary>To be added.</summary>
		InvalidOffsetX = -21769,
		/// <summary>To be added.</summary>
		InvalidOffsetY = -21770,
		/// <summary>To be added.</summary>
		MemoryAllocationError = -21771,
		/// <summary>To be added.</summary>
		NullPointerArgument = -21772,
		/// <summary>To be added.</summary>
		InvalidParameter = -21773,
		/// <summary>To be added.</summary>
		BufferSizeMismatch = -21774,
		/// <summary>To be added.</summary>
		UnknownFlagsBit = -21775,
		/// <summary>To be added.</summary>
		InternalError = -21776,
		/// <summary>To be added.</summary>
		InvalidRowBytes = -21777,
		/// <summary>To be added.</summary>
		InvalidImageFormat = -21778,
		/// <summary>To be added.</summary>
		ColorSyncIsAbsent = -21779,
		/// <summary>To be added.</summary>
		OutOfPlaceOperationRequired = -21780,
	}

	// anonymous enum - Transform.h
	/// <summary>Enumerates algorithms for gamma correction.</summary>
	///     <remarks>To be added.</remarks>
	public enum vImageGamma {
		/// <summary>To be added.</summary>
		kUseGammaValue = 0,
		/// <summary>To be added.</summary>
		kUseGammaValueHalfPrecision = 1,
		/// <summary>To be added.</summary>
		k5over9_HalfPrecision = 2,
		/// <summary>To be added.</summary>
		k9over5_HalfPrecision = 3,
		/// <summary>To be added.</summary>
		k5over11_HalfPrecision = 4,
		/// <summary>To be added.</summary>
		k11ove_5_HalfPrecision = 5,
		/// <summary>To be added.</summary>
		ksRGB_ForwardHalfPrecision = 6,
		/// <summary>To be added.</summary>
		ksRGB_ReverseHalfPrecision = 7,
		/// <summary>To be added.</summary>
		k11over9_HalfPrecision = 8,
		/// <summary>To be added.</summary>
		k9over11_HalfPrecision = 9,
		/// <summary>To be added.</summary>
		kBT709_ForwardHalfPrecision = 10,
		/// <summary>To be added.</summary>
		kBT709_ReverseHalfPrecision = 11,
	};

	// vImageMDTableUsageHint (untyped) - Transform.h
	/// <summary>Enumerates hints for using a multi-dimensional table.</summary>
	///     <remarks>To be added.</remarks>
	public enum vImageMDTableUsageHint : int {
		/// <summary>To be added.</summary>
		k16Q12 = 1,
		/// <summary>To be added.</summary>
		kFloat = 2,
	}

	// vImage_InterpolationMethod (untyped) - Transform.h
	/// <summary>Enumerates algorithms for image interpolation.</summary>
	///     <remarks>To be added.</remarks>
	[NativeName ("vImage_InterpolationMethod")]
	public enum vImageInterpolationMethod : int {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Full = 1,
		/// <summary>To be added.</summary>
		Half = 2,
	}

	/// <summary>Enumerates options for processing images.</summary>
	///     <remarks>To be added.</remarks>
	[Flags]
	// vImage_Flags (uint32_t) - vImage_Types.h
	public enum vImageFlags : uint {
		/// <summary>To be added.</summary>
		NoFlags = 0,
		/// <summary>To be added.</summary>
		LeaveAlphaUnchanged = 1,
		/// <summary>To be added.</summary>
		CopyInPlace = 2,
		/// <summary>To be added.</summary>
		BackgroundColorFill = 4,
		/// <summary>To be added.</summary>
		EdgeExtend = 8,
		/// <summary>To be added.</summary>
		DoNotTile = 16,
		/// <summary>To be added.</summary>
		HighQualityResampling = 32,
		/// <summary>To be added.</summary>
		TruncateKernel = 64,
		/// <summary>To be added.</summary>
		GetTempBufferSize = 128,
		/// <summary>To be added.</summary>
		PrintDiagnosticsToConsole = 256,
		/// <summary>To be added.</summary>
		NoAllocate = 512,
	}

	/// <summary>Represents a pixel using 32-bit floating points values for its alpha, red, green and blue components.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct PixelFFFF {
		/// <summary>Alpha channel component.</summary>
		///         <remarks>
		///         </remarks>
		public float A;
		/// <summary>Red channel component.</summary>
		///         <remarks>
		///         </remarks>
		public float R;
		/// <summary>Green channel component.</summary>
		///         <remarks>
		///         </remarks>
		public float G;
		/// <summary>Blue channel component.</summary>
		///         <remarks>
		///         </remarks>
		public float B;
		/// <summary>Pixel with all the values set to zero.</summary>
		///         <remarks>To be added.</remarks>
		public readonly static PixelFFFF Zero;
	}

	/// <summary>Represents a pixel using 8-bit integers for its red, green, blue and alpha components.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct Pixel8888 {
		/// <summary>Alpha channel component.</summary>
		///         <remarks>
		///         </remarks>
		public byte A;
		/// <summary>Red channel component.</summary>
		///         <remarks>
		///         </remarks>
		public byte R;
		/// <summary>Green channel component.</summary>
		///         <remarks>
		///         </remarks>
		public byte G;
		/// <summary>Blue channel component.</summary>
		///         <remarks>
		///         </remarks>
		public byte B;
		/// <summary>Pixerl with all the values set to zero.</summary>
		///         <remarks>
		///         </remarks>
		public readonly static Pixel8888 Zero;
	}

	/// <summary>Represents a pixel using 16-bit unsigned integers for its alpha, red, green and blue components.</summary>
	///     <remarks>
	///       <para>
	///       </para>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct PixelARGB16U {
		/// <summary>Alpha Component.</summary>
		///         <remarks>To be added.</remarks>
		public Pixel16U A;
		/// <summary>Red Component.</summary>
		///         <remarks>To be added.</remarks>
		public Pixel16U R;
		/// <summary>Green Component.</summary>
		///         <remarks>To be added.</remarks>
		public Pixel16U G;
		/// <summary>Blue Component.</summary>
		///         <remarks>To be added.</remarks>
		public Pixel16U B;
		/// <summary>Pixel with all the values set to zero.</summary>
		///         <remarks>To be added.</remarks>
		public readonly static PixelARGB16U Zero;
	}

	/// <summary>Represents a pixel using 16-bit signed integers for its alpha, red, green and blue components.</summary>
	///     <remarks>
	///       <para>
	///       </para>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct PixelARGB16S {
		/// <summary>Alpha Component.</summary>
		///         <remarks>To be added.</remarks>
		public Pixel16S A;
		/// <summary>Red Component.</summary>
		///         <remarks>To be added.</remarks>
		public Pixel16S R;
		/// <summary>Green Component.</summary>
		///         <remarks>To be added.</remarks>
		public Pixel16S G;
		/// <summary>Blue Component.</summary>
		///         <remarks>To be added.</remarks>
		public Pixel16S B;
		/// <summary>Pixel with all the values set to zero.</summary>
		///         <remarks>To be added.</remarks>
		public readonly static PixelARGB16S Zero;
	}

	/// <summary>Accelerated image operations.</summary>
	///     <remarks>
	///       <para>
	/// 	The vImage class provides a collection of methods that operate on images represented by the <see cref="Accelerate.vImageBuffer" /> structure.</para>
	///       <para>
	///       </para>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	unsafe public static class vImage {

		#region Convolve

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolve_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, Pixel8 backgroundColor, vImageFlags flags);
		public static vImageError ConvolvePlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, Pixel8 backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageConvolve_Planar8 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, divisor, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolve_PlanarF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, PixelF backgroundColor, vImageFlags flags);
		public static vImageError ConvolvePlanarF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, PixelF backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageConvolve_PlanarF (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, Pixel8888* backgroundColor, vImageFlags flags);
		public static vImageError ConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, Pixel8888* backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageConvolve_ARGB8888 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, divisor, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolve_ARGBFFFF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, PixelFFFF backgroundColor, vImageFlags flags);
		public static vImageError ConvolveARGBFFFF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, PixelFFFF backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageConvolve_ARGBFFFF (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolveWithBias_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, int bias, Pixel8 backgroundColor, vImageFlags flags);
		public static vImageError ConvolveWithBiasPlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, int bias, Pixel8 backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageConvolveWithBias_Planar8 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_width, kernel_height, divisor, bias, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolveWithBias_PlanarF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, /* defined as float*/ float* kernel, uint kernel_height, uint kernel_width, float bias, PixelF backgroundColor, vImageFlags flags);
		public static vImageError ConvolveWithBiasPlanarF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, float bias, PixelF backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageConvolveWithBias_PlanarF (
				srcPtr,
				destPtr,
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, bias, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary, EntryPoint = "vImageConvolveWithBias_ARGB8888")]
		extern static nint vImageConvolveWithBias_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, int bias, Pixel8888 backgroundColor, vImageFlags flags);
		public static vImageError ConvolveWithBiasARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, int bias, Pixel8888 backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageConvolveWithBias_ARGB8888 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, divisor, bias, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary, EntryPoint = "vImageConvolveWithBias_ARGBFFFF")]
		extern static nint vImageConvolveWithBias_ARGBFFFF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, float bias, PixelFFFF backgroundColor, vImageFlags flags);
		public static vImageError ConvolveWithBiasARGBFFFF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, float bias, PixelFFFF backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageConvolveWithBias_ARGBFFFF (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, bias, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolveMultiKernel_ARGB8888 (vImageBuffer* src,
										 vImageBuffer* dest,
										 IntPtr tempBuffer,
										 vImagePixelCount srcOffsetToROI_X,
										 vImagePixelCount srcOffsetToROI_Y,
										 short* [] kernels,         // must be 4
										 uint kernel_height,
										 uint kernel_width,
										 int [] divisors,       // must be 4
										 int [] biases,     // must be 4
										 Pixel8888 backgroundColor,
										 vImageFlags flags);

		public static vImageError ConvolveMultiKernelARGB8888 (ref vImageBuffer src,
									ref vImageBuffer dest,
									IntPtr tempBuffer,
									vImagePixelCount srcOffsetToROI_X,
									vImagePixelCount srcOffsetToROI_Y,
									short [] [] kernels,        // must be 4
									uint kernel_height,
									uint kernel_width,
									int [] divisors,        // must be 4
									int [] biases,      // must be 4
									Pixel8888 backgroundColor,
									vImageFlags flags)
		{
			if (kernels is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (kernels));
			if (divisors is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (divisors));
			if (biases is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (biases));
			if (kernels.Length < 4)
				throw new ArgumentException ("Must contain at least four elements", "kernels");
			if (divisors.Length < 4)
				throw new ArgumentException ("Must contain at least four elements", "divisors");
			if (biases.Length < 4)
				throw new ArgumentException ("Must contain at least four elements", "biases");
			unsafe {
				fixed (short* f1 = kernels [0]) {
					fixed (short* f2 = kernels [1]) {
						fixed (short* f3 = kernels [2]) {
							fixed (short* f4 = kernels [3]) {
								var ptrs = new short* [4];
								ptrs [0] = f1;
								ptrs [1] = f2;
								ptrs [2] = f3;
								ptrs [3] = f4;
								fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
									return (vImageError) (long) vImageConvolveMultiKernel_ARGB8888 (
										srcPtr,
										destPtr,
										tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, ptrs, kernel_height, kernel_width, divisors, biases, backgroundColor, flags);
								}
							}
						}
					}
				}
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		// Convolution.h
		extern static nint vImageConvolveMultiKernel_ARGBFFFF (vImageBuffer* src,
										 vImageBuffer* dest,
										 IntPtr tempBuffer,
										 vImagePixelCount srcOffsetToROI_X,
										 vImagePixelCount srcOffsetToROI_Y,
										 float* [] kernels,   //must be 4, defined as 'float*[4]'
										 uint kernel_height,
										 uint kernel_width,
										 float [] biases,    // must be 4
										 PixelFFFF backgroundColor,
										 vImageFlags flags);

		public static vImageError ConvolveMultiKernelARGBFFFF (ref vImageBuffer src,
									   ref vImageBuffer dest,
									   IntPtr tempBuffer,
									   vImagePixelCount srcOffsetToROI_X,
									   vImagePixelCount srcOffsetToROI_Y,
									   float [] [] kernels,   //must be 4
									   uint kernel_height,
									   uint kernel_width,
									   float [] biases,    // must be 4
									   PixelFFFF backgroundColor,
									   vImageFlags flags)
		{
			if (kernels is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (kernels));
			if (biases is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (biases));
			if (kernels.Length < 4)
				throw new ArgumentException ("Must contain at least four elements", "kernels");
			if (biases.Length < 4)
				throw new ArgumentException ("Must contain at least four elements", "biases");
			unsafe {
				fixed (float* f1 = kernels [0]) {
					fixed (float* f2 = kernels [1]) {
						fixed (float* f3 = kernels [2]) {
							fixed (float* f4 = kernels [3]) {
								var ptrs = new float* [4];
								ptrs [0] = f1;
								ptrs [1] = f2;
								ptrs [2] = f3;
								ptrs [3] = f4;
								fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
									return (vImageError) (long) vImageConvolveMultiKernel_ARGBFFFF (
										srcPtr,
										destPtr,
										tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, ptrs, kernel_height, kernel_width, biases, backgroundColor, flags);
								}
							}
						}
					}
				}
			}
		}


		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageRichardsonLucyDeConvolve_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, short* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, int divisor, int divisor2, Pixel8 backgroundColor, uint iterationCount, vImageFlags flags);
		public static vImageError RichardsonLucyDeConvolvePlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, short* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, int divisor, int divisor2, Pixel8 backgroundColor, uint iterationCount, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageRichardsonLucyDeConvolve_Planar8 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel2, kernel_height, kernel_width, kernel_height2, kernel_width2, divisor, divisor2, backgroundColor, iterationCount, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageRichardsonLucyDeConvolve_PlanarF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, float* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, PixelF backgroundColor, uint iterationCount, vImageFlags flags);
		public static vImageError RichardsonLucyDeConvolvePlanarF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, float* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, PixelF backgroundColor, uint iterationCount, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageRichardsonLucyDeConvolve_PlanarF (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel2, kernel_height, kernel_width, kernel_height2, kernel_width2, backgroundColor, iterationCount, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageRichardsonLucyDeConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, short* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, int divisor, int divisor2, Pixel8888 backgroundColor, uint iterationCount, vImageFlags flags);
		public static vImageError RichardsonLucyDeConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, short* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, int divisor, int divisor2, Pixel8888 backgroundColor, uint iterationCount, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageRichardsonLucyDeConvolve_ARGB8888 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel2, kernel_height, kernel_width, kernel_height2, kernel_width2, divisor, divisor2, backgroundColor, iterationCount, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageRichardsonLucyDeConvolve_ARGBFFFF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, float* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, PixelFFFF backgroundColor, uint iterationCount, vImageFlags flags);
		public static vImageError RichardsonLucyDeConvolveARGBFFFF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, float* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, PixelFFFF backgroundColor, uint iterationCount, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageRichardsonLucyDeConvolve_ARGBFFFF (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel2, kernel_height, kernel_width, kernel_height2, kernel_width2, backgroundColor, iterationCount, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageBoxConvolve_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8 backgroundColor, vImageFlags flags);
		public static vImageError BoxConvolvePlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8 backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageBoxConvolve_Planar8 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageBoxConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888 backgroundColor, vImageFlags flags);
		public static vImageError BoxConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888 backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageBoxConvolve_ARGB8888 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageBoxConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888* backgroundColor, vImageFlags flags);
		public static vImageError BoxConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888* backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageBoxConvolve_ARGB8888 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageTentConvolve_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8 backgroundColor, vImageFlags flags);
		public static vImageError TentConvolvePlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8 backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageTentConvolve_Planar8 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
			}
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageTentConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888 backgroundColor, vImageFlags flags);
		public static vImageError TentConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888 backgroundColor, vImageFlags flags)
		{
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageTentConvolve_ARGB8888 (
					srcPtr,
					destPtr,
					tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
			}
		}

		#endregion

		#region Transform
		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageMatrixMultiply_ARGB8888 (vImageBuffer* src,
									vImageBuffer* dest,
									short [] matrix, // matrix is [4*4],
									int divisor,
									short []? pre_bias,      //Must be an array of 4 int16_t's. NULL is okay.
									int []? post_bias,     //Must be an array of 4 int32_t's. NULL is okay.
									vImageFlags flags);

		/// <param name="src">Source image data..</param>
		///         <param name="dest">Target image data.</param>
		///         <param name="matrix">To be added.</param>
		///         <param name="divisor">To be added.</param>
		///         <param name="pre_bias">To be added.</param>
		///         <param name="post_bias">To be added.</param>
		///         <param name="flags">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static vImageError MatrixMultiplyARGB8888 (ref vImageBuffer src,
								   ref vImageBuffer dest,
								   short [] matrix, // matrix is [4*4],
								   int divisor,
								   short [] pre_bias,      //Must be an array of 4 int16_t's. NULL is okay. 
								   int [] post_bias,     //Must be an array of 4 int32_t's. NULL is okay. 
								   vImageFlags flags)
		{
			if (matrix is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (matrix));
			if (pre_bias is not null && pre_bias.Length != 4)
				throw new ArgumentException ("Must have four elements", nameof (pre_bias));
			if (post_bias is not null && post_bias.Length != 4)
				throw new ArgumentException ("Must have four elements", nameof (post_bias));
			fixed (vImageBuffer* srcPtr = &src, destPtr = &dest) {
				return (vImageError) (long) vImageMatrixMultiply_ARGB8888 (
					srcPtr,
					destPtr,
					matrix, divisor, pre_bias, post_bias, flags);
			}
		}

		//vImage_Error vImageMatrixMultiply_Planar16S( __const vImage_Buffer *srcs[],
		//                                             __const vImage_Buffer *dests[],    
		//                                             uint32_t                 src_planes,
		//                                             uint32_t                 dest_planes,
		//                                             __const int16_t              matrix[],
		//                                             int32_t              divisor,
		//                                             __const int16_t         *pre_bias, 
		//                                             __const int32_t         *post_bias,
		//                                             vImage_Flags             flags )
		//                                                                                 __attribute__ ((nonnull(1,2,5)))
		//                                                                                 __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//    
		//# 263 "/fx/Accelerate.framework/Frameworks/vImage.framework/Headers/Transform.h"
		//vImage_Error vImageMatrixMultiply_Planar8(          __const vImage_Buffer *srcs[],      //A set of src_planes as a __const array of pointers to vImage_Buffer structs that reference vImage_Buffers.
		//                                                    __const vImage_Buffer *dests[],     //A set of src_planes as a __const array of pointers to vImage_Buffer structs that reference vImage_Buffers.
		//                                                    uint32_t            src_planes,
		//                                                    uint32_t            dest_planes,
		//                                                    __const int16_t     matrix[],
		//                                                    int32_t             divisor,
		//                                                    __const int16_t     *pre_bias,      //A packed array of src_plane int16_t values. ((void*)0) is okay
		//                                                    __const int32_t     *post_bias,     //A packed array of dest_plane int32_t values. ((void*)0) is okay
		//                                                    vImage_Flags        flags ) __attribute__ ((nonnull(1,2,5))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//vImage_Error vImageMatrixMultiply_PlanarF(          __const vImage_Buffer *srcs[],        //A set of src_planes as a __const array of pointers to vImage_Buffer structs that reference vImage_Buffers.
		//                                                    __const vImage_Buffer *dests[],       //A set of src_planes as a __const array of pointers to vImage_Buffer structs that reference vImage_Buffers.
		//                                                    uint32_t            src_planes,
		//                                                    uint32_t            dest_planes,
		//                                                    __const float               matrix[],               
		//                                                    __const float       *pre_bias,      //A packed array of float values. ((void*)0) is okay
		//                                                    __const float       *post_bias,     //A packed array of float values. ((void*)0) is okay
		//                                                    vImage_Flags flags ) __attribute__ ((nonnull(1,2,5))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//vImage_Error vImageMatrixMultiply_ARGBFFFF(         __const vImage_Buffer *src,
		//                                                    __const vImage_Buffer *dest,
		//                                                    __const float               matrix[4*4],
		//                                                    __const float               *pre_bias,      //Must be an array of 4 floats. ((void*)0) is okay. 
		//                                                    __const float               *post_bias,     //Must be an array of 4 floats. ((void*)0) is okay. 
		//                                                    vImage_Flags        flags ) __attribute__ ((nonnull(1,2,3))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//
		//GammaFunction   vImageCreateGammaFunction(          float           gamma,
		//                                                    int             gamma_type,
		//                                                    vImage_Flags    flags )             __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//void            vImageDestroyGammaFunction( GammaFunction f )                           __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//vImage_Error    vImageGamma_Planar8toPlanarF(       __const vImage_Buffer *src,
		//                                                    __const vImage_Buffer *dest,           
		//                                                    __const GammaFunction gamma,
		//                                                    vImage_Flags        flags ) __attribute__ ((nonnull(1,2))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//                                                    
		//vImage_Error    vImageGamma_PlanarFtoPlanar8(       __const vImage_Buffer *src,           
		//                                                    __const vImage_Buffer *dest,          
		//                                                    __const GammaFunction gamma,
		//                                                    vImage_Flags        flags ) __attribute__ ((nonnull(1,2))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//                                                    
		//vImage_Error    vImageGamma_PlanarF(                __const vImage_Buffer *src,           
		//                                                    __const vImage_Buffer *dest,          
		//                                                    __const GammaFunction gamma,
		//                                                    vImage_Flags        flags ) __attribute__ ((nonnull(1,2))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//                                        
		//vImage_Error vImagePiecewiseGamma_Planar8(__const vImage_Buffer *src,
		//                                          __const vImage_Buffer *dest,
		//                                          __const float         exponentialCoeffs[3],
		//                                          __const float         gamma,
		//                                          __const float         linearCoeffs[2],
		//                                          __const Pixel_8       boundary,
		//                                          vImage_Flags        flags) __attribute__ ((nonnull(1,2,3,5))) __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//    
		//vImage_Error vImagePiecewiseGamma_Planar8toPlanar16Q12(__const vImage_Buffer *src,
		//                                                       __const vImage_Buffer *dest,
		//                                                       __const float         exponentialCoeffs[3],
		//                                                       __const float         gamma,
		//                                                       __const float         linearCoeffs[2],
		//                                                       __const Pixel_8       boundary,
		//                                                       vImage_Flags        flags) __attribute__ ((nonnull(1,2,3,5))) __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//    
		//vImage_Error vImagePiecewiseGamma_Planar16Q12(__const vImage_Buffer *src,
		//                                              __const vImage_Buffer *dest,
		//                                              __const float         exponentialCoeffs[3],
		//                                              __const float         gamma,
		//                                              __const float         linearCoeffs[2],
		//                                              __const Pixel_16S     boundary,
		//                                              vImage_Flags        flags) __attribute__ ((nonnull(1,2,3,5))) __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//    
		//vImage_Error vImagePiecewiseGamma_Planar16Q12toPlanar8(__const vImage_Buffer *src,
		//                                                       __const vImage_Buffer *dest,
		//                                                       __const float         exponentialCoeffs[3],
		//                                                       __const float         gamma,
		//                                                       __const float         linearCoeffs[2],
		//                                                       __const Pixel_16S     boundary,
		//                                                       vImage_Flags        flags) __attribute__ ((nonnull(1,2,3,5))) __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//    
		//vImage_Error vImagePiecewiseGamma_Planar8toPlanarF(__const vImage_Buffer *src,
		//                                                   __const vImage_Buffer *dest,
		//                                                   __const float         exponentialCoeffs[3],
		//                                                   __const float         gamma,
		//                                                   __const float         linearCoeffs[2],
		//                                                   __const Pixel_8       boundary,
		//                                                   vImage_Flags        flags) __attribute__ ((nonnull(1,2,3,5))) __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//vImage_Error vImagePiecewiseGamma_PlanarF(__const vImage_Buffer *src,
		//                                          __const vImage_Buffer *dest,
		//                                          __const float         exponentialCoeffs[3],
		//                                          __const float         gamma,
		//                                          __const float         linearCoeffs[2],
		//                                          __const float         boundary,
		//                                          vImage_Flags        flags) __attribute__ ((nonnull(1,2,3,5))) __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//    
		//vImage_Error vImagePiecewiseGamma_PlanarFtoPlanar8(__const vImage_Buffer *src,
		//                                                   __const vImage_Buffer *dest,
		//                                                   __const float         exponentialCoeffs[3],
		//                                                   __const float         gamma,
		//                                                   __const float         linearCoeffs[2],
		//                                                   __const float         boundary,
		//                                                   vImage_Flags        flags) __attribute__ ((nonnull(1,2,3,5))) __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//# 661 "/fx/Accelerate.framework/Frameworks/vImage.framework/Headers/Transform.h"
		//                                                                                
		//vImage_Error    vImagePiecewisePolynomial_PlanarF(  __const vImage_Buffer *src,       //floating point data
		//                                                    __const vImage_Buffer *dest,       //floating point data
		//                                                    __const float         **coefficients,
		//                                                    __const float         *boundaries,
		//                                                    uint32_t            order,
		//                                                    uint32_t            log2segments,
		//                                                    vImage_Flags        flags ) __attribute__ ((nonnull(1,2,3,4))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//vImage_Error    vImagePiecewisePolynomial_Planar8toPlanarF( __const vImage_Buffer *src,        //8-bit data
		//                                                            __const vImage_Buffer *dest,       //floating point data
		//                                                            __const float         **coefficients,
		//                                                            __const float         *boundaries,
		//                                                            uint32_t            order,
		//                                                            uint32_t            log2segments,
		//                                                            vImage_Flags        flags ) __attribute__ ((nonnull(1,2,3,4))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//vImage_Error    vImagePiecewisePolynomial_PlanarFtoPlanar8( __const vImage_Buffer *src,       //floating point data
		//                                                            __const vImage_Buffer *dest,      //8-bit data
		//                                                            __const float         **coefficients,
		//                                                            __const float         *boundaries,  
		//                                                            uint32_t            order,
		//                                                            uint32_t            log2segments,
		//                                                            vImage_Flags        flags ) __attribute__ ((nonnull(1,2,3,4))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//# 739 "/fx/Accelerate.framework/Frameworks/vImage.framework/Headers/Transform.h"
		//vImage_Error    vImagePiecewiseRational_PlanarF(  __const vImage_Buffer *src,         //floating point data
		//                                                    __const vImage_Buffer *dest,       //floating point data
		//                                                    __const float         **topCoefficients,
		//                                                    __const float         **bottomCoefficients,
		//                                                    __const float         *boundaries,
		//                                                    uint32_t            topOrder,
		//                                                    uint32_t            bottomOrder,
		//                                                    uint32_t            log2segments,
		//                                                    vImage_Flags        flags ) __attribute__ ((nonnull(1,2,3,4,5))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//    
		//# 764 "/fx/Accelerate.framework/Frameworks/vImage.framework/Headers/Transform.h"
		//    
		//vImage_Error vImageLookupTable_Planar8toPlanar16(__const vImage_Buffer *src,
		//                                                 __const vImage_Buffer *dest,
		//                                                 __const Pixel_16U      table[256],
		//                                                 vImage_Flags         flags)
		//    __attribute__ ((nonnull(1,2,3))) __OSX_AVAILABLE_STARTING(1090, 70000);
		//    
		//# 787 "/fx/Accelerate.framework/Frameworks/vImage.framework/Headers/Transform.h"
		//    
		//vImage_Error vImageLookupTable_Planar8toPlanarF(__const vImage_Buffer *src,
		//                                                __const vImage_Buffer *dest,
		//                                                __const Pixel_F        table[256],
		//                                                vImage_Flags         flags)
		//vImage_Error
		//vImageLookupTable_PlanarFtoPlanar8(
		//    __const vImage_Buffer *src,          
		//    __const vImage_Buffer *dest,         
		//    __const Pixel_8       table[4096],
		//    vImage_Flags        flags )
		//    __attribute__ ((nonnull(1,2,3))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//# 851 "/fx/Accelerate.framework/Frameworks/vImage.framework/Headers/Transform.h"
		//vImage_Error
		//vImageLookupTable_8to64U(
		//    __const vImage_Buffer *src,
		//    __const vImage_Buffer *dest,
		//    __const uint64_t      LUT[256],
		//    vImage_Flags flags)
		//    __attribute__ ((nonnull(1,2,3)))   __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//# 897 "/fx/Accelerate.framework/Frameworks/vImage.framework/Headers/Transform.h"
		//vImage_Error    vImageInterpolatedLookupTable_PlanarF(  __const vImage_Buffer *src,
		//                                                        __const vImage_Buffer *dest,
		//                                                        __const Pixel_F       *table,
		//                                                        vImagePixelCount    tableEntries,
		//                                                        float               maxFloat,
		//                                                        float               minFloat,
		//                                                        vImage_Flags        flags ) __attribute__ ((nonnull(1,2,3))) __OSX_AVAILABLE_STARTING( 1040, 50000 );
		//    
		//vImage_MultidimensionalTable vImageMultidimensionalTable_Create( __const uint16_t *tableData,
		//                                                                 uint32_t numSrcChannels,
		//                                                                 uint32_t numDestChannels,
		//                                                                 uint8_t table_entries_per_dimension[],   
		//                                                                 vImageMDTableUsageHint hint,
		//                                                                 vImage_Flags flags,
		//                                                                 vImage_Error *err )
		//                                                                 __attribute__ ((nonnull(1,4)))
		//                                                                 __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//    
		//vImage_Error vImageMultiDimensionalInterpolatedLookupTable_PlanarF( __const vImage_Buffer srcs[],
		//                                                                    __const vImage_Buffer dests[],
		//                                                                    void *tempBuffer,
		//                                                                    vImage_MultidimensionalTable table,
		//                                                                    vImage_InterpolationMethod method,
		//                                                                    vImage_Flags flags )
		//                                                                    __attribute__ ((nonnull(1,2,4)))
		//                                                                    __OSX_AVAILABLE_STARTING( 1090, 70000 );
		//vImage_Error vImageMultiDimensionalInterpolatedLookupTable_Planar16Q12( __const vImage_Buffer srcs[],
		//                                                                        __const vImage_Buffer dests[],
		//                                                                        void *tempBuffer,
		//                                                                        vImage_MultidimensionalTable table,
		//                                                                        vImage_InterpolationMethod method, 
		//                                                                        vImage_Flags flags )
		//                                                                        __attribute__ ((nonnull(1,2,4)))
		//                                                                        __OSX_AVAILABLE_STARTING( 1090, 70000 );

		#endregion
	}
}
