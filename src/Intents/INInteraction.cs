//
// INInteraction.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if IOS || WATCH
using System;
using Foundation;
using ObjCRuntime;

namespace Intents {
	public partial class INInteraction {

		public T GetParameterValue<T> (INParameter parameter) where T : NSObject
		{
			return Runtime.GetNSObject<T> (_GetParameterValue (parameter));
		}
	}
}
#endif
