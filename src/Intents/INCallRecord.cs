//
// INCallRecord.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !TVOS
using System;

using Foundation;

using ObjCRuntime;

namespace Intents {
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
