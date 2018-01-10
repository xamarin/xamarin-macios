// Copyright 2014 Xamarin Inc
//
//
using System;

namespace Foundation {

	public partial class NSUrlComponents {

		// we cannot make the ToString method depend on a new (iOS8) selector as the type was added before (iOS7)
//		public override string ToString ()
//		{
//			return _String;
//		}
	}
}
