// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using Foundation;
using NUnit.Framework;

namespace LinkSdk {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class LocaleTest {

		[Test]
		[SetCulture ("cs-CZ")]
#if NET && __MACCATALYST__
		[Ignore ("No globalization data yet for Mac Catalyst - https://github.com/xamarin/xamarin-macios/issues/11392")]
#endif
		public void CzechDictComparer ()
		{
			DictComparer ();
		}

		[Test]
		[SetCulture ("en-US")]
#if NET && __MACCATALYST__
		[Ignore ("No globalization data yet for Mac Catalyst - https://github.com/xamarin/xamarin-macios/issues/11392")]
#endif
		public void EnglishDictComparer ()
		{
			DictComparer ();
		}

		[Test]
		// runs with whatever the device / simulator is configured
		public void DefaultDictComparer ()
		{
			DictComparer ();
		}

		void DictComparer ()
		{
			var n1 = "SEARCHFIELDS";
			var n2 = "Searchfields";

			Assert.True (string.Equals (n1, n2, StringComparison.OrdinalIgnoreCase), "string equality");

			var dict = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase);
			dict [n1] = "test";

			string result;
			Assert.True (dict.TryGetValue (n2, out result), "dictionary value");
		}
	}
}
