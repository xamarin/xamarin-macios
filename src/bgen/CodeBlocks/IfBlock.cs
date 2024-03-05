using System.Collections.Generic;
using System.IO;

public class IfBlock : CodeBlock {
	List<IfBlock> ElseIfBlocks = new ();
	CodeBlock? ElseBlock = null;
	public IfBlock (string condition)
	{
		HeaderText = condition;
	}

	public IfBlock (string condition, List<ICodeBlock> blocks)
	{
		HeaderText = condition;
		Blocks.AddRange (blocks);
	}

	public IfBlock (string condition, params string [] lines)
	{
		HeaderText = condition;
		Blocks.AddRange (new CodeBlock (lines));
	}

	public IfBlock AddElseIf (string condition, List<ICodeBlock> blocks)
	{
		ElseIfBlocks.Add (new IfBlock (condition, blocks));
		return this;
	}

	public IfBlock AddElseIf (string condition, params string [] lines)
	{
		ElseIfBlocks.Add (new IfBlock (condition, lines));
		return this;
	}

	public IfBlock AddElse (List<ICodeBlock> blocks)
	{
		ElseBlock = new CodeBlock ("else", blocks);
		return this;
	}

	public IfBlock AddElse (params string [] lines)
	{
		ElseBlock = new CodeBlock ("else", lines);
		return this;
	}

	protected override void WriteHeaderText (TextWriter writer)
	{
		WriteIndent (writer);
		writer.Write ("if (");
		writer.Write (HeaderText);
		writer.Write (')');
		writer.Write (NewLine);
	}

	protected void WriteElseHeaderText (TextWriter writer)
	{
		WriteIndent (writer);
		writer.Write ("else if (");
		writer.Write (HeaderText);
		writer.Write (')');
		writer.Write (NewLine);
	}

	public override void Print (TextWriter writer)
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

		foreach (IfBlock block in ElseIfBlocks) {
			block.SetIndent (CurrentIndent);
			block.WriteElseHeaderText (writer);
			writer.Write (StartBrace);
			writer.Write (NewLine);
			foreach (ICodeBlock b in block.Blocks) {
				b.SetIndent (CurrentIndent + Indent);
				b.Print (writer);

			}
			writer.Write (EndBrace);
			writer.Write (NewLine);
		}

		if (ElseBlock is not null)
			ElseBlock.Print (writer);
	}
}
