// Copyright 2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DictationPhraseTest {
		
		[Test]
		public void Defaults ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (5, 1))
				return;
			
			using (UIDictationPhrase dp = new UIDictationPhrase ()) {
				Assert.Null (dp.AlternativeInterpretations, "AlternativeInterpretations");
				Assert.Null (dp.Text, "Text");
			}
		}
	}
}

#endif // !__WATCHOS__
