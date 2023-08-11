using System;
using System.Text.Json;

using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.System.Text.Json {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class SerializationTest {
		[Test]
		public void Serialize ()
		{
			var txt = JsonSerializer.Serialize (42);
			Console.WriteLine (txt);
			Assert.AreEqual ("?", txt, "serialized blob");
		}
	}
}
