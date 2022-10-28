#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreLocation {

#nullable enable
#if !NET && !WATCH

#if !TVOS
	public partial class CLHeading {

		[Obsolete ("Use the 'Description' property from 'NSObject'.")]
		public new virtual string Description ()
		{
			return base.Description;
		}
	}
#endif // !TVOS

	public partial class CLLocation {

		[Obsolete ("Use the 'Description' property from 'NSObject'.")]
		public new virtual string Description ()
		{
			return base.Description;
		}
	}
#endif // !NET && !WATCH

#if !NET && (WATCH || TVOS || MONOMAC)

	// Symbol in Xcode 13.2 from watchOS and tvOS

	[Obsolete (Constants.UnavailableOnThisPlatform)]
	[Native]
	public enum CLLocationPushServiceError : long {
		Unknown = 0,
		MissingPushExtension = 1,
		MissingPushServerEnvironment = 2,
		MissingEntitlement = 3,
	}

	[Obsolete (Constants.UnavailableOnThisPlatform)]
	public static class CLLocationPushServiceErrorExtensions {
		public static NSString? GetDomain (this CLLocationPushServiceError self) => throw new PlatformNotSupportedException (Constants.UnavailableOnThisPlatform);
	}

	[Obsolete (Constants.UnavailableOnThisPlatform)]
	public interface ICLLocationPushServiceExtension : INativeObject, IDisposable
	{
		void DidReceiveLocationPushPayload (NSDictionary<NSString, NSObject> payload, Action completion);
	}

	[Obsolete (Constants.UnavailableOnThisPlatform)]
	public static partial class CLLocationPushServiceExtension_Extensions {
		public static void ServiceExtensionWillTerminate (this ICLLocationPushServiceExtension This) => throw new PlatformNotSupportedException (Constants.UnavailableOnThisPlatform);
	}

#endif // !NET && (WATCH || TVOS)
}
