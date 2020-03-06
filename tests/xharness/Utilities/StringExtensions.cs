using System;

namespace Xharness.Utilities
{
	static class StringExtensions
	{
		internal static string [] SplitLines (this string s) => s.Split (new [] { Environment.NewLine }, StringSplitOptions.None);

		// Adds an element to an array and returns a new array with the added element.
		// The original array is not modified.
		// If the original array is null, a new array is also created, with just the new value.
		internal static T [] CopyAndAdd<T> (this T [] array, T value)
		{
			if (array == null || array.Length == 0)
				return new T [] { value };
			var tmpArray = array;
			Array.Resize (ref array, array.Length + 1);
			tmpArray [tmpArray.Length - 1] = value;
			return tmpArray;
		}
	}
}
