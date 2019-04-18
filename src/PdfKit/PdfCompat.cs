#if !XAMCORE_4_0
using System;

namespace PdfKit {
	
	partial class PdfAnnotation {
#if __IOS__
		[Obsolete ("Empty stub (not a public API on iOS).")]
		public virtual string amirvenus { get; set; }
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
#endif
	}
}

#endif
