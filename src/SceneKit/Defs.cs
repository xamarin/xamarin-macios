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

#nullable enable

namespace SceneKit {

	[MacCatalyst (13, 1)]
	[Native] // untyped enum (SceneKitTypes.h) but described as the value of `code` for `NSError` which is an NSInteger
	[ErrorDomain ("SCNErrorDomain")]
	public enum SCNErrorCode : long {
		ProgramCompilationError = 1,
	}

	/// <summary>Enumeration of 2D geometry primitives.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNGeometryPrimitiveType : long {
		Triangles,
		TriangleStrip,
		Line,
		Point,
		[MacCatalyst (13, 1)]
		Polygon,
	}

	/// <summary>Enumerates techniques for calculating transparency.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNTransparencyMode : long {
		AOne,
		RgbZero,
		[MacCatalyst (13, 1)]
		SingleLayer = 2,
		[MacCatalyst (13, 1)]
		DualLayer = 3,
		[MacCatalyst (13, 1)]
		Default = AOne,
	}

	/// <summary>Enumeration determining which faces of a surface <see cref="T:SceneKit.SCNMaterial" /> are rendered.</summary>
	///     <remarks>
	///       <para>See <see cref="P:SceneKit.SCNMaterial.CullMode" />.</para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNCullMode : long {
		Back,
		Front,
	}

	/// <summary>Enumeration of texture filtering modes.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNFilterMode : long {
		None,
		Nearest,
		Linear,
	}

	/// <summary>Enumerates texture-wrapping techniques.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNWrapMode : long {
		Clamp = 1,
		Repeat,
		// added in iOS 8, removed in 8.3 (mistake?) but added back in 9.0 betas
		ClampToBorder,
		Mirror,
	}

	/// <summary>Enumerates the states of an SCNSceneSource.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNSceneSourceStatus : long {
		Error = -1,
		Parsing = 4,
		Validating = 8,
		Processing = 12,
		Complete = 16,
	}

	/// <summary>Enumerates the ways a <see cref="T:SceneKit.SCNShape" /> can be chamfered; on its front, back, or both sides.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNChamferMode : long {
		Both,
		Front,
		Back,
	}

	/// <summary>Enumeration of valid interpolation formulae for <see cref="P:SceneKit.SCNMorpher.CalculationMode" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNMorpherCalculationMode : long {
		Normalized,
		Additive,
	}

	/// <summary>Enumerates rate curves for use with <see cref="T:SceneKit.SCNAction" /> objects.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNActionTimingMode : long {
		Linear,
		EaseIn,
		EaseOut,
		EaseInEaseOut,
	}

	/// <summary>Enumeration controlling when shadows are calculated.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNShadowMode : long {
		Forward,
		Deferred,
		Modulated,
	}

	/// <summary>An enumeration specifying whether the <see cref="T:SceneKit.SCNPhysicsBody" /> is dynamic, kinematic, or static. Used with <see cref="M:SceneKit.SCNPhysicsBody.CreateBody(SceneKit.SCNPhysicsBodyType,SceneKit.SCNPhysicsShape)" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNPhysicsBodyType : long {
		Static,
		Dynamic,
		Kinematic,
	}

	/// <summary>Enumerates values specifying whether an <see cref="T:SceneKit.SCNPhysicsField" /> affects objects inside or outside its border.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNPhysicsFieldScope : long {
		InsideExtent,
		OutsideExtent,
	}

	/// <summary>Enumeration specifying the order in which particles emitted by a <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Scene%20Kit%20SCNParticle%20Scene&amp;scope=Xamarin" title="T:SceneKit.SCNParticleScene">T:SceneKit.SCNParticleScene</a></format> are rendered.</summary>
	///     <remarks>
	///       <para>Along with <see cref="P:SceneKit.SCNParticleSystem.BlendMode" />, <see cref="P:SceneKit.SCNParticleSystem.SortingMode" /> affects the appearance of overlapping particles.</para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleSortingMode : long {
		None,
		ProjectedDepth,
		Distance,
		OldestFirst,
		YoungestFirst,
	}

	/// <summary>Enumeration of the ways in which overlapping particles emitted by a <see cref="T:SceneKit.SCNParticleSystem" /> will be rendered.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleBlendMode : long {
		Additive,
		Subtract,
		Multiply,
		Screen,
		Alpha,
		Replace,
	}

	/// <summary>Enumerates the alignment of particles emitted by a <see cref="T:SceneKit.SCNParticleSystem" />. Used with <see cref="P:SceneKit.SCNParticleSystem.OrientationMode" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleOrientationMode : long {
		BillboardScreenAligned,
		BillboardViewAligned,
		Free,
		BillboardYAligned,
	}

	/// <summary>Enumeration of the initial location of particles emitted by a <see cref="T:SceneKit.SCNParticleSystem" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleBirthLocation : long {
		Surface,
		Volume,
		Vertex,
	}

	/// <summary>Enumerates the initial direction of particles emitted by a <see cref="T:SceneKit.SCNParticleSystem" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleBirthDirection : long {
		Constant,
		SurfaceNormal,
		Random,
	}

	/// <summary>Enumeration of playing modes for <see cref="T:SceneKit.SCNParticleSystem" />'s whose particles are rendered as a sequence of images.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleImageSequenceAnimationMode : long {
		Repeat,
		Clamp,
		AutoReverse,
	}

	/// <summary>Enumerates how a particle property is animated (over the lifetime of the particle, as the particle travels over a distance, or based on another property). Used with <see cref="P:SceneKit.SCNParticlePropertyController.InputMode" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleInputMode : long {
		OverLife,
		OverDistance,
		OverOtherProperty,
	}

	/// <summary>Enumerates moments when the modifier specified in <see cref="M:SceneKit.SCNParticleSystem.AddModifier(Foundation.NSString[],SceneKit.SCNParticleModifierStage,SceneKit.SCNParticleModifierHandler)" /> should be applied.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleModifierStage : long {
		PreDynamics,
		PostDynamics,
		PreCollision,
		PostCollision,
	}

	/// <summary>Enumeration of lifecycle events for particles emitted by a <see cref="T:SceneKit.SCNParticleSystem" />. Used with <see cref="M:SceneKit.SCNParticleSystem.HandleEvent(SceneKit.SCNParticleEvent,Foundation.NSString[],SceneKit.SCNParticleEventHandler)" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNParticleEvent : long {
		Birth,
		Death,
		Collision,
	}

	// Utility enum
	/// <summary>Enumerates semantics for geometry data.</summary>
	public enum SCNGeometrySourceSemantics {
		Vertex,
		Normal,
		Color,
		Texcoord,
		VertexCrease,
		EdgeCrease,
		BoneWeights,
		BoneIndices,
	}

	// Utility enum
	/// <summary>Enumerates animation import policies.</summary>
	public enum SCNAnimationImportPolicy {
		Unknown,
		Play,
		PlayRepeatedly,
		DoNotPlay,
		PlayUsingSceneTimeBase,
	}

	// Utility enum
	/// <summary>Enumerates values that control which physics search results are returned.</summary>
	public enum SCNPhysicsSearchMode {
		Unknown = -1,
		Any,
		Closest,
		All,
	}

	/// <summary>Enumerates values that control antialiasing behavior.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNAntialiasingMode : ulong {
		None,
		Multisampling2X,
		Multisampling4X,
