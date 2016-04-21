//
// Parser.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Pmcs.CSharp.Ast;

namespace Xamarin.Pmcs.CSharp
{
	public class Parser
	{
		public sealed class SyntaxException : Exception
		{
			internal SyntaxException (Tokenizer tokenizer, Token token)
				: base (String.Format ("Invalid token {0} @ {1}:{2}",
					token, tokenizer.Path, tokenizer.Line))
			{
			}

			internal SyntaxException (Tokenizer tokenizer, Token token, TokenType expectedType)
				: base (String.Format ("Invalid token {0}; expected a {1} @ {2}:{3}",
					token, expectedType, tokenizer.Path, tokenizer.Line))
			{
			}
		}

		readonly Tokenizer tokenizer;
		readonly Trace trace;
		readonly Queue<Token> consumedTokens = new Queue<Token> ();
		readonly Func<Expression> lowestPrecedenceOpParser;
		Token current;

		public Queue<Token> ConsumedTokens {
			get { return consumedTokens; }
		}

		public Parser (Tokenizer tokenizer)
		{
			if (tokenizer == null)
				throw new ArgumentNullException (nameof (tokenizer));

			this.tokenizer = tokenizer;

			trace = new Trace (this, Console.Error);

			// Factor
			// ^-- Primary
			//     ^-- Unary
			//         ^-- Multiplicitive
			//             ^-- Additive
			//                 ^-- ... etc BinOpParsers below ...
			//                     ^ Assign

			var multiplicitive = new BinOpParser ("Multiplicive", this, Unary) {
				new BinOpParserEntry (TokenType.Multiply),
				new BinOpParserEntry (TokenType.Divide),
				new BinOpParserEntry (TokenType.Modulo)
			};

			var additive = new BinOpParser ("Additive", this, multiplicitive) {
				new BinOpParserEntry (TokenType.Add),
				new BinOpParserEntry (TokenType.Subtract),
			};

			var shift = new BinOpParser ("Shift", this, additive) {
				new BinOpParserEntry (TokenType.LeftShift),
				new BinOpParserEntry (TokenType.RightShift)
			};

			var relational = new BinOpParser ("Relational", this, shift) {
				new BinOpParserEntry (TokenType.LessThan),
				new BinOpParserEntry (TokenType.GreaterThan),
				new BinOpParserEntry (TokenType.LessThanOrEqual),
				new BinOpParserEntry (TokenType.GreaterThanOrEqual),
				// is, as, in
			};

			var equality = new BinOpParser ("Equality", this, relational) {
				new BinOpParserEntry (TokenType.Equal),
				new BinOpParserEntry (TokenType.NotEqual)
			};

			var logicalAnd = new BinOpParser ("LogicalAnd", this, equality) {
				new BinOpParserEntry (TokenType.And)
			};

			var logicalExclusiveOr = new BinOpParser ("LogicalExclusiveOr", this, logicalAnd) {
				new BinOpParserEntry (TokenType.ExclusiveOr)
			};

			var logicalOr = new BinOpParser ("LogicalOr", this, logicalExclusiveOr) {
				new BinOpParserEntry (TokenType.Or)
			};

			var andAlso = new BinOpParser ("ConditionalAnd", this, logicalOr) {
				new BinOpParserEntry (TokenType.AndAlso)
			};

			var orElse = new BinOpParser ("ConditionalOr", this, andAlso) {
				new BinOpParserEntry (TokenType.OrElse)
			};

			Func<Expression> conditional = () => {
				try {
					trace.Enter ("Conditional");
					var expr = orElse.Descend ();
					if (current.Type == TokenType.QuestionMark) {
						Read ();
						var trueExpr = Expr ();
						Match (TokenType.Colon);
						expr = new ConditionalExpression (
							expr, trueExpr, Expr ());
					}
					return expr;
				} finally {
					trace.Leave ();
				}
			};

			var assign = new BinOpParser ("Assign", this, conditional) {
				new BinOpParserEntry (TokenType.Assign),
				new BinOpParserEntry (TokenType.MultiplyAssign),
				new BinOpParserEntry (TokenType.DivideAssign),
				new BinOpParserEntry (TokenType.ModuloAssign),
				new BinOpParserEntry (TokenType.AddAssign),
				new BinOpParserEntry (TokenType.SubtractAssign),
				new BinOpParserEntry (TokenType.LeftShiftAssign),
				new BinOpParserEntry (TokenType.RightShiftAssign),
				new BinOpParserEntry (TokenType.AndAssign),
				new BinOpParserEntry (TokenType.ExclusiveOrAssign),
				new BinOpParserEntry (TokenType.OrAssign),
			};

			lowestPrecedenceOpParser = assign.Descend;
		}

		Token Read ()
		{
			do {
				current = tokenizer.Scan ();
				ConsumedTokens.Enqueue (current);
			} while (current.Type == TokenType.WhiteSpace ||
				current.Type == TokenType.MultiLineComment ||
				current.Type == TokenType.SingleLineComment);

			return current;
		}

