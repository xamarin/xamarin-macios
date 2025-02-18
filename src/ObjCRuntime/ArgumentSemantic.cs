namespace ObjCRuntime {
	public enum ArgumentSemantic : int {
		/// <summary>No argument semantics is specified.</summary>
		None = -1,
		/// <summary>Assigning is merely a pointer copy. This is identical to <see cref="F:ObjCRuntime.ArgumentSemantic.UnsafeUnretained" /></summary>
		Assign = 0,
		/// <summary>A copy of the object is made.</summary>
		Copy = 1,
		/// <summary>The assigned object is retained (its reference count increased). This is identical to <see cref="F:ObjCRuntime.ArgumentSemantic.Strong" />.</summary>
		Retain = 2,
		/// <summary>A weak reference is created to the assigned object, and the property will automatically be nulled out when the assigned object is freed.</summary>
		Weak = 3,
		/// <summary>The assigned object is retained (its reference count increased). This is identical to <see cref="F:ObjCRuntime.ArgumentSemantic.Retain" />.</summary>
		Strong = Retain,
		/// <summary>Merely performs a pointer copy in unmanaged code. This is identical to <see cref="F:ObjCRuntime.ArgumentSemantic.Assign" />.</summary>
		UnsafeUnretained = Assign,
	}
}
