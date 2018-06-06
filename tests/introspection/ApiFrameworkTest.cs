#if XAMCORE_2_0
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using Foundation;
using ObjCRuntime;


public class Application {
	public bool IsSimulatorBuild {
		get {
#if __IOS__
			return Runtime.Arch == Arch.SIMULATOR;
#else
			return true;
#endif
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
			if (@namespace == null)
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
			// Some CF* types that requires CFNetwork which we always link with
			// ref: tools/common/CompilerFlags.cs
			case "CoreServices":
				return true;
#elif __MACOS__
			// always included, ref: tools/common/CompilerFlags.cs
			case "CFNetwork":
				return true;
			// not a framework, largely p/invokes to /usr/lib/libSystem.dylib
			case "Darwin":
				return true;
			// not directly bindings
			case "System.Net.Http":
				return true;
#endif
			default:
				return false;
			}
		}

		Frameworks GetFrameworks ()
		{
#if __IOS__
			return Frameworks.GetiOSFrameworks (app);
#elif __TVOS__
			return Frameworks.TVOSFrameworks;
#elif __WATCHOS__
			return Frameworks.GetwatchOSFrameworks (app);
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
				// Either Skip method or Frameworks.cs needs to be updated
				ReportError ("Unknown framework '{0}'", ns);
			}
			AssertIfErrors ("Unknown frameworks found");
		}
	}
}
#endif