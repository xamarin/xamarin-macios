#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Xharness {
	sealed class TestShard {
		public int Index { get; }
		public int Count { get; }

		TestShard (int index, int count)
		{
			Index = index;
			Count = count;
		}

		public static TestShard Parse (string value)
		{
			ArgumentNullException.ThrowIfNull (value);

			var parts = value.Split ('/');
			if (parts.Length != 2 ||
				!int.TryParse (parts [0], NumberStyles.None, CultureInfo.InvariantCulture, out var index) ||
				!int.TryParse (parts [1], NumberStyles.None, CultureInfo.InvariantCulture, out var count) ||
				index < 1 || count < 1 || index > count)
				throw new ArgumentException ("Expected a test shard in the form INDEX/COUNT, with 1 <= INDEX <= COUNT.", nameof (value));

			return new TestShard (index, count);
		}

		public IEnumerable<T> Select<T> (IEnumerable<T> values, Func<T, string> keySelector, Func<T, bool> isEnabled)
		{
			ArgumentNullException.ThrowIfNull (values);
			ArgumentNullException.ThrowIfNull (keySelector);
			ArgumentNullException.ThrowIfNull (isEnabled);

			// Eligibility changes must not move other variations between shards.
			return values.OrderBy (keySelector, StringComparer.Ordinal)
				.Where ((value, index) => index % Count == Index - 1)
				.Where (isEnabled);
		}
	}
}
