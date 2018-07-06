using ObjCRuntime;
using System;

namespace Foundation {
	public partial class NSHttpCookieStorage {
#if !XAMCORE_2_0
		// sadly they were not readonly
		public static NSString CookiesChangedNotification;
		public static NSString AcceptPolicyChangedNotification;

		static NSHttpCookieStorage ()
		{
			var handle = Libraries.Foundation.Handle;
			if (handle == IntPtr.Zero)
				return;

			CookiesChangedNotification = Dlfcn.GetStringConstant (handle, "NSHTTPCookieManagerCookiesChangedNotification");
			AcceptPolicyChangedNotification = Dlfcn.GetStringConstant (handle, "NSHTTPCookieManagerAcceptPolicyChangedNotification");
		}
#endif
	}
}