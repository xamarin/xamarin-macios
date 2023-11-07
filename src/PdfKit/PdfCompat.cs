#if !NET
using System;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace PdfKit {

	partial class PdfAnnotation {
#if __IOS__
		[Obsolete ("Empty stub (not a public API on iOS).")]
		public virtual string? ToolTip { get; }
#endif
	}

	partial class PdfBorder {
		[Obsolete ("Empty stub (not a public API).")]
		public virtual nfloat HorizontalCornerRadius { get; set; }

		[Obsolete ("Empty stub (not a public API).")]
		public virtual nfloat VerticalCornerRadius { get; set; }
	}

	partial class PdfView {
#if __IOS__
		[Obsolete ("Empty stub (not a public API on iOS).")]
		public virtual nfloat GreekingThreshold { get; set; }

		[Obsolete ("Empty stub (not a public API on iOS).")]
		public virtual bool ShouldAntiAlias { get; set; }

		[Obsolete ("Empty stub (not a public API on iOS).")]
		public virtual void TakeBackgroundColor (NSObject sender) {}

		[Obsolete ("Empty stub (not a public API on iOS).")]
		public virtual void TakePasswordFrom (NSObject sender) {}
#endif
	}

	public partial class PdfAnnotation {

#if __IOS__
		[Obsolete ("Empty stub (not a public API).")]
		public virtual PdfAction? MouseUpAction { get; set; }

		[Obsolete ("Empty stub (not a public API).")]
		public virtual void RemoveAllAppearanceStreams () {}
#endif
	}
}

#endif
