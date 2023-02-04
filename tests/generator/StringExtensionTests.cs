using System.Collections;
using NUnit.Framework;

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
			=> Assert.IsNull (illegal.GetSafeParamName (), "paramName == null");

		[TestCase ("1param")]
		public void StartsWithFixableIllegalChar (string illegal)
		{
			var legal = illegal.GetSafeParamName ();
			Assert.IsNotNull (legal, "legal != null");
			Assert.AreEqual ("@" + illegal, legal, "legal");
		}
	}
}
