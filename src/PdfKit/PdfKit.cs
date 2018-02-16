using System;

using CoreGraphics;
using Foundation;

#if XAMCORE_2_0
namespace PdfKit {
	partial class PdfBorder {
		nfloat [] DashPattern {
			get {
				var arr = WeakDashPattern;
				if (arr == null)
					return null;
				var rv = new nfloat [arr.Count];
				for (uint i = 0; i < rv.Length; i++)
					rv [i] = arr.GetItem<NSNumber> (i).NFloatValue;
				return rv;
			}
			set {
				if (value == null) {
					WeakDashPattern = null;
				} else {
					var arr = new NSNumber [value.Length];
					for (int i = 0; i < arr.Length; i++)
						arr [i] = new NSNumber (value [i]);
					WeakDashPattern = NSArray.FromNSObjects (arr);
				}
			}
		}
	}

#if !IOS
	partial class PdfAnnotationMarkup {
		CGPoint [] QuadrilateralPoints {
			get {
				var arr = WeakQuadrilateralPoints;
				if (arr == null)
					return null;
				var rv = new CGPoint [arr.Count];
				for (uint i = 0; i < rv.Length; i++)
					rv [i] = arr.GetItem<NSValue> (i).CGPointValue;
				return rv;
			}
			set {
				if (value == null) {
					WeakQuadrilateralPoints = null;
				} else {
					var arr = new NSValue [value.Length];
					for (int i = 0; i < arr.Length; i++)
						arr [i] = NSValue.FromCGPoint (value [i]);
					WeakQuadrilateralPoints = NSArray.FromNSObjects (arr);
				}
			}
		}
	}
#endif // !IOS
}
#endif
