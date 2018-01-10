// Copyright 2012 Xamarin Inc

#if !MONOMAC

using System;

namespace Foundation {

	public partial class NSCompoundPredicate {

#if !XAMCORE_2_0
		[Obsolete ("This instance of NSCompoundPredicate will be unusable. Symbol kept for binary compatibility")]
		public NSCompoundPredicate () : base (IntPtr.Zero)
		{
		}
#endif
	}
}

#endif // !MONOMAC
