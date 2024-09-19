#if NET
using System;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace CoreData {
	public partial class NSPersistentStoreCoordinator {
#if !__TVOS__
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[UnsupportedOSPlatform ("tvos")]
		public NSManagedObjectID GetManagedObjectId (string value)
		{
			using var str = new TransientString (value);
			return GetManagedObjectId ((IntPtr) str, (nuint) value.Length);
		}
#endif // !__TVOS__
	}
}
#endif
