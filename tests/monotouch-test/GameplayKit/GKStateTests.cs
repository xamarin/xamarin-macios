//
// Unit tests for GKState
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;

using Foundation;
using GameplayKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.GameplayKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKStateTests {

		[OneTimeSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void IsValidNextState ()
		{
			var chaseState = new ValidState ();
			var isValid = chaseState.IsValidNextState (typeof (InvalidState));
			Assert.IsFalse (isValid, "Type");

			var invalid = new InvalidState ();
			isValid = chaseState.IsValidNextState (invalid);
			Assert.IsFalse (isValid, "Instance");

			isValid = chaseState.IsValidNextState (invalid.Class);
			Assert.IsFalse (isValid, "Class");
		}

		[Test]
		public void NullIsValidNextState ()
		{
			// this will test the (manual) binding code (and not the test implementation)
			using (var state = new InvalidState ()) {
				Assert.Throws<ArgumentNullException> (delegate { state.IsValidNextState ((Class) null); }, "Class");
				Assert.Throws<ArgumentNullException> (delegate { state.IsValidNextState ((Type) null); }, "Type");
				Assert.Throws<ArgumentNullException> (delegate { state.IsValidNextState ((GKState) null); }, "Instance");
			}
		}

		[Test]
		public void Concrete ()
		{
			// GKState is an abstract type - but it does implement IsValidNextState (and accept anything)
			using (var s1 = new ValidState ())
			using (var s2 = new InvalidState ()) {
				Assert.True (s2.IsValidNextState (s2), "self");
				Assert.True (s2.IsValidNextState (s1), "different");
			}
		}
	}

	class ValidState : GKState {
		public override bool IsValidNextState (Class stateClass)
		{
			return (Class.Lookup (stateClass) != typeof (InvalidState));
		}
	}

	class InvalidState : GKState { }
}

#endif // __WATCHOS__
