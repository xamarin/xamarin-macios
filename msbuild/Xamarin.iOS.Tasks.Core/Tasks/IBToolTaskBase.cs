using System;
using System.Collections.Generic;

using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks {
	public abstract class IBToolTaskBase : Xamarin.MacDev.Tasks.IBToolTaskBase
	{
		protected override bool AutoActivateCustomFonts {
			get { return true; }
		}

		static bool IsWatchExtension (PDictionary plist)
		{
			PDictionary extension;
			PString id;

			if (!plist.TryGetValue ("NSExtension", out extension))
				return false;

			if (!extension.TryGetValue ("NSExtensionPointIdentifier", out id))
				return false;

			return id.Value == "com.apple.watchkit";
		}

		protected override IEnumerable<string> GetTargetDevices (PDictionary plist)
		{
			if (AppleSdkSettings.XcodeVersion < new Version (6, 3))
				yield break;

			var devices = IPhoneDeviceType.NotSet;
			bool watch = false;

			if (plist != null) {
				if (!(watch = plist.GetWKWatchKitApp ())) {
					// the project is either a normal iOS project or an extension
					if ((devices = plist.GetUIDeviceFamily ()) == IPhoneDeviceType.NotSet) {
						// library projects and extension projects will not have this key, but
						// we'll want them to work for both iPhones and iPads if the
						// xib or storyboard supports them
						devices = IPhoneDeviceType.IPhoneAndIPad;
					}

					// if the project is a watch extension, we'll also want to include watch support
					watch = IsWatchExtension (plist);
				} else {
					// the project is a WatchApp, only include watch support
				}
			} else {
				devices = IPhoneDeviceType.IPhoneAndIPad;
			}

			if ((devices & IPhoneDeviceType.IPhone) != 0)
				yield return "iphone";

			if ((devices & IPhoneDeviceType.IPad) != 0)
				yield return "ipad";

			if (watch)
				yield return "watch";

			yield break;
		}
	}
}
