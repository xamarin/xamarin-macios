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
			var base_path = Path.Combine (AppContext.BaseDirectory, Assembly.GetExecutingAssembly ().GetName ().Name);
			var dll_config = base_path + ".dll.config";
			var exe_config = base_path + ".exe.config";
			string config_file;
			if (File.Exists (dll_config)) {
				config_file = dll_config;
			} else if (File.Exists (exe_config)) {
				config_file = exe_config;
			} else {
				Assert.Fail ("No config file found");
				return;
			}
			Assert.That (File.ReadAllText (config_file), Contains.Substring ("<secretMessage>Xamarin rocks</secretMessage>"), "content");
		}
	}
}
