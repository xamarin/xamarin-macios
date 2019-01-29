#if MONOMAC
using System;
#if XAMCORE_2_0
using Foundation;
using MediaToolbox;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaToolbox;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MediaToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ProfessionalVideoWorkflowTest {

		[Test]
		public void RegisterFormatReaders ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 10, throwIfOtherPlatform: false);
			MTProfessionalVideoWorkflow.RegisterFormatReaders ();
		}
	}
}

#endif
