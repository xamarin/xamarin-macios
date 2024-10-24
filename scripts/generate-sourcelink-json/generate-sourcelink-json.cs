using System.IO;
using System.Text;

var idx = 0;
var latestCommit = args [idx++];
var src = args [idx++];
var outputPath = args [idx++];

using (var writer = new StreamWriter (outputPath)) {
	writer.WriteLine ("{");
	writer.WriteLine ("  \"documents\": {");
	writer.WriteLine ($"    \"{src}*\": \"https://raw.githubusercontent.com/xamarin/xamarin-macios/{latestCommit}/src*\"");
	writer.WriteLine ("  }");
	writer.WriteLine ("}");
}
