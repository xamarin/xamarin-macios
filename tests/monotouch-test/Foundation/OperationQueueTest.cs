//
// Unit tests for NSOperationQueue
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class OperationQueueTest {

		[Test]
		public void Add_NSAction_Null ()
		{
			using (var q = new NSOperationQueue ()) {
				// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSBlockOperation addExecutionBlock:]: block is nil
				Assert.Throws<ArgumentNullException> (() => q.AddOperation ((Action) null));
			}
		}

		[Test]
		public void Add_NSOperation_Null ()
		{
			using (var q = new NSOperationQueue ()) {
				q.AddOperation ((NSOperation) null);
				Assert.That (q.OperationCount, Is.EqualTo ((nint) 0), "OperationCount");
				Assert.That (q.Operations.Length, Is.EqualTo (0), "Operations");
			}
		}

		[Test]
		public void Add_NSOperations_Null ()
		{
			using (var q = new NSOperationQueue ()) {
				q.AddOperations (null, true);
				Assert.That (q.OperationCount, Is.EqualTo ((nint) 0), "OperationCount");
				Assert.That (q.Operations.Length, Is.EqualTo (0), "Operations");
			}
		}
	}
}
