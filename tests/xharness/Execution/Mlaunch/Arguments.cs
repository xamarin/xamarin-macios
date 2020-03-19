namespace Xharness.Execution.Mlaunch {

	/// <summary>
	/// Specify the location of Apple SDKs, default to 'xcode-select' value.
	/// </summary>
	public sealed class SdkRootArgument : SingleValueArgument {
		public SdkRootArgument (string sdkPath) : base ("sdkroot", sdkPath)
		{
		}
	}

	/// <summary>
	/// List the currently connected devices and their UDIDs.
	/// </summary>
	public sealed class ListDevicesArgument : SingleValueArgument {
		public ListDevicesArgument (string outputFile) : base ("listdev", outputFile)
		{
		}
	}

	/// <summary>
	/// List the available simulators. The output is xml, and written to the specified file.
	/// </summary>
	public sealed class ListSimulatorsArgument : SingleValueArgument {
		public ListSimulatorsArgument (string outputFile) : base ("listsim", outputFile)
		{
		}
	}

	/// <summary>
	/// Lists crash reports on the specified device
	/// </summary>
	public sealed class ListCrashReportsArgument : SingleValueArgument {
		public ListCrashReportsArgument (string outputFile) : base ("list-crash-reports", outputFile)
		{
		}
	}

	/// <summary>
	/// Specifies the device type to launch the simulator as.
	/// </summary>
	public sealed class DeviceArgument : SingleValueArgument {
		public DeviceArgument (string deviceType) : base ("device", deviceType)
		{
		}
	}

	/// <summary>
	/// Specify which device (when many are present) the [install|lauch|kill|log]dev command applies.
	/// </summary>
	public sealed class DeviceNameArgument : SingleValueArgument {
		public DeviceNameArgument (string deviceName) : base ("devname", deviceName)
		{
		}
	}

	/// <summary>
	/// Install the specified iOS app bundle on the device.
	/// </summary>
	public sealed class InstallAppArgument : SingleValueArgument {
		public InstallAppArgument (string appPath) : base ("installdev", appPath)
		{
		}
	}

	/// <summary>
	/// Uninstall the specified bundle id from the device.
	/// </summary>
	public sealed class UninstallAppArgument : SingleValueArgument {
		public UninstallAppArgument (string appBundleId) : base ("uninstalldevbundleid", appBundleId)
		{
		}
	}
	
	/// <summary>
	/// Specify the output format for some commands as Default.
	/// </summary>
	public sealed class DefaultOutputFormatArgument : SingleValueArgument {
		public DefaultOutputFormatArgument () : base ("output-format", "Default")
		{
		}
	}

	/// <summary>
	/// Specify the output format for some commands as XML.
	/// </summary>
	public sealed class XmlOutputFormatArgument : SingleValueArgument {
		public XmlOutputFormatArgument () : base ("output-format", "XML")
		{
		}
	}

	/// <summary>
	/// Download a crash report from the specified device.
	/// </summary>
	public sealed class DownloadCrashReportArgument : SingleValueArgument {
		public DownloadCrashReportArgument (string deviceName) : base ("download-crash-report", deviceName)
		{
		}
	}

	/// <summary>
	/// Specifies the file to save the downloaded crash report.
	/// </summary>
	public sealed class DownloadCrashReportToArgument : SingleValueArgument {
		public DownloadCrashReportToArgument (string outputFile) : base ("download-crash-report-to", outputFile)
		{
		}
	}

	/// <summary>
	/// Include additional data (which can take some time to fetch) when listing the connected devices.
	/// Only applicable when output format is xml.
	/// </summary>
	public sealed class ListExtraDataArgument : OptionArgument {
		public ListExtraDataArgument () : base ("list-extra-data")
		{
		}
	}

	public sealed class VerbosityArgument : OptionArgument {
		public VerbosityArgument () : base ("v")
		{
		}
	}
}
