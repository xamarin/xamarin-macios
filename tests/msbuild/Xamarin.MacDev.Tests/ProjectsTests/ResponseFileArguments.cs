using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public class ResponseFileArguments : ProjectTest {
		public ResponseFileArguments () : base ("iPhoneSimulator")
		{
		}

		[Test]
		public void ProjectWithExtraArgment_CorrectlyOverridesLinkingParam ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			BuildProject ("AppWithExtraArgumentThatOverrides");
			Assert.True (Engine.Logger.MessageEvents.Any (x => x.Message.Contains ("using mode 'SDKOnly'")));
		}
	}
}
