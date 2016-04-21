//
// ExpressionList.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public class ExpressionList : Node
	{
		public TokenType Delimiter { get; set; }

		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitExpressionList (this);
		}
	}
}