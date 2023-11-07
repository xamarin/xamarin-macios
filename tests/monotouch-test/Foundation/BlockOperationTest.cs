//
// Unit tests for NSBlockOperation
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class BlockOperationTest {

		[Test]
		public void Create_Null ()
		{
			// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSBlockOperation addExecutionBlock:]: block is nil
			Assert.Throws<ArgumentNullException> (() => NSBlockOperation.Create (null));
		}

		[Test]
		public void Add_Null ()
		{
			using (var bo = NSBlockOperation.Create (Create_Null)) {
				// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSBlockOperation addExecutionBlock:]: block is nil
				Assert.Throws<ArgumentNullException> (() => bo.AddExecutionBlock (null));
			}
		}

		[Test]
		public void ExecutionBlocks ()
		{
			using (var bo = NSBlockOperation.Create (Create_Null)) {
				bo.AddExecutionBlock (Add_Null);
				Assert.That (bo.ExecutionBlocks.Length, Is.EqualTo (2), "ExecutionBlocks");
			}
		}
	}
}
