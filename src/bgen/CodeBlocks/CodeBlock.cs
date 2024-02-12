using System.Collections.Generic;

public class CodeBlock : ICodeBlock {
	protected int CurrentIndent;
	protected int Indent = 4;
	protected string HeaderText = string.Empty;
	protected List<ICodeBlock> Blocks = new ();
	readonly string startBrace = "{";
	readonly string endBrace = "}";
	public readonly string newLine = "\n";

	public CodeBlock (int currentIndent)
	{
		this.CurrentIndent = currentIndent;
	}

	public CodeBlock (int currentIndent, string text)
	{
		this.CurrentIndent = currentIndent;
		HeaderText = text;
	}

	public void AddBlock (ICodeBlock block)
	{
		block.SetIndent (CurrentIndent + Indent);
		Blocks.Add (block);
	}

	public void AddLine (string text)
	{
		LineBlock line = new LineBlock (CurrentIndent + Indent, text);
		Blocks.Add (line);
	}

	protected void DecrementIndent ()
	{
		CurrentIndent -= Indent;
	}

	public virtual string Print ()
	{
		string s = string.Empty;
		if (HeaderText != string.Empty)
			s = new string (' ', CurrentIndent) + HeaderText + newLine;

		s += new string (' ', CurrentIndent) + startBrace + newLine;
		SetIndent (CurrentIndent + Indent);
		foreach (ICodeBlock block in Blocks) {
			block.SetIndent (CurrentIndent);
			s += block.Print ();
		}
		SetIndent (CurrentIndent - Indent);
		s += new string (' ', CurrentIndent) + endBrace + newLine;
		return s;
	}

	public void SetIndent (int indent)
	{
		CurrentIndent = indent;
	}
}
