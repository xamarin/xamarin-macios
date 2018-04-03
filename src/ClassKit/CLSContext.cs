//
// CLSContext.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.ClassKit {
	public partial class CLSContext {

		public CLSContextTopic Topic {
			get => CLSContextTopicExtensions.GetValue (WeakTopic);
			set => WeakTopic = value.GetConstant ();
		}
	}
}
#endif
