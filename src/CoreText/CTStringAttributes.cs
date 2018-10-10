// 
// CTStringAttributes.cs: Implements the managed CTStringAttributes
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2012 - 2014 Xamarin Inc.
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
using Foundation;
using CoreFoundation;
using CoreGraphics;

#if !MONOMAC
using UIKit;
#endif

namespace CoreText {

#region CFAttributedStringRef AttributeKey Prototypes
	// defined as int32_t - System/Library/Frameworks/CoreText.framework/Headers/CTStringAttributes.h
	public enum CTUnderlineStyle : int {
		None    = 0x00,
		Single  = 0x01,
		Thick   = 0x02,
		Double  = 0x09,
	}

	// defined as int32_t - System/Library/Frameworks/CoreText.framework/Headers/CTStringAttributes.h
	public enum CTUnderlineStyleModifiers : int {
		PatternSolid      = 0x0000,
		PatternDot        = 0x0100,
		PatternDash       = 0x0200,
		PatternDashDot    = 0x0300,
		PatternDashDotDot = 0x0400,
	}

	public enum CTLigatureFormation {
		Essential = 0,
		Standard  = 1,
		All       = 2,
	}

	public enum CTSuperscriptStyle {
		None = 0,
		Superscript = 1,
		Subscript   = -1,
	}

	public static class CTStringAttributeKey {
		public static readonly NSString Font;
		public static readonly NSString ForegroundColorFromContext;
		public static readonly NSString KerningAdjustment;
		public static readonly NSString LigatureFormation;
		public static readonly NSString ForegroundColor;
		public static readonly NSString BackgroundColor;
		public static readonly NSString ParagraphStyle;
		public static readonly NSString StrokeWidth;
		public static readonly NSString StrokeColor;
		public static readonly NSString UnderlineStyle;
		public static readonly NSString Superscript;
		public static readonly NSString UnderlineColor;
		public static readonly NSString VerticalForms;
		public static readonly NSString HorizontalInVerticalForms;
		public static readonly NSString GlyphInfo;
		public static readonly NSString CharacterShape;
		public static readonly NSString RunDelegate;
		// Since 6,0
		internal static readonly NSString BaselineClass;
		internal static readonly NSString BaselineInfo;
		internal static readonly NSString BaselineReferenceInfo;
		internal static readonly NSString BaselineOffset;
		internal static readonly NSString WritingDirection;

		static CTStringAttributeKey ()
		{
			var handle = Libraries.CoreText.Handle;
			Font                        = Dlfcn.GetStringConstant (handle, "kCTFontAttributeName");
			ForegroundColorFromContext  = Dlfcn.GetStringConstant (handle, "kCTForegroundColorFromContextAttributeName");
			KerningAdjustment           = Dlfcn.GetStringConstant (handle, "kCTKernAttributeName");
			LigatureFormation           = Dlfcn.GetStringConstant (handle, "kCTLigatureAttributeName");
			ForegroundColor             = Dlfcn.GetStringConstant (handle, "kCTForegroundColorAttributeName");
			BackgroundColor             = Dlfcn.GetStringConstant (handle, "kCTBackgroundColorAttributeName");
			ParagraphStyle              = Dlfcn.GetStringConstant (handle, "kCTParagraphStyleAttributeName");
			StrokeWidth                 = Dlfcn.GetStringConstant (handle, "kCTStrokeWidthAttributeName");
			StrokeColor                 = Dlfcn.GetStringConstant (handle, "kCTStrokeColorAttributeName");
			UnderlineStyle              = Dlfcn.GetStringConstant (handle, "kCTUnderlineStyleAttributeName");
			Superscript                 = Dlfcn.GetStringConstant (handle, "kCTSuperscriptAttributeName");
			UnderlineColor              = Dlfcn.GetStringConstant (handle, "kCTUnderlineColorAttributeName");
			VerticalForms               = Dlfcn.GetStringConstant (handle, "kCTVerticalFormsAttributeName");
			HorizontalInVerticalForms   = Dlfcn.GetStringConstant (handle, "kCTHorizontalInVerticalFormsAttributeName");
			GlyphInfo                   = Dlfcn.GetStringConstant (handle, "kCTGlyphInfoAttributeName");
			CharacterShape              = Dlfcn.GetStringConstant (handle, "kCTCharacterShapeAttributeName");
			RunDelegate                 = Dlfcn.GetStringConstant (handle, "kCTRunDelegateAttributeName");
			BaselineOffset              = Dlfcn.GetStringConstant (handle, "kCTBaselineOffsetAttributeName");

#if !MONOMAC
			BaselineClass               = Dlfcn.GetStringConstant (handle, "kCTBaselineClassAttributeName");
			BaselineInfo                = Dlfcn.GetStringConstant (handle, "kCTBaselineInfoAttributeName");
			BaselineReferenceInfo       = Dlfcn.GetStringConstant (handle, "kCTBaselineReferenceInfoAttributeName");
			WritingDirection            = Dlfcn.GetStringConstant (handle, "kCTWritingDirectionAttributeName");
#endif
		}
	}
#endregion

