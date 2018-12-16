using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BCLTests {

	/// <summary>
	/// Class that can parser a file/stream with the ignored tests and will
	/// return a list of the ignored tests.
	/// </summary>
	public static class IgnoreFileParser {

		public static async Task<string[]> ParseStream (TextReader textReader)
		{
			var ignoredMethods = new List<string> ();
			string line;
			while ((line = await textReader.ReadLineAsync()) != null) {
				// we have to make sure of several things, first, lets
				// remove any char after the first # which would mean
				// we have comments:
				var pos = line.IndexOf ('#');
				if (pos > -1) {
					line = line.Remove (pos);
				}
				line = line.Trim ();
				
				// continue if the line was empty or was just a comment
				if (string.IsNullOrEmpty (line))
					continue;
				// we removed all comments or empty spaces
				ignoredMethods.Add (line);
			}
			return ignoredMethods.ToArray ();
		}
	}
}