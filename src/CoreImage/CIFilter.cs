//
// CIFilter.cs: Extensions
//
// Authors:
//   Miguel de Icaza
//   Marek Safar (marek.safar@gmail.com)
//
// Interpreation of magic strings in code, for parameters can be found here:
//
//  https://developer.apple.com/library/prerelease/ios/documentation/GraphicsImaging/Reference/CoreImageFilterReference/Reference/reference.html#//apple_ref/doc/uid/TP40004346
//
// Copyright 2011, 2012, 2015 Xamarin Inc.
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
// Adding new Filters:
//    * Create a new class/interface in coreimage.cs
//    * Add it to CIFilter.FromName switch
//    * Implement properties for each parameter
//    * Use the documented string in "Parameter" in each section as the key for the get/set methods
//    * Ensure the proper return type.
//
// New filters on iOS 7:
//    CIBlendWithAlphaMask,
//    CIBumpDistortion,
//    CIBumpDistortionLinear,
//    CIColorCrossPolynomial,
//    CIColorCubeWithColorSpace,
//    CIColorPolynomial,
//    CIConvolution3X3,
//    CIConvolution5X5,
//    CIConvolution9Horizontal,
//    CIConvolution9Vertical,
//    CILinearToSRGBToneCurve,
//    CIPerspectiveTransformWithExtent,
//    CIPhotoEffectChrome,
//    CIPhotoEffectFade,
//    CIPhotoEffectInstant,
//    CIPhotoEffectMono,
//    CIPhotoEffectNoir,
//    CIPhotoEffectProcess,
//    CIPhotoEffectTonal,
//    CIPhotoEffectTransfer,
//    CIQRCodeGenerator,
//    CISmoothLinearGradient,
//    CISRGBToneCurveToLinear,
//    CIVignetteEffect,
//    CIWhitePointAdjust
//
// New filters on iOS 8:
//    CIAreaHistogram, 
//    CIAztecCodeGenerator, 
//    CICode128BarcodeGenerator, 
//    CIDivideBlendMode,
//    CIGlassDistortion,
//    CIHistogramDisplayFilter,
//    CILinearBurnBlendMode,
//    CILinearDodgeBlendMode,
//    CIPerspectiveCorrection,
//    CIPinLightBlendMode,
//    CISubtractBlendMode
//    CIAccordionFoldTransition (beta2)
//
// New filters on iOS 8.3
//    CIMotionBlur
//    CIZoomBlur
//
// Totally* new filters on iOS 9.0
//    CIPDF417BarcodeGenerator
//    * most of OSX specicic filters are now available on iOS9
//
// New filters on Xcode 9
//    CIAreaMinMaxRed
//    CIAttributedTextImageGenerator
//    CIBarcodeGenerator
//    CIBicubicScaleTransform
//    CIBokehBlur
//    CIColorCubesMixedWithMask
//    CIColorCurves
//    CIDepthBlurEffect
//    CIDepthToDisparity
//    CIDisparityToDepth
//    CIEdgePreserveUpsampleFilter
//    CILabDeltaE
//    CITextImageGenerator
//    CIMorphologyGradient
//    CIMorphologyMaximum
//    CIMorphologyMinimum
//    CIBlendWithBlueMask
//    CIBlendWithRedMask
//
using System;
using System.Diagnostics;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
#if !MONOMAC
using UIKit;
#endif

#nullable enable

namespace CoreImage {
	public partial class CIFilter {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		protected CIFilter () : base ()
		{
		}

		internal CIFilter (string name)
			: base (CreateFilter (name))
		{
		}

		public static string [] FilterNamesInCategories (params string [] categories)
		{
			return _FilterNamesInCategories (categories);
		}

		public NSObject? this [NSString key] {
			get {
				return ValueForKey (key.GetHandle ());
			}
			set {
				SetValueForKey (value, key.GetHandle ());
			}
		}

		internal NSObject? ValueForKey (string key)
		{
			var ptr = CFString.CreateNative (key);
			var value = ValueForKey (ptr);
			CFString.ReleaseNative (ptr);
			return value;
		}

		internal void SetValue (string key, NSObject? value)
		{
			SetHandle (key, value.GetHandle ());
		}

		internal static IntPtr CreateFilter (string name)
		{
			var ptr = CFString.CreateNative (name);
			var result = ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (class_ptr, Selector.GetHandle ("filterWithName:"), ptr);
			CFString.ReleaseNative (ptr);
			return result;
		}

