using System;
using System.Collections.Generic;

namespace Xamarin.MMP.Tests {
	public static class EnumerableExtensions {
		public static IEnumerable<T> FromSingleItem<T> (this T item)
		{
			yield return item;
		}
	}
}
