using System.Collections.Generic;

public class IfBlock : CodeBlock
{
	public IfBlock(string condition, int currentIndent) : base(currentIndent)
	{
		headerText = "if (" + condition + ")";
	}
	public IfBlock(string condition, int currentIndent, List<ICodeBlock> blocks) : base(currentIndent)
	{
		headerText = "if (" + condition + ")";
		this.blocks = blocks;
	}
}
