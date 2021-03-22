using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.iOS.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class SystemMemoryReferenceTests : ProjectTest {
		
		public SystemMemoryReferenceTests (string platform) : base (platform)      
		{
		}

		[Test]
		public void BasicTest ()
		{
			this.BuildProject ("SystemMemoryReference", clean: false);

			Assert.IsFalse (File.Exists (Path.Combine (AppBundlePath, "System.Memory.dll")), "System.Memory.dll was incorrectly copied from NuGet");
		}
	}
}

