// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

#nullable enable

namespace Mono.Linker {
	internal static class ModuleDefinitionExtensions {
		public static bool IsCrossgened (this ModuleDefinition module)
		{
			return (module.Attributes & ModuleAttributes.ILOnly) == 0 &&
				(module.Attributes & ModuleAttributes.ILLibrary) != 0;
		}

		public static TypeDefinition? ResolveType (this ModuleDefinition module, string typeFullName, ITryResolveMetadata resolver)
		{
			var type = module.GetType (typeFullName);
			if (type is not null)
				return type;

			if (!module.HasExportedTypes)
				return null;

			// When resolving a forwarded type from a string, typeFullName should be a simple type name.
			int idx = typeFullName.LastIndexOf ('.');
			(string typeNamespace, string typeName) = idx > 0 ? (typeFullName.Substring (0, idx), typeFullName.Substring (idx + 1)) :
				(string.Empty, typeFullName);

			TypeReference typeReference = new TypeReference (typeNamespace, typeName, module, module);
			return resolver.TryResolve (typeReference);
		}
	}
}
