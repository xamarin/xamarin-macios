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

#nullable enable

using System.Collections.Generic;

using CoreFoundation;
using CoreGraphics;

#if !MONOMAC
using UIKit;
#endif

namespace CoreText {

	#region CFAttributedStringRef AttributeKey Prototypes
	// defined as int32_t - System/Library/Frameworks/CoreText.framework/Headers/CTStringAttributes.h
	/// <summary>Specifies the style of an underline ornament.</summary>
	///     <remarks>To be added.</remarks>
	public enum CTUnderlineStyle : int {
		/// <summary>To be added.</summary>
		None = 0x00,
		/// <summary>To be added.</summary>
		Single = 0x01,
		/// <summary>To be added.</summary>
		Thick = 0x02,
		/// <summary>To be added.</summary>
		Double = 0x09,
	}

	// defined as int32_t - System/Library/Frameworks/CoreText.framework/Headers/CTStringAttributes.h
	/// <summary>An enumeration whose values specify options for <see cref="CoreText.CTUnderlineStyle" />s.</summary>
	///     <remarks>To be added.</remarks>
	public enum CTUnderlineStyleModifiers : int {
		/// <summary>To be added.</summary>
		PatternSolid = 0x0000,
		/// <summary>To be added.</summary>
		PatternDot = 0x0100,
		/// <summary>To be added.</summary>
		PatternDash = 0x0200,
		/// <summary>To be added.</summary>
		PatternDashDot = 0x0300,
		/// <summary>To be added.</summary>
		PatternDashDotDot = 0x0400,
	}

	/// <summary>An enumeration whose values specify the importance of ligatures in a <see cref="CoreText.CTStringAttributes" />.</summary>
	///     <remarks>To be added.</remarks>
	///     <altmember cref="CoreText.CTStringAttributes.LigatureFormation" />
	public enum CTLigatureFormation {
		/// <summary>To be added.</summary>
		Essential = 0,
		/// <summary>To be added.</summary>
		Standard = 1,
		/// <summary>To be added.</summary>
		All = 2,
	}

	/// <summary>An enumeration whose values describe the style of super- and sub- -scripts.</summary>
	///     <remarks>To be added.</remarks>
	public enum CTSuperscriptStyle {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Superscript = 1,
		/// <summary>To be added.</summary>
		Subscript = -1,
	}
	#endregion

