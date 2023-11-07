using Foundation;
using ObjCRuntime;
using ScriptingBridge;

namespace Finder {
	[BaseType (typeof (SBObject))]
	interface FinderItem {
		[Export ("name")]
		string Name { get; }
	}

	[BaseType (typeof (FinderItem))]
	interface FinderContainer {
	}

	[BaseType (typeof (FinderContainer))]
	interface FinderTrashObject {
		[Export ("items")]
		SBElementArray Items { get; }
	}

	[BaseType (typeof (SBApplication))]
	interface FinderApplication {
		[Export ("trash")]
		FinderTrashObject Trash { get; }
	}
}
