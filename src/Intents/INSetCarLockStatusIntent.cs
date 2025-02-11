//
// INSetCarLockStatusIntent.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if IOS

using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetCarLockStatusIntent {

		public INSetCarLockStatusIntent (bool? locked, INSpeakableString carName) :
			this (locked.HasValue ? new NSNumber (locked.Value) : null, carName)
		{
		}
	}
}

#endif
