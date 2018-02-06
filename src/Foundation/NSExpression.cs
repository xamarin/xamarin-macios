// TODO: The NSExpression class is a cluster class in cococa. This means that now all the properties are supported by all the types of NSExpressions.
//       At the point of this written all the properties have been tested with all types EXCEPT NSExpressionType.Subquery and NSExpressionType.Conditional because writting
//       tests for those was not possible. The properties for these two types have been deduced from the other types yet bugs are possible and an objc excection will be thrown.
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace Foundation {

	public partial class NSExpression {

		[Export ("arguments")]
		public virtual NSExpression[] Arguments {
			get {
				var type = ExpressionType;
				if (type != NSExpressionType.Function && type != NSExpressionType.Block && type != NSExpressionType.KeyPath)
					throw new InvalidOperationException (
						$"NSExpressions of type {type} do not support the Arguments property. Expressions that support the property "
						+ "are of type Function, Block and KeyPath");
				return _Arguments;
			}
		}


		[Export ("collection")]
		public virtual NSObject Collection {
			get {
				var type = ExpressionType;
				if (type != NSExpressionType.NSAggregate)
					throw new InvalidOperationException (
						$"NSExpressions of type {type} do not support the Collection property. Expressions that support the property "
						+ "are of type NSAggregate");
				return _Collection;
			}
		}

		[Export ("predicate")]
		public virtual NSPredicate Predicate { 
			get {
				var type = ExpressionType;
				if (type != NSExpressionType.Conditional && type != NSExpressionType.Subquery)
					throw new InvalidOperationException (
						$"NSExpressions of type {type} do not support the Predicate property. Expressions that support the property "
						+ "are of type Conditional and Subquery");
				return _Predicate;
			}
		}

		[Export ("expressionBlock")]
		public virtual NSExpressionCallbackHandler Block {
			get {
				if (ExpressionType != NSExpressionType.Block)
					throw new InvalidOperationException (
						$"NSExpressions of type {ExpressionType} do not support the Block property. Expressions that support the property "
						+ "are created via the FromFunction (NSExpressionHandler target, NSExpression[] parameters) method.");
				return _Block;
			}
		} 
		
		[Export ("constantValue")]
		public virtual NSObject ConstantValue {
			get {
				if (ExpressionType != NSExpressionType.ConstantValue)
					throw new InvalidOperationException (
						$"NSExpressions of type {ExpressionType} do not support the ConstantValue property. Expressions that support the property "
						+ "are created via the FromConstant methods.");
				return _ConstantValue;
			}
		}

		[Export ("keyPath")]
		public virtual string KeyPath {
			get {
				if (ExpressionType != NSExpressionType.KeyPath)
					throw new InvalidOperationException (
						$"NSExpressions of type {ExpressionType} do not support the KeyPath property. Expressions that support the property "
						+ "are created via the FromKeyPath method.");
				return _KeyPath;
			}
		}

		[Export ("leftExpression")]
		public virtual NSExpression LeftExpression {
			get {
				var type = ExpressionType;
				if (type != NSExpressionType.Conditional && type != NSExpressionType.IntersectSet && type != NSExpressionType.MinusSet
					&& type != NSExpressionType.Subquery && type != NSExpressionType.UnionSet)
					throw new InvalidOperationException (
						$"NSExpressions of type {type} do not support the LeftExpression property. Expressions that support the property "
						+ "are of type Conditional, IntersectSet, MinusSet, Subquery or UnionSet");
				return _LeftExpression;
			}
		}
		
		[Mac(10,11),iOS(9,0)]
		[Export ("trueExpression")]
		public virtual NSExpression TrueExpression {
			get {
				var type = ExpressionType;
				if (type != NSExpressionType.Conditional && type != NSExpressionType.Subquery)
					throw new InvalidOperationException (
						$"NSExpressions of type {type} do not support the TrueExpression property. Expressions that support the property "
						+ "are of type Conditional and Subquery");
				return _TrueExpression;
			}
		}

		[Mac(10,11),iOS(9,0)]
		[Export ("falseExpression")]
		public virtual NSExpression FalseExpression {
			get {
				var type = ExpressionType;
				if (type != NSExpressionType.Conditional && type != NSExpressionType.Subquery)
					throw new InvalidOperationException (
						$"NSExpressions of type {type} do not support the FalseExpression property. Expressions that support the property "
						+ "are of type Conditional and Subquery");
				return _FalseExpression;
			}
		}

		[Export ("rightExpression")]
		public virtual NSExpression RightExpression {
			get {
				var type = ExpressionType;
				if (type != NSExpressionType.Conditional && type != NSExpressionType.IntersectSet && type != NSExpressionType.MinusSet
					&& type != NSExpressionType.Subquery && type != NSExpressionType.UnionSet)
					throw new InvalidOperationException (
						$"NSExpressions of type {type} do not support the RightExpression property. Expressions that support the property "
						+ "are of type Conditional, IntersectSet, MinusSet, Subquery or UnionSet");
				return _RightExpression;
			}
		}

		[Export ("function")]
		public virtual string Function {
			get {
				if (ExpressionType != NSExpressionType.Function)
					throw new InvalidOperationException (
						$"NSExpressions of type {ExpressionType} do not support the Function property. Expressions that support the property "
						+ "are created via the FromFunction (FromFunction (string name, NSExpression[] parameters) or FromFormat methods.");
				return _Function;
			}
		}

		[Export ("variable")]
		public virtual string Variable {
			get {
				if (ExpressionType != NSExpressionType.Variable)
					throw new InvalidOperationException (
						$"NSExpressions of type {ExpressionType} do not support the Function property. Expressions that support the property "
						+ "are created via the FromVariable method.");
				return _Variable;
			}
		}
		
		[Export ("operand")]
		public virtual NSExpression Operand {
			get {
				var type = ExpressionType;
				if (type != NSExpressionType.KeyPath && type != NSExpressionType.Function)
					throw new InvalidOperationException (
						$"NSExpressions of type {type} do not support the Arguments property. Expressions that support the property "
						+ "are of type Function, Block and KeyPath");
				return _Operand;
			}
		}
		
#if !XAMCORE_4_0 && !WATCH
		[Obsolete("Use 'EvaluateWith' instead.")]
		public virtual NSExpression ExpressionValueWithObject (NSObject obj, NSMutableDictionary context) {
			var result = EvaluateWith (obj, context);
			// if it can be casted, do return an NSEXpression else null
			return result as NSExpression;
		}
#endif
	}
}
