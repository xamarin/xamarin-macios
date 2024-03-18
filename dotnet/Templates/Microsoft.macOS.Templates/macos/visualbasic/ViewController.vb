Imports Foundation
Imports ObjCRuntime
Imports AppKit

Namespace macOSApp1
	Public Partial Class ViewController
		Inherits NSViewController

		Protected Sub New(ByVal handle As NativeHandle)
			MyBase.New(handle)
			' This constructor is required if the view controller is loaded from a xib or a storyboard.
			' Do not put any initialization here, use ViewDidLoad instead.
		End Sub

		Public Overrides Sub ViewDidLoad()
			MyBase.ViewDidLoad()

			' Do any additional setup after loading the view.
		End Sub

		Public Overrides Property RepresentedObject As NSObject
			Get
				Return MyBase.RepresentedObject
			End Get
			Set
				MyBase.RepresentedObject = value

				' Update the view, if already loaded.
			End Set
		End Property
	End Class
End Namespace
