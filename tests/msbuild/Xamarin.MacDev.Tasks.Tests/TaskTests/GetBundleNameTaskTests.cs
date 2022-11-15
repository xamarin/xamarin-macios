using System;
using System.IO;
using NUnit.Framework;

using Xamarin.MacDev.Tasks;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class GetBundleNameTaskTests : TestBase {
		[Test]
		public void GetBundleName_MissingName ()
		{
			var task = CreateTask<GenerateBundleName> ();
			Assert.IsFalse (task.Execute (), "#1");
			Assert.AreEqual (1, Engine.Logger.ErrorEvents.Count, "#2");
		}

		[Test]
		public void GetBundleName ()
		{
			var task = CreateTask<GenerateBundleName> ();
			task.ProjectName = "!@£///Hello_World%£";

			Assert.IsTrue (task.Execute (), "#1");
			Assert.AreEqual ("Hello_World", task.BundleName, "#2");
			Assert.AreEqual (0, Engine.Logger.ErrorEvents.Count, "#3");
		}
	}
}
