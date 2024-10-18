// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Mono.Cecil;

namespace Mono.Linker
{
	internal class SubstitutionInfo
	{
		internal Dictionary<MethodDefinition, MethodAction> MethodActions { get; }
		internal Dictionary<MethodDefinition, object?> MethodStubValues { get; }
		internal Dictionary<FieldDefinition, object?> FieldValues { get; }
		internal HashSet<FieldDefinition> FieldInit { get; }

		internal SubstitutionInfo ()
		{
			MethodActions = new Dictionary<MethodDefinition, MethodAction> ();
			MethodStubValues = new Dictionary<MethodDefinition, object?> ();
			FieldValues = new Dictionary<FieldDefinition, object?> ();
			FieldInit = new HashSet<FieldDefinition> ();
		}

		internal void SetMethodAction (MethodDefinition method, MethodAction action)
		{
			MethodActions[method] = action;
		}

		internal void SetMethodStubValue (MethodDefinition method, object? value)
		{
			MethodStubValues[method] = value;
		}

		internal void SetFieldValue (FieldDefinition field, object? value)
		{
			FieldValues[field] = value;
		}

		internal void SetFieldInit (FieldDefinition field)
		{
			FieldInit.Add (field);
		}
	}
}
