//
// Unit tests for NLGazetteer
//
// Copyright 2019 Microsoft Corp. All rights reserved.
//

using System;

using Foundation;

using NaturalLanguage;

using NUnit.Framework;

namespace MonoTouchFixtures.NaturalLanguage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GazetteerTest {

		[Test]
		public void Dictionary ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			var sd = new NLStrongDictionary ();
			sd ["a"] = new [] { "Allo" };
			sd [new NSString ("b")] = new [] { "Bonjour" };

			var wd = sd.Dictionary;
			Assert.That (wd.Count, Is.EqualTo ((nuint) 2), "Count");

			using (var weak = new NLGazetteer (wd, NLLanguage.French.GetConstant (), out var error)) {
				Assert.Null (error, "weak error");
			}

			using (var strong = new NLGazetteer (sd, NLLanguage.French, out var error)) {
				Assert.Null (error, "strong error");
			}
		}
	}
}
