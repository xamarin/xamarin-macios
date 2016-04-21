//
// IAstVisitor.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public interface IAstVisitor
	{
		void VisitExpressionList (ExpressionList expressionList);
		void VisitLiteralExpression (LiteralExpression literalExpression);
		void VisitParenthesizedExpression (ParenthesizedExpression parenthesizedExpression);
		void VisitUnaryExpression (UnaryExpression unaryExpression);
		void VisitBinaryExpression (BinaryExpression binaryExpression);
		void VisitNamedExpression (NamedExpression namedExpression);
		void VisitConditionalExpression (ConditionalExpression conditionalExpression);
		void VisitInvocationExpression (InvocationExpression invocationExpression);
	}
}