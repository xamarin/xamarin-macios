//
// INSaveProfileInCarIntent Extensions
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if IOS

using Foundation;
using Intents;
using ObjCRuntime;
using UIKit;

namespace Intents {
	public partial class INSaveProfileInCarIntent {

		public INSaveProfileInCarIntent (NSNumber profileNumber, string profileLabel)
		{
			// Apple created this change in 10,2
			if (SystemVersion.CheckiOS (10, 2))
				InitializeHandle (InitWithProfileNumberName (profileNumber, profileLabel));
			else
				InitializeHandle (InitWithProfileNumberLabel (profileNumber, profileLabel));
		}
	}
}
#endif
