using XamCore.ObjCRuntime;

namespace XamCore.InputMethodKit {

	[Native]
	public enum IMKCandidatePanelType : nuint {
		SingleColumnScrolling = 1,
		ScrollingGrid = 2,
		SingleRowStepping = 3
	}

	[Native]
	public enum IMKStyleType : nuint {
		Main = 0,
		Annotation = 1,
		SubList = 2
	}

	[Native]
	public enum IMKCandidatesLocationHint : nuint {
		Above = 1,
		Below = 2,
		Left = 3,
		Right = 4
	}
}
