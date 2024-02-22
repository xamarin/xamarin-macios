using System.Collections.Generic;

public class MethodBlock : CodeBlock {
	public MethodBlock (string methodSignature, string [] parameters)
	{
		string allParameters = "";
		for (int i = 0; i < parameters.Length; i++) {
			allParameters += parameters [i];
			if (i != parameters.Length - 1) {
				allParameters += ", ";
			}
		}
		HeaderText = methodSignature + "(" + allParameters + ")";
	}
	public MethodBlock (string methodSignature, string [] parameters, List<ICodeBlock> blocks)
	{
		string allParameters = "";
		for (int i = 0; i < parameters.Length; i++) {
			allParameters += parameters [i];
			if (i != parameters.Length - 1) {
				allParameters += ", ";
			}
		}
		HeaderText = methodSignature + "(" + allParameters + ")";
		this.Blocks = blocks;
	}
}
