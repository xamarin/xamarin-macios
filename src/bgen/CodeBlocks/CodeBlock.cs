using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CodeBlock : ICodeBlock, IEnumerable<ICodeBlock> {
	protected int CurrentIndent = 0;
	protected int Indent = 4;
	protected string HeaderText = string.Empty;
	protected List<ICodeBlock> Blocks = new ();
	protected const char StartBrace = '{';
	protected const char EndBrace = '}';
	protected const char NewLine = '\n';

	public CodeBlock () { }

	public CodeBlock (string text)
	{
		HeaderText = text;
	}

	public CodeBlock (string text, IList<ICodeBlock> blocks)
	{
		Blocks.AddRange (blocks);
		HeaderText = text;
	}

	public CodeBlock (IList<ICodeBlock> blocks)
	{
		Blocks.AddRange (blocks);
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

	public void Add (ICodeBlock block)
	{
		block.SetIndent (CurrentIndent + Indent);
		Blocks.Add (block);
	}

	public void Add (string text)
	{
		LineBlock line = new LineBlock (text);
		line.SetIndent (CurrentIndent + Indent);
		Blocks.Add (line);
	}

	public void AddLine (string text)
	{
		LineBlock line = new LineBlock (text);
		line.SetIndent (CurrentIndent + Indent);
		Blocks.Add (line);
	}

	protected void WriteIndent (TextWriter writer)
	{
		for (var i = 0; i < CurrentIndent; i++)
			writer.Write (' ');
	}

	protected virtual void WriteHeaderText (TextWriter writer)
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
		writer.Write (StartBrace);
		writer.Write (NewLine);

		SetIndent (CurrentIndent + Indent);
		foreach (ICodeBlock block in Blocks) {
			block.SetIndent (CurrentIndent);
			block.Print (writer);
		}
		SetIndent (CurrentIndent - Indent);

		WriteIndent (writer);
		writer.Write (EndBrace);
		writer.Write (NewLine);
	}

	public void SetIndent (int indent)
	{
		CurrentIndent = indent;
	}

	public IEnumerator<ICodeBlock> GetEnumerator () => Blocks.GetEnumerator ();
	IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
}
