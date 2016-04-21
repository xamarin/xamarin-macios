//
// Enums.cs: enums for SpriteKit
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013-2014 Xamarin Inc

using XamCore.ObjCRuntime;

namespace XamCore.SpriteKit {

	// NSInteger -> SKKeyframeSequence.h
	[Native]
	public enum SKInterpolationMode : nint {
		Linear = 1,
		Spline = 2,
		Step = 3
	}

	// NSInteger -> SKKeyframeSequence.h
	[Native]
	public enum SKRepeatMode : nint {
		Clamp = 1,
		Loop = 2
	}

	// NSInteger -> SKAction.h
	[Native]
	public enum SKActionTimingMode : nint {
		Linear = 0,
		EaseIn = 1,
		EaseOut = 2,
		EaseInEaseOut = 3
	}

	// NSInteger -> SKLabelNode.h
	[Native]
	public enum SKLabelVerticalAlignmentMode : nint {
		Baseline = 0,
		Center = 1,
		Top = 2,
		Bottom = 3
	}

	// NSInteger -> SKLabelNode.h
	[Native]
	public enum SKLabelHorizontalAlignmentMode : nint {
		Center = 0,
		Left = 1,
		Right = 2
	}

	// NSInteger -> SKNode.h
	[Native]
	public enum SKBlendMode : nint {
		Alpha = 0,
		Add = 1,
		Subtract = 2,
		Multiply = 3,
		MultiplyX2 = 4,
		Screen = 5,
		Replace = 6
	}

	// NSInteger -> SKScene.h
	[Native]
	public enum SKSceneScaleMode : nint {
		Fill = 0,
		AspectFill = 1,
		AspectFit = 2,
		ResizeFill = 3
	}

	// NSInteger -> SKTexture.h
	[Native]
	public enum SKTextureFilteringMode : nint {
		Nearest = 0,
		Linear = 1
	}

	// NSInteger -> SKTransition.h
	[Native]
	public enum SKTransitionDirection : nint {
		Up = 0,
		Down = 1,
		Right = 2,
		Left = 3
	}

	[Native]
	public enum SKUniformType : nint {
		None,
		Float,
		FloatVector2,
		FloatVector3,
		FloatVector4,
		FloatMatrix2,
		FloatMatrix3,
		FloatMatrix4,
		Texture,
	}

	[Native]
	public enum SKParticleRenderOrder : nuint {
		OldestLast,
		OldestFirst,
		DontCare,
	}
}
