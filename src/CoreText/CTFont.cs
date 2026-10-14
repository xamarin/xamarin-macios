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

using System.Collections.Generic;
using System.Runtime.CompilerServices;

using CoreFoundation;
using CoreGraphics;

using CGGlyph = System.UInt16;

namespace CoreText {

	/// <summary>Options used when creating new instances of the <see cref="CoreText.CTFont" /> class.</summary>
	///     <remarks>
	///     </remarks>
	[Flags]
	[Native]
	// defined as CFOptionFlags (unsigned long [long] = nuint) - /System/Library/Frameworks/CoreText.framework/Headers/CTFont.h
	public enum CTFontOptions : ulong {
		/// <summary>Use default options.</summary>
		Default = 0,
		/// <summary>Prevents font activation.</summary>
		PreventAutoActivation = 1 << 0,
		[SupportedOSPlatform ("tvos16.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst")]
		PreventAutoDownload = 1 << 1,
		/// <summary>Give preferences to Apple/System fonts.</summary>
		PreferSystemFont = 1 << 2,
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFont.h
	/// <summary>An enumeration whose values specify the intended use of a font. Used with <see cref="CTFont.CTFont(CTFontUIFontType, nfloat, System.String)" /></summary>
	///     <remarks>To be added.</remarks>
	public enum CTFontUIFontType : uint {
		/// <summary>To be added.</summary>
		None = unchecked((uint) (-1)),
		/// <summary>To be added.</summary>
		User = 0,
		/// <summary>To be added.</summary>
		UserFixedPitch = 1,
		/// <summary>To be added.</summary>
		System = 2,
		/// <summary>To be added.</summary>
		EmphasizedSystem = 3,
		/// <summary>To be added.</summary>
		SmallSystem = 4,
		/// <summary>To be added.</summary>
		SmallEmphasizedSystem = 5,
		/// <summary>To be added.</summary>
		MiniSystem = 6,
		/// <summary>To be added.</summary>
		MiniEmphasizedSystem = 7,
		/// <summary>To be added.</summary>
		Views = 8,
		/// <summary>To be added.</summary>
		Application = 9,
		/// <summary>To be added.</summary>
		Label = 10,
		/// <summary>To be added.</summary>
		MenuTitle = 11,
		/// <summary>To be added.</summary>
		MenuItem = 12,
		/// <summary>To be added.</summary>
		MenuItemMark = 13,
		/// <summary>To be added.</summary>
		MenuItemCmdKey = 14,
		/// <summary>To be added.</summary>
		WindowTitle = 15,
		/// <summary>To be added.</summary>
		PushButton = 16,
		/// <summary>To be added.</summary>
		UtilityWindowTitle = 17,
		/// <summary>To be added.</summary>
		AlertHeader = 18,
		/// <summary>To be added.</summary>
		SystemDetail = 19,
		/// <summary>To be added.</summary>
		EmphasizedSystemDetail = 20,
		/// <summary>To be added.</summary>
		Toolbar = 21,
		/// <summary>To be added.</summary>
		SmallToolbar = 22,
		/// <summary>To be added.</summary>
		Message = 23,
		/// <summary>To be added.</summary>
		Palette = 24,
		/// <summary>To be added.</summary>
		ToolTip = 25,
		/// <summary>To be added.</summary>
		ControlContent = 26,
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFont.h
	/// <summary>An enumeration whose values represent tags for accessing font-table data.</summary>
	///     <remarks>To be added.</remarks>
	public enum CTFontTable : uint {
		/// <summary>To be added.</summary>
		BaselineBASE = 0x42415345,  // 'BASE'
		/// <summary>To be added.</summary>
		ColorBitmapData = 0x43424454,  // 'CBDT'
		/// <summary>To be added.</summary>
		ColorBitmapLocationData = 0x43424c43,  // 'CBLC'
		/// <summary>To be added.</summary>
		PostscriptFontProgram = 0x43464620,  // 'CFF '
		/// <summary>To be added.</summary>
		CompactFontFormat2 = 0x43464632,  // 'CFF2'
		/// <summary>To be added.</summary>
		ColorTable = 0x434f4c52,  // 'COLR'
		/// <summary>To be added.</summary>
		ColorPaletteTable = 0x4350414c,  // 'CPAL'
		/// <summary>To be added.</summary>
		DigitalSignature = 0x44534947,  // 'DSIG'
		/// <summary>To be added.</summary>
		EmbeddedBitmap = 0x45424454,  // 'EBDT'
		/// <summary>To be added.</summary>
		EmbeddedBitmapLocation = 0x45424c43,  // 'EBLC'
		/// <summary>To be added.</summary>
		EmbeddedBitmapScaling = 0x45425343,  // 'EBSC'
		/// <summary>To be added.</summary>
		GlyphDefinition = 0x47444546,  // 'GDEF'
		/// <summary>To be added.</summary>
		GlyphPositioning = 0x47504f53,  // 'GPOS'
		/// <summary>To be added.</summary>
		GlyphSubstitution = 0x47535542,  // 'GSUB'
		/// <summary>To be added.</summary>
		HorizontalMetricsVariations = 0x48564152,  // 'HVAR'
		/// <summary>To be added.</summary>
		JustificationJSTF = 0x4a535446,  // 'JSTF'
		/// <summary>To be added.</summary>
		LinearThreshold = 0x4c545348,  // 'LTSH'
		/// <summary>To be added.</summary>
		MathLayoutData = 0x4d415448,  // 'MATH'
		/// <summary>To be added.</summary>
		Merge = 0x4d455247,  // 'MERG'
		/// <summary>To be added.</summary>
		MetricsVariations = 0x4d564152,  // 'MVAR'
		/// <summary>To be added.</summary>
		WindowsSpecificMetrics = 0x4f532f32,  // 'OS2 '
		/// <summary>To be added.</summary>
		Pcl5Data = 0x50434c54,  // 'PCLT'
		/// <summary>To be added.</summary>
		VerticalDeviceMetrics = 0x56444d58,  // 'VDMX'
		/// <summary>To be added.</summary>
		StyleAttributes = 0x53544154,  // 'STAT'
		/// <summary>To be added.</summary>
		ScalableVectorGraphics = 0x53564720,  // 'SVG '
		/// <summary>To be added.</summary>
		VerticalOrigin = 0x564f5247,  // 'VORG'
		/// <summary>To be added.</summary>
		VerticalMetricsVariations = 0x56564152,  // 'VVAR'
		/// <summary>To be added.</summary>
		GlyphReference = 0x5a617066,  // 'Zapf'
		/// <summary>To be added.</summary>
		AccentAttachment = 0x61636e74,  // 'Acnt'
		/// <summary>To be added.</summary>
		AnchorPoints = 0x616e6b72,  // 'ankr'
		/// <summary>To be added.</summary>
		AxisVariation = 0x61766172,  // 'Avar'
		/// <summary>To be added.</summary>
		BitmapData = 0x62646174,  // 'Bdat'
		/// <summary>To be added.</summary>
		BitmapFontHeader = 0x62686564,  // 'Bhed'
		/// <summary>To be added.</summary>
		BitmapLocation = 0x626c6f63,  // 'Bloc'
		/// <summary>To be added.</summary>
		BaselineBsln = 0x62736c6e,  // 'Bsln'
		/// <summary>To be added.</summary>
		CharacterToGlyphMapping = 0x636d6170,  // 'Cmap'
		/// <summary>To be added.</summary>
		ControlValueTableVariation = 0x63766172,  // 'Cvar'
		/// <summary>To be added.</summary>
		ControlValueTable = 0x63767420,  // 'Cvt '
		/// <summary>To be added.</summary>
		FontDescriptor = 0x66647363,  // 'Fdsc'
		/// <summary>To be added.</summary>
		LayoutFeature = 0x66656174,  // 'Feat'
		/// <summary>To be added.</summary>
		FontMetrics = 0x666d7478,  // 'Fmtx'
		/// <summary>To be added.</summary>
		FondAndNfntData = 0x666f6e64,  // 'fond'
		/// <summary>To be added.</summary>
		FontProgram = 0x6670676d,  // 'Fpgm'
		/// <summary>To be added.</summary>
		FontVariation = 0x66766172,  // 'Fvar'
		/// <summary>To be added.</summary>
		GridFitting = 0x67617370,  // 'Gasp'
		/// <summary>To be added.</summary>
		GlyphData = 0x676c7966,  // 'Glyf'
		/// <summary>To be added.</summary>
		GlyphVariation = 0x67766172,  // 'Gvar'
		/// <summary>To be added.</summary>
		HorizontalDeviceMetrics = 0x68646d78,  // 'Hdmx'
		/// <summary>To be added.</summary>
		FontHeader = 0x68656164,  // 'Head'
		/// <summary>To be added.</summary>
		HorizontalHeader = 0x68686561,  // 'Hhea'
		/// <summary>To be added.</summary>
		HorizontalMetrics = 0x686d7478,  // 'Hmtx'
		/// <summary>To be added.</summary>
		HorizontalStyle = 0x68737479,  // 'Hsty'
		/// <summary>To be added.</summary>
		JustificationJust = 0x6a757374,  // 'Just'
		/// <summary>To be added.</summary>
		Kerning = 0x6b65726e,  // 'Kern'
		/// <summary>To be added.</summary>
		ExtendedKerning = 0x6b657278,  // 'Kerx'
		/// <summary>To be added.</summary>
		LigatureCaret = 0x6c636172,  // 'Lcar'
		/// <summary>To be added.</summary>
		IndexToLocation = 0x6c6f6361,  // 'Loca'
		/// <summary>To be added.</summary>
		LanguageTags = 0x6c746167,  // 'ltag'
		/// <summary>To be added.</summary>
		MaximumProfile = 0x6d617870,  // 'Maxp'
		/// <summary>To be added.</summary>
		Metadata = 0x6d657461,  // 'meta'
		/// <summary>To be added.</summary>
		Morph = 0x6d6f7274,  // 'Mort'
		/// <summary>To be added.</summary>
		ExtendedMorph = 0x6d6f7278,  // 'Morx'
		/// <summary>To be added.</summary>
		Name = 0x6e616d65,  // 'Name'
		/// <summary>To be added.</summary>
		OpticalBounds = 0x6f706264,  // 'Opbd'
		/// <summary>To be added.</summary>
		PostScriptInformation = 0x706f7374,  // 'Post'
		/// <summary>To be added.</summary>
		ControlValueTableProgram = 0x70726570,  // 'Prep'
		/// <summary>To be added.</summary>
		Properties = 0x70726f70,  // 'Prop'
		/// <summary>To be added.</summary>
		SBitmapData = 0x73626974,  // 'sbit'
		/// <summary>To be added.</summary>
		SExtendedBitmapData = 0x73626978,  // 'sbix'
		/// <summary>To be added.</summary>
		Tracking = 0x7472616b,  // 'Trak'
		/// <summary>To be added.</summary>
		VerticalHeader = 0x76686561,  // 'Vhea'
		/// <summary>To be added.</summary>
		VerticalMetrics = 0x766d7478,  // 'Vmtx'
		/// <summary>To be added.</summary>
		CrossReference = 0x78726566,  // 'xref'
	}

