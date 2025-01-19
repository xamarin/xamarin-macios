//
// Resource Bundling Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

using Foundation;
using ObjCRuntime;

namespace BundledResources {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ResourcesTest {

		[Test]
		public void Bundled ()
		{
			// files are extracted (by MonoDevelop) so we can see them in the file system
			// that's true for simulator or devices and whatever the linker settings are
			var dir = NSBundle.MainBundle.ResourcePath;
			Assert.True (File.Exists (Path.Combine (dir, "basn3p08.png")), "file-basn3p08.png");
			Assert.True (File.Exists (Path.Combine (dir, "basn3p08_with_loc.png")), "file-basn3p08_with_loc.png");
			Assert.True (File.Exists (Path.Combine (dir, "xamvideotest.mp4")), "xamvideotest.mp4");

			// resources are removed by the linker or an extra step (e.g. "link sdk" or "don't link") but that
			// extra step is done only on device (to keep the simulator builds as fast as possible)
			var resources = typeof (ResourcesTest).Assembly.GetManifestResourceNames ();
#if __MACOS__ || __MACCATALYST__
			var hasResources = false;
#else
			var hasResources = Runtime.Arch != Arch.DEVICE;
#endif
			if (!hasResources) {
				Assert.That (resources.Length, Is.EqualTo (0), "No resources");
			} else {
				var expectedResources = new string [] {
					"basn3p08.png",
					"basn3p08__with__loc.png",
					"xamvideotest.mp4",
				};
				var oldPrefixed = expectedResources.Select (v => $"__monotouch_content_{v}").ToArray ();
				var newPrefixed = expectedResources.Select (v => $"__monotouch_item_BundleResource_{v}").ToArray ();
				Assert.That (resources, Is.EquivalentTo (oldPrefixed).Or.EquivalentTo (newPrefixed), "Resources");
			}
		}
	}
}
