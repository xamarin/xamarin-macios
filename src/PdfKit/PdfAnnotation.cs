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
		public bool SetValue<T> (T value, PDFAnnotationKey key) where T : class, INativeObject
		{
			return _SetValue (value.Handle, key.GetConstant ());
		}

		[Mac (10,12)]
		public bool SetValue (string str, PDFAnnotationKey key)
		{
			var nstr = NSString.CreateNative (str);
			try {
				return _SetValue (nstr, key.GetConstant ());
			} finally {
				NSString.ReleaseNative (nstr);
			}
		}

		[Mac (10,12)]
		public T GetValue<T> (PDFAnnotationKey key) where T : class, INativeObject
		{
			return Runtime.GetINativeObject<T> (_GetValue (key.GetConstant ()), true);
		}
	}
}
#endif // XAMCORE_2_0
