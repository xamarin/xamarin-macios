//
// UITypes.cs: Various types defined in UIKit
//
// Authors:
//   Miguel de Icaza
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2016 Xamarin Inc.
//
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreGraphics;

namespace UIKit {

	[StructLayout (LayoutKind.Sequential)]
	public struct UIEdgeInsets {

		// API match for UIEdgeInsetsZero field/constant
		[Field ("UIEdgeInsetsZero")] // fake (but helps testing and could also help documentation)
		public static readonly UIEdgeInsets Zero;

		public nfloat Top, Left, Bottom, Right;

#if !COREBUILD
		public UIEdgeInsets (nfloat top, nfloat left, nfloat bottom, nfloat right)
		{
			Top = top;
			Left = left;
			Bottom = bottom;
			Right = right;
		}

		// note: UIEdgeInsetsInsetRect (UIGeometry.h) is a macro
		public CGRect InsetRect (CGRect rect)
		{
			return new CGRect (rect.X + Left,
			                       rect.Y + Top,
			                       rect.Width - Left - Right,
			                       rect.Height - Top - Bottom);
		}

		// note: UIEdgeInsetsEqualToEdgeInsets (UIGeometry.h) is a macro
		public bool Equals (UIEdgeInsets other)
		{
			if (Left != other.Left)
				return false;
			if (Right != other.Right)
				return false;
			if (Top != other.Top)
				return false;
			return (Bottom == other.Bottom);
		}

		public override bool Equals (object obj)
		{
			if (obj is UIEdgeInsets)
				return Equals ((UIEdgeInsets) obj);
			return false;
		}

		public static bool operator == (UIEdgeInsets insets1, UIEdgeInsets insets2)
		{
			return insets1.Equals (insets2);
		}

		public static bool operator != (UIEdgeInsets insets1, UIEdgeInsets insets2)
		{
			return !insets1.Equals (insets2);
		}

		public override int GetHashCode ()
		{
			return Top.GetHashCode () ^ Left.GetHashCode () ^ Right.GetHashCode () ^ Bottom.GetHashCode ();
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static UIEdgeInsets UIEdgeInsetsFromString (IntPtr /* NSString */ s);

		static public UIEdgeInsets FromString (string s)
		{
			// note: null is allowed
			var ptr = NSString.CreateNative (s);
			var value = UIEdgeInsetsFromString (ptr);
			NSString.ReleaseNative (ptr);
			return value;
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static IntPtr /* NSString */ NSStringFromUIEdgeInsets (UIEdgeInsets insets);

		// note: ensure we can roundtrip ToString into FromString
		public override string ToString ()
		{
			using (var ns = new NSString (NSStringFromUIEdgeInsets (this)))
				return ns.ToString ();
		}
#endif
	}

	[Watch (4,0), TV (11,0), iOS (11,0)]
	[StructLayout (LayoutKind.Sequential)]
	public struct NSDirectionalEdgeInsets {

		// API match for NSDirectionalEdgeInsetsZero field/constant
		[Field ("NSDirectionalEdgeInsetsZero")] // fake (but helps testing and could also help documentation)
		public static readonly NSDirectionalEdgeInsets Zero;

		public nfloat Top, Leading, Bottom, Trailing;

#if !COREBUILD
		public NSDirectionalEdgeInsets (nfloat top, nfloat leading, nfloat bottom, nfloat trailing)
		{
			Top = top;
			Leading = leading;
			Bottom = bottom;
			Trailing = trailing;
		}

		// note: NSDirectionalEdgeInsetsEqualToDirectionalEdgeInsets (UIGeometry.h) is a macro
		public bool Equals (NSDirectionalEdgeInsets other)
		{
			if (Leading != other.Leading)
				return false;
			if (Trailing != other.Trailing)
				return false;
			if (Top != other.Top)
				return false;
			return (Bottom == other.Bottom);
		}

		public override bool Equals (object obj)
		{
			if (obj is NSDirectionalEdgeInsets)
				return Equals ((NSDirectionalEdgeInsets) obj);
			return false;
		}

		public static bool operator == (NSDirectionalEdgeInsets insets1, NSDirectionalEdgeInsets insets2)
		{
			return insets1.Equals (insets2);
		}

		public static bool operator != (NSDirectionalEdgeInsets insets1, NSDirectionalEdgeInsets insets2)
		{
			return !insets1.Equals (insets2);
		}

		public override int GetHashCode ()
		{
			return Top.GetHashCode () ^ Leading.GetHashCode () ^ Trailing.GetHashCode () ^ Bottom.GetHashCode ();
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static NSDirectionalEdgeInsets NSDirectionalEdgeInsetsFromString (IntPtr /* NSString */ s);

		static public NSDirectionalEdgeInsets FromString (string s)
		{
			// note: null is allowed
			var ptr = NSString.CreateNative (s);
			var value = NSDirectionalEdgeInsetsFromString (ptr);
			NSString.ReleaseNative (ptr);
			return value;
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static IntPtr /* NSString */ NSStringFromDirectionalEdgeInsets (NSDirectionalEdgeInsets insets);

		// note: ensure we can roundtrip ToString into FromString
		public override string ToString ()
		{
			using (var ns = new NSString (NSStringFromDirectionalEdgeInsets (this)))
				return ns.ToString ();
		}
#endif
	}

#if !WATCH
	[iOS (9,0)]
	[StructLayout (LayoutKind.Sequential)]
	public struct UIFloatRange : IEquatable<UIFloatRange> {

		public nfloat Minimum, Maximum;
		
		public UIFloatRange (nfloat minimum, nfloat maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static bool UIFloatRangeIsInfinite (UIFloatRange range);

		public bool IsInfinite {
			get {
				return UIFloatRangeIsInfinite (this);
			}
		}

		[DllImport (Constants.UIKitLibrary)]
		static extern bool UIFloatRangeIsEqualToRange (UIFloatRange range, UIFloatRange otherRange);

		public bool Equals (UIFloatRange other)
		{
			return UIFloatRangeIsEqualToRange (this, other);
		}

		public override bool Equals (object other)
		{
			if (other is UIFloatRange)
				return Equals ((UIFloatRange) other);
			return false;
		}

		public override int GetHashCode ()
		{
			return Minimum.GetHashCode () ^ Maximum.GetHashCode ();
		}

		[Field ("UIFloatRangeZero")] // fake (but helps testing and could also help documentation)
		public static UIFloatRange Zero;

		[Field ("UIFloatRangeInfinite")] // fake (but helps testing and could also help documentation)
#if !XAMCORE_2_0
		public static UIFloatRange Infinite = new UIFloatRange (float.NegativeInfinity, float.PositiveInfinity);
#else
		public static UIFloatRange Infinite = new UIFloatRange (nfloat.NegativeInfinity, nfloat.PositiveInfinity);
#endif
	}
#endif
	
#if false
	[Protocol]
	public interface IUITextInputTraits {
		[Export ("autocapitalizationType")]
		UITextAutocapitalizationType AutocapitalizationType { get; set; }

		[Export ("autocorrectionType")]
		UITextAutocorrectionType AutocorrectionType { get; set; }

		[Export ("keyboardType")]
		UIKeyboardType KeyboardType { get; set; }

		[Export ("keyboardAppearance")]
		UIKeyboardAppearance KeyboardAppearance { get; set; }

		[Export ("returnKeyType")]
		UIReturnKeyType ReturnKeyType { get; set; }

		[Export ("enablesReturnKeyAutomatically")]
		bool EnablesReturnKeyAutomatically { get; set; } 

		[Export ("secureTextEntry")]
		bool SecureTextEntry { get; set; } 
	}
#endif
}
