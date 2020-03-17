namespace Xharness.Execution.Mlaunch {

	public sealed class SdkRootArgument : SingleValueArgument {
		public SdkRootArgument (string sdkPath) : base ("sdkroot", sdkPath)
		{
		}
	}

	public sealed class ListDevicesArgument : SingleValueArgument {
		public ListDevicesArgument (string outputFile) : base ("listdev", outputFile)
		{
		}
	}

	public sealed class ListSimulatorsArgument : SingleValueArgument {
		public ListSimulatorsArgument (string outputFile) : base ("listsim", outputFile)
		{
		}
	}

	public sealed class ListCrashReportsArgument : SingleValueArgument {
		public ListCrashReportsArgument (string outputFile) : base ("list-crash-reports", outputFile)
		{
		}
	}

	public sealed class DeviceNameArgument : SingleValueArgument {
		public DeviceNameArgument (string deviceName) : base ("devname", deviceName)
		{
		}
	}

	public sealed class DefaultOutputFormatArgument : SingleValueArgument {
		public DefaultOutputFormatArgument () : base ("output-format", "Default")
		{
		}
	}

	public sealed class XmlOutputFormatArgument : SingleValueArgument {
		public XmlOutputFormatArgument () : base ("output-format", "XML")
		{
		}
	}

	public sealed class ListExtraDataArgument : OptionArgument {
		public ListExtraDataArgument () : base ("list-extra-data")
		{
		}
	}
}
