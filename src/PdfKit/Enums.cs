//
// Copyright 2011, Novell, Inc.
// Copyright 2011, Regan Sarwas
//
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

//
// Enums.cs: Enums for PdfKit
//
using System;

using ObjCRuntime;
#if MONOMAC || (IOS && XAMCORE_2_0)

namespace PdfKit {

	[iOS (11,0)]
	[Native]
	public enum PdfActionNamedName : long {
		None         = 0,
		NextPage     = 1,
		PreviousPage = 2,
		FirstPage    = 3,
		LastPage     = 4,
		GoBack       = 5,
		GoForward    = 6,
		GoToPage     = 7,
		Find         = 8,
		Print        = 9,
		ZoomIn       = 10,
		ZoomOut      = 11
	}

	[iOS (11,0)]
	[Native]
	public enum PdfWidgetControlType : long {
		Unknown    = -1,
		PushButton  = 0,
		RadioButton = 1,
		CheckBox    = 2
	}

	[iOS (11,0)]
	[Native]
	public enum PdfLineStyle : long {
		None        = 0,
		Square      = 1,
		Circle      = 2,
		Diamond     = 3,
		OpenArrow   = 4,
		ClosedArrow = 5
	}

	[iOS (11,0)]
	[Native]
	public enum PdfMarkupType : long {
		Highlight = 0,
		StrikeOut = 1,
		Underline = 2
	}

	[iOS (11,0)]
	[Native]
	public enum PdfTextAnnotationIconType : long {
		Comment      = 0,
		Key          = 1,
		Note         = 2,
		Help         = 3,
		NewParagraph = 4,
		Paragraph    = 5,
		Insert       = 6
	}

	[iOS (11,0)]
	[Native]
	public enum PdfBorderStyle : long {
		Solid     = 0,
		Dashed    = 1,
		Beveled   = 2,
		Inset     = 3,
		Underline = 4
	}

	[iOS (11,0)]
	[Native]
	public enum PdfPrintScalingMode : long {
		None      = 0,
		ToFit     = 1,
		DownToFit = 2
	}

	[iOS (11,0)]
	[Native]
	public enum PdfDocumentPermissions : long {
		None  = 0,
		User  = 1,
		Owner = 2
	}

	[iOS (11,0)]
	[Native]
	public enum PdfDisplayBox : long {
		Media = 0,
		Crop  = 1,
		Bleed = 2,
		Trim  = 3,
		Art   = 4
	}

	[iOS (11,0)]
	[Native]
	public enum PdfDisplayMode : long {
		SinglePage           = 0,
		SinglePageContinuous = 1,
		TwoUp                = 2,
		TwoUpContinuous      = 3
	}

	[iOS (11,0)]
	[Flags]
	[Native]
	public enum PdfAreaOfInterest : long {
		NoArea         = 0,
		PageArea       = 1 << 0,
		TextArea       = 1 << 1,
		AnnotationArea = 1 << 2,
		LinkArea       = 1 << 3,
		ControlArea    = 1 << 4,
		TextFieldArea  = 1 << 5,
		IconArea       = 1 << 6,
		PopupArea      = 1 << 7,
		ImageArea      = 1 << 8
	}

	[Mac (10,13)]
	[iOS (11,0)]
	[Native]
	public enum PdfDisplayDirection : long {
		Vertical = 0,
		Horizontal = 1,
	}

	[iOS (11,0)]
	[Native]
	public enum PdfInterpolationQuality : long {
		None = 0,
		Low = 1,
		High = 2,
	}

	[NoMac]
	[iOS (11,0)]
	[Native]
	public enum PdfThumbnailLayoutMode : long {
		Vertical = 0,
		Horizontal = 1,
	}

	[iOS (11,0), Mac (10,12)]
	[Native]
	public enum PdfWidgetCellState : long {
		Mixed = -1,
		Off = 0,
		On = 1,
	}
}
#endif
