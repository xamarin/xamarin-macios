//
// MLMultiArray.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.CoreML {
	public partial class MLMultiArray {

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		public NSNumber this [nint idx] {
			get { return GetObject (idx); }
			set { SetObject (value, idx); }
		}

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		public NSNumber this [NSNumber [] key] {
			get { return GetObject (key); }
			set { SetObject (value, key); }
		}
	}
}
#endif
