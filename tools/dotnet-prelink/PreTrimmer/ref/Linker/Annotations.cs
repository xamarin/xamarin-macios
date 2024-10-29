// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Mono.Cecil;

namespace Mono.Linker
{

	public partial class AnnotationStore
	{
		public partial IEnumerable<OverrideInformation>? GetOverrides (MethodDefinition method);

		public partial void Mark (IMetadataTokenProvider provider);
		public partial void Mark (CustomAttribute attribute);

		public partial bool IsMarked (IMetadataTokenProvider provider);
		public partial bool IsMarked (CustomAttribute attribute);

		partial void AddPreservedMethod (MethodDefinition key, MethodDefinition method);
		public partial void AddPreservedMethod (TypeDefinition type, MethodDefinition method);
		public partial void SetPreserve (TypeDefinition type, TypePreserve preserve);

		public partial void SetAction (MethodDefinition method, MethodAction action);
		public partial void SetStubValue (MethodDefinition method, object value);

		public partial AssemblyAction GetAction (AssemblyDefinition assembly);
		public partial void SetAction (AssemblyDefinition assembly, AssemblyAction action);
		public partial bool HasAction (AssemblyDefinition assembly);

		public partial object? GetCustomAnnotation (object key, IMetadataTokenProvider item);
		public partial void SetCustomAnnotation (object key, IMetadataTokenProvider item, object value);
	}
}
