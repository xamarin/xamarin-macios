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
	/// <summary>An enumeration whose values specify the interpolation mode of a <see cref="T:SpriteKit.SKKeyframeSequence" />.</summary>
	[Native]
	public enum SKInterpolationMode : long {
		Linear = 1,
		Spline = 2,
		Step = 3
	}

	// NSInteger -> SKKeyframeSequence.h
	/// <summary>An enumeration whose values specify whether the time value of a <see cref="T:SpriteKit.SKKeyframeSequence" /> should cycle.</summary>
	[Native]
	public enum SKRepeatMode : long {
		Clamp = 1,
		Loop = 2
	}

	// NSInteger -> SKAction.h
	/// <summary>An enumeration whose values specify the time-varying behavior of a <see cref="T:SpriteKit.SKAction" />. Used with <see cref="P:SpriteKit.SKAction.TimingMode" />.</summary>
	[Native]
	public enum SKActionTimingMode : long {
		Linear = 0,
		EaseIn = 1,
		EaseOut = 2,
		EaseInEaseOut = 3
	}

	// NSInteger -> SKLabelNode.h
	/// <summary>
	///  An enumeration whose values specify vertical alignment of a <see cref="T:SpriteKit.SKLabelNode" />. Used with <see cref="P:SpriteKit.SKLabelNode.VerticalAlignmentMode" /></summary>
	[Native]
	public enum SKLabelVerticalAlignmentMode : long {
		Baseline = 0,
		Center = 1,
		Top = 2,
		Bottom = 3
	}

	// NSInteger -> SKLabelNode.h
	/// <summary>An enumeration whose values specify horizontal alignment of a <see cref="T:SpriteKit.SKLabelNode" />. Used with <see cref="P:SpriteKit.SKLabelNode.HorizontalAlignmentMode" /></summary>
	[Native]
	public enum SKLabelHorizontalAlignmentMode : long {
		Center = 0,
		Left = 1,
		Right = 2
	}

	// NSInteger -> SKNode.h
	/// <summary>An enumeration whose values specify options for blending of visual <see cref="T:SpriteKit.SKNode" />s or particles.</summary>
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
	/// <summary>An enumeration whose values specify the way in which a <see cref="T:SpriteKit.SKScene" /> scales to the view in which it is being displayed.</summary>
	[Native]
	public enum SKSceneScaleMode : long {
		Fill = 0,
		AspectFill = 1,
		AspectFit = 2,
		ResizeFill = 3
	}

	// NSInteger -> SKTexture.h
	/// <summary>An enumeration whose values specify how a <see cref="T:SpriteKit.SKTexture" /> is rendered on a <see cref="T:SpriteKit.SKSpriteNode" /> of a different size.</summary>
	[Native]
	public enum SKTextureFilteringMode : long {
		Nearest = 0,
		Linear = 1
	}

	// NSInteger -> SKTransition.h
	/// <summary>An enumeration of directions for use with <see cref="T:SpriteKit.SKTransition" />s.</summary>
	[Native]
	public enum SKTransitionDirection : long {
		Up = 0,
		Down = 1,
		Right = 2,
		Left = 3
	}

	/// <summary>Contains values that describe the data with which an <see cref="T:SpriteKit.SKUniform" /> was initialized.</summary>
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

	/// <summary>Enumerates values used with <see cref="P:SpriteKit.SKEmitterNode.ParticleRenderOrder" />.</summary>
	[Native]
	public enum SKParticleRenderOrder : ulong {
		OldestLast,
		OldestFirst,
		DontCare,
	}

	/// <summary>Enumeration of valid types for <see cref="T:SpriteKit.SKAttribute" /> values.</summary>
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

	/// <summary>Enumerates how a <see cref="T:SpriteKit.SKTileDefinition" /> kind may be rotated.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKTileDefinitionRotation : ulong {
		Angle0 = 0,
		Angle90,
		Angle180,
		Angle270,
	}

	/// <summary>Enumerates supported tiling schemes.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKTileSetType : ulong {
		Grid,
		Isometric,
		HexagonalFlat,
		HexagonalPointy,
	}

	/// <summary>Enumerates how neighboring tiles may be automatically placed.</summary>
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

	/// <summary>Enumerates the various ways a <see cref="T:SpriteKit.SKNode" /> may be focusable.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKNodeFocusBehavior : long {
		None = 0,
		Occluding,
		Focusable,
	}
}
