using System;

namespace Extrospection {

	public static partial class Helpers {

		public static bool Filter (string fx)
		{
			switch (fx) {
			// macOS only - unsupported frameworks
			case "AE":
			case "AGL":
			case "AppleScriptObjC":
			case "ATS":
			case "AudioVideoBridging":
			case "Automator":
			case "CalendarStore":
			case "CarbonCore":
			case "CFOpenDirectory":
			case "Collaboration":
			case "ColorSync":
			case "CommonPanels":
			case "CryptoTokenKit":
			case "DictionaryServices":
			case "DirectoryService":
			case "DiskArbitration":
			case "DiscRecording":
			case "DiscRecordingUI":
			case "DVDPlayback":
			case "ExceptionHandling":
			case "ForceFeedback":
			case "FSEvents":
			case "FWAUserLib":
			case "GLUT":
			case "Help":
			case "HIServices":
			case "HIToolbox":
			case "Hypervisor":
			case "ICADevices":
			case "ImageCaptureCore":
			case "ImageCapture":
			case "IMServicePlugIn":
			case "Ink":
			case "InputMethodKit":
			case "InstallerPlugins":
			case "IOBluetooth":
			case "IOBluetoothUI":
			case "IOKit":
			case "JavaFrameEmbedding":
			case "JavaVM":
			case "Kerberos":
			case "LatentSemanticMapping":
			case "LaunchServices":
			case "Metadata":
			case "NavigationServices":
			case "NetFS":
			case "OpenCL":
			case "OpenDirectory":
			case "OpenScripting":
			case "OSAKit":
			case "OSServices":
			case "PreferencePanes":
			case "Python":
			case "QD":
			case "QuartzFilters":
			case "ruby":
			case "ScreenSaver":
			case "Scripting":
			case "ScriptingBridge":
			case "SearchKit":
			case "SecurityFoundation": // bugzilla
			case "SecurityHI":
			case "ServiceManagement":
			case "SharedFileList":
			case "SpeechRecognition":
			case "SpeechSynthesis":
			case "SyncServices":
			case "Tcl":
			case "TWAIN":
			case "vmnet":
				// other non-supported frameworks
			case "GSS": // iOS and macOS
			case "vecLib": // all
				return true;
			default:
				return false;
			}
		}
	}
}
