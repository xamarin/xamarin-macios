#if MONOMAC
using System;
using Foundation;
using MediaToolbox;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MediaToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ProfessionalVideoWorkflowTest {

		[Test]
		public void RegisterFormatReaders ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 10, throwIfOtherPlatform: false);
			MTProfessionalVideoWorkflow.RegisterFormatReaders ();
		}
	}
}

#endif
