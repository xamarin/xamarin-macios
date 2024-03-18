Imports Foundation
Imports UIKit

Namespace tvOSApp1
	<Register("AppDelegate")>
	Public Class AppDelegate
		Inherits UIApplicationDelegate

		Public Overrides Property Window As UIWindow

		Public Overrides Function FinishedLaunching(ByVal application As UIApplication, ByVal launchOptions As NSDictionary) As Boolean
			' Override point for customization after application launch.
			' If not required for your application you can safely delete this method

			Return True
		End Function
	End Class
End Namespace
