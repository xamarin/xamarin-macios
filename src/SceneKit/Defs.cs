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

using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;

namespace XamCore.SceneKit {

	[Watch (3,0)]
	[Availability (Platform.Mac_10_8 | Platform.iOS_8_0)]
	[Native] // untyped enum (SceneKitTypes.h) but described as the value of `code` for `NSError` which is an NSInteger
	[ErrorDomain ("SCNErrorDomain")]
	public enum SCNErrorCode : nint {
		ProgramCompilationError = 1,
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_8 | Platform.iOS_8_0)]
	[Native]
	public enum SCNGeometryPrimitiveType : nint {
		Triangles,
		TriangleStrip,
		Line,
		Point,
		[TV (10,0), Mac (10,12), iOS (10,0)]
		Polygon
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_8 | Platform.iOS_8_0)]
	[Native]
	public enum SCNTransparencyMode : nint {
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
	[Availability (Platform.Mac_10_8 | Platform.iOS_8_0)]
	[Native]
	public enum SCNCullMode : nint {
		Back, Front
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_8 | Platform.iOS_8_0)]
	[Native]
	public enum SCNFilterMode : nint {
		None,
		Nearest,
		Linear
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_8 | Platform.iOS_8_0)]
	[Native]
	public enum SCNWrapMode : nint {
		Clamp = 1,
		Repeat,
		// added in iOS 8, removed in 8.3 (mistake?) but added back in 9.0 betas
		ClampToBorder,
		Mirror
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_8 | Platform.iOS_8_0)]
	[Native]
	public enum SCNSceneSourceStatus : nint {
		Error = -1,
		Parsing = 4,
		Validating = 8,
		Processing = 12,
		Complete = 16
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_9 | Platform.iOS_8_0)]
	[Native]
	public enum SCNChamferMode : nint {
		Both,
		Front,
		Back
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_9 | Platform.iOS_8_0)]
	[Native]
	public enum SCNMorpherCalculationMode : nint {
		Normalized,
		Additive
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNActionTimingMode : nint {
		Linear,
		EaseIn,
		EaseOut,
		EaseInEaseOut
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNShadowMode : nint {
		Forward,
		Deferred,
		Modulated
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNPhysicsBodyType : nint {
		Static,
		Dynamic,
		Kinematic
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNPhysicsFieldScope : nint {
		InsideExtent,
		OutsideExtent
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleSortingMode : nint {
		None,
		ProjectedDepth,
		Distance,
		OldestFirst,
		YoungestFirst
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleBlendMode : nint {
		Additive,
		Subtract,
		Multiply,
		Screen,
		Alpha,
		Replace
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleOrientationMode : nint {
		BillboardScreenAligned,
		BillboardViewAligned,
		Free,
		BillboardYAligned
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleBirthLocation : nint {
		Surface,
		Volume,
		Vertex
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleBirthDirection : nint {
		Constant,
		SurfaceNormal,
		Random
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleImageSequenceAnimationMode : nint {
		Repeat,
		Clamp,
		AutoReverse
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleInputMode : nint {
		OverLife,
		OverDistance,
		OverOtherProperty
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleModifierStage : nint {
		PreDynamics,
		PostDynamics,
		PreCollision,
		PostCollision
	}

	[Watch (3,0)]
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum SCNParticleEvent : nint {
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
	public enum SCNAntialiasingMode : nuint {
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
	public enum SCNPhysicsCollisionCategory : nuint {
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
	public enum SCNBillboardAxis : nuint {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2,
		All = (X | Y | Z)
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum SCNReferenceLoadingPolicy : nint {
		Immediate = 0,
		OnDemand = 1
	}

	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum SCNBlendMode : nint
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
	public enum SCNDebugOptions : nuint
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
	public enum SCNRenderingApi : nuint
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
	public enum SCNBufferFrequency : nint
	{
		Frame = 0,
		Node = 1,
		Shadable = 2,
	}

	[Watch (3,0)]
	[TV (10, 0), Mac (10, 12), iOS (10, 0)]
	[Native]
	public enum SCNMovabilityHint : nint {
		Fixed,
		Movable
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	[Flags]
	public enum SCNColorMask : nint
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
	public enum SCNInteractionMode : nint
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
	public enum SCNFillMode : nuint
	{
		Fill = 0,
		Lines = 1,
	}

	[NoWatch, NoTV, Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNTessellationSmoothingMode : nint
	{
		None = 0,
		PNTriangles,
		Phong,
	}
	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNHitTestSearchMode : nint
	{
		Closest = 0,
		All = 1,
		Any = 2,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNCameraProjectionDirection : nint
	{
		Vertical = 0,
		Horizontal = 1,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum SCNNodeFocusBehavior : nint
	{
		None = 0,
		Occluding,
		Focusable,
	}
}
