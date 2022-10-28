//
// INSearchCallHistoryIntent.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !(NET && __MACOS__)
#if !TVOS
using System;
using Foundation;
using ObjCRuntime;

namespace Intents {
	public partial class INSearchCallHistoryIntent {

		public bool? Unseen {
			get { return WeakUnseen?.BoolValue; }
		}
	}
}
#endif
#endif // !(NET && __MACOS__)
