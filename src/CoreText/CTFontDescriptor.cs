// 
// CTFontDescriptor.cs: Implements the managed CTFontDescriptor
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2011 - 2014 Xamarin Inc
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontDescriptor.h
	public enum CTFontOrientation : uint {
		Default = 0,
		Horizontal = 1,
		Vertical = 2,
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontDescriptor.h
	public enum CTFontFormat : uint {
		Unrecognized = 0,
		OpenTypePostScript = 1,
		OpenTypeTrueType = 2,
		TrueType = 3,
		PostScript = 4,
		Bitmap = 5,
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontDescriptor.h
	public enum CTFontPriority : uint {
		System = 10000,
		Network = 20000,
		Computer = 30000,
		User = 40000,
		Dynamic = 50000,
		Process = 60000,
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontDescriptor.h
	public enum CTFontDescriptorMatchingState : uint {
		Started,
		Finished,
		WillBeginQuerying,
		Stalled,
		WillBeginDownloading,
		Downloading,
		DownloadingFinished,
		Matched,
		FailedWithError
	}

#if !NET
	public static class CTFontDescriptorAttributeKey {
		public static readonly NSString? Url;
		public static readonly NSString? Name;
		public static readonly NSString? DisplayName;
		public static readonly NSString? FamilyName;
		public static readonly NSString? StyleName;
		public static readonly NSString? Traits;
		public static readonly NSString? Variation;
		public static readonly NSString? Size;
		public static readonly NSString? Matrix;
		public static readonly NSString? CascadeList;
		public static readonly NSString? CharacterSet;
		public static readonly NSString? Languages;
		public static readonly NSString? BaselineAdjust;
		public static readonly NSString? MacintoshEncodings;
		public static readonly NSString? Features;
		public static readonly NSString? FeatureSettings;
		public static readonly NSString? FixedAdvance;
		public static readonly NSString? FontOrientation;
		public static readonly NSString? FontFormat;
		public static readonly NSString? RegistrationScope;
		public static readonly NSString? Priority;
		public static readonly NSString? Enabled;

		static CTFontDescriptorAttributeKey ()
		{
			var handle = Libraries.CoreText.Handle;
			Url = Dlfcn.GetStringConstant (handle, "kCTFontURLAttribute");
			Name = Dlfcn.GetStringConstant (handle, "kCTFontNameAttribute");
			DisplayName = Dlfcn.GetStringConstant (handle, "kCTFontDisplayNameAttribute");
			FamilyName = Dlfcn.GetStringConstant (handle, "kCTFontFamilyNameAttribute");
			StyleName = Dlfcn.GetStringConstant (handle, "kCTFontStyleNameAttribute");
			Traits = Dlfcn.GetStringConstant (handle, "kCTFontTraitsAttribute");
			Variation = Dlfcn.GetStringConstant (handle, "kCTFontVariationAttribute");
			Size = Dlfcn.GetStringConstant (handle, "kCTFontSizeAttribute");
			Matrix = Dlfcn.GetStringConstant (handle, "kCTFontMatrixAttribute");
			CascadeList = Dlfcn.GetStringConstant (handle, "kCTFontCascadeListAttribute");
			CharacterSet = Dlfcn.GetStringConstant (handle, "kCTFontCharacterSetAttribute");
			Languages = Dlfcn.GetStringConstant (handle, "kCTFontLanguagesAttribute");
			BaselineAdjust = Dlfcn.GetStringConstant (handle, "kCTFontBaselineAdjustAttribute");
			MacintoshEncodings = Dlfcn.GetStringConstant (handle, "kCTFontMacintoshEncodingsAttribute");
			Features = Dlfcn.GetStringConstant (handle, "kCTFontFeaturesAttribute");
			FeatureSettings = Dlfcn.GetStringConstant (handle, "kCTFontFeatureSettingsAttribute");
			FixedAdvance = Dlfcn.GetStringConstant (handle, "kCTFontFixedAdvanceAttribute");
			FontOrientation = Dlfcn.GetStringConstant (handle, "kCTFontOrientationAttribute");
			FontFormat = Dlfcn.GetStringConstant (handle, "kCTFontFormatAttribute");
			RegistrationScope = Dlfcn.GetStringConstant (handle, "kCTFontRegistrationScopeAttribute");
			Priority = Dlfcn.GetStringConstant (handle, "kCTFontPriorityAttribute");
			Enabled = Dlfcn.GetStringConstant (handle, "kCTFontEnabledAttribute");
		}
	}
#endif // !NET

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontDescriptorAttributes {

		public CTFontDescriptorAttributes ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFontDescriptorAttributes (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary { get; private set; }

		public NSUrl? Url {
			get { return (NSUrl?) Dictionary [CTFontDescriptorAttributeKey.Url]; }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Url!, value); }
		}

		public string? Name {
			get { return Adapter.GetStringValue (Dictionary, CTFontDescriptorAttributeKey.Name); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Name!, value); }
		}

		public string? DisplayName {
			get { return Adapter.GetStringValue (Dictionary, CTFontDescriptorAttributeKey.DisplayName); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.DisplayName!, value); }
		}

		public string? FamilyName {
			get { return Adapter.GetStringValue (Dictionary, CTFontDescriptorAttributeKey.FamilyName); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FamilyName!, value); }
		}

		public string? StyleName {
			get { return Adapter.GetStringValue (Dictionary, CTFontDescriptorAttributeKey.StyleName); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.StyleName!, value); }
		}

		public CTFontTraits? Traits {
			get {
				if (CTFontDescriptorAttributeKey.Traits is NSString traitsKey && Dictionary [traitsKey] is NSDictionary traits)
					return new CTFontTraits (traits);
				return null;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Traits!, value?.Dictionary);
			}
		}

		public CTFontVariation? Variation {
			get {
				if (CTFontDescriptorAttributeKey.Variation is NSString variationKey && Dictionary [variationKey] is NSDictionary variation)
					return new CTFontVariation (variation);
				return null;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Variation!, value?.Dictionary);
			}
		}

		// CFNumber
		public float? Size {
			get { return Adapter.GetSingleValue (Dictionary, CTFontDescriptorAttributeKey.Size); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Size!, value); }
		}

		public unsafe CGAffineTransform? Matrix {
			get {
				if (CTFontDescriptorAttributeKey.Matrix is NSString matrixKey && Dictionary [matrixKey] is NSData d)
					return Marshal.PtrToStructure<CGAffineTransform> (d.Bytes);
				return null;
			}
			set {
				if (CTFontDescriptorAttributeKey.Matrix is null)
					throw new ArgumentOutOfRangeException (nameof (CTFontDescriptorAttributeKey.Matrix));
				if (!value.HasValue)
					Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Matrix!, (NSObject?) null);
				else {
					byte [] data = new byte [Marshal.SizeOf<CGAffineTransform> ()];
					fixed (byte* p = data) {
						Marshal.StructureToPtr<CGAffineTransform> (value.Value, (IntPtr) p, false);
					}
					Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Matrix!, NSData.FromArray (data));
				}
			}
		}

		public IEnumerable<CTFontDescriptor>? CascadeList {
			get {
				if (CTFontDescriptorAttributeKey.CascadeList is NSString cascadeList)
					return Adapter.GetNativeArray (Dictionary, cascadeList, d => new CTFontDescriptor (d, false));
				return null;
			}
			set { Adapter.SetNativeValue (Dictionary, CTFontDescriptorAttributeKey.CascadeList!, value); }
		}

		public NSCharacterSet? CharacterSet {
			get { return (NSCharacterSet) Dictionary [CTFontDescriptorAttributeKey.CharacterSet]; }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.CharacterSet!, value); }
		}

		public IEnumerable<string>? Languages {
			get { return Adapter.GetStringArray (Dictionary, CTFontDescriptorAttributeKey.Languages); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Languages!, value); }
		}

		// float represented as a CFNumber
		public float? BaselineAdjust {
			get { return Adapter.GetSingleValue (Dictionary, CTFontDescriptorAttributeKey.BaselineAdjust); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.BaselineAdjust!, value); }
		}

		public float? MacintoshEncodings {
			get { return Adapter.GetSingleValue (Dictionary, CTFontDescriptorAttributeKey.MacintoshEncodings); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.MacintoshEncodings!, value); }
		}

		public IEnumerable<CTFontFeatures>? Features {
			get {
				if (CTFontDescriptorAttributeKey.Features is NSString features) {
					return Adapter.GetNativeArray (Dictionary, features,
						d => new CTFontFeatures ((NSDictionary) Runtime.GetNSObject (d)!));
				}
				return null;
			}
			set {
				if (CTFontDescriptorAttributeKey.Features is null)
					throw new ArgumentOutOfRangeException (nameof (CTFontDescriptorAttributeKey.Features));

				List<CTFontFeatures> v;
				if (value is null || (v = new List<CTFontFeatures> (value)).Count == 0) {
					Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Features!, (NSObject?) null);
					return;
				}
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Features!,
						NSArray.FromNSObjects ((IList<NSObject>) v.ConvertAll (e => (NSObject) e.Dictionary)));
			}
		}

		public IEnumerable<CTFontFeatureSettings>? FeatureSettings {
			get {
				if (CTFontDescriptorAttributeKey.Features is NSString features) {
					return Adapter.GetNativeArray (Dictionary, CTFontDescriptorAttributeKey.Features,
						d => new CTFontFeatureSettings ((NSDictionary) Runtime.GetNSObject (d)!));
				}
				return null;
			}
			set {
				if (CTFontDescriptorAttributeKey.Features is null)
					throw new ArgumentOutOfRangeException (nameof (CTFontDescriptorAttributeKey.Features));
				List<CTFontFeatureSettings> v;
				if (value is null || (v = new List<CTFontFeatureSettings> (value)).Count == 0) {
					Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Features!, (NSObject?) null);
					return;
				}

				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FeatureSettings!,
						NSArray.FromNSObjects ((IList<NSObject>) v.ConvertAll (e => (NSObject) e.Dictionary)));
			}
		}

		// CFNumber
		public float? FixedAdvance {
			get { return Adapter.GetSingleValue (Dictionary, CTFontDescriptorAttributeKey.FixedAdvance); }
			set { Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FixedAdvance!, value); }
		}

		public CTFontOrientation? FontOrientation {
			get {
				if (CTFontDescriptorAttributeKey.FontOrientation is NSString fontOrientation) {
					var value = Adapter.GetUInt32Value (Dictionary, fontOrientation);
					return !value.HasValue ? null : (CTFontOrientation?) value.Value;
				}
				return null;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FontOrientation!,
						value.HasValue ? (uint?) value.Value : null);
			}
		}

		public CTFontFormat? FontFormat {
			get {
				if (CTFontDescriptorAttributeKey.FontFormat is NSString fontFormat) {
					var value = Adapter.GetUInt32Value (Dictionary, fontFormat);
					return !value.HasValue ? null : (CTFontFormat?) value.Value;
				}
				return null;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FontFormat!,
						value.HasValue ? (uint?) value.Value : null);
			}
		}

		public CTFontManagerScope? RegistrationScope {
			get {
				if (CTFontDescriptorAttributeKey.RegistrationScope is NSString registrationScope) {
					var value = Adapter.GetUnsignedIntegerValue (Dictionary, registrationScope);
					return !value.HasValue ? null : (CTFontManagerScope?) (ulong) value.Value;
				}
				return null;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.RegistrationScope!,
				value.HasValue ? (nuint?) (ulong) value.Value : null);
			}
		}

		public CTFontPriority? Priority {
			get {
				if (CTFontDescriptorAttributeKey.Priority is NSString priority) {
					var value = Adapter.GetUInt32Value (Dictionary, priority);
					return !value.HasValue ? null : (CTFontPriority?) value.Value;
				}
				return null;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Priority!,
						value.HasValue ? (uint?) value.Value : null);
			}
		}

