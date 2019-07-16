//
// CGEnums.cs: Enumerations
//
// Author:
//   Vincent Dondain (vidondai@microsoft.com)
//
// Copyright 2018-2019 Microsoft Corporation
//

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace CoreGraphics {

	public enum MatrixOrder {
		Prepend = 0,
		Append = 1,
	}

	[Mac (10,15)]
	[iOS (13,0)]
	public enum CGPdfTagType /* int32_t */ {
		Document = 100,
		Part,
		Art,
		Section,
		Div,
		BlockQuote,
		Caption,
		Toc,
		Toci,
		Index,
		NonStructure,
		Private,
		Paragraph = 200,
		Header,
		Header1,
		Header2,
		Header3,
		Header4,
		Header5,
		Header6,
		List = 300,
		ListItem,
		Label,
		ListBody,
		Table = 400,
		TableRow,
		TableHeaderCell,
		TableDataCell,
		TableHeader,
		TableBody,
		TableFooter,
		Span = 500,
		Quote,
		Note,
		Reference,
		Bibliography,
		Code,
		Link,
		Annotation,
		Ruby = 600,
		RubyBaseText,
		RubyAnnotationText,
		RubyPunctuation,
		Warichu,
		WarichuText,
		WarichuPunctiation,
		Figure = 700,
		Formula,
		Form,
	}

	[Mac (10,15)]
	[iOS (13,0)]
	[TV (13,0)]
	[Watch (6,0)]
	public static class CGPdfTagType_Extensions {

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* const char * _Nullable */ IntPtr CGPDFTagTypeGetName (CGPdfTagType tagType);

		public static string GetName (this CGPdfTagType self)
		{
			return Marshal.PtrToStringAnsi (CGPDFTagTypeGetName (self));
		}
	}
}
