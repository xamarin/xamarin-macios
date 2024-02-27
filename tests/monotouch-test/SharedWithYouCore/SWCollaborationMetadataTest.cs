//
// Unit tests for SWCollaborationMetadata
//
// Authors:
//	Israel Soto <issoto@microsoft.com>
//
// Copyright 2022 Microsoft Corporation.
//

#if !__WATCHOS__ && !__TVOS__

using System;
using Foundation;
using ObjCRuntime;
using SharedWithYouCore;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.SharedWithYouCore {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SWCollaborationMetadataTests {

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (14, 0);

		[Test]
		public void LocalIdentifierConstructorTest ()
		{
			// SharedWithYouCore framework seems to work only for devices
			TestRuntime.AssertNotSimulator ();

			Assert.DoesNotThrow (() => { _ = new SWCollaborationMetadata (Guid.NewGuid ().ToString ()); },
				"_InitWithLocalIdentifier");
		}

		[TestCase (SWIdentifierType.Local, "_InitWithLocalIdentifier")]
		[TestCase (SWIdentifierType.Collaboration, "_InitWithCollaborationIdentifier")]
		public void IdentifierTypeConstructorTest (SWIdentifierType identifierType, string methodName)
		{
			// SharedWithYouCore framework seems to work only for devices
			TestRuntime.AssertNotSimulator ();

			Assert.DoesNotThrow (() => { _ = new SWCollaborationMetadata (Guid.NewGuid ().ToString (), identifierType); },
				methodName);
		}
	}
}

#endif // !__WATCHOS__ && !__TVOS__