	public class CTStringAttributes {

		public CTStringAttributes ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTStringAttributes (NSDictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException ("dictionary");
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary {get; private set;}

		public CTFont Font {
			get {
				var h = CFDictionary.GetValue (Dictionary.Handle, CTStringAttributeKey.Font.Handle);
				return h == IntPtr.Zero ? null : new CTFont (h, false);
			}
			set {Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.Font, value);}
		}

		public bool ForegroundColorFromContext {
			get {
				return CFDictionary.GetBooleanValue (Dictionary.Handle, 
						CTStringAttributeKey.ForegroundColorFromContext.Handle);
			}
			set {
				Adapter.AssertWritable (Dictionary);
				CFMutableDictionary.SetValue (Dictionary.Handle,
						CTStringAttributeKey.ForegroundColorFromContext.Handle, value);
			}
		}

		// Header says 'Value must be a CFNumberRef float' - System/Library/Frameworks/CoreText.framework/Headers/CTStringAttributes.h
		public float? KerningAdjustment {
			get {return Adapter.GetSingleValue (Dictionary, CTStringAttributeKey.KerningAdjustment);}
			set {Adapter.SetValue (Dictionary, CTStringAttributeKey.KerningAdjustment, value);}
		}

		// Documentation says this must be 'CFNumber', doesn't specify exact type (but implies it's an integer value)
		public CTLigatureFormation? LigatureFormation {
			get {
				var value = Adapter.GetInt32Value (Dictionary, CTStringAttributeKey.LigatureFormation);
				return !value.HasValue ? null : (CTLigatureFormation?) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTStringAttributeKey.LigatureFormation,
						value.HasValue ? (int?) value.Value : null);
			}
		}

