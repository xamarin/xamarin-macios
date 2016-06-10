using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public partial class MMPTests 
	{
		[Test]
		public void TodayExtension_SmokeTest ()
		{
			if (!PlatformHelpers.CheckSystemVersion (10, 10))
				return;

			RunMMPTest (tmpDir =>
			{
				string testPath = Path.Combine (TI.FindSourceDirectory (), @"Today/TodayExtensionTest.csproj");
				TI.BuildProject (testPath, true, false);
			});
		}

		[Test]
		public void FinderExtension_SmokeTest ()
		{
			if (!PlatformHelpers.CheckSystemVersion (10, 10))
				return;

			RunMMPTest (tmpDir =>
			{
				string testPath = Path.Combine (TI.FindSourceDirectory (), @"Finder/FinderExtensionTest.csproj");
				TI.BuildProject (testPath, true, false);
			});
		}

		[Test]
		public void ShareExtension_SmokeTest ()
		{
			if (!PlatformHelpers.CheckSystemVersion (10, 10))
				return;

			RunMMPTest (tmpDir =>
			{
				string testPath = Path.Combine (TI.FindSourceDirectory (), @"Share/ShareExtensionTest.csproj");
				TI.BuildProject (testPath, true, false);
			});
		}
	}
}
