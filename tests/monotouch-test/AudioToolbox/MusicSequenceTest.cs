//
// MusicSequence unit Tests
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using AudioToolbox;
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.AudioToolbox;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MusicSequenceTest {

		[Test]
		public void Defaults ()
		{
			using (var ms = new MusicSequence ()) {
				Assert.NotNull (ms.AUGraph, "AUGraph");
				Assert.That (ms.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (ms.SequenceType, Is.EqualTo (MusicSequenceType.Beats), "SequenceType");
				Assert.That (ms.TrackCount, Is.EqualTo (0), "TrackCount");
			}
		}

		[Test]
		public void SMPTEResolution ()
		{
			// calls are CF_INLINE (in header files) which means it can't be p/invoked
			// it will throw EntryPointNotFoundException since it's not part of the library
			using (var ms = new MusicSequence ()) {
				sbyte fps;
				byte ticks;
				ms.GetSmpteResolution (Int16.MaxValue, out fps, out ticks);
				Assert.That (fps, Is.EqualTo (127), "fps");
				Assert.That (ticks, Is.EqualTo (127), "ticks");

				// not a roundtip - but identical to ObjC results
				Assert.That (ms.SetSmpteResolution (fps, ticks), Is.EqualTo (-32385), "set");
			}
		}
	}
}

#endif // !__WATCHOS__
