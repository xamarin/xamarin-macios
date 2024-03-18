using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using NUnit.Framework;

using Foundation;
using ObjCRuntime;


public class Application {
	public bool IsSimulatorBuild {
		get {
			return TestRuntime.IsSimulator;
		}
	}
}

namespace Introspection {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ApiFrameworkTest : ApiBaseTest {

		HashSet<string> namespaces = new HashSet<string> ();
		Application app = new Application ();

		public bool Skip (string @namespace)
		{
			if (@namespace is null)
				return true;
			if (namespaces.Contains (@namespace))
				return true;
			namespaces.Add (@namespace);
			switch (@namespace) {
			// we always link with the CoreFoundation framework
			case "CoreFoundation":
				return true;
			// not a framework but a dynamic library /usr/lib/libcompression.dylib - tracked elsewhere (p/invoke only)
			case "Compression":
				return true;
			// not a framework, largely p/invokes to /usr/lib/libobjc.dylib
			case "ObjCRuntime":
				return true;
			// pinvokes into OpenGL[ES]
			case "OpenTK":
				return true;
			// n[u]int, nfloat and friends
			case "System":
			case "System.Drawing":
				return true;
#if __IOS__
#if !NET
			// Some CF* types that requires CFNetwork which we always link with
			// ref: tools/common/CompilerFlags.cs
			case "CoreServices":
#endif
#if !NET
			case "WatchKit": // Apple removed WatchKit from iOS
#endif
				return true;
#elif __TVOS__ && !NET
			// mistakes (can't be fixed without breaking binary compatibility)
			case "CoreSpotlight":
			case "WebKit":
				return true;
#elif __WATCHOS__ && !NET
			// helpers (largely enums) for AVFoundation API - no p/invokes or obj-C API that requires native linking
			case "AudioToolbox":
				return true;
			// mistakes (can't be fixed without breaking binary compatibility)
			case "WebKit":
				return true;
#elif __MACOS__
			// always included, ref: tools/common/CompilerFlags.cs
			case "CFNetwork":
				return true;
			// not a framework, largely p/invokes to /usr/lib/libSystem.dylib
			case "Darwin":
				return true;
#endif
			// not directly bindings
			case "System.Net.Http":
				return true;
			// Removed in Xcode 15
			case "NewsstandKit":
				return true;
			default:
				return false;
			}
		}

		Frameworks GetFrameworks ()
		{
#if __MACCATALYST__
			return Frameworks.GetMacCatalystFrameworks ();
#elif __IOS__
			return Frameworks.GetiOSFrameworks (app.IsSimulatorBuild);
#elif __TVOS__
			return Frameworks.TVOSFrameworks;
#elif __WATCHOS__
			return Frameworks.GetwatchOSFrameworks (app.IsSimulatorBuild);
#elif __MACOS__
			return Frameworks.MacFrameworks;
#else
			throw new NotImplementedException ();
#endif
		}

		[Test]
		public void NativeFrameworks ()
		{
			ContinueOnFailure = true;
			Errors = 0;
			int n = 0;
			var frameworks = GetFrameworks ();

			foreach (Type t in Assembly.GetTypes ()) {
				if (!t.IsPublic)
					continue;
				var ns = t.Namespace;
				if (Skip (ns))
					continue;
				n++;
				if (LogProgress)
					Console.WriteLine ($"Namespace candidate '{ns}'");
				if (frameworks.TryGetValue (ns, out var f))
					continue;
				// skip System.Net.Http handlers (since we moved them out of the BCL)
				switch (ns) {
				case "System.Net.Http":
					switch (t.Name) {
					case "CFNetworkHandler":
					case "NSUrlSessionHandler":
						continue;
					}
					break;
				case "Errors":
					continue;
				}
				// Either Skip method or Frameworks.cs needs to be updated
				ReportError ("Unknown framework '{0}'", ns);
			}
			AssertIfErrors ($"{Errors} unknown frameworks found:\n{ErrorData}");
		}

#if __IOS__ && !__MACCATALYST__ && !NET
		[Test]
		public void Simlauncher ()
		{
			TestRuntime.AssertSimulator ("Only needed on simulator");

			var all = GetFrameworks ();

			var namespaces = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
			foreach (Type t in Assembly.GetTypes ()) {
				if (!t.IsPublic)
					continue;
				namespaces.Add (t.Namespace);
			}

			foreach (var line in File.ReadAllLines ("simlauncher64-sgen.frameworks")) {
				var c = line.IndexOf (" (compatibility");
				if (c < 0)
					continue;
				var path = line.Substring (1, c - 1);
				if (!path.StartsWith ("/System/Library/Frameworks/", StringComparison.Ordinal))
					continue;
				var fx = Path.GetFileNameWithoutExtension (path);

				// match with mtouch framework list
				if (!all.TryGetValue (fx, out var framework)) {
					// special cases
					switch (fx) {
					case "CoreAudio": // AudioToolbox, AVFoundation...
					case "CoreFoundation": // implied (always linked)
					case "CFNetwork": // implied (StartWWAN) and included (mostly) in CoreServices
					case "OpenAL": // part of OpenTK
						break;
					case "CoreMIDI":
						// CoreMidi (case) in the fx list
						break;
					default:
						ReportError ($"{fx} is not part of mtouch's GetFrameworks");
						break;
					}
				}

				// match with Xamarin.iOS.dll namespaces
				if (!namespaces.Contains (fx)) {
					// special cases
					switch (fx) {
					case "CoreAudio": // AudioToolbox, AVFoundation...
					case "CFNetwork": // implied (StartWWAN) and included (mostly) in CoreServices
					case "OpenAL": // part of OpenTK
						break;
					default:
						ReportError ($"{fx} is not part of mtouch's GetFrameworks");
						break;
					}
				}

			}

			AssertIfErrors ($"{Errors} unknown frameworks found:\n{ErrorData}");
		}
#endif
	}
}
