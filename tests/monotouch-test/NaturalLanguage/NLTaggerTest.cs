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
				Assert.That (range.Location, Is.EqualTo ((nint) 0), "Location");
				Assert.That (range.Length, Is.EqualTo ((nint) 3), "Length");
				// rdar https://trello.com/c/3oN5kuYk
				if (tag is not null)
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

		[Test]
		public void GetModels ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using (var tagger = new NLTagger (NLTagScheme.LexicalClass) { String = Text }) {
#if NET
				foreach (var scheme in typeof (NLTagScheme).GetEnumValues ()) {
#else
				foreach (NLTagScheme scheme in Enum.GetValues (typeof (NLTagScheme))) {
#endif
					var constant = ((NLTagScheme) scheme).GetConstant ();
					if (constant is null)
						continue; // can vary by SDK version
					Assert.That (tagger.GetModels (constant), Is.Empty, constant);
				}
			}
		}

		[Test]
		public void GetTagHypotheses ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			using (var tagger = new NLTagger (NLTagScheme.LexicalClass) { String = Text }) {
				var dict = tagger.GetTagHypotheses (0, NLTokenUnit.Sentence, NLTagScheme.LexicalClass, nuint.MaxValue);
				Assert.That (dict.Count, Is.EqualTo (1), "Count");
				var item = dict [NLLanguage.Unevaluated];
				Assert.That (item, Is.EqualTo (1.0d), "value");
			}
		}

		[Test]
		public void GetTagHypotheses_Range ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			using (var tagger = new NLTagger (NLTagScheme.LexicalClass) { String = Text }) {
				var dict = tagger.GetTagHypotheses (0, NLTokenUnit.Sentence, NLTagScheme.LexicalClass, nuint.MaxValue, out NSRange range);
				Assert.That (dict.Count, Is.EqualTo (1), "Count");
				var item = dict [NLLanguage.Unevaluated];
				Assert.That (item, Is.EqualTo (1.0d), "value");
				Assert.That (range.Location, Is.EqualTo ((nint) 0), "Location");
				Assert.That (range.Length, Is.EqualTo ((nint) 88), "Length");
			}
		}

		[Test]
		public void GetNativeTagHypothesesNullSchemeTest ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			using (var tagger = new NLTagger (NLTagScheme.LexicalClass) { String = Text }) {
				var fakeScheme = (NLTagScheme) int.MaxValue;
				Assert.Throws<ArgumentOutOfRangeException> (() => { _ = tagger.GetTagHypotheses (0, NLTokenUnit.Sentence, fakeScheme, nuint.MaxValue); }, "We should throw an ArgumentOutOfRangeException if GetTagHypotheses (nuint characterIndex, NLTokenUnit unit, NSString scheme, nuint maximumCount, IntPtr tokenRange) has a null value for the 'scheme' parameter.");
				Assert.Throws<ArgumentOutOfRangeException> (() => { _ = tagger.GetTagHypotheses (0, NLTokenUnit.Sentence, fakeScheme, nuint.MaxValue, out NSRange range); }, "We should throw an ArgumentOutOfRangeException if GetTagHypotheses (nuint characterIndex, NLTokenUnit unit, NSString scheme, nuint maximumCount, IntPtr tokenRange) has a null value for the 'scheme' parameter.");
			}
		}
	}
}
