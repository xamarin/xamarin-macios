//
// Defs.cs: Enumerations and core types
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2012-2014, 2016 Xamarin, Inc.
//

using System;

using ObjCRuntime;

using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;

namespace SceneKit {

	[Watch (3,0)]
	[iOS (8, 0)]
	[Native] // untyped enum (SceneKitTypes.h) but described as the value of `code` for `NSError` which is an NSInteger
	[ErrorDomain ("SCNErrorDomain")]
	public enum SCNErrorCode : long {
		ProgramCompilationError = 1,
	}

	[Watch (3,0)]
	[iOS (8, 0)]
	[Native]
	public enum SCNGeometryPrimitiveType : long {
		Triangles,
		TriangleStrip,
		Line,
		Point,
		[TV (10,0), Mac (10,12), iOS (10,0)]
		Polygon
	}

	[Watch (3,0)]
	[iOS (8, 0)]
	[Native]
	public enum SCNTransparencyMode : long {
		AOne,
		RgbZero,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		SingleLayer = 2,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		DualLayer = 3,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		Default = AOne,
	}

	[Watch (3,0)]
	[iOS (8, 0)]
	[Native]
	public enum SCNCullMode : long {
		Back, Front
	}

	[Watch (3,0)]
	[iOS (8, 0)]
	[Native]
	public enum SCNFilterMode : long {
		None,
		Nearest,
		Linear
	}

	[Watch (3,0)]
	[iOS (8, 0)]
	[Native]
	public enum SCNWrapMode : long {
		Clamp = 1,
		Repeat,
		// added in iOS 8, removed in 8.3 (mistake?) but added back in 9.0 betas
		ClampToBorder,
		Mirror
	}

	[Watch (3,0)]
	[iOS (8, 0)]
	[Native]
	public enum SCNSceneSourceStatus : long {
		Error = -1,
		Parsing = 4,
		Validating = 8,
		Processing = 12,
		Complete = 16
	}

	[Watch (3,0)]
	[Mac (10, 9)]
	[iOS (8, 0)]
	[Native]
	public enum SCNChamferMode : long {
		Both,
		Front,
		Back
	}

	[Watch (3,0)]
	[Mac (10, 9)]
	[iOS (8, 0)]
	[Native]
	public enum SCNMorpherCalculationMode : long {
		Normalized,
		Additive
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNActionTimingMode : long {
		Linear,
		EaseIn,
		EaseOut,
		EaseInEaseOut
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNShadowMode : long {
		Forward,
		Deferred,
		Modulated
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNPhysicsBodyType : long {
		Static,
		Dynamic,
		Kinematic
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNPhysicsFieldScope : long {
		InsideExtent,
		OutsideExtent
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleSortingMode : long {
		None,
		ProjectedDepth,
		Distance,
		OldestFirst,
		YoungestFirst
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleBlendMode : long {
		Additive,
		Subtract,
		Multiply,
		Screen,
		Alpha,
		Replace
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleOrientationMode : long {
		BillboardScreenAligned,
		BillboardViewAligned,
		Free,
		BillboardYAligned
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleBirthLocation : long {
		Surface,
		Volume,
		Vertex
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleBirthDirection : long {
		Constant,
		SurfaceNormal,
		Random
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleImageSequenceAnimationMode : long {
		Repeat,
		Clamp,
		AutoReverse
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleInputMode : long {
		OverLife,
		OverDistance,
		OverOtherProperty
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleModifierStage : long {
		PreDynamics,
		PostDynamics,
		PreCollision,
		PostCollision
	}

	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum SCNParticleEvent : long {
		Birth,
		Death,
		Collision
	}

	// Utility enum
	public enum SCNGeometrySourceSemantics
	{
		Vertex,
		Normal,
		Color,
		Texcoord,
		VertexCrease,
		EdgeCrease,
		BoneWeights,
		BoneIndices
	}

	// Utility enum
	public enum SCNAnimationImportPolicy
	{
		Unknown,
		Play,
		PlayRepeatedly,
		DoNotPlay,
		PlayUsingSceneTimeBase
	}

	// Utility enum
	public enum SCNPhysicsSearchMode {
		Unknown = -1,
		Any, Closest, All, 
	}

	[Watch (3,0)]
	[Native]
	public enum SCNAntialiasingMode : ulong {
		None,
		Multisampling2X,
		Multisampling4X,
#if MONOMAC
		Multisampling8X,
		Multisampling16X,
#endif
	}

	[Watch (3,0)]
	[Native]
	public enum SCNPhysicsCollisionCategory : ulong {
		None	= 0,
		Default	= 1 << 0,
		Static	= 1 << 1,
#if XAMCORE_2_0
		All		= UInt64.MaxValue
#else
		All		= UInt32.MaxValue
#endif
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum SCNBillboardAxis : ulong {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2,
		All = (X | Y | Z)
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum SCNReferenceLoadingPolicy : long {
		Immediate = 0,
		OnDemand = 1
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum SCNBlendMode : long
	{
		Alpha = 0,
		Add = 1,
		Subtract = 2,
		Multiply = 3,
		Screen = 4,
		Replace = 5,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		Max = 6,
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Native]
	[Flags]
	public enum SCNDebugOptions : ulong
	{
		None = 0,
		ShowPhysicsShapes = 1 << 0,
		ShowBoundingBoxes = 1 << 1,
		ShowLightInfluences = 1 << 2,
		ShowLightExtents = 1 << 3,
		ShowPhysicsFields = 1 << 4,
		ShowWireframe = 1 << 5,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		RenderAsWireframe = 1 << 6,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		ShowSkeletons = 1 << 7,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		ShowCreases = 1 << 8,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		ShowConstraints = 1 << 9,
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		ShowCameras = 1 << 10,
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum SCNRenderingApi : ulong
	{
		Metal,
#if !MONOMAC
		OpenGLES2,
#else
		OpenGLLegacy,
		OpenGLCore32,
		OpenGLCore41
#endif
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum SCNBufferFrequency : long
	{
		Frame = 0,
		Node = 1,
		Shadable = 2,
	}

	[Watch (3,0)]
	[TV (10, 0), Mac (10, 12), iOS (10, 0)]
	[Native]
	public enum SCNMovabilityHint : long {
		Fixed,
		Movable
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	[Flags]
	public enum SCNColorMask : long
	{
		None = 0,
		Red = 1 << 3,
		Green = 1 << 2,
		Blue = 1 << 1,
		Alpha = 1 << 0,
		All = 15,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNInteractionMode : long
	{
		Fly,
		OrbitTurntable,
		OrbitAngleMapping,
		OrbitCenteredArcball,
		OrbitArcball,
		Pan,
		Truck,
	}
		
	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNFillMode : ulong
	{
		Fill = 0,
		Lines = 1,
	}

	[NoWatch, Mac (10,13), iOS (11,0)]
	[TV (12,0)]
	[Native]
	public enum SCNTessellationSmoothingMode : long
	{
		None = 0,
		PNTriangles,
		Phong,
	}
	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNHitTestSearchMode : long
	{
		Closest = 0,
		All = 1,
		Any = 2,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNCameraProjectionDirection : long
	{
		Vertical = 0,
		Horizontal = 1,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNNodeFocusBehavior : long
	{
		None = 0,
		Occluding,
		Focusable,
	}
}
