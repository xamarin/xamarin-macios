using System;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch {

	/// <summary>
	/// Specify the location of Apple SDKs, default to 'xcode-select' value.
	/// </summary>
	public sealed class SdkRootArgument : SingleValueArgument {
		public SdkRootArgument (string sdkPath) : base ("sdkroot", sdkPath, false)
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
	/// Write the syslog from the device to the console.
	/// </summary>
	public sealed class LogDevArgument : OptionArgument {
		public LogDevArgument () : base ("logdev")
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
		public DeviceNameArgument (string deviceName) : base ("devname", deviceName, false)
		{
		}
	}

	/// <summary>
	/// Install the specified iOS app bundle on the device.
	/// </summary>
	public sealed class InstallAppOnDeviceArgument : SingleValueArgument {
		public InstallAppOnDeviceArgument (string appPath) : base ("installdev", appPath, false)
		{
		}
	}

	/// <summary>
	/// Uninstall the specified bundle id from the device.
	/// </summary>
	public sealed class UninstallAppFromDeviceArgument : SingleValueArgument {
		public UninstallAppFromDeviceArgument (string appBundleId) : base ("uninstalldevbundleid", appBundleId, false)
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

	/// <summary>
	/// Attach native debugger.
	/// </summary>
	public sealed class AttachNativeDebuggerArgument : OptionArgument {
		public AttachNativeDebuggerArgument () : base ("attach-native-debugger")
		{
		}
	}

	/// <summary>
	/// Attempt to disable memory limits for launched apps.
	/// This is just an attempt, some or all usual limits may still be enforced.
	/// </summary>
	public sealed class DisableMemoryLimitsArgument : OptionArgument {
		public DisableMemoryLimitsArgument () : base ("disable-memory-limits")
		{
		}
	}

	public sealed class WaitForExitArgument : OptionArgument {
		public WaitForExitArgument () : base ("wait-for-exit")
		{
		}
	}

	/// <summary>
	/// Launch the app with this command line argument. This must be specified multiple times for multiple arguments.
	/// </summary>
	public sealed class SetAppArgumentArgument : MlaunchArgument {
		readonly string value;

		public SetAppArgumentArgument (string value, bool isAppArg = false)
		{
			this.value = value ?? throw new ArgumentNullException (nameof (value));

			if (isAppArg)
				this.value = "-app-arg:" + this.value;
		}

		public override string AsCommandLineArgument () => "-argument=" + value;
	}

	/// <summary>
	/// Set the environment variable in the application on startup.
	/// </summary>
	public sealed class SetEnvVariableArgument : MlaunchArgument {
		readonly string variableName;
		readonly string variableValue;

		public SetEnvVariableArgument (string variableName, object variableValue)
		{
			this.variableName = variableName ?? throw new ArgumentNullException (nameof (variableName));

			if (variableValue is bool b)
				this.variableValue = b.ToString ().ToLowerInvariant ();
			else
				this.variableValue = variableValue?.ToString ();
		}

		public override string AsCommandLineArgument () => Escape ($"-setenv={variableName}={variableValue}");
	}

	/// <summary>
	/// Redirect the standard output for the simulated application to the specified file.
	/// </summary>
	public sealed class SetStdoutArgument : SingleValueArgument {
		public SetStdoutArgument (string targetFile) : base ("stdout", targetFile)
		{
		}
	}

	/// <summary>
	/// Redirect the standard error for the simulated application to the specified file.
	/// </summary>
	public sealed class SetStderrArgument : SingleValueArgument {
		public SetStderrArgument (string targetFile) : base ("stderr", targetFile)
		{
		}
	}

	/// <summary>
	/// Launch an app that is installed on device, specified by bundle identifier.
	/// </summary>
	public sealed class LaunchDeviceArgument : SingleValueArgument {
		public LaunchDeviceArgument (string launchAppPath) : base ("launchdev", launchAppPath, false)
		{
		}
	}

	/// <summary>
	/// Launch the specified MonoTouch.app in the simulator.
	/// </summary>
	public sealed class LaunchSimulatorArgument : SingleValueArgument {
		public LaunchSimulatorArgument (string launchAppPath) : base ("launchsim", launchAppPath, false)
		{
		}
	}

	/// <summary>
	/// Specify which simulator to launch.
	/// </summary>
	public sealed class SimulatorUDIDArgument : MlaunchArgument {
		readonly string udid;

		public SimulatorUDIDArgument (string udid)
		{
			this.udid = udid ?? throw new ArgumentNullException (nameof (udid));
		}

		public override string AsCommandLineArgument () => $"--device=:v2:udid={udid}";
	}

	/// <summary>
	/// Launch an app that is installed on device, specified by bundle identifier.
	/// </summary>
	public sealed class LaunchSimulatorExtensionArgument : MlaunchArgument {
		readonly string launchAppPath;
		readonly string bundleId;

		public LaunchSimulatorExtensionArgument (string launchAppPath, string bundleId)
		{
			this.launchAppPath = launchAppPath ?? throw new ArgumentNullException (nameof (launchAppPath));
			this.bundleId = bundleId ?? throw new ArgumentNullException (nameof (bundleId));
		}

		public override string AsCommandLineArgument () => "--launchsimbundleid " +
			"todayviewforextensions:" + Escape (bundleId) + " " +
			"--observe-extension " + Escape (launchAppPath);
	}

	/// <summary>
	/// Launch the specified bundle id in the simulator (which must already be installed).
	/// </summary>
	public sealed class LaunchDeviceExtensionArgument : MlaunchArgument {
		readonly string launchAppPath;
		readonly string bundleId;

		public LaunchDeviceExtensionArgument (string launchAppPath, string bundleId)
		{
			this.launchAppPath = launchAppPath ?? throw new ArgumentNullException (nameof (launchAppPath));
			this.bundleId = bundleId ?? throw new ArgumentNullException (nameof (bundleId));
		}

		public override string AsCommandLineArgument () => "--launchdevbundleid " +
			"todayviewforextensions:" + Escape (bundleId) + " " +
			"--observe-extension " + Escape (launchAppPath);
	}

	/// <summary>
	/// Set the verbosity level. Can be used repeatedly to lower the level.
	/// </summary>
	public sealed class VerbosityArgument : MlaunchArgument {
		public VerbosityArgument ()
		{
		}

		public override string AsCommandLineArgument () => "-v";
	}

	public sealed class TcpTunnelArgument : MlaunchArgument {
		readonly int port;

		public TcpTunnelArgument (int port)
		{
			if (port <= 0)
				throw new ArgumentOutOfRangeException (nameof (port));
			this.port = port;
		}

		public override string AsCommandLineArgument () => $"--tcp-tunnel={port}:{port}";
	}
}
