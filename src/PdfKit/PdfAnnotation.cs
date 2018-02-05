//
// PdfAnnotation.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

#if XAMCORE_2_0
namespace PdfKit {
	public partial class PdfAnnotation {

		[Mac (10,12)]
		public bool SetValue<T> (T value, PdfAnnotationKey key) where T : class, INativeObject
		{
			if (value == null)
				throw new ArgumentNullException (nameof (value));

			return _SetValue (value.Handle, key.GetConstant ());
		}

		[Mac (10,12)]
		public bool SetValue (string str, PdfAnnotationKey key)
		{
			var nstr = NSString.CreateNative (str);
			try {
				return _SetValue (nstr, key.GetConstant ());
			} finally {
				NSString.ReleaseNative (nstr);
			}
		}

		[Mac (10,12)]
		public T GetValue<T> (PdfAnnotationKey key) where T : class, INativeObject
		{
			return Runtime.GetINativeObject<T> (_GetValue (key.GetConstant ()), true);
		}

		public PdfAnnotationKey AnnotationType {
			get { return PdfAnnotationKeyExtensions.GetValue ((NSString) Type); }
			set { Type = value.GetConstant (); }
		}

		[Mac (10,13)]
		public CGPoint[] QuadrilateralPoints {
			get {
				return NSArray.ArrayFromHandleFunc<CGPoint> (_QuadrilateralPoints, (v) =>
					{
						using (var value = new NSValue (v))
							return value.CGPointValue;
					});
			}
			set {
				if (value == null) {
					_QuadrilateralPoints = IntPtr.Zero;
				} else {
					using (var arr = new NSMutableArray ()) {
						for (int i = 0; i < value.Length; i++)
							arr.Add (NSValue.FromCGPoint (value [i]));
						_QuadrilateralPoints = arr.Handle;
					}
				}
			}
		}
	}
}
#endif // XAMCORE_2_0
