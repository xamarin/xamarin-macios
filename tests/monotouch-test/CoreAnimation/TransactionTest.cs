//
// Unit tests for CATransaction
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using CoreAnimation;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreAnimation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TransactionTest {

		[Test]
		public void CompletionBlock_Null ()
		{
			CATransaction.CompletionBlock = null;
			Assert.Null (CATransaction.CompletionBlock, "CompletionBlock");
		}

		[Test]
		public void AnimationTimingFunction_Null ()
		{
			// NULL is not specified in Apple doc
			// but since it's the default value it makes sense to be able to set it back
			CATransaction.AnimationTimingFunction = null;
			Assert.Null (CATransaction.AnimationTimingFunction, "AnimationTimingFunction");
		}
	}
}

#endif // !__WATCHOS__
