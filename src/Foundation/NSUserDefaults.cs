using System;

using ObjCRuntime;

#nullable enable

namespace Foundation {

	public enum NSUserDefaultsType {
		UserName,
		SuiteName
	}

	public partial class NSUserDefaults {
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.10")]
		[ObsoletedOSPlatform ("ios7.0")]
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
#endif
		public NSUserDefaults (string name) : this (name, NSUserDefaultsType.UserName)
		{
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSUserDefaults (string? name, NSUserDefaultsType type)
		{
			// two different `init*` would share the same C# signature
			switch (type) {
			case NSUserDefaultsType.UserName:
				if (name is null)
					throw new ArgumentNullException (nameof (name));
				Handle = InitWithUserName (name);
				break;
			case NSUserDefaultsType.SuiteName:
				Handle = InitWithSuiteName (name);
				break;
			default:
				throw new ArgumentException (nameof (type));
			}
		}

		public void SetString (string? value, string defaultName)
		{
			using var str = (NSString?) value;
			SetObjectForKey (str, defaultName);
		}

		public NSObject? this [string key] {
			get {
				return ObjectForKey (key);
			}

			set {
				SetObjectForKey (value, key);
			}
		}
	}
}
