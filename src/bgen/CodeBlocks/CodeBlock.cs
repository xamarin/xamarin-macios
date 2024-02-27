using System.Collections.Generic;
using System.IO;

public class CodeBlock : ICodeBlock {
	protected int CurrentIndent = 0;
	protected int Indent = 4;
	protected string HeaderText = string.Empty;
	protected List<ICodeBlock> Blocks = new ();
	const char startBrace = '{';
	const char endBrace = '}';
	public const char NewLine = '\n';

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

	public CodeBlock (string headerText, params string [] lines)
	{
		HeaderText = headerText;
		foreach (string line in lines) {
			Blocks.Add (new LineBlock (line));
		}
	}

	public CodeBlock (params string [] lines)
	{
		foreach (string line in lines) {
			Blocks.Add (new LineBlock (line));
		}
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

	public void WriteIndent (TextWriter writer)
	{
		for (var i = 0; i < CurrentIndent; i++)
			writer.Write (' ');
	}

	public virtual void WriteHeaderText (TextWriter writer)
	{
		WriteIndent (writer);
		writer.Write (HeaderText);
		writer.Write (NewLine);
	}

	public virtual void Print (TextWriter writer)
	{
		if (HeaderText != string.Empty)
			WriteHeaderText (writer);

		WriteIndent (writer);
		writer.Write (startBrace);
		writer.Write (NewLine);

		SetIndent (CurrentIndent + Indent);
		foreach (ICodeBlock block in Blocks) {
			block.SetIndent (CurrentIndent);
			block.Print (writer);
		}
		SetIndent (CurrentIndent - Indent);

		WriteIndent (writer);
		writer.Write (endBrace);
		writer.Write (NewLine);
	}

	public void SetIndent (int indent)
	{
		CurrentIndent = indent;
	}
}
