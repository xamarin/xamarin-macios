using System;
using XamCore.ObjCRuntime;

namespace XamCore.FinderSync {
	[Native]
	public enum FIMenuKind : nuint
	{
		ContextualMenuForItems = 0,
		ContextualMenuForContainer = 1,
		ContextualMenuForSidebar = 2,
		ToolbarItemMenu = 3
	}
}