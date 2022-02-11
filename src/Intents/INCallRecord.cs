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
using System.Runtime.Versioning;

namespace Intents {
#if NET
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
#endif
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
