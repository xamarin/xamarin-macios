using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using NUnit.Framework;

using Foundation;
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

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
			// no a framework, namespace for the binding attrs for rgen
			case "ObjCBindings":
				return true;
			// pinvokes into OpenGL[ES]
			case "OpenTK":
				return true;
			// n[u]int, nfloat and friends
			case "System":
			case "System.Drawing":
				return true;
#if __MACOS__
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
			// Removed in Xcode 15.3
			case "AssetsLibrary":
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

	}
}
