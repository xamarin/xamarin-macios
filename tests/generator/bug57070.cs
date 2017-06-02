using Foundation;
using ObjCRuntime;

[BaseType (typeof(NSObject))]
interface SomeClass
{
	[iOS (10,0)]
	[Export ("doSomething")]
	void DoSomething ();
}