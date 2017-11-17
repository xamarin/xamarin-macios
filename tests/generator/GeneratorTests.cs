using System;
using System.IO;

using NUnit.Framework;

using Xamarin;
using Xamarin.Tests;

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
			File.WriteAllLines (bgen.ResponseFile, new string [] { "--target-framework:Xamarin.iOS,v1.0" });
			bgen.AssertExecute ("response file");
			bgen.AssertNoWarnings ();
		}
	}
}
