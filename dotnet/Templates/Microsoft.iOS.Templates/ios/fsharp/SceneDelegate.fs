namespace iOSApp1

 open UIKit
 open Foundation

 [<Register(nameof SceneDelegate)>]
 type SceneDelegate() =
    inherit UIResponder()
    interface IUIWindowSceneDelegate

    [<Export("window")>]
    member val Window : UIWindow = null with get, set

    [<Export("scene:willConnectToSession:options:")>]
    member _.WillConnect(scene: UIScene, session: UISceneSession, connectionOptions: UISceneConnectionOptions) =
        // Use this method to optionally configure and attach the UIWindow `window` to the provided UIWindowScene `scene`.
        // If using a storyboard, the `window` property will automatically be initialized and attached to the scene.
        // This delegate does not imply the connecting scene or session are new (see UIApplicationDelegate `GetConfiguration` instead).
        ()


    [<Export("sceneDidDisconnect:")>]
    member _.DidDisconnect(scene: UIScene) =
        // Called as the scene is being released by the system.
        // This occurs shortly after the scene enters the background, or when its session is discarded.
        // Release any resources associated with this scene that can be re-created the next time the scene connects.
        // The scene may re-connect later, as its session was not neccessarily discarded (see UIApplicationDelegate `DidDiscardSceneSessions` instead).
        ()

    [<Export("sceneDidBecomeActive:")>]
    member _.DidBecomeActive(scene: UIScene) =
        // Called when the scene has moved from an inactive state to an active state.
        // Use this method to restart any tasks that were paused (or not yet started) when the scene was inactive.
        ()

    [<Export("sceneWillResignActive:")>]
    member _.WillResignActive(scene: UIScene) =
        // Called when the scene will move from an active state to an inactive state.
        // This may occur due to temporary interruptions (ex. an incoming phone call).
        ()

    [<Export("sceneWillEnterForeground:")>]
    member _.WillEnterForeground(scene: UIScene) =
        // Called as the scene transitions from the background to the foreground.
        // Use this method to undo the changes made on entering the background.
        ()

    [<Export("sceneDidEnterBackground:")>]
    member _.DidEnterBackground(scene: UIScene) =
        // Called as the scene transitions from the foreground to the background.
        // Use this method to save data, release shared resources, and store enough scene-specific state information
        // to restore the scene back to its current state.
        ()