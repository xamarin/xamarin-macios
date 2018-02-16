using System;
using System.Collections.Generic;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using Security;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.Security;
#endif
using NUnit.Framework;

namespace monotouchtest
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSLinguisticAnalysisTest
	{
		List<NSString>  words;

		[SetUp]
		public void SetUp ()
		{
			words = new List<NSString> ();
		}

		public bool Enumerator (NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop)
		{
			words.Add (tag);
			stop = false;
			return true;
		}

		public bool StopEnumerator (NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop)
		{
			words.Add (tag);
			stop = true;
			return true;
		}

		[Test]
		public void EnumerateSubstringsInRangeTest ()
		{
			var testString = new NSString ("Hello Hola Bonjour!");
			var range = new NSRange (0, testString.Length - 1);
			testString.EnumerateLinguisticTags (range, NSLinguisticTagScheme.Token, NSLinguisticTaggerOptions.OmitWhitespace, null, Enumerator);
			Assert.AreEqual (3, words.Count, "Word count");
#if XAMCORE_4_0
			Assert.True (words.Contains (NSLinguisticTag.Word.GetConstant ()), "Token type.");
#else
			Assert.True (words.Contains (NSLinguisticTagUnit.Word.GetConstant ()), "Token type.");
#endif
		}

		[Test]
		public void StopEnumerateSubstringsInRangeTest ()
		{
			var testString = new NSString ("Hello Hola Bonjour!");
			var range = new NSRange (0, testString.Length - 1);
			testString.EnumerateLinguisticTags (range, NSLinguisticTagScheme.Token, NSLinguisticTaggerOptions.OmitWhitespace, null, StopEnumerator);
			Assert.AreEqual (1, words.Count, "Word count");
#if XAMCORE_4_0
			Assert.True (words.Contains (NSLinguisticTag.Word.GetConstant ()), "Token type.");
#else
			Assert.True (words.Contains (NSLinguisticTagUnit.Word.GetConstant ()), "Token type.");
#endif
		}

		[Test]
		public void GetLinguisticTagsTest ()
		{
			var testString = new NSString ("Hello Hola Bonjour!");
			var range = new NSRange (0, testString.Length - 1); 
			NSValue[] tokenRanges;
			var tags = testString.GetLinguisticTags (range, NSLinguisticTagScheme.NameOrLexicalClass, NSLinguisticTaggerOptions.OmitWhitespace, null, out tokenRanges);
			Assert.AreEqual (3, tags.Length, "Tags Length");
		}
	}
}