#if MONOMAC || __MACCATALYST__
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		Multisampling8X,
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		Multisampling16X,
#endif
	}

	/// <summary>Defaults for the collision properties of a <see cref="T:SceneKit.SCNPhysicsBody" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNPhysicsCollisionCategory : ulong {
		None = 0,
		Default = 1 << 0,
		Static = 1 << 1,
		All = UInt64.MaxValue,
	}

	/// <summary>Enumeration of axes' locks available to nodes constrained by <see cref="T:SceneKit.SCNBillboardConstraint" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNBillboardAxis : ulong {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2,
		All = (X | Y | Z),
	}

	/// <summary>Enumerates possible loading policies for <see cref="T:SceneKit.SCNReferenceNode" /> objects.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNReferenceLoadingPolicy : long {
		Immediate = 0,
		OnDemand = 1,
	}

	/// <summary>Enumeration of the ways SceneKit can blend colors from a material with colors that already exist in the render target.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNBlendMode : long {
		Alpha = 0,
		Add = 1,
		Subtract = 2,
		Multiply = 3,
		Screen = 4,
		Replace = 5,
		[MacCatalyst (13, 1)]
		Max = 6,
	}

	/// <summary>Enumerates debug overlay options.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum SCNDebugOptions : ulong {
		None = 0,
		ShowPhysicsShapes = 1 << 0,
		ShowBoundingBoxes = 1 << 1,
		ShowLightInfluences = 1 << 2,
		ShowLightExtents = 1 << 3,
		ShowPhysicsFields = 1 << 4,
		ShowWireframe = 1 << 5,
		[MacCatalyst (13, 1)]
		RenderAsWireframe = 1 << 6,
		[MacCatalyst (13, 1)]
		ShowSkeletons = 1 << 7,
		[MacCatalyst (13, 1)]
		ShowCreases = 1 << 8,
		[MacCatalyst (13, 1)]
		ShowConstraints = 1 << 9,
		[MacCatalyst (13, 1)]
		ShowCameras = 1 << 10,
	}

	/// <summary>Enumerates values that signify the Metal or OpenGLES2 APIs.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNRenderingApi : ulong {
		Metal,
