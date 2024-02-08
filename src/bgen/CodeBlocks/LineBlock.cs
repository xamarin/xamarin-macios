class LineBlock : ICodeBlock
{
	string line = string.Empty;
	int currentIndent = 0;

	public LineBlock(int currentIndent, string line)
	{
		this.line = line;
		this.currentIndent = currentIndent;
	}

	public void SetIndent(int indent)
	{
		currentIndent = indent;
	}

	public string Output()
	{
		return new string(' ', currentIndent) + line + "\n";
	}
}
