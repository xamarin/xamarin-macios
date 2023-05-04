using System.Text;

#nullable enable

namespace Registrar {
	abstract partial class Registrar {
		internal static string? CreateSetterSelector (string? getterSelector)
		{
#if NET
			if (string.IsNullOrEmpty (getterSelector))
#else
			if (getterSelector is null || string.IsNullOrEmpty (getterSelector))
#endif
				return getterSelector;

			var first = (int) getterSelector [0];
			// Objective-C uses the native 'toupper' function, which only handles a-z and translates them to A-Z.
			if (first >= 'a' && first <= 'z')
				first = (char) (first - 32 /* 'a' - 'A' */);
			return "set" + ((char) first).ToString () + getterSelector.Substring (1) + ":";
		}

		public static string SanitizeObjectiveCName (string name)
		{
			StringBuilder? sb = null;

			for (int i = 0; i < name.Length; i++) {
				var ch = name [i];
				switch (ch) {
				case '.':
				case '+':
				case '/':
				case '`':
				case '@':
				case '<':
				case '>':
				case '$':
				case '-':
					if (sb is null)
						sb = new StringBuilder (name, 0, i, name.Length);
					sb.Append ('_');
					break;
				default:
					if (sb is not null)
						sb.Append (ch);
					break;
				}
			}

			if (sb is not null)
				return sb.ToString ();

			return name;
		}
	}
}
