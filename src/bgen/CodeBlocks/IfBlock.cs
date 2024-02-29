using System.Collections.Generic;
using System.IO;

public class IfBlock : CodeBlock {
	List<CodeBlock> ElseIfBlocks = new ();
	CodeBlock? ElseBlock = null;
	public IfBlock (string condition)
	{
		HeaderText = "if (" + condition + ")";
	}

	public IfBlock (string condition, List<ICodeBlock> blocks)
	{
		HeaderText = "if (" + condition + ")";
		Blocks.AddRange (blocks);
	}

	public void AddElseIf (string condition, List<ICodeBlock> blocks)
	{
		ElseIfBlocks.Add (new CodeBlock ("else if (" + condition + ")", blocks));
	}

	public void AddElse (List<ICodeBlock> blocks)
	{
		ElseBlock = new CodeBlock ("else", blocks);
	}

	protected override void WriteHeaderText (TextWriter writer)
	{
		WriteIndent (writer);
		writer.Write (HeaderText);
		writer.Write (NewLine);
	}

	public override void Print (TextWriter writer)
	{
		base.Print (writer);

		foreach (ICodeBlock block in ElseIfBlocks) {
			block.SetIndent (CurrentIndent);
			block.Print (writer);
		}

		if (ElseBlock is not null)
			ElseBlock.Print (writer);
	}
}
