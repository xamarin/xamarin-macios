// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

namespace Mono.Linker
{
	class MemberActionStore
	{
		SubstitutionInfo PrimarySubstitutionInfo { get; }
		private readonly Dictionary<AssemblyDefinition, SubstitutionInfo?> _embeddedXmlInfos;
		private readonly Dictionary<MethodDefinition, bool> _featureCheckValues;
		readonly LinkContext _context;

		MemberActionStore (LinkContext context)
		{
			PrimarySubstitutionInfo = new SubstitutionInfo ();
			_embeddedXmlInfos = new Dictionary<AssemblyDefinition, SubstitutionInfo?> ();
			_featureCheckValues = new Dictionary<MethodDefinition, bool> ();
			_context = context;
		}
	}
}
