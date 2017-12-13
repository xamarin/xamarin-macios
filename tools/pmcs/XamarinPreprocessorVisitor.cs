//
// XamarinPreprocessorVisitor.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Pmcs.CSharp;
using Xamarin.Pmcs.CSharp.Ast;

namespace Xamarin.Pmcs
{
	class XamarinPreprocessorVisitor : Preprocessor.PreprocessorVisitor
	{
		public XamarinPreprocessorVisitor (Preprocessor preprocessor) : base (preprocessor)
		{
		}

		public override void VisitInvocationExpression (InvocationExpression invocationExpression)
		{
			base.VisitInvocationExpression (invocationExpression);
		}
	}
}
