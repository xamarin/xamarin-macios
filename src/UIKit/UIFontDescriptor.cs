//
// UIFontDescriptor.cs: Supporting classes for UIFontDescriptor
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2013-2014 Xamarin Inc
//
using System;
using ObjCRuntime;
using CoreGraphics;
using Foundation;

namespace UIKit {

	public class UIFontAttributes : DictionaryContainer {
		public UIFontAttributes () { }

#if !COREBUILD
		public UIFontAttributes (NSDictionary dictionary) : base (dictionary) { }

#if !WATCH
		public UIFontAttributes (params UIFontFeature [] features)
		{
			FeatureSettings = features;
		}
#endif

		public string Family {
			get {
				return GetStringValue (UIFontDescriptor.FamilyAttribute);
			}
			set {
				SetStringValue (UIFontDescriptor.FamilyAttribute, value);
			}
		}

		public string Name {
			get {
				return GetStringValue (UIFontDescriptor.NameAttribute);
			}
			set {
				SetStringValue (UIFontDescriptor.NameAttribute, value);
			}
		}

		public string Face {
			get {
				return GetStringValue (UIFontDescriptor.FaceAttribute);
			}
			set {
				SetStringValue (UIFontDescriptor.FaceAttribute, value);
			}
		}

		// Size is encoded as a string, but should contain a float
		public float? Size {
			get {
				return GetFloatValue (UIFontDescriptor.SizeAttribute);
			}
			set {
				SetNumberValue (UIFontDescriptor.SizeAttribute, value);
			}
		}

		public string VisibleName {
			get {
				return GetStringValue (UIFontDescriptor.VisibleNameAttribute);
			}
			set {
				SetStringValue (UIFontDescriptor.VisibleNameAttribute, value);
			}
		}

		public NSString TextStyle {
			get {
				return Dictionary [UIFontDescriptor.TextStyleAttribute] as NSString;
			}

			set {
				Dictionary [UIFontDescriptor.TextStyleAttribute] = value;
			}
		}

		public CGAffineTransform? Matrix {
			get {
				NSObject value;
				if (!Dictionary.TryGetValue (UIFontDescriptor.MatrixAttribute, out value))
					return null;

				return ((NSValue) value).CGAffineTransformValue;
			}
			set {
				if (!value.HasValue) {
					RemoveValue (UIFontDescriptor.MatrixAttribute);
					return;
				}
				Dictionary [UIFontDescriptor.MatrixAttribute] = NSValue.FromCGAffineTransform (value.Value);
			}
		}

		public NSCharacterSet CharacterSet {
			get {
				return Dictionary [UIFontDescriptor.CharacterSetAttribute] as NSCharacterSet;
			}
			set {
				if (value is null) {
					RemoveValue (UIFontDescriptor.CharacterSetAttribute);
					return;
				}
				Dictionary [UIFontDescriptor.CharacterSetAttribute] = value;
			}
		}

		public UIFontDescriptor [] CascadeList {
			get {
				return GetArray<UIFontDescriptor> (UIFontDescriptor.CascadeListAttribute);

			}
			set {
				if (value is null) {
					RemoveValue (UIFontDescriptor.CascadeListAttribute);
					return;
				}
				Dictionary [UIFontDescriptor.CascadeListAttribute] = NSArray.FromNSObjects (value);
			}
		}

		public UIFontTraits Traits {
			get {
				var traits = GetNSDictionary (UIFontDescriptor.TraitsAttribute);
				if (traits is null)
					return null;
				return new UIFontTraits (traits);
			}
			set {
				if (value is null) {
					RemoveValue (UIFontDescriptor.TraitsAttribute);
					return;
				}

				Dictionary [UIFontDescriptor.TraitsAttribute] = value.Dictionary;
			}
		}

		public float? FixedAdvance {
			get {
				return GetFloatValue (UIFontDescriptor.FixedAdvanceAttribute);
			}
			set {
				SetNumberValue (UIFontDescriptor.FixedAdvanceAttribute, value);
			}
		}

