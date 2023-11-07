using ObjCRuntime;

namespace macOS;

public partial class ViewController : NSViewController {
	protected ViewController (NativeHandle handle) : base (handle)
	{
	}

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Do any additional setup after loading the view.
	}

	public override void ViewDidAppear ()
	{
		var alert = new NSAlert ();
		alert.MessageText = ConsumerTests.Consumer.Test ();
		alert.RunModal ();
		NSApplication.SharedApplication.Terminate (this);
	}

	public override NSObject RepresentedObject {
		get => base.RepresentedObject;
		set {
			base.RepresentedObject = value;

			// Update the view, if already loaded.
		}
	}
}
