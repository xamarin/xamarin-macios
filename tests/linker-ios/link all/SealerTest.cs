// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
#if XAMCORE_2_0
using Foundation;
#else
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace Linker.Sealer {

	[Preserve (AllMembers = true)]
	public class Unsealable {

		public virtual bool A () { return true; }
		public virtual bool B () { return false; }
	}

	[Preserve (AllMembers = true)]
	public class Sealable : Unsealable {
		public override bool B () { return true; }
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SealerTest {

		[Test]
		public void Sealed ()
		{
			// this can not be optimized into a sealed type
			Assert.False (typeof (Unsealable).IsSealed, "Unsealed");
#if DEBUG
			// this is not a sealed type (in the source)
			Assert.False (typeof (Sealable).IsSealed, "Sealed");
#else
			// but it can be optimized / sealed as nothing else is (or can) subclass it
			Assert.True (typeof (Sealable).IsSealed, "Sealed");
#endif
		}

		[Test]
		public void Final ()
		{
			var t = typeof (Sealable);
#if DEBUG
			// this is not a sealed method (in the source)
			Assert.False (t.GetMethod ("B").IsFinal, "Not Final");
#else
			// but it can be optimized / sealed as nothing else is (or can) overrides it
			Assert.True (t.GetMethod ("B").IsFinal, "Final");
#endif
		}

		[Test]
		public void Virtual ()
		{
			var t = typeof (Sealable);
#if DEBUG
			// both methods are virtual (iin the source)
			Assert.True (t.GetMethod ("A").IsVirtual, "A");
			Assert.True (t.GetMethod ("B").IsVirtual, "B");
#else
			// but A can be de-virtualized as it overrides nothing and is not overridden
			Assert.False (t.GetMethod ("A").IsVirtual, "A");
			Assert.True (t.GetMethod ("B").IsVirtual, "B");
#endif
		}
	}
}
