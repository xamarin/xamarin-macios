//
// Unit tests for NLLanguageRecognizer.
//
// Authors:
//	Manuel de la Peña <mandel@microsoft.com>
//
// Copyright 2018 Microsoft Corp. All rights reserved.
//

using System;
using System.Collections.Generic;
using Foundation;
using NaturalLanguage;
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
		public void Process ()
		{
			using (var recognizer = new NLLanguageRecognizer ()) {
				var languages = new Dictionary<NLLanguage, double> () {
					{ NLLanguage.German, 1 },
					{ NLLanguage.Spanish, 10 },
				};
				Assert.That (recognizer.LanguageHints.Count, Is.EqualTo (0), "LanguageHints/0");
				recognizer.LanguageHints = languages;
				Assert.That (recognizer.LanguageHints.Count, Is.EqualTo (2), "LanguageHints/2");

				Assert.That (recognizer.DominantLanguage, Is.EqualTo (NLLanguage.Unevaludated), "DominantLanguage/Pre-Process");
				var text = "Die Kleinen haben friedlich zusammen gespielt.";
				recognizer.Process (text);
				Assert.That (recognizer.DominantLanguage, Is.EqualTo (NLLanguage.German), "DominantLanguage/Post-Process");

				// just test that we do return something. We are not testing the API perse.
				var hypo = recognizer.GetLanguageHypotheses (5);
				Assert.That (hypo.Count, Is.GreaterThan (0), "GetLanguageHypotheses");
			}
		}
	}
}
