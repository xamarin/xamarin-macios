using System;
using System.IO;

public interface ICodeBlock
{
	public void Print (StreamWriter writer);
	public void SetIndent(int indent);
}

