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
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using Foundation;

namespace CoreText {

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontDescriptor.h
	public enum CTFontOrientation : uint {
		Default = 0,
		Horizontal = 1,
		Vertical = 2,
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontDescriptor.h
	public enum CTFontFormat : uint {
		Unrecognized        = 0,
		OpenTypePostScript  = 1,
		OpenTypeTrueType    = 2,
		TrueType            = 3,
		PostScript          = 4,
		Bitmap              = 5,
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFontDescriptor.h
	public enum CTFontPriority : uint {
		System       =  10000,
		Network      =  20000,
		Computer     =  30000,
		User         =  40000,
		Dynamic      =  50000,
		Process      =  60000,
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
	
	public static class CTFontDescriptorAttributeKey {

		public static readonly NSString Url;
		public static readonly NSString Name;
		public static readonly NSString DisplayName;
		public static readonly NSString FamilyName;
		public static readonly NSString StyleName;
		public static readonly NSString Traits;
		public static readonly NSString Variation;
		public static readonly NSString Size;
		public static readonly NSString Matrix;
		public static readonly NSString CascadeList;
		public static readonly NSString CharacterSet;
		public static readonly NSString Languages;
		public static readonly NSString BaselineAdjust;
		public static readonly NSString MacintoshEncodings;
		public static readonly NSString Features;
		public static readonly NSString FeatureSettings;
		public static readonly NSString FixedAdvance;
		public static readonly NSString FontOrientation;
		public static readonly NSString FontFormat;
		public static readonly NSString RegistrationScope;
		public static readonly NSString Priority;
		public static readonly NSString Enabled;

		static CTFontDescriptorAttributeKey ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreTextLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				Url                 = Dlfcn.GetStringConstant (handle, "kCTFontURLAttribute");
				Name                = Dlfcn.GetStringConstant (handle, "kCTFontNameAttribute");
				DisplayName         = Dlfcn.GetStringConstant (handle, "kCTFontDisplayNameAttribute");
				FamilyName          = Dlfcn.GetStringConstant (handle, "kCTFontFamilyNameAttribute");
				StyleName           = Dlfcn.GetStringConstant (handle, "kCTFontStyleNameAttribute");
				Traits              = Dlfcn.GetStringConstant (handle, "kCTFontTraitsAttribute");
				Variation           = Dlfcn.GetStringConstant (handle, "kCTFontVariationAttribute");
				Size                = Dlfcn.GetStringConstant (handle, "kCTFontSizeAttribute");
				Matrix              = Dlfcn.GetStringConstant (handle, "kCTFontMatrixAttribute");
				CascadeList         = Dlfcn.GetStringConstant (handle, "kCTFontCascadeListAttribute");
				CharacterSet        = Dlfcn.GetStringConstant (handle, "kCTFontCharacterSetAttribute");
				Languages           = Dlfcn.GetStringConstant (handle, "kCTFontLanguagesAttribute");
				BaselineAdjust      = Dlfcn.GetStringConstant (handle, "kCTFontBaselineAdjustAttribute");
				MacintoshEncodings  = Dlfcn.GetStringConstant (handle, "kCTFontMacintoshEncodingsAttribute");
				Features            = Dlfcn.GetStringConstant (handle, "kCTFontFeaturesAttribute");
				FeatureSettings     = Dlfcn.GetStringConstant (handle, "kCTFontFeatureSettingsAttribute");
				FixedAdvance        = Dlfcn.GetStringConstant (handle, "kCTFontFixedAdvanceAttribute");
				FontOrientation     = Dlfcn.GetStringConstant (handle, "kCTFontOrientationAttribute");
				FontFormat          = Dlfcn.GetStringConstant (handle, "kCTFontFormatAttribute");
				RegistrationScope   = Dlfcn.GetStringConstant (handle, "kCTFontRegistrationScopeAttribute");
				Priority            = Dlfcn.GetStringConstant (handle, "kCTFontPriorityAttribute");
				Enabled             = Dlfcn.GetStringConstant (handle, "kCTFontEnabledAttribute");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}
	}

	public class CTFontDescriptorAttributes {

		public CTFontDescriptorAttributes ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFontDescriptorAttributes (NSDictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException ("dictionary");
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary {get; private set;}

		public NSUrl Url {
			get {return (NSUrl) Dictionary [CTFontDescriptorAttributeKey.Url];}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Url, value);}
		}

