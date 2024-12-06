// 
// Authors
//    Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2018 Microsoft Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace Mono.ApiTools {

	abstract class Formatter {
		public string OutputPath { get; set; }

		Stack<(StringBuilder, TextWriter)> builders;
		public StringBuilder StringBuilder { get; private set; }
		protected TextWriter output;

		protected int IndentLevel { get; set; }

		protected Formatter (State state, bool isMultiplexed = false)
		{
			State = state;
			if (!isMultiplexed) {
				StringBuilder = new StringBuilder ();
				output = new StringWriter (StringBuilder);
				builders = new Stack<(StringBuilder, TextWriter)> ();
			}
		}

		public virtual void Flush ()
		{
			output.Flush ();
		}

		public virtual void WriteLine ()
		{
			output.WriteLine ();
		}

		public virtual void WriteLine (string line)
		{
			output.WriteLine (line);
		}

		public virtual void WriteLine (string format, params object[] arguments)
		{
			output.WriteLine (format, arguments);
		}

		public virtual void WriteLine (StringBuilder sb)
		{
			output.WriteLine (sb);
		}

		public virtual void Write (char value)
		{
			output.Write (value);
		}

		public virtual void Write (string line)
		{
			output.Write (line);
		}

		public virtual void Write (string format, params object [] arguments)
		{
			output.Write (format, arguments);
		}

		public virtual void Write (StringBuilder sb)
		{
			output.Write (sb);
		}

		public virtual void WriteIndentation ()
		{
			for (int i = 0; i < IndentLevel; i++)
				Write ('\t');
		}

		public State State { get; }

		public virtual void PushOutput ()
		{
			builders.Push ((StringBuilder, output));
			StringBuilder = new StringBuilder ();
			output = new StringWriter (StringBuilder);
		}

		public StringBuilder PopOutput ()
		{
			var rv = StringBuilder;
			output.Flush ();
			output.Dispose ();

			var popped = builders.Pop ();
			StringBuilder = popped.Item1;
			output = popped.Item2;

			return rv;
		}

		public abstract string LesserThan { get; }
		public abstract string GreaterThan { get; }

		public abstract void BeginDocument (string title);
		public virtual void EndDocument ()
		{
		}

		public abstract void BeginAssembly ();
		public virtual void EndAssembly ()
		{
		}

		public abstract void BeginNamespace (string action = "");
		public virtual void EndNamespace ()
		{
		}

		public abstract void BeginTypeAddition ();
		public abstract void EndTypeAddition ();

		public abstract void BeginTypeModification ();
		public virtual void EndTypeModification ()
		{
		}

		public abstract void BeginTypeRemoval ();
		public virtual void EndTypeRemoval ()
		{
		}

		public abstract void BeginMemberAddition (IEnumerable<XElement> list, MemberComparer member);
		public abstract void AddMember (MemberComparer member, bool isInterfaceBreakingChange, string obsolete, string description);
		public abstract void EndMemberAddition ();

		public abstract void BeginMemberModification (string sectionName);
		public abstract void EndMemberModification ();

		public abstract void BeginMemberRemoval (IEnumerable<XElement> list, MemberComparer member);
		public abstract void RemoveMember (MemberComparer member, bool breaking, string obsolete, string description);
		public abstract void EndMemberRemoval ();

		public abstract void RenderObsoleteMessage (TextChunk chunk, MemberComparer member, string description, string optionalObsoleteMessage);

		public abstract void DiffAddition (TextChunk chunk, string text, bool breaking);
		public abstract void DiffModification (TextChunk chunk, string old, string @new, bool breaking);
		public abstract void DiffRemoval (TextChunk chunk, string text, bool breaking);
		public abstract void Diff (ApiChange apichange);

		public virtual void IncreaseIndentation ()
		{
			IndentLevel++;
		}

		public virtual void DecreaseIndentation ()
		{
			IndentLevel--;
		}
	}

	class TextChunk {
		List<(Formatter Formatter, StringBuilder StringBuilder)> stringbuilders = new List<(Formatter, StringBuilder)> ();

		StringBuilder cachedOutput = new StringBuilder ();
		public StringBuilder GetStringBuilder (Formatter formatter)
		{
			foreach (var kvp in stringbuilders)
				if (kvp.Formatter == formatter)
					return kvp.StringBuilder;
			var rv = new StringBuilder ();
			rv.Append (cachedOutput);
			stringbuilders.Add (new (formatter, rv));
			return rv;
		}

		public void Append (string value)
		{
			foreach (var kvp in stringbuilders)
				kvp.StringBuilder.Append (value);
			cachedOutput.Append (value);
		}

		public override string ToString ()
		{
			throw new InvalidOperationException ();
		}
	}
}
