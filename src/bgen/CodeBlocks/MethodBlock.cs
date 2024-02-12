using System.Collections.Generic;

public class MethodBlock : CodeBlock {
	public MethodBlock (string methodSignature, string [] parameters, int currentIndent) : base (currentIndent)
	{
		HeaderText = methodSignature + "(" + parameters [0] + ")";
	}
	public MethodBlock (string methodSignature, string [] parameters, List<ICodeBlock> blocks, int currentIndent) : base (currentIndent)
	{
		HeaderText = methodSignature + "(" + parameters [0] + ")";
		this.Blocks = blocks;
	}
}
