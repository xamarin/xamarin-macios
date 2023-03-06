//
// Enums.cs: enums for SpriteKit
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013-2014 Xamarin Inc

using ObjCRuntime;

#nullable enable

namespace SpriteKit {

	// NSInteger -> SKKeyframeSequence.h
	[Native]
	public enum SKInterpolationMode : long {
		Linear = 1,
		Spline = 2,
		Step = 3
	}

	// NSInteger -> SKKeyframeSequence.h
	[Native]
	public enum SKRepeatMode : long {
		Clamp = 1,
		Loop = 2
	}

	// NSInteger -> SKAction.h
	[Native]
	public enum SKActionTimingMode : long {
		Linear = 0,
		EaseIn = 1,
		EaseOut = 2,
		EaseInEaseOut = 3
	}

	// NSInteger -> SKLabelNode.h
	[Native]
	public enum SKLabelVerticalAlignmentMode : long {
		Baseline = 0,
		Center = 1,
		Top = 2,
		Bottom = 3
	}

	// NSInteger -> SKLabelNode.h
	[Native]
	public enum SKLabelHorizontalAlignmentMode : long {
		Center = 0,
		Left = 1,
		Right = 2
	}

	// NSInteger -> SKNode.h
	[Native]
	public enum SKBlendMode : long {
		Alpha = 0,
		Add = 1,
		Subtract = 2,
		Multiply = 3,
		MultiplyX2 = 4,
		Screen = 5,
		Replace = 6,
		MultiplyAlpha = 7,
	}

	// NSInteger -> SKScene.h
	[Native]
	public enum SKSceneScaleMode : long {
		Fill = 0,
		AspectFill = 1,
		AspectFit = 2,
		ResizeFill = 3
	}

	// NSInteger -> SKTexture.h
	[Native]
	public enum SKTextureFilteringMode : long {
		Nearest = 0,
		Linear = 1
	}

	// NSInteger -> SKTransition.h
	[Native]
	public enum SKTransitionDirection : long {
		Up = 0,
		Down = 1,
		Right = 2,
		Left = 3
	}

	[Native]
	public enum SKUniformType : long {
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
	public enum SKParticleRenderOrder : ulong {
		OldestLast,
		OldestFirst,
		DontCare,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum SKAttributeType : long {
		None = 0,
		Float = 1,
		VectorFloat2 = 2,
		VectorFloat3 = 3,
		VectorFloat4 = 4,
		HalfFloat = 5,
		VectorHalfFloat2 = 6,
		VectorHalfFloat3 = 7,
		VectorHalfFloat4 = 8,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum SKTileDefinitionRotation : ulong {
		Angle0 = 0,
		Angle90,
		Angle180,
		Angle270,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum SKTileSetType : ulong {
		Grid,
		Isometric,
		HexagonalFlat,
		HexagonalPointy,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum SKTileAdjacencyMask : ulong {
		Up = 1 << 0,
		UpperRight = 1 << 1,
		Right = 1 << 2,
		LowerRight = 1 << 3,
		Down = 1 << 4,
		LowerLeft = 1 << 5,
		Left = 1 << 6,
		UpperLeft = 1 << 7,
		All = Up | UpperRight | Right | LowerRight | Down | LowerLeft | Left | UpperLeft,
		HexFlatUp = 1 << 0,
		HexFlatUpperRight = 1 << 1,
		HexFlatLowerRight = 1 << 2,
		HexFlatDown = 1 << 3,
		HexFlatLowerLeft = 1 << 4,
		HexFlatUpperLeft = 1 << 5,
		HexFlatAll = HexFlatUp | HexFlatUpperRight | HexFlatLowerRight | HexFlatDown | HexFlatLowerLeft | HexFlatUpperLeft,
		HexPointyUpperLeft = 1 << 0,
		HexPointyUpperRight = 1 << 1,
		HexPointyRight = 1 << 2,
		HexPointyLowerRight = 1 << 3,
		HexPointyLowerLeft = 1 << 4,
		HexPointyLeft = 1 << 5,
		HexPointyAll = HexPointyUpperLeft | HexPointyUpperRight | HexPointyRight | HexPointyLowerRight | HexPointyLowerLeft | HexPointyLeft,
		UpEdge = Right | LowerRight | Down | LowerLeft | Left,
		UpperRightEdge = Down | LowerLeft | Left,
		RightEdge = Down | LowerLeft | Left | UpperLeft | Up,
		LowerRightEdge = Left | UpperLeft | Up,
		DownEdge = Up | UpperRight | Right | Left | UpperLeft,
		LowerLeftEdge = Up | UpperRight | Right,
		LeftEdge = Up | UpperRight | Right | LowerRight | Down,
		UpperLeftEdge = Right | LowerRight | Down,
		UpperRightCorner = Up | UpperRight | Right | LowerRight | Down | Left | UpperLeft,
		LowerRightCorner = Up | UpperRight | Right | LowerRight | Down | LowerLeft | Left,
		LowerLeftCorner = Up | Right | LowerRight | Down | LowerLeft | Left | UpperLeft,
		UpperLeftCorner = Up | UpperRight | Right | Down | LowerLeft | Left | UpperLeft,
	}

	[NoMac]
	[Watch (9, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKNodeFocusBehavior : long {
		None = 0,
		Occluding,
		Focusable,
	}
}
