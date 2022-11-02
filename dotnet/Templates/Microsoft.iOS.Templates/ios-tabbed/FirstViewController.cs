namespace iOSTabbedApp1;

using ObjCRuntime;

public partial class FirstViewController : UIViewController {
	protected FirstViewController (NativeHandle handle) : base (handle)
	{
		// This constructor is required if the view controller is loaded from a xib or a storyboard.
		// Do not put any initialization here, use ViewDidLoad instead.
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
