// Compatibility stubs

using System;
using System.ComponentModel;

using ObjCRuntime;

#nullable enable

namespace CoreSpotlight {

#if !NET && IOS
	partial class CSCustomAttributeKey {

		[Obsolete ("Use .ctor(string)")]
		public CSCustomAttributeKey () : this (String.Empty)
		{
		}
	}
#endif

#if !TV
	public partial class CSSearchQueryContext {

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property was removed. The getter always returns null and the setter throws and InvalidOperationException.")]
#if NET
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public virtual string []? ProtectionClasses {
			get => null;
			set => throw new InvalidOperationException (Constants.ApiRemovedGeneral);
		}
	}
#endif
}

