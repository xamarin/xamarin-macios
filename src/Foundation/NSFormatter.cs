using System;

namespace Foundation {
	public partial class NSFormatter {
#if !XAMCORE_4_0
		[Obsolete ("Use 'IsPartialStringValid (ref string partialString, out NSRange proposedSelRange, string origString, NSRange origSelRange, out string error)' instead.")]
		public virtual bool IsPartialStringValid (out string partialString, out NSRange proposedSelRange, string origString, NSRange origSelRange, out NSString error)
		{
			partialString = origString;
			proposedSelRange = origSelRange;
			error = null;

			return true;
		}
	}
#endif
}
