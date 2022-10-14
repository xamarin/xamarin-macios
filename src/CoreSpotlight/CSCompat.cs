// Compatibility stubs

#if !NET && IOS

using System;
using ObjCRuntime;

namespace CoreSpotlight {

	partial class CSCustomAttributeKey {

		[Obsolete ("Use .ctor(string)")]
		public CSCustomAttributeKey () : this (String.Empty)
		{
		}
	}

#if !TV
	public partial class CSSearchQueryContext {

#if !NET
		[Obsolete ("This property was removed. The getter always returns null and the setter throws and InvalidOperationException.")]
#else
		[UnsupportedOSPlatform ("ios16.1")]
		[UnsupportedOSPlatform ("maccatalyst16.1")]
		[UnsupportedOSPlatform ("macos13.0")]
#endif
		public string[] ProtectionClasses { 
			get => null; 
			set => throw new InvalidOperationException (Constants.ApiRemovedGeneral);
		}
	}
#endif
}

#endif
