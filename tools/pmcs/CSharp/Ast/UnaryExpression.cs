//
// UnaryExpression.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public sealed class UnaryExpression : Expression
	{
		public static readonly Role OperandRole = new Role (nameof (OperandRole));

		public TokenType Operator { get; set; }

		public Expression Operand {
			get { return GetChild<Expression> (OperandRole); }
			set { SetChild (value, OperandRole); }
		}

		public UnaryExpression ()
		{
		}

		public UnaryExpression (TokenType op, Expression operand)
		{
			if (operand != null)
				AddChild (operand, OperandRole);

			Operator = op;
		}

		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitUnaryExpression (this);
		}
	}
}