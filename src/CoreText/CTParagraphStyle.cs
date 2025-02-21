// 
// CTParagraphStyle.cs: Implements the managed CTParagraphStyle
//
// Authors: Mono Team
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
using Foundation;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

	#region Paragraph Style Values

	// defined as uint8_t - /System/Library/Frameworks/CoreText.framework/Headers/CTParagraphStyle.h
	public enum CTTextAlignment : byte {
		/// <summary>To be added.</summary>
		Left = 0,
		/// <summary>To be added.</summary>
		Right = 1,
		/// <summary>To be added.</summary>
		Center = 2,
		/// <summary>To be added.</summary>
		Justified = 3,
		/// <summary>To be added.</summary>
		Natural = 4,
	}

	// defined as uint8_t - /System/Library/Frameworks/CoreText.framework/Headers/CTParagraphStyle.h
	public enum CTLineBreakMode : byte {
		/// <summary>To be added.</summary>
		WordWrapping = 0,
		/// <summary>To be added.</summary>
		CharWrapping = 1,
		/// <summary>To be added.</summary>
		Clipping = 2,
		/// <summary>To be added.</summary>
		TruncatingHead = 3,
		/// <summary>To be added.</summary>
		TruncatingTail = 4,
		/// <summary>To be added.</summary>
		TruncatingMiddle = 5,
	}

	[Flags]
	// defined as int8_t - /System/Library/Frameworks/CoreText.framework/Headers/CTParagraphStyle.h
	public enum CTWritingDirection : sbyte {
		/// <summary>To be added.</summary>
		Natural = -1,
		/// <summary>To be added.</summary>
		LeftToRight = 0,
		/// <summary>To be added.</summary>
		RightToLeft = 1,

		// part of an unnamed enum inside CTStringAttributes.h
		/// <summary>To be added.</summary>
		Embedding = (0 << 1),
		/// <summary>To be added.</summary>
		Override = (1 << 1)
	}

	// defined as uint32_t - /System/Library/Frameworks/CoreText.framework/Headers/CTParagraphStyle.h
	internal enum CTParagraphStyleSpecifier : uint {
		Alignment = 0,
		FirstLineHeadIndent = 1,
		HeadIndent = 2,
		TailIndent = 3,
		TabStops = 4,
		DefaultTabInterval = 5,
		LineBreakMode = 6,
		LineHeightMultiple = 7,
		MaximumLineHeight = 8,
		MinimumLineHeight = 9,
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.8", "Use 'MaximumLineSpacing' instead.")]
		[ObsoletedOSPlatform ("ios6.0", "Use 'MaximumLineSpacing' instead.")]
		[ObsoletedOSPlatform ("tvos16.0", "Use 'MaximumLineSpacing' instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Use 'MaximumLineSpacing' instead.")]
#else
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'MaximumLineSpacing' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'MaximumLineSpacing' instead.")]
		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use 'MaximumLineSpacing' instead.")]
#endif
		LineSpacing = 10,
		ParagraphSpacing = 11,
		ParagraphSpacingBefore = 12,
		BaseWritingDirection = 13,
		MaximumLineSpacing = 14,
		MinimumLineSpacing = 15,
		LineSpacingAdjustment = 16,
		LineBoundsOptions = 17,

		Count = 18,
	}

	internal struct CTParagraphStyleSetting {
		public CTParagraphStyleSpecifier spec;
		public nuint /* size_t */ valueSize;
		public IntPtr value;
	}
	#endregion

	[StructLayout (LayoutKind.Explicit)]
	internal struct CTParagraphStyleSettingValue {
		[FieldOffset (0)] public byte int8;
		[FieldOffset (0)] public nfloat single;
		[FieldOffset (0)] public nuint native_uint;
		[FieldOffset (0)] public IntPtr pointer;
	}

	internal abstract class CTParagraphStyleSpecifierValue {

		protected CTParagraphStyleSpecifierValue (CTParagraphStyleSpecifier spec)
		{
			Spec = spec;
		}

		internal CTParagraphStyleSpecifier Spec { get; private set; }

		internal abstract int ValueSize { get; }
		internal abstract void WriteValue (CTParagraphStyleSettingValue [] values, int index);

		public virtual void Dispose (CTParagraphStyleSettingValue [] values, int index)
		{
		}
	}

	internal class CTParagraphStyleSpecifierByteValue : CTParagraphStyleSpecifierValue {
		byte value;

		public CTParagraphStyleSpecifierByteValue (CTParagraphStyleSpecifier spec, byte value)
			: base (spec)
		{
			this.value = value;
		}

		internal override int ValueSize {
			get { return sizeof (byte); }
		}

		internal override void WriteValue (CTParagraphStyleSettingValue [] values, int index)
		{
			values [index].int8 = value;
		}
	}

	internal class CTParagraphStyleSpecifierNativeIntValue : CTParagraphStyleSpecifierValue {
		nuint value;

		public CTParagraphStyleSpecifierNativeIntValue (CTParagraphStyleSpecifier spec, nuint value)
			: base (spec)
		{
			this.value = value;
		}

		internal override int ValueSize {
			get { return IntPtr.Size; }
		}

		internal override void WriteValue (CTParagraphStyleSettingValue [] values, int index)
		{
			values [index].native_uint = value;
		}
	}

	internal class CTParagraphStyleSpecifierSingleValue : CTParagraphStyleSpecifierValue {
		nfloat value;

		public CTParagraphStyleSpecifierSingleValue (CTParagraphStyleSpecifier spec, nfloat value)
			: base (spec)
		{
			this.value = value;
		}

		internal override int ValueSize {
			get { return IntPtr.Size; }
		}

		internal override void WriteValue (CTParagraphStyleSettingValue [] values, int index)
		{
			values [index].single = value;
		}
	}

	internal class CTParagraphStyleSpecifierIntPtrsValue : CTParagraphStyleSpecifierValue {
		CFArray value;

		public CTParagraphStyleSpecifierIntPtrsValue (CTParagraphStyleSpecifier spec, NativeHandle [] value)
			: base (spec)
		{
			this.value = CFArray.FromIntPtrs (value);
		}

		internal override int ValueSize {
			get { return IntPtr.Size; }
		}

		internal override void WriteValue (CTParagraphStyleSettingValue [] values, int index)
		{
			values [index].pointer = value.Handle;
		}

		public override void Dispose (CTParagraphStyleSettingValue [] values, int index)
		{
			values [index].pointer = IntPtr.Zero;
			value.Dispose ();
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTParagraphStyleSettings {

		public CTParagraphStyleSettings ()
		{
		}

		public IEnumerable<CTTextTab>? TabStops { get; set; }
		public CTTextAlignment? Alignment { get; set; }
		public CTLineBreakMode? LineBreakMode { get; set; }
		public CTWritingDirection? BaseWritingDirection { get; set; }
		public CTLineBoundsOptions? LineBoundsOptions { get; set; }
		public nfloat? FirstLineHeadIndent { get; set; }
		public nfloat? HeadIndent { get; set; }
		public nfloat? TailIndent { get; set; }
		public nfloat? DefaultTabInterval { get; set; }
		public nfloat? LineHeightMultiple { get; set; }
		public nfloat? MaximumLineHeight { get; set; }
		public nfloat? MinimumLineHeight { get; set; }
		public nfloat? LineSpacing { get; set; }
		public nfloat? ParagraphSpacing { get; set; }
		public nfloat? ParagraphSpacingBefore { get; set; }
		public nfloat? MaximumLineSpacing { get; set; }
		public nfloat? MinimumLineSpacing { get; set; }
		public nfloat? LineSpacingAdjustment { get; set; }

		internal List<CTParagraphStyleSpecifierValue> GetSpecifiers ()
		{
			var values = new List<CTParagraphStyleSpecifierValue> ();

			if (TabStops is not null)
				values.Add (CreateValue (CTParagraphStyleSpecifier.TabStops, TabStops));
			if (Alignment.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.Alignment, (byte) Alignment.Value));
			if (LineBreakMode.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.LineBreakMode, (byte) LineBreakMode.Value));
			if (BaseWritingDirection.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.BaseWritingDirection, (byte) BaseWritingDirection.Value));
			if (LineBoundsOptions.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.LineBoundsOptions, (nuint) (ulong) LineBoundsOptions.Value));
			if (FirstLineHeadIndent.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.FirstLineHeadIndent, FirstLineHeadIndent.Value));
			if (HeadIndent.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.HeadIndent, HeadIndent.Value));
			if (TailIndent.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.TailIndent, TailIndent.Value));
			if (DefaultTabInterval.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.DefaultTabInterval, DefaultTabInterval.Value));
			if (LineHeightMultiple.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.LineHeightMultiple, LineHeightMultiple.Value));
			if (MaximumLineHeight.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.MaximumLineHeight, MaximumLineHeight.Value));
			if (MinimumLineHeight.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.MinimumLineHeight, MinimumLineHeight.Value));
			if (LineSpacing.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.LineSpacing, LineSpacing.Value));
			if (ParagraphSpacing.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.ParagraphSpacing, ParagraphSpacing.Value));
			if (ParagraphSpacingBefore.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.ParagraphSpacingBefore, ParagraphSpacingBefore.Value));
			if (MaximumLineSpacing.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.MaximumLineSpacing, MaximumLineSpacing.Value));
			if (MinimumLineSpacing.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.MinimumLineSpacing, MinimumLineSpacing.Value));
			if (LineSpacingAdjustment.HasValue)
				values.Add (CreateValue (CTParagraphStyleSpecifier.LineSpacingAdjustment, LineSpacingAdjustment.Value));
			return values;
		}

		static CTParagraphStyleSpecifierValue CreateValue (CTParagraphStyleSpecifier spec, IEnumerable<CTTextTab> value)
		{
			var handles = new List<NativeHandle> ();
			foreach (var ts in value)
				handles.Add (ts.Handle);
			return new CTParagraphStyleSpecifierIntPtrsValue (spec, handles.ToArray ());
		}

		static CTParagraphStyleSpecifierValue CreateValue (CTParagraphStyleSpecifier spec, byte value)
		{
			return new CTParagraphStyleSpecifierByteValue (spec, value);
		}

		static CTParagraphStyleSpecifierValue CreateValue (CTParagraphStyleSpecifier spec, nfloat value)
		{
			return new CTParagraphStyleSpecifierSingleValue (spec, value);
		}

		static CTParagraphStyleSpecifierValue CreateValue (CTParagraphStyleSpecifier spec, nuint value)
		{
			return new CTParagraphStyleSpecifierNativeIntValue (spec, value);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTParagraphStyle : NativeObject {
		[Preserve (Conditional = true)]
		internal CTParagraphStyle (NativeHandle handle, bool owns)
			: base (handle, owns, true)
		{
		}

		#region Paragraph Style Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTParagraphStyleCreate (CTParagraphStyleSetting []? settings, nint settingCount);
		public CTParagraphStyle (CTParagraphStyleSettings? settings)
			: base (settings is null ? CTParagraphStyleCreate (null, 0) : CreateFromSettings (settings), true, true)
		{
		}

		static unsafe IntPtr CreateFromSettings (CTParagraphStyleSettings s)
		{
			var handle = IntPtr.Zero;

			var specifiers = s.GetSpecifiers ();

			var settings = new CTParagraphStyleSetting [specifiers.Count];
			var values = new CTParagraphStyleSettingValue [specifiers.Count];

			int i = 0;
			foreach (var e in specifiers) {
				e.WriteValue (values, i);
				settings [i].spec = e.Spec;
				settings [i].valueSize = (uint) e.ValueSize;
				++i;
			}

			fixed (CTParagraphStyleSettingValue* pv = values) {
				for (i = 0; i < settings.Length; ++i) {
					// TODO: is this safe on the ARM?
					byte* p = &pv [i].int8;
					settings [i].value = (IntPtr) p;
				}
				handle = CTParagraphStyleCreate (settings, settings.Length);
			}
			// Yes this weird Dispose implementation is correct, this bugzilla
			// comment explains more about it. TL;DR: check CTParagraphStyleSpecifierIntPtrsValue
			// https://bugzilla.xamarin.com/show_bug.cgi?id=54148#c4
			i = 0;
			foreach (var e in specifiers) {
				e.Dispose (values, i);
			}

			return handle;
		}

		public CTParagraphStyle ()
			: this (null)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTParagraphStyleCreateCopy (IntPtr paragraphStyle);
		public CTParagraphStyle Clone ()
		{
			return new CTParagraphStyle (CTParagraphStyleCreateCopy (Handle), true);
		}
		#endregion

		#region Paragraph Style Access
		[DllImport (Constants.CoreTextLibrary)]
		static extern unsafe byte CTParagraphStyleGetValueForSpecifier (IntPtr paragraphStyle, CTParagraphStyleSpecifier spec, nuint valueBufferSize, void* valueBuffer);

		public unsafe CTTextTab? []? GetTabStops ()
		{
			IntPtr cfArrayRef;
			if (CTParagraphStyleGetValueForSpecifier (Handle, CTParagraphStyleSpecifier.TabStops, (uint) IntPtr.Size, (void*) &cfArrayRef) == 0)
				throw new InvalidOperationException ("Unable to get property value.");
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTTextTab> ();
			return NSArray.ArrayFromHandle (cfArrayRef, p => new CTTextTab (p, false));
		}

		public CTTextAlignment Alignment {
			get { return (CTTextAlignment) GetByteValue (CTParagraphStyleSpecifier.Alignment); }
		}

		unsafe byte GetByteValue (CTParagraphStyleSpecifier spec)
		{
			byte value;
			if (CTParagraphStyleGetValueForSpecifier (Handle, spec, sizeof (byte), &value) == 0)
				throw new InvalidOperationException ("Unable to get property value.");
			return value;
		}

		public CTLineBreakMode LineBreakMode {
			get { return (CTLineBreakMode) GetByteValue (CTParagraphStyleSpecifier.LineBreakMode); }
		}

		public CTWritingDirection BaseWritingDirection {
			get { return (CTWritingDirection) GetByteValue (CTParagraphStyleSpecifier.BaseWritingDirection); }
		}

#if NET
		public nfloat FirstLineHeadIndent {
#else
		public float FirstLineHeadIndent {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.FirstLineHeadIndent); }
		}

#if NET
		unsafe nfloat GetFloatValue (CTParagraphStyleSpecifier spec)
#else
		unsafe float GetFloatValue (CTParagraphStyleSpecifier spec)
#endif
		{
			nfloat value;
			if (CTParagraphStyleGetValueForSpecifier (Handle, spec, (nuint) sizeof (nfloat), &value) == 0)
				throw new InvalidOperationException ("Unable to get property value.");
#if NET
			return value;
#else
			return (float) value;
#endif
		}

#if NET
		public nfloat HeadIndent {
#else
		public float HeadIndent {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.HeadIndent); }
		}

#if NET
		public nfloat TailIndent {
#else
		public float TailIndent {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.TailIndent); }
		}

#if NET
		public nfloat DefaultTabInterval {
#else
		public float DefaultTabInterval {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.DefaultTabInterval); }
		}

#if NET
		public nfloat LineHeightMultiple {
#else
		public float LineHeightMultiple {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.LineHeightMultiple); }
		}

#if NET
		public nfloat MaximumLineHeight {
#else
		public float MaximumLineHeight {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.MaximumLineHeight); }
		}

#if NET
		public nfloat MinimumLineHeight {
#else
		public float MinimumLineHeight {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.MinimumLineHeight); }
		}

#if NET
		public nfloat LineSpacing {
#else
		public float LineSpacing {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.LineSpacing); }
		}

#if NET
		public nfloat ParagraphSpacing {
#else
		public float ParagraphSpacing {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.ParagraphSpacing); }
		}

#if NET
		public nfloat ParagraphSpacingBefore {
#else
		public float ParagraphSpacingBefore {
#endif
			get { return GetFloatValue (CTParagraphStyleSpecifier.ParagraphSpacingBefore); }
		}
		#endregion
	}
}
