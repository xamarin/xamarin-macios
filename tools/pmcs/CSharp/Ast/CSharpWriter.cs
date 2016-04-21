//
// CSharpWriter.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.IO;

namespace Xamarin.Pmcs.CSharp.Ast
{
	public sealed class CSharpWriter : IAstVisitor
	{
		readonly TextWriter writer;

		public CSharpWriter (TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException (nameof (writer));

			this.writer = writer;
		}

		public void VisitExpressionList (ExpressionList expressionList)
		{
			bool first = true;
			foreach (var child in expressionList.Children) {
				if (!first) {
					writer.Write (expressionList.Delimiter.AsWritten ());
					writer.Write (' ');
				}
				first = false;
				child.AcceptVisitor (this);
			}
		}

		public void VisitLiteralExpression(LiteralExpression literalExpression)
		{
			writer.Write (literalExpression.Value);
		}

		public void VisitParenthesizedExpression (ParenthesizedExpression parenthesizedExpression)
		{
			writer.Write ('(');
			foreach (var child in parenthesizedExpression.Children)
				child.AcceptVisitor (this);
			writer.Write (')');
		}

		public void VisitUnaryExpression (UnaryExpression unaryExpression)
		{
			writer.Write (unaryExpression.Operator.AsWritten ());
			unaryExpression.Operand.AcceptVisitor (this);
		}

		public void VisitBinaryExpression (BinaryExpression binaryExpression)
		{
			binaryExpression.LeftOperand.AcceptVisitor (this);
			if (!(binaryExpression is NamedExpression) || binaryExpression.Operator != TokenType.Colon)
				writer.Write (' ');
			writer.Write (binaryExpression.Operator.AsWritten ());
			writer.Write (' ');
			binaryExpression.RightOperand.AcceptVisitor (this);
		}

		public void VisitNamedExpression (NamedExpression namedExpression)
		{
			VisitBinaryExpression (namedExpression);
		}

		public void VisitConditionalExpression (ConditionalExpression conditionalExpression)
		{
			conditionalExpression.Condition.AcceptVisitor (this);
			writer.Write (" ? ");
			conditionalExpression.TrueExpression.AcceptVisitor (this);
			writer.Write (" : ");
			conditionalExpression.FalseExpression.AcceptVisitor (this);
		}

		public void VisitInvocationExpression (InvocationExpression invocationExpression)
		{
			invocationExpression.Target.AcceptVisitor (this);
			writer.Write (" (");
			bool first = true;
			foreach (var child in invocationExpression.Arguments) {
				if (!first)
					writer.Write (", ");
				first = false;
				child.AcceptVisitor (this);
			}
			writer.Write (')');
		}
	}
}