		void Match (TokenType type, bool read = true)
		{
			if (current.Type != type)
				throw new SyntaxException (tokenizer, current, type);

			if (read)
				Read ();
		}

		public ExpressionList ParseExpressionList (TokenType expressionDelimiter,
			TokenType expressionListTerminator = TokenType.None)
		{
			try {
				trace.Enter ();

				ConsumedTokens.Clear ();
				Read ();

				if (current.Type == expressionListTerminator)
					return null;

				var expr = new ExpressionList { Delimiter = expressionDelimiter };
				expr.AddChild (Expr ());

				while (true) {
					if (current.Type == expressionDelimiter) {
						Read ();
						if (current.Type == TokenType.Eof ||
							current.Type == expressionListTerminator)
							break;
						expr.AddChild (Expr ());
					} else if (current.Type == TokenType.Eof ||
						current.Type == expressionListTerminator)
						break;
					else
						throw new SyntaxException (tokenizer, current, expressionDelimiter);
				}

				return expr;
			} finally {
				trace.Leave ();
			}
		}

		public Expression ParseExpression ()
		{
			try {
				trace.Enter ();
				ConsumedTokens.Clear ();
				return Expr ();
			} finally {
				trace.Leave ();
			}
		}

		Expression Expr ()
		{
			try {
				trace.Enter ();
				return lowestPrecedenceOpParser ();
			} finally {
				trace.Leave ();
			}
		}

		Expression Unary ()
		{
			try {
				trace.Enter ();
				var op = current.Type;
				switch (current.Type) {
				case TokenType.Subtract:
				case TokenType.Add:
				case TokenType.Not:
				case TokenType.OnesComplement:
				case TokenType.Increment:
				case TokenType.Decrement:
					Read ();
					return new UnaryExpression (op, Unary ());
				default:
					return Factor ();
				}
			} finally {
				trace.Leave ();
			}
		}

		Expression Factor ()
		{
			try {
				trace.Enter ();
				Expression expr;

				switch (current.Type) {
				case TokenType.StringLiteral:
				case TokenType.VerbatimStringLiteral:
				case TokenType.InterpolatedStringLiteral:
				case TokenType.CharLiteral:
					expr = new LiteralExpression {
						Value = current.Value
					};
					Read ();
					return expr;
				case TokenType.UnknownChar:
					expr = new LiteralExpression {
						Value = current.UnknownChar.ToString ()
					};
					Read ();
					return expr;
				case TokenType.RawLiteral:
					expr = new LiteralExpression { Value = current.Value };
					Read ();
					if (current.Type == TokenType.LeftParen)
						expr = Invoke (expr);
					return expr;
				case TokenType.LeftParen:
					Read ();
					expr = new ParenthesizedExpression ();
					expr.AddChild (Expr ());
					Match (TokenType.RightParen);
					return expr;
				default:
					throw new SyntaxException (tokenizer, current);
				}
			} finally {
				trace.Leave ();
			}
		}

		InvocationExpression Invoke (Expression targetExpr)
		{
			try {
				trace.Enter ();

				Match (TokenType.LeftParen, read: false);
				Read ();

				var expr = new InvocationExpression ();
				expr.AddChild (targetExpr, InvocationExpression.TargetRole);

				if (current.Type != TokenType.RightParen) {
					while (true) {
						var param = Expr ();
						if (param is LiteralExpression && (current.Type == TokenType.Colon ||
							current.Type == TokenType.Assign)) {
							Read ();
							param = new NamedExpression (
								param,
								TokenType.Colon,
								Expr ()
							);
						}
						expr.AddArgument (param);
						if (current.Type != TokenType.Comma)
							break;
						Read ();
					}
				}

				Match (TokenType.RightParen);
				return expr;
			} finally {
				trace.Leave ();
			}
		}

		public struct BinOpParserEntry
		{
			public TokenType Type;
			public Func<Expression, Expression, Expression> Generator;

			public BinOpParserEntry (TokenType op, 
				Func<Expression, Expression, Expression> generator)
			{
				Type = op;
				Generator = generator;
			}


			public BinOpParserEntry (TokenType op)
			{
				Type = op;
				Generator = null;
				Generator = BinOp;
			}

			BinaryExpression BinOp (Expression left, Expression right)
			{
				return new BinaryExpression (left, Type, right);
			}
		}

		public class BinOpParser : IEnumerable<BinOpParserEntry>
		{
			readonly List<BinOpParserEntry> operators = new List<BinOpParserEntry> ();
			readonly Func<Expression> descender;
			readonly Parser parser;
			readonly string name;

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
			readonly Parser parser;
			readonly Stack<string> depth = new Stack<string> ();

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

			[Conditional ("TRACE")]
			public void Enter (string name = null)
			{
				if (Writer != null) {
					name = name ?? new StackTrace ().GetFrame (1).GetMethod ().Name;
					Indent (depth.Count);
					depth.Push (name);
					Writer.WriteLine ("Enter {0} @ {1}", name, parser.current);
				}
			}

			[Conditional ("TRACE")]
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