	/// <summary>An enumeration whose values can be used as flags for options relating to font tables.</summary>
	///     <remarks>To be added.</remarks>
	[Flags]
	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTFont.h
	public enum CTFontTableOptions : uint {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios6.0")]
		[ObsoletedOSPlatform ("maccatalyst13.1")]
		[ObsoletedOSPlatform ("macos10.8")]
		[UnsupportedOSPlatform ("tvos")]
		ExcludeSynthetic = (1 << 0),
	}

	// anonymous and typeless native enum - /System/Library/Frameworks/CoreText.framework/Headers/SFNTLayoutTypes.h
	/// <summary>An enumeration whose values specify various types of font features.</summary>
	///     <remarks>To be added.</remarks>
	///     <altmember cref="CoreText.CTFontFeatures.FeatureGroup" />
	///     <altmember cref="CoreText.CTFontFeatureSettings.FeatureGroup" />
	public enum FontFeatureGroup {
		/// <summary>To be added.</summary>
		AllTypographicFeatures = 0,
		/// <summary>To be added.</summary>
		Ligatures = 1,
		/// <summary>To be added.</summary>
		CursiveConnection = 2,
		/// <summary>Developers should not use this deprecated field. </summary>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.7")]
		[ObsoletedOSPlatform ("ios6.0")]
		[ObsoletedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("maccatalyst")]
		LetterCase = 3,
		/// <summary>To be added.</summary>
		VerticalSubstitution = 4,
		/// <summary>To be added.</summary>
		LinguisticRearrangement = 5,
		/// <summary>To be added.</summary>
		NumberSpacing = 6,
		/// <summary>To be added.</summary>
		SmartSwash = 8,
		/// <summary>To be added.</summary>
		Diacritics = 9,
		/// <summary>To be added.</summary>
		VerticalPosition = 10,
		/// <summary>To be added.</summary>
		Fractions = 11,
		/// <summary>To be added.</summary>
		OverlappingCharacters = 13,
		/// <summary>To be added.</summary>
		TypographicExtras = 14,
		/// <summary>To be added.</summary>
		MathematicalExtras = 15,
		/// <summary>To be added.</summary>
		OrnamentSets = 16,
		/// <summary>To be added.</summary>
		CharacterAlternatives = 17,
		/// <summary>To be added.</summary>
		DesignComplexity = 18,
		/// <summary>To be added.</summary>
		StyleOptions = 19,
		/// <summary>To be added.</summary>
		CharacterShape = 20,
		/// <summary>To be added.</summary>
		NumberCase = 21,
		/// <summary>To be added.</summary>
		TextSpacing = 22,
		/// <summary>To be added.</summary>
		Transliteration = 23,
		/// <summary>To be added.</summary>
		Annotation = 24,
		/// <summary>To be added.</summary>
		KanaSpacing = 25,
		/// <summary>To be added.</summary>
		IdeographicSpacing = 26,
		/// <summary>To be added.</summary>
		UnicodeDecomposition = 27,
		/// <summary>To be added.</summary>
		RubyKana = 28,
		/// <summary>To be added.</summary>
		CJKSymbolAlternatives = 29,
		/// <summary>To be added.</summary>
		IdeographicAlternatives = 30,
		/// <summary>To be added.</summary>
		CJKVerticalRomanPlacement = 31,
		/// <summary>To be added.</summary>
		ItalicCJKRoman = 32,
		/// <summary>To be added.</summary>
		CaseSensitiveLayout = 33,
		/// <summary>To be added.</summary>
		AlternateKana = 34,
		/// <summary>To be added.</summary>
		StylisticAlternatives = 35,
		/// <summary>To be added.</summary>
		ContextualAlternates = 36,
		/// <summary>To be added.</summary>
		LowerCase = 37,
		/// <summary>To be added.</summary>
		UpperCase = 38,
		/// <summary>To be added.</summary>
		CJKRomanSpacing = 103,
	}

