// Copyright 2014 Xamarin, Inc.
#if IOS || TVOS
using System;
using System.Runtime.InteropServices;
using XamCore.Foundation;
#if IOS
using XamCore.NotificationCenter;
#endif
using XamCore.ObjCRuntime;

namespace XamCore.UIKit {

	public partial class UIVibrancyEffect {

#if IOS // This code comes from NotificationCenter
		// This is a [Category] -> C# extension method (see adlib.cs) but it targets on static selector
		// the resulting syntax does not look good in user code so we provide a better looking API
		// https://trello.com/c/iQpXOxCd/227-category-and-static-methods-selectors
		// note: we cannot reuse the same method name - as it would break compilation of existing apps
		[iOS (8,0)]
		static public UIVibrancyEffect CreateForNotificationCenter ()
		{
			return (null as UIVibrancyEffect).NotificationCenterVibrancyEffect ();
		}
#endif // IOS
	}
}
#endif // IOS || TVOS
