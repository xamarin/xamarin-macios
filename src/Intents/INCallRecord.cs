//
// INCallRecord.cs
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

namespace XamCore.Intents {
	public partial class INCallRecord {

		public double? CallDuration {
			get { return WeakCallDuration?.DoubleValue; }
		}

		public bool? Unseen {
			get { return WeakUnseen?.BoolValue; }
		}
	}
}
#endif
