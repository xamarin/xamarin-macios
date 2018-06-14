//
// Unit tests for NLLanguageRecognizer.
//
// Authors:
//	Manuel de la Peña <mandel@microsoft.com>
//
// Copyright 2018 Microsoft Corp. All rights reserved.
//

#if !__WATCHOS__ 

using System;
using System.Collections.Generic;
#if XAMCORE_2_0
using Foundation;
using NaturalLanguage;
#else
using MonoTouch.Foundation;
using MonoTouch.NaturalLanguage;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.NaturalLanguage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NLLanguageRecognizerTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void GetDominantLanguageTest ()
		{
			var text = "Die Kleinen haben friedlich zusammen gespielt.";
			var lang = NLLanguageRecognizer.GetDominantLanguage (text);
			Assert.AreEqual (NLLanguage.German, lang);
		}

		[Test]
		public void GetLanguageHypothesesTest ()
		{
			using (var recognizer = new NLLanguageRecognizer ()) {
				var languages = new Dictionary<NLLanguage, nuint> () {
					{ NLLanguage.German, 1 },
					{ NLLanguage.Spanish, 10 },
				};
				recognizer.LanguageHints = languages;

				var text = "Die Kleinen haben friedlich zusammen gespielt.";
				recognizer.Process (text);
				// just test that we do return something. We are not testing the API perse.
				var hypo = recognizer.GetLanguageHypotheses (5);
				Assert.AreNotEqual (0, hypo.Keys.Count);
			}
		}

		[Test]
		public void LanguageHintsTest ()
		{
			var languages = new Dictionary<NLLanguage, nuint> () {
				{ NLLanguage.German, 1 },
				{ NLLanguage.Spanish, 10 },
			};
			using (var recognizer = new NLLanguageRecognizer ()) {
				// testing setter
				recognizer.LanguageHints = languages;
				// testing getter
				Assert.AreEqual (languages.Keys.Count, recognizer.LanguageHints.Keys.Count, "Size");
			}
		}
	}
}

#endif // !__WATCHOS__