		// helper methods
		internal void SetFloat (string key, float value)
		{
			using (var n = new NSNumber (value))
				SetHandle (key, n.Handle);
		}

		internal void SetInt (string key, int value)
		{
			using (var n = new NSNumber (value))
				SetHandle (key, n.Handle);
		}

		internal void SetNInt (string key, nint value)
		{
			using (var n = new NSNumber (value))
				SetHandle (key, n.Handle);
		}

		internal void SetNUInt (string key, nuint value)
		{
			using (var n = new NSNumber (value))
				SetHandle (key, n.Handle);
		}

		internal void SetBool (string key, bool value)
		{
			using (var n = new NSNumber (value ? 1 : 0))
				SetHandle (key, n.Handle);
		}

		internal void SetString (string key, string value)
		{
			var ptr = CFString.CreateNative (value);
			SetHandle (key, ptr);
			CFString.ReleaseNative (ptr);
		}

		internal void SetValue (string key, CGPoint value)
		{
			using (var nsv = new CIVector (value.X, value.Y))
				SetHandle (key, nsv.Handle);
		}

		internal void SetValue (string key, CGRect value)
		{
			using (var nsv = new CIVector (value.X, value.Y, value.Width, value.Height))
				SetHandle (key, nsv.Handle);
		}

		internal T? Get<T> (string key) where T : class
		{
			var ptr = CFString.CreateNative (key);
			var value = ValueForKey (key) as T;
			CFString.ReleaseNative (ptr);
			return value;
		}

		internal float GetFloat (string key)
		{
			return Get<NSNumber> (key)?.FloatValue ?? default (float);
		}

		internal int GetInt (string key)
		{
			return Get<NSNumber> (key)?.Int32Value ?? default (int);
		}

		internal nint GetNInt (string key)
		{
			return Get<NSNumber> (key)?.NIntValue ?? default (nint);
		}

		internal nuint GetNUInt (string key)
		{
			return Get<NSNumber> (key)?.NUIntValue ?? default (nuint);
		}

		internal bool GetBool (string key)
		{
			return Get<NSNumber> (key)?.BoolValue ?? default (bool);
		}

		internal void SetHandle (string key, IntPtr handle)
		{
			var nsname = CFString.CreateNative (key);

			if (IsDirectBinding) {
				Messaging.void_objc_msgSend_IntPtr_IntPtr (
					this.Handle, Selector.GetHandle ("setValue:forKey:"), handle, nsname);
			} else {
				Messaging.void_objc_msgSendSuper_IntPtr_IntPtr (
					this.SuperHandle, Selector.GetHandle ("setValue:forKey:"), handle, nsname);
			}
			CFString.ReleaseNative (nsname);
		}

		internal IntPtr GetHandle (string key)
		{
			var nsname = CFString.CreateNative (key);
			IntPtr ret;

			if (IsDirectBinding)
				ret = Messaging.IntPtr_objc_msgSend_IntPtr (Handle, Selector.GetHandle ("valueForKey:"), nsname);
			else
				ret = Messaging.IntPtr_objc_msgSendSuper_IntPtr (SuperHandle, Selector.GetHandle ("valueForKey:"), nsname);

			CFString.ReleaseNative (nsname);
			return ret;
		}

		internal CGPoint GetPoint (string key)
		{
			var v = Get<CIVector> (key);
			return v is not null ? new CGPoint (v.X, v.Y) : default (CGPoint);
		}

		internal CGRect GetRect (string key)
		{
			var v = Get<CIVector> (key);
			return v is not null ? new CGRect (v.X, v.Y, v.Z, v.W) : default (CGRect);
		}

#if MONOMAC
		public virtual CIImage? OutputImage {
			get { return ValueForKey (CIFilterOutputKey.Image) as CIImage; }
		}
#endif

		// Calls the selName selector for cases where we do not have an instance created
		static internal string? GetFilterName (IntPtr filterHandle)
		{
			return CFString.FromHandle (ObjCRuntime.Messaging.IntPtr_objc_msgSend (filterHandle, Selector.GetHandle ("name")));
		}

