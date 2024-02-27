using System;
using Foundation;
using CoreData;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreData {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FetchRequestExpressionTest {

		[Test]
		public void WeakFramework ()
		{
			using (var exp = new NSExpression (NSExpressionType.Block))
			using (var context = new NSExpression (NSExpressionType.EvaluatedObject))
			using (var fetch = NSFetchRequestExpression.FromFetch (exp, context, false))
				Assert.That (exp.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
		}

		[Test]
		public void GetRequest ()
		{
			using (var exp = new NSExpression (NSExpressionType.Block))
			using (var context = new NSExpression (NSExpressionType.EvaluatedObject))
			using (var fetch = NSFetchRequestExpression.FromFetch (exp, context, false))
				Assert.NotNull (fetch.Request);
		}

		[Test]
		public void GetContext ()
		{
			using (var exp = new NSExpression (NSExpressionType.Block))
			using (var context = new NSExpression (NSExpressionType.EvaluatedObject))
			using (var fetch = NSFetchRequestExpression.FromFetch (exp, context, false))
				Assert.NotNull (fetch.Context);
		}

		[Test]
		public void GetIsCountOnly ()
		{
			using (var exp = new NSExpression (NSExpressionType.Block))
			using (var context = new NSExpression (NSExpressionType.EvaluatedObject))
			using (var fetch = NSFetchRequestExpression.FromFetch (exp, context, false))
				Assert.IsFalse (fetch.IsCountOnly, "IsCountOnly was not correctly set to false.");

			using (var exp = new NSExpression (NSExpressionType.Block))
			using (var context = new NSExpression (NSExpressionType.EvaluatedObject))
			using (var fetch = NSFetchRequestExpression.FromFetch (exp, context, true))
				Assert.IsTrue (fetch.IsCountOnly, "IsCountOnly was not correctly set to true.");
		}
	}
}