		public string Name {
			get {return Adapter.GetStringValue (Dictionary, CTFontDescriptorAttributeKey.Name);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Name, value);}
		}

		public string DisplayName {
			get {return Adapter.GetStringValue (Dictionary, CTFontDescriptorAttributeKey.DisplayName);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.DisplayName, value);}
		}

		public string FamilyName {
			get {return Adapter.GetStringValue (Dictionary, CTFontDescriptorAttributeKey.FamilyName);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FamilyName, value);}
		}

		public string StyleName {
			get {return Adapter.GetStringValue (Dictionary, CTFontDescriptorAttributeKey.StyleName);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.StyleName, value);}
		}

		public CTFontTraits Traits {
			get {
				var traits = (NSDictionary) Dictionary [CTFontDescriptorAttributeKey.Traits];
				if (traits == null)
					return null;
				return new CTFontTraits (traits);
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Traits, 
						value == null ? null : value.Dictionary);
			}
		}

		public CTFontVariation Variation {
			get {
				var variation = (NSDictionary) Dictionary [CTFontDescriptorAttributeKey.Variation];
				return variation == null ? null : new CTFontVariation (variation);
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Variation,
						value == null ? null : value.Dictionary);
			}
		}

		// CFNumber
		public float? Size {
			get {return Adapter.GetSingleValue (Dictionary, CTFontDescriptorAttributeKey.Size);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Size, value);}
		}

		public unsafe CGAffineTransform? Matrix {
			get {
				var d = (NSData) Dictionary [CTFontDescriptorAttributeKey.Matrix];
				if (d == null)
					return null;
				return (CGAffineTransform) Marshal.PtrToStructure (d.Bytes, typeof (CGAffineTransform));
			}
			set {
				if (!value.HasValue)
					Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Matrix, (NSObject) null);
				else {
					byte[] data = new byte [Marshal.SizeOf (typeof (CGAffineTransform))];
					fixed (byte* p = data) {
						Marshal.StructureToPtr (value.Value, (IntPtr) p, false);
					}
					Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Matrix, NSData.FromArray (data));
				}
			}
		}

		public IEnumerable<CTFontDescriptor> CascadeList {
			get {
				return Adapter.GetNativeArray (Dictionary, CTFontDescriptorAttributeKey.CascadeList,
						d => new CTFontDescriptor (d, false));
			}
			set {Adapter.SetNativeValue (Dictionary, CTFontDescriptorAttributeKey.CascadeList, value);}
		}

		public NSCharacterSet CharacterSet {
			get {return (NSCharacterSet) Dictionary [CTFontDescriptorAttributeKey.CharacterSet];}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.CharacterSet, value);}
		}

		public IEnumerable<string> Languages {
			get {return Adapter.GetStringArray (Dictionary, CTFontDescriptorAttributeKey.Languages);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Languages, value);}
		}

		// float represented as a CFNumber
		public float? BaselineAdjust {
			get {return Adapter.GetSingleValue (Dictionary, CTFontDescriptorAttributeKey.BaselineAdjust);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.BaselineAdjust, value);}
		}

		public float? MacintoshEncodings {
			get {return Adapter.GetSingleValue (Dictionary, CTFontDescriptorAttributeKey.MacintoshEncodings);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.MacintoshEncodings, value);}
		}

		public IEnumerable<CTFontFeatures> Features {
			get {
				return Adapter.GetNativeArray (Dictionary, CTFontDescriptorAttributeKey.Features,
						d => new CTFontFeatures ((NSDictionary) Runtime.GetNSObject (d)));
			}
			set {
				List<CTFontFeatures> v;
				if (value == null || (v = new List<CTFontFeatures> (value)).Count == 0) {
					Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Features, (NSObject) null);
					return;
				}
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Features,
						NSArray.FromNSObjects (v.ConvertAll (e => (NSObject) e.Dictionary)));
			}
		}

		public IEnumerable<CTFontFeatureSettings> FeatureSettings {
			get {
				return Adapter.GetNativeArray (Dictionary, CTFontDescriptorAttributeKey.Features,
						d => new CTFontFeatureSettings ((NSDictionary) Runtime.GetNSObject (d)));
			}
			set {
				List<CTFontFeatureSettings> v;
				if (value == null || (v = new List<CTFontFeatureSettings> (value)).Count == 0) {
					Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Features, (NSObject) null);
					return;
				}

				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FeatureSettings,
						NSArray.FromNSObjects (v.ConvertAll (e => (NSObject) e.Dictionary)));
			}
		}

		// CFNumber
		public float? FixedAdvance {
			get {return Adapter.GetSingleValue (Dictionary, CTFontDescriptorAttributeKey.FixedAdvance);}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FixedAdvance, value);}
		}

		public CTFontOrientation? FontOrientation {
			get {
				var value = Adapter.GetUInt32Value (Dictionary, CTFontDescriptorAttributeKey.FontOrientation);
				return !value.HasValue ? null : (CTFontOrientation?) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FontOrientation, 
						value.HasValue ? (uint?) value.Value : null);
			}
		}

		public CTFontFormat? FontFormat {
			get {
				var value = Adapter.GetUInt32Value (Dictionary, CTFontDescriptorAttributeKey.FontFormat);
				return !value.HasValue ? null : (CTFontFormat?) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.FontFormat, 
						value.HasValue ? (uint?) value.Value : null);
			}
		}

