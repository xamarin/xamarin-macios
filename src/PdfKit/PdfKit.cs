using System;

using CoreGraphics;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace PdfKit {
	partial class PdfBorder {
		public nfloat []? DashPattern {
			get {
				var arr = WeakDashPattern;
				if (arr is null)
					return null;
				var rv = new nfloat [arr.Count];
				for (uint i = 0; i < rv.Length; i++)
					rv [i] = arr.GetItem<NSNumber> (i).NFloatValue;
				return rv;
			}
			set {
				if (value is null) {
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
		public CGPoint []? QuadrilateralPoints {
			get {
				var arr = WeakQuadrilateralPoints;
				if (arr is null)
					return null;
				var rv = new CGPoint [arr.Count];
				for (uint i = 0; i < rv.Length; i++)
					rv [i] = arr.GetItem<NSValue> (i).CGPointValue;
				return rv;
			}
			set {
				if (value is null) {
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