		public CGColor ForegroundColor {
			get {
				var h = CFDictionary.GetValue (Dictionary.Handle, CTStringAttributeKey.ForegroundColor.Handle);
				return h == IntPtr.Zero ? null : new CGColor (h);
			}
			set {Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.ForegroundColor, value);}
		}

		[iOS (10,0)][Mac (10,12)]
		public CGColor BackgroundColor {
			get {
				var h = IntPtr.Zero;
				var x = CTStringAttributeKey.BackgroundColor;
				if (x != null)
					h = CFDictionary.GetValue (Dictionary.Handle, x.Handle);
				return h == IntPtr.Zero ? null : new CGColor (h);
			}
			set {
				var x = CTStringAttributeKey.BackgroundColor;
				if (x != null)
					Adapter.SetNativeValue (Dictionary, x, value);
			}
		}

		public CTParagraphStyle ParagraphStyle {
			get {
				var h = CFDictionary.GetValue (Dictionary.Handle, CTStringAttributeKey.ParagraphStyle.Handle);
				return h == IntPtr.Zero ? null : new CTParagraphStyle (h, false);
			}
			set {Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.ParagraphStyle, value);}
		}

		// Documentation says this must be 'CFNumber', doesn't specify exact type (but implies it's a floating point value)
		public float? StrokeWidth {
			get {return Adapter.GetSingleValue (Dictionary, CTStringAttributeKey.StrokeWidth);}
			set {Adapter.SetValue (Dictionary, CTStringAttributeKey.StrokeWidth, value);}
		}

		public CGColor StrokeColor {
			get {
				var h = CFDictionary.GetValue (Dictionary.Handle, CTStringAttributeKey.StrokeColor.Handle);
				return h == IntPtr.Zero ? null : new CGColor (h);
			}
			set {Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.StrokeColor, value);}
		}

		// Documentation says this must be 'CFNumber', doesn't specify exact type
		public int? UnderlineStyleValue {
			get {return Adapter.GetInt32Value (Dictionary, CTStringAttributeKey.UnderlineStyle);}
			set {Adapter.SetValue (Dictionary, CTStringAttributeKey.UnderlineStyle, value);}
		}

		const int UnderlineStyleMask          = 0x000F;
		const int UnderlineStyleModifiersMask = 0x0700;

		public CTUnderlineStyle? UnderlineStyle {
			get {
				var v = UnderlineStyleValue;
				return !v.HasValue ? null : (CTUnderlineStyle?) (v.Value & UnderlineStyleMask);
			}
			set {
				var m = UnderlineStyleModifiers;
				UnderlineStyleValue = Adapter.BitwiseOr (
						m.HasValue ? (int?) m.Value : null,
						value.HasValue ? (int?) value.Value : null);
			}
		}

		public CTUnderlineStyleModifiers? UnderlineStyleModifiers {
			get {
				var v = UnderlineStyleValue;
				return !v.HasValue ? null : (CTUnderlineStyleModifiers?) (v.Value & UnderlineStyleModifiersMask);
			}
			set {
				var m = UnderlineStyleModifiers;
				UnderlineStyleValue = Adapter.BitwiseOr (
						m.HasValue ? (int?) m.Value : null,
						value.HasValue ? (int?) value.Value : null);
			}
		}

		public CTSuperscriptStyle? Superscript {
			get {
				var value = Adapter.GetInt32Value (Dictionary, CTStringAttributeKey.Superscript);
				return !value.HasValue ? null : (CTSuperscriptStyle?) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTStringAttributeKey.Superscript,
						value.HasValue ? (int?) value.Value : null);
			}
		}

		public CGColor UnderlineColor {
			get {
				var h = CFDictionary.GetValue (Dictionary.Handle, CTStringAttributeKey.UnderlineColor.Handle);
				return h == IntPtr.Zero ? null : new CGColor (h);
			}
			set {Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.UnderlineColor, value);}
		}

		public bool VerticalForms {
			get {
				return CFDictionary.GetBooleanValue (Dictionary.Handle, 
						CTStringAttributeKey.VerticalForms.Handle);
			}
			set {
				Adapter.AssertWritable (Dictionary);
				CFMutableDictionary.SetValue (Dictionary.Handle,
						CTStringAttributeKey.VerticalForms.Handle, value);
			}
		}

		[iOS (10,0)][Mac (10,12)][Watch (3,0)][TV (10,0)]
		public int? HorizontalInVerticalForms {
			get {
				var x = CTStringAttributeKey.HorizontalInVerticalForms;
				return x != null ? Adapter.GetInt32Value (Dictionary, x) : null;
			}
			set {
				var x = CTStringAttributeKey.HorizontalInVerticalForms;
				if (x != null)
					Adapter.SetValue (Dictionary, x, value);
			}
		}

		[iOS (11,0), Mac (10,13), TV (11,0), Watch (4,0)]
		public float? BaselineOffset {
			get { return Adapter.GetSingleValue (Dictionary, CTStringAttributeKey.BaselineOffset); }
			set { Adapter.SetValue (Dictionary, CTStringAttributeKey.BaselineOffset, value); }
		}

		public CTGlyphInfo GlyphInfo {
			get {
				var h = CFDictionary.GetValue (Dictionary.Handle, CTStringAttributeKey.GlyphInfo.Handle);
				return h == IntPtr.Zero ? null : new CTGlyphInfo (h, false);
			}
			set {Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.GlyphInfo, value);}
		}

		public int? CharacterShape {
			get {return Adapter.GetInt32Value (Dictionary, CTStringAttributeKey.CharacterShape);}
			set {Adapter.SetValue (Dictionary, CTStringAttributeKey.CharacterShape, value);}
		}

		public CTRunDelegate RunDelegate {
			get {
				var h = CFDictionary.GetValue (Dictionary.Handle, CTStringAttributeKey.RunDelegate.Handle);
				return h == IntPtr.Zero ? null : new CTRunDelegate (h, false);
			}
			set {Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.RunDelegate, value);}
		}

		[iOS (6,0)]
		public CTBaselineClass? BaselineClass {
			get {
				var value = CFDictionary.GetValue (Dictionary.Handle, CTStringAttributeKey.BaselineClass.Handle);
				return value == IntPtr.Zero ? (CTBaselineClass?) null : CTBaselineClassID.FromHandle (value);
			}
			set {
				var ns_value = value == null ? null : CTBaselineClassID.ToNSString (value.Value);
				Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.BaselineClass, ns_value);
			}
		}

		[iOS (6, 0)]
		public void SetBaselineInfo (CTBaselineClass baselineClass, double offset)
		{
			SetBaseline (baselineClass, offset, CTStringAttributeKey.BaselineInfo);
		}

		[iOS (6, 0)]
		public void SetBaselineReferenceInfo (CTBaselineClass baselineClass, double offset)
		{
			SetBaseline (baselineClass, offset, CTStringAttributeKey.BaselineReferenceInfo);
		}

		void SetBaseline (CTBaselineClass baselineClass, double offset, NSString infoKey)
		{
			var ptr = CFDictionary.GetValue (Dictionary.Handle, infoKey.Handle);
			var dict = ptr == IntPtr.Zero ? new NSMutableDictionary () : new NSMutableDictionary (ptr);

			var key = CTBaselineClassID.ToNSString (baselineClass);
			Adapter.SetValue (dict, key, new NSNumber (offset));

			if (ptr == IntPtr.Zero)
				Adapter.SetNativeValue (Dictionary, infoKey, (INativeObject)dict);
		}

		[iOS (6, 0)]
		// 'Value must be a CFArray of CFNumberRefs' - System/Library/Frameworks/CoreText.framework/Headers/CTStringAttributes.h
		public void SetWritingDirection (params CTWritingDirection[] writingDirections)
		{
			var ptrs = new IntPtr [writingDirections.Length];
			for (int i = 0; i < writingDirections.Length; ++i) {
				ptrs[i] = (new NSNumber ((int) writingDirections[i])).Handle;
			}

			var array = CFArray.Create (ptrs);
			CFMutableDictionary.SetValue (Dictionary.Handle, CTStringAttributeKey.WritingDirection.Handle, array);
		}
	}
}

