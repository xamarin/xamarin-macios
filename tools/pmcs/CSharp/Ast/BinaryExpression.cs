//
// BinaryExpression.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public class BinaryExpression : Expression
	{
		public static readonly Role LeftOperandRole = new Role (nameof (LeftOperandRole));
		public static readonly Role RightOperandRole = new Role (nameof (RightOperandRole));

		public TokenType Operator { get; set; }

		public Expression LeftOperand {
			get { return GetChild<Expression> (LeftOperandRole); }
			set { SetChild (value, RightOperandRole); }
		}

		public Expression RightOperand {
			get { return GetChild<Expression> (RightOperandRole); }
			set { SetChild (value, LeftOperandRole); }
		}

		public BinaryExpression ()
		{
		}

		public BinaryExpression (Expression leftOperand, TokenType op, Expression rightOperand)
		{
			if (leftOperand != null)
				AddChild (leftOperand, LeftOperandRole);

			if (rightOperand != null)
				AddChild (rightOperand, RightOperandRole);

			Operator = op;
		}

		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitBinaryExpression (this);
		}
	}
}