		public NSDictionary [] WeakFeatureSettings {
			get {
				return GetArray<NSDictionary> (UIFontDescriptor.FeatureSettingsAttribute);
			}
			set {
				if (value is null) {
					RemoveValue (UIFontDescriptor.FeatureSettingsAttribute);
					return;
				}
				Dictionary [UIFontDescriptor.FeatureSettingsAttribute] = NSArray.FromNSObjects (value);
			}
		}

#if !WATCH
		public UIFontFeature [] FeatureSettings {
			get {
				var dictArray = WeakFeatureSettings;
				if (dictArray is null)
					return new UIFontFeature [0];

				var strong = new UIFontFeature [dictArray.Length];
				for (int i = 0; i < dictArray.Length; i++)
					strong [i] = new UIFontFeature (dictArray [i]);
				return strong;
			}
			set {
				Dictionary [UIFontDescriptor.FeatureSettingsAttribute] = NSArray.FromNativeObjects (value);
			}
		}
#endif
#endif
	}

#if !COREBUILD
	public partial class UIFontDescriptor {
		public static UIFontDescriptor PreferredHeadline {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Headline);
			}
		}

		public static UIFontDescriptor PreferredBody {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Body);
			}
		}

		public static UIFontDescriptor PreferredSubheadline {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Subheadline);
			}
		}

		public static UIFontDescriptor PreferredFootnote {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Footnote);
			}
		}

		public static UIFontDescriptor PreferredCaption1 {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Caption1);
			}
		}

		public static UIFontDescriptor PreferredCaption2 {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Caption2);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static UIFontDescriptor PreferredTitle1 {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Title1);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static UIFontDescriptor PreferredTitle2 {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Title2);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static UIFontDescriptor PreferredTitle3 {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Title3);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static UIFontDescriptor PreferredCallout {
			get {
				return GetPreferredDescriptorForTextStyle (UIFontTextStyle.Callout);
			}
		}

		public UIFontDescriptor [] GetMatchingFontDescriptors (params UIFontDescriptorAttribute [] mandatoryKeys)
		{
			var n = mandatoryKeys.Length;
			if (n == 0)
				return GetMatchingFontDescriptors ((NSSet) null);
			var all = new NSString [n];
			for (int i = 0; i < n; i++) {
				switch (mandatoryKeys [i]) {
				case UIFontDescriptorAttribute.Family:
					all [i] = FamilyAttribute;
					break;
				case UIFontDescriptorAttribute.Face:
					all [i] = FaceAttribute;
					break;
				case UIFontDescriptorAttribute.Name:
					all [i] = NameAttribute;
					break;
				case UIFontDescriptorAttribute.Size:
					all [i] = SizeAttribute;
					break;
				case UIFontDescriptorAttribute.VisibleName:
					all [i] = VisibleNameAttribute;
					break;
				case UIFontDescriptorAttribute.Matrix:
					all [i] = MatrixAttribute;
					break;
				case UIFontDescriptorAttribute.CharacterSet:
					all [i] = CharacterSetAttribute;
					break;
				case UIFontDescriptorAttribute.CascadeList:
					all [i] = CascadeListAttribute;
					break;
				case UIFontDescriptorAttribute.Traits:
					all [i] = TraitsAttribute;
					break;
				case UIFontDescriptorAttribute.FixedAdvance:
					all [i] = FixedAdvanceAttribute;
					break;
				case UIFontDescriptorAttribute.FeatureSettings:
					all [i] = FeatureSettingsAttribute;
					break;
				case UIFontDescriptorAttribute.TextStyle:
					all [i] = TextStyleAttribute;
					break;
				}
			}
			return GetMatchingFontDescriptors (new NSSet (all));
		}

		NSString GetStringValue (NSString key)
		{
			return (NSString) GetObject (key);
		}

		float? GetFloatValue (NSString key)
		{
			var n = (NSNumber) GetObject (key);
			if (n is not null)
				return n.FloatValue;
			return null;
		}

		public string Family {
			get {
				return GetStringValue (UIFontDescriptor.FamilyAttribute);
			}
		}

		public string Name {
			get {
				return GetStringValue (UIFontDescriptor.NameAttribute);
			}
		}

		public string Face {
			get {
				return GetStringValue (UIFontDescriptor.FaceAttribute);
			}
		}

		public float? Size {
			get {
				return GetFloatValue (UIFontDescriptor.SizeAttribute);
			}
		}

		public string VisibleName {
			get {
				return GetStringValue (UIFontDescriptor.VisibleNameAttribute);
			}
		}

		public NSString TextStyle {
			get {
				return GetStringValue (UIFontDescriptor.TextStyleAttribute);
			}
		}

		public NSCharacterSet CharacterSet {
			get {
				return GetObject (UIFontDescriptor.CharacterSetAttribute) as NSCharacterSet;
			}
		}

		public UIFontDescriptor [] CascadeList {
			get {
				var o = GetObject (UIFontDescriptor.CascadeListAttribute) as NSArray;
				if (o is null)
					return new UIFontDescriptor [0];
				return NSArray.FromArray<UIFontDescriptor> (o);
			}
		}

		public UIFontTraits Traits {
			get {
				var traits = GetObject (UIFontDescriptor.TraitsAttribute) as NSDictionary;
				if (traits is null)
					return null;
				return new UIFontTraits (traits);
			}
		}

		public float? FixedAdvance {
			get {
				return GetFloatValue (UIFontDescriptor.FixedAdvanceAttribute);
			}
		}

		public NSDictionary [] WeakFeatureSettings {
			get {
				var wf = GetObject (UIFontDescriptor.FeatureSettingsAttribute) as NSArray;
				if (wf is null)
					return null;
				return NSArray.FromArray<NSDictionary> (wf);
			}
		}

