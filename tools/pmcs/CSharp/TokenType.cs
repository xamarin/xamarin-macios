//
// TokenType.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.

namespace Xamarin.Pmcs.CSharp
{
	public static class TokenTypeExtensions
	{
		static readonly string [] tokenTypeMap;

		static TokenTypeExtensions ()
		{
			tokenTypeMap = new string [(int)TokenType.LastToken];

			tokenTypeMap [(int)TokenType.LeftParen] = "(";
			tokenTypeMap [(int)TokenType.RightParen] = ")";
			tokenTypeMap [(int)TokenType.LeftSquareBracket] = "[";
			tokenTypeMap [(int)TokenType.RightSquareBracket] = "]";
			tokenTypeMap [(int)TokenType.LeftCurlyBracket] = "{";
			tokenTypeMap [(int)TokenType.RightCurlyBracket] = "}";
			tokenTypeMap [(int)TokenType.Period] = ".";
			tokenTypeMap [(int)TokenType.Comma] = ",";
			tokenTypeMap [(int)TokenType.Colon] = ":";
			tokenTypeMap [(int)TokenType.Semicolon] = ";";
			tokenTypeMap [(int)TokenType.QuestionMark] = "?";
			tokenTypeMap [(int)TokenType.DoubleQuestionMark] = "??";
			tokenTypeMap [(int)TokenType.Increment] = "++";
			tokenTypeMap [(int)TokenType.Decrement] = "--";
			tokenTypeMap [(int)TokenType.Add] = "+";
			tokenTypeMap [(int)TokenType.Subtract] = "-";
			tokenTypeMap [(int)TokenType.Not] = "!";
			tokenTypeMap [(int)TokenType.OnesComplement] = "~";
			tokenTypeMap [(int)TokenType.Multiply] = "*";
			tokenTypeMap [(int)TokenType.Divide] = "/";
			tokenTypeMap [(int)TokenType.Modulo] = "%";
			tokenTypeMap [(int)TokenType.LeftShift] = "<<";
			tokenTypeMap [(int)TokenType.RightShift] = ">>";

			tokenTypeMap [(int)TokenType.LessThan] = "<";
			tokenTypeMap [(int)TokenType.GreaterThan ] = ">";
			tokenTypeMap [(int)TokenType.LessThanOrEqual] = "<=";
			tokenTypeMap [(int)TokenType.GreaterThanOrEqual] = ">=";

			tokenTypeMap [(int)TokenType.Equal] = "==";
			tokenTypeMap [(int)TokenType.NotEqual] = "!=";

			tokenTypeMap [(int)TokenType.And] = "&";
			tokenTypeMap [(int)TokenType.ExclusiveOr] = "^";
			tokenTypeMap [(int)TokenType.Or] = "|";

			tokenTypeMap [(int)TokenType.AndAlso] = "&&";
			tokenTypeMap [(int)TokenType.OrElse] = "||";

			tokenTypeMap [(int)TokenType.Assign] = "=";
			tokenTypeMap [(int)TokenType.MultiplyAssign] = "*=";
			tokenTypeMap [(int)TokenType.DivideAssign] = "/=";
			tokenTypeMap [(int)TokenType.ModuloAssign] = "%=";
			tokenTypeMap [(int)TokenType.AddAssign] = "+=";
			tokenTypeMap [(int)TokenType.SubtractAssign] = "-=";
			tokenTypeMap [(int)TokenType.LeftShiftAssign] = "<<=";
			tokenTypeMap [(int)TokenType.RightShiftAssign] = ">>=";
			tokenTypeMap [(int)TokenType.AndAssign] = "&=";
			tokenTypeMap [(int)TokenType.ExclusiveOrAssign] = "^=";
			tokenTypeMap [(int)TokenType.OrAssign] = "|=";
		}

		public static string AsWritten (this TokenType tokenType)
		{
			return tokenTypeMap [(int)tokenType];
		}
	}

	public enum TokenType : byte
	{
		None,
		StringLiteral,
		VerbatimStringLiteral,
		InterpolatedStringLiteral,
		CharLiteral,
		WhiteSpace,
		SingleLineComment,
		MultiLineComment,
		RawLiteral,
		UnknownChar,
		Eof,

		LeftParen,
		RightParen,
		LeftCurlyBracket,
		RightCurlyBracket,
		LeftSquareBracket,
		RightSquareBracket,
		Period,
		Comma,
		Colon,
		Semicolon,
		QuestionMark,
		DoubleQuestionMark,
		Increment,
		Decrement,
		Add,
		Subtract,
		Not,
		OnesComplement,
		Multiply,
		Divide,
		Modulo,
		LeftShift,
		RightShift,

		LessThan,
		GreaterThan,
		LessThanOrEqual,
		GreaterThanOrEqual,

		Equal,
		NotEqual,

		And,
		ExclusiveOr,
		Or,

		AndAlso,
		OrElse,

		Assign,
		MultiplyAssign,
		DivideAssign,
		ModuloAssign,
		AddAssign,
		SubtractAssign,
		LeftShiftAssign,
		RightShiftAssign,
		AndAssign,
		ExclusiveOrAssign,
		OrAssign,

		LastToken
	}
}