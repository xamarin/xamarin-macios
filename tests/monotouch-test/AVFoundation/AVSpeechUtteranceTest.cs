//
// Unit tests for AVSpeechUtterance

using System;

using AVFoundation;
using Foundation;

using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVSpeechUtteranceTest {
		[Test]
		public void StringCtor ()
		{
			using var utterance = new AVSpeechUtterance ("hello world");
			Assert.AreEqual (utterance.SpeechString, "hello world", "SpeechString");
		}

		[Test]
		public void StringOptionCtor_PlainText ()
		{
			using var utterance = new AVSpeechUtterance ("hello world", AVSpeechUtteranceInitializationOption.PlainText);
			Assert.AreEqual (utterance.SpeechString, "hello world", "SpeechString");
		}

		[Test]
		public void StringOptionCtor_Ssml ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);

			var ssml = $"""<speak>Hello World</speak>""";
			using var utterance = new AVSpeechUtterance (ssml, AVSpeechUtteranceInitializationOption.SsmlRepresentation);
			Assert.AreEqual (utterance.SpeechString, "Hello World", "SpeechString");
		}
	}
}
