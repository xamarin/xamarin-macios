using System.IO;

public class BlockContainer : CodeBlock {
	public override void Print (TextWriter writer)
	{
		foreach (ICodeBlock block in Blocks) {
			block.SetIndent (CurrentIndent);
			block.Print (writer);
		}
	}
}
