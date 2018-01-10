namespace InputMethodKit {

	[Native]
	public enum IMKCandidatePanelType : ulong /* : NSUInteger */ {
		SingleColumnScrolling = 1,
		ScrollingGrid = 2,
		SingleRowStepping = 3
	}

	[Native]
	public enum IMKStyleType : ulong /* : NSUInteger */ {
		Main = 0,
		Annotation = 1,
		SubList = 2
	}

	[Native]
	public enum IMKCandidatesLocationHint : ulong /* : NSUInteger */ {
		Above = 1,
		Below = 2,
		Left = 3,
		Right = 4
	}
}
