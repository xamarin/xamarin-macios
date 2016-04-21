//
// Preprocessor.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.

using System;
using System.IO;

using Xamarin.Pmcs.CSharp;
using Xamarin.Pmcs.CSharp.Ast;

namespace Xamarin.Pmcs
{
	public class Preprocessor
	{
		public Profile Profile { get; set; }

		enum EnumState {
			None,
			Keyword,
			Name,
			Colon,
			LeftCurlyBracket
		}

		public unsafe void Preprocess (string path, TextReader reader, TextWriter writer)
		{
			const int tokenBufferSize = 32 * 1024;
			char *tokenBuffer = stackalloc char [tokenBufferSize];

			var tokenizer = new Tokenizer (reader, tokenBuffer, tokenBufferSize) { Path = path };
			var parser = new Parser (tokenizer);
			var astVisitor = new XamarinPreprocessorVisitor (this);
			var csharpWriter = new CSharpWriter (writer);
			var enumState = EnumState.None;

			while (true) {
				// we may have tokens consumed by the expression parser that
				// failed to actually parse, so dequeue them until they've
				// been blitted - they will not be parsed again; otherwise
				// read a token from the reader
				var token = parser.ConsumedTokens.Count > 0
					? parser.ConsumedTokens.Dequeue ()
					: tokenizer.Scan ();

				switch (token.Type) {
				case TokenType.Eof:
					return;
				case TokenType.None:
					break;
				case TokenType.StringLiteral:
				case TokenType.VerbatimStringLiteral:
				case TokenType.InterpolatedStringLiteral:
				case TokenType.CharLiteral:
				case TokenType.WhiteSpace:
				case TokenType.SingleLineComment:
				case TokenType.MultiLineComment:
					writer.Write (token.Value);
					break;
				case TokenType.UnknownChar:
					writer.Write (token.UnknownChar);
					break;
				case TokenType.RawLiteral:
					string replacementToken = null;

					switch (enumState) {
					case EnumState.None:
						if (token.Value == "enum")
							enumState = EnumState.Keyword;
						break;
					case EnumState.Keyword:
						enumState = EnumState.Name;
						break;
					case EnumState.Colon:
					case EnumState.LeftCurlyBracket:
						Profile.EnumBackingTypeReplacements.Evaluate (token.Value,
							out replacementToken);
						break;
					}

					if (replacementToken == null)
						Profile.GlobalReplacements.Evaluate (token.Value,
							out replacementToken);

					writer.Write (replacementToken ?? token.Value);
					break;
				case TokenType.LeftSquareBracket:
					writer.Write ("[");
					if (parser.ConsumedTokens.Count > 0)
						break;

					try {
						var exprList = parser.ParseExpressionList (
							TokenType.Comma, TokenType.RightSquareBracket);
						if (exprList != null) {
							exprList.AcceptVisitor (astVisitor);
							exprList.AcceptVisitor (csharpWriter);
						}

						parser.ConsumedTokens.Clear ();
						writer.Write ("]");
					} catch (Parser.SyntaxException) {
						// we will unwind the parsed tokens starting on the
						// next scan to blit them verbatim in the case that
						// we cannot parse them into a valid expression
					}

					break;
				default:
					if (enumState == EnumState.Name && token.Type == TokenType.Colon)
						enumState = EnumState.Colon;
					else if ((enumState == EnumState.Colon || enumState == EnumState.Keyword) &&
						token.Type == TokenType.LeftCurlyBracket)
						enumState = EnumState.LeftCurlyBracket;
					else if (enumState == EnumState.LeftCurlyBracket &&
						token.Type == TokenType.RightCurlyBracket)
						enumState = EnumState.None;

					writer.Write (token.Type.AsWritten ());
					break;
				}
			}
		}

		internal class PreprocessorVisitor : DepthFirstAstVisitor
		{
			readonly Preprocessor preprocessor;

			public PreprocessorVisitor (Preprocessor preprocessor)
			{
				if (preprocessor == null)
					throw new ArgumentNullException (nameof (preprocessor));

				this.preprocessor = preprocessor;
			}

			public override void VisitLiteralExpression (LiteralExpression literalExpression)
			{
				string replacement;
				if (preprocessor.Profile.GlobalReplacements.Evaluate (
					    literalExpression.Value,
					    out replacement))
					literalExpression.Value = replacement;
			}
		}
	}
}