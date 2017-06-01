using System;
using System.Text;

namespace Xamarin.Utils {
	public class StringUtils {

		static char shellQuoteChar;
		static char ShellQuoteChar {
			get {
				if (shellQuoteChar == '\0') {
					PlatformID pid = Environment.OSVersion.Platform;
					if (((int)pid != 128 && pid != PlatformID.Unix && pid != PlatformID.MacOSX))
						shellQuoteChar = '"'; // Windows
					else
						shellQuoteChar = '\''; // !Windows
				}

				return shellQuoteChar;
			}
		}

		public static string Quote (string f)
		{
			if (String.IsNullOrEmpty (f))
				return f ?? String.Empty;

			if (f.IndexOf (' ') == -1 && f.IndexOf ('\'') == -1 && f.IndexOf (',') == -1 && f.IndexOf ('$') == -1)
				return f;

			var s = new StringBuilder ();

			s.Append (ShellQuoteChar);
			foreach (var c in f) {
				if (c == '\'' || c == '"' || c == '\\')
					s.Append ('\\');

				s.Append (c);
			}
			s.Append (ShellQuoteChar);

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

	}
}