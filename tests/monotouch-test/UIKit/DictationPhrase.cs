// Copyright 2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DictationPhraseTest {

		[Test]
		public void Defaults ()
		{
			using (UIDictationPhrase dp = new UIDictationPhrase ()) {
				Assert.Null (dp.AlternativeInterpretations, "AlternativeInterpretations");
				Assert.Null (dp.Text, "Text");
			}
		}
	}
}

#endif // !__WATCHOS__
