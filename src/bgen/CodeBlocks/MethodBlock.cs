using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MethodBlock : CodeBlock {
	public MethodBlock (string methodSignature, params string [] parameters)
	{
		StringBuilder allParameters = new ();
		for (int i = 0; i < parameters.Length; i++) {
			allParameters.Append (parameters [i]);
			if (i != parameters.Length - 1) {
				allParameters.Append (", ");
			}
		}
		HeaderText = methodSignature + "(" + allParameters + ")";
	}

	public MethodBlock (string methodSignature, List<ICodeBlock> blocks, params string [] parameters)
	{
		StringBuilder allParameters = new ();
		for (int i = 0; i < parameters.Length; i++) {
			allParameters.Append (parameters [i]);
			if (i != parameters.Length - 1) {
				allParameters.Append (", ");
			}
		}
		HeaderText = methodSignature + "(" + allParameters + ")";
		this.Blocks = blocks;
	}
}
