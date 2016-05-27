#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using ScriptingBridge;
#else
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.ScriptingBridge;
#endif

namespace Finder
{
	[BaseType (typeof(SBObject))]
	interface FinderItem
	{
		[Export ("name")]
		string Name { get; }
	}

	[BaseType (typeof(FinderItem))]
	interface FinderContainer
	{
	}

	[BaseType (typeof(FinderContainer))]
	interface FinderTrashObject
	{
		[Export ("items")]
		SBElementArray Items { get; }
	}

	[BaseType (typeof(SBApplication))]
	interface FinderApplication
	{
		[Export ("trash")]
		FinderTrashObject Trash { get; }
	}
}
