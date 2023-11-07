namespace MacCatalystApp1;

[Register ("Controller1")]
public class Controller1 : UIViewController {
	public override void ViewDidLoad ()
	{
		View = new UIView {
			BackgroundColor = UIColor.Blue,
		};

		base.ViewDidLoad ();

		// Perform any additional setup after loading the view
	}
}
