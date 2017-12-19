//
// INSaveProfileInCarIntent Extensions
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && IOS

using XamCore.Foundation;
using XamCore.Intents;
using XamCore.ObjCRuntime;
using XamCore.UIKit;

namespace XamCore.Intents {
	public partial class INSaveProfileInCarIntent {

		public INSaveProfileInCarIntent (NSNumber profileNumber, string profileLabel)
		{
			// Apple created this change in 10,2
			if (UIDevice.CurrentDevice.CheckSystemVersion (10, 2))
				InitializeHandle (InitWithProfileNumberName (profileNumber, profileLabel));
			else
				InitializeHandle (InitWithProfileNumberLabel (profileNumber, profileLabel));
		}
	}
}
#endif
