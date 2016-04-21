// Copyright 2013 Xamarin Inc. All rights reserved.

using System;

namespace XamCore.Foundation {

	public partial class NSMutableString {

		// check helpers to avoid native exceptions

		void Check (NSRange range)
		{
			if (range.Location + range.Length > Length)
				throw new ArgumentOutOfRangeException ("range");
		}

		void Check (nint index)
		{
			if (index < 0 || index > Length)
				throw new ArgumentOutOfRangeException ("index");
		}

		public bool ApplyTransform (NSStringTransform transform, bool reverse, NSRange range, out NSRange resultingRange)
		{
			return ApplyTransform (NSString.NSStringTransformToCode (transform), reverse, range, out resultingRange);
		}
	}
}
