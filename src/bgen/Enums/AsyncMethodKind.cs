#nullable enable

//
// The kind of Async method we generate
// We typically generate a single one, but if the
// async method has a non-void return, we generate the second with the out parameter
// 
public enum AsyncMethodKind {
	// Plain Async method, original method return void
	Plain,

	// Async method generated when we had a return type from the method
	// ie: [Async] string XyZ (Action completion), the "string" is the
	// result
	WithResultOutParameter,
}
