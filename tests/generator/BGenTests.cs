using System;
using System.IO;
using NUnit.Framework;

using Xamarin.Tests;

namespace GeneratorTests
{
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class BGenTests
	{
		[Test]
		[TestCase (Profile.macClassic)]
		[TestCase (Profile.macFull)]
		[TestCase (Profile.macModern)]
		public void BMac_Smoke (Profile profile)
		{
			BuildFile (profile, "bmac_smoke.cs");
		}

		[Test]
		[TestCase (Profile.macClassic)]
		[TestCase (Profile.macFull)]
		[TestCase (Profile.macModern)]
		public void BMac_With_Hyphen_In_Name (Profile profile)
		{
			BuildFile (profile, "bmac-with-hyphen-in-name.cs");
		}

		[Test]
		[TestCase (Profile.macClassic)]
		[TestCase (Profile.macFull)]
		[TestCase (Profile.macModern)]
		public void PropertyRedefinitionMac (Profile profile)
		{
			BuildFile (profile, "property-redefination-mac.cs");
		}

		[Test]
		[TestCase (Profile.macClassic)]
		[TestCase (Profile.macFull)]
		[TestCase (Profile.macModern)]
		public void NSApplicationPublicEnsureMethods (Profile profile)
		{
			BuildFile (profile, "NSApplicationPublicEnsureMethods.cs");
		}

		[Test]
		[TestCase (Profile.macClassic)]
		[TestCase (Profile.macFull)]
		[TestCase (Profile.macModern)]
		public void ProtocolDuplicateAbstract (Profile profile)
		{
			BuildFile (profile, "protocol-duplicate-abstract.cs");
		}

		BGenTool BuildFile (Profile profile, string filename)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", filename)));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
			return bgen;
		}
	}
}
