using System;
using Foundation;
using CoreData;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreData {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ExpressionDescriptionTest {

		[Test]
		public void WeakFramework ()
		{
			using (var exp = new NSExpressionDescription ())
				Assert.That (exp.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
		}

		[Test]
		public void GetSetExpression ()
		{
			using (var exp = new NSExpressionDescription ()) {
				exp.Name = "Test";
				Assert.IsNull (exp.Expression, "An unset Expression should be null.");
				exp.Expression = new NSExpression (NSExpressionType.Block);
				Assert.IsNotNull (exp.Expression, "Expression was not correctly set.");
			}
		}

		[Test]
		public void GetSetResultType ()
		{
			using (var exp = new NSExpressionDescription ()) {
				exp.Name = "Test";
				Assert.AreEqual (exp.ResultType, NSAttributeType.Undefined,
								 "The default value of an unset ResultType should be 'Undefined'");
				exp.ResultType = NSAttributeType.Boolean;
				Assert.AreEqual (NSAttributeType.Boolean, exp.ResultType,
								 "ResultType was not correctly set.");
			}
		}
	}
}
