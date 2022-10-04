using System;
using System.Text;
using System.Collections.Generic;

namespace Xamarin.MacDev {
	public static class StringParserService {
		public static string Parse (string text, IDictionary<string, string> tags)
		{
			var builder = new StringBuilder ();

			for (int i = 0; i < text.Length; i++) {
				if (text [i] != '$' || i + 1 >= text.Length) {
					builder.Append (text [i]);
					continue;
				}

				i++;

				if ((text [i] == '(' || text [i] == '{') && i + 2 < text.Length) {
					char open = text [i];
					char close = open == '(' ? ')' : '}';
					int startIndex = ++i;

					while (i < text.Length && text [i] != close)
						i++;

					if (text [i] != close) {
						builder.Append ('$').Append (open);
						i = startIndex - 1;
						continue;
					}

					var tag = text.Substring (startIndex, i - startIndex);
					string value;

					if (!tags.TryGetValue (tag, out value)) {
						builder.Append ('$').Append (open).Append (tag).Append (close);
					} else {
						builder.Append (value);
					}
				} else {
					builder.Append ('$').Append (text [i]);
				}
			}

			return builder.ToString ();
		}
	}
}
