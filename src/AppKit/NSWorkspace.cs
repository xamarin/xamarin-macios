using System;
using Foundation;

namespace AppKit {

	public partial class NSWorkspace {

		public virtual bool OpenUrls (NSUrl[] urls, string bundleIdentifier, NSWorkspaceLaunchOptions options, NSAppleEventDescriptor descriptor, string[] identifiers)
		{
			// Ignore the passed in argument, because if you pass it in we will crash on cleanup.
			return _OpenUrls (urls, bundleIdentifier, options, descriptor, null);
		}

		public virtual bool OpenUrls (NSUrl[] urls, string bundleIdentifier, NSWorkspaceLaunchOptions options, NSAppleEventDescriptor descriptor)
		{
			return _OpenUrls (urls, bundleIdentifier, options, descriptor, null);
		}

#if !XAMCORE_4_0
		[Obsolete ("Use the overload that takes 'ref NSError' instead.")]
		public virtual NSRunningApplication LaunchApplication (NSUrl url, NSWorkspaceLaunchOptions options, NSDictionary configuration, NSError error)
		{
			return LaunchApplication (url, options, configuration, out error);
		}
#endif
	}
}