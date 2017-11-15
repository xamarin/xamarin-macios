using System;
using System.IO;
using System.Linq;

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

		[Test]
		public void Bug31788 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.macClassic;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bug31788.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();

			bgen.AssertApiCallsMethod ("Test", "MarshalInProperty", "get_Shared", "xamarin_IntPtr_objc_msgSend", "MarshalInProperty.Shared getter");
			bgen.AssertApiCallsMethod ("Test", "MarshalOnProperty", "get_Shared", "xamarin_IntPtr_objc_msgSend", "MarshalOnProperty.Shared getter");
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
