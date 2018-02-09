
namespace Registrar {
	abstract partial class Registrar {
		internal static string CreateSetterSelector (string getterSelector)
		{
			if (string.IsNullOrEmpty (getterSelector))
				return getterSelector;

			var first = (int) getterSelector [0];
			// Objective-C uses the native 'toupper' function, which only handles a-z and translates them to A-Z.
			if (first >= 'a' && first <= 'z')
				first = (char) (first - 32 /* 'a' - 'A' */);
			return "set" + ((char) first).ToString () + getterSelector.Substring (1) + ":";
		}
	}
}
