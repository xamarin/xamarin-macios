using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class SystemMemoryReferenceTests : ProjectTest {

		public SystemMemoryReferenceTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			this.BuildProject ("SystemMemoryReference", clean: false);

			Assert.IsTrue (File.Exists (Path.Combine (AppBundlePath, "SystemMemoryReference")), "App bundle not created properly");
			Assert.IsFalse (File.Exists (Path.Combine (AppBundlePath, "System.Memory.dll")), "System.Memory.dll was incorrectly copied from NuGet");
		}

		[Test]
		public void NetStandard2_0ReferenceFromLibraryAndDirectNuGetReference ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			BuildProject ("SystemMemoryLibrary", clean: false, nuget_restore: true, is_library: true);
			BuildProject ("SystemMemoryFromNetStandard2_0", clean: false, nuget_restore: true);

			var primaryReferenceIndex = Engine.MessageEvents.FindIndex ((v) => v.Message.TrimStart ().StartsWith ("Primary reference \"System.Memory, Version") && v.ProjectFile.Contains ("SystemMemoryFromNetStandard2_0.csproj"));
			Assert.That (primaryReferenceIndex, Is.GreaterThanOrEqualTo (0), "Failure to find primary reference result in build log");
			var resolvedFilePath = Engine.MessageEvents [primaryReferenceIndex + 1];

			Assert.That (resolvedFilePath.Message, Does.Contain ("Resolved file path is "), "ResolvedFilePath 1");
			Assert.That (resolvedFilePath.Message, Does.Contain ("/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Facades/System.Memory.dll"), "ResolvedFilePath 2");
		}
	}
}
