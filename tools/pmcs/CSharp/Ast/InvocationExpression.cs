//
// InvocationExpression.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System.Collections.Generic;

namespace Xamarin.Pmcs.CSharp.Ast
{
	public sealed class InvocationExpression : Expression
	{
		public static readonly Role TargetRole = new Role (nameof (TargetRole));
		public static readonly Role ArgumentRole = new Role (nameof (ArgumentRole));

		public Expression Target {
			get { return GetChild<Expression> (TargetRole); }
			set { SetChild (value, TargetRole); }
		}

		public IEnumerable<Expression> Arguments {
			get { return GetChildren<Expression> (ArgumentRole); }
		}

		public Expression FirstArgument {
			get { return GetChild<Expression> (ArgumentRole); }
		}

		public InvocationExpression ()
		{
		}

		public InvocationExpression (string target)
		{
			Target = new LiteralExpression (target);
		}

		public void AddArgument (Expression argument)
		{
			AddChild (argument, ArgumentRole);
		}

		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitInvocationExpression (this);
		}
	}
}