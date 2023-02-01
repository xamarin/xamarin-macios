Imports ObjCRuntime
Imports UIKit

Namespace tvOSApp1
	Public Partial Class ViewController
		Inherits UIViewController
		Protected Sub New(ByVal handle As NativeHandle)
			MyBase.New(handle)
			' This constructor is required if the view controller is loaded from a xib or a storyboard.
			' Do not put any initialization here, use ViewDidLoad instead.
		End Sub
	End Class
End Namespace
