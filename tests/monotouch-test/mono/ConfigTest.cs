using System;
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
			var config_file = Path.Combine (AppContext.BaseDirectory, Assembly.GetExecutingAssembly ().GetName ().Name + ".dll.config");
			Assert.That (config_file, Does.Exist, "existence");
			Assert.That (File.ReadAllText (config_file), Contains.Substring ("<secretMessage>Xamarin rocks</secretMessage>"), "content");
		}
	}
}