		// TODO could be generated too
		internal static CIFilter FromName (string? filterName, IntPtr handle)
		{
			switch (filterName) {
			case "CIAdditionCompositing":
				return new CIAdditionCompositing (handle);
			case "CIAffineTransform":
				return new CIAffineTransform (handle);
			case "CICheckerboardGenerator":
				return new CICheckerboardGenerator (handle);
			case "CIColorBlendMode":
				return new CIColorBlendMode (handle);
			case "CIColorBurnBlendMode":
				return new CIColorBurnBlendMode (handle);
			case "CIColorControls":
				return new CIColorControls (handle);
			case "CIColorCube":
				return new CIColorCube (handle);
			case "CIColorDodgeBlendMode":
				return new CIColorDodgeBlendMode (handle);
			case "CIColorInvert":
				return new CIColorInvert (handle);
			case "CIColorMatrix":
				return new CIColorMatrix (handle);
			case "CIColorMonochrome":
				return new CIColorMonochrome (handle);
			case "CIConstantColorGenerator":
				return new CIConstantColorGenerator (handle);
			case "CICrop":
				return new CICrop (handle);
			case "CIDarkenBlendMode":
				return new CIDarkenBlendMode (handle);
			case "CIDifferenceBlendMode":
				return new CIDifferenceBlendMode (handle);
			case "CIExclusionBlendMode":
				return new CIExclusionBlendMode (handle);
			case "CIExposureAdjust":
				return new CIExposureAdjust (handle);
			case "CIFalseColor":
				return new CIFalseColor (handle);
			case "CIGammaAdjust":
				return new CIGammaAdjust (handle);
			case "CIGaussianGradient":
				return new CIGaussianGradient (handle);
			case "CIHardLightBlendMode":
				return new CIHardLightBlendMode (handle);
			case "CIHighlightShadowAdjust":
				return new CIHighlightShadowAdjust (handle);
			case "CIHueAdjust":
				return new CIHueAdjust (handle);
			case "CIHueBlendMode":
				return new CIHueBlendMode (handle);
			case "CILightenBlendMode":
				return new CILightenBlendMode (handle);
			case "CILinearGradient":
				return new CILinearGradient (handle);
			case "CILuminosityBlendMode":
				return new CILuminosityBlendMode (handle);
			case "CIMaximumCompositing":
				return new CIMaximumCompositing (handle);
			case "CIMinimumCompositing":
				return new CIMinimumCompositing (handle);
			case "CIMultiplyBlendMode":
				return new CIMultiplyBlendMode (handle);
			case "CIMultiplyCompositing":
				return new CIMultiplyCompositing (handle);
			case "CIOverlayBlendMode":
				return new CIOverlayBlendMode (handle);
			case "CIRadialGradient":
				return new CIRadialGradient (handle);
			case "CISaturationBlendMode":
				return new CISaturationBlendMode (handle);
			case "CIScreenBlendMode":
				return new CIScreenBlendMode (handle);
			case "CISepiaTone":
				return new CISepiaTone (handle);
			case "CISoftLightBlendMode":
				return new CISoftLightBlendMode (handle);
			case "CISourceAtopCompositing":
				return new CISourceAtopCompositing (handle);
			case "CISourceInCompositing":
				return new CISourceInCompositing (handle);
			case "CISourceOutCompositing":
				return new CISourceOutCompositing (handle);
			case "CISourceOverCompositing":
				return new CISourceOverCompositing (handle);
			case "CIStraightenFilter":
				return new CIStraightenFilter (handle);
			case "CIStripesGenerator":
				return new CIStripesGenerator (handle);
			case "CITemperatureAndTint":
				return new CITemperatureAndTint (handle);
			case "CIToneCurve":
				return new CIToneCurve (handle);
			case "CIVibrance":
				return new CIVibrance (handle);
			case "CIWhitePointAdjust":
				return new CIWhitePointAdjust (handle);
			case "CIFaceBalance":
				return new CIFaceBalance (handle);
			case "CIAffineClamp":
				return new CIAffineClamp (handle);
			case "CIAffineTile":
				return new CIAffineTile (handle);
			case "CIBlendWithMask":
				return new CIBlendWithMask (handle);
			case "CIBarsSwipeTransition":
				return new CIBarsSwipeTransition (handle);
			case "CICopyMachineTransition":
				return new CICopyMachineTransition (handle);
			case "CIDisintegrateWithMaskTransition":
				return new CIDisintegrateWithMaskTransition (handle);
			case "CIDissolveTransition":
				return new CIDissolveTransition (handle);
			case "CIFlashTransition":
				return new CIFlashTransition (handle);
			case "CIModTransition":
				return new CIModTransition (handle);
			case "CISwipeTransition":
				return new CISwipeTransition (handle);
			case "CIBloom":
				return new CIBloom (handle);
			case "CICircularScreen":
				return new CICircularScreen (handle);
			case "CIDotScreen":
				return new CIDotScreen (handle);
			case "CIHatchedScreen":
				return new CIHatchedScreen (handle);
			case "CILineScreen":
				return new CILineScreen (handle);
			case "CIColorMap":
				return new CIColorMap (handle);
			case "CIColorPosterize":
				return new CIColorPosterize (handle);
			case "CIEightfoldReflectedTile":
				return new CIEightfoldReflectedTile (handle);
			case "CIFourfoldReflectedTile":
				return new CIFourfoldReflectedTile (handle);
			case "CIFourfoldRotatedTile":
				return new CIFourfoldRotatedTile (handle);
			case "CIFourfoldTranslatedTile":
				return new CIFourfoldTranslatedTile (handle);
			case "CISixfoldReflectedTile":
				return new CISixfoldReflectedTile (handle);
			case "CISixfoldRotatedTile":
				return new CISixfoldRotatedTile (handle);
			case "CITwelvefoldReflectedTile":
				return new CITwelvefoldReflectedTile (handle);
			case "CIGaussianBlur":
				return new CIGaussianBlur (handle);
			case "CIGloom":
				return new CIGloom (handle);
			case "CIHoleDistortion":
				return new CIHoleDistortion (handle);
			case "CIPinchDistortion":
				return new CIPinchDistortion (handle);
			case "CITwirlDistortion":
				return new CITwirlDistortion (handle);
			case "CIVortexDistortion":
				return new CIVortexDistortion (handle);
			case "CILanczosScaleTransform":
				return new CILanczosScaleTransform (handle);
			case "CIMaskToAlpha":
				return new CIMaskToAlpha (handle);
			case "CIMaximumComponent":
				return new CIMaximumComponent (handle);
			case "CIMinimumComponent":
				return new CIMinimumComponent (handle);
			case "CIPersonSegmentation":
				return new CIPersonSegmentation (handle);
			case "CIPerspectiveTile":
				return new CIPerspectiveTile (handle);
			case "CIPerspectiveTransform":
				return new CIPerspectiveTransform (handle);
			case "CIPixellate":
				return new CIPixellate (handle);
			case "CIRandomGenerator":
				return new CIRandomGenerator (handle);
			case "CISharpenLuminance":
				return new CISharpenLuminance (handle);
			case "CIStarShineGenerator":
				return new CIStarShineGenerator (handle);
			case "CIUnsharpMask":
				return new CIUnsharpMask (handle);
			case "CICircleSplashDistortion":
				return new CICircleSplashDistortion (handle);
			case "CIColorClamp":
				return new CIColorClamp (handle);
			case "CIDepthOfField":
				return new CIDepthOfField (handle);
			case "CIPageCurlTransition":
				return new CIPageCurlTransition (handle);
			case "CIRippleTransition":
				return new CIRippleTransition (handle);
			case "CILightTunnel":
				return new CILightTunnel (handle);
			case "CITriangleKaleidoscope":
				return new CITriangleKaleidoscope (handle);
			case "CIVignette":
				return new CIVignette (handle);
			case "CIBlendWithAlphaMask":
				return new CIBlendWithAlphaMask (handle);
			case "CIBumpDistortion":
				return new CIBumpDistortion (handle);
			case "CIBumpDistortionLinear":
				return new CIBumpDistortionLinear (handle);
			case "CIColorCrossPolynomial":
				return new CIColorCrossPolynomial (handle);
			case "CIColorCubeWithColorSpace":
				return new CIColorCubeWithColorSpace (handle);
			case "CIColorPolynomial":
				return new CIColorPolynomial (handle);
			case "CIConvolution3X3":
				return new CIConvolution3X3 (handle);
			case "CIConvolution5X5":
				return new CIConvolution5X5 (handle);
			case "CIConvolution7X7":
				return new CIConvolution7X7 (handle);
			case "CIConvolution9Horizontal":
				return new CIConvolution9Horizontal (handle);
			case "CIConvolution9Vertical":
				return new CIConvolution9Vertical (handle);
			case "CIConvolutionRGB3X3":
				return new CIConvolutionRGB3X3 (handle);
			case "CIConvolutionRGB5X5":
				return new CIConvolutionRGB5X5 (handle);
			case "CIConvolutionRGB7X7":
				return new CIConvolutionRGB7X7 (handle);
			case "CIConvolutionRGB9Horizontal":
				return new CIConvolutionRGB9Horizontal (handle);
			case "CIConvolutionRGB9Vertical":
				return new CIConvolutionRGB9Vertical (handle);
			case "CILinearToSRGBToneCurve":
				return new CILinearToSRGBToneCurve (handle);
			case "CIPerspectiveTransformWithExtent":
				return new CIPerspectiveTransformWithExtent (handle);
			case "CIPhotoEffectChrome":
				return new CIPhotoEffectChrome (handle);
			case "CIPhotoEffectFade":
				return new CIPhotoEffectFade (handle);
			case "CIPhotoEffectInstant":
				return new CIPhotoEffectInstant (handle);
			case "CIPhotoEffectMono":
				return new CIPhotoEffectMono (handle);
			case "CIPhotoEffectNoir":
				return new CIPhotoEffectNoir (handle);
			case "CIPhotoEffectProcess":
				return new CIPhotoEffectProcess (handle);
			case "CIPhotoEffectTonal":
				return new CIPhotoEffectTonal (handle);
			case "CIPhotoEffectTransfer":
				return new CIPhotoEffectTransfer (handle);
			case "CIQRCodeGenerator":
				return new CIQRCodeGenerator (handle);
			case "CISmoothLinearGradient":
				return new CISmoothLinearGradient (handle);
			case "CISRGBToneCurveToLinear":
				return new CISRGBToneCurveToLinear (handle);
			case "CIVignetteEffect":
				return new CIVignetteEffect (handle);
			case "CIWhitePointAdjus":
				return new CIWhitePointAdjust (handle);
			case "CIAreaHistogram":
				return new CIAreaHistogram (handle);
			case "CIAztecCodeGenerator":
				return new CIAztecCodeGenerator (handle);
			case "CICode128BarcodeGenerator":
				return new CICode128BarcodeGenerator (handle);
			case "CIDivideBlendMode":
				return new CIDivideBlendMode (handle);
			case "CIGlassDistortion":
				return new CIGlassDistortion (handle);
			case "CIHistogramDisplayFilter":
				return new CIHistogramDisplayFilter (handle);
			case "CILinearBurnBlendMode":
				return new CILinearBurnBlendMode (handle);
			case "CILinearDodgeBlendMode":
				return new CILinearDodgeBlendMode (handle);
			case "CILinearLightBlendMode":
				return new CILinearLightBlendMode (handle);
			case "CIPerspectiveCorrection":
				return new CIPerspectiveCorrection (handle);
			case "CIPinLightBlendMode":
				return new CIPinLightBlendMode (handle);
			case "CISubtractBlendMode":
				return new CISubtractBlendMode (handle);
			case "CIVividLightBlendMode":
				return new CIVividLightBlendMode (handle);
			case "CIAccordionFoldTransition":
				return new CIAccordionFoldTransition (handle);
			case "CIAreaAverage":
				return new CIAreaAverage (handle);
			case "CIAreaMaximum":
				return new CIAreaMaximum (handle);
			case "CIAreaMaximumAlpha":
				return new CIAreaMaximumAlpha (handle);
			case "CIAreaMinimum":
				return new CIAreaMinimum (handle);
			case "CIAreaMinimumAlpha":
				return new CIAreaMinimumAlpha (handle);
			case "CIBoxBlur":
				return new CIBoxBlur (handle);
			case "CICircularWrap":
				return new CICircularWrap (handle);
			case "CICmykHalftone":
			case "CICMYKHalftone":
				return new CICmykHalftone (handle);
			case "CIColumnAverage":
				return new CIColumnAverage (handle);
			case "CIComicEffect":
				return new CIComicEffect (handle);
			case "CICrystallize":
				return new CICrystallize (handle);
			case "CIDiscBlur":
				return new CIDiscBlur (handle);
			case "CIDisplacementDistortion":
				return new CIDisplacementDistortion (handle);
			case "CIDroste":
				return new CIDroste (handle);
			case "CIEdges":
				return new CIEdges (handle);
			case "CIEdgeWork":
				return new CIEdgeWork (handle);
			case "CIGlassLozenge":
				return new CIGlassLozenge (handle);
			case "CIHeightFieldFromMask":
				return new CIHeightFieldFromMask (handle);
			case "CIHexagonalPixellate":
				return new CIHexagonalPixellate (handle);
			case "CIKaleidoscope":
				return new CIKaleidoscope (handle);
			case "CILenticularHaloGenerator":
				return new CILenticularHaloGenerator (handle);
			case "CILineOverlay":
				return new CILineOverlay (handle);
			case "CIMaskedVariableBlur":
				return new CIMaskedVariableBlur (handle);
			case "CIMedianFilter":
				return new CIMedianFilter (handle);
			case "CINoiseReduction":
				return new CINoiseReduction (handle);
			case "CIOpTile":
				return new CIOpTile (handle);
			case "CIPageCurlWithShadowTransition":
				return new CIPageCurlWithShadowTransition (handle);
			case "CIParallelogramTile":
				return new CIParallelogramTile (handle);
			case "CIPointillize":
				return new CIPointillize (handle);
			case "CIRowAverage":
				return new CIRowAverage (handle);
			case "CIShadedMaterial":
				return new CIShadedMaterial (handle);
			case "CISpotColor":
				return new CISpotColor (handle);
			case "CISpotLight":
				return new CISpotLight (handle);
			case "CIStretchCrop":
				return new CIStretchCrop (handle);
			case "CISunbeamsGenerator":
				return new CISunbeamsGenerator (handle);
			case "CITorusLensDistortion":
				return new CITorusLensDistortion (handle);
			case "CITriangleTile":
				return new CITriangleTile (handle);
			case "CIMotionBlur":
				return new CIMotionBlur (handle);
			case "CIZoomBlur":
				return new CIZoomBlur (handle);
			case "CIPDF417BarcodeGenerator":
			case "CIPdf417BarcodeGenerator":
				return new CIPdf417BarcodeGenerator (handle);
			case "CIAreaMinMaxRed":
				return new CIAreaMinMaxRed (handle);
			case "CIAttributedTextImageGenerator":
				return new CIAttributedTextImageGenerator (handle);
			case "CIBarcodeGenerator":
				return new CIBarcodeGenerator (handle);
			case "CIBicubicScaleTransform":
				return new CIBicubicScaleTransform (handle);
			case "CIBokehBlur":
				return new CIBokehBlur (handle);
			case "CIColorCubesMixedWithMask":
				return new CIColorCubesMixedWithMask (handle);
			case "CIColorCurves":
				return new CIColorCurves (handle);
			case "CIDepthBlurEffect":
				return new CIDepthBlurEffect (handle);
			case "CIDepthToDisparity":
				return new CIDepthToDisparity (handle);
			case "CIDisparityToDepth":
				return new CIDisparityToDepth (handle);
			case "CIEdgePreserveUpsampleFilter":
				return new CIEdgePreserveUpsampleFilter (handle);
			case "CILabDeltaE":
				return new CILabDeltaE (handle);
			case "CITextImageGenerator":
				return new CITextImageGenerator (handle);
			case "CIMorphologyGradient":
				return new CIMorphologyGradient (handle);
			case "CIMorphologyMaximum":
				return new CIMorphologyMaximum (handle);
			case "CIMorphologyMinimum":
				return new CIMorphologyMinimum (handle);
			case "CIBlendWithBlueMask":
				return new CIBlendWithBlueMask (handle);
			case "CIBlendWithRedMask":
				return new CIBlendWithRedMask (handle);
			default:
				throw new NotImplementedException (String.Format ("Unknown filter type returned: `{0}', returning a default CIFilter", filterName));
			}
		}

#if !NET
		// not every CIFilter supports inputImage, i.e.
		// NSUnknownKeyException [<CICheckerboardGenerator 0x1648cb20> valueForUndefinedKey:]: this class is not key value coding-compliant for the key inputImage.
		// and those will crash (on devices) if the property is called - and that includes displaying it in the debugger
		[Obsolete ("Use 'InputImage' instead. If not available then the filter does not support it.")]
		public CIImage? Image {
			get {
				return SupportsInputImage ? ValueForKey (CIFilterInputKey.Image) as CIImage : null;
			}
			set {
				if (!SupportsInputImage)
					ObjCRuntime.ThrowHelper.ThrowArgumentException ("inputImage is not supported by this filter");
				SetValueForKey (value, CIFilterInputKey.Image.GetHandle ());
			}
		}

		bool? supportsInputImage;

		bool SupportsInputImage {
			get {
				if (!supportsInputImage.HasValue)
					supportsInputImage = Array.IndexOf (InputKeys, "inputImage") >= 0;
				return supportsInputImage.Value;
			}
		}
#endif
	}

#if MONOMAC && !XAMCORE_3_0 && !NET
	[Obsolete ("This type has been renamed to CICmykHalftone.")]
	public class CICMYKHalftone : CICmykHalftone {
		public CICMYKHalftone () {}
		public CICMYKHalftone (IntPtr handle) : base (handle) {}
	}
#endif
}
