namespace iOSTabbedApp1;

using ObjCRuntime;

public partial class FirstViewController : UIViewController {
	public FirstViewController (NativeHandle handle) : base (handle)
	{
	}

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.
	}

	public override void DidReceiveMemoryWarning ()
	{
		base.DidReceiveMemoryWarning ();
		// Release any cached data, images, etc that aren't in use.
	}
}
