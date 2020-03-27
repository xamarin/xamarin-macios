using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Utilities {
	public class StringUtils {
		static readonly char shellQuoteChar;
		static readonly char [] mustQuoteCharacters = new char [] { ' ', '\'', ',', '$', '\\' };
		static readonly char [] mustQuoteCharactersProcess = { ' ', '\\', '"', '\'' };

		static StringUtils ()
		{
			PlatformID pid = Environment.OSVersion.Platform;
			if ((int) pid != 128 && pid != PlatformID.Unix && pid != PlatformID.MacOSX)
				shellQuoteChar = '"'; // Windows
			else
				shellQuoteChar = '\''; // !Windows
		}

		public static string FormatArguments (params string [] arguments)
		{
			return FormatArguments ((IList<string>) arguments);
		}

		public static string FormatArguments (IList<string> arguments)
		{
			return string.Join (" ", QuoteForProcess (arguments));
		}

		static string [] QuoteForProcess (params string [] array)
		{
			if (array == null || array.Length == 0)
				return array;

			var rv = new string [array.Length];
			for (var i = 0; i < array.Length; i++)
				rv [i] = QuoteForProcess (array [i]);
			return rv;
		}

		public static string Quote (string f)
		{
			if (string.IsNullOrEmpty (f))
				return f ?? string.Empty;

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

		// Quote input according to how System.Diagnostics.Process needs it quoted.
		static string QuoteForProcess (string f)
		{
			if (string.IsNullOrEmpty (f))
				return f ?? string.Empty;

			if (f.IndexOfAny (mustQuoteCharactersProcess) == -1)
				return f;

			var s = new StringBuilder ();

			s.Append ('"');
			foreach (var c in f) {
				if (c == '"') {
					s.Append ('\\');
					s.Append (c).Append (c);
				} else if (c == '\\') {
					s.Append (c);
				}
				s.Append (c);
			}
			s.Append ('"');

			return s.ToString ();
		}

		static string [] QuoteForProcess (IList<string> arguments)
		{
			if (arguments == null)
				return Array.Empty<string> ();
			return QuoteForProcess (arguments.ToArray ());
		}
	}
}
