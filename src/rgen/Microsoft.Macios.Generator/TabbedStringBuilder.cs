using System;
using System.Text;

namespace Microsoft.Macios.Generator;


/// <summary>
/// String builder wrapper that keeps track of the current indentation level by abusing the IDisposable pattern. Rather
/// than dispose data, what the IDisposable pattern allows is to create a new block with an increased indentation, that
/// way we do not need to keep track of the current indentation level.
/// <example>
/// var classBlock = new TabbedStringBuilder (sb);
/// classBlock.AppendFormatLine ("public static NSString? GetConstant (this {0} self)", enumSymbol.Name);
/// // open a new {} block, no need to keep track of the indentation, the new block has it
/// using (var getConstantBlock = classBlock.CreateBlock (isBlock: true)) {
///    // write the contents of the method here.
/// }
/// </example>
/// </summary>
class TabbedStringBuilder : IDisposable {
	readonly StringBuilder sb;
	readonly uint tabCount;
	readonly bool isBlock;
	bool disposed;

	/// <summary>
	/// Created a new tabbed string builder that will use the given sb to write code.
	/// </summary>
	/// <param name="builder">The string builder to be used to write code.</param>
	/// <param name="currentCount">The original tab size.</param>
	/// <param name="block">States if we are creating a {} block.</param>
	public TabbedStringBuilder (StringBuilder builder, uint currentCount = 0, bool block = false)
	{
		sb = builder;
		isBlock = block;
		if (isBlock) {
			// increase by 1 because we are in a block
			tabCount = currentCount + 1;
			var braceTab = new string ('\t', (int) (tabCount - 1));
			this.sb.AppendLine ($"{braceTab}{{");
		} else {
			tabCount = currentCount;
		}
	}

	StringBuilder WriteTabs () => sb.Append ('\t', (int) tabCount);

	/// <summary>
	/// Append a new empty line to the string builder using the correct tab size.
	/// </summary>
	/// <returns>The current tabbed string builder.</returns>
	public TabbedStringBuilder AppendLine ()
	{
		sb.AppendLine ();
		return this;
	}

	/// <summary>
	/// Append a line, but do not add a \n
	/// </summary>
	/// <param name="line"></param>
	/// <returns></returns>
	public TabbedStringBuilder Append (string line)
	{
		if (string.IsNullOrWhiteSpace (line)) {
			sb.Append (line);
		} else {
			WriteTabs ().Append (line);
		}
		return this;
	}

	/// <summary>
	/// Append a new tabbed line.
	/// </summary>
	/// <param name="line">The line to append.</param>
	/// <returns>The current builder.</returns>
	public TabbedStringBuilder AppendLine (string line)
	{
		if (string.IsNullOrWhiteSpace (line)) {
			sb.AppendLine (line);
		} else {
			WriteTabs ().AppendLine (line);
		}
		return this;
	}

	public TabbedStringBuilder AppendFormatLine (string format, params object [] args)
	{
		WriteTabs ().AppendFormat (format, args);
		sb.AppendLine ();
		return this;
	}

	/// <summary>
	/// Append a new raw literal by prepending the correct indentation.
	/// </summary>
	/// <param name="rawString">The raw string to append.</param>
	/// <returns>The current builder.</returns>
	public TabbedStringBuilder AppendRaw (string rawString)
	{
		// we will split the raw string in lines and then append them so that the
		// tabbing is correct
		var lines = rawString.Split (['\n'], StringSplitOptions.None);
		for (var index = 0; index < lines.Length; index++) {
			var line = lines [index];
			if (index == lines.Length - 1) {
				Append (line);
			} else {
				AppendLine (line);
			}
		}
		return this;
	}

	/// <summary>
	/// Append the generated code attribute to the current string builder. Added for convenience.
	/// </summary>
	/// <param name="optimizable">If the binding is Optimizable or not.</param>
	/// <returns>The current builder.</returns>
	public TabbedStringBuilder AppendGeneratedCodeAttribute (bool optimizable = true)
	{
		if (optimizable) {
			const string attr = "[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]";
			AppendLine (attr);
		} else {
			const string attr = "[BindingImpl (BindingImplOptions.GeneratedCode)]";
			AppendLine (attr);
		}

		return this;
	}

	/// <summary>
	/// Append a EditorBrowsable attribute. Added for convenience.
	/// </summary>
	/// <returns>The current builder.</returns>
	public TabbedStringBuilder AppendEditorBrowsableAttribute ()
	{
		const string attr = "[EditorBrowsable (EditorBrowsableState.Never)]";
		AppendLine (attr);
		return this;
	}

	/// <summary>
	/// Create a bew empty block.
	/// </summary>
	/// <param name="block">If it is a block that uses {} or not.</param>
	/// <returns>The new bloc.</returns>
	public TabbedStringBuilder CreateBlock (bool block) => CreateBlock (string.Empty, block);

	/// <summary>
	/// Create a new block with the given line. This method can be used to write if/else statements.
	/// </summary>
	/// <param name="line">The new line to append</param>
	/// <param name="block">If the new line should considered a block.</param>
	/// <returns>The current builder.</returns>
	public TabbedStringBuilder CreateBlock (string line, bool block)
	{
		if (!string.IsNullOrEmpty (line)) {
			WriteTabs ().AppendLine (line);
		}

		return new TabbedStringBuilder (sb, tabCount, block);
	}

	/// <summary>
	/// Return the string builder as a string.
	/// </summary>
	/// <returns></returns>
	public override string ToString ()
	{
		Dispose ();
		return sb.ToString ();
	}

	/// <summary>
	/// Does not really dispose anything, it just closes the current block.
	/// </summary>
	public void Dispose ()
	{
		if (disposed || !isBlock) return;

		disposed = true;
		sb.Append ('\t', (int) tabCount - 1);
		sb.AppendLine ("}");
	}
}
