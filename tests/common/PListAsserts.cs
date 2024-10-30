using System;
using System.IO;
using System.Linq;

using Xamarin.MacDev;
using Xamarin.Utils;

using NUnit.Framework;

#nullable enable

namespace Xamarin.Tests {
	public static class PListAsserts {
		// Compare two plists, and assert if there are any differences.
		public static void AreStringsEqual (string expectedXml, string actualXml, string? message = null)
		{
			if (string.Equals (expectedXml, actualXml, StringComparison.Ordinal))
				return;

			var expected = string.IsNullOrEmpty (expectedXml) ? new PDictionary () : PDictionary.FromString (expectedXml)!;
			var actual = string.IsNullOrEmpty (actualXml) ? new PDictionary () : PDictionary.FromString (actualXml)!;
			if (expected is not PDictionary expectedDictionary)
				throw new InvalidOperationException ($"Root element for the expected plist isn't a dictionary");
			if (actual is not PDictionary actualDictionary)
				throw new InvalidOperationException ($"Root element for the actual plist isn't a dictionary");
			AssertPDictionaryEqual (expectedXml, actualXml, expectedDictionary, actualDictionary, "<root>", message);
		}

		// Compare two plists, and assert if there are any differences.
		public static void AreFilesEqual (string expectedXmlPath, string actualXmlPath, string? message = null)
		{
			Assert.That (expectedXmlPath, Does.Exist, message);
			Assert.That (actualXmlPath, Does.Exist, message);
			AreStringsEqual (File.ReadAllText (expectedXmlPath), File.ReadAllText (actualXmlPath), message);
		}

		static void AssertPDictionaryEqual (string expectedXml, string actualXml, PDictionary expected, PDictionary actual, string key, string? message = null)
		{
			var expectedKeys = expected.Select (v => v.Key ?? string.Empty).OrderBy (v => v).ToArray ();
			var actualKeys = actual.Select (v => v.Key ?? string.Empty).OrderBy (v => v).ToArray ();
			if (expectedKeys.Length != actualKeys.Length) {
				Assert.Fail (
					$"Expected {expectedKeys.Length} entries in 'dict' for key '{key}', got {actualKeys.Length} entries: {message}\n" +
					$"\tExpected keys: {string.Join (", ", expectedKeys)}\n" +
					$"\tActual keys: {string.Join (", ", actualKeys)}\n" +
					$"\tExpected xml:\n" +
					$"\t\t{expectedXml.Replace ("\n", "\n\t\t")}\n" +
					$"\tActual xml:\n" +
					$"\t\t{actualXml.Replace ("\n", "\n\t\t")}"
				);
				return;
			}
			for (var i = 0; i < expectedKeys.Length; i++) {
				if (expectedKeys [i] != actualKeys [i]) {
					Assert.Fail (
						$"Expected key '{expectedKeys [i]}' in 'dict' for key '{key}', got key '{actualKeys [i]}': {message}\n" +
						$"\tExpected xml:\n" +
						$"\t\t{expectedXml.Replace ("\n", "\n\t\t")}\n" +
						$"\tActual xml:\n" +
						$"\t\t{actualXml.Replace ("\n", "\n\t\t")}"
					);
					return;
				}
				var expectedValue = expected [expectedKeys [i]]!;
				var actualValue = actual [actualKeys [i]]!;
				AssertPObjectEqual (expectedXml, actualXml, expectedValue, actualValue, expectedKeys [i], message);
			}
		}

		static void AssertPArrayEqual (string expectedXml, string actualXml, PArray expected, PArray actual, string key, string? message = null)
		{
			if (expected.Count != actual.Count) {
				Assert.Fail (
					$"Expected {expected.Count} items in 'array' for key '{key}', got {actual.Count} items: {message}\n" +
					$"\tExpected xml:\n" +
					$"\t\t{expectedXml.Replace ("\n", "\n\t\t")}\n" +
					$"\tActual xml:\n" +
					$"\t\t{actualXml.Replace ("\n", "\n\t\t")}"
				);
				return;
			}
			for (var i = 0; i < expected.Count; i++) {
				AssertPObjectEqual (expectedXml, actualXml, expected [i], actual [i], key, message);
			}
		}

		static void AssertPStringEqual (string expectedXml, string actualXml, PString expected, PString actual, string key, string? message = null)
		{
			if (expected.Value != actual.Value) {
				Assert.Fail (
					$"Expected string of value '{expected.Value}' for key '{key}', got string of value '{actual.Value}': {message}\n" +
					$"\tExpected xml:\n" +
					$"\t\t{expectedXml.Replace ("\n", "\n\t\t")}\n" +
					$"\tActual xml:\n" +
					$"\t\t{actualXml.Replace ("\n", "\n\t\t")}"
				);
			}
		}

		static void AssertPObjectEqual (string expectedXml, string actualXml, PObject expected, PObject actual, string key, string? message = null)
		{
			if (expected.GetType () != actual.GetType ()) {
				Assert.Fail (
					$"Expected item of type '{expected.GetType ().Name}' for key '{key}', got item of type '{actual.GetType ()}': {message}\n" +
					$"\tExpected xml:\n" +
					$"\t\t{expectedXml.Replace ("\n", "\n\t\t")}\n" +
					$"\tActual xml:\n" +
					$"\t\t{actualXml.Replace ("\n", "\n\t\t")}"
				);
				return;
			}

			if (expected is PDictionary expectedDictionary) {
				AssertPDictionaryEqual (expectedXml, actualXml, expectedDictionary, (PDictionary) actual, key, message);
			} else if (expected is PString expectedString) {
				AssertPStringEqual (expectedXml, actualXml, expectedString, (PString) actual, key, message);
			} else if (expected is PArray expectedArray) {
				AssertPArrayEqual (expectedXml, actualXml, expectedArray, (PArray) actual, key, message);
			} else {
				throw new NotImplementedException ($"Comparing PList objects of type {expected.GetType ().Name}");
			}
		}
	}
}
