using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Tests.Templating
{
	public interface IReplacement
	{
		string Apply (string text);
	}

	public class Replacement : IReplacement
	{
		string Token;
		string Value;

		public Replacement (string token, string value)
		{
			Token = token;
			Value = value;
		}

		static public Replacement Create (string token, string value) => new Replacement (token, value);

		public string Apply (string text) => text.Replace (Token, Value);
	}

	public class ReplacementGroup : IReplacement
	{
		List<Replacement> Replacements;

		public ReplacementGroup () : this (new List<Replacement> ())
		{
		}

		public ReplacementGroup (IEnumerable<Replacement> replacements)
		{
			Replacements = replacements.ToList ();
		}

		static public ReplacementGroup Create (params Replacement[] replacements) => new ReplacementGroup (replacements);

		public string Apply (string text)
		{
			foreach (var r in Replacements)
				text = r.Apply (text);
			return text;
		}

		public void Append (Replacement replacement) => Replacements.Add (replacement);
	}

}
