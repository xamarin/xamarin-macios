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
			string output = string.Join ("\n", Engine.Logger.MessageEvents.Select (x => x.Message));
			Assert.True (Engine.Logger.MessageEvents.Any (x => x.Message.Contains ("using mode 'SDKOnly'")));
		}
	}
}

