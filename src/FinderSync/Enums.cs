using System;
using ObjCRuntime;

namespace FinderSync {
	[Native]
	public enum FIMenuKind : nuint
	{
		ContextualMenuForItems = 0,
		ContextualMenuForContainer = 1,
		ContextualMenuForSidebar = 2,
		ToolbarItemMenu = 3
	}
}