// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mono.Cecil;

namespace Mono.Linker.Steps
{

	public abstract partial class BaseStep : IStep
	{
		// public LinkContext Context { get { throw null; } }
		// public AnnotationStore Annotations { get { throw null; } }
		public partial void Process (LinkContext context);
		protected virtual partial bool ConditionToProcess ();
		protected virtual partial void Process ();
		protected virtual partial void EndProcess ();
		protected virtual partial void ProcessAssembly (AssemblyDefinition assembly);
	}
}
