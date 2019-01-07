//
// Unit tests for NLTagger
//
// Copyright 2018 Microsoft Corp. All rights reserved.
//

using System;
using Foundation;
using NaturalLanguage;
using NUnit.Framework;

namespace MonoTouchFixtures.NaturalLanguage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NLTaggerTest {

		const string Text = "The ripe taste of cheese improves with age, but it can not get younger nor shouldn't it.";

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void GetTag ()
		{
			using (var tagger = new NLTagger (NLTagScheme.Lemma) { String = Text })
			using (var tag = tagger.GetTag (0, NLTokenUnit.Word, NLTagScheme.Lemma, out var range)) {
				Assert.That (tagger.DominantLanguage, Is.EqualTo (NLLanguage.English), "DominantLanguage");
				Assert.That (range.Location, Is.EqualTo (0), "Location");
				Assert.That (range.Length, Is.EqualTo (3), "Length");
				// rdar https://trello.com/c/3oN5kuYk
				if (tag != null)
					Assert.That (tag.ToString (), Is.EqualTo ("the"), "First word");
			}
		}

		[Test]
		public void GetTags ()
		{
			using (var tagger = new NLTagger (NLTagScheme.LexicalClass, NLTagScheme.Lemma) { String = Text }) {
				Assert.That (tagger.DominantLanguage, Is.EqualTo (NLLanguage.English), "DominantLanguage");
				var tags = tagger.GetTags (new NSRange (0, Text.Length), NLTokenUnit.Word, NLTagScheme.Lemma, NLTaggerOptions.OmitWhitespace | NLTaggerOptions.OmitPunctuation, out var ranges);
				Assert.That (tags.Length, Is.EqualTo (ranges.Length), "Length");
				foreach (var tag in tags)
					Assert.NotNull (tag, tag);
			}
		}
       }
}
