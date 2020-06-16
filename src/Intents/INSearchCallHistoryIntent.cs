//
// INSearchCallHistoryIntent.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

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
