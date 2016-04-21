//
// Token.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.

using System;

namespace Xamarin.Pmcs.CSharp
{
	[Flags]
	public enum TokenFlags : byte
	{
		None = 0,
		MaybeCast = 1,
		IdentifierConstant = 2
	}

	public struct Token
	{
		public TokenType Type { get; private set; }
		public TokenFlags Flags { get; private set; }
		public string Value { get; private set; }
		public char UnknownChar { get; private set; }

		public unsafe Token (TokenType type, char *valueBuffer, int valueLength)
		{
			Type = type;
			Value = new string (valueBuffer, 0, valueLength);
			Flags = TokenFlags.None;
			UnknownChar = default (char);
		}

		public Token (TokenType type, string value = null)
		{
			Type = type;
			Value = value;
			Flags = TokenFlags.None;
			UnknownChar = default (char);
		}

		public Token (TokenType type, char unknownChar)
		{
			Type = type;
			Value = null;
			Flags = TokenFlags.None;
			UnknownChar = unknownChar;
		}

		public override string ToString ()
		{
			if (Type == TokenType.WhiteSpace)
				return String.Format ("<{0}: {1}>", Type, Value
					.Replace ("\n", "\\n")
					.Replace ("\t", "\\t")
					.Replace ("\r", "\\r")
				);
			else if (Type == TokenType.UnknownChar)
				return String.Format ("<{0}: {1}>", Type, UnknownChar);
			else if (Value == null)
				return String.Format ("<{0}: '{1}'>", Type, Type.AsWritten ());
			else
				return String.Format ("<{0}: {1}>", Type, Value);
		}
	}
}