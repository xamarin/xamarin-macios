using System.Collections.Generic;
using System.IO;

public class CodeBlock : ICodeBlock {
	protected int CurrentIndent = 0;
	protected int Indent = 4;
	protected string HeaderText = string.Empty;
	protected List<ICodeBlock> Blocks = new ();
	readonly string startBrace = "{";
	readonly string endBrace = "}";
	public readonly string newLine = "\n";

	public CodeBlock () { }

	public CodeBlock (string text)
	{
		HeaderText = text;
	}

	public CodeBlock (string text, List<ICodeBlock> blocks)
	{
		this.Blocks = blocks;
		HeaderText = text;
	}

	public CodeBlock (List<ICodeBlock> blocks)
	{
		this.Blocks = blocks;
	}

	public void AddBlock (ICodeBlock block)
	{
		block.SetIndent (CurrentIndent + Indent);
		Blocks.Add (block);
	}

	public void AddLine (string text)
	{
		LineBlock line = new LineBlock (text);
		line.SetIndent (CurrentIndent + Indent);
		Blocks.Add (line);
	}

	protected void DecrementIndent ()
	{
		CurrentIndent -= Indent;
	}

	public virtual void Print (StreamWriter writer)
	{
		if (HeaderText != string.Empty)
			writer.Write (new string (' ', CurrentIndent) + HeaderText + newLine);

		writer.Write (new string (' ', CurrentIndent) + startBrace + newLine);
		SetIndent (CurrentIndent + Indent);
		foreach (ICodeBlock block in Blocks) {
			block.SetIndent (CurrentIndent);
			block.Print (writer);
		}
		SetIndent (CurrentIndent - Indent);
		writer.Write (new string (' ', CurrentIndent) + endBrace + newLine);
	}

	public void SetIndent (int indent)
	{
		CurrentIndent = indent;
	}
}
