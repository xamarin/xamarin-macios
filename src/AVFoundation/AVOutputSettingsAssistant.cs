//
#if IOS
using System;
using System.Drawing;
using System.Threading.Tasks;
using XamCore.CoreMedia;
using XamCore.CoreMotion;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreAnimation;
using XamCore.CoreLocation;
using OpenTK;

namespace XamCore.AVFoundation {

	public unsafe partial class AVOutputSettingsAssistant : NSObject {
		public AVOutputSettingsAssistant Preset640x480 {
			get {
				return FromPreset (_Preset640x480);
			}
		}
		
		public AVOutputSettingsAssistant Preset960x540 {
			get {
				return FromPreset (_Preset960x540);
			}
		}
		
		public AVOutputSettingsAssistant Preset1280x720 {
			get {
				return FromPreset (_Preset1280x720);
			}
		}
		
		public AVOutputSettingsAssistant Preset1920x1080 {
			get {
				return FromPreset (_Preset1920x1080);
			}
		}

		public AVOutputSettingsAssistant Preset3840x2160 {
			get {
				return FromPreset (_Preset3840x2160);
			}
		}
	}
}

#endif // IOS
