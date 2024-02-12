using System.Collections.Generic;

public class IfBlock : CodeBlock
{
	public IfBlock(string condition, int currentIndent) : base(currentIndent)
	{
		HeaderText = "if (" + condition + ")";
	}
	public IfBlock(string condition, int currentIndent, List<ICodeBlock> blocks) : base(currentIndent)
	{
		HeaderText = "if (" + condition + ")";
		this.Blocks = blocks;
	}
}
