using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

public interface IMemberGatherer {
	IEnumerable<MethodInfo> GetTypeContractMethods (Type source);
}
