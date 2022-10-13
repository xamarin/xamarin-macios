namespace tvOSApp1;

[Register ("Controller1")]
public class Controller1 : UIViewController {
	public override void ViewDidLoad ()
	{
		View = new UIView {
			BackgroundColor = UIColor.Green,
		};

		base.ViewDidLoad ();

		// Perform any additional setup after loading the view
	}
}
