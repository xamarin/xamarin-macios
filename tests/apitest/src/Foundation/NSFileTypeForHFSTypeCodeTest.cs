using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.Foundation;
using MonoMac.AppKit;
#else
using Foundation;
using AppKit;
#endif

namespace apitest
{
	[TestFixture]
	public class NSFileTypeForHFSTypeCodeTest
	{

		[Test]
		public void GetHFSTypeCodeString_ShouldGetSingleQuotesString ()
		{
			var str = NSFileTypeForHFSType.GetHFSTypeCodeString(NSFileTypeForHFSTypeCode.ClipboardIcon);

			Assert.AreEqual (str, "'CLIP'");
		}
	}
}

