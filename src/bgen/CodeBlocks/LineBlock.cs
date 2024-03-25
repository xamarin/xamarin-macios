using System.IO;

public class LineBlock : ICodeBlock {
	readonly string line;
	int currentIndent = 0;
	const char newLine = '\n';

	public LineBlock (string line)
	{
		this.line = line;
	}

	public void SetIndent (int indent)
	{
		currentIndent = indent;
	}

	public void WriteIndent (TextWriter writer)
	{
		for (var i = 0; i < currentIndent; i++)
			writer.Write (' ');
	}

	public void Print (TextWriter writer)
	{
		WriteIndent (writer);
		writer.Write (line);
		writer.Write (newLine);
	}
}
