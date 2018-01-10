//
// NSPredicateEditorRowTemplate.cs
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc
//

using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;
using CoreData;

namespace AppKit
{
	public partial class NSPredicateEditorRowTemplate
	{
		public NSPredicateEditorRowTemplate (params NSCompoundPredicateType [] compoundTypes)
			: this (compoundTypes.Select (t => NSNumber.FromUInt32 ((uint)t)).ToArray ())
		{
		}

		public NSPredicateEditorRowTemplate (
			IEnumerable<NSExpression> leftExpressions,
			IEnumerable<NSExpression> rightExpressions,
			IEnumerable<NSPredicateOperatorType> operators,
			NSComparisonPredicateModifier modifier = NSComparisonPredicateModifier.Direct,
			NSComparisonPredicateOptions options = NSComparisonPredicateOptions.None)
			: this (
				leftExpressions.ToArray (),
				rightExpressions.ToArray (),
				modifier,
				operators.Select (o => NSNumber.FromUInt32 ((uint)o)).ToArray (),
				options)
		{
		}

		public NSPredicateEditorRowTemplate (
			IEnumerable<string> leftExpressionsFromKeyPaths,
			IEnumerable<string> rightExpressionsFromConstants,
			IEnumerable<NSPredicateOperatorType> operators,
			NSComparisonPredicateModifier modifier = NSComparisonPredicateModifier.Direct,
			NSComparisonPredicateOptions options = NSComparisonPredicateOptions.None)
			: this (
				leftExpressionsFromKeyPaths.Select (k => NSExpression.FromKeyPath (k)),
				rightExpressionsFromConstants.Select (k => NSExpression.FromConstant (new NSString (k))),
				operators,
				modifier,
				options)
		{
		}

		public NSPredicateEditorRowTemplate (
			string leftExpressionFromKeyPath,
			string rightExpressionFromConstant,
			IEnumerable<NSPredicateOperatorType> operators,
			NSComparisonPredicateModifier modifier = NSComparisonPredicateModifier.Direct,
			NSComparisonPredicateOptions options = NSComparisonPredicateOptions.None)
			: this (
				new [] { leftExpressionFromKeyPath },
				new [] { rightExpressionFromConstant },
				operators,
				modifier,
				options)
		{
		}

		public NSPredicateEditorRowTemplate (
			string leftExpressionFromKeyPath,
			IEnumerable<string> rightExpressionsFromConstants,
			IEnumerable<NSPredicateOperatorType> operators,
			NSComparisonPredicateModifier modifier = NSComparisonPredicateModifier.Direct,
			NSComparisonPredicateOptions options = NSComparisonPredicateOptions.None)
			: this (
				new [] { leftExpressionFromKeyPath },
				rightExpressionsFromConstants,
				operators,
				modifier,
				options)
		{
		}

		public NSPredicateEditorRowTemplate (
			IEnumerable<NSExpression> leftExpressions,
			NSAttributeType attributeType,
			IEnumerable<NSPredicateOperatorType> operators,
			NSComparisonPredicateModifier modifier = NSComparisonPredicateModifier.Direct,
			NSComparisonPredicateOptions options = NSComparisonPredicateOptions.None)
			: this (
				leftExpressions.ToArray (),
				attributeType,
				modifier,
				operators.Select (o => NSNumber.FromUInt32 ((uint)o)).ToArray (),
				options)
		{
		}

		public NSPredicateEditorRowTemplate (
			IEnumerable<string> leftExpressionsFromKeyPaths,
			NSAttributeType attributeType,
			IEnumerable<NSPredicateOperatorType> operators,
			NSComparisonPredicateModifier modifier = NSComparisonPredicateModifier.Direct,
			NSComparisonPredicateOptions options = NSComparisonPredicateOptions.None)
			: this (
				leftExpressionsFromKeyPaths.Select (k => NSExpression.FromKeyPath (k)),
				attributeType,
				operators,
				modifier,
				options)
		{
		}

		public NSPredicateEditorRowTemplate (
			string leftExpressionFromKeyPath,
			NSAttributeType attributeType,
			IEnumerable<NSPredicateOperatorType> operators,
			NSComparisonPredicateModifier modifier = NSComparisonPredicateModifier.Direct,
			NSComparisonPredicateOptions options = NSComparisonPredicateOptions.None)
			: this (
				new [] { leftExpressionFromKeyPath },
				attributeType,
				operators,
				modifier,
				options)
		{
		}
	}
}
