using System;
using System.IO;

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
#if NET
			File.WriteAllLines (bgen.ResponseFile, new string [] { $"--target-framework:{TargetFramework.DotNet_6_0_iOS_String}" });
#else
			File.WriteAllLines (bgen.ResponseFile, new string [] { "--target-framework:Xamarin.iOS,v1.0" });
#endif
			bgen.AssertExecute ("response file");
			bgen.AssertNoWarnings ();
		}
	}
}
