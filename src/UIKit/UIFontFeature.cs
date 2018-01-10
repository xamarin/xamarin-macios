//
// UIFontFeatuers.cs: 
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2013, Xamarin Inc
//
#if !WATCH
using System;
using System.Runtime.InteropServices;
using System.Collections;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using CoreText;

namespace UIKit {
	public class UIFontFeature : INativeObject {
		static NSObject [] keys = new NSObject [] { UIFontDescriptor.UIFontFeatureTypeIdentifierKey, UIFontDescriptor.UIFontFeatureSelectorIdentifierKey };
		
		NSDictionary dictionary;

		// To easily implement ToString
		FontFeatureGroup fontFeature;
		object fontFeatureValue;  

		IntPtr INativeObject.Handle {
			get {
				return dictionary.Handle;
			}
		}
			
		// internal ctor
		UIFontFeature (FontFeatureGroup fontFeature, int fontFeatureSelector, object value)
		{
			this.dictionary = NSDictionary.FromObjectsAndKeys (new NSObject [] { new NSNumber ((int)fontFeature), new NSNumber (fontFeatureSelector) }, keys);
			this.fontFeature = fontFeature;
			this.fontFeatureValue = value;
		}

		internal UIFontFeature (NSDictionary dict)
		{
			dictionary = dict;
			NSNumber v = dict [UIFontDescriptor.UIFontFeatureTypeIdentifierKey] as NSNumber;
			fontFeature = (FontFeatureGroup) (v == null ? -1 : v.Int32Value);
			
			v = dict [UIFontDescriptor.UIFontFeatureSelectorIdentifierKey] as NSNumber;
			int n = v == null ? 0 : v.Int32Value;
			
			switch (fontFeature){
			case FontFeatureGroup.AllTypographicFeatures:
				fontFeatureValue = (CTFontFeatureAllTypographicFeatures.Selector) n;
				break;
			case FontFeatureGroup.Ligatures:
				fontFeatureValue = (CTFontFeatureLigatures.Selector) n;
				break;
			case FontFeatureGroup.CursiveConnection:
				fontFeatureValue = (CTFontFeatureCursiveConnection.Selector) n;
				break;
			case FontFeatureGroup.LetterCase:
				fontFeatureValue = (CTFontFeatureLetterCase.Selector) n;
				break;
			case FontFeatureGroup.VerticalSubstitution:
				fontFeatureValue = (CTFontFeatureVerticalSubstitutionConnection.Selector) n;
				break;
			case FontFeatureGroup.LinguisticRearrangement:
				fontFeatureValue = (CTFontFeatureLinguisticRearrangementConnection.Selector) n;
				break;
			case FontFeatureGroup.NumberSpacing:
				fontFeatureValue = (CTFontFeatureNumberSpacing.Selector) n;
				break;
			case FontFeatureGroup.SmartSwash:
				fontFeatureValue = (CTFontFeatureSmartSwash.Selector) n;
				break;
			case FontFeatureGroup.Diacritics:
				fontFeatureValue = (CTFontFeatureDiacritics.Selector) n;
				break;
			case FontFeatureGroup.VerticalPosition:
				fontFeatureValue = (CTFontFeatureVerticalPosition.Selector) n;
				break;
			case FontFeatureGroup.Fractions:
				fontFeatureValue = (CTFontFeatureFractions.Selector) n;
				break;
			case FontFeatureGroup.OverlappingCharacters:
				fontFeatureValue = (CTFontFeatureOverlappingCharacters.Selector) n;
				break;
			case FontFeatureGroup.TypographicExtras:
				fontFeatureValue = (CTFontFeatureTypographicExtras.Selector) n;
				break;
			case FontFeatureGroup.MathematicalExtras:
				fontFeatureValue = (CTFontFeatureMathematicalExtras.Selector) n;
				break;
			case FontFeatureGroup.OrnamentSets:
				fontFeatureValue = (CTFontFeatureOrnamentSets.Selector) n;
				break;
			case FontFeatureGroup.CharacterAlternatives:
				fontFeatureValue = (CTFontFeatureCharacterAlternatives.Selector) n;
				break;
			case FontFeatureGroup.DesignComplexity:
				fontFeatureValue = (CTFontFeatureDesignComplexity.Selector) n;
				break;
			case FontFeatureGroup.StyleOptions:
				fontFeatureValue = (CTFontFeatureStyleOptions.Selector) n;
				break;
			case FontFeatureGroup.CharacterShape:
				fontFeatureValue = (CTFontFeatureCharacterShape.Selector) n;
				break;
			case FontFeatureGroup.NumberCase:
				fontFeatureValue = (CTFontFeatureNumberCase.Selector) n;
				break;
			case FontFeatureGroup.TextSpacing:
				fontFeatureValue = (CTFontFeatureTextSpacing.Selector) n;
				break;
			case FontFeatureGroup.Transliteration:
				fontFeatureValue = (CTFontFeatureTransliteration.Selector) n;
				break;
			case FontFeatureGroup.Annotation:
				fontFeatureValue = (CTFontFeatureAnnotation.Selector) n;
				break;
			case FontFeatureGroup.KanaSpacing:
				fontFeatureValue = (CTFontFeatureKanaSpacing.Selector) n;
				break;
			case FontFeatureGroup.IdeographicSpacing:
				fontFeatureValue = (CTFontFeatureIdeographicSpacing.Selector) n;
				break;
			case FontFeatureGroup.UnicodeDecomposition:
				fontFeatureValue = (CTFontFeatureUnicodeDecomposition.Selector) n;
				break;
			case FontFeatureGroup.RubyKana:
				fontFeatureValue = (CTFontFeatureRubyKana.Selector) n;
				break;
			case FontFeatureGroup.CJKSymbolAlternatives:
				fontFeatureValue = (CTFontFeatureCJKSymbolAlternatives.Selector) n;
				break;
			case FontFeatureGroup.IdeographicAlternatives:
				fontFeatureValue = (CTFontFeatureIdeographicAlternatives.Selector) n;
				break;
			case FontFeatureGroup.CJKVerticalRomanPlacement:
				fontFeatureValue = (CTFontFeatureCJKVerticalRomanPlacement.Selector) n;
				break;
			case FontFeatureGroup.ItalicCJKRoman:
				fontFeatureValue = (CTFontFeatureItalicCJKRoman.Selector) n;
				break;
			case FontFeatureGroup.CaseSensitiveLayout:
				fontFeatureValue = (CTFontFeatureCaseSensitiveLayout.Selector) n;
				break;
			case FontFeatureGroup.AlternateKana:
				fontFeatureValue = (CTFontFeatureAlternateKana.Selector) n;
				break;
			case FontFeatureGroup.StylisticAlternatives:
				fontFeatureValue = (CTFontFeatureStylisticAlternatives.Selector) n;
				break;
			case FontFeatureGroup.ContextualAlternates:
				fontFeatureValue = (CTFontFeatureContextualAlternates.Selector) n;
				break;
			case FontFeatureGroup.LowerCase:
				fontFeatureValue = (CTFontFeatureLowerCase.Selector) n;
				break;
			case FontFeatureGroup.UpperCase:
				fontFeatureValue = (CTFontFeatureUpperCase.Selector) n;
				break;
			case FontFeatureGroup.CJKRomanSpacing:
				fontFeatureValue = (CTFontFeatureCJKRomanSpacing.Selector) n;
				break;
			}
			if (v == null)
				fontFeatureValue = "InvalidKeyFound";
		}
		
