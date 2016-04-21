//
// DepthFirstAstVisitor.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public abstract class DepthFirstAstVisitor : IAstVisitor
	{
		protected DepthFirstAstVisitor ()
		{
		}

		protected virtual void VisitChildren (Node node)
		{
			Node next;
			for (var child = node.FirstChild; child != null; child = next) {
				next = child.NextSibling;
				child.AcceptVisitor (this);
			}
		}

		public virtual void VisitExpressionList (ExpressionList expressionList)
		{
			VisitChildren (expressionList);
		}

		public virtual void VisitLiteralExpression (LiteralExpression literalExpression)
		{
			VisitChildren (literalExpression);
		}

		public virtual void VisitParenthesizedExpression (ParenthesizedExpression parenthesizedExpression)
		{
			VisitChildren (parenthesizedExpression);
		}

		public virtual void VisitUnaryExpression (UnaryExpression unaryExpression)
		{
			VisitChildren (unaryExpression);
		}

		public virtual void VisitBinaryExpression (BinaryExpression binaryExpression)
		{
			VisitChildren (binaryExpression);
		}

		public virtual void VisitNamedExpression (NamedExpression namedExpression)
		{
			VisitChildren (namedExpression);
		}

		public virtual void VisitConditionalExpression (ConditionalExpression conditionalExpression)
		{
			VisitChildren (conditionalExpression);
		}

		public virtual void VisitInvocationExpression (InvocationExpression invocationExpression)
		{
			VisitChildren (invocationExpression);
		}
	}
}