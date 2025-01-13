using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreMedia;
using ObjCRuntime;

namespace AVFoundation {
	// Yes, CMTagCollectionCreateWithVideoOutputPreset is in AVFoundation, not CoreMedia.
	public static class CMTagCollectionVideoOutputPreset_Extensions {

		[SupportedOSPlatform ("ios17.2")]
		[SupportedOSPlatform ("maccatalyst17.2")]
		[SupportedOSPlatform ("macos14.2")]
		[SupportedOSPlatform ("tvos17.2")]
		[DllImport (Constants.AVFoundationLibrary)]
		static extern unsafe CMTagCollectionError /* OSStatus */ CMTagCollectionCreateWithVideoOutputPreset (
			IntPtr /* CFAllocatorRef CM_NULLABLE */ allocator,
			CMTagCollectionVideoOutputPreset /* CMTagCollectionVideoOutputPreset */ preset,
			IntPtr* /* CM_RETURNS_RETAINED_PARAMETER CMTagCollectionRef CM_NULLABLE * CM_NONNULL */ newCollection);

		/// <summary>Create a new <see cref="CMTagCollection" /> with the tags that describes the specified video output requirements.</summary>
		/// <param name="value">The requested video output preset to create tags for.</param>
		/// <param name="status">An error code in case of failure, <see cref="CMTagCollectionError.Success" /> otherwise.</param>
		/// <returns>A new <see cref="CMTagCollection" /> with the tags that describes the specified video output requirements, or null in case of failure.</returns>
		[SupportedOSPlatform ("ios17.2")]
		[SupportedOSPlatform ("maccatalyst17.2")]
		[SupportedOSPlatform ("macos14.2")]
		[SupportedOSPlatform ("tvos17.2")]
		public static CMTagCollection? Create (this CMTagCollectionVideoOutputPreset value, out CMTagCollectionError status)
		{
			IntPtr handle;
			unsafe {
				status = CMTagCollectionCreateWithVideoOutputPreset (IntPtr.Zero, value, &handle);
			}
			return CMTagCollection.Create (handle, true);
		}
	}
}
