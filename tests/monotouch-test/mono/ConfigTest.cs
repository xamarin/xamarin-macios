using System;
#if NET
using System.Configuration;
#endif
using System.IO;
using System.Reflection;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class ConfigTest {
		[Test]
		public void Existence ()
		{
#if NET
			Assert.AreEqual ("Xamarin Rocks", ConfigurationManager.AppSettings ["secretMessage"]);
#else
			var config_file = Assembly.GetExecutingAssembly ().Location + ".config";
			Assert.True (File.Exists (config_file), "existence");
			Assert.That (File.ReadAllText (config_file), Contains.Substring ("<add key=\"secretMessage\" value=\"Xamarin Rocks\"/"), "content");
#endif
		}
	}
}
