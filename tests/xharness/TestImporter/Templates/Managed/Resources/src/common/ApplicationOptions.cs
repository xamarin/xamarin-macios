// Modified version of the TouchOptions found in 
using System;
using Foundation;
using Mono.Options;
using Xamarin.iOS.UnitTests;

namespace BCLTests {

	public enum XmlMode {
		Default = 0,
		Wrapped = 1,
	}

	public enum XmlVersion {
		NUnitV2 = 0,
		NUnitV3 = 1,
	}

	public class ApplicationOptions {

		static public ApplicationOptions Current = new ApplicationOptions ();

		public ApplicationOptions ()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			TerminateAfterExecution = defaults.BoolForKey ("execution.autoexit");
			AutoStart = defaults.BoolForKey ("execution.autostart");
			EnableNetwork = defaults.BoolForKey ("network.enabled");
			EnableXml = defaults.BoolForKey ("xml.enabled");
			HostName = defaults.StringForKey ("network.host.name");
			HostPort = (int) defaults.IntForKey ("network.host.port");
			UseTcpTunnel = defaults.BoolForKey ("execution.usetcptunnel");
			Transport = defaults.StringForKey ("network.transport");
			SortNames = defaults.BoolForKey ("display.sort");
			LogFile = defaults.StringForKey ("log.file");
			ResultFile = defaults.StringForKey ("result.file");

			bool b;
			if (bool.TryParse (Environment.GetEnvironmentVariable ("NUNIT_AUTOEXIT"), out b))
				TerminateAfterExecution = b;
			if (bool.TryParse (Environment.GetEnvironmentVariable ("NUNIT_AUTOSTART"), out b))
				AutoStart = b;
			if (bool.TryParse (Environment.GetEnvironmentVariable ("NUNIT_ENABLE_NETWORK"), out b))
				EnableNetwork = b;
			if (bool.TryParse (Environment.GetEnvironmentVariable ("USE_TCP_TUNNEL"), out b))
				UseTcpTunnel = b;
			if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("NUNIT_HOSTNAME")))
				HostName = Environment.GetEnvironmentVariable ("NUNIT_HOSTNAME");
			int i;
			if (int.TryParse (Environment.GetEnvironmentVariable ("NUNIT_HOSTPORT"), out i))
				HostPort = i;
			if (bool.TryParse (Environment.GetEnvironmentVariable ("NUNIT_SORTNAMES"), out b))
				SortNames = b;
			if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("NUNIT_TRANSPORT")))
				Transport = Environment.GetEnvironmentVariable ("NUNIT_TRANSPORT");
			if (bool.TryParse (Environment.GetEnvironmentVariable ("NUNIT_ENABLE_XML_OUTPUT"), out b))
				EnableXml = b;
			var xml_mode = Environment.GetEnvironmentVariable ("NUNIT_ENABLE_XML_MODE");
			if (!string.IsNullOrEmpty (xml_mode))
				XmlMode = (XmlMode) Enum.Parse (typeof (XmlMode), xml_mode, true);
			var xml_version = Environment.GetEnvironmentVariable ("NUNIT_XML_VERSION");
			if (!string.IsNullOrEmpty (xml_version))
				XmlVersion = (XmlVersion) Enum.Parse (typeof (XmlVersion), xml_version, true);
			if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("NUNIT_LOG_FILE")))
				LogFile = Environment.GetEnvironmentVariable ("NUNIT_LOG_FILE");

			var os = new OptionSet () {
				{ "autoexit", "If the app should exit once the test run has completed.", v => TerminateAfterExecution = true },
				{ "autostart", "If the app should automatically start running the tests.", v => AutoStart = true },
				{ "hostname=", "Comma-separated list of host names or IP address to (try to) connect to", v => HostName = v },
				{ "hostport=", "HTTP/TCP port to connect to.", v => HostPort = int.Parse (v) },
				{ "use_tcp_tunnel", "Use a tcp tunnel to connect to the host.", v => UseTcpTunnel = true },
				{ "enablenetwork", "Enable the network reporter.", v => EnableNetwork = true },
				{ "transport=", "Select transport method. Either TCP (default), HTTP or FILE.", v => Transport = v },
				{ "enablexml:", "Enable the xml reported.", v => EnableXml = string.IsNullOrEmpty (v) ? true : bool.Parse (v) },
				{ "xmlmode=", "The xml mode.", v => XmlMode = (XmlMode) Enum.Parse (typeof (XmlMode), v, true) },
				{ "xmlversion=", "The xml version.", v => XmlVersion = (XmlVersion) Enum.Parse (typeof (XmlVersion), v, true) },
				{ "logfile=", "A path where output will be saved.", v => LogFile = v },
				{ "result=", "The path to be used to store the result", v => ResultFile = v},
			};

			try {
				os.Parse (Environment.GetCommandLineArgs ());
			} catch (OptionException oe) {
				Console.WriteLine ("{0} for options '{1}'", oe.Message, oe.OptionName);
			}
		}

		private bool EnableNetwork { get; set; }

		public XmlMode XmlMode { get; set; }

		public XmlVersion XmlVersion { get; set; } = XmlVersion.NUnitV2;

		public bool EnableXml { get; set; }

		public string HostName { get; private set; }

		public int HostPort { get; private set; }

		public bool UseTcpTunnel { get; set; } = false;

		public bool AutoStart { get; set; }

		public bool TerminateAfterExecution { get; set; }

		public string Transport { get; set; } = "TCP";

		public string LogFile { get; set; }

		public string ResultFile { get; set; }

		public bool ShowUseNetworkLogger {
			get { return (EnableNetwork && !String.IsNullOrWhiteSpace (HostName) && (HostPort > 0 || Transport == "FILE")); }
		}

		public bool SortNames { get; set; }

	}
}
