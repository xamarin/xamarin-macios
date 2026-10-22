#nullable enable

using Xharness;

namespace Xamarin.Tests {
	[TestFixture]
	public class TestShardTest {
		[TestCase ("1/3", 1, 3)]
		[TestCase ("3/3", 3, 3)]
		[TestCase ("1/1", 1, 1)]
		[TestCase ("2/2", 2, 2)]
		public void Parse (string value, int index, int count)
		{
			var shard = TestShard.Parse (value);
			Assert.That (shard.Index, Is.EqualTo (index));
			Assert.That (shard.Count, Is.EqualTo (count));
		}

		[TestCase ("")]
		[TestCase ("1")]
		[TestCase ("0/3")]
		[TestCase ("4/3")]
		[TestCase ("1/0")]
		[TestCase ("1/-3")]
		[TestCase ("a/3")]
		[TestCase ("1/2/3")]
		[TestCase (" 1/3")]
		[TestCase ("1 /3")]
		[TestCase ("2147483648/3")]
		public void InvalidShard (string value)
		{
			Assert.Throws<ArgumentException> (() => TestShard.Parse (value));
		}

		[TestCase (null)]
		public void NullShard (string value)
		{
			Assert.Throws<ArgumentNullException> (() => TestShard.Parse (value));
		}

		[TestCase (0, 3)]
		[TestCase (1, 3)]
		[TestCase (2, 3)]
		[TestCase (20, 3)]
		[TestCase (21, 3)]
		[TestCase (40, 2)]
		[TestCase (20, 1)]
		public void DisjointAndExhaustive (int variationCount, int shardCount)
		{
			var variations = Enumerable.Range (0, variationCount).Select (v => $"variation-{v:D3}").ToArray ();
			var partitions = Enumerable.Range (1, shardCount).Select (index =>
				TestShard.Parse ($"{index}/{shardCount}").Select (variations, v => v, v => true).ToArray ()).ToArray ();
			var combined = partitions.SelectMany (v => v).ToArray ();

			Assert.That (combined, Is.EquivalentTo (variations));
			Assert.That (combined, Is.Unique);
			Assert.That (partitions.Max (v => v.Length) - partitions.Min (v => v.Length), Is.LessThanOrEqualTo (1));
		}

		[Test]
		public void OrdinalAndIndependentOfEnumerationOrder ()
		{
			var variations = new [] { "Z", "a", "B", "b" };
			var shard = TestShard.Parse ("1/2");
			Assert.That (shard.Select (variations, v => v, v => true), Is.EqualTo (new [] { "B", "a" }));
			Assert.That (shard.Select (variations.Reverse (), v => v, v => true), Is.EqualTo (new [] { "B", "a" }));
		}

		[TestCase (1)]
		[TestCase (2)]
		[TestCase (3)]
		public void EligibilityDoesNotMoveOtherVariations (int index)
		{
			var variations = Enumerable.Range (0, 21).Select (v => $"variation-{v:D3}").ToArray ();
			var shard = TestShard.Parse ($"{index}/3");
			var all = shard.Select (variations, v => v, v => true).ToArray ();
			var enabled = shard.Select (variations, v => v, v => v != "variation-001").ToArray ();

			Assert.That (enabled, Is.EqualTo (all.Where (v => v != "variation-001")));
		}
	}
}
