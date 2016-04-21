namespace XamCore.InputMethodKit {

	[Native]
	public enum IMKCandidatePanelType : nuint_compat_int /* : NSUInteger */ {
		SingleColumnScrolling = 1,
		ScrollingGrid = 2,
		SingleRowStepping = 3
	}

	[Native]
	public enum IMKStyleType : nuint_compat_int /* : NSUInteger */ {
		Main = 0,
		Annotation = 1,
		SubList = 2
	}

	[Native]
	public enum IMKCandidatesLocationHint : nuint_compat_int /* : NSUInteger */ {
		Above = 1,
		Below = 2,
		Left = 3,
		Right = 4
	}
}
