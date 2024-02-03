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

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

using Pixel8 = System.Byte;
using PixelF = System.Single;
using Pixel16U = System.UInt16;
using Pixel16S = System.Int16;
using ResamplingFilter = System.IntPtr;
using GammaFunction = System.IntPtr;

#if NET
using vImagePixelCount = System.IntPtr;
#else
using vImagePixelCount = System.nint;
#endif

#nullable enable

namespace Accelerate {
	// vImage_Buffer - vImage_Types.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct vImageBuffer {
		public IntPtr Data { get; set; }

		// There is no way a row in the image will have more than 2^32 pixels
		// for their dimensions or row size.   If they ever do, we expose a
		// long property.
		// so expose these as ints as a convenience.
		vImagePixelCount HeightIntPtr;
		vImagePixelCount WidthIntPtr;
		nint RowBytesCountIntPtr; // size_t = nint

		public int BytesPerRow {
			get { return (int) RowBytesCountIntPtr; }
			set { RowBytesCountIntPtr = value; }
		}

		public int Width {
			get { return (int) WidthIntPtr; }
			set { WidthIntPtr = (vImagePixelCount) value; }
		}

		public int Height {
			get { return (int) HeightIntPtr; }
			set { HeightIntPtr = (vImagePixelCount) value; }
		}
	}

	// vImage_AffineTransform - vImage_Types.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct vImageAffineTransformFloat {
		// all defined as 'float'
		public float a, b, c, d;
		public float tx, ty;

		// TODO: constructor from CGAffineTransform, vImageAffineTransformDouble
	}

	// vImage_AffineTransform_Double - vImage_Types.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct vImageAffineTransformDouble {
		public double a, b, c, d;
		public double tx, ty;

		// TODO: constructor from CGAffineTransform, vImageAffineTransformFloat
	}

	// vImage_Error (ssize_t) - vImageTypes.h
	[Native]
	public enum vImageError : long {
		NoError = 0,
		RoiLargerThanInputBuffer = -21766,
		InvalidKernelSize = -21767,
		InvalidEdgeStyle = -21768,
		InvalidOffsetX = -21769,
		InvalidOffsetY = -21770,
		MemoryAllocationError = -21771,
		NullPointerArgument = -21772,
		InvalidParameter = -21773,
		BufferSizeMismatch = -21774,
		UnknownFlagsBit = -21775,
		InternalError = -21776,
		InvalidRowBytes = -21777,
		InvalidImageFormat = -21778,
		ColorSyncIsAbsent = -21779,
		OutOfPlaceOperationRequired = -21780
	}

	// anonymous enum - Transform.h
	public enum vImageGamma {
		kUseGammaValue = 0,
		kUseGammaValueHalfPrecision = 1,
		k5over9_HalfPrecision = 2,
		k9over5_HalfPrecision = 3,
		k5over11_HalfPrecision = 4,
		k11ove_5_HalfPrecision = 5,
		ksRGB_ForwardHalfPrecision = 6,
		ksRGB_ReverseHalfPrecision = 7,
		k11over9_HalfPrecision = 8,
		k9over11_HalfPrecision = 9,
		kBT709_ForwardHalfPrecision = 10,
		kBT709_ReverseHalfPrecision = 11
	};

	// vImageMDTableUsageHint (untyped) - Transform.h
	public enum vImageMDTableUsageHint : int {
		k16Q12 = 1,
		kFloat = 2,
	}

	// vImage_InterpolationMethod (untyped) - Transform.h
	public enum vImageInterpolationMethod : int {
		None = 0,
		Full = 1,
		Half = 2
	}

	[Flags]
	// vImage_Flags (uint32_t) - vImage_Types.h
	public enum vImageFlags : uint {
		NoFlags = 0,
		LeaveAlphaUnchanged = 1,
		CopyInPlace = 2,
		BackgroundColorFill = 4,
		EdgeExtend = 8,
		DoNotTile = 16,
		HighQualityResampling = 32,
		TruncateKernel = 64,
		GetTempBufferSize = 128,
		PrintDiagnosticsToConsole = 256,
		NoAllocate = 512
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct PixelFFFF {
		// all defined as 'float'
		public float A, R, G, B;
		public readonly static PixelFFFF Zero;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct Pixel8888 {
		public byte A, R, G, B;
		public readonly static Pixel8888 Zero;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct PixelARGB16U {
		public Pixel16U A, R, G, B;
		public readonly static PixelARGB16U Zero;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct PixelARGB16S {
		public Pixel16S A, R, G, B;
		public readonly static PixelARGB16S Zero;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	unsafe public static class vImage {

		#region Convolve

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolve_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, Pixel8 backgroundColor, vImageFlags flags);
		public static vImageError ConvolvePlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, Pixel8 backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageConvolve_Planar8 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, divisor, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolve_PlanarF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, PixelF backgroundColor, vImageFlags flags);
		public static vImageError ConvolvePlanarF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, PixelF backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageConvolve_PlanarF (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, Pixel8888* backgroundColor, vImageFlags flags);
		public static vImageError ConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, Pixel8888* backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageConvolve_ARGB8888 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, divisor, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolve_ARGBFFFF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, PixelFFFF backgroundColor, vImageFlags flags);
		public static vImageError ConvolveARGBFFFF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, PixelFFFF backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageConvolve_ARGBFFFF (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolveWithBias_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, int bias, Pixel8 backgroundColor, vImageFlags flags);
		public static vImageError ConvolveWithBiasPlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, int bias, Pixel8 backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageConvolveWithBias_Planar8 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_width, kernel_height, divisor, bias, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageConvolveWithBias_PlanarF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, /* defined as float*/ float* kernel, uint kernel_height, uint kernel_width, float bias, PixelF backgroundColor, vImageFlags flags);
		public static vImageError ConvolveWithBiasPlanarF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, float bias, PixelF backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageConvolveWithBias_PlanarF (
			(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
			(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
			tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, bias, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary, EntryPoint = "vImageConvolveWithBias_ARGB8888")]
		extern static nint vImageConvolveWithBias_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, int bias, Pixel8888 backgroundColor, vImageFlags flags);
		public static vImageError ConvolveWithBiasARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, uint kernel_height, uint kernel_width, int divisor, int bias, Pixel8888 backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageConvolveWithBias_ARGB8888 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, divisor, bias, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary, EntryPoint = "vImageConvolveWithBias_ARGBFFFF")]
		extern static nint vImageConvolveWithBias_ARGBFFFF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, float bias, PixelFFFF backgroundColor, vImageFlags flags);
		public static vImageError ConvolveWithBiasARGBFFFF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, uint kernel_height, uint kernel_width, float bias, PixelFFFF backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageConvolveWithBias_ARGBFFFF (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel_height, kernel_width, bias, backgroundColor, flags);
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
								return (vImageError) (long) vImageConvolveMultiKernel_ARGB8888 (
									(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
									(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
									tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, ptrs, kernel_height, kernel_width, divisors, biases, backgroundColor, flags);
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
								return (vImageError) (long) vImageConvolveMultiKernel_ARGBFFFF (
									(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
									(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
									tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, ptrs, kernel_height, kernel_width, biases, backgroundColor, flags);
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
			return (vImageError) (long) vImageRichardsonLucyDeConvolve_Planar8 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel2, kernel_height, kernel_width, kernel_height2, kernel_width2, divisor, divisor2, backgroundColor, iterationCount, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageRichardsonLucyDeConvolve_PlanarF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, float* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, PixelF backgroundColor, uint iterationCount, vImageFlags flags);
		public static vImageError RichardsonLucyDeConvolvePlanarF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, float* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, PixelF backgroundColor, uint iterationCount, vImageFlags flags)
		{
			return (vImageError) (long) vImageRichardsonLucyDeConvolve_PlanarF (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel2, kernel_height, kernel_width, kernel_height2, kernel_width2, backgroundColor, iterationCount, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageRichardsonLucyDeConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, short* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, int divisor, int divisor2, Pixel8888 backgroundColor, uint iterationCount, vImageFlags flags);
		public static vImageError RichardsonLucyDeConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, short* kernel, short* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, int divisor, int divisor2, Pixel8888 backgroundColor, uint iterationCount, vImageFlags flags)
		{
			return (vImageError) (long) vImageRichardsonLucyDeConvolve_ARGB8888 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel2, kernel_height, kernel_width, kernel_height2, kernel_width2, divisor, divisor2, backgroundColor, iterationCount, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageRichardsonLucyDeConvolve_ARGBFFFF (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, float* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, PixelFFFF backgroundColor, uint iterationCount, vImageFlags flags);
		public static vImageError RichardsonLucyDeConvolveARGBFFFF (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, float* kernel, float* kernel2, uint kernel_height, uint kernel_width, uint kernel_height2, uint kernel_width2, PixelFFFF backgroundColor, uint iterationCount, vImageFlags flags)
		{
			return (vImageError) (long) vImageRichardsonLucyDeConvolve_ARGBFFFF (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel, kernel2, kernel_height, kernel_width, kernel_height2, kernel_width2, backgroundColor, iterationCount, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageBoxConvolve_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8 backgroundColor, vImageFlags flags);
		public static vImageError BoxConvolvePlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8 backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageBoxConvolve_Planar8 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageBoxConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888 backgroundColor, vImageFlags flags);
		public static vImageError BoxConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888 backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageBoxConvolve_ARGB8888 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageBoxConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888* backgroundColor, vImageFlags flags);
		public static vImageError BoxConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888* backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageBoxConvolve_ARGB8888 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageTentConvolve_Planar8 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8 backgroundColor, vImageFlags flags);
		public static vImageError TentConvolvePlanar8 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8 backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageTentConvolve_Planar8 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
		}

		[DllImport (Constants.AccelerateImageLibrary)]
		extern static nint vImageTentConvolve_ARGB8888 (vImageBuffer* src, vImageBuffer* dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888 backgroundColor, vImageFlags flags);
		public static vImageError TentConvolveARGB8888 (ref vImageBuffer src, ref vImageBuffer dest, IntPtr tempBuffer, vImagePixelCount srcOffsetToROI_X, vImagePixelCount srcOffsetToROI_Y, uint kernel_height, uint kernel_width, Pixel8888 backgroundColor, vImageFlags flags)
		{
			return (vImageError) (long) vImageTentConvolve_ARGB8888 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				tempBuffer, srcOffsetToROI_X, srcOffsetToROI_Y, kernel_height, kernel_width, backgroundColor, flags);
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
			return (vImageError) (long) vImageMatrixMultiply_ARGB8888 (
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref src),
				(vImageBuffer*) Unsafe.AsPointer<vImageBuffer> (ref dest),
				matrix, divisor, pre_bias, post_bias, flags);
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
