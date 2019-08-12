//
// Unit tests for NLEmbedding
//
// Copyright 2019 Microsoft Corp. All rights reserved.
//

using System;
using System.IO;
using Foundation;
using NaturalLanguage;
using NUnit.Framework;

namespace MonoTouchFixtures.NaturalLanguage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EmbeddingTest {

#if __IOS__ || __MACOS__
		[Test]
		public void Vector ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			using (var e = NLEmbedding.GetWordEmbedding (NLLanguage.English)) {
				Assert.NotNull (e, "GetWordEmbedding");
				Assert.Null (e.GetVector ("Xamarin"), "GetVector");
				Assert.False (e.TryGetVector ("Xamarin", out var vector), "TryGetVector");
				Assert.Null (vector, "vector");
			}
		}
#endif

		[Test]
		public void Write ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			var temp = Path.Combine (Path.GetTempPath (), "NLEmbedding.Test");
			File.Delete (temp);

			var vd = new NLVectorDictionary ();
			vd ["a"] = new [] { 0.7f, 1.0f };
			var wd = vd.Dictionary;
			Assert.That (wd.Count, Is.EqualTo (1), "Count");

			using (var url = NSUrl.FromFilename (temp)) {
				var strong = NLEmbedding.Write (vd, NLLanguage.French, 1, url, out var error);
				Assert.True (strong, "strong");
				Assert.Null (error, "strong error");

				var weak = NLEmbedding.Write (wd, NLLanguage.French.GetConstant (), 1, url, out error);
				Assert.True (strong, "strong");
				Assert.Null (error, "weak error");
			}
		}
	}
}
