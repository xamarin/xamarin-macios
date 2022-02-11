//
// CLSContext.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace ClassKit {
#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios11.4")]
	[UnsupportedOSPlatform ("tvos")]
#endif
	public partial class CLSContext {

		public CLSContextTopic Topic {
			get => CLSContextTopicExtensions.GetValue (WeakTopic);
			set => WeakTopic = value.GetConstant ();
		}
	}
}
