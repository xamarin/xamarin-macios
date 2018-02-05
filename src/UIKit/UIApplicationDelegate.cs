//
// UIApplicationDelegate.cs
//
// Copyrigh 2014, Xamarin Inc.
//

#if !XAMCORE_2_0

using System;
using Foundation;

namespace UIKit {

	public partial class UIApplicationDelegate {

		[Obsolete ("Override the overload accepting an UIApplication argument")]
		public virtual void UserActivityUpdated (NSUserActivity userActivity) 
		{
			throw new NotImplementedException ();
		}
	}

	public static partial class UIApplicationDelegate_Extensions {

		[Obsolete ("Override the overload accepting an UIApplication argument")]
		public static void UserActivityUpdated (IUIApplicationDelegate This, MonoTouch.Foundation.NSUserActivity userActivity)
		{
			throw new NotImplementedException ();
		}
	}
}

#endif
