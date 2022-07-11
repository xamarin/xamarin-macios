using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Xamarin;
using Xamarin.Tests;
using Xamarin.Utils;

namespace GeneratorTests
{
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class BGen
	{
		[Test]
		public void ResponseFile ()
		{
			var bgen = new BGenTool ();
			bgen.CreateTemporaryBinding ("");
			bgen.ResponseFile = Path.Combine (Cache.CreateTemporaryDirectory (), "rspfile");

			var arguments = new List<string> ();
#if NET
			var targetFramework = TargetFramework.DotNet_iOS_String;
			var tf = TargetFramework.Parse (targetFramework);
			arguments.Add ($"--baselib={Configuration.GetBaseLibrary (tf)}");
			arguments.Add ($"--attributelib={Configuration.GetBindingAttributePath (tf)}");
			arguments.AddRange (Directory.GetFiles (Configuration.DotNetBclDir, "*.dll").Select (v => $"-r:{v}"));
#else
			var targetFramework = "Xamarin.iOS,v1.0";
#endif
			arguments.Add ($"--target-framework={targetFramework}");

			File.WriteAllLines (bgen.ResponseFile, arguments.ToArray ());
			bgen.AssertExecute ("response file");
			bgen.AssertNoWarnings ();
		}
	}
}
