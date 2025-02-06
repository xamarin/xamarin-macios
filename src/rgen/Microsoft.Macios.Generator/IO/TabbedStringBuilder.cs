// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Text;

namespace Microsoft.Macios.Generator.IO;

/// <summary>
/// String builder wrapper that keeps track of the current indentation level by abusing the IDisposable pattern. Rather
/// than dispose data, what the IDisposable pattern allows is to create a new block with an increased indentation, that
/// way we do not need to keep track of the current indentation level.
/// <example>
/// var classBlock = new TabbedStringBuilder (sb);
/// classBlock.AppendLine ("public static NSString? GetConstant (this {enumSymbol.Name} self)");
/// // open a new {} block, no need to keep track of the indentation, the new block has it
/// using (var getConstantBlock = classBlock.CreateBlock (isBlock: true)) {
///    // write the contents of the method here.
/// }
/// </example>
/// </summary>
partial class TabbedStringBuilder : TabbedWriter<StringWriter> {

	/// <summary>
	/// Created a new tabbed string builder that will use the given sb to write code.
	/// </summary>
	/// <param name="builder">The string builder to be used to write code.</param>
	/// <param name="currentCount">The original tab size.</param>
	/// <param name="block">States if we are creating a {} block.</param>
	public TabbedStringBuilder (StringBuilder builder, int currentCount = 0, bool block = false)
		: base (new StringWriter (builder), currentCount, block) {}
	
	/// <summary>
	/// Create a new block with the given line. This method can be used to write if/else statements.
	/// </summary>
	/// <param name="line">The new line to append</param>
	/// <param name="block">If the new line should considered a block.</param>
	/// <returns>The current builder.</returns>
	public override TabbedWriter<StringWriter> CreateBlock (string line, bool block)
	{
		if (!string.IsNullOrEmpty (line)) {
			Writer.WriteLine (line);
		}

		return new TabbedStringBuilder (InnerWriter.GetStringBuilder (), Writer.Indent, block);
	}

	/// <summary>
	/// Return code as a string.
	/// </summary>
	/// <returns>The code written so far.</returns>
	public string ToCode ()
	{
		// we need to make sure that if we are a block, that we close it
		if (IsBlock) {
			Writer.Flush ();
			Writer.Indent -= 1;
			Writer.WriteLine ('}');
			IsBlock = false;
		}
		return InnerWriter.GetStringBuilder ().ToString ();
	}

	/// <summary>
	/// Clear the content of the internal string builder.
	/// </summary>
	public void Clear ()
	{
		if (Writer.InnerWriter is StringWriter stringWriter) {
			stringWriter.GetStringBuilder ().Clear ();
		} else {
			Writer.InnerWriter.Flush ();
		}
	}
}
