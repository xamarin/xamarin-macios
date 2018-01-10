//
#if IOS
using System;
using System.Drawing;
using System.Threading.Tasks;
using CoreMedia;
using CoreMotion;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using CoreLocation;
using OpenTK;

namespace AVFoundation {

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

		public AVOutputSettingsAssistant PresetHevc1920x1080 {
			get {
				return FromPreset (_PresetHevc1920x1080);
			}
		}

		public AVOutputSettingsAssistant PresetHevc3840x2160 {
			get {
				return FromPreset (_PresetHevc3840x2160);
			}
		}

	}
}

#endif // IOS
