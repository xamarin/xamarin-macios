using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.MMP.Tests {
	[TestFixture ("Debug")]
	[TestFixture ("Release")]
	public partial class DotNetTests {
		public string Configuration { get; private set; }

		public DotNetTests (string configuration)
		{
			this.Configuration = configuration;
		}

		[TestCase ("MyCocoaApp")]
		[TestCase ("MySpriteKitGame")]
		[TestCase ("MySceneKitGame")]
		[TestCase ("MyMetalGame")]
		public void CompareBuilds (string project)
		{
			var project_dir = Path.Combine (Xamarin.Tests.Configuration.RootPath, "tests", "msbuild-mac", "projects");
			var net461 = Xamarin.Tests.Configuration.CloneTestDirectory (project_dir, "net461");
			var dotnet = Xamarin.Tests.Configuration.CloneTestDirectory (project_dir, "dotnet");
			Xamarin.Tests.Configuration.FixupTestFiles (dotnet, "dotnet5");

			var properties = new Dictionary<string, string> () {
				{"Configuration", Configuration },
			};

			Console.WriteLine ("Building old-style");
			var old_proj = Path.Combine (net461, project, project + ".csproj");
			TI.BuildProject (old_proj, false, Configuration == "Release");
			Console.WriteLine ("Done building old-style");
			var net461_bundle = Path.Combine (Path.GetDirectoryName (old_proj), "bin", Configuration, project + ".app");

			Console.WriteLine ("Building dotnet");
			var new_proj = Path.Combine (dotnet, project, project + ".csproj");
			Assert.AreEqual (0, DotNet.Execute ("build", new_proj, properties, out var _), "dotnet build return value");
			Console.WriteLine ("Done building dotnet");
			var dotnet_bundle = Path.Combine (Path.GetDirectoryName (new_proj), "bin", "AnyCPU", Configuration, "xamarinmac10", project + ".app");

			DotNet.CompareApps (net461_bundle, dotnet_bundle);
		}
	}
}
