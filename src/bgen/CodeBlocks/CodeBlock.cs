using System.Collections.Generic;
using System.IO;

public class CodeBlock : ICodeBlock {
	protected int CurrentIndent = 0;
	protected int Indent = 4;
	protected string HeaderText = string.Empty;
	protected List<ICodeBlock> Blocks = new ();
	readonly string startBrace = "{";
	readonly string endBrace = "}";
	public readonly string NewLine = "\n";

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

	public virtual void Print (TextWriter writer)
	{
		if (HeaderText != string.Empty)
			writer.Write (new string (' ', CurrentIndent) + HeaderText + NewLine);

		writer.Write (new string (' ', CurrentIndent) + startBrace + NewLine);
		SetIndent (CurrentIndent + Indent);
		foreach (ICodeBlock block in Blocks) {
			block.SetIndent (CurrentIndent);
			block.Print (writer);
		}
		SetIndent (CurrentIndent - Indent);
		writer.Write (new string (' ', CurrentIndent) + endBrace + NewLine);
	}

	public void SetIndent (int indent)
	{
		CurrentIndent = indent;
	}
}
