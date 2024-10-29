using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

namespace Mono.Linker {
	public partial class LinkContext {
		public AnnotationStore Annotations => _annotations;

		public partial TypeDefinition? GetType (string fullName);

		public partial void LogMessage (MessageContainer message);
		public partial string GetAssemblyLocation (AssemblyDefinition assembly);
		public partial AssemblyDefinition? GetLoadedAssembly (string name);

		public partial bool HasCustomData (string key);
		public partial bool TryGetCustomData (string key, [NotNullWhen (true)] out string? value);

		public partial MethodDefinition? Resolve (MethodReference methodReference);
		public partial FieldDefinition? Resolve (FieldReference fieldReference);
		public partial TypeDefinition? Resolve (TypeReference typeReference);

		public partial MethodDefinition? TryResolve (MethodReference methodReference);
		public partial FieldDefinition? TryResolve (FieldReference fieldReference);
		public partial TypeDefinition? TryResolve (TypeReference typeReference);

		public partial AssemblyDefinition? Resolve (AssemblyNameReference nameReference);
	}
}
