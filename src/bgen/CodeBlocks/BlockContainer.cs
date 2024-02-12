public class BlockContainer : CodeBlock
{
	public BlockContainer(int currentIndent) : base(currentIndent)
	{
		Indent = 0;
	}

	public override string Print()
	{
		string s = string.Empty;

		foreach (ICodeBlock block in Blocks)
		{
			block.SetIndent(CurrentIndent);
			s += block.Print();
		}
		return s;
	}

	public void SetIndent(int indent)
	{
		CurrentIndent = indent;
	}
}
