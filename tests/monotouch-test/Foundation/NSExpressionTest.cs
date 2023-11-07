using System;
using System.Collections.Generic;
using System.Reflection;

using Foundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

[assembly: Preserve (typeof (NSExpression), AllMembers = true)]

namespace MonoTouchFixtures.Foundation {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSExpressionTest {
		List<string> properties = new List<string> { "Block", "ConstantValue", "KeyPath",  "Function",
			"Variable", "Operand", "Arguments", "Collection", "Predicate", "LeftExpression",
			"RightExpression", "TrueExpression", "FalseExpression"};

		void TestProperties (NSExpression expression, List<string> availableProperties)
		{
			foreach (var pName in properties) {
				var type = typeof (NSExpression);
				var pInfo = type.GetProperty (pName);
				if (!availableProperties.Contains (pName)) {
					Assert.Throws<InvalidOperationException> (() => {
						// we want to make sure that is an invalid operation exception and not
						// a reflection one
						try {
							pInfo.GetValue (expression);
						} catch (TargetInvocationException e) {
							throw e.GetBaseException ();
						}
					}, $"Expressions of type '{expression.ExpressionType}' does not support the property '{pName}'");
				} else {
					Assert.DoesNotThrow (() => {
						pInfo.GetValue (expression);
					}, $"Expressions of type '{expression.ExpressionType}' do support the property '{pName}'");
				}
			}
		}

		[TestCase ("Foo", ExpectedResult = "Foo")]
		[TestCase (null, ExpectedResult = null)]
		public object FromConstant (object input)
		{
			NSObject value = null;

			switch (input) {
			case String stringValue:
				value = new NSString (stringValue);
				break;
			}

			using (var expression = NSExpression.FromConstant (value))
			using (var result = expression.EvaluateWith (null, null) as NSObject)
				return result?.ToString ();
		}

		[Test]
		public void FromKeyPath ()
		{
			using (var expression = NSExpression.FromKeyPath ("value"))
			using (var result = expression.EvaluateWith (null, null) as NSString)
				Assert.IsNull (result);
		}

		[Test]
		public void FromFunctionTest ()
		{
			using (var expression = NSExpression.FromFunction ((o, e, c) => { return new NSString ("Foo"); }, new NSExpression [] { }))
			using (var result = expression.EvaluateWith (null, null) as NSString)
				Assert.AreEqual ("Foo", result.ToString ());
		}

		[Test]
		public void FromFormatWithArgsTest ()
		{
			using (var expression = NSExpression.FromFormat ("%f*%f", new NSObject [] { new NSNumber (2.0), new NSNumber (2.0) }))
			using (var result = expression.EvaluateWith (null, null) as NSNumber)
				Assert.AreEqual (4.0, result.DoubleValue);
		}

		[Test]
		public void FromFormatWithNoArgsTest ()
		{
			using (var expression = NSExpression.FromFormat ("2*2"))
			using (var result = expression.EvaluateWith (null, null) as NSNumber)
				Assert.AreEqual (4.0, result.DoubleValue);
		}

		[Test]
		public void FromFormatConstant ()
		{
			using (var expression = NSExpression.FromFormat ("2"))
			using (var result = expression.EvaluateWith (null, null) as NSNumber)
				Assert.AreEqual (2, result.DoubleValue);
		}

