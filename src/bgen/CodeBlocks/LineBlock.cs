using System.IO;

public class LineBlock : ICodeBlock {
	readonly string line;
	int currentIndent = 0;
	readonly string newLine = "\n";

	public LineBlock (string line)
	{
		this.line = line;
	}

	public void SetIndent (int indent)
	{
		currentIndent = indent;
	}

	public void Print (StreamWriter writer)
	{
		writer.Write (new string (' ', currentIndent) + line + newLine);
	}
}
