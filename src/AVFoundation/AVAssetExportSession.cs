#if !WATCH
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XamCore.AVFoundation {

	internal static class AVAssetExportSessionPresetMethods
	{
		public static string GetString (this AVAssetExportSessionPreset preset)
		{
			switch (preset) {
			case AVAssetExportSessionPreset.LowQuality:
				return AVAssetExportSession.PresetLowQuality;
			case AVAssetExportSessionPreset.MediumQuality:
				return AVAssetExportSession.PresetMediumQuality;
			case AVAssetExportSessionPreset.HighestQuality:
				return AVAssetExportSession.PresetHighestQuality;
			case AVAssetExportSessionPreset.Preset640x480:
				return AVAssetExportSession.Preset640x480;
			case AVAssetExportSessionPreset.Preset960x540:
				return AVAssetExportSession.Preset960x540;
			case AVAssetExportSessionPreset.Preset1280x720:
				return AVAssetExportSession.Preset1280x720;
			case AVAssetExportSessionPreset.Preset1920x1080:
				return AVAssetExportSession.Preset1920x1080;
#if !MONOMAC
			case AVAssetExportSessionPreset.Preset3840x2160:
				return AVAssetExportSession.Preset3840x2160;
#endif
			case AVAssetExportSessionPreset.AppleM4A:
				return AVAssetExportSession.PresetAppleM4A;
			case AVAssetExportSessionPreset.Passthrough:
				return AVAssetExportSession.PresetPassthrough ;
			default:
				return null;
			}
		}
	} 
}
#endif
