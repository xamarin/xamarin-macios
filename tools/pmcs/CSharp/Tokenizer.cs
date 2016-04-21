//
// Tokenizer.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Xamarin.Pmcs.CSharp
{
	public unsafe class Tokenizer
	{
		readonly TextReader reader;
		readonly char *tokenBuffer;
#if FIND_RANDOM_CRASHES
		int tokenBufferSize;
		int _tokenLength;
		int tokenLength {
			get {
				return _tokenLength;
			}
			set {
				_tokenLength = value;
				if (_tokenLength > tokenBufferSize)
					throw new Exception (string.Format ("Crash is imminent! path: {0} line: {1} tokenLength: {2} tokenBufferSize: {3}", Path, Line, _tokenLength, tokenBufferSize));
			}
		}
#else
		int tokenLength;
#endif

		public int Line { get; private set; }
		public string Path { get; set; }

		public unsafe Tokenizer (TextReader reader, char *tokenBuffer, int tokenBufferSize)
		{
			if (reader == null)
				throw new ArgumentNullException (nameof (reader));

			this.reader = reader;
			this.tokenBuffer = tokenBuffer;
#if FIND_RANDOM_CRASHES
			this.tokenBufferSize = tokenBufferSize;
#endif
		}

		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		bool IsRawLiteralTokenStartChar (char c, char p)
		{
			return Char.IsLetterOrDigit (c) || c == '_' || c == '@' || (
			        c == '.' && !Char.IsWhiteSpace (p) && p != Char.MaxValue);
		}

		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		bool IsRawLiteralTokenChar (char c)
		{
			if (c == '_' || c == '.' || Char.IsLetterOrDigit (c))
				return true;

			if (c == ':' && (tokenLength == 6 || tokenLength == 7)) {
				if (tokenBuffer [0] == 'g' &&
				    tokenBuffer [1] == 'l' &&
				    tokenBuffer [2] == 'o' &&
				    tokenBuffer [3] == 'b' &&
				    tokenBuffer [4] == 'a' &&
				    tokenBuffer [5] == 'l') {
					return tokenLength != 7 || tokenBuffer [6] == ':';
				}
			}

			return false;
		}

		public Token Scan ()
		{
			tokenLength = 0;
			var c = (char)reader.Read ();
			var p = (char)reader.Peek ();

			if (Char.IsWhiteSpace (c)) {
				tokenBuffer [tokenLength++] = c;
				while (Char.IsWhiteSpace (p = (char)reader.Peek ())) {
					if (reader.Read () == '\n')
						Line++;
					tokenBuffer [tokenLength++] = p;
				}
				return new Token (TokenType.WhiteSpace, tokenBuffer, tokenLength);
			} else if (c == '/' && p == '/') {
				reader.Read ();
				tokenBuffer [tokenLength++] = c;
				tokenBuffer [tokenLength++] = p;
				while (reader.Peek () != '\n' && reader.Peek () != -1)
					tokenBuffer [tokenLength++] = (char)reader.Read ();
				Line++;
				return new Token (TokenType.SingleLineComment, tokenBuffer, tokenLength);
			} else if (c == '/' && p == '*') {
				reader.Read ();
				tokenBuffer [tokenLength++] = c;
				tokenBuffer [tokenLength++] = p;
				while ((c = (char)reader.Read ()) != Char.MaxValue) {
					p = (char)reader.Peek ();
					if (c == '*' && p == '/') {
						reader.Read ();
						tokenBuffer [tokenLength++] = c;
						tokenBuffer [tokenLength++] = p;
						break;
					}

					tokenBuffer [tokenLength++] = c;
				}

				return new Token (TokenType.MultiLineComment, tokenBuffer, tokenLength);
			} else if (c == '\'' || c == '"' || ((c == '@' || c == '$') && p == '"')) {
				var terminator = '"';
				var escape = '\\';
				TokenType type;

				switch (c) {
				case '\'':
					type = TokenType.CharLiteral;
					terminator = '\'';
					break;
				case '"':
					type = TokenType.StringLiteral;
					break;
				case '@':
					escape = '"';
					type = TokenType.VerbatimStringLiteral;
					tokenBuffer [tokenLength++] = c;
					c = (char)reader.Read ();
					break;
				case '$':
					type = TokenType.InterpolatedStringLiteral;
					tokenBuffer [tokenLength++] = c;
					c = (char)reader.Read ();
					break;
				default:
					throw new Exception ("should not be reached");
				}

				tokenBuffer [tokenLength++] = c;
				while (reader.Peek () != -1) {
					tokenBuffer [tokenLength++] = c = (char)reader.Read ();
					if (c == '\n')
						Line++;
					p = (char)reader.Peek ();
					if (c == escape && (p == escape || p == terminator))
						tokenBuffer [tokenLength++] = (char)reader.Read ();
					else if (c == terminator)
						break;
				}

				return new Token (type, tokenBuffer, tokenLength);
			} else if (IsRawLiteralTokenStartChar (c, p)) {
				tokenBuffer [tokenLength++] = c;
				while (IsRawLiteralTokenChar ((char)reader.Peek ()))
					tokenBuffer [tokenLength++] = (char)reader.Read ();
				return new Token (TokenType.RawLiteral, tokenBuffer, tokenLength);
			}

			switch (c) {
			case '=':
				if (p == '=') {
					reader.Read ();
					return new Token (TokenType.Equal);
				}

				return new Token (TokenType.Assign);
			case '!':
				if (p == '=') {
					reader.Read ();
					return new Token (TokenType.NotEqual);
				}

				return new Token (TokenType.Not);
			case '<':
				if (p == '<') {
					reader.Read ();
					if (reader.Peek () == '=') {
						reader.Read ();
						return new Token (TokenType.LeftShiftAssign);
					}

					return new Token (TokenType.LeftShift);
				} else if (p == '=') {
					reader.Read ();
					return new Token (TokenType.LessThanOrEqual);
				}

				return new Token (TokenType.LessThan);
			case '>':
				if (p == '>') {
					reader.Read ();
					if (reader.Peek () == '=') {
						reader.Read ();
						return new Token (TokenType.RightShiftAssign);
					}

					return new Token (TokenType.RightShift);
				} else if (p == '=') {
					reader.Read ();
					return new Token (TokenType.GreaterThanOrEqual);
				}

				return new Token (TokenType.GreaterThan);
			case '&':
				if (p == '&') {
					reader.Read ();
					return new Token (TokenType.AndAlso);
				} else if (p == '=') {
					reader.Read ();
					return new Token (TokenType.AndAssign);
				}

				return new Token (TokenType.And);
			case '|':
				if (p == '|') {
					reader.Read ();
					return new Token (TokenType.OrElse);
				} else if (p == '=') {
					reader.Read ();
					return new Token (TokenType.OrAssign);
				}

				return new Token (TokenType.Or);
			case '^':
				if (p == '=') {
					reader.Read ();
					return new Token (TokenType.ExclusiveOrAssign);
				}

				return new Token (TokenType.ExclusiveOr);
			case '+':
				if (p == '=') {
					reader.Read ();
					return new Token (TokenType.AddAssign);
				} else if (p == '+') {
					reader.Read ();
					return new Token (TokenType.Increment);
				}

				return new Token (TokenType.Add);
			case '-':
				if (p == '=') {
					reader.Read ();
					return new Token (TokenType.SubtractAssign);
				} else if (p == '-') {
					reader.Read ();
					return new Token (TokenType.Decrement);
				}

				return new Token (TokenType.Subtract);
			case '*':
				if (p == '=') {
					reader.Read ();
					return new Token (TokenType.MultiplyAssign);
				}

				return new Token (TokenType.Multiply);
			case '/':
				if (p == '=') {
					reader.Read ();
					return new Token (TokenType.DivideAssign);
				}

				return new Token (TokenType.Divide);
			case '%':
				if (p == '=') {
					reader.Read ();
					return new Token (TokenType.ModuloAssign);
				}

				return new Token (TokenType.Modulo);
			case '~':
				return new Token (TokenType.OnesComplement);
			case '(':
				return new Token (TokenType.LeftParen);
			case ')':
				return new Token (TokenType.RightParen);
			case '{':
				return new Token (TokenType.LeftCurlyBracket);
			case '}':
				return new Token (TokenType.RightCurlyBracket);
			case '[':
				return new Token (TokenType.LeftSquareBracket);
			case ']':
				return new Token (TokenType.RightSquareBracket);
			case '?':
				if (p == '?') {
					reader.Read ();
					return new Token (TokenType.DoubleQuestionMark);
				}

				return new Token (TokenType.QuestionMark);
			case ':':
				return new Token (TokenType.Colon);
			case ';':
				return new Token (TokenType.Semicolon);
			case '.':
				return new Token (TokenType.Period);
			case ',':
				return new Token (TokenType.Comma);
			default:
				if (c == Char.MaxValue)
					return new Token (TokenType.Eof);

				return new Token (TokenType.UnknownChar, c);
			}
		}
	}
}