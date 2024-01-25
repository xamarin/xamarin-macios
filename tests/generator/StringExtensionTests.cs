using System;
using System.Collections;

using NUnit.Framework;

#nullable enable

namespace GeneratorTests {
	public class StringExtensionTests {

		public class KeywordsDataSource : IEnumerable {
			static string [] keywords = {
				"abstract","event","new","struct","as","explicit","null","switch","base","extern",
				"this","false","operator","throw","break","finally","out","true",
				"fixed","override","try","case","params","typeof","catch","for",
				"private","foreach","protected","checked","goto","public",
				"unchecked","class","if","readonly","unsafe","const","implicit","ref",
				"continue","in","return","using","virtual","default",
				"interface","sealed","volatile","delegate","internal","do","is",
				"sizeof","while","lock","stackalloc","else","static","enum",
				"namespace",
				"object","bool","byte","float","uint","char","ulong","ushort",
				"decimal","int","sbyte","short","double","long","string","void",
				"partial", "yield", "where"
			};

			public IEnumerator GetEnumerator ()
			{
				foreach (var key in keywords) {
					yield return key;
				}
			}
		}

		[TestCaseSource (typeof (KeywordsDataSource))]
		public void KeywordsTest (string keyword)
			=> Assert.AreNotEqual (keyword, keyword.GetSafeParamName (), "keyword");

		[TestCase ("😀OhOh")]
		[TestCase (" OhOh")]
		[TestCase ("Oh Oh")]
		public void NotFixableIllegalChar (string illegal)
			=> Assert.IsNull (illegal.GetSafeParamName (), "paramName is null");

		[TestCase ("1param")]
		public void StartsWithFixableIllegalChar (string illegal)
		{
			var legal = illegal.GetSafeParamName ();
			Assert.IsNotNull (legal, "legal is not null");
			Assert.AreEqual ("@" + illegal, legal, "legal");
		}

		[Test]
		public void QuoteNullString ()
		{
			string? str = null;
			Assert.AreEqual (string.Empty, str.Quote ());
		}

		[Test]
		public void QuoteEmptyString ()
		{
			string str = String.Empty;
			Assert.AreEqual (@"""""", str.Quote ());
		}

		[TestCase ("No quotes", "@\"No quotes\"")]
		[TestCase ("\"quotes\"", "@\"\"\"quotes\"\"\"")]
		public void QuoteString (string input, string output)
		{
			Assert.AreEqual (output, input.Quote ());
		}

		[Test]
		public void CamelCaseTest ()
		{
			var str = "pascalCaseExample";
			Assert.AreEqual ("PascalCaseExample", str.CamelCase ());
		}

		[Test]
		public void PascalCaseTest ()
		{
			var str = "CamelCaseExample";
			Assert.AreEqual ("camelCaseExample", str.PascalCase ());
		}

		[TestCase ("@thisIsNotCapitalized", "ThisIsNotCapitalized")]
		[TestCase ("thisIsNotCapitalized", "ThisIsNotCapitalized")]
		[TestCase ("t", "T")]
		public void CapitalizeTest (string input, string output)
			=> Assert.AreEqual (output, input.Capitalize ());

		[TestCase ("ArityTest", "ArityTest")]
		[TestCase ("Arity`Test", "Arity")]
		[TestCase ("Arity`", "Arity")]
		[TestCase ("`Arity", "`Arity")]
		[TestCase ("A`rity", "A")]
		[TestCase ("`", "`")]
		[TestCase (null, null)]
		public void RemoveArityTest (string input, string output)
			=> Assert.AreEqual (output, input.RemoveArity ());
	}
}
