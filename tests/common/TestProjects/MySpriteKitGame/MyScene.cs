using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace MySpriteKitGame {
	public class MyScene : SKScene {
		public MyScene (CGSize size) : base (size)
		{
			// Setup your scene here
			BackgroundColor = new UIColor (0.15f, 0.15f, 0.3f, 1.0f);

			var myLabel = new SKLabelNode ("Chalkduster") {
				Text = "Hello, World!",
				FontSize = 30,
				Position = new CGPoint (Frame.Width / 2, Frame.Height / 2)
			};

			AddChild (myLabel);
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			// Called when a touch begins
			foreach (var touch in touches) {
				var location = ((UITouch) touch).LocationInNode (this);

				var sprite = new SKSpriteNode ("Spaceship") {
					Position = location,
					XScale = 0.4f,
					YScale = 0.4f
				};

				var action = SKAction.RotateByAngle ((float) Math.PI, 1.0);

				sprite.RunAction (SKAction.RepeatActionForever (action));

				AddChild (sprite);
			}
		}

		public override void Update (double currentTime)
		{
			// Run before each frame is rendered
			base.Update (currentTime);
		}
	}
}
