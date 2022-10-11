using ObjCRuntime;

namespace macOSApp1;

[Register ("Controller1")]
public class Controller1 : NSViewController {
	public Controller1 ()
	{
	}

	protected Controller1 (NativeHandle handle) : base (handle)
	{
		// This constructor is required if the view controller is loaded from a xib or a storyboard.
		// Do not put any initialization here, use ViewDidLoad instead.
	}

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		// Perform any additional setup after loading the view
		View.WantsLayer = true;
		View.Layer!.BackgroundColor = NSColor.Blue.CGColor;
	}
}
