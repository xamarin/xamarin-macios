using System.IO;

public interface ICodeBlock {
	public void Print (TextWriter writer);
	public void SetIndent (int indent);
}

