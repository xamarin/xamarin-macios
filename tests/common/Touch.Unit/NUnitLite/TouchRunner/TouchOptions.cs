// TouchOptions.cs: MonoTouch.Dialog-based options
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;

using Foundation;
#if !__MACOS__
using UIKit;
#endif

#if !__WATCHOS__ && !__MACOS__
using MonoTouch.Dialog;
#endif

using Mono.Options;

namespace MonoTouch.NUnit.UI {
	
	public enum XmlMode {
		Default = 0,
		Wrapped = 1,
	}

	public enum XmlVersion {
		NUnitV2 = 0,
		NUnitV3 = 1,
	}

#if !NET
	[CLSCompliant (false)]
#endif
	public class TouchOptions {

		static TouchOptions current;
		static public TouchOptions Current {
			get {
				if (current == null)
					current = new TouchOptions ();
				return current;
			}
			set {
				current = value;
			}
		}
		
		public TouchOptions (IList<string> arguments)
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			TerminateAfterExecution = defaults.BoolForKey ("execution.autoexit");
			AutoStart = defaults.BoolForKey ("execution.autostart");
			EnableNetwork = defaults.BoolForKey ("network.enabled");
			EnableXml = defaults.BoolForKey ("xml.enabled");
			HostName = defaults.StringForKey ("network.host.name");
			HostPort = (int)defaults.IntForKey ("network.host.port");
			UseTcpTunnel = defaults.BoolForKey ("execution.usetcptunnel");
			Transport = defaults.StringForKey ("network.transport");
			SortNames = defaults.BoolForKey ("display.sort");
			LogFile = defaults.StringForKey ("log.file");
			TestName = defaults.StringForKey ("test.name");
			
			bool b;
			if (bool.TryParse (Environment.GetEnvironmentVariable ("NUNIT_AUTOEXIT"), out b))
				TerminateAfterExecution = b;
			if (bool.TryParse (Environment.GetEnvironmentVariable ("NUNIT_AUTOSTART"), out b))
				AutoStart = b;
			if (bool.TryParse (Environment.GetEnvironmentVariable ("NUNIT_ENABLE_NETWORK"), out b))
				EnableNetwork = b;
			if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("NUNIT_HOSTNAME")))
				HostName = Environment.GetEnvironmentVariable ("NUNIT_HOSTNAME");
			if (bool.TryParse (Environment.GetEnvironmentVariable ("USE_TCP_TUNNEL"), out b))
				UseTcpTunnel = b;
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
				XmlVersion = (XmlVersion)Enum.Parse (typeof (XmlVersion), xml_version, true);
			if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("NUNIT_LOG_FILE")))
				LogFile = Environment.GetEnvironmentVariable ("NUNIT_LOG_FILE");
			TestName = Environment.GetEnvironmentVariable ("NUNIT_TEST_NAME");

			var os = new OptionSet () {
				{ "autoexit", "If the app should exit once the test run has completed.", v => TerminateAfterExecution = true },
				{ "autostart", "If the app should automatically start running the tests.", v => AutoStart = true },
				{ "hostname=", "Comma-separated list of host names or IP address to (try to) connect to", v => HostName = v },
				{ "hostport=", "HTTP/TCP port to connect to.", v => HostPort = int.Parse (v) },
				{ "use-tcp-tunnel", "Use a TCP tunnel to connect to the host.", v => UseTcpTunnel = true },
				{ "enablenetwork", "Enable the network reporter.", v => EnableNetwork = true },
				{ "transport=", "Select transport method. Either TCP (default), HTTP or FILE.", v => Transport = v },
				{ "enablexml:", "Enable the xml reported.", v => EnableXml = string.IsNullOrEmpty (v) ? true : bool.Parse (v) },
				{ "xmlmode=", "The xml mode.", v => XmlMode = (XmlMode) Enum.Parse (typeof (XmlMode), v, true) },
				{ "xmlversion=", "The xml version.", v => XmlVersion = (XmlVersion) Enum.Parse (typeof (XmlVersion), v, true) },
				{ "logfile=", "A path where output will be saved.", v => LogFile = v },
				{ "test=", "A test to run. Only applicable if autostart is true.", v => TestName = v },
			};
			
			try {
				os.Parse (arguments);
			} catch (OptionException oe) {
				Console.WriteLine ("{0} for options '{1}'", oe.Message, oe.OptionName);
			}
		}

		public TouchOptions ()
			: this (Environment.GetCommandLineArgs ())
		{
		}
		
		private bool EnableNetwork { get; set; }

		public XmlMode XmlMode { get; set; }

		public XmlVersion XmlVersion { get; set; } = XmlVersion.NUnitV2;

		public bool EnableXml { get; set; }
		
		public string HostName { get; private set; }
		
		public int HostPort { get; private set; }

		public bool UseTcpTunnel { get; set; } = true;
		
		public bool AutoStart { get; set; }
		
		public bool TerminateAfterExecution { get; set; }
		
		public string Transport { get; set; } = "TCP";

		public string LogFile { get; set; }

		public bool ShowUseNetworkLogger {
			get { return (EnableNetwork && !String.IsNullOrWhiteSpace (HostName) && (HostPort > 0)) || Transport == "FILE"; }
		}

		public bool SortNames { get; set; }
		
		public string TestName { get; set; }

#if !__WATCHOS__ && !__MACOS__
		public UIViewController GetViewController ()
		{
#if TVOS
			var network = new StringElement (string.Format ("Enabled: {0}", EnableNetwork));
#else
			var network = new BooleanElement ("Enable", EnableNetwork);
#endif
			var host = new EntryElement ("Host Name", "name", HostName);
			host.KeyboardType = UIKeyboardType.ASCIICapable;
			
			var port = new EntryElement ("Port", "name", HostPort.ToString ());
			port.KeyboardType = UIKeyboardType.NumberPad;

			var useTunnel = new BooleanElement ("Use TCP Tunnel", UseTcpTunnel);
			
#if TVOS
			var sort = new StringElement (string.Format ("Sort Names: ", SortNames));
#else
			var sort = new BooleanElement ("Sort Names", SortNames);
#endif

			var root = new RootElement ("Options") {
				new Section ("Remote Server") { network, host, port, useTunnel },
				new Section ("Display") { sort }
			};
				
			var dv = new DialogViewController (root, true) { Autorotate = true };
			dv.ViewDisappearing += delegate {
#if !TVOS
				EnableNetwork = network.Value;
#endif
				HostName = host.Value;
				ushort p;
				if (UInt16.TryParse (port.Value, out p))
					HostPort = p;
				else
					HostPort = -1;
				UseTcpTunnel = useTunnel.Value;
#if !TVOS
				SortNames = sort.Value;
#endif
				
				var defaults = NSUserDefaults.StandardUserDefaults;
				defaults.SetBool (EnableNetwork, "network.enabled");
				defaults.SetString (HostName ?? String.Empty, "network.host.name");
				defaults.SetInt (HostPort, "network.host.port");
				defaults.SetBool (SortNames, "display.sort");
				defaults.SetBool (UseTcpTunnel, "execution.usetcptunnel");
			};
			
			return dv;
		}
#endif // !__WATCHOS__
	}
}