#if !MONOMAC
		[Unavailable (PlatformName.MacCatalyst)]
		[NoMac]
		OpenGLES2,
#else
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		OpenGLLegacy,
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		OpenGLCore32,
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		OpenGLCore41,
#endif
	}

	/// <summary>Enumerates values that control whether handlers are invoked per frame, per node per frame, or per node per frame per shaded renderable.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNBufferFrequency : long {
		Frame = 0,
		Node = 1,
		Shadable = 2,
	}

	/// <summary>Enumerates values that tell SceneKit whether nodes are expected to move over time.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNMovabilityHint : long {
		Fixed,
		Movable,
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum SCNColorMask : long {
		None = 0,
		Red = 1 << 3,
		Green = 1 << 2,
		Blue = 1 << 1,
		Alpha = 1 << 0,
		All = 15,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNInteractionMode : long {
		Fly,
		OrbitTurntable,
		OrbitAngleMapping,
		OrbitCenteredArcball,
		OrbitArcball,
		Pan,
		Truck,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNFillMode : ulong {
		Fill = 0,
		Lines = 1,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNTessellationSmoothingMode : long {
		None = 0,
		PNTriangles,
		Phong,
	}
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNHitTestSearchMode : long {
		Closest = 0,
		All = 1,
		Any = 2,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNCameraProjectionDirection : long {
		Vertical = 0,
		Horizontal = 1,
	}

	/// <summary>Enumerates the focusable states of a <see cref="T:SceneKit.SCNNode" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNNodeFocusBehavior : long {
		None = 0,
		Occluding,
		Focusable,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNLightProbeType : long {
		Irradiance = 0,
		Radiance = 1,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNLightProbeUpdateType : long {
		Never = 0,
		Realtime = 1,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SCNLightAreaType : long {
		Rectangle = 1,
		Polygon = 4,
	}

	/// <summary>Enumeration of categories for <see cref="T:SceneKit.SCNPhysicsShape" />s.</summary>
	[MacCatalyst (13, 1)]
	public enum SCNPhysicsShapeType {
		ConvexHull,
		BoundingBox,
		ConcavePolyhedron,
	}
}
