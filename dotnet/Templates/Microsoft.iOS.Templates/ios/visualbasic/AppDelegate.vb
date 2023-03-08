Imports Foundation
Imports UIKit

Namespace iOSApp1
	<Register("AppDelegate")>
	Public Class AppDelegate
		Inherits UIApplicationDelegate

		Public Overrides Property Window As UIWindow

		Public Overrides Function FinishedLaunching(ByVal application As UIApplication, ByVal launchOptions As NSDictionary) As Boolean
			' create a new window instance based on the screen size
			Window = new UIWindow(UIScreen.MainScreen.Bounds)

			' create a UIViewController with a single UILabel
			Dim vc = new UIViewController()
			Dim label As New UILabel(Window.Frame) With
			{
				.BackgroundColor = UIColor.SystemBackground,
				.TextAlignment = UITextAlignment.Center,
				.Text = "Hello, iOS!",
				.AutoresizingMask = UIViewAutoresizing.All
			}
			vc.View.AddSubview(label)
			Window.RootViewController = vc

			' make the window visible
			Window.MakeKeyAndVisible()

			Return True
		End Function
	End Class
End Namespace
