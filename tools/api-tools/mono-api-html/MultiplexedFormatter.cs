// 
// Authors
//    Rolf Kvinge <rolf@xamarin.com>
//
// Copyright 2022 Microsoft Inc.
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

#nullable enable

namespace Mono.ApiTools {

	class MultiplexedFormatter : Formatter {
		Formatter [] formatters;

		public MultiplexedFormatter (State state, params Formatter [] formatters)
			: base (state, isMultiplexed: true)
		{
			this.formatters = formatters;
		}

		public override string LesserThan => "%LESSERTHANREPLACEMENT%";
		public override string GreaterThan => "%GREATERTHANREPLACEMENT%";

		string Replace (Formatter formatter, string text)
		{
			return text.Replace (LesserThan, formatter.LesserThan).Replace (GreaterThan, formatter.GreaterThan);
		}

		public override void BeginDocument (string title)
		{
			foreach (var formatter in formatters)
				formatter.BeginDocument (Replace (formatter, title));
		}

		public override void BeginAssembly ()
		{
			foreach (var formatter in formatters)
				formatter.BeginAssembly ();
		}

		public override void BeginNamespace (string action)
		{
			foreach (var formatter in formatters)
				formatter.BeginNamespace (Replace (formatter, action));
		}

		public override void BeginTypeAddition ()
		{
			foreach (var formatter in formatters)
				formatter.BeginTypeAddition ();
		}

		public override void EndTypeAddition ()
		{
			foreach (var formatter in formatters)
				formatter.EndTypeAddition ();
		}

		public override void BeginTypeModification ()
		{
			foreach (var formatter in formatters)
				formatter.BeginTypeModification ();
		}

		public override void BeginTypeRemoval ()
		{
			foreach (var formatter in formatters)
				formatter.BeginTypeRemoval ();
		}

		public override void BeginMemberAddition (IEnumerable<XElement> list, MemberComparer member)
		{
			foreach (var formatter in formatters)
				formatter.BeginMemberAddition (list, member);
		}

		public override void AddMember (MemberComparer member, bool isInterfaceBreakingChange, string obsolete, string description)
		{
			foreach (var formatter in formatters)
				formatter.AddMember (member, isInterfaceBreakingChange, Replace (formatter, obsolete), Replace (formatter, description));
		}

		public override void EndMemberAddition ()
		{
			foreach (var formatter in formatters)
				formatter.EndMemberAddition ();
		}

		public override void BeginMemberModification (string sectionName)
		{
			foreach (var formatter in formatters)
				formatter.BeginMemberModification (Replace (formatter, sectionName));
		}

		public override void EndMemberModification ()
		{
			foreach (var formatter in formatters)
				formatter.EndMemberModification ();
		}

		public override void BeginMemberRemoval (IEnumerable<XElement> list, MemberComparer member)
		{
			foreach (var formatter in formatters)
				formatter.BeginMemberRemoval (list, member);
		}

		public override void RemoveMember (MemberComparer member, bool is_breaking, string obsolete, string description)
		{
			foreach (var formatter in formatters)
				formatter.RemoveMember (member, is_breaking, Replace (formatter, obsolete), Replace (formatter, description));
		}

		public override void EndMemberRemoval ()
		{
			foreach (var formatter in formatters)
				formatter.EndMemberRemoval ();
		}

		public override void RenderObsoleteMessage (TextChunk chunk, MemberComparer member, string description, string optionalObsoleteMessage)
		{
			foreach (var formatter in formatters)
				formatter.RenderObsoleteMessage (chunk, member, Replace (formatter, description), Replace (formatter, optionalObsoleteMessage));
		}

		public override void DiffAddition (TextChunk chunk, string text, bool breaking)
		{
			foreach (var formatter in formatters)
				formatter.DiffAddition (chunk, Replace (formatter, text), breaking);
		}

		public override void DiffModification (TextChunk chunk, string old, string @new, bool breaking)
		{
			foreach (var formatter in formatters)
				formatter.DiffModification (chunk, Replace (formatter, old), Replace (formatter, @new), breaking);
		}

		public override void DiffRemoval (TextChunk chunk, string text, bool breaking)
		{
			foreach (var formatter in formatters)
				formatter.DiffRemoval (chunk, Replace (formatter, text), breaking);
		}

		public override void Diff (ApiChange apichange)
		{
			foreach (var formatter in formatters)
				formatter.Diff (apichange);
		}

		public override void PushOutput ()
		{
			foreach (var formatter in formatters)
				formatter.PushOutput ();
		}

		public override void IncreaseIndentation ()
		{
			foreach (var formatter in formatters)
				formatter.IncreaseIndentation ();
		}

		public override void DecreaseIndentation ()
		{
			foreach (var formatter in formatters)
				formatter.DecreaseIndentation ();
		}

		public override void WriteIndentation ()
		{
			foreach (var formatter in formatters)
				formatter.WriteIndentation ();
		}

		public override void Flush ()
		{
			foreach (var formatter in formatters)
				formatter.Flush ();
		}

		public override void WriteLine ()
		{
			foreach (var formatter in formatters)
				formatter.WriteLine ();
		}

		public override void WriteLine (string line)
		{
			foreach (var formatter in formatters)
				formatter.WriteLine (line);
		}

		public override void WriteLine (string format, params object [] arguments)
		{
			foreach (var formatter in formatters)
				formatter.WriteLine (format, arguments);
		}

		public override void WriteLine (StringBuilder sb)
		{
			foreach (var formatter in formatters)
				formatter.WriteLine (sb);
		}

		public override void Write (char value)
		{
			foreach (var formatter in formatters)
				formatter.Write (value);
		}

		public override void Write (string line)
		{
			foreach (var formatter in formatters)
				formatter.Write (line);
		}

		public override void Write (string format, params object [] arguments)
		{
			foreach (var formatter in formatters)
				formatter.Write (format, arguments);
		}

		public override void Write (StringBuilder sb)
		{
			foreach (var formatter in formatters)
				formatter.Write (sb);
		}
	}
}
