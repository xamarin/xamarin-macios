using System.Collections.Generic;

public class CodeBlock : ICodeBlock
{
	public int currentIndent;
	public int indent = 4;

	public string headerText = string.Empty;

	public List<ICodeBlock> blocks = new();
	public string startBrace = "{"; // Default value for lines
	public string endBrace = "}"; // Default value for lines

	public CodeBlock(int currentIndent)
	{
		this.currentIndent = currentIndent;
	}

	public CodeBlock(int currentIndent, string text) // TODO
	{
		this.currentIndent = currentIndent;
		headerText = text;
	}

	public void AddBlock(ICodeBlock block)
	{
		block.SetIndent(currentIndent + indent);
		blocks.Add(block);
	}

	public void AddLine(string text)
	{
		LineBlock line = new LineBlock(currentIndent + indent, text);
		blocks.Add(line);
	}

	protected void DecrementIndent()
	{
		currentIndent -= indent;
	}

	public virtual string Output()
	{
		// TODO logic for not using braces if line is just 1
		string s = string.Empty;
		if (headerText != string.Empty)
			s = new string(' ', currentIndent) + headerText + "\n";

		s += new string(' ', currentIndent) + startBrace + "\n";
		SetIndent(currentIndent + indent);
		foreach (ICodeBlock block in blocks)
		{
			block.SetIndent(currentIndent);
			s += block.Output();
		}
		SetIndent(currentIndent - indent);
		s += new string(' ', currentIndent) + endBrace + "\n";
		return s;
	}

	public void SetIndent(int indent)
	{
		currentIndent = indent;
	}
}
