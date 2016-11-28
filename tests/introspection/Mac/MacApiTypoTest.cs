using System;
using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using AppKit;
#else
using MonoMac;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.AppKit;
#endif

namespace Introspection
{

	[TestFixture]
	public class MacApiTypoTest : ApiTypoTest
	{
		NSSpellChecker checker = new NSSpellChecker ();

		[SetUp]
		public void SetUp ()
		{
			var sdk = new Version (Constants.SdkVersion);
			if (!PlatformHelper.CheckSystemVersion (sdk.Major, sdk.Minor))
				Assert.Ignore ("Typos only verified using the latest SDK");
		}

		public override string GetTypo (string txt)
		{
			var checkRange = new NSRange (0, txt.Length);
			var typoRange = checker.CheckSpelling (txt, 0);
			if (typoRange.Length == 0)
				return String.Empty;
			return txt.Substring ((int)typoRange.Location, (int)typoRange.Length);
		}
	}
}
