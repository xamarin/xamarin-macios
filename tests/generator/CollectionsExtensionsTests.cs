using System.Linq;

using NUnit.Framework;

namespace GeneratorTests {

	[TestFixture]
	public class CollectionsExtensionsTests {

		[Test]
		public void Yield ()
			=> Assert.AreEqual (1, "test".Yield ().Count ());

		[Test]
		public void DropLast ()
		{
			var array = new [] { "first", "second", "last" };
			var result = array.DropLast ();
			Assert.AreEqual (array.Length - 1, result.Length, "Result Length");
			Assert.False (result.Contains (array.Last ()), "Contains last item");
		}

	}
}