		public FontFeatureGroup FontFeature {
			get {
				return fontFeature;
			}
		}
		
		public object FontFeatureValue {
			get {
				return fontFeatureValue;
			}
		}
		
		public override string ToString ()
		{
			return String.Format ("{0}={1}", FontFeature == ((FontFeatureGroup)(-1)) ? "Invalid" : FontFeature.ToString (), FontFeatureValue);
		}

		public UIFontFeature (CTFontFeatureAllTypographicFeatures.Selector featureSelector) : this (FontFeatureGroup.AllTypographicFeatures, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureLigatures.Selector featureSelector) : this (FontFeatureGroup.Ligatures, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureCursiveConnection.Selector featureSelector) : this (FontFeatureGroup.CursiveConnection, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureLetterCase.Selector featureSelector) : this (FontFeatureGroup.LetterCase, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureVerticalSubstitutionConnection.Selector featureSelector) : this (FontFeatureGroup.VerticalSubstitution, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureLinguisticRearrangementConnection.Selector featureSelector) : this (FontFeatureGroup.LinguisticRearrangement, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureNumberSpacing.Selector featureSelector) : this (FontFeatureGroup.NumberSpacing, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureSmartSwash.Selector featureSelector) : this (FontFeatureGroup.SmartSwash, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureDiacritics.Selector featureSelector) : this (FontFeatureGroup.Diacritics, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureVerticalPosition.Selector featureSelector) : this (FontFeatureGroup.VerticalPosition, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureFractions.Selector featureSelector) : this (FontFeatureGroup.Fractions, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureOverlappingCharacters.Selector featureSelector) : this (FontFeatureGroup.OverlappingCharacters, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureTypographicExtras.Selector featureSelector) : this (FontFeatureGroup.TypographicExtras, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureMathematicalExtras.Selector featureSelector) : this (FontFeatureGroup.MathematicalExtras, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureOrnamentSets.Selector featureSelector) : this (FontFeatureGroup.OrnamentSets, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureCharacterAlternatives.Selector featureSelector) : this (FontFeatureGroup.CharacterAlternatives, (int) featureSelector, featureSelector) {}
		public UIFontFeature (int characterAlternatives) : this (FontFeatureGroup.CharacterAlternatives, (int) characterAlternatives, characterAlternatives) {}
		public UIFontFeature (CTFontFeatureDesignComplexity.Selector featureSelector) : this (FontFeatureGroup.DesignComplexity, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureStyleOptions.Selector featureSelector) : this (FontFeatureGroup.StyleOptions, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureCharacterShape.Selector featureSelector) : this (FontFeatureGroup.CharacterShape, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureNumberCase.Selector featureSelector) : this (FontFeatureGroup.NumberCase, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureTextSpacing.Selector featureSelector) : this (FontFeatureGroup.TextSpacing, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureTransliteration.Selector featureSelector) : this (FontFeatureGroup.Transliteration, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureAnnotation.Selector featureSelector) : this (FontFeatureGroup.Annotation, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureKanaSpacing.Selector featureSelector) : this (FontFeatureGroup.KanaSpacing, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureIdeographicSpacing.Selector featureSelector) : this (FontFeatureGroup.IdeographicSpacing, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureUnicodeDecomposition.Selector featureSelector) : this (FontFeatureGroup.UnicodeDecomposition, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureRubyKana.Selector featureSelector) : this (FontFeatureGroup.RubyKana, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureCJKSymbolAlternatives.Selector featureSelector) : this (FontFeatureGroup.CJKSymbolAlternatives, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureIdeographicAlternatives.Selector featureSelector) : this (FontFeatureGroup.IdeographicAlternatives, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureCJKVerticalRomanPlacement.Selector featureSelector) : this (FontFeatureGroup.CJKVerticalRomanPlacement, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureItalicCJKRoman.Selector featureSelector) : this (FontFeatureGroup.ItalicCJKRoman, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureCaseSensitiveLayout.Selector featureSelector) : this (FontFeatureGroup.CaseSensitiveLayout, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureAlternateKana.Selector featureSelector) : this (FontFeatureGroup.AlternateKana, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureStylisticAlternatives.Selector featureSelector) : this (FontFeatureGroup.StylisticAlternatives, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureContextualAlternates.Selector featureSelector) : this (FontFeatureGroup.ContextualAlternates, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureLowerCase.Selector featureSelector) : this (FontFeatureGroup.LowerCase, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureUpperCase.Selector featureSelector) : this (FontFeatureGroup.UpperCase, (int) featureSelector, featureSelector) {}
		public UIFontFeature (CTFontFeatureCJKRomanSpacing.Selector featureSelector) : this (FontFeatureGroup.CJKRomanSpacing, (int) featureSelector, featureSelector) {}
	}
}
#endif
