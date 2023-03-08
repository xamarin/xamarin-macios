Imports Foundation
Imports UIKit

Namespace MacCatalystApp1
	<Register("SceneDelegate")>
	Public Class SceneDelegate
		Inherits UIResponder
		Implements IUIWindowSceneDelegate

		<Export("window")>
		Public Property Window As UIWindow

		<Export("scene:willConnectToSession:options:")>
		Public Sub WillConnect(ByVal scene As UIScene, ByVal session As UISceneSession, ByVal connectionOptions As UISceneConnectionOptions)
			' Use this method to optionally configure and attach the UIWindow `window` to the provided UIWindowScene `scene`.
			' If using a storyboard, the `window` property will automatically be initialized and attached to the scene.
			' This delegate does not imply the connecting scene or session are new (see UIApplicationDelegate `GetConfiguration` instead).
		End Sub

		<Export("sceneDidDisconnect:")>
		Public Sub DidDisconnect(ByVal scene As UIScene)
			' Called as the scene is being released by the system.
			' This occurs shortly after the scene enters the background, or when its session is discarded.
			' Release any resources associated with this scene that can be re-created the next time the scene connects.
			' The scene may re-connect later, as its session was not neccessarily discarded (see UIApplicationDelegate `DidDiscardSceneSessions` instead).
		End Sub

		<Export("sceneDidBecomeActive:")>
		Public Sub DidBecomeActive(ByVal scene As UIScene)
			' Called when the scene has moved from an inactive state to an active state.
			' Use this method to restart any tasks that were paused (or not yet started) when the scene was inactive.
		End Sub

		<Export("sceneWillResignActive:")>
		Public Sub WillResignActive(ByVal scene As UIScene)
			' Called when the scene will move from an active state to an inactive state.
			' This may occur due to temporary interruptions (ex. an incoming phone call).
		End Sub

		<Export("sceneWillEnterForeground:")>
		Public Sub WillEnterForeground(ByVal scene As UIScene)
			' Called as the scene transitions from the background to the foreground.
			' Use this method to undo the changes made on entering the background.
		End Sub

		<Export("sceneDidEnterBackground:")>
		Public Sub DidEnterBackground(ByVal scene As UIScene)
			' Called as the scene transitions from the foreground to the background.
			' Use this method to save data, release shared resources, and store enough scene-specific state information
			' to restore the scene back to its current state.
		End Sub
	End Class
End Namespace
