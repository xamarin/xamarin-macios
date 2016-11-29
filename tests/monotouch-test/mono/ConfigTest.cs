using System;
using System.IO;
using System.Reflection;

#if XAMCORE_2_0
using Foundation;
#else
using MonoTouch;
using MonoTouch.Foundation;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class ConfigTest
	{
		[Test]
		public void Existence ()
		{
			var config_file = Assembly.GetExecutingAssembly ().Location + ".config";
			Assert.True (File.Exists (config_file), "existence");
			Assert.That (File.ReadAllText (config_file), Contains.Substring ("<secretMessage>Xamarin rocks</secretMessage>"), "content");
		}
	}
}

