//
// ConditionalExpression.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public sealed class ConditionalExpression : Expression
	{
		public static readonly Role ConditionRole = new Role (nameof (ConditionRole));
		public static readonly Role TrueRole = new Role (nameof (TrueRole));
		public static readonly Role FalseRole = new Role (nameof (FalseRole));

		public Expression Condition {
			get { return GetChild<Expression> (ConditionRole); }
			set { SetChild (value, ConditionRole); }
		}

		public Expression TrueExpression {
			get { return GetChild<Expression> (TrueRole); }
			set { SetChild (value, TrueRole); }
		}

		public Expression FalseExpression {
			get { return GetChild<Expression> (FalseRole); }
			set { SetChild (value, FalseRole); }
		}

		public ConditionalExpression ()
		{
		}

		public ConditionalExpression (Expression conditionExpression,
			Expression trueExpression, Expression falseExpression)
		{
			if (conditionExpression != null)
				AddChild (conditionExpression, ConditionRole);

			if (trueExpression != null)
				AddChild (trueExpression, TrueRole);

			if (falseExpression != null)
				AddChild (falseExpression, FalseRole);
		}

		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitConditionalExpression (this);
		}
	}
}