// 
// CTFont.cs: Implements the managed CTFont
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
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using Foundation;

using CGGlyph = System.UInt16;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

	[Flags]
	[Native]
	// defined as CFOptionFlags (unsigned long [long] = nuint) - /System/Library/Frameworks/CoreText.framework/Headers/CTFont.h
	public enum CTFontOptions : ulong {
		Default = 0,
		PreventAutoActivation = 1 << 0,
#if NET
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
#else
		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Mac (13, 0), Watch (9, 0)]
#endif
		PreventAutoDownload = 1 << 1,
		PreferSystemFont = 1 << 2,
#if !NET
		[Obsolete ("This API is not available on this platform.")]
		IncludeDisabled = 1 << 7,
#endif
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFont.h
	public enum CTFontUIFontType : uint {
		None = unchecked((uint) (-1)),
		User = 0,
		UserFixedPitch = 1,
		System = 2,
		EmphasizedSystem = 3,
		SmallSystem = 4,
		SmallEmphasizedSystem = 5,
		MiniSystem = 6,
		MiniEmphasizedSystem = 7,
		Views = 8,
		Application = 9,
		Label = 10,
		MenuTitle = 11,
		MenuItem = 12,
		MenuItemMark = 13,
		MenuItemCmdKey = 14,
		WindowTitle = 15,
		PushButton = 16,
		UtilityWindowTitle = 17,
		AlertHeader = 18,
		SystemDetail = 19,
		EmphasizedSystemDetail = 20,
		Toolbar = 21,
		SmallToolbar = 22,
		Message = 23,
		Palette = 24,
		ToolTip = 25,
		ControlContent = 26,
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFont.h
	public enum CTFontTable : uint {
		BaselineBASE = 0x42415345,  // 'BASE'
		ColorBitmapData = 0x43424454,  // 'CBDT'
		ColorBitmapLocationData = 0x43424c43,  // 'CBLC'
		PostscriptFontProgram = 0x43464620,  // 'CFF '
		CompactFontFormat2 = 0x43464632,  // 'CFF2'
		ColorTable = 0x434f4c52,  // 'COLR'
		ColorPaletteTable = 0x4350414c,  // 'CPAL'
		DigitalSignature = 0x44534947,  // 'DSIG'
		EmbeddedBitmap = 0x45424454,  // 'EBDT'
		EmbeddedBitmapLocation = 0x45424c43,  // 'EBLC'
		EmbeddedBitmapScaling = 0x45425343,  // 'EBSC'
		GlyphDefinition = 0x47444546,  // 'GDEF'
		GlyphPositioning = 0x47504f53,  // 'GPOS'
		GlyphSubstitution = 0x47535542,  // 'GSUB'
		HorizontalMetricsVariations = 0x48564152,  // 'HVAR'
		JustificationJSTF = 0x4a535446,  // 'JSTF'
		LinearThreshold = 0x4c545348,  // 'LTSH'
		MathLayoutData = 0x4d415448,  // 'MATH'
		Merge = 0x4d455247,  // 'MERG'
		MetricsVariations = 0x4d564152,  // 'MVAR'
		WindowsSpecificMetrics = 0x4f532f32,  // 'OS2 '
		Pcl5Data = 0x50434c54,  // 'PCLT'
		VerticalDeviceMetrics = 0x56444d58,  // 'VDMX'
		StyleAttributes = 0x53544154,  // 'STAT'
		ScalableVectorGraphics = 0x53564720,  // 'SVG '
		VerticalOrigin = 0x564f5247,  // 'VORG'
		VerticalMetricsVariations = 0x56564152,  // 'VVAR'
		GlyphReference = 0x5a617066,  // 'Zapf'
		AccentAttachment = 0x61636e74,  // 'Acnt'
		AnchorPoints = 0x616e6b72,  // 'ankr'
		AxisVariation = 0x61766172,  // 'Avar'
		BitmapData = 0x62646174,  // 'Bdat'
		BitmapFontHeader = 0x62686564,  // 'Bhed'
		BitmapLocation = 0x626c6f63,  // 'Bloc'
		BaselineBsln = 0x62736c6e,  // 'Bsln'
		CharacterToGlyphMapping = 0x636d6170,  // 'Cmap'
		ControlValueTableVariation = 0x63766172,  // 'Cvar'
		ControlValueTable = 0x63767420,  // 'Cvt '
		FontDescriptor = 0x66647363,  // 'Fdsc'
		LayoutFeature = 0x66656174,  // 'Feat'
		FontMetrics = 0x666d7478,  // 'Fmtx'
		FondAndNfntData = 0x666f6e64,  // 'fond'
		FontProgram = 0x6670676d,  // 'Fpgm'
		FontVariation = 0x66766172,  // 'Fvar'
		GridFitting = 0x67617370,  // 'Gasp'
		GlyphData = 0x676c7966,  // 'Glyf'
		GlyphVariation = 0x67766172,  // 'Gvar'
		HorizontalDeviceMetrics = 0x68646d78,  // 'Hdmx'
		FontHeader = 0x68656164,  // 'Head'
		HorizontalHeader = 0x68686561,  // 'Hhea'
		HorizontalMetrics = 0x686d7478,  // 'Hmtx'
		HorizontalStyle = 0x68737479,  // 'Hsty'
		JustificationJust = 0x6a757374,  // 'Just'
		Kerning = 0x6b65726e,  // 'Kern'
		ExtendedKerning = 0x6b657278,  // 'Kerx'
		LigatureCaret = 0x6c636172,  // 'Lcar'
		IndexToLocation = 0x6c6f6361,  // 'Loca'
		LanguageTags = 0x6c746167,  // 'ltag'
		MaximumProfile = 0x6d617870,  // 'Maxp'
		Metadata = 0x6d657461,  // 'meta'
		Morph = 0x6d6f7274,  // 'Mort'
		ExtendedMorph = 0x6d6f7278,  // 'Morx'
		Name = 0x6e616d65,  // 'Name'
		OpticalBounds = 0x6f706264,  // 'Opbd'
		PostScriptInformation = 0x706f7374,  // 'Post'
		ControlValueTableProgram = 0x70726570,  // 'Prep'
		Properties = 0x70726f70,  // 'Prop'
		SBitmapData = 0x73626974,  // 'sbit'
		SExtendedBitmapData = 0x73626978,  // 'sbix'
		Tracking = 0x7472616b,  // 'Trak'
		VerticalHeader = 0x76686561,  // 'Vhea'
		VerticalMetrics = 0x766d7478,  // 'Vmtx'
		CrossReference = 0x78726566,  // 'xref'
	}

	[Flags]
	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFont.h
	public enum CTFontTableOptions : uint {
		None = 0,
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios6.0")]
		[ObsoletedOSPlatform ("maccatalyst13.1")]
		[ObsoletedOSPlatform ("macos10.8")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.WatchOS, 9, 0)]
#endif
		ExcludeSynthetic = (1 << 0),
	}

	// anonymous and typeless native enum - /System/Library/Frameworks/CoreText.framework/Headers/SFNTLayoutTypes.h
	public enum FontFeatureGroup {
		AllTypographicFeatures = 0,
		Ligatures = 1,
		CursiveConnection = 2,
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.7")]
		[ObsoletedOSPlatform ("ios6.0")]
#else
		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
		LetterCase = 3,
		VerticalSubstitution = 4,
		LinguisticRearrangement = 5,
		NumberSpacing = 6,
		SmartSwash = 8,
		Diacritics = 9,
		VerticalPosition = 10,
		Fractions = 11,
		OverlappingCharacters = 13,
		TypographicExtras = 14,
		MathematicalExtras = 15,
		OrnamentSets = 16,
		CharacterAlternatives = 17,
		DesignComplexity = 18,
		StyleOptions = 19,
		CharacterShape = 20,
		NumberCase = 21,
		TextSpacing = 22,
		Transliteration = 23,
		Annotation = 24,
		KanaSpacing = 25,
		IdeographicSpacing = 26,
		UnicodeDecomposition = 27,
		RubyKana = 28,
		CJKSymbolAlternatives = 29,
		IdeographicAlternatives = 30,
		CJKVerticalRomanPlacement = 31,
		ItalicCJKRoman = 32,
		CaseSensitiveLayout = 33,
		AlternateKana = 34,
		StylisticAlternatives = 35,
		ContextualAlternates = 36,
		LowerCase = 37,
		UpperCase = 38,
		CJKRomanSpacing = 103
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatures {

		public CTFontFeatures ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFontFeatures (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary { get; private set; }

		public string? Name {
			get { return Adapter.GetStringValue (Dictionary, CTFontFeatureKey.Name); }
			set { Adapter.SetValue (Dictionary, CTFontFeatureKey.Name, value); }
		}

		public FontFeatureGroup FeatureGroup {
			get {
				return (FontFeatureGroup) (int) (NSNumber) Dictionary [CTFontFeatureKey.Identifier];
			}
		}

		public bool Exclusive {
			get {
				return CFDictionary.GetBooleanValue (Dictionary.Handle,
						CTFontFeatureKey.Exclusive.Handle);
			}
			set {
				CFMutableDictionary.SetValue (Dictionary.Handle,
						CTFontFeatureKey.Exclusive.Handle,
						value);
			}
		}

		public IEnumerable<CTFontFeatureSelectors>? Selectors {
			get {
				return Adapter.GetNativeArray (Dictionary, CTFontFeatureKey.Selectors,
						d => CTFontFeatureSelectors.Create (FeatureGroup, Runtime.GetNSObject<NSDictionary> (d)!));
			}
			set {
				List<CTFontFeatureSelectors> v;
				if (value is null || (v = new List<CTFontFeatureSelectors> (value)).Count == 0) {
					Adapter.SetValue (Dictionary, CTFontFeatureKey.Selectors, (NSObject?) null);
					return;
				}
				Adapter.SetValue (Dictionary, CTFontFeatureKey.Selectors,
						NSArray.FromNSObjects ((IList<NSObject>) v.ConvertAll (e => (NSObject) e.Dictionary)));
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureSelectors {

		public CTFontFeatureSelectors ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFontFeatureSelectors (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		internal static CTFontFeatureSelectors Create (FontFeatureGroup featureGroup, NSDictionary dictionary)
		{
			switch (featureGroup) {
			case FontFeatureGroup.AllTypographicFeatures:
				return new CTFontFeatureAllTypographicFeatures (dictionary);
			case FontFeatureGroup.Ligatures:
				return new CTFontFeatureLigatures (dictionary);
			case FontFeatureGroup.CursiveConnection:
				return new CTFontFeatureCursiveConnection (dictionary);
#pragma warning disable 618
			case FontFeatureGroup.LetterCase:
				return new CTFontFeatureLetterCase (dictionary);
#pragma warning restore 618
			case FontFeatureGroup.VerticalSubstitution:
				return new CTFontFeatureVerticalSubstitutionConnection (dictionary);
			case FontFeatureGroup.LinguisticRearrangement:
				return new CTFontFeatureLinguisticRearrangementConnection (dictionary);
			case FontFeatureGroup.NumberSpacing:
				return new CTFontFeatureNumberSpacing (dictionary);
			case FontFeatureGroup.SmartSwash:
				return new CTFontFeatureSmartSwash (dictionary);
			case FontFeatureGroup.Diacritics:
				return new CTFontFeatureDiacritics (dictionary);
			case FontFeatureGroup.VerticalPosition:
				return new CTFontFeatureVerticalPosition (dictionary);
			case FontFeatureGroup.Fractions:
				return new CTFontFeatureFractions (dictionary);
			case FontFeatureGroup.OverlappingCharacters:
				return new CTFontFeatureOverlappingCharacters (dictionary);
			case FontFeatureGroup.TypographicExtras:
				return new CTFontFeatureTypographicExtras (dictionary);
			case FontFeatureGroup.MathematicalExtras:
				return new CTFontFeatureMathematicalExtras (dictionary);
			case FontFeatureGroup.OrnamentSets:
				return new CTFontFeatureOrnamentSets (dictionary);
			case FontFeatureGroup.CharacterAlternatives:
				return new CTFontFeatureCharacterAlternatives (dictionary);
			case FontFeatureGroup.DesignComplexity:
				return new CTFontFeatureDesignComplexity (dictionary);
			case FontFeatureGroup.StyleOptions:
				return new CTFontFeatureStyleOptions (dictionary);
			case FontFeatureGroup.CharacterShape:
				return new CTFontFeatureCharacterShape (dictionary);
			case FontFeatureGroup.NumberCase:
				return new CTFontFeatureNumberCase (dictionary);
			case FontFeatureGroup.TextSpacing:
				return new CTFontFeatureTextSpacing (dictionary);
			case FontFeatureGroup.Transliteration:
				return new CTFontFeatureTransliteration (dictionary);
			case FontFeatureGroup.Annotation:
				return new CTFontFeatureAnnotation (dictionary);
			case FontFeatureGroup.KanaSpacing:
				return new CTFontFeatureKanaSpacing (dictionary);
			case FontFeatureGroup.IdeographicSpacing:
				return new CTFontFeatureIdeographicSpacing (dictionary);
			case FontFeatureGroup.UnicodeDecomposition:
				return new CTFontFeatureUnicodeDecomposition (dictionary);
			case FontFeatureGroup.RubyKana:
				return new CTFontFeatureRubyKana (dictionary);
			case FontFeatureGroup.CJKSymbolAlternatives:
				return new CTFontFeatureCJKSymbolAlternatives (dictionary);
			case FontFeatureGroup.IdeographicAlternatives:
				return new CTFontFeatureIdeographicAlternatives (dictionary);
			case FontFeatureGroup.CJKVerticalRomanPlacement:
				return new CTFontFeatureCJKVerticalRomanPlacement (dictionary);
			case FontFeatureGroup.ItalicCJKRoman:
				return new CTFontFeatureItalicCJKRoman (dictionary);
			case FontFeatureGroup.CaseSensitiveLayout:
				return new CTFontFeatureCaseSensitiveLayout (dictionary);
			case FontFeatureGroup.AlternateKana:
				return new CTFontFeatureAlternateKana (dictionary);
			case FontFeatureGroup.StylisticAlternatives:
				return new CTFontFeatureStylisticAlternatives (dictionary);
			case FontFeatureGroup.ContextualAlternates:
				return new CTFontFeatureContextualAlternates (dictionary);
			case FontFeatureGroup.LowerCase:
				return new CTFontFeatureLowerCase (dictionary);
			case FontFeatureGroup.UpperCase:
				return new CTFontFeatureUpperCase (dictionary);
			case FontFeatureGroup.CJKRomanSpacing:
				return new CTFontFeatureCJKRomanSpacing (dictionary);
			default:
				return new CTFontFeatureSelectors (dictionary);
			}
		}

		public NSDictionary Dictionary { get; private set; }

		protected int FeatureWeak {
			get {
				return (int) (NSNumber) Dictionary [CTFontFeatureSelectorKey.Identifier];
			}
		}

		public string? Name {
			get { return Adapter.GetStringValue (Dictionary, CTFontFeatureSelectorKey.Name); }
			set { Adapter.SetValue (Dictionary, CTFontFeatureSelectorKey.Name, value); }
		}

		public bool Default {
			get {
				return CFDictionary.GetBooleanValue (Dictionary.Handle,
						CTFontFeatureSelectorKey.Default.Handle);
			}
			set {
				CFMutableDictionary.SetValue (Dictionary.Handle,
						CTFontFeatureSelectorKey.Default.Handle,
						value);
			}
		}

		public bool Setting {
			get {
				return CFDictionary.GetBooleanValue (Dictionary.Handle,
						CTFontFeatureSelectorKey.Setting.Handle);
			}
			set {
				CFMutableDictionary.SetValue (Dictionary.Handle,
						CTFontFeatureSelectorKey.Setting.Handle,
						value);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureAllTypographicFeatures : CTFontFeatureSelectors {
		public enum Selector {
			AllTypeFeaturesOn = 0,
			AllTypeFeaturesOff = 1
		}

		public CTFontFeatureAllTypographicFeatures (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureLigatures : CTFontFeatureSelectors {
		public enum Selector {
			RequiredLigaturesOn = 0,
			RequiredLigaturesOff = 1,
			CommonLigaturesOn = 2,
			CommonLigaturesOff = 3,
			RareLigaturesOn = 4,
			RareLigaturesOff = 5,
			LogosOn = 6,
			LogosOff = 7,
			RebusPicturesOn = 8,
			RebusPicturesOff = 9,
			DiphthongLigaturesOn = 10,
			DiphthongLigaturesOff = 11,
			SquaredLigaturesOn = 12,
			SquaredLigaturesOff = 13,
			AbbrevSquaredLigaturesOn = 14,
			AbbrevSquaredLigaturesOff = 15,
			SymbolLigaturesOn = 16,
			SymbolLigaturesOff = 17,
			ContextualLigaturesOn = 18,
			ContextualLigaturesOff = 19,
			HistoricalLigaturesOn = 20,
			HistoricalLigaturesOff = 21
		}

		public CTFontFeatureLigatures (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[ObsoletedOSPlatform ("macos10.7")]
	[ObsoletedOSPlatform ("ios6.0")]
#else
	[Deprecated (PlatformName.iOS, 6, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 7)]
#endif
	public class CTFontFeatureLetterCase : CTFontFeatureSelectors {
		public enum Selector {
			UpperAndLowerCase = 0,
			AllCaps = 1,
			AllLowerCase = 2,
			SmallCaps = 3,
			InitialCaps = 4,
			InitialCapsAndSmallCaps = 5
		}

		public CTFontFeatureLetterCase (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureCursiveConnection : CTFontFeatureSelectors {
		public enum Selector {
			Unconnected = 0,
			PartiallyConnected = 1,
			Cursive = 2
		}

		public CTFontFeatureCursiveConnection (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureVerticalSubstitutionConnection : CTFontFeatureSelectors {
		public enum Selector {
			SubstituteVerticalFormsOn = 0,
			SubstituteVerticalFormsOff = 1
		}

		public CTFontFeatureVerticalSubstitutionConnection (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureLinguisticRearrangementConnection : CTFontFeatureSelectors {
		public enum Selector {
			LinguisticRearrangementOn = 0,
			LinguisticRearrangementOff = 1
		}

		public CTFontFeatureLinguisticRearrangementConnection (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureNumberSpacing : CTFontFeatureSelectors {
		public enum Selector {
			MonospacedNumbers = 0,
			ProportionalNumbers = 1,
			ThirdWidthNumbers = 2,
			QuarterWidthNumbers = 3
		}

		public CTFontFeatureNumberSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureSmartSwash : CTFontFeatureSelectors {
		public enum Selector {
			WordInitialSwashesOn = 0,
			WordInitialSwashesOff = 1,
			WordFinalSwashesOn = 2,
			WordFinalSwashesOff = 3,
			LineInitialSwashesOn = 4,
			LineInitialSwashesOff = 5,
			LineFinalSwashesOn = 6,
			LineFinalSwashesOff = 7,
			NonFinalSwashesOn = 8,
			NonFinalSwashesOff = 9
		}

		public CTFontFeatureSmartSwash (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureDiacritics : CTFontFeatureSelectors {
		public enum Selector {
			ShowDiacritics = 0,
			HideDiacritics = 1,
			DecomposeDiacritics = 2
		}

		public CTFontFeatureDiacritics (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureVerticalPosition : CTFontFeatureSelectors {
		public enum Selector {
			NormalPosition = 0,
			Superiors = 1,
			Inferiors = 2,
			Ordinals = 3,
			ScientificInferiors = 4
		}

		public CTFontFeatureVerticalPosition (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureFractions : CTFontFeatureSelectors {
		public enum Selector {
			NoFractions = 0,
			VerticalFractions = 1,
			DiagonalFractions = 2
		}

		public CTFontFeatureFractions (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureOverlappingCharacters : CTFontFeatureSelectors {
		public enum Selector {
			PreventOverlapOn = 0,
			PreventOverlapOff = 1
		}

		public CTFontFeatureOverlappingCharacters (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureTypographicExtras : CTFontFeatureSelectors {
		public enum Selector {
			HyphensToEmDashOn = 0,
			HyphensToEmDashOff = 1,
			HyphenToEnDashOn = 2,
			HyphenToEnDashOff = 3,
			SlashedZeroOn = 4,
			SlashedZeroOff = 5,
			FormInterrobangOn = 6,
			FormInterrobangOff = 7,
			SmartQuotesOn = 8,
			SmartQuotesOff = 9,
			PeriodsToEllipsisOn = 10,
			PeriodsToEllipsisOff = 11
		}

		public CTFontFeatureTypographicExtras (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureMathematicalExtras : CTFontFeatureSelectors {
		public enum Selector {
			HyphenToMinusOn = 0,
			HyphenToMinusOff = 1,
			AsteriskToMultiplyOn = 2,
			AsteriskToMultiplyOff = 3,
			SlashToDivideOn = 4,
			SlashToDivideOff = 5,
			InequalityLigaturesOn = 6,
			InequalityLigaturesOff = 7,
			ExponentsOn = 8,
			ExponentsOff = 9,
			MathematicalGreekOn = 10,
			MathematicalGreekOff = 11
		}

		public CTFontFeatureMathematicalExtras (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureOrnamentSets : CTFontFeatureSelectors {
		public enum Selector {
			NoOrnaments = 0,
			Dingbats = 1,
			PiCharacters = 2,
			Fleurons = 3,
			DecorativeBorders = 4,
			InternationalSymbols = 5,
			MathSymbols = 6
		}

		public CTFontFeatureOrnamentSets (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureCharacterAlternatives : CTFontFeatureSelectors {
		public enum Selector {
			NoAlternates = 0,
		}

		public CTFontFeatureCharacterAlternatives (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureDesignComplexity : CTFontFeatureSelectors {
		public enum Selector {
			DesignLevel1 = 0,
			DesignLevel2 = 1,
			DesignLevel3 = 2,
			DesignLevel4 = 3,
			DesignLevel5 = 4
		}

		public CTFontFeatureDesignComplexity (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureStyleOptions : CTFontFeatureSelectors {
		public enum Selector {
			NoStyleOptions = 0,
			DisplayText = 1,
			EngravedText = 2,
			IlluminatedCaps = 3,
			TitlingCaps = 4,
			TallCaps = 5
		}

		public CTFontFeatureStyleOptions (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureCharacterShape : CTFontFeatureSelectors {
		public enum Selector {
			TraditionalCharacters = 0,
			SimplifiedCharacters = 1,
			JIS1978Characters = 2,
			JIS1983Characters = 3,
			JIS1990Characters = 4,
			TraditionalAltOne = 5,
			TraditionalAltTwo = 6,
			TraditionalAltThree = 7,
			TraditionalAltFour = 8,
			TraditionalAltFive = 9,
			ExpertCharacters = 10,
			JIS2004Characters = 11,
			HojoCharacters = 12,
			NLCCharacters = 13,
			TraditionalNamesCharacters = 14
		}

		public CTFontFeatureCharacterShape (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureNumberCase : CTFontFeatureSelectors {
		public enum Selector {
			LowerCaseNumbers = 0,
			UpperCaseNumbers = 1
		}

		public CTFontFeatureNumberCase (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureTextSpacing : CTFontFeatureSelectors {
		public enum Selector {
			ProportionalText = 0,
			MonospacedText = 1,
			HalfWidthText = 2,
			ThirdWidthText = 3,
			QuarterWidthText = 4,
			AltProportionalText = 5,
			AltHalfWidthText = 6
		}

		public CTFontFeatureTextSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureTransliteration : CTFontFeatureSelectors {
		public enum Selector {
			NoTransliteration = 0,
			HanjaToHangul = 1,
			HiraganaToKatakana = 2,
			KatakanaToHiragana = 3,
			KanaToRomanization = 4,
			RomanizationToHiragana = 5,
			RomanizationToKatakana = 6,
			HanjaToHangulAltOne = 7,
			HanjaToHangulAltTwo = 8,
			HanjaToHangulAltThree = 9
		}

		public CTFontFeatureTransliteration (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureAnnotation : CTFontFeatureSelectors {
		public enum Selector {
			NoAnnotation = 0,
			BoxAnnotation = 1,
			RoundedBoxAnnotation = 2,
			CircleAnnotation = 3,
			InvertedCircleAnnotation = 4,
			ParenthesisAnnotation = 5,
			PeriodAnnotation = 6,
			RomanNumeralAnnotation = 7,
			DiamondAnnotation = 8,
			InvertedBoxAnnotation = 9,
			InvertedRoundedBoxAnnotation = 10
		}

		public CTFontFeatureAnnotation (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureKanaSpacing : CTFontFeatureSelectors {
		public enum Selector {
			FullWidthKana = 0,
			ProportionalKana = 1
		}

		public CTFontFeatureKanaSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureIdeographicSpacing : CTFontFeatureSelectors {
		public enum Selector {
			FullWidthIdeographs = 0,
			ProportionalIdeographs = 1,
			HalfWidthIdeographs = 2
		}

		public CTFontFeatureIdeographicSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureUnicodeDecomposition : CTFontFeatureSelectors {
		public enum Selector {
			CanonicalCompositionOn = 0,
			CanonicalCompositionOff = 1,
			CompatibilityCompositionOn = 2,
			CompatibilityCompositionOff = 3,
			TranscodingCompositionOn = 4,
			TranscodingCompositionOff = 5
		}

		public CTFontFeatureUnicodeDecomposition (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureRubyKana : CTFontFeatureSelectors {
		public enum Selector {
#if NET
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[UnsupportedOSPlatform ("tvos")]
			[UnsupportedOSPlatform ("maccatalyst")]
			[ObsoletedOSPlatform ("macos10.8")]
			[ObsoletedOSPlatform ("ios5.1")]
#else
			[Deprecated (PlatformName.iOS, 5, 1)]
			[Deprecated (PlatformName.MacOSX, 10, 8)]
#endif
			NoRubyKana = 0,
#if NET
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[UnsupportedOSPlatform ("tvos")]
			[UnsupportedOSPlatform ("maccatalyst")]
			[ObsoletedOSPlatform ("macos10.8")]
			[ObsoletedOSPlatform ("ios5.1")]
#else
			[Deprecated (PlatformName.iOS, 5, 1)]
			[Deprecated (PlatformName.MacOSX, 10, 8)]
#endif
			RubyKana = 1,
			RubyKanaOn = 2,
			RubyKanaOff = 3
		}

		public CTFontFeatureRubyKana (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureCJKSymbolAlternatives : CTFontFeatureSelectors {
		public enum Selector {
			NoCJKSymbolAlternatives = 0,
			CJKSymbolAltOne = 1,
			CJKSymbolAltTwo = 2,
			CJKSymbolAltThree = 3,
			CJKSymbolAltFour = 4,
			CJKSymbolAltFive = 5
		}

		public CTFontFeatureCJKSymbolAlternatives (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureIdeographicAlternatives : CTFontFeatureSelectors {
		public enum Selector {
			NoIdeographicAlternatives = 0,
			IdeographicAltOne = 1,
			IdeographicAltTwo = 2,
			IdeographicAltThree = 3,
			IdeographicAltFour = 4,
			IdeographicAltFive = 5
		}

		public CTFontFeatureIdeographicAlternatives (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureCJKVerticalRomanPlacement : CTFontFeatureSelectors {
		public enum Selector {
			CJKVerticalRomanCentered = 0,
			CJKVerticalRomanHBaseline = 1
		}

		public CTFontFeatureCJKVerticalRomanPlacement (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureItalicCJKRoman : CTFontFeatureSelectors {
		public enum Selector {
#if NET
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[UnsupportedOSPlatform ("tvos")]
			[UnsupportedOSPlatform ("maccatalyst")]
			[ObsoletedOSPlatform ("macos10.8")]
			[ObsoletedOSPlatform ("ios5.1")]
#else
			[Deprecated (PlatformName.iOS, 5, 1)]
			[Deprecated (PlatformName.MacOSX, 10, 8)]
#endif
			NoCJKItalicRoman = 0,
#if NET
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[UnsupportedOSPlatform ("tvos")]
			[UnsupportedOSPlatform ("maccatalyst")]
			[ObsoletedOSPlatform ("macos10.8")]
			[ObsoletedOSPlatform ("ios5.1")]
#else
			[Deprecated (PlatformName.iOS, 5, 1)]
			[Deprecated (PlatformName.MacOSX, 10, 8)]
#endif
			CJKItalicRoman = 1,
			CJKItalicRomanOn = 2,
			CJKItalicRomanOff = 3
		}

		public CTFontFeatureItalicCJKRoman (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureCaseSensitiveLayout : CTFontFeatureSelectors {
		public enum Selector {
			CaseSensitiveLayoutOn = 0,
			CaseSensitiveLayoutOff = 1,
			CaseSensitiveSpacingOn = 2,
			CaseSensitiveSpacingOff = 3
		}

		public CTFontFeatureCaseSensitiveLayout (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureAlternateKana : CTFontFeatureSelectors {
		public enum Selector {
			AlternateHorizKanaOn = 0,
			AlternateHorizKanaOff = 1,
			AlternateVertKanaOn = 2,
			AlternateVertKanaOff = 3
		}

		public CTFontFeatureAlternateKana (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureStylisticAlternatives : CTFontFeatureSelectors {
		public enum Selector {
			NoStylisticAlternates = 0,
			StylisticAltOneOn = 2,
			StylisticAltOneOff = 3,
			StylisticAltTwoOn = 4,
			StylisticAltTwoOff = 5,
			StylisticAltThreeOn = 6,
			StylisticAltThreeOff = 7,
			StylisticAltFourOn = 8,
			StylisticAltFourOff = 9,
			StylisticAltFiveOn = 10,
			StylisticAltFiveOff = 11,
			StylisticAltSixOn = 12,
			StylisticAltSixOff = 13,
			StylisticAltSevenOn = 14,
			StylisticAltSevenOff = 15,
			StylisticAltEightOn = 16,
			StylisticAltEightOff = 17,
			StylisticAltNineOn = 18,
			StylisticAltNineOff = 19,
			StylisticAltTenOn = 20,
			StylisticAltTenOff = 21,
			StylisticAltElevenOn = 22,
			StylisticAltElevenOff = 23,
			StylisticAltTwelveOn = 24,
			StylisticAltTwelveOff = 25,
			StylisticAltThirteenOn = 26,
			StylisticAltThirteenOff = 27,
			StylisticAltFourteenOn = 28,
			StylisticAltFourteenOff = 29,
			StylisticAltFifteenOn = 30,
			StylisticAltFifteenOff = 31,
			StylisticAltSixteenOn = 32,
			StylisticAltSixteenOff = 33,
			StylisticAltSeventeenOn = 34,
			StylisticAltSeventeenOff = 35,
			StylisticAltEighteenOn = 36,
			StylisticAltEighteenOff = 37,
			StylisticAltNineteenOn = 38,
			StylisticAltNineteenOff = 39,
			StylisticAltTwentyOn = 40,
			StylisticAltTwentyOff = 41
		}

		public CTFontFeatureStylisticAlternatives (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureContextualAlternates : CTFontFeatureSelectors {
		public enum Selector {
			ContextualAlternatesOn = 0,
			ContextualAlternatesOff = 1,
			SwashAlternatesOn = 2,
			SwashAlternatesOff = 3,
			ContextualSwashAlternatesOn = 4,
			ContextualSwashAlternatesOff = 5
		}

		public CTFontFeatureContextualAlternates (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureLowerCase : CTFontFeatureSelectors {
		public enum Selector {
			DefaultLowerCase = 0,
			LowerCaseSmallCaps = 1,
			LowerCasePetiteCaps = 2
		}

		public CTFontFeatureLowerCase (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureUpperCase : CTFontFeatureSelectors {
		public enum Selector {
			DefaultUpperCase = 0,
			UpperCaseSmallCaps = 1,
			UpperCasePetiteCaps = 2
		}

		public CTFontFeatureUpperCase (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureCJKRomanSpacing : CTFontFeatureSelectors {
		public enum Selector {
			HalfWidthCJKRoman = 0,
			ProportionalCJKRoman = 1,
			DefaultCJKRoman = 2,
			FullWidthCJKRoman = 3
		}

		public CTFontFeatureCJKRomanSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontFeatureSettings {

		internal CTFontFeatureSettings (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary { get; private set; }

		public FontFeatureGroup FeatureGroup {
			get {
				return (FontFeatureGroup) (int) (NSNumber) Dictionary [CTFontFeatureKey.Identifier];
			}
		}

		public int FeatureWeak {
			get {
				return (int) (NSNumber) Dictionary [CTFontFeatureSelectorKey.Identifier];
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontVariationAxes {

		public CTFontVariationAxes ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFontVariationAxes (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary { get; private set; }

		public NSNumber Identifier {
			get { return (NSNumber) Dictionary [CTFontVariationAxisKey.Identifier]; }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.Identifier, value); }
		}

		public NSNumber MinimumValue {
			get { return (NSNumber) Dictionary [CTFontVariationAxisKey.MinimumValue]; }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.MinimumValue, value); }
		}

		public NSNumber MaximumValue {
			get { return (NSNumber) Dictionary [CTFontVariationAxisKey.MaximumValue]; }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.MaximumValue, value); }
		}

		public NSNumber DefaultValue {
			get { return (NSNumber) Dictionary [CTFontVariationAxisKey.DefaultValue]; }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.DefaultValue, value); }
		}

		public string? Name {
			get { return Adapter.GetStringValue (Dictionary, CTFontVariationAxisKey.Name); }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.Name, value); }
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public bool? Hidden {
			get { return Adapter.GetBoolValue (Dictionary, CTFontVariationAxisKey.Hidden); }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.Hidden, value); }
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontVariation {

		public CTFontVariation ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFontVariation (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary { get; private set; }
	}

	public partial class CTFont : NativeObject {
		[Preserve (Conditional = true)]
		internal CTFont (NativeHandle handle, bool owns)
			: base (handle, owns, true)
		{
		}

		#region Font Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateWithName (IntPtr name, nfloat size, IntPtr matrix);

		static IntPtr Create (string name, nfloat size)
		{
			var n = CFString.CreateNative (name);
			try {
				var handle = CTFontCreateWithName (n, size, IntPtr.Zero);
				if (handle == IntPtr.Zero)
					throw ConstructorError.Unknown (typeof (CTFont));
				return handle;
			} finally {
				CFString.ReleaseNative (n);
			}
		}

		public CTFont (string name, nfloat size)
			: base (Create (name, size), true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateWithName (IntPtr name, nfloat size, ref CGAffineTransform matrix);

		static IntPtr Create (string name, nfloat size, ref CGAffineTransform matrix)
		{
			var n = CFString.CreateNative (name);
			try {
				var handle = CTFontCreateWithName (n, size, ref matrix);
				if (handle == IntPtr.Zero)
					throw ConstructorError.Unknown (typeof (CTFont));
				return handle;
			} finally {
				CFString.ReleaseNative (n);
			}
		}

		public CTFont (string name, nfloat size, ref CGAffineTransform matrix)
			: base (Create (name, size, ref matrix), true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateWithFontDescriptor (IntPtr descriptor, nfloat size, IntPtr matrix);

		static IntPtr Create (CTFontDescriptor descriptor, nfloat size)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			var handle = CTFontCreateWithFontDescriptor (descriptor.Handle, size, IntPtr.Zero);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		public CTFont (CTFontDescriptor descriptor, nfloat size)
			: base (Create (descriptor, size), true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateWithFontDescriptor (IntPtr descriptor, nfloat size, ref CGAffineTransform matrix);

		static IntPtr Create (CTFontDescriptor descriptor, nfloat size, ref CGAffineTransform matrix)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			var handle = CTFontCreateWithFontDescriptor (descriptor.Handle, size, ref matrix);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		public CTFont (CTFontDescriptor descriptor, nfloat size, ref CGAffineTransform matrix)
			: base (Create (descriptor, size, ref matrix), true)
		{
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateWithNameAndOptions (IntPtr name, nfloat size, IntPtr matrix, nuint options);

		static IntPtr Create (string name, nfloat size, CTFontOptions options)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			var n = CFString.CreateNative (name);
			try {
				var handle = CTFontCreateWithNameAndOptions (n, size, IntPtr.Zero, (nuint) (ulong) options);
				if (handle == IntPtr.Zero)
					throw ConstructorError.Unknown (typeof (CTFont));
				return handle;
			} finally {
				CFString.ReleaseNative (n);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public CTFont (string name, nfloat size, CTFontOptions options)
			: base (Create (name, size, options), true)
		{
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateWithNameAndOptions (IntPtr name, nfloat size, ref CGAffineTransform matrix, nuint options);

		static IntPtr Create (string name, nfloat size, ref CGAffineTransform matrix, CTFontOptions options)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			var n = CFString.CreateNative (name);
			try {
				var handle = CTFontCreateWithNameAndOptions (n, size, ref matrix, (nuint) (ulong) options);
				if (handle == IntPtr.Zero)
					throw ConstructorError.Unknown (typeof (CTFont));
				return handle;
			} finally {
				CFString.ReleaseNative (n);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public CTFont (string name, nfloat size, ref CGAffineTransform matrix, CTFontOptions options)
			: base (Create (name, size, ref matrix, options), true)
		{
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateWithFontDescriptorAndOptions (IntPtr descriptor, nfloat size, IntPtr matrix, nuint options);

		static IntPtr Create (CTFontDescriptor descriptor, nfloat size, CTFontOptions options)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			var handle = CTFontCreateWithFontDescriptorAndOptions (descriptor.Handle, size, IntPtr.Zero, (nuint) (ulong) options);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public CTFont (CTFontDescriptor descriptor, nfloat size, CTFontOptions options)
			: base (Create (descriptor, size, options), true)
		{
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateWithFontDescriptorAndOptions (IntPtr descriptor, nfloat size, ref CGAffineTransform matrix, nuint options);

		static IntPtr Create (CTFontDescriptor descriptor, nfloat size, CTFontOptions options, ref CGAffineTransform matrix)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			var handle = CTFontCreateWithFontDescriptorAndOptions (descriptor.Handle, size, ref matrix, (nuint) (ulong) options);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public CTFont (CTFontDescriptor descriptor, nfloat size, CTFontOptions options, ref CGAffineTransform matrix)
			: base (Create (descriptor, size, options, ref matrix), true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CTFontRef __nonnull */ IntPtr CTFontCreateWithGraphicsFont (
			/* CGFontRef __nonnull */ IntPtr cgfontRef, nfloat size,
			/* const CGAffineTransform * __nullable */ ref CGAffineTransform affine,
			/* CTFontDescriptorRef __nullable */ IntPtr attrs);

		static IntPtr Create (CGFont font, nfloat size, CGAffineTransform transform, CTFontDescriptor descriptor)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
			var handle = CTFontCreateWithGraphicsFont (font.Handle, size, ref transform, descriptor.GetHandle ());
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		[DllImport (Constants.CoreTextLibrary, EntryPoint = "CTFontCreateWithGraphicsFont")]
		static extern IntPtr CTFontCreateWithGraphicsFont2 (IntPtr cgfontRef, nfloat size, IntPtr affine, IntPtr attrs);

		public CTFont (CGFont font, nfloat size, CGAffineTransform transform, CTFontDescriptor descriptor)
			: base (Create (font, size, transform, descriptor), true)
		{
		}

		static IntPtr Create (CGFont font, nfloat size, CTFontDescriptor descriptor)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
			var handle = CTFontCreateWithGraphicsFont2 (font.Handle, size, IntPtr.Zero, descriptor.GetHandle ());
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		public CTFont (CGFont font, nfloat size, CTFontDescriptor descriptor)
			: base (Create (font, size, descriptor), true)
		{
		}

		static IntPtr Create (CGFont font, nfloat size, CGAffineTransform transform)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
			var handle = CTFontCreateWithGraphicsFont (font.Handle, size, ref transform, IntPtr.Zero);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		public CTFont (CGFont font, nfloat size, CGAffineTransform transform)
			: base (Create (font, size, transform), true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateUIFontForLanguage (CTFontUIFontType uiType, nfloat size, IntPtr language);

		static IntPtr Create (CTFontUIFontType uiType, nfloat size, string language)
		{
			var n = CFString.CreateNative (language);
			try {
				var handle = CTFontCreateUIFontForLanguage (uiType, size, n);
				if (handle == IntPtr.Zero)
					throw ConstructorError.Unknown (typeof (CTFont));
				return handle;
			} finally {
				CFString.ReleaseNative (n);
			}
		}

		public CTFont (CTFontUIFontType uiType, nfloat size, string language)
			: base (Create (uiType, size, language), true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateCopyWithAttributes (IntPtr font, nfloat size, IntPtr matrix, IntPtr attributues);
		public CTFont? WithAttributes (nfloat size, CTFontDescriptor attributes)
		{
			if (attributes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (attributes));
			return CreateFont (CTFontCreateCopyWithAttributes (Handle, size, IntPtr.Zero, attributes.Handle));
		}

		static CTFont? CreateFont (IntPtr h)
		{
			if (h == IntPtr.Zero)
				return null;
			return new CTFont (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateCopyWithAttributes (IntPtr font, nfloat size, ref CGAffineTransform matrix, IntPtr attributes);
		public CTFont? WithAttributes (nfloat size, CTFontDescriptor attributes, ref CGAffineTransform matrix)
		{
			return CreateFont (CTFontCreateCopyWithAttributes (Handle, size, ref matrix, attributes.GetHandle ()));
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateCopyWithSymbolicTraits (IntPtr font, nfloat size, IntPtr matrix, CTFontSymbolicTraits symTraitValue, CTFontSymbolicTraits symTraitMask);
		public CTFont? WithSymbolicTraits (nfloat size, CTFontSymbolicTraits symTraitValue, CTFontSymbolicTraits symTraitMask)
		{
			return CreateFont (
					CTFontCreateCopyWithSymbolicTraits (Handle, size, IntPtr.Zero, symTraitValue, symTraitMask));
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateCopyWithSymbolicTraits (IntPtr font, nfloat size, ref CGAffineTransform matrix, CTFontSymbolicTraits symTraitValue, CTFontSymbolicTraits symTraitMask);
		public CTFont? WithSymbolicTraits (nfloat size, CTFontSymbolicTraits symTraitValue, CTFontSymbolicTraits symTraitMask, ref CGAffineTransform matrix)
		{
			return CreateFont (
					CTFontCreateCopyWithSymbolicTraits (Handle, size, ref matrix, symTraitValue, symTraitMask));
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateCopyWithFamily (IntPtr font, nfloat size, IntPtr matrix, IntPtr family);
		public CTFont? WithFamily (nfloat size, string family)
		{
			if (family is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (family));
			var n = CFString.CreateNative (family);
			try {
				return CreateFont (CTFontCreateCopyWithFamily (Handle, size, IntPtr.Zero, n));
			} finally {
				CFString.ReleaseNative (n);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreateCopyWithFamily (IntPtr font, nfloat size, ref CGAffineTransform matrix, IntPtr family);
		public CTFont? WithFamily (nfloat size, string family, ref CGAffineTransform matrix)
		{
			if (family is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (family));
			var n = CFString.CreateNative (family);
			try {
				return CreateFont (CTFontCreateCopyWithFamily (Handle, size, ref matrix, n));
			} finally {
				CFString.ReleaseNative (n);
			}
		}

		#endregion

		#region Font Cascading

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CTFontRef __nonnull */ IntPtr CTFontCreateForString (
			/* CTFontRef __nonnull */ IntPtr currentFont,
			/* CFStringRef __nonnull */ IntPtr @string,
			NSRange range);

		public CTFont? ForString (string value, NSRange range)
		{
			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
			var n = CFString.CreateNative (value);
			try {
				return CreateFont (CTFontCreateForString (Handle, n, range));
			} finally {
				CFString.ReleaseNative (n);
			}
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CTFontRef */ IntPtr CTFontCreateForStringWithLanguage (
			/* CTFontRef */ IntPtr currentFont,
			/* CFStringRef */ IntPtr @string,
			NSRange range,
			/* CFStringRef _Nullable */ IntPtr language);

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		public CTFont? ForString (string value, NSRange range, string? language)
		{
			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

			var v = CFString.CreateNative (value);
			var l = CFString.CreateNative (language);
			try {
				return CreateFont (CTFontCreateForStringWithLanguage (Handle, v, range, l));
			} finally {
				CFString.ReleaseNative (l);
				CFString.ReleaseNative (v);
			}
		}

		#endregion

		#region Font Accessors

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CTFontDescriptorRef __nonnull */ IntPtr CTFontCopyFontDescriptor (
			/* CTFontRef __nonnull */ IntPtr font);

		public CTFontDescriptor GetFontDescriptor ()
		{
			var h = CTFontCopyFontDescriptor (Handle);
			return new CTFontDescriptor (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFTypeRef __nullable */ IntPtr CTFontCopyAttribute (/* CTFontRef __nonnull */ IntPtr font,
			/* CFStringRef __nonnull */ IntPtr attribute);

		public NSObject? GetAttribute (NSString attribute)
		{
			if (attribute is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (attribute));
			return Runtime.GetNSObject (CTFontCopyAttribute (Handle, attribute.Handle));
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetSize (IntPtr font);
		public nfloat Size {
			get { return CTFontGetSize (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGAffineTransform CTFontGetMatrix (/* CTFontRef __nonnull */ IntPtr font);

		public CGAffineTransform Matrix {
			get { return CTFontGetMatrix (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CTFontSymbolicTraits CTFontGetSymbolicTraits (IntPtr font);
		public CTFontSymbolicTraits SymbolicTraits {
			get { return CTFontGetSymbolicTraits (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyTraits (IntPtr font);
		public CTFontTraits? GetTraits ()
		{
			var d = Runtime.GetNSObject<NSDictionary> (CTFontCopyTraits (Handle), true);
			if (d is null)
				return null;
			return new CTFontTraits (d);
		}

		#endregion

		#region Font Names
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyPostScriptName (IntPtr font);
		public string? PostScriptName {
			get { return CFString.FromHandle (CTFontCopyPostScriptName (Handle), releaseHandle: true); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFStringRef __nonnull */ IntPtr CTFontCopyFamilyName (
			/* CTFontRef __nonnull */ IntPtr font);

		public string? FamilyName {
			get { return CFString.FromHandle (CTFontCopyFamilyName (Handle), releaseHandle: true); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFStringRef __nonnull */ IntPtr CTFontCopyFullName (
			/* CTFontRef __nonnull */ IntPtr font);

		public string? FullName {
			get { return CFString.FromHandle (CTFontCopyFullName (Handle), releaseHandle: true); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFStringRef __nonnull */ IntPtr CTFontCopyDisplayName (
			/* CTFontRef __nonnull */ IntPtr font);

		public string? DisplayName {
			get { return CFString.FromHandle (CTFontCopyDisplayName (Handle), releaseHandle: true); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyName (IntPtr font, IntPtr nameKey);
		public string? GetName (CTFontNameKey nameKey)
		{
			return CFString.FromHandle (CTFontCopyName (Handle, CTFontNameKeyId.ToId (nameKey).GetHandle ()), releaseHandle: true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyLocalizedName (IntPtr font, IntPtr nameKey, out IntPtr actualLanguage);

		public string? GetLocalizedName (CTFontNameKey nameKey)
		{
			return GetLocalizedName (nameKey, out _);
		}

		public string? GetLocalizedName (CTFontNameKey nameKey, out string? actualLanguage)
		{
			IntPtr actual;
			var ret = CFString.FromHandle (CTFontCopyLocalizedName (Handle, CTFontNameKeyId.ToId (nameKey).GetHandle (), out actual), releaseHandle: true);
			actualLanguage = CFString.FromHandle (actual, releaseHandle: true);
			return ret;
		}
		#endregion

		#region Font Encoding
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFCharacterSetRef __nonnull */ IntPtr CTFontCopyCharacterSet (
			/* CTFontRef __nonnull */ IntPtr font);

		public NSCharacterSet? CharacterSet {
			get {
				return Runtime.GetNSObject<NSCharacterSet> (CTFontCopyCharacterSet (Handle), true);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern uint CTFontGetStringEncoding (IntPtr font);
		public uint StringEncoding {
			get { return CTFontGetStringEncoding (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopySupportedLanguages (IntPtr font);
		public string? [] GetSupportedLanguages ()
		{
			var cfArrayRef = CTFontCopySupportedLanguages (Handle);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<string> ();
			return CFArray.StringArrayFromHandle (cfArrayRef, true)!;
		}

		[DllImport (Constants.CoreTextLibrary, CharSet = CharSet.Unicode)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CTFontGetGlyphsForCharacters (IntPtr font, char [] characters, CGGlyph [] glyphs, nint count);

		public bool GetGlyphsForCharacters (char [] characters, CGGlyph [] glyphs, nint count)
		{
			AssertCount (count);
			AssertLength ("characters", characters, count);
			AssertLength ("glyphs", characters, count);

			return CTFontGetGlyphsForCharacters (Handle, characters, glyphs, count);
		}

		public bool GetGlyphsForCharacters (char [] characters, CGGlyph [] glyphs)
		{
			return GetGlyphsForCharacters (characters, glyphs, Math.Min (characters.Length, glyphs.Length));
		}

#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (7, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[iOS (14, 0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe /* CFStringRef _Nullable */ IntPtr CTFontCopyNameForGlyph (/* CTFontRef */ IntPtr font, CGGlyph glyph);

#if NET
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (7, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[iOS (14, 0)]
#endif
		public string? GetGlyphName (CGGlyph glyph)
		{
			return CFString.FromHandle (CTFontCopyNameForGlyph (Handle, glyph), releaseHandle: true);
		}

		static void AssertCount (nint count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException (nameof (count), "cannot be negative");
		}

		static void AssertLength<T> (string name, T []? array, nint count)
		{
			AssertLength (name, array, count, false);
		}

		static void AssertLength<T> (string name, T []? array, nint count, bool canBeNull)
		{
			if (canBeNull && array is null)
				return;
			if (array is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			if (array.Length < count)
				throw new ArgumentException (string.Format ("{0}.Length cannot be < count", name), name);
		}
		#endregion

		#region Font Metrics
		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetAscent (/* CTFontRef __nonnull */ IntPtr font);

		public nfloat AscentMetric {
			get { return CTFontGetAscent (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetDescent (/* CTFontRef __nonnull */ IntPtr font);

		public nfloat DescentMetric {
			get { return CTFontGetDescent (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetLeading (/* CTFontRef __nonnull */ IntPtr font);

		public nfloat LeadingMetric {
			get { return CTFontGetLeading (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern uint CTFontGetUnitsPerEm (IntPtr font);
		public uint UnitsPerEmMetric {
			get { return CTFontGetUnitsPerEm (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFIndex */ nint CTFontGetGlyphCount (/* CTFontRef __nonnull */ IntPtr font);

		public nint GlyphCount {
			get { return CTFontGetGlyphCount (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTFontGetBoundingBox (/* CTFontRef __nonnull */ IntPtr font);

		public CGRect BoundingBox {
			get { return CTFontGetBoundingBox (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetUnderlinePosition (IntPtr font);
		public nfloat UnderlinePosition {
			get { return CTFontGetUnderlinePosition (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetUnderlineThickness (IntPtr font);
		public nfloat UnderlineThickness {
			get { return CTFontGetUnderlineThickness (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetSlantAngle (IntPtr font);
		public nfloat SlantAngle {
			get { return CTFontGetSlantAngle (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetCapHeight (/* CTFontRef __nonnull */ IntPtr font);

		public nfloat CapHeightMetric {
			get { return CTFontGetCapHeight (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetXHeight (IntPtr font);
		public nfloat XHeightMetric {
			get { return CTFontGetXHeight (Handle); }
		}
		#endregion

		#region Font Glyphs
		[DllImport (Constants.CoreTextLibrary)]
		static extern CGGlyph CTFontGetGlyphWithName (/* CTFontRef __nonnull */ IntPtr font,
			/* CFStringRef __nonnull */ IntPtr glyphName);

		public CGGlyph GetGlyphWithName (string glyphName)
		{
			if (glyphName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (glyphName));
			var nameHandle = CFString.CreateNative (glyphName);
			try {
				return CTFontGetGlyphWithName (Handle, nameHandle);
			} finally {
				CFString.ReleaseNative (nameHandle);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTFontGetBoundingRectsForGlyphs (IntPtr font, CTFontOrientation orientation, [In] CGGlyph [] glyphs, [Out] CGRect []? boundingRects, nint count);
		public CGRect GetBoundingRects (CTFontOrientation orientation, CGGlyph [] glyphs, CGRect []? boundingRects, nint count)
		{
			AssertCount (count);
			AssertLength ("glyphs", glyphs, count);
			AssertLength ("boundingRects", boundingRects, count, true);

			return CTFontGetBoundingRectsForGlyphs (Handle, orientation, glyphs, boundingRects, count);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTFontGetOpticalBoundsForGlyphs (IntPtr font, [In] CGGlyph [] glyphs, [Out] CGRect [] boundingRects, nint count, nuint options);

		public CGRect GetOpticalBounds (CGGlyph [] glyphs, CGRect [] boundingRects, nint count, CTFontOptions options = 0)
		{
			AssertCount (count);
			AssertLength ("glyphs", glyphs, count);
			AssertLength ("boundingRects", boundingRects, count, true);

			return CTFontGetOpticalBoundsForGlyphs (Handle, glyphs, boundingRects, count, (nuint) (ulong) options);
		}

		public CGRect GetBoundingRects (CTFontOrientation orientation, CGGlyph [] glyphs)
		{
			if (glyphs is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (glyphs));
			return GetBoundingRects (orientation, glyphs, null, glyphs.Length);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern double CTFontGetAdvancesForGlyphs (IntPtr font, CTFontOrientation orientation, [In] CGGlyph [] glyphs, [Out] CGSize []? advances, nint count);
		public double GetAdvancesForGlyphs (CTFontOrientation orientation, CGGlyph [] glyphs, CGSize []? advances, nint count)
		{
			AssertCount (count);
			AssertLength ("glyphs", glyphs, count);
			AssertLength ("advances", advances, count, true);

			return CTFontGetAdvancesForGlyphs (Handle, orientation, glyphs, advances, count);
		}

		public double GetAdvancesForGlyphs (CTFontOrientation orientation, CGGlyph [] glyphs)
		{
			if (glyphs is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (glyphs));
			return GetAdvancesForGlyphs (orientation, glyphs, null, glyphs.Length);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTFontGetVerticalTranslationsForGlyphs (IntPtr font, [In] CGGlyph [] glyphs, [Out] CGSize [] translations, nint count);
		public void GetVerticalTranslationsForGlyphs (CGGlyph [] glyphs, CGSize [] translations, nint count)
		{
			AssertCount (count);
			AssertLength ("glyphs", glyphs, count);
			AssertLength ("translations", translations, count);

			CTFontGetVerticalTranslationsForGlyphs (Handle, glyphs, translations, count);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreatePathForGlyph (IntPtr font, CGGlyph glyph, IntPtr transform);
		public CGPath? GetPathForGlyph (CGGlyph glyph)
		{
			var h = CTFontCreatePathForGlyph (Handle, glyph, IntPtr.Zero);
			if (h == IntPtr.Zero)
				return null;
			return new CGPath (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCreatePathForGlyph (IntPtr font, CGGlyph glyph, ref CGAffineTransform transform);
		public CGPath? GetPathForGlyph (CGGlyph glyph, ref CGAffineTransform transform)
		{
			var h = CTFontCreatePathForGlyph (Handle, glyph, ref transform);
			if (h == IntPtr.Zero)
				return null;
			return new CGPath (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTFontDrawGlyphs (/* CTFontRef __nonnull */ IntPtr font,
			[In] CGGlyph [] glyphs, [In] CGPoint [] positions, nint count,
			/* CGContextRef __nonnull */ IntPtr context);

		public void DrawGlyphs (CGContext context, CGGlyph [] glyphs, CGPoint [] positions)
		{
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			if (glyphs is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (glyphs));
			if (positions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (positions));
			int gl = glyphs.Length;
			if (gl != positions.Length)
				throw new ArgumentException ("array sizes fo context and glyphs differ");
			CTFontDrawGlyphs (Handle, glyphs, positions, gl, context.Handle);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe nint CTFontGetLigatureCaretPositions (IntPtr handle, CGGlyph glyph, [Out] nfloat* positions, nint max);

		public nint GetLigatureCaretPositions (CGGlyph glyph, nfloat [] positions)
		{
			if (positions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (positions));
			unsafe {
				fixed (nfloat* positionsPtr = positions) {
					return CTFontGetLigatureCaretPositions (Handle, glyph, positionsPtr, positions.Length);
				}
			}
		}
		#endregion

		#region Font Variations
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyVariationAxes (IntPtr font);
		public CTFontVariationAxes [] GetVariationAxes ()
		{
			var cfArrayRef = CTFontCopyVariationAxes (Handle);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontVariationAxes> ();
			return NSArray.ArrayFromHandle (cfArrayRef,
					d => new CTFontVariationAxes (Runtime.GetNSObject<NSDictionary> (d)!), true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyVariation (IntPtr font);
		public CTFontVariation? GetVariation ()
		{
			var cfDictionaryRef = CTFontCopyVariation (Handle);
			if (cfDictionaryRef == IntPtr.Zero)
				return null;
			return new CTFontVariation (Runtime.GetNSObject<NSDictionary> (cfDictionaryRef)!);
		}
		#endregion

		#region Font Features
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef __nullable */ IntPtr CTFontCopyFeatures (
			/* CTFontRef __nonnull */ IntPtr font);

		// Always returns only default features
		public CTFontFeatures [] GetFeatures ()
		{
			var cfArrayRef = CTFontCopyFeatures (Handle);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontFeatures> ();
			return NSArray.ArrayFromHandle (cfArrayRef,
					d => new CTFontFeatures (Runtime.GetNSObject<NSDictionary> (d)!), true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef __nullable */ IntPtr CTFontCopyFeatureSettings (
			/* CTFontRef __nonnull */ IntPtr font);

		public CTFontFeatureSettings [] GetFeatureSettings ()
		{
			var cfArrayRef = CTFontCopyFeatureSettings (Handle);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontFeatureSettings> ();
			return NSArray.ArrayFromHandle (cfArrayRef,
					d => new CTFontFeatureSettings (Runtime.GetNSObject<NSDictionary> (d)!), true);
		}
		#endregion

		#region Font Conversion
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyGraphicsFont (IntPtr font, IntPtr attributes);
		public CGFont? ToCGFont (CTFontDescriptor? attributes)
		{
			var h = CTFontCopyGraphicsFont (Handle, attributes.GetHandle ());
			if (h == IntPtr.Zero)
				return null;
			return new CGFont (h, true);
		}

		public CGFont? ToCGFont ()
		{
			return ToCGFont (null);
		}
		#endregion

		#region Font Tables
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef __nullable */ IntPtr CTFontCopyAvailableTables (
			/* CTFontRef __nonnull */ IntPtr font, CTFontTableOptions options);

		public CTFontTable [] GetAvailableTables (CTFontTableOptions options)
		{
			var cfArrayRef = CTFontCopyAvailableTables (Handle, options);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontTable> ();
			return NSArray.ArrayFromHandle (cfArrayRef, v => {
				return (CTFontTable) (uint) (IntPtr) v;
			}, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyTable (IntPtr font, CTFontTable table, CTFontTableOptions options);
		public NSData? GetFontTableData (CTFontTable table, CTFontTableOptions options)
		{
			var cfDataRef = CTFontCopyTable (Handle, table, options);
			return Runtime.GetNSObject<NSData> (cfDataRef, true);
		}
		#endregion

		#region
		[DllImport (Constants.CoreTextLibrary)]
		extern static /* CFArrayRef __nullable */ IntPtr CTFontCopyDefaultCascadeListForLanguages (
			/* CTFontRef __nonnull */ IntPtr font, /* CFArrayRef __nullable */ IntPtr languagePrefList);

		public CTFontDescriptor? []? GetDefaultCascadeList (string [] languages)
		{
			using (var arr = languages is null ? null : NSArray.FromStrings (languages)) {
				var h = CTFontCopyDefaultCascadeListForLanguages (Handle, arr.GetHandle ());
				return CFArray.ArrayFromHandleFunc<CTFontDescriptor> (h,
					(handle) => new CTFontDescriptor (handle, false), true);
			}
		}

		#endregion
		public override string? ToString ()
		{
			return FullName;
		}

		[DllImport (Constants.CoreTextLibrary, EntryPoint = "CTFontGetTypeID")]
		public extern static nint GetTypeID ();
	}
}
