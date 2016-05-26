using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.Foundation;
#else
using AppKit;
using Foundation;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class AppleScriptTests
	{		
		[Test]
		public void AppleScript_BasicTest ()
		{
#pragma warning disable 0219
			const string script = @"tell application ""Finder""
end tell";
			NSAppleScript s = new NSAppleScript (script);

			NSDictionary errorInfo;
			bool success = s.CompileAndReturnError(out errorInfo);
			Assert.IsTrue (success);
			Assert.IsNull (errorInfo);
			Assert.IsTrue (s.Compiled);

			NSAppleEventDescriptor descriptor = s.ExecuteAndReturnError (out errorInfo);
			Assert.IsNull (errorInfo);
#pragma warning restore 0219
		}
	}
}