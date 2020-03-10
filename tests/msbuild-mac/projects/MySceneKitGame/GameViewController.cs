using System;
using System.Collections.Generic;

using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace MySceneKitGame {
	[Register ("GameViewController")]
	public partial class GameViewController : NSViewController {
		public override void AwakeFromNib ()
		{
			// create a new scene
			var scene = SCNScene.FromFile ("art.scnassets/ship");

			// create and add a camera to the scene
			var cameraNode = SCNNode.Create ();
			cameraNode.Camera = SCNCamera.Create ();
			scene.RootNode.AddChildNode (cameraNode);

			// place the camera
			cameraNode.Position = new SCNVector3 (0, 0, 15);

			// create and add a light to the scene
			var lightNode = SCNNode.Create ();
			lightNode.Light = SCNLight.Create ();
			lightNode.Light.LightType = SCNLightType.Omni;
			lightNode.Position = new SCNVector3 (0, 10, 10);
			scene.RootNode.AddChildNode (lightNode);

			// create and add an ambient light to the scene
			var ambientLightNode = SCNNode.Create ();
			ambientLightNode.Light = SCNLight.Create ();
			ambientLightNode.Light.LightType = SCNLightType.Ambient;
			ambientLightNode.Light.Color = NSColor.DarkGray;
			scene.RootNode.AddChildNode (ambientLightNode);

			// retrieve the ship node
			var ship = scene.RootNode.FindChildNode ("ship", true);

			// animate the 3d object
			var animation = CABasicAnimation.FromKeyPath ("rotation");
			animation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (nfloat) Math.PI * 2));
			animation.Duration = 3;
			animation.RepeatCount = float.MaxValue; //repeat forever
			ship.AddAnimation (animation, null);

			// set the scene to the view
			MyGameView.Scene = scene;

			// allows the user to manipulate the camera
			MyGameView.AllowsCameraControl = true;

			// show statistics such as fps and timing information
			MyGameView.ShowsStatistics = true;

			// configure the view
			MyGameView.BackgroundColor = NSColor.Black;
		}
	}
}