	/// <summary>Specifies the attributes of a <see cref="CoreText.CTRun" />.</summary>
	///     <remarks>To be added.</remarks>
	///     <altmember cref="CoreText.CTRun.GetAttributes" />
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/SimpleTextInput/">SimpleTextInput</related>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTStringAttributes {

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTStringAttributes ()
			: this (new NSMutableDictionary ())
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTStringAttributes (NSDictionary dictionary)
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
		public CTFont? Font {
			get {
				var h = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.Font);
				return h == IntPtr.Zero ? null : new CTFont (h, false);
			}
			set { Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.Font!, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool ForegroundColorFromContext {
			get {
				return CFDictionary.GetBooleanValue (Dictionary,
						CTStringAttributeKey.ForegroundColorFromContext);
			}
			set {
				Adapter.AssertWritable (Dictionary);
				CFMutableDictionary.SetValue (Dictionary,
						CTStringAttributeKey.ForegroundColorFromContext, value);
			}
		}

		// Header says 'Value must be a CFNumberRef float' - System/Library/Frameworks/CoreText.framework/Headers/CTStringAttributes.h
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public float? KerningAdjustment {
			get { return Adapter.GetSingleValue (Dictionary, CTStringAttributeKey.KerningAdjustment); }
			set { Adapter.SetValue (Dictionary, CTStringAttributeKey.KerningAdjustment!, value); }
		}

		// Documentation says this must be 'CFNumber', doesn't specify exact type (but implies it's an integer value)
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CTLigatureFormation? LigatureFormation {
			get {
				var value = Adapter.GetInt32Value (Dictionary, CTStringAttributeKey.LigatureFormation);
				return !value.HasValue ? null : (CTLigatureFormation?) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTStringAttributeKey.LigatureFormation!,
						value.HasValue ? (int?) value.Value : null);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CGColor? ForegroundColor {
			get {
				var h = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.ForegroundColor);
				return h == IntPtr.Zero ? null : new CGColor (h, false);
			}
			set { Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.ForegroundColor!, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		public CGColor? BackgroundColor {
			get {
				var x = CTStringAttributeKey.BackgroundColor;
				if (x is not null) {
					var h = Dictionary.LowlevelObjectForKey (x);
					return new CGColor (h, false);
				} else {
					return null;
				}
			}
			set {
				var x = CTStringAttributeKey.BackgroundColor;
				if (x is not null)
					Adapter.SetNativeValue (Dictionary, x, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CTParagraphStyle? ParagraphStyle {
			get {
				var h = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.ParagraphStyle);
				return h == IntPtr.Zero ? null : new CTParagraphStyle (h, false);
			}
			set { Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.ParagraphStyle!, value); }
		}

		// Documentation says this must be 'CFNumber', doesn't specify exact type (but implies it's a floating point value)
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public float? StrokeWidth {
			get { return Adapter.GetSingleValue (Dictionary, CTStringAttributeKey.StrokeWidth); }
			set { Adapter.SetValue (Dictionary, CTStringAttributeKey.StrokeWidth!, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CGColor? StrokeColor {
			get {
				var h = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.StrokeColor);
				return h == IntPtr.Zero ? null : new CGColor (h, false);
			}
			set { Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.StrokeColor!, value); }
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public float? TrackingAdjustment {
			get { return Adapter.GetSingleValue (Dictionary, CTStringAttributeKey.TrackingAttributeName); }
			set { Adapter.SetValue (Dictionary, CTStringAttributeKey.TrackingAttributeName, value); }
		}

		// Documentation says this must be 'CFNumber', doesn't specify exact type
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int? UnderlineStyleValue {
			get { return Adapter.GetInt32Value (Dictionary, CTStringAttributeKey.UnderlineStyle); }
			set { Adapter.SetValue (Dictionary, CTStringAttributeKey.UnderlineStyle!, value); }
		}

		const int UnderlineStyleMask = 0x000F;
		const int UnderlineStyleModifiersMask = 0x0700;

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CTSuperscriptStyle? Superscript {
			get {
				var value = Adapter.GetInt32Value (Dictionary, CTStringAttributeKey.Superscript);
				return !value.HasValue ? null : (CTSuperscriptStyle?) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTStringAttributeKey.Superscript!,
						value.HasValue ? (int?) value.Value : null);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CGColor? UnderlineColor {
			get {
				var h = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.UnderlineColor);
				return h == IntPtr.Zero ? null : new CGColor (h, false);
			}
			set { Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.UnderlineColor!, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool VerticalForms {
			get {
				return CFDictionary.GetBooleanValue (Dictionary,
						CTStringAttributeKey.VerticalForms);
			}
			set {
				Adapter.AssertWritable (Dictionary);
				CFMutableDictionary.SetValue (Dictionary,
						CTStringAttributeKey.VerticalForms, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public int? HorizontalInVerticalForms {
			get {
				var x = CTStringAttributeKey.HorizontalInVerticalForms;
				return x is not null ? Adapter.GetInt32Value (Dictionary, x) : null;
			}
			set {
				var x = CTStringAttributeKey.HorizontalInVerticalForms;
				if (x is not null)
					Adapter.SetValue (Dictionary, x, value);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		public float? BaselineOffset {
			get { return Adapter.GetSingleValue (Dictionary, CTStringAttributeKey.BaselineOffset); }
			set { Adapter.SetValue (Dictionary, CTStringAttributeKey.BaselineOffset!, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CTGlyphInfo? GlyphInfo {
			get {
				var h = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.GlyphInfo);
				return h == IntPtr.Zero ? null : new CTGlyphInfo (h, false);
			}
			set { Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.GlyphInfo!, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int? CharacterShape {
			get { return Adapter.GetInt32Value (Dictionary, CTStringAttributeKey.CharacterShape); }
			set { Adapter.SetValue (Dictionary, CTStringAttributeKey.CharacterShape!, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CTRunDelegate? RunDelegate {
			get {
				var h = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.RunDelegate);
				return h == IntPtr.Zero ? null : new CTRunDelegate (h, false);
			}
			set { Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.RunDelegate!, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CTBaselineClass? BaselineClass {
			get {
				var value = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.BaselineClass);
				return value == IntPtr.Zero ? (CTBaselineClass?) null : CTBaselineClassID.FromHandle (value);
			}
			set {
				var ns_value = value is null ? null : CTBaselineClassID.ToNSString (value.Value);
				Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.BaselineClass!, ns_value);
			}
		}

		/// <param name="baselineClass">To be added.</param>
		///         <param name="offset">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetBaselineInfo (CTBaselineClass baselineClass, double offset)
		{
			SetBaseline (baselineClass, offset, CTStringAttributeKey.BaselineInfo);
		}

		/// <param name="baselineClass">The kind of baseline to set.</param>
		///         <param name="offset">The offset to alter.</param>
		///         <summary>Applies a baseline change.</summary>
		///         <remarks>
		///         </remarks>
		public void SetBaselineReferenceInfo (CTBaselineClass baselineClass, double offset)
		{
			SetBaseline (baselineClass, offset, CTStringAttributeKey.BaselineReferenceInfo);
		}

		void SetBaseline (CTBaselineClass baselineClass, double offset, NSString? infoKey)
		{
			var ptr = Dictionary.LowlevelObjectForKey (infoKey);
			var dict = ptr == IntPtr.Zero ? new NSMutableDictionary () : new NSMutableDictionary (ptr);

			var key = CTBaselineClassID.ToNSString (baselineClass);
			Adapter.SetValue (dict, key!, new NSNumber (offset));

			if (ptr == IntPtr.Zero)
				Adapter.SetNativeValue (Dictionary, infoKey!, (INativeObject) dict);
		}

		// 'Value must be a CFArray of CFNumberRefs' - System/Library/Frameworks/CoreText.framework/Headers/CTStringAttributes.h
		/// <param name="writingDirections">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetWritingDirection (params CTWritingDirection [] writingDirections)
		{
			var ptrs = new NativeHandle [writingDirections.Length];
			var numbers = new NSNumber [writingDirections.Length];
			for (int i = 0; i < writingDirections.Length; ++i) {
				numbers [i] = new NSNumber ((int) writingDirections [i]);
				ptrs [i] = numbers [i].Handle;
			}

			var array = CFArray.Create (ptrs);
			CFMutableDictionary.SetValue (Dictionary, CTStringAttributeKey.WritingDirection, array);
			GC.KeepAlive (numbers); // make sure the numbers aren't freed until we're done with them
		}

		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		// The attribute value must be an object conforming to the CTAdaptiveImageProviding protocol.
		public ICTAdaptiveImageProviding? AdaptiveImageProvider {
			get {
				var h = Dictionary.LowlevelObjectForKey (CTStringAttributeKey.AdaptiveImageProvider);
				return Runtime.GetINativeObject<ICTAdaptiveImageProviding> (h, owns: false);
			}
			set {
				Adapter.SetNativeValue (Dictionary, CTStringAttributeKey.AdaptiveImageProvider!, value);
			}
		}
	}
}