#if XAMCORE_5_0
		public bool? Enabled {
#else
		public bool? WeakEnabled {
#endif
			get {
				var value = (NSNumber?) Dictionary [CTFontDescriptorAttributeKey.Enabled];
				if (value is null)
					return null;
				return value.Int32Value != 0;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Enabled!, value is null ? null : new NSNumber ((value.Value ? 1 : 0)));
			}
		}

#if !XAMCORE_5_0
		public bool Enabled {
			get {
				var value = (NSNumber) Dictionary [CTFontDescriptorAttributeKey.Enabled];
				if (value is null)
					return false;
				return value.Int32Value != 0;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Enabled!, value ? new NSNumber (1) : null);
			}
		}
#endif // !XAMCORE_5_0
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class CTFontDescriptor : NativeObject {
		[Preserve (Conditional = true)]
		internal CTFontDescriptor (NativeHandle handle, bool owns)
			: base (handle, owns, true)
		{
		}

		#region Descriptor Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateWithNameAndSize (IntPtr name, nfloat size);
		static IntPtr Create (string name, nfloat size)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			var nameHandle = CFString.CreateNative (name);
			try {
				return CTFontDescriptorCreateWithNameAndSize (nameHandle, size);
			} finally {
				CFString.ReleaseNative (nameHandle);
			}
		}

		public CTFontDescriptor (string name, nfloat size)
			: base (Create (name, size), true, true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateWithAttributes (IntPtr attributes);
		public CTFontDescriptor (CTFontDescriptorAttributes attributes)
			: base (CTFontDescriptorCreateWithAttributes (Runtime.ThrowOnNull (attributes, nameof (attributes)).Dictionary.Handle), true, true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateCopyWithAttributes (IntPtr original, IntPtr attributes);
		public CTFontDescriptor? WithAttributes (NSDictionary attributes)
		{
			if (attributes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (attributes));
			return CreateDescriptor (CTFontDescriptorCreateCopyWithAttributes (Handle, attributes.Handle));
		}

		static CTFontDescriptor? CreateDescriptor (IntPtr h)
		{
			if (h == IntPtr.Zero)
				return null;
			return new CTFontDescriptor (h, true);
		}

		public CTFontDescriptor? WithAttributes (CTFontDescriptorAttributes attributes)
		{
			if (attributes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (attributes));
			return CreateDescriptor (CTFontDescriptorCreateCopyWithAttributes (Handle, attributes.Dictionary.Handle));
		}

		// TODO: is there a better type to use for variationIdentifier?  
		// uint perhaps?  "This is the four character code of the variation axis"
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateCopyWithVariation (IntPtr original, IntPtr variationIdentifier, nfloat variationValue);
		public CTFontDescriptor? WithVariation (uint variationIdentifier, nfloat variationValue)
		{
			using (var id = new NSNumber (variationIdentifier))
				return CreateDescriptor (CTFontDescriptorCreateCopyWithVariation (Handle,
							id.Handle, variationValue));
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateCopyWithFeature (IntPtr original, IntPtr featureTypeIdentifier, IntPtr featureSelectorIdentifier);

		public CTFontDescriptor? WithFeature (CTFontFeatureAllTypographicFeatures.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.AllTypographicFeatures, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureLigatures.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Ligatures, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureCursiveConnection.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CursiveConnection, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureVerticalSubstitutionConnection.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.VerticalSubstitution, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureLinguisticRearrangementConnection.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.LinguisticRearrangement, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureNumberSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.NumberSpacing, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureSmartSwash.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.SmartSwash, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureDiacritics.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Diacritics, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureVerticalPosition.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.VerticalPosition, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureFractions.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Fractions, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureOverlappingCharacters.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.OverlappingCharacters, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureTypographicExtras.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.TypographicExtras, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureMathematicalExtras.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.MathematicalExtras, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureOrnamentSets.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.OrnamentSets, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureCharacterAlternatives.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CharacterAlternatives, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureDesignComplexity.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.DesignComplexity, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureStyleOptions.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.StyleOptions, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureCharacterShape.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CharacterShape, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureNumberCase.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.NumberCase, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureTextSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.TextSpacing, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureTransliteration.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Transliteration, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureAnnotation.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Annotation, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureKanaSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.KanaSpacing, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureIdeographicSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.IdeographicSpacing, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureUnicodeDecomposition.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.UnicodeDecomposition, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureRubyKana.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.RubyKana, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureCJKSymbolAlternatives.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CJKSymbolAlternatives, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureIdeographicAlternatives.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.IdeographicAlternatives, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureCJKVerticalRomanPlacement.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CJKVerticalRomanPlacement, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureItalicCJKRoman.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.ItalicCJKRoman, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureCaseSensitiveLayout.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CaseSensitiveLayout, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureAlternateKana.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.AlternateKana, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureStylisticAlternatives.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.StylisticAlternatives, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureContextualAlternates.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.ContextualAlternates, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureLowerCase.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.LowerCase, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureUpperCase.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.UpperCase, (int) featureSelector);
		}

		public CTFontDescriptor? WithFeature (CTFontFeatureCJKRomanSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CJKRomanSpacing, (int) featureSelector);
		}

		CTFontDescriptor? WithFeature (FontFeatureGroup featureGroup, int featureSelector)
		{
			using (NSNumber t = new NSNumber ((int) featureGroup), f = new NSNumber (featureSelector)) {
				return CreateDescriptor (CTFontDescriptorCreateCopyWithFeature (Handle, t.Handle, f.Handle));
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateMatchingFontDescriptors (IntPtr descriptor, IntPtr mandatoryAttributes);
		public CTFontDescriptor [] GetMatchingFontDescriptors (NSSet? mandatoryAttributes)
		{
			var cfArrayRef = CTFontDescriptorCreateMatchingFontDescriptors (Handle, mandatoryAttributes.GetHandle ());
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontDescriptor> ();
			return CFArray.ArrayFromHandleFunc (cfArrayRef, fd => new CTFontDescriptor (cfArrayRef, false), true)!;
		}

		public CTFontDescriptor? []? GetMatchingFontDescriptors (params NSString [] mandatoryAttributes)
		{
			NSSet attrs = NSSet.MakeNSObjectSet (mandatoryAttributes);
			return GetMatchingFontDescriptors (attrs);
		}

		public CTFontDescriptor? []? GetMatchingFontDescriptors ()
		{
			NSSet? attrs = null;
			return GetMatchingFontDescriptors (attrs);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateMatchingFontDescriptor (IntPtr descriptor, IntPtr mandatoryAttributes);
		public CTFontDescriptor? GetMatchingFontDescriptor (NSSet? mandatoryAttributes)
		{
			return CreateDescriptor (CTFontDescriptorCreateMatchingFontDescriptors (Handle, mandatoryAttributes.GetHandle ()));
		}

		public CTFontDescriptor? GetMatchingFontDescriptor (params NSString [] mandatoryAttributes)
		{
			NSSet attrs = NSSet.MakeNSObjectSet (mandatoryAttributes);
			return GetMatchingFontDescriptor (attrs);
		}

		public CTFontDescriptor? GetMatchingFontDescriptor ()
		{
			NSSet? attrs = null;
			return GetMatchingFontDescriptor (attrs);
		}
		#endregion

		#region Descriptor Accessors
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCopyAttributes (IntPtr descriptor);
		public CTFontDescriptorAttributes? GetAttributes ()
		{
			var cfDictRef = CTFontDescriptorCopyAttributes (Handle);
			var dict = Runtime.GetNSObject<NSDictionary> (cfDictRef, true);
			if (dict is null)
				return null;
			return new CTFontDescriptorAttributes (dict);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCopyAttribute (IntPtr descriptor, IntPtr attribute);
		public NSObject? GetAttribute (NSString attribute)
		{
			if (attribute is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (attribute));
			return Runtime.GetNSObject<NSObject> (CTFontDescriptorCopyAttribute (Handle, attribute.Handle), true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCopyLocalizedAttribute (IntPtr descriptor, IntPtr attribute, IntPtr language);
		public NSObject? GetLocalizedAttribute (NSString attribute)
		{
			return Runtime.GetNSObject<NSObject> (CTFontDescriptorCopyLocalizedAttribute (Handle, attribute.Handle, IntPtr.Zero), true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCopyLocalizedAttribute (IntPtr descriptor, IntPtr attribute, out IntPtr language);
		public NSObject? GetLocalizedAttribute (NSString attribute, out NSString? language)
		{
			var o = Runtime.GetNSObject<NSObject> (CTFontDescriptorCopyLocalizedAttribute (Handle, attribute.Handle, out var lang), true);
			language = Runtime.GetNSObject<NSString> (lang, true);
			return o;
		}
		#endregion
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static unsafe extern byte CTFontDescriptorMatchFontDescriptorsWithProgressHandler (IntPtr descriptors, IntPtr mandatoryAttributes, BlockLiteral* progressBlock);

		public delegate bool CTFontDescriptorProgressHandler (CTFontDescriptorMatchingState state, CTFontDescriptorMatchingProgress progress);

#if !NET
		delegate byte ct_font_desctiptor_progress_handler_t (IntPtr block, CTFontDescriptorMatchingState state, IntPtr progress);
		static ct_font_desctiptor_progress_handler_t static_MatchFontDescriptorsHandler = MatchFontDescriptorsHandler;

		[MonoPInvokeCallback (typeof (ct_font_desctiptor_progress_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static byte MatchFontDescriptorsHandler (IntPtr block, CTFontDescriptorMatchingState state, IntPtr progress)
		{
			var del = BlockLiteral.GetTarget<CTFontDescriptorProgressHandler> (block);
			if (del is not null) {
				var progressDictionary = Runtime.GetNSObject<NSDictionary> (progress)!;
				var strongDictionary = new CTFontDescriptorMatchingProgress (progressDictionary);
				var rv = del (state, strongDictionary);
				return rv ? (byte) 1 : (byte) 0;
			}
			return 0;
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public static bool MatchFontDescriptors (CTFontDescriptor [] descriptors, NSSet? mandatoryAttributes, CTFontDescriptorProgressHandler progressHandler)
		{
			if (descriptors is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptors));

			if (progressHandler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (progressHandler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, CTFontDescriptorMatchingState, IntPtr, byte> trampoline = &MatchFontDescriptorsHandler;
				using var block = new BlockLiteral (trampoline, progressHandler, typeof (CTFontDescriptor), nameof (MatchFontDescriptorsHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_MatchFontDescriptorsHandler, progressHandler);
#endif
				using var descriptorsArray = NSArray.FromNSObjects (descriptors);
				var rv = CTFontDescriptorMatchFontDescriptorsWithProgressHandler (descriptorsArray.GetHandle (), mandatoryAttributes.GetHandle (), &block);
				return rv != 0;
			}
		}

#if !XAMCORE_5_0
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'MatchFontDescriptors (CTFontDescriptor[], NSSet, CTFontDescriptorProgressHandler)' instead.")]
		public static bool MatchFontDescriptors (CTFontDescriptor [] descriptors, NSSet? mandatoryAttributes, Func<CTFontDescriptorMatchingState, IntPtr, bool> progressHandler)
		{
			throw new NotImplementedException ();
		}
#endif
	}
}
