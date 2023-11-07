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
#if NATIVEAOT
#if __MACCATALYST__ || __MACOS__
			var config_dir = Path.Combine (Path.GetDirectoryName (AppContext.BaseDirectory.TrimEnd ('/')), "MonoBundle");
#else
			var config_dir = AppContext.BaseDirectory;
#endif
			var config_file = Path.Combine (config_dir, Assembly.GetExecutingAssembly ().GetName ().Name + ".dll.config");
#else
			var config_file = Assembly.GetExecutingAssembly ().Location + ".config";
#endif
			Assert.That (config_file, Does.Exist, "existence");
			Assert.That (File.ReadAllText (config_file), Contains.Substring ("<secretMessage>Xamarin rocks</secretMessage>"), "content");
		}
	}
}
