using System;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;

namespace Xamarin.Tests {
	public delegate void Action ();

	public static class Asserts {
		public static void Throws<T> (Action action, string expectedExceptionMessage, string message = "") where T : Exception
		{
			try {
				action ();
				throw new AssertionException (string.Format ("Expected {0}, but no exception was thrown. {1}.", typeof (T).FullName, message));
			} catch (T ex) {
				Assert.AreEqual (expectedExceptionMessage, ex.Message, message);
			}
		}

		public static void ThrowsPartial<T> (Action action, string [] expectedExceptionMessages, string message = "") where T : Exception
		{
			try {
				action ();
				throw new AssertionException (string.Format ("Expected {0}, but no exception was thrown. {1}.", typeof (T).FullName, message));
			} catch (T ex) {
				string [] actual = ex.Message.Split (new string [] { Environment.NewLine }, StringSplitOptions.None);
				for (int i = 0; i < expectedExceptionMessages.Length; i++)
					StartsWith (expectedExceptionMessages [i], actual [i], i.ToString ());
			}
		}

		public static void ThrowsPattern<T> (Action action, string expectedExceptionPattern, string message = "") where T : Exception
		{
			try {
				action ();
				throw new AssertionException (string.Format ("Expected {0}, but no exception was thrown. {1}.", typeof (T).FullName, message));
			} catch (T ex) {
				IsLike (expectedExceptionPattern, ex.Message, message);
			}
		}

		public static void StartsWith (string expectedStartsWith, string actual, string message)
		{
			if (!actual.StartsWith (expectedStartsWith, StringComparison.Ordinal))
				throw new AssertionException (string.Format ("Expected '{0}' to start with '{1}'. {2}", actual, expectedStartsWith, message));
		}

		public static void IsLike (string pattern, string actual, string message)
		{
			if (!Regex.IsMatch (actual, pattern, RegexOptions.CultureInvariant))
				throw new AssertionException (string.Format ("Expected '{0}' to match pattern '{1}'. {2}", actual, pattern, message));
		}

		public static void Contains (string expectedToContain, string actual, string message)
		{
			if (!actual.Contains (expectedToContain))
				throw new AssertionException (string.Format ("Expected '{0}' to contain '{1}'. {2}", actual, expectedToContain, message));
		}

		public static void DoesNotContain (string expectedToNotContain, string actual, string message)
		{
			if (actual.Contains (expectedToNotContain))
				throw new AssertionException (string.Format ("Did not expect '{0}' to contain '{1}'. {2}", actual, expectedToNotContain, message));
		}
	}
}
