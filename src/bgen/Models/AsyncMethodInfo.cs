using System;
using System.Linq;
using System.Reflection;

#nullable enable

class AsyncMethodInfo : MemberInformation {
	public ParameterInfo [] AsyncInitialParams { get; }
	public ParameterInfo [] AsyncCompletionParams { get; }
	public bool HasNSError { get; }
	public bool IsNSErrorNullable { get; }
	public bool IsVoidAsync { get; }
	public bool IsSingleArgAsync { get; }
	public MethodInfo MethodInfo { get; }

	public AsyncMethodInfo (Generator generator, IMemberGatherer gather, Type type, MethodInfo mi, Type categoryExtensionType, bool isExtensionMethod)
		: base (generator, gather, mi, type, categoryExtensionType, false, isExtensionMethod)
	{
		this.MethodInfo = mi;
		this.AsyncInitialParams = mi.GetParameters ().DropLast ();

		var lastType = mi.GetParameters ().Last ().ParameterType;
		if (!lastType.IsSubclassOf (generator.TypeCache.System_Delegate))
			throw new BindingException (1036, true, mi.DeclaringType?.FullName, mi.Name, lastType.FullName);
		var cbParams = lastType.GetMethod ("Invoke")?.GetParameters () ?? Array.Empty<ParameterInfo> ();
		AsyncCompletionParams = cbParams;

		var lastParam = cbParams.LastOrDefault ();
		if (lastParam is not null && lastParam.ParameterType.Name == "NSError") {
			HasNSError = true;
			IsNSErrorNullable = generator.AttributeManager.HasAttribute<NullAllowedAttribute> (lastParam);
			cbParams = cbParams.DropLast ();
		}

		IsVoidAsync = cbParams.Length == 0;
		IsSingleArgAsync = cbParams.Length == 1;
	}

	public string GetUniqueParamName (string suggestion)
	{
		while (true) {
			bool next = false;

			foreach (var pi in AsyncCompletionParams) {
				if (pi.Name == suggestion) {
					next = true;
					break;
				}
			}

			if (!next)
				return suggestion;

			suggestion = "_" + suggestion;
		}
	}

}
