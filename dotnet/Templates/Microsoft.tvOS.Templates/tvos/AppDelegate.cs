namespace tvOSApp1;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// Override point for customization after application launch.
		// If not required for your application you can safely delete this method

		return true;
	}
}
