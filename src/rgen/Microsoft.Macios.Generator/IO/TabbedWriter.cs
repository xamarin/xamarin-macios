// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Macios.Generator.IO;

abstract class TabbedWriter<T> : IDisposable where T : TextWriter {
	protected readonly IndentedTextWriter Writer;
	protected readonly T InnerWriter;
	protected bool IsBlock;

	/// <summary>
	/// Created a new tabbed string builder that will use the given sb to write code.
	/// </summary>
	/// <param name="innerTextWriter">The string builder to be used to write code.</param>
	/// <param name="currentCount">The original tab size.</param>
	/// <param name="block">States if we are creating a {} block.</param>
	public TabbedWriter (T innerTextWriter, int currentCount = 0, bool block = false)
	{
		InnerWriter = innerTextWriter;
		Writer = new IndentedTextWriter (innerTextWriter, "\t");
		IsBlock = block;
		if (IsBlock) {
			// increase by 1 because we are in a block
			Writer.Indent = currentCount;
			Writer.WriteLine ('{');
			Writer.Indent += 1;
		} else {
			Writer.Indent = currentCount;
		}
	}

	/// <summary>
	/// Append a new empty line to the string builder using no tabs since it is empty.
	/// </summary>
	/// <returns>The current tabbed string builder.</returns>
	public TabbedWriter<T> WriteLine ()
	{
		var oldIndent = Writer.Indent;
		Writer.Indent = 0;
		Writer.WriteLine ();
		Writer.Indent = oldIndent;
		return this;
	}

	/// <summary>
	/// Append content, but do not add a \n
	/// </summary>
	/// <param name="line">The content to append.</param>
	/// <returns>The current builder.</returns>
	public TabbedWriter<T> Write (string line)
	{
		if (!string.IsNullOrWhiteSpace (line)) {
			Writer.Write (line);
		}

		return this;
	}

	/// <summary>
	/// Append content, but do not add a \n
	/// </summary>
	/// <param name="span">The content to append.</param>
	/// <returns>The current builder.</returns>
	public TabbedWriter<T> Write (ReadOnlySpan<char> span)
	{
		if (!span.IsWhiteSpace ()) {
			Writer.Write (span);
		}

		return this;
	}

	/// <summary>
	/// Append a new tabbed line.
	/// </summary>
	/// <param name="line">The line to append.</param>
	/// <returns>The current builder.</returns>
	public TabbedWriter<T> WriteLine (string line)
	{
		if (string.IsNullOrWhiteSpace (line)) {
			WriteLine ();
		} else {
			Writer.WriteLine (line);
		}

		return this;
	}

	/// <summary>
	/// Append a new tabbed lien from the span.
	/// </summary>
	/// <param name="span">The line to append.</param>
	/// <returns>The current builder.</returns>
	public TabbedWriter<T> WriteLine (ReadOnlySpan<char> span)
	{
		if (span.IsWhiteSpace ()) {
			WriteLine ();
		} else {
			Writer.WriteLine (span);
		}

		return this;
	}

#if NET9_0
	public TabbedWriter<T> Write (ref DefaultInterpolatedStringHandler handler)
	{
		Writer.Write (handler.ToStringAndClear ());
		return this;
	}

	public TabbedWriter<T> WriteLine (ref DefaultInterpolatedStringHandler handler)
	{
		Writer.WriteLine (handler.ToStringAndClear ());
		return this;
	}
	
	/// <summary>
	/// Append a new raw literal by prepending the correct indentation.
	/// </summary>
	/// <param name="rawString">The raw string to append.</param>
	/// <returns>The current builder.</returns>
	public TabbedWriter<T> WriteRaw (string rawString)
	{
		// we will split the raw string in lines and then append them so that the
		// tabbing is correct
		var lines = rawString.AsSpan().Split('\n');
		var enumerator = lines.GetEnumerator();
		bool hasNext = enumerator.MoveNext();
		while (hasNext)
		{
			var range = enumerator.Current;
			var line = rawString.AsSpan(range);
			hasNext = enumerator.MoveNext();
			if (!hasNext)
			{
				Write (line);
				break;
			}
			WriteLine (line);
		}
		return this;
	}
#else
	/// <summary>
	/// Append a new raw literal by prepending the correct indentation.
	/// </summary>
	/// <param name="rawString">The raw string to append.</param>
	/// <returns>The current builder.</returns>
	public TabbedWriter<T> WriteRaw (string rawString)
	{
		// we will split the raw string in lines and then append them so that the
		// tabbing is correct
		var lines = rawString.Split (['\n'], StringSplitOptions.None);
		for (var index = 0; index < lines.Length; index++) {
			var line = lines [index];
			if (index == lines.Length - 1) {
				Write (line);
			} else {
				WriteLine (line);
			}
		}
		return this;
	}
#endif

	/// <summary>
	/// Create a new block with the given line. This method can be used to write if/else statements.
	/// </summary>
	/// <param name="line">The new line to append</param>
	/// <param name="block">If the new line should considered a block.</param>
	/// <returns>The current builder.</returns>
	public abstract TabbedWriter<T> CreateBlock (string line, bool block);

	/// <summary>
	/// Create a bew empty block.
	/// </summary>
	/// <param name="block">If it is a block that uses {} or not.</param>
	/// <returns>The new bloc.</returns>
	public TabbedWriter<T> CreateBlock (bool block) => CreateBlock (string.Empty, block);

	/// <summary>
	/// Concatenate an array of lines as a single block. The generated code is similar to the following
	///
	/// <code>
	/// using (var nsobject = Create ())
	/// using (var nsstring = Create ())
	/// using (var x = new NSString ()) {
	///    // your code goes here.
	/// }
	/// </code>
	/// </summary>
	/// <param name="lines">The lines to concatenate.</param>
	/// <param name="block">True if the block should use braces, false otherwise.</param>
	/// <returns></returns>
	public TabbedWriter<T> CreateBlock (IEnumerable<string> lines, bool block)
	{
		var array = lines as string [] ?? lines.ToArray ();
		if (array.Length == 0) {
			return CreateBlock (IsBlock);
		}

		// append all the lines, then create a block
		for (var i = 0; i < array.Length - 1; i++) {
			Writer.WriteLine (array [i]);
		}
		return CreateBlock (array [^1], block);
	}

	protected virtual void Dispose (bool disposing)
	{
		if (disposing) {
			if (IsBlock) {
				Writer.Indent -= 1;
				Writer.WriteLine ('}');
				IsBlock = false;
			}
			Writer.Dispose ();
		}
	}

	/// <inheritdoc/>
	public void Dispose ()
	{
		Dispose (true);
		GC.SuppressFinalize (this);
	}
}
