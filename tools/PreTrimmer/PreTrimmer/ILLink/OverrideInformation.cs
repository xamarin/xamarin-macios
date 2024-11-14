// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mono.Cecil;

namespace Mono.Linker {
	public class OverrideInformation {
		internal OverrideInformation (MethodDefinition @base, MethodDefinition @override)
		{
			Base = @base;
			Override = @override;
		}

		public MethodDefinition Base { get; private set; }
		public MethodDefinition Override { get; private set; }
	}
}
