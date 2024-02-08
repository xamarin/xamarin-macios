public class BlockContainer : CodeBlock
{
	public BlockContainer(int currentIndent) : base(currentIndent)
	{
		indent = 0;
	}

	public override string Output()
	{
		string s = string.Empty;

		foreach (ICodeBlock block in blocks)
		{
			block.SetIndent(currentIndent);
			s += block.Output();
		}
		return s;
	}

	public void SetIndent(int indent)
	{
		currentIndent = indent;
	}
}
