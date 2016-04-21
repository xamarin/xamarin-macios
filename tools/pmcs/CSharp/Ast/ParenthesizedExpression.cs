//
// ParenthesizedExpression.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public sealed class ParenthesizedExpression : Expression
	{
		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitParenthesizedExpression (this);
		}
	}
}