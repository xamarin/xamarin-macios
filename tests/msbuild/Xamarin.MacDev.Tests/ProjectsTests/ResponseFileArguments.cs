using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	public class ResponseFileArguments : ProjectTest
	{
		public ResponseFileArguments () : base ("iPhoneSimulator")
		{
		}

		[Test]
		public void ProjectWithExtraArgment_CorrectlyOverridesLinkingParam ()
		{
			BuildProject ("AppWithExtraArgumentThatOverrides", Platform, "Debug", clean: true);
			Assert.True (Engine.Logger.MessageEvents.Any (x => x.Message.Contains ("using mode 'SDKOnly'")));
		}
	}
}