#if !WATCH
		public UIFontFeature [] FeatureSettings {
			get {
				var dictArray = WeakFeatureSettings;
				if (dictArray is null)
					return new UIFontFeature [0];

				var strong = new UIFontFeature [dictArray.Length];
				for (int i = 0; i < dictArray.Length; i++)
					strong [i] = new UIFontFeature (dictArray [i]);
				return strong;
			}
		}
#endif
	}

	// that's a convenience enum that maps to UIFontDescriptorXXX which are internal (hidden) NSString
	public enum UIFontDescriptorAttribute {
		Family, Face, Name, Size, VisibleName, Matrix, CharacterSet, CascadeList, Traits, FixedAdvance, FeatureSettings, TextStyle
	}

	public class UIFontTraits : DictionaryContainer {
		public UIFontTraits () { }
		public UIFontTraits (NSDictionary dictionary) : base (dictionary) { }

		public UIFontDescriptorSymbolicTraits? SymbolicTrait {
			get {
				return (UIFontDescriptorSymbolicTraits?) GetInt32Value (UIFontDescriptor.SymbolicTrait);
			}
			set {
				SetNumberValue (UIFontDescriptor.SymbolicTrait, (int) value);
			}
		}

		public float? Weight {
			get {
				return GetInt32Value (UIFontDescriptor.WeightTrait);
			}
			set {
				SetNumberValue (UIFontDescriptor.WeightTrait, value);
			}
		}

		public float? Width {
			get {
				return GetInt32Value (UIFontDescriptor.WidthTrait);
			}
			set {
				SetNumberValue (UIFontDescriptor.WidthTrait, value);
			}
		}

		public float? Slant {
			get {
				return GetInt32Value (UIFontDescriptor.SlantTrait);
			}
			set {
				SetNumberValue (UIFontDescriptor.SlantTrait, value);
			}
		}
	}
#endif

}
