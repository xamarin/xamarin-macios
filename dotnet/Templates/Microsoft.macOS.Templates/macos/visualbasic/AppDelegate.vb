Imports AppKit
Imports Foundation

Namespace macOSApp1
	<Register("AppDelegate")>
	Public Class AppDelegate
		Inherits NSApplicationDelegate

		Public Overrides Sub DidFinishLaunching(ByVal notification As NSNotification)
			' Insert code here to initialize your application
		End Sub

		Public Overrides Sub WillTerminate(ByVal notification As NSNotification)
			' Insert code here to tear down your application
		End Sub
	End Class
End Namespace
