// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Mono.Linker.Steps
{
	public partial class MarkSubStepsDispatcher : IMarkHandler
	{
		// public MarkSubStepsDispatcher (IEnumerable<ISubStep> subSteps);

		public partial void Initialize (LinkContext context, MarkContext markContext);
	}
}