#if XAMCORE_2_0
		public CTFontManagerScope? RegistrationScope {
			get {
				var value = Adapter.GetUnsignedIntegerValue (Dictionary, CTFontDescriptorAttributeKey.RegistrationScope);
				return !value.HasValue ? null : (CTFontManagerScope?) (ulong) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.RegistrationScope,
				value.HasValue ? (nuint?) (ulong) value.Value : null);
			}
		}
#else
		// TODO: docs mention CTFontManagerScope values, but I don't see any such enumeration.
		public NSNumber RegistrationScope {
			get {return (NSNumber) Dictionary [CTFontDescriptorAttributeKey.RegistrationScope];}
			set {Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.RegistrationScope, value);}
		}
#endif

		public CTFontPriority? Priority {
			get {
				var value = Adapter.GetUInt32Value (Dictionary, CTFontDescriptorAttributeKey.Priority);
				return !value.HasValue ? null : (CTFontPriority?) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Priority, 
						value.HasValue ? (uint?) value.Value : null);
			}
		}

		public bool Enabled {
			get {
				var value = (NSNumber) Dictionary [CTFontDescriptorAttributeKey.Enabled];
				if (value == null)
					return false;
				return value.Int32Value != 0;
			}
			set {
				Adapter.SetValue (Dictionary, CTFontDescriptorAttributeKey.Enabled, 
						value ? new NSNumber (1) : null);
			}
		}
	}

	public class CTFontDescriptor : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CTFontDescriptor (IntPtr handle)
			: this (handle, false)
		{
		}

		internal CTFontDescriptor (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw ConstructorError.ArgumentNull (this, "handle");
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}

		~CTFontDescriptor ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get {return handle;}
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

