using System;

using AppKit;

using Foundation;

using ObjCRuntime;

static class MainClass {
	static int Main (string [] args)
	{
		Runtime.AssemblyRegistration += (object sender, AssemblyRegistrationEventArgs ea) => {
			Console.WriteLine (ea.AssemblyName);
			switch (ea.AssemblyName.Name) {
			case "AssemblyRegistration":
				ea.Register = false;
				break;
			default:
				break;
			}
		};
		NSApplication.Init ();
		return 0;
	}
}

// We deliberately declare two functions with the same selector
// Then we skip registration for this assembly, so that no exception is thrown.
[Register ("TestClass")]
public class TestClass : NSObject {
	[Export ("xap")]
	public void Foo ()
	{
	}

	[Export ("xap")]
	public void Bar ()
	{
	}
}
