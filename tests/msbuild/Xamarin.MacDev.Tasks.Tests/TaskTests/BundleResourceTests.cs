using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.MacDev;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class BundleResourceTest {
		ITaskItem CreateItem (string path, string? localMSBuildProjectFullPath = null, string? localDefiningProjectFullPath = null, bool? isDefaultItem = null)
		{
			var rv = new TaskItem (path);
			if (localMSBuildProjectFullPath is not null)
				rv.SetMetadata ("LocalMSBuildProjectFullPath", localMSBuildProjectFullPath);
			if (localDefiningProjectFullPath is not null)
				rv.SetMetadata ("LocalDefiningProjectFullPath", localDefiningProjectFullPath);
			if (isDefaultItem is not null)
				rv.SetMetadata ("IsDefaultItem", isDefaultItem.Value ? "true" : "false");
			return rv;
		}

		[Test]
		public void GetVirtualProjectPathTest ()
		{
			Assert.Multiple (() => {
				Assert.AreEqual ("Archer_Attack.atlas/archer_attack_0001.png",
					BundleResource.GetVirtualProjectPath (
						null!,
						"/Users/rolf/work/maccore/windows/xamarin-macios/tests/dotnet/LibraryWithResources/iOS",
						CreateItem (
							"../Archer_Attack.atlas/archer_attack_0001.png",
							localMSBuildProjectFullPath: "/Users/rolf/work/maccore/windows/xamarin-macios/tests/dotnet/LibraryWithResources/shared.csproj",
							localDefiningProjectFullPath: "/Users/rolf/work/maccore/windows/xamarin-macios/tests/dotnet/LibraryWithResources/shared.csproj"
						),
						false),
					"A");

				Assert.AreEqual ("Archer_Attack.atlas/archer_attack_0001.png",
					BundleResource.GetVirtualProjectPath (
						null!,
						"C:/src/xamarin-macios/tests/dotnet/LibraryWithResources/iOS",
						CreateItem (
							"../Archer_Attack.atlas/archer_attack_0001.png",
							localMSBuildProjectFullPath: @"C:\src\xamarin-macios\tests\dotnet\LibraryWithResources\shared.csproj",
							localDefiningProjectFullPath: @"C:\src\xamarin-macios\tests\dotnet\LibraryWithResources\shared.csproj"
						),
						true),
					"B");
			});
		}
	}
}
