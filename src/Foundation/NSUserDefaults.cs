using System;
using ObjCRuntime;

namespace Foundation {

	public enum  NSUserDefaultsType {
		UserName,
		SuiteName
	}

	public partial class NSUserDefaults {
#if NET
		[SupportedOSPlatform ("macos10.9")]
		[UnsupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("ios7.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.10.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Mac (10, 9)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
#endif
		public NSUserDefaults (string name) : this (name, NSUserDefaultsType.UserName)
		{
		}

#if NET
		[SupportedOSPlatform ("ios7.0")]
#else
		[iOS (7,0)]
#endif
		public NSUserDefaults (string name, NSUserDefaultsType type)
		{
			// two different `init*` would share the same C# signature
			switch (type) {
			case NSUserDefaultsType.UserName:
				Handle = InitWithUserName (name);
				break;
			case NSUserDefaultsType.SuiteName:
				Handle = InitWithSuiteName (name);
				break;
			default:
				throw new ArgumentException ("type");
			}
		}

		public void SetString (string value, string defaultName)
		{
			NSString str = new NSString (value);

			SetObjectForKey (str, defaultName);
			
			str.Dispose ();
		}

		public NSObject this [string key] {
			get {
				return ObjectForKey (key);
			}

			set {
				SetObjectForKey (value, key);
			}
		}
	}
}
