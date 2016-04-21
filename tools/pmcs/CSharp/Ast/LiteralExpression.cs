//
// LiteralExpression.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp.Ast
{
	public sealed class LiteralExpression : Expression
	{
		public string Value { get; set; }

		public LiteralExpression ()
		{
		}

		public LiteralExpression (string value)
		{
			Value = value;
		}

		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitLiteralExpression (this);
		}
	}
}