		[Test]
		public void AggregatePropertiesTest ()
		{
			var availableProperties = new List<string> { "Collection" };
			using (var lower = NSExpression.FromConstant (new NSNumber (0)))
			using (var upper = NSExpression.FromConstant (new NSNumber (5)))
			using (var expression = NSExpression.FromAggregate (new NSExpression [] { lower, upper })) {
				Assert.AreEqual (NSExpressionType.NSAggregate, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void UnionSetPropertiesTest ()
		{
			var availableProperties = new List<string> { "LeftExpression", "RightExpression" };

			using (var llower = NSExpression.FromConstant (new NSNumber (0)))
			using (var lupper = NSExpression.FromConstant (new NSNumber (5)))
			using (var lh = NSExpression.FromAggregate (new NSExpression [] { llower, lupper }))
			using (var rlower = NSExpression.FromConstant (new NSNumber (10)))
			using (var rupper = NSExpression.FromConstant (new NSNumber (50)))
			using (var rh = NSExpression.FromAggregate (new NSExpression [] { rlower, rupper }))
			using (var expression = NSExpression.FromUnionSet (lh, rh)) {
				Assert.AreEqual (NSExpressionType.UnionSet, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void IntersectSetPropertiesTest ()
		{
			var availableProperties = new List<string> { "LeftExpression", "RightExpression" };

			using (var llower = NSExpression.FromConstant (new NSNumber (0)))
			using (var lupper = NSExpression.FromConstant (new NSNumber (5)))
			using (var lh = NSExpression.FromAggregate (new NSExpression [] { llower, lupper }))
			using (var rlower = NSExpression.FromConstant (new NSNumber (10)))
			using (var rupper = NSExpression.FromConstant (new NSNumber (50)))
			using (var rh = NSExpression.FromAggregate (new NSExpression [] { rlower, rupper }))
			using (var expression = NSExpression.FromIntersectSet (lh, rh)) {
				Assert.AreEqual (NSExpressionType.IntersectSet, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void MinusSetPropertiesTest ()
		{
			var availableProperties = new List<string> { "LeftExpression", "RightExpression" };

			using (var llower = NSExpression.FromConstant (new NSNumber (0)))
			using (var lupper = NSExpression.FromConstant (new NSNumber (5)))
			using (var lh = NSExpression.FromAggregate (new NSExpression [] { llower, lupper }))
			using (var rlower = NSExpression.FromConstant (new NSNumber (10)))
			using (var rupper = NSExpression.FromConstant (new NSNumber (50)))
			using (var rh = NSExpression.FromAggregate (new NSExpression [] { rlower, rupper }))
			using (var expression = NSExpression.FromMinusSet (lh, rh)) {
				Assert.AreEqual (NSExpressionType.MinusSet, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void ConstantPropertiesTest ()
		{
			var availableProperties = new List<string> { "ConstantValue" };
			using (var expression = NSExpression.FromFormat ("2")) {
				Assert.AreEqual (NSExpressionType.ConstantValue, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void VariablePropertiesTest ()
		{
			var availableProperties = new List<string> { "Variable" };
			using (var expression = NSExpression.FromVariable ("Variable")) {
				Assert.AreEqual (NSExpressionType.Variable, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void KeyPathPropertiesTest ()
		{
			var availableProperties = new List<string> { "KeyPath", "Operand", "Arguments" };
			using (var expression = NSExpression.FromKeyPath ("value")) {
				Assert.AreEqual (NSExpressionType.KeyPath, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void FunctionPropertiesTest ()
		{
			var availableProperties = new List<string> { "Function", "Operand", "Arguments" };
			using (var expression = NSExpression.FromFormat ("2*2")) {
				Assert.AreEqual (NSExpressionType.Function, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void BlockPropertiesTest ()
		{
			var availableProperties = new List<string> { "Block", "Arguments" };
			using (var expression = NSExpression.FromFunction ((o, e, c) => { return new NSString ("Foo"); }, new NSExpression [] { })) {
				Assert.AreEqual (NSExpressionType.Block, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void EvaluatedObjectPropertiesTest ()
		{
			var availableProperties = new List<string> { };
			var mySearchKey = new NSString ("James");
			using (var predicate = NSPredicate.FromFormat ("ANY employees.firstName like 'Matthew'") as NSComparisonPredicate)
			using (var expression = predicate.LeftExpression.Operand) { // NSExpressionType.EvaluatedObject;
				Assert.AreEqual (NSExpressionType.EvaluatedObject, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}

		[Test]
		public void AnyKeyPropertiesTest ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 9, throwIfOtherPlatform: false);

			var availableProperties = new List<string> { };
			using (var expression = NSExpression.FromAnyKey ()) {
				Assert.AreEqual (NSExpressionType.AnyKey, expression.ExpressionType);
				TestProperties (expression, availableProperties);
			}
		}
	}
}
