//
// PdfAnnotation.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

#if XAMCORE_2_0
namespace XamCore.PdfKit {
	public partial class PdfAnnotation {

		[Mac (10,12)]
		public bool SetValue<T> (T value, PdfAnnotationKey key) where T : class, INativeObject
		{
			if (value == null)
				throw new ArgumentNullException (nameof (value));

			return _SetValue (value.Handle, key.GetConstant ());
		}

		// [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
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
	}
}
#endif // XAMCORE_2_0
