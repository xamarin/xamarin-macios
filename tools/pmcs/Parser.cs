// Parser.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Xamarin.Pmcs
{
	public class Parser
	{
		readonly Func<Expression> lowestPrecedenceOpParser;

		public BinOpParser Multiplicitive { get; set; }
		public BinOpParser Additive { get; set; }
		public BinOpParser Shift { get; set; }
		public BinOpParser Relational { get; set; }
		public BinOpParser Equality { get; set; }
		public BinOpParser LogicalAnd { get; set; }
		public BinOpParser LogicalExclusiveOr { get; set; }
		public BinOpParser LogicalOr { get; set; }
		public BinOpParser AndAlso { get; set; }
		public BinOpParser OrElse { get; set; }
		public BinOpParser Assign { get; set; }

		public Parser ()
		{
			// Factor
			// ^-- Primary
			//     ^-- Unary

			Multiplicitive = new BinOpParser ("Multiplicive", this, Unary) {
				new BinOpParserEntry (TokenType.Multiply, Expression.Multiply),
				new BinOpParserEntry (TokenType.Divide, Expression.Divide),
				new BinOpParserEntry (TokenType.Modulo, Expression.Modulo)
			};

			Additive = new BinOpParser ("Additive", this, Multiplicitive) {
				new BinOpParserEntry (TokenType.Add, Expression.Add (l, r)),
				new BinOpParserEntry (TokenType.Subtract, Expression.Subtract),
			};

			Shift = new BinOpParser ("Shift", this, Additive) {
				new BinOpParserEntry (TokenType.LeftShift, Expression.LeftShift),
				new BinOpParserEntry (TokenType.RightShift, Expression.RightShift)
			};

			Relational = new BinOpParser ("Relational", this, Shift) {
				new BinOpParserEntry (TokenType.LessThan, Expression.LessThan),
				new BinOpParserEntry (TokenType.GreaterThan, Expression.GreaterThan),
				new BinOpParserEntry (TokenType.LessThanOrEqual, Expression.LessThanOrEqual),
				new BinOpParserEntry (TokenType.GreaterThanOrEqual, Expression.GreaterThanOrEqual),
				new BinOpParserEntry (TokenType.Is,
					(l, r) => Expression.TypeIs (l, (Type)((ConstantExpression)r).Value)),
				new BinOpParserEntry (TokenType.As,
					(l, r) => Expression.TypeAs (l, (Type)((ConstantExpression)r).Value)),
				new BinOpParserEntry (TokenType.In,
					(l, r) => Expression.Call (enumerable_Contains.MakeGenericMethod (l.Type), r, l))
			};

			Equality = new BinOpParser ("Equality", this, Relational) {
				new BinOpParserEntry (TokenType.Equal, Expression.Equal),
				new BinOpParserEntry (TokenType.NotEqual, Expression.NotEqual)
			};

			LogicalAnd = new BinOpParser ("LogicalAnd", this, Equality) {
				new BinOpParserEntry (TokenType.And, Expression.And)
			};

			LogicalExclusiveOr = new BinOpParser ("LogicalExclusiveOr", this, LogicalAnd) {
				new BinOpParserEntry (TokenType.ExclusiveOr, Expression.ExclusiveOr)
			};

			LogicalOr = new BinOpParser ("LogicalOr", this, LogicalExclusiveOr) {
				new BinOpParserEntry (TokenType.Or, Expression.Or)
			};

			AndAlso = new BinOpParser ("ConditionalAnd", this, LogicalOr) {
				new BinOpParserEntry (TokenType.AndAlso, Expression.AndAlso)
			};

			OrElse = new BinOpParser ("ConditionalOr", this, AndAlso) {
				new BinOpParserEntry (TokenType.OrElse, Expression.OrElse)
			};

			Func<Expression> conditional = () => {
				try {
					var expr = OrElse.Descend ();
					if (current.Type == TokenType.QuestionMark) {
						Read ();
						var trueExpr = Expr ();
						Match (TokenType.Colon);
						expr = Expression.Condition (expr, trueExpr, Expr ());
					}
					return expr;
				} finally {
					trace.Leave ();
				}
			};

			var assign = new BinOpParser ("Assign", this, conditional) {
				new BinOpParserEntry (TokenType.Assign, Expression.Assign),
				new BinOpParserEntry (TokenType.MultiplyAssign, Expression.MultiplyAssign),
				new BinOpParserEntry (TokenType.DivideAssign, Expression.DivideAssign),
				new BinOpParserEntry (TokenType.ModuloAssign, Expression.ModuloAssign),
				new BinOpParserEntry (TokenType.AddAssign, Expression.AddAssign),
				new BinOpParserEntry (TokenType.SubtractAssign, Expression.SubtractAssign),
				new BinOpParserEntry (TokenType.LeftShiftAssign, Expression.LeftShiftAssign),
				new BinOpParserEntry (TokenType.RightShiftAssign, Expression.RightShiftAssign),
				new BinOpParserEntry (TokenType.AndAssign, Expression.AndAssign),
				new BinOpParserEntry (TokenType.ExclusiveOrAssign, Expression.ExclusiveOrAssign),
				new BinOpParserEntry (TokenType.OrAssign, Expression.OrAssign),
			};

			lowestPrecedenceOpParser = assign.Descend;
		}

		IEnumerable<T> Concat<T> (IEnumerable<T> a, IEnumerable<T> b)
		{
			if (a == null && b == null)
				return null;
			else if (a == null)
				return b;
			else if (b == null)
				return a;
			return a.Concat (b);
		}

		public Expression<TDelegate> Parse<TDelegate> (Tokenizer tokenizer,
			IEnumerable<ParameterExpression> parameters = null,
			IEnumerable<ParameterExpression> variables = null,
			IEnumerable<Expression> preamble = null)
		{
			return Expression.Lambda<TDelegate> (Parse (tokenizer, parameters, variables, preamble), parameters);
		}

		public BlockExpression Parse (Tokenizer tokenizer,
			IEnumerable<ParameterExpression> parameters = null,
			IEnumerable<ParameterExpression> variables = null,
			IEnumerable<Expression> preamble = null)
		{
			return null;
		}

		Token Read ()
		{
			tokenizer.Scan ();
			current = tokenizer.Current;
			return current;
		}

		void Match (TokenType type, bool read = true)
		{
			if (current.Type != type)
				throw new SyntaxException (current, type);
			if (read)
				Read ();
		}

		Expression Expr ()
		{
			try {
				trace.Enter ("Expr");
				return lowestPrecedenceOpParser ();
			} finally {
				trace.Leave ();
			}
		}

		Expression Factor ()
		{
			try {
				trace.Enter ("Factor");

				Expression expr = null;

				switch (current.Type) {
				case TokenType.Constant:
					expr = current.Expression;
					Read ();
					return expr;
				case TokenType.Identifier:
					return Resolve ();
				case TokenType.LeftParen:
					Read ();
					expr = Expr ();
					Match (TokenType.RightParen);
					return expr;
				default:
					throw new SyntaxException (current);
				}
			} finally {
				trace.Leave ();
			}
		}

		Expression Primary ()
		{
			try {
				trace.Enter ("Primary");

				var expr = Factor ();

				while (current.Type == TokenType.Period) {
					switch (current.Type) {
					case TokenType.Period:
						Read ();
						expr = Resolve (expr);
						break;
					}
				}

				return expr;
			} finally {
				trace.Leave ();
			}
		}

		Expression Unary ()
		{
			try {
				trace.Enter ("Unary");

				switch (current.Type) {
				case TokenType.Subtract:
					Read ();
					return Expression.Negate (Unary ());
				case TokenType.Add:
					Read ();
					return Expression.UnaryPlus (Unary ());
				case TokenType.Not:
					Read ();
					return Expression.Not (Unary ());
				case TokenType.OnesComplement:
					Read ();
					return Expression.OnesComplement (Unary ());
				case TokenType.Increment:
					Read ();
					return Expression.PostIncrementAssign (Unary ());
				case TokenType.Decrement:
					Read ();
					return Expression.PostDecrementAssign (Unary ());
				case TokenType.LeftParen:
					if (!current.Flags.HasFlag (TokenFlags.MaybeCast))
						return Primary ();

					Read ();

					// a left paren flagged as a possible cast is guaranteed
					// by the tokenizer to be followed by an identifier,
					// but just in case...
					Match (TokenType.Identifier, read: false);

					Type type = null;
					var typeExpr = Resolve () as ConstantExpression;

					if (typeExpr == null || (type = typeExpr.Value as Type) == null) {
						// the tokenizer guarantees that this will be a factor or an error
						var expr = Factor ();
						Match (TokenType.RightParen);
						return expr;
					}

					Match (TokenType.RightParen);
					return Expression.Convert (Primary (), type);
				default:
					return Primary ();
				}
			} finally {
				trace.Leave ();
			}
		}

		public struct BinOpParserEntry
		{
			public TokenType Type;
			public Func<Expression, Expression, Expression> Generator;

			public BinOpParserEntry (TokenType op, Func<Expression, Expression, Expression> generator)
			{
				Type = op;
				Generator = generator;
			}
		}

		public class BinOpParser : IEnumerable<BinOpParserEntry>
		{
			List<BinOpParserEntry> operators = new List<BinOpParserEntry> ();
			Func<Expression> descender;
			Parser parser;
			string name;

			public BinOpParser (string name, Parser parser, Func<Expression> descender)
			{
				this.name = name;
				this.parser = parser;
				this.descender = descender;
			}

			public BinOpParser (string name, Parser parser, BinOpParser descender)
			{
				this.name = name;
				this.parser = parser;
				this.descender = descender.Descend;
			}

			public void Add (BinOpParserEntry op)
			{
				operators.Add (op);
			}

			public bool IsMatch (TokenType op, out BinOpParserEntry entry)
			{
				entry = default (BinOpParserEntry);

				foreach (var e in operators) {
					if (e.Type == op) {
						entry = e;
						return true;
					}
				}
				return false;
			}

			public void Swizzle (TokenType op, Func<Func<Expression, Expression, Expression>,
				Func<Expression, Expression, Expression>> swizzler)
			{
				for (int i = 0; i < operators.Count; i++) {
					if (operators [i].Type == op) {
						operators [i] = new BinOpParserEntry {
							Type = op,
							Generator = swizzler (operators [i].Generator)
						};
						break;
					}
				}
			}

			public IEnumerator<BinOpParserEntry> GetEnumerator ()
			{
				return operators.GetEnumerator ();
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return operators.GetEnumerator ();
			}

			public Expression Descend ()
			{
				try {
					parser.trace.Enter (name);
					var expr = descender ();
					BinOpParserEntry op;
					while (IsMatch (parser.current.Type, out op)) {
						parser.Read ();
						expr = op.Generator (expr, descender ());
					}
					return expr;
				} finally {
					parser.trace.Leave ();
				}
			}
		}

		class Trace
		{
			Parser parser;
			Stack<string> depth = new Stack<string> ();

			public TextWriter Writer { get; set; }

			public Trace (Parser parser, TextWriter writer = null)
			{
				this.parser = parser;
				Writer = writer;
			}

			void Indent (int level)
			{
				if (Writer != null)
					Writer.Write (String.Empty.PadRight (level * 2));
			}

			public void Enter (string name)
			{
				if (Writer != null) {
					Indent (depth.Count);
					depth.Push (name);
					Writer.WriteLine ("Enter {0} @ {1}", name, parser.current);
				}
			}

			public void Leave ()
			{
				if (Writer != null) {
					var name = depth.Pop ();
					Indent (depth.Count);
					Writer.WriteLine ("Leave {0} @ {1}", name, parser.current);
				}
			}
		}
	}
}