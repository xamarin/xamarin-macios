using System.Collections.Generic;
using System.IO;

public class CodeBlock : ICodeBlock
{
	protected int CurrentIndent;
	protected int Indent = 4;
	protected string HeaderText = string.Empty;
	protected List<ICodeBlock> Blocks = new();
	readonly string startBrace = "{";
	readonly string endBrace = "}";
	public readonly string newLine = "\n";

	public CodeBlock(int currentIndent)
	{
		this.CurrentIndent = currentIndent;
	}

	public CodeBlock(int currentIndent, string text)
	{
		this.CurrentIndent = currentIndent;
		HeaderText = text;
	}

	public void AddBlock(ICodeBlock block)
	{
		block.SetIndent(CurrentIndent + Indent);
		Blocks.Add(block);
	}

	public void AddLine(string text)
	{
		LineBlock line = new LineBlock(CurrentIndent + Indent, text);
		Blocks.Add(line);
	}

	protected void DecrementIndent()
	{
		CurrentIndent -= Indent;
	}

	public virtual void Print (StreamWriter writer)
	{
		if (HeaderText != string.Empty)
			writer.Write(new string(' ', CurrentIndent) + HeaderText + newLine);

		writer.Write(new string(' ', CurrentIndent) + startBrace + newLine);
		SetIndent(CurrentIndent + Indent);
		foreach (ICodeBlock block in Blocks)
		{
			block.SetIndent(CurrentIndent);
			block.Print(writer);
		}
		SetIndent(CurrentIndent - Indent);
		writer.Write(new string(' ', CurrentIndent) + endBrace + newLine);
	}

	public void SetIndent(int indent)
	{
		CurrentIndent = indent;
	}
}