	/// <summary>Encapsulates the features of a <see cref="CoreText.CTFont" />.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatures {

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatures ()
			: this (new NSMutableDictionary ())
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatures (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		/// <summary>The NSDictionary that reflects the current values in the strongly typed CTFontFeatures.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public NSDictionary Dictionary { get; private set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Name {
			get { return Adapter.GetStringValue (Dictionary, CTFontFeatureKey.Name); }
			set { Adapter.SetValue (Dictionary, CTFontFeatureKey.Name, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public FontFeatureGroup FeatureGroup {
			get {
				var number = (NSNumber?) Dictionary [CTFontFeatureKey.Identifier];
				if (number is null)
					return default;
				return (FontFeatureGroup) (int) number;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Exclusive {
			get {
				return CFDictionary.GetBooleanValue (Dictionary,
						CTFontFeatureKey.Exclusive);
			}
			set {
				CFMutableDictionary.SetValue (Dictionary,
						CTFontFeatureKey.Exclusive,
						value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

	/// <summary>Encapsulates a font feature-dictionary. </summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureSelectors {

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureSelectors ()
			: this (new NSMutableDictionary ())
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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
#pragma warning disable CA1422 // This call site is reachable on: 'ios' 12.2 and later, 'maccatalyst' 12.2 and later, 'macOS/OSX' 12.0 and later, 'tvos' 12.2 and later. 'CTFontFeatureLetterCase' is obsoleted on: 'ios' 6.0 and later, 'maccatalyst' 6.0 and later, 'macOS/OSX' 10.7 and later.
			case FontFeatureGroup.LetterCase:
				return new CTFontFeatureLetterCase (dictionary);
#pragma warning restore CA1422
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary Dictionary { get; private set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		protected int FeatureWeak {
			get {
				var number = (NSNumber?) Dictionary [CTFontFeatureSelectorKey.Identifier];
				if (number is null)
					return default;
				return (int) number;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Name {
			get { return Adapter.GetStringValue (Dictionary, CTFontFeatureSelectorKey.Name); }
			set { Adapter.SetValue (Dictionary, CTFontFeatureSelectorKey.Name, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Default {
			get {
				return CFDictionary.GetBooleanValue (Dictionary,
						CTFontFeatureSelectorKey.Default);
			}
			set {
				CFMutableDictionary.SetValue (Dictionary,
						CTFontFeatureSelectorKey.Default,
						value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Setting {
			get {
				return CFDictionary.GetBooleanValue (Dictionary,
						CTFontFeatureSelectorKey.Setting);
			}
			set {
				CFMutableDictionary.SetValue (Dictionary,
						CTFontFeatureSelectorKey.Setting,
						value);
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that represents all type features.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureAllTypographicFeatures : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values can be used as arguments for <see cref="CoreText.CTFontDescriptor.WithFeature(CoreText.CTFontFeatureVerticalSubstitutionConnection.Selector)" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			AllTypeFeaturesOn = 0,
			/// <summary>To be added.</summary>
			AllTypeFeaturesOff = 1,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureAllTypographicFeatures (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe whether ligature features are on or off.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureLigatures : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureLigatures.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			RequiredLigaturesOn = 0,
			/// <summary>To be added.</summary>
			RequiredLigaturesOff = 1,
			/// <summary>To be added.</summary>
			CommonLigaturesOn = 2,
			/// <summary>To be added.</summary>
			CommonLigaturesOff = 3,
			/// <summary>To be added.</summary>
			RareLigaturesOn = 4,
			/// <summary>To be added.</summary>
			RareLigaturesOff = 5,
			/// <summary>To be added.</summary>
			LogosOn = 6,
			/// <summary>To be added.</summary>
			LogosOff = 7,
			/// <summary>To be added.</summary>
			RebusPicturesOn = 8,
			/// <summary>To be added.</summary>
			RebusPicturesOff = 9,
			/// <summary>To be added.</summary>
			DiphthongLigaturesOn = 10,
			/// <summary>To be added.</summary>
			DiphthongLigaturesOff = 11,
			/// <summary>To be added.</summary>
			SquaredLigaturesOn = 12,
			/// <summary>To be added.</summary>
			SquaredLigaturesOff = 13,
			/// <summary>To be added.</summary>
			AbbrevSquaredLigaturesOn = 14,
			/// <summary>To be added.</summary>
			AbbrevSquaredLigaturesOff = 15,
			/// <summary>To be added.</summary>
			SymbolLigaturesOn = 16,
			/// <summary>To be added.</summary>
			SymbolLigaturesOff = 17,
			/// <summary>To be added.</summary>
			ContextualLigaturesOn = 18,
			/// <summary>To be added.</summary>
			ContextualLigaturesOff = 19,
			/// <summary>To be added.</summary>
			HistoricalLigaturesOn = 20,
			/// <summary>To be added.</summary>
			HistoricalLigaturesOff = 21,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureLigatures (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to capitalization options such as initial capitalization.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[ObsoletedOSPlatform ("macos10.7")]
	[ObsoletedOSPlatform ("ios6.0")]
	[ObsoletedOSPlatform ("tvos")]
	[ObsoletedOSPlatform ("maccatalyst")]
	public class CTFontFeatureLetterCase : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureLetterCase.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			UpperAndLowerCase = 0,
			/// <summary>To be added.</summary>
			AllCaps = 1,
			/// <summary>To be added.</summary>
			AllLowerCase = 2,
			/// <summary>To be added.</summary>
			SmallCaps = 3,
			/// <summary>To be added.</summary>
			InitialCaps = 4,
			/// <summary>To be added.</summary>
			InitialCapsAndSmallCaps = 5,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureLetterCase (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to the connection of cursive letters.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureCursiveConnection : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCursiveConnection.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			Unconnected = 0,
			/// <summary>To be added.</summary>
			PartiallyConnected = 1,
			/// <summary>To be added.</summary>
			Cursive = 2,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureCursiveConnection (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to vertical substitution.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureVerticalSubstitutionConnection : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureVerticalSubstitutionConnection.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			SubstituteVerticalFormsOn = 0,
			/// <summary>To be added.</summary>
			SubstituteVerticalFormsOff = 1,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureVerticalSubstitutionConnection (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe whether linguistic rearrangement is on or off.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureLinguisticRearrangementConnection : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureLinguisticRearrangementConnection.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			LinguisticRearrangementOn = 0,
			/// <summary>To be added.</summary>
			LinguisticRearrangementOff = 1,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureLinguisticRearrangementConnection (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to spacing of numbers.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureNumberSpacing : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureNumberSpacing.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			MonospacedNumbers = 0,
			/// <summary>To be added.</summary>
			ProportionalNumbers = 1,
			/// <summary>To be added.</summary>
			ThirdWidthNumbers = 2,
			/// <summary>To be added.</summary>
			QuarterWidthNumbers = 3,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureNumberSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to smart swashes.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureSmartSwash : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureSmartSwash.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			WordInitialSwashesOn = 0,
			/// <summary>To be added.</summary>
			WordInitialSwashesOff = 1,
			/// <summary>To be added.</summary>
			WordFinalSwashesOn = 2,
			/// <summary>To be added.</summary>
			WordFinalSwashesOff = 3,
			/// <summary>To be added.</summary>
			LineInitialSwashesOn = 4,
			/// <summary>To be added.</summary>
			LineInitialSwashesOff = 5,
			/// <summary>To be added.</summary>
			LineFinalSwashesOn = 6,
			/// <summary>To be added.</summary>
			LineFinalSwashesOff = 7,
			/// <summary>To be added.</summary>
			NonFinalSwashesOn = 8,
			/// <summary>To be added.</summary>
			NonFinalSwashesOff = 9,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureSmartSwash (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to the visibility and composition of diacritical marks.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureDiacritics : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureDiacritics.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			ShowDiacritics = 0,
			/// <summary>To be added.</summary>
			HideDiacritics = 1,
			/// <summary>To be added.</summary>
			DecomposeDiacritics = 2,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureDiacritics (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to vertical positioning.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureVerticalPosition : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureVerticalPosition.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NormalPosition = 0,
			/// <summary>To be added.</summary>
			Superiors = 1,
			/// <summary>To be added.</summary>
			Inferiors = 2,
			/// <summary>To be added.</summary>
			Ordinals = 3,
			/// <summary>To be added.</summary>
			ScientificInferiors = 4,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureVerticalPosition (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to how fractions should be displayed.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureFractions : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureFractions.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoFractions = 0,
			/// <summary>To be added.</summary>
			VerticalFractions = 1,
			/// <summary>To be added.</summary>
			DiagonalFractions = 2,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureFractions (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that allow or disallow characters to overlap.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureOverlappingCharacters : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureOverlappingCharacters.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			PreventOverlapOn = 0,
			/// <summary>To be added.</summary>
			PreventOverlapOff = 1,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureOverlappingCharacters (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to typographic extras such as interrobangs, conversion of dashes to em- or en-dashes, etc..</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureTypographicExtras : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureTypographicExtras.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			HyphensToEmDashOn = 0,
			/// <summary>To be added.</summary>
			HyphensToEmDashOff = 1,
			/// <summary>To be added.</summary>
			HyphenToEnDashOn = 2,
			/// <summary>To be added.</summary>
			HyphenToEnDashOff = 3,
			/// <summary>To be added.</summary>
			SlashedZeroOn = 4,
			/// <summary>To be added.</summary>
			SlashedZeroOff = 5,
			/// <summary>To be added.</summary>
			FormInterrobangOn = 6,
			/// <summary>To be added.</summary>
			FormInterrobangOff = 7,
			/// <summary>To be added.</summary>
			SmartQuotesOn = 8,
			/// <summary>To be added.</summary>
			SmartQuotesOff = 9,
			/// <summary>To be added.</summary>
			PeriodsToEllipsisOn = 10,
			/// <summary>To be added.</summary>
			PeriodsToEllipsisOff = 11,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureTypographicExtras (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to mathematical formulae.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureMathematicalExtras : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureMathematicalExtras.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			HyphenToMinusOn = 0,
			/// <summary>To be added.</summary>
			HyphenToMinusOff = 1,
			/// <summary>To be added.</summary>
			AsteriskToMultiplyOn = 2,
			/// <summary>To be added.</summary>
			AsteriskToMultiplyOff = 3,
			/// <summary>To be added.</summary>
			SlashToDivideOn = 4,
			/// <summary>To be added.</summary>
			SlashToDivideOff = 5,
			/// <summary>To be added.</summary>
			InequalityLigaturesOn = 6,
			/// <summary>To be added.</summary>
			InequalityLigaturesOff = 7,
			/// <summary>To be added.</summary>
			ExponentsOn = 8,
			/// <summary>To be added.</summary>
			ExponentsOff = 9,
			/// <summary>To be added.</summary>
			MathematicalGreekOn = 10,
			/// <summary>To be added.</summary>
			MathematicalGreekOff = 11,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureMathematicalExtras (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to case-sensitive spacing or layout.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureOrnamentSets : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureOrnamentSets.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoOrnaments = 0,
			/// <summary>To be added.</summary>
			Dingbats = 1,
			/// <summary>To be added.</summary>
			PiCharacters = 2,
			/// <summary>To be added.</summary>
			Fleurons = 3,
			/// <summary>To be added.</summary>
			DecorativeBorders = 4,
			/// <summary>To be added.</summary>
			InternationalSymbols = 5,
			/// <summary>To be added.</summary>
			MathSymbols = 6,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureOrnamentSets (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe a feature allowing character alternatives.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureCharacterAlternatives : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCharacterAlternatives.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoAlternates = 0,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureCharacterAlternatives (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to design-level complexity.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureDesignComplexity : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureDesignComplexity.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			DesignLevel1 = 0,
			/// <summary>To be added.</summary>
			DesignLevel2 = 1,
			/// <summary>To be added.</summary>
			DesignLevel3 = 2,
			/// <summary>To be added.</summary>
			DesignLevel4 = 3,
			/// <summary>To be added.</summary>
			DesignLevel5 = 4,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureDesignComplexity (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to font features such as illuminated capitals and engraved text.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureStyleOptions : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureStyleOptions.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoStyleOptions = 0,
			/// <summary>To be added.</summary>
			DisplayText = 1,
			/// <summary>To be added.</summary>
			EngravedText = 2,
			/// <summary>To be added.</summary>
			IlluminatedCaps = 3,
			/// <summary>To be added.</summary>
			TitlingCaps = 4,
			/// <summary>To be added.</summary>
			TallCaps = 5,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureStyleOptions (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to character shapes such as Hojo Kanji forms, JIS 78 Forms, etc..</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureCharacterShape : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCharacterShape.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			TraditionalCharacters = 0,
			/// <summary>To be added.</summary>
			SimplifiedCharacters = 1,
			/// <summary>To be added.</summary>
			JIS1978Characters = 2,
			/// <summary>To be added.</summary>
			JIS1983Characters = 3,
			/// <summary>To be added.</summary>
			JIS1990Characters = 4,
			/// <summary>To be added.</summary>
			TraditionalAltOne = 5,
			/// <summary>To be added.</summary>
			TraditionalAltTwo = 6,
			/// <summary>To be added.</summary>
			TraditionalAltThree = 7,
			/// <summary>To be added.</summary>
			TraditionalAltFour = 8,
			/// <summary>To be added.</summary>
			TraditionalAltFive = 9,
			/// <summary>To be added.</summary>
			ExpertCharacters = 10,
			/// <summary>To be added.</summary>
			JIS2004Characters = 11,
			/// <summary>To be added.</summary>
			HojoCharacters = 12,
			/// <summary>To be added.</summary>
			NLCCharacters = 13,
			/// <summary>To be added.</summary>
			TraditionalNamesCharacters = 14,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureCharacterShape (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to the display of capital numbers.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureNumberCase : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureNumberCase.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			LowerCaseNumbers = 0,
			/// <summary>To be added.</summary>
			UpperCaseNumbers = 1,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureNumberCase (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to text spacing.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureTextSpacing : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureTextSpacing.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			ProportionalText = 0,
			/// <summary>To be added.</summary>
			MonospacedText = 1,
			/// <summary>To be added.</summary>
			HalfWidthText = 2,
			/// <summary>To be added.</summary>
			ThirdWidthText = 3,
			/// <summary>To be added.</summary>
			QuarterWidthText = 4,
			/// <summary>To be added.</summary>
			AltProportionalText = 5,
			/// <summary>To be added.</summary>
			AltHalfWidthText = 6,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureTextSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to transliteration.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureTransliteration : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureTransliteration.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoTransliteration = 0,
			/// <summary>To be added.</summary>
			HanjaToHangul = 1,
			/// <summary>To be added.</summary>
			HiraganaToKatakana = 2,
			/// <summary>To be added.</summary>
			KatakanaToHiragana = 3,
			/// <summary>To be added.</summary>
			KanaToRomanization = 4,
			/// <summary>To be added.</summary>
			RomanizationToHiragana = 5,
			/// <summary>To be added.</summary>
			RomanizationToKatakana = 6,
			/// <summary>To be added.</summary>
			HanjaToHangulAltOne = 7,
			/// <summary>To be added.</summary>
			HanjaToHangulAltTwo = 8,
			/// <summary>To be added.</summary>
			HanjaToHangulAltThree = 9,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureTransliteration (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe feature annotations.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureAnnotation : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureAnnotation.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoAnnotation = 0,
			/// <summary>To be added.</summary>
			BoxAnnotation = 1,
			/// <summary>To be added.</summary>
			RoundedBoxAnnotation = 2,
			/// <summary>To be added.</summary>
			CircleAnnotation = 3,
			/// <summary>To be added.</summary>
			InvertedCircleAnnotation = 4,
			/// <summary>To be added.</summary>
			ParenthesisAnnotation = 5,
			/// <summary>To be added.</summary>
			PeriodAnnotation = 6,
			/// <summary>To be added.</summary>
			RomanNumeralAnnotation = 7,
			/// <summary>To be added.</summary>
			DiamondAnnotation = 8,
			/// <summary>To be added.</summary>
			InvertedBoxAnnotation = 9,
			/// <summary>To be added.</summary>
			InvertedRoundedBoxAnnotation = 10,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureAnnotation (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to Kana spacing.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureKanaSpacing : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCaseSensitiveLayout.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			FullWidthKana = 0,
			/// <summary>To be added.</summary>
			ProportionalKana = 1,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureKanaSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to ideographic spacing.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureIdeographicSpacing : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureIdeographicSpacing.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			FullWidthIdeographs = 0,
			/// <summary>To be added.</summary>
			ProportionalIdeographs = 1,
			/// <summary>To be added.</summary>
			HalfWidthIdeographs = 2,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureIdeographicSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to how Unicode is decomposed.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureUnicodeDecomposition : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureUnicodeDecomposition.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			CanonicalCompositionOn = 0,
			/// <summary>To be added.</summary>
			CanonicalCompositionOff = 1,
			/// <summary>To be added.</summary>
			CompatibilityCompositionOn = 2,
			/// <summary>To be added.</summary>
			CompatibilityCompositionOff = 3,
			/// <summary>To be added.</summary>
			TranscodingCompositionOn = 4,
			/// <summary>To be added.</summary>
			TranscodingCompositionOff = 5,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureUnicodeDecomposition (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to applications of rubies to Kana.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureRubyKana : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureRubyKana.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>Developers should not use this deprecated field. </summary>
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[UnsupportedOSPlatform ("tvos")]
			[UnsupportedOSPlatform ("maccatalyst")]
			[ObsoletedOSPlatform ("macos10.8")]
			[ObsoletedOSPlatform ("ios5.1")]
			NoRubyKana = 0,
			/// <summary>To be added.</summary>
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[UnsupportedOSPlatform ("tvos")]
			[UnsupportedOSPlatform ("maccatalyst")]
			[ObsoletedOSPlatform ("macos10.8")]
			[ObsoletedOSPlatform ("ios5.1")]
			RubyKana = 1,
			/// <summary>To be added.</summary>
			RubyKanaOn = 2,
			/// <summary>To be added.</summary>
			RubyKanaOff = 3,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureRubyKana (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to Chines, Japanese, and Korean typography.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureCJKSymbolAlternatives : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCJKSymbolAlternatives.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoCJKSymbolAlternatives = 0,
			/// <summary>To be added.</summary>
			CJKSymbolAltOne = 1,
			/// <summary>To be added.</summary>
			CJKSymbolAltTwo = 2,
			/// <summary>To be added.</summary>
			CJKSymbolAltThree = 3,
			/// <summary>To be added.</summary>
			CJKSymbolAltFour = 4,
			/// <summary>To be added.</summary>
			CJKSymbolAltFive = 5,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureCJKSymbolAlternatives (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to ideographic alternatives.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureIdeographicAlternatives : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureIdeographicAlternatives.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoIdeographicAlternatives = 0,
			/// <summary>To be added.</summary>
			IdeographicAltOne = 1,
			/// <summary>To be added.</summary>
			IdeographicAltTwo = 2,
			/// <summary>To be added.</summary>
			IdeographicAltThree = 3,
			/// <summary>To be added.</summary>
			IdeographicAltFour = 4,
			/// <summary>To be added.</summary>
			IdeographicAltFive = 5,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureIdeographicAlternatives (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to Chines, Japanese, and Korean typography.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureCJKVerticalRomanPlacement : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCJKVerticalRomanPlacement.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			CJKVerticalRomanCentered = 0,
			/// <summary>To be added.</summary>
			CJKVerticalRomanHBaseline = 1,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureCJKVerticalRomanPlacement (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to Chines, Japanese, and Korean italicized text.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureItalicCJKRoman : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureItalicCJKRoman.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[UnsupportedOSPlatform ("tvos")]
			[UnsupportedOSPlatform ("maccatalyst")]
			[ObsoletedOSPlatform ("macos10.8")]
			[ObsoletedOSPlatform ("ios5.1")]
			NoCJKItalicRoman = 0,
			/// <summary>Developers should not use this deprecated field. </summary>
			[SupportedOSPlatform ("ios")]
			[SupportedOSPlatform ("macos")]
			[UnsupportedOSPlatform ("tvos")]
			[UnsupportedOSPlatform ("maccatalyst")]
			[ObsoletedOSPlatform ("macos10.8")]
			[ObsoletedOSPlatform ("ios5.1")]
			CJKItalicRoman = 1,
			/// <summary>To be added.</summary>
			CJKItalicRomanOn = 2,
			/// <summary>To be added.</summary>
			CJKItalicRomanOff = 3,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureItalicCJKRoman (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to case-sensitive spacing or layout.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureCaseSensitiveLayout : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCaseSensitiveLayout.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			CaseSensitiveLayoutOn = 0,
			/// <summary>To be added.</summary>
			CaseSensitiveLayoutOff = 1,
			/// <summary>To be added.</summary>
			CaseSensitiveSpacingOn = 2,
			/// <summary>To be added.</summary>
			CaseSensitiveSpacingOff = 3,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureCaseSensitiveLayout (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> for alternate kana.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureAlternateKana : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureAlternateKana.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			AlternateHorizKanaOn = 0,
			/// <summary>To be added.</summary>
			AlternateHorizKanaOff = 1,
			/// <summary>To be added.</summary>
			AlternateVertKanaOn = 2,
			/// <summary>To be added.</summary>
			AlternateVertKanaOff = 3,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureAlternateKana (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to alternative styles.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureStylisticAlternatives : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCaseSensitiveLayout.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			NoStylisticAlternates = 0,
			/// <summary>To be added.</summary>
			StylisticAltOneOn = 2,
			/// <summary>To be added.</summary>
			StylisticAltOneOff = 3,
			/// <summary>To be added.</summary>
			StylisticAltTwoOn = 4,
			/// <summary>To be added.</summary>
			StylisticAltTwoOff = 5,
			/// <summary>To be added.</summary>
			StylisticAltThreeOn = 6,
			/// <summary>To be added.</summary>
			StylisticAltThreeOff = 7,
			/// <summary>To be added.</summary>
			StylisticAltFourOn = 8,
			/// <summary>To be added.</summary>
			StylisticAltFourOff = 9,
			/// <summary>To be added.</summary>
			StylisticAltFiveOn = 10,
			/// <summary>To be added.</summary>
			StylisticAltFiveOff = 11,
			/// <summary>To be added.</summary>
			StylisticAltSixOn = 12,
			/// <summary>To be added.</summary>
			StylisticAltSixOff = 13,
			/// <summary>To be added.</summary>
			StylisticAltSevenOn = 14,
			/// <summary>To be added.</summary>
			StylisticAltSevenOff = 15,
			/// <summary>To be added.</summary>
			StylisticAltEightOn = 16,
			/// <summary>To be added.</summary>
			StylisticAltEightOff = 17,
			/// <summary>To be added.</summary>
			StylisticAltNineOn = 18,
			/// <summary>To be added.</summary>
			StylisticAltNineOff = 19,
			/// <summary>To be added.</summary>
			StylisticAltTenOn = 20,
			/// <summary>To be added.</summary>
			StylisticAltTenOff = 21,
			/// <summary>To be added.</summary>
			StylisticAltElevenOn = 22,
			/// <summary>To be added.</summary>
			StylisticAltElevenOff = 23,
			/// <summary>To be added.</summary>
			StylisticAltTwelveOn = 24,
			/// <summary>To be added.</summary>
			StylisticAltTwelveOff = 25,
			/// <summary>To be added.</summary>
			StylisticAltThirteenOn = 26,
			/// <summary>To be added.</summary>
			StylisticAltThirteenOff = 27,
			/// <summary>To be added.</summary>
			StylisticAltFourteenOn = 28,
			/// <summary>To be added.</summary>
			StylisticAltFourteenOff = 29,
			/// <summary>To be added.</summary>
			StylisticAltFifteenOn = 30,
			/// <summary>To be added.</summary>
			StylisticAltFifteenOff = 31,
			/// <summary>To be added.</summary>
			StylisticAltSixteenOn = 32,
			/// <summary>To be added.</summary>
			StylisticAltSixteenOff = 33,
			/// <summary>To be added.</summary>
			StylisticAltSeventeenOn = 34,
			/// <summary>To be added.</summary>
			StylisticAltSeventeenOff = 35,
			/// <summary>To be added.</summary>
			StylisticAltEighteenOn = 36,
			/// <summary>To be added.</summary>
			StylisticAltEighteenOff = 37,
			/// <summary>To be added.</summary>
			StylisticAltNineteenOn = 38,
			/// <summary>To be added.</summary>
			StylisticAltNineteenOff = 39,
			/// <summary>To be added.</summary>
			StylisticAltTwentyOn = 40,
			/// <summary>To be added.</summary>
			StylisticAltTwentyOff = 41,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureStylisticAlternatives (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to swash alternatives.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureContextualAlternates : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureContextualAlternates.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			ContextualAlternatesOn = 0,
			/// <summary>To be added.</summary>
			ContextualAlternatesOff = 1,
			/// <summary>To be added.</summary>
			SwashAlternatesOn = 2,
			/// <summary>To be added.</summary>
			SwashAlternatesOff = 3,
			/// <summary>To be added.</summary>
			ContextualSwashAlternatesOn = 4,
			/// <summary>To be added.</summary>
			ContextualSwashAlternatesOff = 5,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureContextualAlternates (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to how lower-case letters are rendered.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureLowerCase : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureLowerCase.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			DefaultLowerCase = 0,
			/// <summary>To be added.</summary>
			LowerCaseSmallCaps = 1,
			/// <summary>To be added.</summary>
			LowerCasePetiteCaps = 2,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureLowerCase (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to how upper-case letters should be displayed.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureUpperCase : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureUpperCase.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			DefaultUpperCase = 0,
			/// <summary>To be added.</summary>
			UpperCaseSmallCaps = 1,
			/// <summary>To be added.</summary>
			UpperCasePetiteCaps = 2,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureUpperCase (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>A <see cref="CoreText.CTFontFeatureSelectors" /> that describe features related to Chines, Japanese, and Korean typography.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureCJKRomanSpacing : CTFontFeatureSelectors {
		/// <summary>An enumeration whose values are returned by <see cref="CoreText.CTFontFeatureCJKRomanSpacing.Feature" />.</summary>
		///     <remarks>To be added.</remarks>
		public enum Selector {
			/// <summary>To be added.</summary>
			HalfWidthCJKRoman = 0,
			/// <summary>To be added.</summary>
			ProportionalCJKRoman = 1,
			/// <summary>To be added.</summary>
			DefaultCJKRoman = 2,
			/// <summary>To be added.</summary>
			FullWidthCJKRoman = 3,
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureCJKRomanSpacing (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public Selector Feature {
			get {
				return (Selector) FeatureWeak;
			}
		}
	}

	/// <summary>The feature settings of a <see cref="CoreText.CTFont" /> or <see cref="CoreText.CTFontDescriptorAttributes" />.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontFeatureSettings {

		internal CTFontFeatureSettings (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary Dictionary { get; private set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public FontFeatureGroup FeatureGroup {
			get {
				var number = (NSNumber?) Dictionary [CTFontFeatureKey.Identifier];
				if (number is null)
					return default;
				return (FontFeatureGroup) (int) number;
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int FeatureWeak {
			get {
				var number = (NSNumber?) Dictionary [CTFontFeatureSelectorKey.Identifier];
				if (number is null)
					return default;
				return (int) number;
			}
		}
	}

	/// <summary>Encapsulates a font-variation-axis dictionary.</summary>
	///     <remarks>To be added.</remarks>
	///     <altmember cref="CoreText.CTFontVariationAxisKey" />
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontVariationAxes {

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontVariationAxes ()
			: this (new NSMutableDictionary ())
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontVariationAxes (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary Dictionary { get; private set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSNumber? Identifier {
			get { return (NSNumber?) Dictionary [CTFontVariationAxisKey.Identifier]; }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.Identifier, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSNumber? MinimumValue {
			get { return (NSNumber?) Dictionary [CTFontVariationAxisKey.MinimumValue]; }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.MinimumValue, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSNumber? MaximumValue {
			get { return (NSNumber?) Dictionary [CTFontVariationAxisKey.MaximumValue]; }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.MaximumValue, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSNumber? DefaultValue {
			get { return (NSNumber?) Dictionary [CTFontVariationAxisKey.DefaultValue]; }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.DefaultValue, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Name {
			get { return Adapter.GetStringValue (Dictionary, CTFontVariationAxisKey.Name); }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.Name, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool? Hidden {
			get { return Adapter.GetBoolValue (Dictionary, CTFontVariationAxisKey.Hidden); }
			set { Adapter.SetValue (Dictionary, CTFontVariationAxisKey.Hidden, value); }
		}
	}

	/// <summary>Encapsulates a font-variation dictionary.</summary>
	///     <remarks>To be added.</remarks>
	///     <altmember cref="CoreText.CTFont.GetVariation" />
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontVariation {

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontVariation ()
			: this (new NSMutableDictionary ())
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTFontVariation (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary Dictionary { get; private set; }
	}

	/// <summary>Represents a CoreText Font.</summary>
	///     <remarks>
	///       <para>
	/// 	CoreText does not synthesize font styles (italic and bold).
	/// 	This means that if you pick a font that has neither a Bolded
	/// 	or Italicized versions available, CoreText will not create a
	/// 	dynamic font that is merely a slanted version of the font for
	/// 	italic, or a boldened version from the original font.  In
	/// 	those cases, if you want to synthesize the font, you could
	/// 	apply a Matrix transformation to slant the font (it will still
	/// 	be wrong, but will look slanted).  For bolding, you could
	/// 	stroke the font twice, or manually extend the glyph path.
	///
	///       </para>
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/SimpleTextInput/">SimpleTextInput</related>
	public partial class CTFont : NativeObject {
		[Preserve (Conditional = true)]
		internal CTFont (NativeHandle handle, bool owns)
			: base (handle, owns, true)
		{
		}

		#region Font Creation
		static IntPtr Create (string name, nfloat size)
		{
			var n = CFString.CreateNative (name);
			try {
				IntPtr handle;
				unsafe {
					handle = CTFontCreateWithName (n, size, null);
				}
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
		unsafe static extern IntPtr CTFontCreateWithName (IntPtr name, nfloat size, CGAffineTransform* matrix);

		static IntPtr Create (string name, nfloat size, ref CGAffineTransform matrix)
		{
			var n = CFString.CreateNative (name);
			try {
				IntPtr handle;
				unsafe {
					fixed (CGAffineTransform* matrixPtr = &matrix)
						handle = CTFontCreateWithName (n, size, matrixPtr);
				}
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

		static IntPtr Create (CTFontDescriptor descriptor, nfloat size)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			IntPtr handle;
			unsafe {
				handle = CTFontCreateWithFontDescriptor (descriptor.Handle, size, null);
				GC.KeepAlive (descriptor);
			}
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		public CTFont (CTFontDescriptor descriptor, nfloat size)
			: base (Create (descriptor, size), true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern IntPtr CTFontCreateWithFontDescriptor (IntPtr descriptor, nfloat size, CGAffineTransform* matrix);

		static IntPtr Create (CTFontDescriptor descriptor, nfloat size, ref CGAffineTransform matrix)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			IntPtr handle;
			unsafe {
				fixed (CGAffineTransform* matrixPtr = &matrix) {
					handle = CTFontCreateWithFontDescriptor (descriptor.Handle, size, matrixPtr);
					GC.KeepAlive (descriptor);
				}
			}
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		public CTFont (CTFontDescriptor descriptor, nfloat size, ref CGAffineTransform matrix)
			: base (Create (descriptor, size, ref matrix), true)
		{
		}

		static IntPtr Create (string name, nfloat size, CTFontOptions options)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			var n = CFString.CreateNative (name);
			try {
				IntPtr handle;
				unsafe {
					handle = CTFontCreateWithNameAndOptions (n, size, null, (nuint) (ulong) options);
				}
				if (handle == IntPtr.Zero)
					throw ConstructorError.Unknown (typeof (CTFont));
				return handle;
			} finally {
				CFString.ReleaseNative (n);
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public CTFont (string name, nfloat size, CTFontOptions options)
			: base (Create (name, size, options), true)
		{
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern IntPtr CTFontCreateWithNameAndOptions (IntPtr name, nfloat size, CGAffineTransform* matrix, nuint options);

		static IntPtr Create (string name, nfloat size, ref CGAffineTransform matrix, CTFontOptions options)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			var n = CFString.CreateNative (name);
			try {
				IntPtr handle;
				unsafe {
					fixed (CGAffineTransform* matrixPtr = &matrix) {
						handle = CTFontCreateWithNameAndOptions (n, size, matrixPtr, (nuint) (ulong) options);
					}
				}
				if (handle == IntPtr.Zero)
					throw ConstructorError.Unknown (typeof (CTFont));
				return handle;
			} finally {
				CFString.ReleaseNative (n);
			}
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public CTFont (string name, nfloat size, ref CGAffineTransform matrix, CTFontOptions options)
			: base (Create (name, size, ref matrix, options), true)
		{
		}

		static IntPtr Create (CTFontDescriptor descriptor, nfloat size, CTFontOptions options)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			IntPtr handle;
			unsafe {
				handle = CTFontCreateWithFontDescriptorAndOptions (descriptor.Handle, size, null, (nuint) (ulong) options);
				GC.KeepAlive (descriptor);
			}
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public CTFont (CTFontDescriptor descriptor, nfloat size, CTFontOptions options)
			: base (Create (descriptor, size, options), true)
		{
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern IntPtr CTFontCreateWithFontDescriptorAndOptions (IntPtr descriptor, nfloat size, CGAffineTransform* matrix, nuint options);

		static IntPtr Create (CTFontDescriptor descriptor, nfloat size, CTFontOptions options, ref CGAffineTransform matrix)
		{
			if (descriptor is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptor));
			IntPtr handle;
			unsafe {
				fixed (CGAffineTransform* matrixPtr = &matrix) {
					handle = CTFontCreateWithFontDescriptorAndOptions (descriptor.Handle, size, matrixPtr, (nuint) (ulong) options);
					GC.KeepAlive (descriptor);
				}
			}
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public CTFont (CTFontDescriptor descriptor, nfloat size, CTFontOptions options, ref CGAffineTransform matrix)
			: base (Create (descriptor, size, options, ref matrix), true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern /* CTFontRef __nonnull */ IntPtr CTFontCreateWithGraphicsFont (
			/* CGFontRef __nonnull */ IntPtr cgfontRef, nfloat size,
			/* const CGAffineTransform * __nullable */ CGAffineTransform* affine,
			/* CTFontDescriptorRef __nullable */ IntPtr attrs);

		static IntPtr Create (CGFont font, nfloat size, CGAffineTransform transform, CTFontDescriptor descriptor)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
			IntPtr handle;
			unsafe {
				handle = CTFontCreateWithGraphicsFont (font.Handle, size, &transform, descriptor.GetHandle ());
				GC.KeepAlive (font);
				GC.KeepAlive (descriptor);
			}
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (typeof (CTFont));
			return handle;
		}

		public CTFont (CGFont font, nfloat size, CGAffineTransform transform, CTFontDescriptor descriptor)
			: base (Create (font, size, transform, descriptor), true)
		{
		}

		static IntPtr Create (CGFont font, nfloat size, CTFontDescriptor descriptor)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
			IntPtr handle;
			unsafe {
				handle = CTFontCreateWithGraphicsFont (font.Handle, size, null, descriptor.GetHandle ());
				GC.KeepAlive (font);
				GC.KeepAlive (descriptor);
			}
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
			IntPtr handle;
			unsafe {
				handle = CTFontCreateWithGraphicsFont (font.Handle, size, &transform, IntPtr.Zero);
				GC.KeepAlive (font);
			}
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

		public CTFont? WithAttributes (nfloat size, CTFontDescriptor attributes)
		{
			if (attributes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (attributes));
			unsafe {
				CTFont? result = CreateFont (CTFontCreateCopyWithAttributes (Handle, size, null, attributes.Handle));
				GC.KeepAlive (attributes);
				return result;
			}
		}

		static CTFont? CreateFont (IntPtr h)
		{
			if (h == IntPtr.Zero)
				return null;
			return new CTFont (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern IntPtr CTFontCreateCopyWithAttributes (IntPtr font, nfloat size, CGAffineTransform* matrix, IntPtr attributes);
		public CTFont? WithAttributes (nfloat size, CTFontDescriptor attributes, ref CGAffineTransform matrix)
		{
			unsafe {
				fixed (CGAffineTransform* matrixPtr = &matrix) {
					CTFont? result = CreateFont (CTFontCreateCopyWithAttributes (Handle, size, matrixPtr, attributes.GetHandle ()));
					GC.KeepAlive (attributes);
					return result;
				}
			}
		}

		public CTFont? WithSymbolicTraits (nfloat size, CTFontSymbolicTraits symTraitValue, CTFontSymbolicTraits symTraitMask)
		{
			unsafe {
				return CreateFont (CTFontCreateCopyWithSymbolicTraits (Handle, size, null, symTraitValue, symTraitMask));
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern IntPtr CTFontCreateCopyWithSymbolicTraits (IntPtr font, nfloat size, CGAffineTransform* matrix, CTFontSymbolicTraits symTraitValue, CTFontSymbolicTraits symTraitMask);
		public CTFont? WithSymbolicTraits (nfloat size, CTFontSymbolicTraits symTraitValue, CTFontSymbolicTraits symTraitMask, ref CGAffineTransform matrix)
		{
			unsafe {
				fixed (CGAffineTransform* matrixPtr = &matrix) {
					return CreateFont (CTFontCreateCopyWithSymbolicTraits (Handle, size, matrixPtr, symTraitValue, symTraitMask));
				}
			}
		}

		public CTFont? WithFamily (nfloat size, string family)
		{
			if (family is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (family));
			var n = CFString.CreateNative (family);
			try {
				unsafe {
					return CreateFont (CTFontCreateCopyWithFamily (Handle, size, null, n));
				}
			} finally {
				CFString.ReleaseNative (n);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern IntPtr CTFontCreateCopyWithFamily (IntPtr font, nfloat size, CGAffineTransform* matrix, IntPtr family);
		public CTFont? WithFamily (nfloat size, string family, ref CGAffineTransform matrix)
		{
			if (family is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (family));
			var n = CFString.CreateNative (family);
			try {
				unsafe {
					fixed (CGAffineTransform* matrixPtr = &matrix) {
						return CreateFont (CTFontCreateCopyWithFamily (Handle, size, matrixPtr, n));
					}
				}
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

		/// <param name="value">To be added.</param>
		///         <param name="range">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CTFontRef */ IntPtr CTFontCreateForStringWithLanguage (
			/* CTFontRef */ IntPtr currentFont,
			/* CFStringRef */ IntPtr @string,
			NSRange range,
			/* CFStringRef _Nullable */ IntPtr language);

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
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

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CTFontDescriptor GetFontDescriptor ()
		{
			var h = CTFontCopyFontDescriptor (Handle);
			return new CTFontDescriptor (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFTypeRef __nullable */ IntPtr CTFontCopyAttribute (/* CTFontRef __nonnull */ IntPtr font,
			/* CFStringRef __nonnull */ IntPtr attribute);

		/// <param name="attribute">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public NSObject? GetAttribute (NSString attribute)
		{
			if (attribute is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (attribute));
			var result = Runtime.GetNSObject (CTFontCopyAttribute (Handle, attribute.Handle), true);
			GC.KeepAlive (attribute);
			return result;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetSize (IntPtr font);
		/// <summary>The font size.</summary>
		///         <value>
		///         </value>
		///         <remarks>This is the size that was used when the font was constructed.</remarks>
		public nfloat Size {
			get { return CTFontGetSize (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGAffineTransform CTFontGetMatrix (/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>The transformation matrix used when this font was created.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public CGAffineTransform Matrix {
			get { return CTFontGetMatrix (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CTFontSymbolicTraits CTFontGetSymbolicTraits (IntPtr font);
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CTFontSymbolicTraits SymbolicTraits {
			get { return CTFontGetSymbolicTraits (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CTFontUIFontType CTFontGetUIFontType (IntPtr font);

		/// <summary>Gets the UI font type of the font.</summary>
		/// <value>The UI font type, or <see cref="CTFontUIFontType.None" /> if the font is not a UI font.</value>
		[SupportedOSPlatform ("ios26.4")]
		[SupportedOSPlatform ("maccatalyst26.4")]
		[SupportedOSPlatform ("macos26.4")]
		[SupportedOSPlatform ("tvos26.4")]
		public CTFontUIFontType UIFontType {
			get { return CTFontGetUIFontType (GetCheckedHandle ()); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyTraits (IntPtr font);
		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? PostScriptName {
			get { return CFString.FromHandle (CTFontCopyPostScriptName (Handle), releaseHandle: true); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFStringRef __nonnull */ IntPtr CTFontCopyFamilyName (
			/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? FamilyName {
			get { return CFString.FromHandle (CTFontCopyFamilyName (Handle), releaseHandle: true); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFStringRef __nonnull */ IntPtr CTFontCopyFullName (
			/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? FullName {
			get { return CFString.FromHandle (CTFontCopyFullName (Handle), releaseHandle: true); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFStringRef __nonnull */ IntPtr CTFontCopyDisplayName (
			/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? DisplayName {
			get { return CFString.FromHandle (CTFontCopyDisplayName (Handle), releaseHandle: true); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyName (IntPtr font, IntPtr nameKey);
		/// <param name="nameKey">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public string? GetName (CTFontNameKey nameKey)
		{
			var id = CTFontNameKeyId.ToId (nameKey);
			string? name = CFString.FromHandle (CTFontCopyName (Handle, id.GetHandle ()), releaseHandle: true);
			GC.KeepAlive (id);
			return name;
		}

		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern IntPtr CTFontCopyLocalizedName (IntPtr font, IntPtr nameKey, IntPtr* actualLanguage);

		/// <param name="nameKey">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public string? GetLocalizedName (CTFontNameKey nameKey)
		{
			return GetLocalizedName (nameKey, out _);
		}

		/// <param name="nameKey">To be added.</param>
		///         <param name="actualLanguage">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public string? GetLocalizedName (CTFontNameKey nameKey, out string? actualLanguage)
		{
			IntPtr actual;
			string? ret;
			unsafe {
				var id = CTFontNameKeyId.ToId (nameKey);
				ret = CFString.FromHandle (CTFontCopyLocalizedName (Handle, id.GetHandle (), &actual), releaseHandle: true);
				GC.KeepAlive (id);
			}
			actualLanguage = CFString.FromHandle (actual, releaseHandle: true);
			return ret;
		}
		#endregion

		#region Font Encoding
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFCharacterSetRef __nonnull */ IntPtr CTFontCopyCharacterSet (
			/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSCharacterSet? CharacterSet {
			get {
				return Runtime.GetNSObject<NSCharacterSet> (CTFontCopyCharacterSet (Handle), true);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern uint CTFontGetStringEncoding (IntPtr font);
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public uint StringEncoding {
			get { return CTFontGetStringEncoding (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopySupportedLanguages (IntPtr font);
		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public string? [] GetSupportedLanguages ()
		{
			var cfArrayRef = CTFontCopySupportedLanguages (Handle);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<string> ();
			return CFArray.StringArrayFromHandle (cfArrayRef, true)!;
		}

		[DllImport (Constants.CoreTextLibrary, CharSet = CharSet.Unicode)]
		unsafe static extern byte CTFontGetGlyphsForCharacters (IntPtr font, ushort* characters, CGGlyph* glyphs, nint count);

		public bool GetGlyphsForCharacters (char [] characters, CGGlyph [] glyphs, nint count)
		{
			AssertCount (count);
			AssertLength ("characters", characters, count);
			AssertLength ("glyphs", characters, count);

			unsafe {
				fixed (char* charactersPtr = characters) {
					fixed (CGGlyph* glyphsPtr = glyphs) {
						return CTFontGetGlyphsForCharacters (Handle, (ushort*) charactersPtr, glyphsPtr, count) != 0;
					}
				}
			}
		}

		/// <param name="characters">To be added.</param>
		///         <param name="glyphs">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool GetGlyphsForCharacters (char [] characters, CGGlyph [] glyphs)
		{
			return GetGlyphsForCharacters (characters, glyphs, Math.Min (characters.Length, glyphs.Length));
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe /* CFStringRef _Nullable */ IntPtr CTFontCopyNameForGlyph (/* CTFontRef */ IntPtr font, CGGlyph glyph);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nfloat AscentMetric {
			get { return CTFontGetAscent (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetDescent (/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nfloat DescentMetric {
			get { return CTFontGetDescent (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetLeading (/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nfloat LeadingMetric {
			get { return CTFontGetLeading (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern uint CTFontGetUnitsPerEm (IntPtr font);
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public uint UnitsPerEmMetric {
			get { return CTFontGetUnitsPerEm (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFIndex */ nint CTFontGetGlyphCount (/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nint GlyphCount {
			get { return CTFontGetGlyphCount (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGRect CTFontGetBoundingBox (/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CGRect BoundingBox {
			get { return CTFontGetBoundingBox (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetUnderlinePosition (IntPtr font);
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nfloat UnderlinePosition {
			get { return CTFontGetUnderlinePosition (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetUnderlineThickness (IntPtr font);
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nfloat UnderlineThickness {
			get { return CTFontGetUnderlineThickness (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetSlantAngle (IntPtr font);
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nfloat SlantAngle {
			get { return CTFontGetSlantAngle (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetCapHeight (/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nfloat CapHeightMetric {
			get { return CTFontGetCapHeight (Handle); }
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nfloat CTFontGetXHeight (IntPtr font);
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nfloat XHeightMetric {
			get { return CTFontGetXHeight (Handle); }
		}
		#endregion

		#region Font Glyphs
		[DllImport (Constants.CoreTextLibrary)]
		static extern CGGlyph CTFontGetGlyphWithName (/* CTFontRef __nonnull */ IntPtr font,
			/* CFStringRef __nonnull */ IntPtr glyphName);

		/// <param name="glyphName">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="orientation">To be added.</param>
		///         <param name="glyphs">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="orientation">To be added.</param>
		///         <param name="glyphs">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="glyph">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CGPath? GetPathForGlyph (CGGlyph glyph)
		{
			IntPtr h;
			unsafe {
				h = CTFontCreatePathForGlyph (Handle, glyph, null);
			}
			if (h == IntPtr.Zero)
				return null;
			return new CGPath (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		unsafe static extern IntPtr CTFontCreatePathForGlyph (IntPtr font, CGGlyph glyph, CGAffineTransform* transform);
		/// <param name="glyph">To be added.</param>
		///         <param name="transform">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CGPath? GetPathForGlyph (CGGlyph glyph, ref CGAffineTransform transform)
		{
			IntPtr h;
			unsafe {
				fixed (CGAffineTransform* transformPtr = &transform) {
					h = CTFontCreatePathForGlyph (Handle, glyph, transformPtr);
				}
			}
			if (h == IntPtr.Zero)
				return null;
			return new CGPath (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern void CTFontDrawGlyphs (/* CTFontRef __nonnull */ IntPtr font,
			[In] CGGlyph [] glyphs, [In] CGPoint [] positions, nint count,
			/* CGContextRef __nonnull */ IntPtr context);

		/// <param name="context">To be added.</param>
		///         <param name="glyphs">To be added.</param>
		///         <param name="positions">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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
			GC.KeepAlive (context);
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
		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CTFontVariationAxes [] GetVariationAxes ()
		{
			var cfArrayRef = CTFontCopyVariationAxes (Handle);
			return NSArray.NonNullDictionaryArrayFromHandleDropNullElements (cfArrayRef, d => new CTFontVariationAxes (d), true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyVariation (IntPtr font);
		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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
		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CTFontFeatures [] GetFeatures ()
		{
			var cfArrayRef = CTFontCopyFeatures (Handle);
			return NSArray.NonNullDictionaryArrayFromHandleDropNullElements (cfArrayRef,
					d => new CTFontFeatures (d), true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef __nullable */ IntPtr CTFontCopyFeatureSettings (
			/* CTFontRef __nonnull */ IntPtr font);

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CTFontFeatureSettings [] GetFeatureSettings ()
		{
			var cfArrayRef = CTFontCopyFeatureSettings (Handle);
			return NSArray.NonNullDictionaryArrayFromHandleDropNullElements (cfArrayRef,
					d => new CTFontFeatureSettings (d), true);
		}
		#endregion

		#region Font Conversion
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyGraphicsFont (IntPtr font, IntPtr attributes);
		/// <param name="attributes">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CGFont? ToCGFont (CTFontDescriptor? attributes)
		{
			var h = CTFontCopyGraphicsFont (Handle, attributes.GetHandle ());
			GC.KeepAlive (attributes);
			if (h == IntPtr.Zero)
				return null;
			return new CGFont (h, true);
		}

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CGFont? ToCGFont ()
		{
			return ToCGFont (null);
		}
		#endregion

		#region Font Tables
		[DllImport (Constants.CoreTextLibrary)]
		static extern /* CFArrayRef __nullable */ IntPtr CTFontCopyAvailableTables (
			/* CTFontRef __nonnull */ IntPtr font, CTFontTableOptions options);

		/// <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CTFontTable [] GetAvailableTables (CTFontTableOptions options)
		{
			var cfArrayRef = CTFontCopyAvailableTables (Handle, options);
			return NSArray.NonNullArrayFromHandle (cfArrayRef, v => {
				return (CTFontTable) (uint) (IntPtr) v;
			}, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCopyTable (IntPtr font, CTFontTable table, CTFontTableOptions options);
		/// <param name="table">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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

		/// <param name="languages">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CTFontDescriptor? []? GetDefaultCascadeList (string []? languages)
		{
			using (var arr = NSArray.FromNullableStrings (languages)) {
				var h = CTFontCopyDefaultCascadeListForLanguages (Handle, arr.GetHandle ());
				return CFArray.ArrayFromHandleFunc<CTFontDescriptor> (h,
					(handle) => new CTFontDescriptor (handle, false), true);
			}
		}

		#endregion

		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		[DllImport (Constants.CoreTextLibrary)]
		extern static /* CGRect */ CGRect CTFontGetTypographicBoundsForAdaptiveImageProvider (
			/* CTFontRef */ IntPtr font,
			/* id<CTAdaptiveImageProviding> __Nullable */ IntPtr provider);

		/// <summary>Computes metrics that clients performing their own typesetting of an adaptive image glyph need.</summary>
		/// <returns>The typographic bounds in points expressed as a rectangle, where the rectangle's Width property corresponds to the advance width, the rectangle's Bottom property corresponds to the ascent (above the baseline), and Top property corresponds to the descent (below the baseline).</returns>
		/// <param name="provider">The adaptive image provider used during the computation. If null, then default results will be returned, on the assumption that an image is not yet available.</param>
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		public CGRect GetTypographicBoundsForAdaptiveImageProvider (ICTAdaptiveImageProviding? provider)
		{
			CGRect result = CTFontGetTypographicBoundsForAdaptiveImageProvider (Handle, provider.GetHandle ());
			GC.KeepAlive (provider);
			return result;
		}

		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		[DllImport (Constants.CoreTextLibrary)]
		extern static void CTFontDrawImageFromAdaptiveImageProviderAtPoint (
			/* CTFontRef */ IntPtr font,
			/* id<CTAdaptiveImageProviding> __Nullable */ IntPtr provider,
			/* CGPoint */ CGPoint point,
			/* CGContexRef */ IntPtr context);

		/// <summary>Draws the image for an adaptive image glyph at the given point.</summary>
		/// <param name="provider">The adaptive image provider used during the rendering.</param>
		/// <param name="point">The adaptive image glyph is rendered relative to this point.</param>
		/// <param name="context">The <see cref="CoreGraphics.CGBitmapContext" /> where the adaptive image glyph is drawn.</param>
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		public void DrawImage (ICTAdaptiveImageProviding provider, CGPoint point, CGContext context)
		{
			CTFontDrawImageFromAdaptiveImageProviderAtPoint (Handle, provider.GetNonNullHandle (nameof (provider)), point, context.GetNonNullHandle (nameof (context)));
			GC.KeepAlive (provider);
			GC.KeepAlive (context);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreTextLibrary)]
		extern static byte CTFontHasTable (
			/* CTFontRef */ IntPtr font,
			/* CTFontTableTag */ CTFontTable tag);

		/// <summary>Checks whether a table is present in a font.</summary>
		/// <param name="tag">The table identifier to check for.</param>
		/// <returns>Whether the table is present in the font or not.</returns>
		/// <remarks>The check behaves as if <see cref="CTFontTableOptions.None" /> was specified.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public bool HasTable (CTFontTable tag)
		{
			return CTFontHasTable (GetCheckedHandle (), tag) != 0;
		}


		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public override string? ToString ()
		{
			return FullName;
		}

		/// <summary>Type identifier for the CoreText.CTFont type.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>
		///           <para>The returned token is the CoreFoundation type identifier (CFType) that has been assigned to this class.</para>
		///           <para>This can be used to determine type identity between different CoreFoundation objects.</para>
		///           <para>You can retrieve the type of a CoreFoundation object by invoking the <see cref="CoreFoundation.CFType.GetTypeID(System.IntPtr)" /> on the native handle of the object</para>
		///           <example>
		///             <code lang="csharp lang-csharp"><![CDATA[bool isCTFont = (CFType.GetTypeID (foo.Handle) == CTFont.GetTypeID ());]]></code>
		///           </example>
		///         </remarks>
		[DllImport (Constants.CoreTextLibrary, EntryPoint = "CTFontGetTypeID")]
		public extern static nint GetTypeID ();
	}
}
