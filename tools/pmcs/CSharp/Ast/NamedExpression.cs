//
// NamedExpression.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public class NamedExpression : BinaryExpression
	{
		public NamedExpression ()
		{
		}

		public NamedExpression (Expression leftOperand, TokenType op, Expression rightOperand)
			: base (leftOperand, op, rightOperand)
		{
		}

		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitNamedExpression (this);
		}
	}
}