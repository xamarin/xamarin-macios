//
// UIEnums.cs:
//
// Copyright 2009-2011 Novell, Inc.
// Copyright 2011-2012, Xamarin Inc.
//
// Author:
//  Miguel de Icaza
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.UIKit {

	// UIGeometry.h
	[iOS (5,0)]
	public struct UIOffset {

		// API match for UIOffsetZero field/constant
		[Field ("UIOffsetZero")] // fake (but helps testing and could also help documentation)
		public static readonly UIOffset Zero;

		public UIOffset (nfloat horizontal, nfloat vertical)
		{
			Horizontal = horizontal;
			Vertical = vertical;
		}
		public /* CGFloat */ nfloat Horizontal;
		public /* CGFloat */ nfloat Vertical;

		public override bool Equals (object obj)
		{
			if (!(obj is UIOffset))
				return false;
			var other = (UIOffset) obj;
			return other.Horizontal == Horizontal && other.Vertical == Vertical;
		}

		public override int GetHashCode ()
		{
			return Horizontal.GetHashCode () ^ Vertical.GetHashCode ();
		}

		public static bool operator == (UIOffset left, UIOffset right)
		{
			return (left.Horizontal == right.Horizontal) && (left.Vertical == right.Vertical);
		}

		public static bool operator != (UIOffset left, UIOffset right)
		{
			return (left.Horizontal != right.Horizontal) || (left.Vertical != right.Vertical);
		}
	}
}
