using System;

using Mono.Cecil;

#nullable enable

namespace Microsoft.MaciOS.Nnyeah.AssemblyComparator {
	public enum VisitKind {
		Start,
		End,
	}

	public class TypeVisitedEventArgs : EventArgs {
		public TypeVisitedEventArgs (VisitKind visitKind, TypeDefinition type)
		{
			Type = type;
			Kind = visitKind;
		}

		public VisitKind Kind { get; init; }
		public TypeDefinition Type { get; init; }
	}

	public class MemberVisitedEventArgs<T> : TypeVisitedEventArgs where T : IMemberDefinition {
		public MemberVisitedEventArgs (VisitKind visitKind, TypeDefinition type, T member)
			: base (visitKind, type)
		{
			Member = member;
		}

		public T Member { get; init; }
	}
}
