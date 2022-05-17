using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable 

namespace Xamarin.Utils {
	internal class StringUtils {
		static StringUtils ()
		{
			PlatformID pid = Environment.OSVersion.Platform;
			if (((int)pid != 128 && pid != PlatformID.Unix && pid != PlatformID.MacOSX))
				shellQuoteChar = '"'; // Windows
			else
				shellQuoteChar = '\''; // !Windows
		}

		static char shellQuoteChar;
		static char[] mustQuoteCharacters = new char [] { ' ', '\'', ',', '$', '\\' };
		static char [] mustQuoteCharactersProcess = { ' ', '\\', '"', '\'' };

		public static string[]? Quote (params string[] array)
		{
			if (array is null || array.Length == 0)
				return array;

			var rv = new string [array.Length];
			for (var i = 0; i < array.Length; i++)
				rv [i] = Quote (array [i]);
			return rv;
		}

		public static string Quote (string f)
		{
			if (String.IsNullOrEmpty (f))
				return f ?? String.Empty;

			if (f.IndexOfAny (mustQuoteCharacters) == -1)
				return f;

			var s = new StringBuilder ();

			s.Append (shellQuoteChar);
			foreach (var c in f) {
				if (c == '\'' || c == '"' || c == '\\')
					s.Append ('\\');

				s.Append (c);
			}
			s.Append (shellQuoteChar);

			return s.ToString ();
		}

		public static string[]? QuoteForProcess (IList<string> arguments)
		{
			if (arguments is null)
				return Array.Empty<string> ();
			return QuoteForProcess (arguments.ToArray ());
		}

		public static string[]? QuoteForProcess (params string [] array)
		{
			if (array is null || array.Length == 0)
				return array;

			var rv = new string [array.Length];
			for (var i = 0; i < array.Length; i++)
				rv [i] = QuoteForProcess (array [i]);
			return rv;
		}

		// Quote input according to how System.Diagnostics.Process needs it quoted.
		public static string QuoteForProcess (string f)
		{
			if (String.IsNullOrEmpty (f))
				return f ?? String.Empty;

			if (f.IndexOfAny (mustQuoteCharactersProcess) == -1)
				return f;

			var s = new StringBuilder ();

			s.Append ('"');
			foreach (var c in f) {
				if (c == '"') {
					s.Append ('\\');
				} else if (c == '\\') {
					s.Append (c);
				}
				s.Append (c);
			}
			s.Append ('"');

			return s.ToString ();
		}

		public static string FormatArguments (params string [] arguments)
		{
			return FormatArguments ((IList<string>) arguments);
		}

		public static string FormatArguments (IList<string> arguments)
		{
			return string.Join (" ", QuoteForProcess (arguments)!);
		}

		public static string? Unquote (string input)
		{
			if (input is null || input.Length == 0 || input [0] != shellQuoteChar)
				return input;

			var builder = new StringBuilder ();
			for (int i = 1; i < input.Length - 1; i++) {
				char c = input [i];
				if (c == '\\') {
					builder.Append (input [i + 1]);
					i++;
					continue;
				}
				builder.Append (input [i]);
			}
			return builder.ToString ();
		}

		public static bool TryParseArguments (string quotedArguments, out string []? argv, out Exception? ex)
		{
			var builder = new StringBuilder ();
			var args = new List<string> ();
			int i = 0, j;
			char c;

			while (i < quotedArguments.Length) {
				c = quotedArguments [i];
				if (c != ' ' && c != '\t') {
					if (GetArgument (builder, quotedArguments, i, out j, out ex) is string argument) {
						args.Add (argument);
						i = j;
					} else {
						argv = null;
						return false;
					}
				}

				i++;
			}

			argv = args.ToArray ();
			ex = null;

			return true;
		}

		static string? GetArgument (StringBuilder builder, string buf, int startIndex, out int endIndex, out Exception? ex)
		{
			bool escaped = false;
			char qchar, c = '\0';
			int i = startIndex;

			builder.Clear ();
			switch (buf [startIndex]) {
			case '\'': qchar = '\''; i++; break;
			case '"': qchar = '"'; i++; break;
			default: qchar = '\0'; break;
			}

			while (i < buf.Length) {
				c = buf [i];

				if (c == qchar && !escaped) {
					// unescaped qchar means we've reached the end of the argument
					i++;
					break;
				}

				if (c == '\\') {
					escaped = true;
				} else if (escaped) {
					builder.Append (c);
					escaped = false;
				} else if (qchar == '\0' && (c == ' ' || c == '\t')) {
					break;
				} else if (qchar == '\0' && (c == '\'' || c == '"')) {
					string sofar = builder.ToString ();

					if (GetArgument (builder, buf, i, out endIndex, out ex) is string embedded ) {
						i = endIndex;
						builder.Clear ();
						builder.Append (sofar);
						builder.Append (embedded);
						continue;
					}

					return null;

				} else {
					builder.Append (c);
				}

				i++;
			}

			if (escaped || (qchar != '\0' && c != qchar)) {
				ex = new FormatException (escaped ? "Incomplete escape sequence." : "No matching quote found.");
				endIndex = -1;
				return null;
			}

			endIndex = i;
			ex = null;

			return builder.ToString ();
		}
		
		// Version.Parse requires, minimally, both major and minor parts.
		// However we want to accept `11` as `11.0`
		public static Version ParseVersion (string v)
		{
			int major;
			if (int.TryParse (v, out major))
				return new Version (major, 0);
			return Version.Parse (v);
		}
	}

	static class StringExtensions
	{
		internal static string [] SplitLines (this string s) => s.Split (new [] { Environment.NewLine }, StringSplitOptions.None);

		// Adds an element to an array and returns a new array with the added element.
		// The original array is not modified.
		// If the original array is null, a new array is also created, with just the new value.
		internal static T [] CopyAndAdd<T>(this T[] array, T value)
		{
			if (array is null || array.Length == 0)
				return new T [] { value };
			var tmpArray = array;
			Array.Resize (ref array, array.Length + 1);
			tmpArray[tmpArray.Length - 1] = value;
			return tmpArray;
		}
	}
}
