using System;
using System.Globalization;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.UIKit {

#if WATCH
	internal
#else
	public
#endif
	partial class UIDevice {
#if !WATCH
		public UIUserInterfaceIdiom UserInterfaceIdiom {
			get {
				Selector userInterfaceIdiom = new Selector ("userInterfaceIdiom");
				if (RespondsToSelector (userInterfaceIdiom))
					return _UserInterfaceIdiom;
				else
					return UIUserInterfaceIdiom.Phone;
			}
		}

		[iOS (4,0)]
		public bool IsMultitaskingSupported {
			get {
				Selector mtsupported = new Selector ("isMultitaskingSupported");
				if (RespondsToSelector (mtsupported))
					return _IsMultitaskingSupported;
				return false;
			}
		}
#endif
		
		public bool CheckSystemVersion (int major, int minor)
		{
#if WATCH
			return Runtime.CheckSystemVersion (major, minor, WatchKit.WKInterfaceDevice.CurrentDevice.SystemVersion);
#else
			return Runtime.CheckSystemVersion (major, minor, SystemVersion);
#endif
		}

#if !XAMCORE_2_0
		[Obsolete ("Deprecated in iOS 5.0. Apple now reject application using it the selector is removed and an empty string is returned")]
		public virtual string UniqueIdentifier {
			get { return string.Empty; }
		}
#endif
	}
}
