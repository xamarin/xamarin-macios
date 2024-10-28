namespace Microsoft.Macios.Generator.Emitters;

/// <summary>
/// Contains the documentation to be inserted in the generated code.
/// </summary>
static class Documentation {
	public const string ClassHandle = @"/// <summary>The Objective-C class handle for this class.</summary>
/// <value>The pointer to the Objective-C class.</value>
/// <remarks>
///     Each managed class mirrors an unmanaged Objective-C class.
///     This value contains the pointer to the Objective-C class.
///     It is similar to calling the managed <see cref=""ObjCRuntime.Class.GetHandle(string)"" /> or the native <see href=""https://developer.apple.com/documentation/objectivec/1418952-objc_getclass"">objc_getClass</see> method with the type name.
/// </remarks>
";
}
