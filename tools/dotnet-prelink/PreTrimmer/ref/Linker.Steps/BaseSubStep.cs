// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mono.Cecil;

namespace Mono.Linker.Steps
{
	public abstract partial class BaseSubStep : ISubStep
	{
		// protected AnnotationStore Annotations { get => throw null; }

		// protected LinkContext Context { get => throw null; }

		// public abstract SubStepTargets Targets { get; }

		public virtual partial void Initialize (LinkContext context);
		public virtual partial bool IsActiveFor (AssemblyDefinition assembly);
		public virtual partial void ProcessAssembly (AssemblyDefinition assembly);
		public virtual partial void ProcessType (TypeDefinition type);
		public virtual partial void ProcessField (FieldDefinition field);
		public virtual partial void ProcessMethod (MethodDefinition method);
		public virtual partial void ProcessProperty (PropertyDefinition property);
		public virtual partial void ProcessEvent (EventDefinition @event);
	}
}
