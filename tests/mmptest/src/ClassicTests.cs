using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

using Xamarin.Tests;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public class ClassicTests
	{
		bool ShouldSkipClassicTest
		{
			get
			{
				return TI.FindMonoVersion () < new Version ("4.3") || !PlatformHelpers.CheckSystemVersion (10, 10); // vstool build requires us to be on a version that XS supports
			}
		}

		[Test]
		public void Classic_SmokeTest ()
		{
			Configuration.AssertXcodeSupports32Bit ();

			if (ShouldSkipClassicTest)
				return;

			MMPTests.RunMMPTest (tmpDir => {
				TI.TestClassicExecutable (tmpDir);
			});
		}

		[Test]
		public void Classic_IntPtr_BindingTest () // Desk 88943, bug 22729, bug 22714
		{
			Configuration.AssertXcodeSupports32Bit ();

			if (ShouldSkipClassicTest)
				return;

			const string IntPtrTestCase = @"NSDictionary d = new NSDictionary ();
				NSObject o = d;
				NSObject v = o.ValueForKey ((NSString)""count"");";
			MMPTests.RunMMPTest (tmpDir => {
				TI.TestClassicExecutable (tmpDir, IntPtrTestCase);
			});
		}

		[Test]
		public void Classic_NewRefCount_Warns ()
		{
			Configuration.AssertXcodeSupports32Bit ();

			if (ShouldSkipClassicTest)
				return;

			MMPTests.RunMMPTest (tmpDir => {
				string buildOutput = TI.TestClassicExecutable (tmpDir, csprojConfig : "<IncludeMonoRuntime>true</IncludeMonoRuntime><MonoBundlingExtraArgs>--new-refcount=false</MonoBundlingExtraArgs>").BuildOutput;
				Assert.IsTrue (buildOutput.Contains ("Disabling the new refcount logic is deprecated"), "Classic_NewRefCount_Warns did not warn as expected:\n\n", buildOutput);
			});
		}
	}
}
