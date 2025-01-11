// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Macios.Generator.DataModel;

namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Returns the emitter that is related to the provided declaration type.
/// </summary>
static class EmitterFactory {

	static readonly Dictionary<BindingType, ICodeEmitter> emitters = new () {
		{ BindingType.Class, new ClassEmitter () },
		{ BindingType.SmartEnum, new EnumEmitter () },
		{ BindingType.Protocol, new InterfaceEmitter () },
		{ BindingType.Category, new CategoryEmitter () },
	};
	public static bool TryCreate (CodeChanges changes, [NotNullWhen (true)] out ICodeEmitter? emitter)
		=> emitters.TryGetValue (changes.BindingType, out emitter);
}
