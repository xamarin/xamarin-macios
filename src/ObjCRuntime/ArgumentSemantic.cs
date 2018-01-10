namespace ObjCRuntime {
	public enum ArgumentSemantic : int {
		None = -1,
		Assign = 0,
		Copy = 1,
		Retain = 2,
		Weak = 3,
		Strong = Retain,
		UnsafeUnretained = Assign,
	}
}
