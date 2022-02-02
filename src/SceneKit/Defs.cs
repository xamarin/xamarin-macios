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

using Foundation;
using ObjCRuntime;

using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;

#nullable enable

namespace SceneKit {

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[iOS (8, 0)]
#endif
	[Native] // untyped enum (SceneKitTypes.h) but described as the value of `code` for `NSError` which is an NSInteger
	[ErrorDomain ("SCNErrorDomain")]
	public enum SCNErrorCode : long {
		ProgramCompilationError = 1,
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNGeometryPrimitiveType : long {
		Triangles,
		TriangleStrip,
		Line,
		Point,
#if NET
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("ios10.0")]
#else
		[TV (10,0)]
		[Mac (10,12)]
		[iOS (10,0)]
#endif
		Polygon
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNTransparencyMode : long {
		AOne,
		RgbZero,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		SingleLayer = 2,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		DualLayer = 3,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		Default = AOne,
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNCullMode : long {
		Back, Front
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNFilterMode : long {
		None,
		Nearest,
		Linear
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNWrapMode : long {
		Clamp = 1,
		Repeat,
		// added in iOS 8, removed in 8.3 (mistake?) but added back in 9.0 betas
		ClampToBorder,
		Mirror
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNSceneSourceStatus : long {
		Error = -1,
		Parsing = 4,
		Validating = 8,
		Processing = 12,
		Complete = 16
	}

#if NET
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 9)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNChamferMode : long {
		Both,
		Front,
		Back
	}

#if NET
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 9)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNMorpherCalculationMode : long {
		Normalized,
		Additive
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNActionTimingMode : long {
		Linear,
		EaseIn,
		EaseOut,
		EaseInEaseOut
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNShadowMode : long {
		Forward,
		Deferred,
		Modulated
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNPhysicsBodyType : long {
		Static,
		Dynamic,
		Kinematic
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNPhysicsFieldScope : long {
		InsideExtent,
		OutsideExtent
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNParticleSortingMode : long {
		None,
		ProjectedDepth,
		Distance,
		OldestFirst,
		YoungestFirst
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNParticleBlendMode : long {
		Additive,
		Subtract,
		Multiply,
		Screen,
		Alpha,
		Replace
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNParticleOrientationMode : long {
		BillboardScreenAligned,
		BillboardViewAligned,
		Free,
		BillboardYAligned
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNParticleBirthLocation : long {
		Surface,
		Volume,
		Vertex
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNParticleBirthDirection : long {
		Constant,
		SurfaceNormal,
		Random
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNParticleImageSequenceAnimationMode : long {
		Repeat,
		Clamp,
		AutoReverse
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNParticleInputMode : long {
		OverLife,
		OverDistance,
		OverOtherProperty
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum SCNParticleModifierStage : long {
		PreDynamics,
		PostDynamics,
		PreCollision,
		PostCollision
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Watch (3,0)]
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
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

#if !NET
	[Watch (3,0)]
#endif
	[Native]
	public enum SCNAntialiasingMode : ulong {
		None,
		Multisampling2X,
		Multisampling4X,
#if MONOMAC || __MACCATALYST__
		Multisampling8X,
		Multisampling16X,
#endif
	}

#if !NET
	[Watch (3,0)]
#endif
	[Native]
	public enum SCNPhysicsCollisionCategory : ulong {
		None	= 0,
		Default	= 1 << 0,
		Static	= 1 << 1,
		All		= UInt64.MaxValue
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[Watch (3,0)]
	[iOS (9,0)]
	[Mac (10,11)]
#endif
	[Native]
	public enum SCNBillboardAxis : ulong {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2,
		All = (X | Y | Z)
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[Watch (3,0)]
	[iOS (9,0)]
	[Mac (10,11)]
#endif
	[Native]
	public enum SCNReferenceLoadingPolicy : long {
		Immediate = 0,
		OnDemand = 1
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[Watch (3,0)]
	[iOS (9,0)]
	[Mac (10,11)]
#endif
	[Native]
	public enum SCNBlendMode : long
	{
		Alpha = 0,
		Add = 1,
		Subtract = 2,
		Multiply = 3,
		Screen = 4,
		Replace = 5,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		Max = 6,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[Watch (3,0)]
	[iOS (9,0)]
	[Mac (10,11)]
#endif
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
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		RenderAsWireframe = 1 << 6,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		ShowSkeletons = 1 << 7,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		ShowCreases = 1 << 8,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		ShowConstraints = 1 << 9,
#if NET
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("ios11.0")]
#else
		[Watch (4,0)]
		[TV (11,0)]
		[Mac (10,13)]
		[iOS (11,0)]
#endif
		ShowCameras = 1 << 10,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[Watch (3,0)]
	[iOS (9,0)]
	[Mac (10,11)]
#endif
	[Native]
	public enum SCNRenderingApi : ulong
	{
		Metal,
#if !MONOMAC
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Unavailable (PlatformName.MacCatalyst)]
#endif
		OpenGLES2,
#else
		OpenGLLegacy,
		OpenGLCore32,
		OpenGLCore41
#endif
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
#else
	[Watch (3,0)]
	[iOS (9,0)]
	[Mac (10,11)]
#endif
	[Native]
	public enum SCNBufferFrequency : long
	{
		Frame = 0,
		Node = 1,
		Shadable = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("ios10.0")]
#else
	[Watch (3,0)]
	[TV (10, 0)]
	[Mac (10, 12)]
	[iOS (10, 0)]
#endif
	[Native]
	public enum SCNMovabilityHint : long {
		Fixed,
		Movable
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[Mac (10,13)]
	[iOS (11,0)]
#endif
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

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[Mac (10,13)]
	[iOS (11,0)]
#endif
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
		
#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[Mac (10,13)]
	[iOS (11,0)]
#endif
	[Native]
	public enum SCNFillMode : ulong
	{
		Fill = 0,
		Lines = 1,
	}

#if NET
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos12.0")]
#else
	[NoWatch]
	[Mac (10,13)]
	[iOS (11,0)]
	[TV (12,0)]
#endif
	[Native]
	public enum SCNTessellationSmoothingMode : long
	{
		None = 0,
		PNTriangles,
		Phong,
	}
#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[Mac (10,13)]
	[iOS (11,0)]
#endif
	[Native]
	public enum SCNHitTestSearchMode : long
	{
		Closest = 0,
		All = 1,
		Any = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[Mac (10,13)]
	[iOS (11,0)]
#endif
	[Native]
	public enum SCNCameraProjectionDirection : long
	{
		Vertical = 0,
		Horizontal = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[Watch (4,0)]
	[TV (11,0)]
	[Mac (10,13)]
	[iOS (11,0)]
#endif
	[Native]
	public enum SCNNodeFocusBehavior : long
	{
		None = 0,
		Occluding,
		Focusable,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[Watch (6,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
#endif
	[Native]
	public enum SCNLightProbeType : long
	{
		Irradiance = 0,
		Radiance = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[Watch (6,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
#endif
	[Native]
	public enum SCNLightProbeUpdateType : long
	{
		Never = 0,
		Realtime = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[Watch (6,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
#endif
	[Native]
	public enum SCNLightAreaType : long
	{
		Rectangle = 1,
		Polygon = 4,
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	public enum SCNPhysicsShapeType
	{
		ConvexHull,
		BoundingBox,
		ConcavePolyhedron,
	}
}
