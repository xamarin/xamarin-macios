using System;
using System.Text;

namespace Xamarin.Utils {
	public class StringUtils {
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

		public static string Unquote (string input)
		{
			if (input == null || input.Length == 0 || input [0] != shellQuoteChar)
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
		public static string [] SplitLines (this string s) => s.Split (new [] { Environment.NewLine }, StringSplitOptions.None);
	}
}