#region Descriptor Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateWithNameAndSize (IntPtr name, nfloat size);
		public CTFontDescriptor (string name, nfloat size)
		{
			if (name == null)
				throw ConstructorError.ArgumentNull (this, "name");
			using (CFString n = name)
				handle = CTFontDescriptorCreateWithNameAndSize (n.Handle, size);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateWithAttributes (IntPtr attributes);
		public CTFontDescriptor (CTFontDescriptorAttributes attributes)
		{
			if (attributes == null)
				throw ConstructorError.ArgumentNull (this, "attributes");
			handle = CTFontDescriptorCreateWithAttributes (attributes.Dictionary.Handle);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateCopyWithAttributes (IntPtr original, IntPtr attributes);
		public CTFontDescriptor WithAttributes (NSDictionary attributes)
		{
			if (attributes == null)
				throw new ArgumentNullException ("attributes");
			return CreateDescriptor (CTFontDescriptorCreateCopyWithAttributes (handle, attributes.Handle));
		}

		static CTFontDescriptor CreateDescriptor (IntPtr h)
		{
			if (h == IntPtr.Zero)
				return null;
			return new CTFontDescriptor (h, true);
		}

		public CTFontDescriptor WithAttributes (CTFontDescriptorAttributes attributes)
		{
			if (attributes == null)
				throw new ArgumentNullException ("attributes");
			return CreateDescriptor (CTFontDescriptorCreateCopyWithAttributes (handle, attributes.Dictionary.Handle));
		}

		// TODO: is there a better type to use for variationIdentifier?  
		// uint perhaps?  "This is the four character code of the variation axis"
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateCopyWithVariation (IntPtr original, IntPtr variationIdentifier, nfloat variationValue);
		public CTFontDescriptor WithVariation (uint variationIdentifier, nfloat variationValue)
		{
			using (var id = new NSNumber (variationIdentifier))
				return CreateDescriptor (CTFontDescriptorCreateCopyWithVariation  (handle, 
							id.Handle, variationValue));
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateCopyWithFeature (IntPtr original, IntPtr featureTypeIdentifier, IntPtr featureSelectorIdentifier);

#if !XAMCORE_2_0
		[Advice ("Use 'WithFeature' with specific selector.")]
		public CTFontDescriptor WithFeature (NSNumber featureTypeIdentifier, NSNumber featureSelectorIdentifier)
		{
			if (featureTypeIdentifier == null)
				throw new ArgumentNullException ("featureTypeIdentifier");
			if (featureSelectorIdentifier == null)
				throw new ArgumentNullException ("featureSelectorIdentifier");
			return CreateDescriptor (CTFontDescriptorCreateCopyWithFeature (handle, featureTypeIdentifier.Handle, featureSelectorIdentifier.Handle));
		}
#endif

		public CTFontDescriptor WithFeature (CTFontFeatureAllTypographicFeatures.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.AllTypographicFeatures, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureLigatures.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Ligatures, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureCursiveConnection.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CursiveConnection, (int) featureSelector);
		}

#if !XAMCORE_2_0
		[Obsolete]
		public CTFontDescriptor WithFeature (CTFontFeatureLetterCase.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.LetterCase, (int) featureSelector);
		}
#endif

		public CTFontDescriptor WithFeature (CTFontFeatureVerticalSubstitutionConnection.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.VerticalSubstitution, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureLinguisticRearrangementConnection.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.LinguisticRearrangement, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureNumberSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.NumberSpacing, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureSmartSwash.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.SmartSwash, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureDiacritics.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Diacritics, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureVerticalPosition.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.VerticalPosition, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureFractions.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Fractions, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureOverlappingCharacters.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.OverlappingCharacters, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureTypographicExtras.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.TypographicExtras, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureMathematicalExtras.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.MathematicalExtras, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureOrnamentSets.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.OrnamentSets, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureCharacterAlternatives.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CharacterAlternatives, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureDesignComplexity.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.DesignComplexity, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureStyleOptions.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.StyleOptions, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureCharacterShape.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CharacterShape, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureNumberCase.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.NumberCase, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureTextSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.TextSpacing, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureTransliteration.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Transliteration, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureAnnotation.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.Annotation, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureKanaSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.KanaSpacing, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureIdeographicSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.IdeographicSpacing, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureUnicodeDecomposition.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.UnicodeDecomposition, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureRubyKana.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.RubyKana, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureCJKSymbolAlternatives.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CJKSymbolAlternatives, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureIdeographicAlternatives.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.IdeographicAlternatives, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureCJKVerticalRomanPlacement.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CJKVerticalRomanPlacement, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureItalicCJKRoman.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.ItalicCJKRoman, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureCaseSensitiveLayout.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CaseSensitiveLayout, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureAlternateKana.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.AlternateKana, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureStylisticAlternatives.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.StylisticAlternatives, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureContextualAlternates.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.ContextualAlternates, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureLowerCase.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.LowerCase, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureUpperCase.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.UpperCase, (int) featureSelector);
		}

		public CTFontDescriptor WithFeature (CTFontFeatureCJKRomanSpacing.Selector featureSelector)
		{
			return WithFeature (FontFeatureGroup.CJKRomanSpacing, (int) featureSelector);
		}

		CTFontDescriptor WithFeature (FontFeatureGroup featureGroup, int featureSelector)
		{
			using (NSNumber t = new NSNumber ((int) featureGroup), f = new NSNumber (featureSelector)) {
				return CreateDescriptor (CTFontDescriptorCreateCopyWithFeature (handle, t.Handle, f.Handle));
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateMatchingFontDescriptors (IntPtr descriptor, IntPtr mandatoryAttributes);
		public CTFontDescriptor[] GetMatchingFontDescriptors (NSSet mandatoryAttributes)
		{
			var cfArrayRef = CTFontDescriptorCreateMatchingFontDescriptors (handle, 
						mandatoryAttributes == null ? IntPtr.Zero : mandatoryAttributes.Handle);
			if (cfArrayRef == IntPtr.Zero)
				return new CTFontDescriptor [0];
			var matches = NSArray.ArrayFromHandle (cfArrayRef,
					fd => new CTFontDescriptor (cfArrayRef, false));
			CFObject.CFRelease (cfArrayRef);
			return matches;
		}

		public CTFontDescriptor[] GetMatchingFontDescriptors (params NSString[] mandatoryAttributes)
		{
			NSSet attrs = NSSet.MakeNSObjectSet (mandatoryAttributes);
			return GetMatchingFontDescriptors (attrs);
		}

		public CTFontDescriptor[] GetMatchingFontDescriptors ()
		{
			NSSet attrs = null;
			return GetMatchingFontDescriptors (attrs);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCreateMatchingFontDescriptor (IntPtr descriptor, IntPtr mandatoryAttributes);
		public CTFontDescriptor GetMatchingFontDescriptor (NSSet mandatoryAttributes)
		{
			return CreateDescriptor (CTFontDescriptorCreateMatchingFontDescriptors (handle, 
						mandatoryAttributes == null ? IntPtr.Zero : mandatoryAttributes.Handle));
		}

		public CTFontDescriptor GetMatchingFontDescriptor (params NSString[] mandatoryAttributes)
		{
			NSSet attrs = NSSet.MakeNSObjectSet (mandatoryAttributes);
			return GetMatchingFontDescriptor (attrs);
		}

		public CTFontDescriptor GetMatchingFontDescriptor ()
		{
			NSSet attrs = null;
			return GetMatchingFontDescriptor (attrs);
		}
#endregion

#region Descriptor Accessors
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCopyAttributes (IntPtr descriptor);
		public CTFontDescriptorAttributes GetAttributes()
		{
			var cfDictRef = CTFontDescriptorCopyAttributes (handle);
			if (cfDictRef == IntPtr.Zero)
				return null;
			var dict = (NSDictionary) Runtime.GetNSObject (cfDictRef);
			dict.DangerousRelease ();
			return new CTFontDescriptorAttributes (dict);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCopyAttribute (IntPtr descriptor, IntPtr attribute);
		public NSObject GetAttribute (NSString attribute)
		{
			if (attribute == null)
				throw new ArgumentNullException ("attribute");
			var obj = Runtime.GetNSObject (CTFontDescriptorCopyAttribute (handle, attribute.Handle));
			obj.DangerousRelease ();
			return obj;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCopyLocalizedAttribute (IntPtr descriptor, IntPtr attribute, IntPtr language);
		public NSObject GetLocalizedAttribute (NSString attribute)
		{
			var obj = Runtime.GetNSObject (CTFontDescriptorCopyLocalizedAttribute (handle, attribute.Handle, IntPtr.Zero));
			obj.DangerousRelease ();
			return obj;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontDescriptorCopyLocalizedAttribute (IntPtr descriptor, IntPtr attribute, out IntPtr language);
		public NSObject GetLocalizedAttribute (NSString attribute, out NSString language)
		{
			IntPtr lang;
			var o = Runtime.GetNSObject (CTFontDescriptorCopyLocalizedAttribute (handle, attribute.Handle, out lang));
			o.DangerousRelease ();
			language = (NSString) Runtime.GetNSObject (lang);
			if (lang != IntPtr.Zero)
				CFObject.CFRelease (lang);
			return o;
		}
#endregion
		[Mac (10,9)][iOS (6,0)]
		[DllImport (Constants.CoreTextLibrary)]
		static extern bool CTFontDescriptorMatchFontDescriptorsWithProgressHandler (IntPtr descriptors, IntPtr mandatoryAttributes,
			Func<CTFontDescriptorMatchingState, IntPtr, bool> progressHandler);

		[Mac (10,9)][iOS (6,0)]
		public static bool MatchFontDescriptors (CTFontDescriptor[] descriptors, NSSet mandatoryAttributes, Func<CTFontDescriptorMatchingState, IntPtr, bool> progressHandler)
		{
			// FIXME: the P/Invoke used below is wrong, it expects a block, not a function pointer.
			// throwing a NIE instead of crashing until this is implemented properly.
			throw new NotImplementedException ();
//			var ma = mandatoryAttributes == null ? IntPtr.Zero : mandatoryAttributes.Handle;
//			// FIXME: SIGSEGV probably due to mandatoryAttributes mismatch
//			using (var ar = CFArray.FromNativeObjects (descriptors)) {
//				return CTFontDescriptorMatchFontDescriptorsWithProgressHandler (ar.Handle, ma, progressHandler);
//			}
		}
	}
}
