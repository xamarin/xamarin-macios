// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Mono.Linker.Steps
{
	public abstract partial class SubStepsDispatcher : IStep
	{
		// protected SubStepsDispatcher ();

		// protected SubStepsDispatcher (IEnumerable<ISubStep> subSteps);

		public partial void Add (ISubStep substep);

		// void IStep.Process (LinkContext context);
	}
}
