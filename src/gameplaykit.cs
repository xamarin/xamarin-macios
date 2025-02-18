//
// GameplayKit bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.ComponentModel;
using ObjCRuntime;
using Foundation;
using SpriteKit;
using SceneKit;
#if NET
using MatrixFloat3x3 = global::CoreGraphics.NMatrix3;
using Vector2 = global::System.Numerics.Vector2;
using Vector3 = global::System.Numerics.Vector3;
using Vector2d = global::CoreGraphics.NVector2d;
using Vector2i = global::CoreGraphics.NVector2i;
using Vector3d = global::CoreGraphics.NVector3d;
#else
using Matrix3 = global::OpenTK.Matrix3;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using Vector2 = global::OpenTK.Vector2;
using Vector3 = global::OpenTK.Vector3;
using Vector2d = global::OpenTK.Vector2d;
using Vector2i = global::OpenTK.Vector2i;
using Vector3d = global::OpenTK.Vector3d;
#endif

#if MONOMAC
using SKColor = AppKit.NSColor;
#else
using SKColor = UIKit.UIColor;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace GameplayKit {

	/// <summary>Holds options for how nodes should be generated in a <see cref="T:GameplayKit.GKMeshGraph`1" />.</summary>
	[Native]
	[Flags]
	[MacCatalyst (13, 1)]
	public enum GKMeshGraphTriangulationMode : ulong {
		Vertices = 1 << 0,
		Centers = 1 << 1,
		EdgeMidpoints = 1 << 2
	}

	[Native]
	[MacCatalyst (13, 1)]
	public enum GKRTreeSplitStrategy : long {
		/// <summary>To be added.</summary>
		Halve = 0,
		/// <summary>To be added.</summary>
		Linear = 1,
		/// <summary>To be added.</summary>
		Quadratic = 2,
		/// <summary>To be added.</summary>
		ReduceOverlap = 3
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:GameplayKit.GKAgentDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:GameplayKit.GKAgentDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:GameplayKit.GKAgentDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:GameplayKit.GKAgentDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IGKAgentDelegate { }

	/// <summary>Delegate object that provides methods relating to synchronizing the state of a <see cref="T:GameplayKit.GKAgent" /> with external constraints, goals, and representations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKAgentDelegate_Protocol/index.html">Apple documentation for <c>GKAgentDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface GKAgentDelegate {

		[Export ("agentWillUpdate:")]
		void AgentWillUpdate (GKAgent agent);

		[Export ("agentDidUpdate:")]
		void AgentDidUpdate (GKAgent agent);
	}

	/// <summary>A <see cref="T:GameplayKit.GKComponent" /> that can move and has goals.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKAgent_Class/index.html">Apple documentation for <c>GKAgent</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKComponent))]
	interface GKAgent : NSSecureCoding {

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		IGKAgentDelegate Delegate { get; set; }

		[NullAllowed]
		[Export ("behavior", ArgumentSemantic.Retain)]
		GKBehavior Behavior { get; set; }

		[Export ("mass")]
		float Mass { get; set; }

		[Export ("radius")]
		float Radius { get; set; }

		[Export ("speed")]
		float Speed {
			get; [MacCatalyst (13, 1)]
			set;
		}

		[Export ("maxAcceleration")]
		float MaxAcceleration { get; set; }

		[Export ("maxSpeed")]
		float MaxSpeed { get; set; }
	}

	/// <summary>A <see cref="T:GameplayKit.GKAgent" /> whose movement is restricted to two dimensions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKAgent2D_Class/index.html">Apple documentation for <c>GKAgent2D</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKAgent))]
	interface GKAgent2D : NSSecureCoding {

		[Export ("position", ArgumentSemantic.Assign)]
		Vector2 Position {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Export ("velocity")]
		Vector2 Velocity {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("rotation")]
		float Rotation { get; set; }

		[Export ("updateWithDeltaTime:")]
		void Update (double deltaTimeInSeconds);
	}

	/// <summary>A 3D agent that responds to goals.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKAgent))]
	interface GKAgent3D {

		[Export ("position", ArgumentSemantic.Assign)]
		Vector3 Position {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Export ("velocity")]
		Vector3 Velocity {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("rightHanded")]
		bool RightHanded { get; set; }

#if !NET
		[Obsolete ("Use 'Rotation3x3' instead.")]
		[Export ("rotation", ArgumentSemantic.Assign)]
		Matrix3 Rotation {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}
#endif

		[Export ("rotation", ArgumentSemantic.Assign)]
#if NET
		MatrixFloat3x3 Rotation {
#else
		[Sealed]
		MatrixFloat3x3 Rotation3x3 {
#endif
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Export ("updateWithDeltaTime:")]
		void Update (double deltaTimeInSeconds);
	}

	// FIXME: @interface GKBehavior : NSObject <NSFastEnumeration>
	// Fix when we have NSFastEnumerator to IEnumerable support
	// https://bugzilla.xamarin.com/show_bug.cgi?id=4391
	/// <summary>A collection of <see cref="T:GameplayKit.GKGoal" /> objects and weights, together defining a cohesive game behavior.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKBehavior_Class/index.html">Apple documentation for <c>GKBehavior</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKBehavior : NSCopying {

		[Export ("goalCount")]
		nint GoalCount { get; }

		[Static]
		[Export ("behaviorWithGoal:weight:")]
		GKBehavior FromGoal (GKGoal goal, float weight);

		[Static]
		[Export ("behaviorWithGoals:")]
		GKBehavior FromGoals (GKGoal [] goals);

		[Static]
		[Export ("behaviorWithGoals:andWeights:")]
		GKBehavior FromGoals (GKGoal [] goals, NSNumber [] weights);

		[Static]
		[Export ("behaviorWithWeightedGoals:")]
		GKBehavior FromGoals (NSDictionary<GKGoal, NSNumber> weightedGoals);

		[Export ("setWeight:forGoal:")]
		void SetWeight (float weight, GKGoal goal);

		[Export ("weightForGoal:")]
		float GetWeight (GKGoal goal);

		[Export ("removeGoal:")]
		void RemoveGoal (GKGoal goal);

		[Export ("removeAllGoals")]
		void RemoveAllGoals ();

		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		GKGoal ObjectAtIndexedSubscript (nuint idx);

		[Internal]
		[Export ("setObject:forKeyedSubscript:")]
		void SetObject (NSNumber weight, GKGoal goal);

		[Internal]
		[return: NullAllowed]
		[Export ("objectForKeyedSubscript:")]
		NSNumber ObjectForKeyedSubscript (GKGoal goal);
	}

	/// <summary>Abstract superclass for components, including <see cref="T:GameplayKit.GKAgent" /> objects, in an Entity-Component architecture (see remarks).</summary>
	///     <remarks>
	///       <para>GameplayKit provides a basic Entity-Component architecture. </para>
	///     </remarks>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKComponent_Class/index.html">Apple documentation for <c>GKComponent</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	interface GKComponent : NSCopying, NSSecureCoding {

		[NullAllowed]
		[Export ("entity", ArgumentSemantic.Weak)]
		GKEntity Entity { get; }

		[Export ("updateWithDeltaTime:")]
		void Update (double deltaTimeInSeconds);

		[MacCatalyst (13, 1)]
		[Export ("didAddToEntity")]
		void DidAddToEntity ();

		[MacCatalyst (13, 1)]
		[Export ("willRemoveFromEntity")]
		void WillRemoveFromEntity ();
	}

	/// <summary>Holds <see cref="T:GameplayKit.GKComponent" /> objects of a specific subtype and updates them periodically.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKComponentSystem_Class/index.html">Apple documentation for <c>GKComponentSystem</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // We have a manual default ctor.
	/* using 'TComponent' for the generic argument, since we have an additional member 'ComponentType' which clashes with Objective-C's generic argument 'ComponentType' */
	interface GKComponentSystem<TComponent>
		where TComponent : GKComponent {

		// note: it's not impossible to get into a situation where no managed type is mapped to a `Class`
		// so we export this `Class` type API (e.g. 3rd party native code)
		[Export ("componentClass")]
		Class ComponentClass { get; }

		[Export ("components", ArgumentSemantic.Retain)]
		TComponent [] Components { get; }

		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		TComponent ObjectAtIndexedSubscript (nuint idx);

		[Export ("initWithComponentClass:")]
		NativeHandle Constructor (Class cls);

		[Export ("addComponent:")]
		void AddComponent (TComponent component);

		[Export ("addComponentWithEntity:")]
		void AddComponent (GKEntity entity);

		[Export ("removeComponentWithEntity:")]
		void RemoveComponent (GKEntity entity);

		[Export ("removeComponent:")]
		void RemoveComponent (TComponent component);

		[Export ("updateWithDeltaTime:")]
		void Update (double deltaTimeInSeconds);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[MacCatalyst (13, 1)]
		[Export ("classForGenericArgumentAtIndex:")]
		Class GetClassForGenericArgument (nuint index);

		[MacCatalyst (13, 1)]
		[Wrap ("Class.Lookup (GetClassForGenericArgument (index))!")]
		Type GetTypeForGenericArgument (nuint index);
	}

	/// <summary>A <see cref="T:GameplayKit.GKBehavior" /> that combines other <see cref="T:GameplayKit.GKBehavior" /> objects.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKBehavior))]
	interface GKCompositeBehavior {

		[Export ("behaviorCount")]
		nint BehaviorCount { get; }

		[Static]
		[Export ("behaviorWithBehaviors:")]
		GKCompositeBehavior FromBehaviors (GKBehavior [] behaviors);

		[Static]
		[Export ("behaviorWithBehaviors:andWeights:")]
		GKCompositeBehavior FromBehaviors (GKBehavior [] behaviors, NSNumber [] weights);

		[Export ("setWeight:forBehavior:")]
		void SetWeight (float weight, GKBehavior behavior);

		[Export ("weightForBehavior:")]
		float GetWeight (GKBehavior behavior);

		[Export ("removeBehavior:")]
		void RemoveBehavior (GKBehavior behavior);

		[Export ("removeAllBehaviors")]
		void RemoveAllBehaviors ();

		[Internal]
		[Export ("objectAtIndexedSubscript:")]
		GKBehavior ObjectAtIndexedSubscript (nuint index);

		[Internal]
		[Export ("setObject:forKeyedSubscript:")]
		void SetObject (NSNumber weight, GKBehavior behavior);

		[Internal]
		[Export ("objectForKeyedSubscript:")]
		NSNumber ObjectForKeyedSubscript (GKBehavior behavior);
	}

	/// <summary>An element in a <see cref="T:GameplayKit.GKDecisionTree" />.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKDecisionNode {

		[Export ("createBranchWithValue:attribute:")]
		GKDecisionNode CreateBranch (NSNumber value, NSObject attribute);

		[Export ("createBranchWithPredicate:attribute:")]
		GKDecisionNode CreateBranch (NSPredicate predicate, NSObject attribute);

		[Export ("createBranchWithWeight:attribute:")]
		GKDecisionNode CreateBranch (nint weight, NSObject attribute);
	}

	/// <summary>A tree of questions, answers, and actions. </summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKDecisionTree : NSSecureCoding {

		[NullAllowed, Export ("rootNode")]
		GKDecisionNode RootNode { get; }

		[Export ("randomSource", ArgumentSemantic.Copy)]
		GKRandomSource RandomSource { get; set; }

		[Export ("initWithAttribute:")]
		NativeHandle Constructor (NSObject attribute);

		[Export ("initWithExamples:actions:attributes:")]
		NativeHandle Constructor (NSArray<NSObject> [] examples, NSObject [] actions, NSObject [] attributes);

		[Export ("findActionForAnswers:")]
		[return: NullAllowed]
		NSObject FindAction (NSDictionary<NSObject, NSObject> answers);

		[MacCatalyst (13, 1)]
		[Export ("initWithURL:error:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] NSError error);

		[MacCatalyst (13, 1)]
		[Export ("exportToURL:error:")]
		bool Export (NSUrl url, [NullAllowed] NSError error);
	}

	/// <summary>A type that is composed of a number of <see cref="T:GameplayKit.GKComponent" /> objects in an Entity-Component architecture.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKEntity_Class/index.html">Apple documentation for <c>GKEntity</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface GKEntity : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Static]
		[Export ("entity")]
		GKEntity GetEntity ();

		[Export ("updateWithDeltaTime:")]
		void Update (double deltaTimeInSeconds);

		[Export ("components", ArgumentSemantic.Retain)]
		GKComponent [] Components { get; }

		[Export ("addComponent:")]
		void AddComponent (GKComponent component);

		[Protected] // should only be used if subclassed
		[Export ("removeComponentForClass:")]
		void RemoveComponent (Class componentClass);

		[Protected] // should only be used if subclassed
		[Export ("componentForClass:")]
		[return: NullAllowed]
		GKComponent GetComponent (Class componentClass);
	}

	interface IGKGameModelUpdate { }

	/// <summary>A valid game move. The minimal data necessary to transition a valid <see cref="T:GameplayKit.IGKGameModel" /> into a valid subsequent state.</summary>
	///     <remarks>
	///       <para>Developers should strive to make their implementations of this interface efficient. A large number of <see cref="T:GameplayKit.IGKGameModelUpdate" /> objects are likely to be produced by <see cref="M:GameplayKit.IGKGameModel.GetGameModelUpdates(GameplayKit.IGKGameModelPlayer)" /> which, in return, is likely to be called many times by <see cref="M:GameplayKit.GKMinMaxStrategist.GetBestMove(GameplayKit.IGKGameModelPlayer)" />.</para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface GKGameModelUpdate {

		[Abstract]
		[Export ("value", ArgumentSemantic.Assign)]
		nint Value { get; set; }
	}

	interface IGKGameModelPlayer { }

	/// <summary>A uniquely-identified player of a game. Developers must implement <see cref="M:GameplayKit.GKGameModelPlayer_Extensions.GetPlayerId(GameplayKit.IGKGameModelPlayer)" />.</summary>
	///     <remarks>
	///       <para>Developers who implement this interface must implement <see cref="M:GameplayKit.GKGameModelPlayer_Extensions.GetPlayerId(GameplayKit.IGKGameModelPlayer)" />. It is incorrectly marked as optional but is, in fact, mandatory and must be implemented.</para>
	///     </remarks>
	/// <summary>Extension methods to the <see cref="T:GameplayKit.IGKGameModelPlayer" /> interface to support all the methods from the <see cref="T:GameplayKit.IGKGameModelPlayer" /> protocol.</summary>
	///     <remarks>
	///       <para>The extension methods for <see cref="T:GameplayKit.IGKGameModelPlayer" /> allow developers to treat instances of the interface as having all the optional methods of the original <see cref="T:GameplayKit.IGKGameModelPlayer" /> protocol.   Since the interface only contains the required members, these extension methods allow developers to call the optional members of the protocol.</para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface GKGameModelPlayer {

#if NET
		[Abstract]
		[Export ("playerId")]
		nint PlayerId { get; }
#else
		// This was a property but changed it to Get semantic due to
		// there are no Extension properties
		[Export ("playerId")]
		nint GetPlayerId ();
#endif
	}

	interface IGKGameModel { }

	/// <summary>The current game state. Particularly useful in conjunction with <see cref="T:GameplayKit.GKMinMaxStrategist" />.</summary>
	///     <remarks>
	///       <para>When <see cref="T:GameplayKit.GKMinMaxStrategist" /> is used as an AI opponent, it uses <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=T:Gameplay.IGKGameModel&amp;scope=Xamarin" title="T:Gameplay.IGKGameModel">T:Gameplay.IGKGameModel</a></format> objects to describe the game's state and <see cref="T:GameplayKit.IGKGameModelUpdate" /> objects to describe potential moves. (See the "AI Opponent" section in the remarks at <see cref="N:GameplayKit" />)</para>
	///     </remarks>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface GKGameModel : NSCopying {

		// This was a property but changed it to Get semantic due to
		// there are no Extension properties
		[Abstract]
		[Export ("players")]
		[return: NullAllowed]
		IGKGameModelPlayer [] GetPlayers ();

		// This was a property but changed it to Get semantic due to
		// there are no Extension properties
		[Abstract]
		[Export ("activePlayer")]
		[return: NullAllowed]
		IGKGameModelPlayer GetActivePlayer ();

		[Abstract]
		[Export ("setGameModel:")]
		void SetGameModel (IGKGameModel gameModel);

		[Abstract]
		[Export ("gameModelUpdatesForPlayer:")]
		[return: NullAllowed]
		IGKGameModelUpdate [] GetGameModelUpdates (IGKGameModelPlayer player);

		[Abstract]
		[Export ("applyGameModelUpdate:")]
		void ApplyGameModelUpdate (IGKGameModelUpdate gameModelUpdate);

		[Export ("scoreForPlayer:")]
		nint GetScore (IGKGameModelPlayer player);

		[Export ("isWinForPlayer:")]
		bool IsWin (IGKGameModelPlayer player);

		[Export ("isLossForPlayer:")]
		bool IsLoss (IGKGameModelPlayer player);

		[MacCatalyst (13, 1)]
		[Export ("unapplyGameModelUpdate:")]
		void UnapplyGameModelUpdate (IGKGameModelUpdate gameModelUpdate);
	}

	/// <summary>Influences the movement of one or more <see cref="T:GameplayKit.GKAgent" /> objects.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKGoal_Class/index.html">Apple documentation for <c>GKGoal</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKGoal : NSCopying {

		[Static]
		[Export ("goalToSeekAgent:")]
		GKGoal GetGoalToSeekAgent (GKAgent agent);

		[Static]
		[Export ("goalToFleeAgent:")]
		GKGoal GetGoalToFleeAgent (GKAgent agent);

		[Static]
		[Export ("goalToAvoidObstacles:maxPredictionTime:")]
		GKGoal GetGoalToAvoidObstacles (GKObstacle [] obstacles, double maxPredictionTime);

		[Static]
		[Export ("goalToAvoidAgents:maxPredictionTime:")]
		GKGoal GetGoalToAvoidAgents (GKAgent [] agents, double maxPredictionTime);

		[Static]
		[Export ("goalToSeparateFromAgents:maxDistance:maxAngle:")]
		GKGoal GetGoalToSeparate (GKAgent [] agents, float maxDistance, float maxAngle);

		[Static]
		[Export ("goalToAlignWithAgents:maxDistance:maxAngle:")]
		GKGoal GetGoalToAlign (GKAgent [] agents, float maxDistance, float maxAngle);

		[Static]
		[Export ("goalToCohereWithAgents:maxDistance:maxAngle:")]
		GKGoal GetGoalToCohere (GKAgent [] agents, float maxDistance, float maxAngle);

		[Static]
		[Export ("goalToReachTargetSpeed:")]
		GKGoal GetGoalToReachTargetSpeed (float targetSpeed);

		[Static]
		[Export ("goalToWander:")]
		GKGoal GetGoalToWander (float speed);

		[Static]
		[Export ("goalToInterceptAgent:maxPredictionTime:")]
		GKGoal GetGoalToInterceptAgent (GKAgent target, double maxPredictionTime);

		[Static]
		[Export ("goalToFollowPath:maxPredictionTime:forward:")]
		GKGoal GetGoalToFollowPath (GKPath path, double maxPredictionTime, bool forward);

		[Static]
		[Export ("goalToStayOnPath:maxPredictionTime:")]
		GKGoal GetGoalToStayOnPath (GKPath path, double maxPredictionTime);
	}

	/// <include file="../docs/api/GameplayKit/GKGraph.xml" path="/Documentation/Docs[@DocId='T:GameplayKit.GKGraph']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKGraph : NSCopying, NSSecureCoding {

		[NullAllowed]
		[Export ("nodes")]
		GKGraphNode [] Nodes { get; }

		[Static]
		[Export ("graphWithNodes:")]
		GKGraph FromNodes (GKGraphNode [] nodes);

		[Export ("initWithNodes:")]
		NativeHandle Constructor (GKGraphNode [] nodes);

		[Export ("connectNodeToLowestCostNode:bidirectional:")]
		void ConnectNodeToLowestCostNode (GKGraphNode node, bool bidirectional);

		[Export ("removeNodes:")]
		void RemoveNodes (GKGraphNode [] nodes);

		[Export ("addNodes:")]
		void AddNodes (GKGraphNode [] nodes);

		[Export ("findPathFromNode:toNode:")]
		GKGraphNode [] FindPath (GKGraphNode startNode, GKGraphNode endNode);
	}

	interface GKObstacleGraph<NodeType> : GKObstacleGraph { }

	/// <summary>A <see cref="T:GameplayKit.GKGraph" /> that generates a space-filling network for representation, allowing smooth, but inefficient, paths.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKObstacleGraph_Class/index.html">Apple documentation for <c>GKObstacleGraph</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKGraph))]
	interface GKObstacleGraph {

		[Export ("obstacles")]
		GKPolygonObstacle [] Obstacles { get; }

		[Export ("bufferRadius")]
		float BufferRadius { get; }

		[Static]
		[Export ("graphWithObstacles:bufferRadius:")]
		GKObstacleGraph FromObstacles (GKPolygonObstacle [] obstacles, float bufferRadius);

		[Export ("initWithObstacles:bufferRadius:")]
		NativeHandle Constructor (GKPolygonObstacle [] obstacles, float bufferRadius);

		[Internal]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("graphWithObstacles:bufferRadius:nodeClass:")]
		IntPtr GraphWithObstacles (GKPolygonObstacle [] obstacles, float bufferRadius, Class nodeClass);

		[Internal]
		[MacCatalyst (13, 1)]
		[Export ("initWithObstacles:bufferRadius:nodeClass:")]
		NativeHandle Constructor (GKPolygonObstacle [] obstacles, float bufferRadius, Class nodeClass);

		[Export ("connectNodeUsingObstacles:")]
		void ConnectNodeUsingObstacles (GKGraphNode2D node);

		[Export ("connectNodeUsingObstacles:ignoringObstacles:")]
		void ConnectNodeUsingObstacles (GKGraphNode2D node, GKPolygonObstacle [] obstaclesToIgnore);

		[Export ("connectNodeUsingObstacles:ignoringBufferRadiusOfObstacles:")]
		void ConnectNodeUsingObstaclesIgnoringBufferRadius (GKGraphNode2D node, GKPolygonObstacle [] obstaclesBufferRadiusToIgnore);

		[Export ("addObstacles:")]
		void AddObstacles (GKPolygonObstacle [] obstacles);

		[Export ("removeObstacles:")]
		void RemoveObstacles (GKPolygonObstacle [] obstacles);

		[Export ("removeAllObstacles")]
		void RemoveAllObstacles ();

		[Internal]
		[Export ("nodesForObstacle:")]
		IntPtr _GetNodes (GKPolygonObstacle obstacle);

		[Export ("lockConnectionFromNode:toNode:")]
		void LockConnection (GKGraphNode2D startNode, GKGraphNode2D endNode);

		[Export ("unlockConnectionFromNode:toNode:")]
		void UnlockConnection (GKGraphNode2D startNode, GKGraphNode2D endNode);

		[Export ("isConnectionLockedFromNode:toNode:")]
		bool IsConnectionLocked (GKGraphNode2D startNode, GKGraphNode2D endNode);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[MacCatalyst (13, 1)]
		[Export ("classForGenericArgumentAtIndex:")]
		Class GetClassForGenericArgument (nuint index);

		[MacCatalyst (13, 1)]
		[Wrap ("Class.Lookup (GetClassForGenericArgument (index))!")]
		Type GetTypeForGenericArgument (nuint index);
	}

	// Apple turned GKGridGraph into a generic type in Xcode8 update
	// 	interface GKGridGraph<NodeType : GKGridGraphNode *>  : GKGraph
	// but we are not doing it since there is not much value to do it right now
	// due to it is only used in the return type of GetNodeAt which in docs says
	// it returns a GKGridGraphNode and we avoid a breaking change. Added a generic GetNodeAt.
	/// <summary>A <see cref="T:GameplayKit.GKGraph" /> in which movement is constrained to an integer grid</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKGridGraph_Class/index.html">Apple documentation for <c>GKGridGraph</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKGraph))]
	interface GKGridGraph {

		[Export ("gridOrigin")]
		Vector2i GridOrigin {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("gridWidth")]
		nuint GridWidth { get; }

		[Export ("gridHeight")]
		nuint GridHeight { get; }

		[Export ("diagonalsAllowed")]
		bool DiagonalsAllowed { get; }

		[Static]
		[Export ("graphFromGridStartingAt:width:height:diagonalsAllowed:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKGridGraph FromGridStartingAt (Vector2i position, int width, int height, bool diagonalsAllowed);

		[Export ("initFromGridStartingAt:width:height:diagonalsAllowed:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Vector2i position, int width, int height, bool diagonalsAllowed);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("graphFromGridStartingAt:width:height:diagonalsAllowed:nodeClass:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKGridGraph FromGridStartingAt (Vector2i position, int width, int height, bool diagonalsAllowed, Class aClass);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("FromGridStartingAt (position, width, height, diagonalsAllowed, new Class (type))")]
		GKGridGraph FromGridStartingAt (Vector2i position, int width, int height, bool diagonalsAllowed, Type type);

		[MacCatalyst (13, 1)]
		[Export ("initFromGridStartingAt:width:height:diagonalsAllowed:nodeClass:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Vector2i position, int width, int height, bool diagonalsAllowed, Class aClass);

		[MacCatalyst (13, 1)]
		[Wrap ("this (position, width, height, diagonalsAllowed, new Class (nodeType))")]
		NativeHandle Constructor (Vector2i position, int width, int height, bool diagonalsAllowed, Type nodeType);

		[Internal]
		[Export ("nodeAtGridPosition:")]
		[return: NullAllowed]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr _GetNodeAt (Vector2i position);

		[Export ("connectNodeToAdjacentNodes:")]
		void ConnectNodeToAdjacentNodes (GKGridGraphNode node);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[MacCatalyst (13, 1)]
		[Export ("classForGenericArgumentAtIndex:")]
		Class GetClassForGenericArgument (nuint index);

		[MacCatalyst (13, 1)]
		[Wrap ("Class.Lookup (GetClassForGenericArgument (index))!")]
		Type GetTypeForGenericArgument (nuint index);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKGraph))]
	interface GKMeshGraph<NodeType> where NodeType : GKGraphNode2D {

		[Export ("obstacles")]
		GKPolygonObstacle [] Obstacles { get; }

		[Export ("bufferRadius")]
		float BufferRadius { get; }

		[Export ("triangulationMode", ArgumentSemantic.Assign)]
		GKMeshGraphTriangulationMode TriangulationMode { get; set; }

		[Export ("triangleCount")]
		nuint TriangleCount { get; }

		[Static]
		[Export ("graphWithBufferRadius:minCoordinate:maxCoordinate:nodeClass:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKMeshGraph<NodeType> FromBufferRadius (float bufferRadius, Vector2 min, Vector2 max, Class nodeClass);

		[Static]
		[Wrap ("FromBufferRadius (bufferRadius, min, max, new Class (nodeType))")]
		GKMeshGraph<NodeType> FromBufferRadius (float bufferRadius, Vector2 min, Vector2 max, Type nodeType);

		[Export ("initWithBufferRadius:minCoordinate:maxCoordinate:nodeClass:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (float bufferRadius, Vector2 min, Vector2 max, Class nodeClass);

		[Wrap ("this (bufferRadius, min, max, new Class (nodeType))")]
		NativeHandle Constructor (float bufferRadius, Vector2 min, Vector2 max, Type nodeType);

		[Static]
		[Export ("graphWithBufferRadius:minCoordinate:maxCoordinate:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKMeshGraph<NodeType> FromBufferRadius (float bufferRadius, Vector2 min, Vector2 max);

		[Export ("initWithBufferRadius:minCoordinate:maxCoordinate:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (float bufferRadius, Vector2 min, Vector2 max);

		[Export ("addObstacles:")]
		void AddObstacles (GKPolygonObstacle [] obstacles);

		[Export ("removeObstacles:")]
		void RemoveObstacles (GKPolygonObstacle [] obstacles);

		[Export ("connectNodeUsingObstacles:")]
		void ConnectNodeUsingObstacles (NodeType node);

		[Export ("triangulate")]
		void Triangulate ();

		[Export ("triangleAtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKTriangle GetTriangle (nuint index);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("classForGenericArgumentAtIndex:")]
		Class GetClassForGenericArgument (nuint index);

		[Wrap ("Class.Lookup (GetClassForGenericArgument (index))!")]
		Type GetTypeForGenericArgument (nuint index);
	}

	/// <summary>The base class for nodes in a <see cref="T:GameplayKit.GKGraph" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKGraphNode_Class/index.html">Apple documentation for <c>GKGraphNode</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKGraphNode : NSSecureCoding {

		[Export ("connectedNodes")]
		GKGraphNode [] ConnectedNodes { get; }

		[Export ("addConnectionsToNodes:bidirectional:")]
		void AddConnections (GKGraphNode [] nodes, bool bidirectional);

		[Export ("removeConnectionsToNodes:bidirectional:")]
		void RemoveConnections (GKGraphNode [] nodes, bool bidirectional);

		[Export ("estimatedCostToNode:")]
		float GetEstimatedCost (GKGraphNode node);

		[Export ("costToNode:")]
		float GetCost (GKGraphNode node);

		[Export ("findPathToNode:")]
		GKGraphNode [] FindPathTo (GKGraphNode goalNode);

		[Export ("findPathFromNode:")]
		GKGraphNode [] FindPathFrom (GKGraphNode startNode);
	}

	/// <summary>A <see cref="T:GameplayKit.GKGraphNode" /> that contains a 2D floating-point position.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKGraphNode2D_Class/index.html">Apple documentation for <c>GKGraphNode2D</c></related>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKGraphNode))]
	interface GKGraphNode2D {

		[Export ("position", ArgumentSemantic.Assign)]
		Vector2 Position {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Static]
		[Export ("nodeWithPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKGraphNode2D FromPoint (Vector2 point);

		[Export ("initWithPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Vector2 point);
	}

	/// <summary>A <see cref="T:GameplayKit.GKGraphNode" /> that exists in three-dimensional space.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKGraphNode))]
	interface GKGraphNode3D {

		[Export ("position", ArgumentSemantic.Assign)]
		Vector3 Position {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Static]
		[Export ("nodeWithPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKGraphNode3D FromPoint (Vector3 point);

		[Export ("initWithPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Vector3 point);
	}

	/// <summary>A <see cref="T:GameplayKit.GKGraphNode" /> that contains a 2D integer position.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKGridGraphNode_Class/index.html">Apple documentation for <c>GKGridGraphNode</c></related>
	[DisableDefaultCtor]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKGraphNode))]
	interface GKGridGraphNode {

		[Export ("gridPosition", ArgumentSemantic.Assign)]
		Vector2i GridPosition {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
#if !NET
			[NotImplemented]
			set;
#endif
		}

		[Static]
		[Export ("nodeWithGridPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKGridGraphNode FromGridPosition (Vector2i gridPosition);

		[Export ("initWithGridPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (Vector2i gridPosition);
	}

	/// <include file="../docs/api/GameplayKit/GKMinMaxStrategist.xml" path="/Documentation/Docs[@DocId='T:GameplayKit.GKMinMaxStrategist']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "GKMinmaxStrategist")]
	interface GKMinMaxStrategist : GKStrategist {
		[Export ("maxLookAheadDepth", ArgumentSemantic.Assign)]
		nint MaxLookAheadDepth { get; set; }

		[Export ("bestMoveForPlayer:")]
		[return: NullAllowed]
		IGKGameModelUpdate GetBestMove (IGKGameModelPlayer player);

		[Export ("randomMoveForPlayer:fromNumberOfBestMoves:")]
		[return: NullAllowed]
		IGKGameModelUpdate GetRandomMove (IGKGameModelPlayer player, nint numMovesToConsider);
	}

	/// <summary>Abstract class representing areas that <see cref="T:GameplayKit.GKAgent" /> objects cannot traverse.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKObstacle_Class/index.html">Apple documentation for <c>GKObstacle</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	interface GKObstacle {
	}

	/// <summary>A <see cref="T:GameplayKit.GKObstacle" /> defined by a location and a radius.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKCircleObstacle_Class/index.html">Apple documentation for <c>GKCircleObstacle</c></related>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKObstacle))]
	interface GKCircleObstacle {

		[Export ("radius")]
		float Radius { get; set; }

		[Export ("position", ArgumentSemantic.Assign)]
		Vector2 Position {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Static]
		[Export ("obstacleWithRadius:")]
		GKCircleObstacle FromRadius (float radius);

		[Export ("initWithRadius:")]
		[DesignatedInitializer]
		NativeHandle Constructor (float radius);
	}

	/// <summary>A <see cref="T:GameplayKit.GKObstacle" /> with an arbitrarily complex shape.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKPolygonObstacle_Class/index.html">Apple documentation for <c>GKPolygonObstacle</c></related>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKObstacle))]
	interface GKPolygonObstacle : NSSecureCoding {

		[Export ("vertexCount")]
		nuint VertexCount { get; }

		[Static, Internal]
		[Export ("obstacleWithPoints:count:")]
		GKPolygonObstacle FromPoints (IntPtr points, nuint numPoints);

		[Internal]
		[Export ("initWithPoints:count:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IntPtr points, nuint numPoints);

		[Export ("vertexAtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetVertex (nuint index);
	}

	/// <summary>A <see cref="T:GameplayKit.GKObstacle" /> that is an impassable spherical volume.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKObstacle))]
	interface GKSphereObstacle {

		[Export ("radius")]
		float Radius { get; set; }

		[Export ("position", ArgumentSemantic.Assign)]
		Vector3 Position {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			set;
		}

		[Static]
		[Export ("obstacleWithRadius:")]
		GKSphereObstacle FromRadius (float radius);

		[Export ("initWithRadius:")]
		[DesignatedInitializer]
		NativeHandle Constructor (float radius);
	}

	/// <summary>Holds a 2D polygonal path that can be followed by a <see cref="T:GameplayKit.GKAgent" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKPath_Class/index.html">Apple documentation for <c>GKPath</c></related>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKPath {

		[Export ("radius")]
		float Radius { get; set; }

		[Export ("numPoints")]
		nuint NumPoints { get; }

		[Export ("cyclical")]
		bool Cyclical { [Bind ("isCyclical")] get; set; }

		[Static, Internal]
		[Export ("pathWithPoints:count:radius:cyclical:")]
		GKPath FromPoints (IntPtr points, nuint count, float radius, bool cyclical);

		[Internal]
		[Export ("initWithPoints:count:radius:cyclical:")]
		IntPtr InitWithPoints (IntPtr points, nuint count, float radius, bool cyclical);

		[MacCatalyst (13, 1)]
		[Static, Internal]
		[Export ("pathWithFloat3Points:count:radius:cyclical:")]
		GKPath FromFloat3Points (IntPtr points, nuint count, float radius, bool cyclical);

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("initWithFloat3Points:count:radius:cyclical:")]
		IntPtr InitWithFloat3Points (IntPtr points, nuint count, float radius, bool cyclical);

		[Static]
		[Export ("pathWithGraphNodes:radius:")]
		GKPath FromGraphNodes (GKGraphNode [] nodes, float radius);

		[Static] // Avoid breaking change
		[Wrap ("FromGraphNodes (nodes: graphNodes, radius: radius)")]
		GKPath FromGraphNodes (GKGraphNode2D [] graphNodes, float radius);

		[Export ("initWithGraphNodes:radius:")]
		NativeHandle Constructor (GKGraphNode [] nodes, float radius);

		[Wrap ("this (nodes: graphNodes, radius: radius)")] // Avoid breaking change
		NativeHandle Constructor (GKGraphNode2D [] graphNodes, float radius);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'GetVector2Point' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'GetVector2Point' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'GetVector2Point' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetVector2Point' instead.")]
		[Export ("pointAtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetPoint (nuint index);

		[MacCatalyst (13, 1)]
		[Export ("float2AtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetVector2Point (nuint index);

		[MacCatalyst (13, 1)]
		[Export ("float3AtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector3 GetVector3Point (nuint index);
	}

	/// <summary>Defines a probability distribution. This class defines a uniform distribution (all values equally likely), while subclasses <see cref="T:GameplayKit.GKGaussianDistribution" /> and <see cref="T:GameplayKit.GKShuffledDistribution" /> provide different likelihoods.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKRandomDistribution_Class/index.html">Apple documentation for <c>GKRandomDistribution</c></related>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKRandomDistribution : GKRandom {

		[Export ("lowestValue", ArgumentSemantic.Assign)]
		nint LowestValue { get; }

		[Export ("highestValue", ArgumentSemantic.Assign)]
		nint HighestValue { get; }

		[Export ("numberOfPossibleOutcomes", ArgumentSemantic.Assign)]
		nuint NumberOfPossibleOutcomes { get; }

		[Export ("initWithRandomSource:lowestValue:highestValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IGKRandom source, nint lowestInclusive, nint highestInclusive);

		//		The following guys are already present in the GKRandom Protocol
		//		[Export ("nextInt")]
		//		nint GetNextInt ();
		//
		//		[Export ("nextIntWithUpperBound:")]
		//		nuint GetNextInt (nuint upperBound);
		//
		//		[Export ("nextUniform")]
		//		float GetNextUniform ();
		//
		//		[Export ("nextBool")]
		//		bool GetNextBool ();

		[Static]
		[Export ("distributionWithLowestValue:highestValue:")]
		GKRandomDistribution GetDistributionBetween (nint lowestInclusive, nint highestInclusive);

		[Static]
		[Export ("distributionForDieWithSideCount:")]
		GKRandomDistribution GetDie (nint sideCount);

		[Static]
		[Export ("d6")]
		GKRandomDistribution GetD6 ();

		[Static]
		[Export ("d20")]
		GKRandomDistribution GetD20 ();
	}

	/// <include file="../docs/api/GameplayKit/GKGaussianDistribution.xml" path="/Documentation/Docs[@DocId='T:GameplayKit.GKGaussianDistribution']/*" />
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKRandomDistribution))]
	interface GKGaussianDistribution {

		[Export ("mean")]
		float Mean { get; }

		[Export ("deviation")]
		float Deviation { get; }

		[Export ("initWithRandomSource:lowestValue:highestValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IGKRandom source, nint lowestInclusive, nint highestInclusive);

		[Export ("initWithRandomSource:mean:deviation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IGKRandom source, float mean, float deviation);
	}

	/// <summary>A <see cref="T:GameplayKit.GKRandomDistribution" /> that shuffles a collection in a manner that makes sequences of similar values unlikely (minimal hot/cold streaks).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKShuffledDistribution_Class/index.html">Apple documentation for <c>GKShuffledDistribution</c></related>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKRandomDistribution))]
	interface GKShuffledDistribution {

		// inlined from base type
		[Export ("initWithRandomSource:lowestValue:highestValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IGKRandom source, nint lowestInclusive, nint highestInclusive);
	}

	interface IGKRandom { }

	/// <summary>Interface for GameplayKit pseudo-random number generators.</summary>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface GKRandom {

		[Abstract]
		[Export ("nextInt")]
		nint GetNextInt ();

		[Abstract]
		[Export ("nextIntWithUpperBound:")]
		nuint GetNextInt (nuint upperBound);

		[Abstract]
		[Export ("nextUniform")]
		float GetNextUniform ();

		[Abstract]
		[Export ("nextBool")]
		bool GetNextBool ();
	}

	/// <include file="../docs/api/GameplayKit/GKRandomSource.xml" path="/Documentation/Docs[@DocId='T:GameplayKit.GKRandomSource']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface GKRandomSource : GKRandom, NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Static]
		[Export ("sharedRandom")]
		GKRandomSource SharedRandom { get; }

		[Export ("arrayByShufflingObjectsInArray:")]
		NSObject [] ShuffleObjects (NSObject [] array);
	}

	/// <summary>Random generator based on the ARC4 algorithm. Often a good choice.</summary>
	///     <remarks>
	///       <para>Unlike the <see cref="P:GameplayKit.GKRandomSource.SharedRandom" /> object, <see cref="T:GameplayKit.GKARC4RandomSource" /> objects do not share state with system-wide <c>arc4random</c> C functions. <see cref="T:GameplayKit.GKARC4RandomSource" /> objects are both deterministic and independent.</para>
	///       <para>
	///         <see cref="T:GameplayKit.GKARC4RandomSource" /> objects are generally good random sources, but may be predicted by analyzing the first 768 values generated. To avoid such possibilities, call <see cref="M:GameplayKit.GKARC4RandomSource.DropValues(System.nuint)" /> with a value of 768 or greater.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKARC4RandomSource_Class/index.html">Apple documentation for <c>GKARC4RandomSource</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKRandomSource))]
	interface GKARC4RandomSource {

		[Export ("seed", ArgumentSemantic.Copy)]
		NSData Seed { get; set; }

		[Export ("initWithSeed:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData seed);

		[Export ("dropValuesWithCount:")]
		void DropValues (nuint count);
	}

	/// <summary>A fast <see cref="T:GameplayKit.GKRandomSource" />. Low-order bits are somewhat less random than in <see cref="T:GameplayKit.GKARC4RandomSource" />.</summary>
	///     <remarks>
	///       <para>
	///         <see cref="T:GameplayKit.GKLinearCongruentialRandomSource" /> generators are fast and generally sufficient. They are not as properly random as <see cref="T:GameplayKit.GKARC4RandomSource" /> generators, much less<see cref="T:GameplayKit.GKMersenneTwisterRandomSource" /> generators.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKLinearCongruentialRandomSource_Class/index.html">Apple documentation for <c>GKLinearCongruentialRandomSource</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKRandomSource))]
	interface GKLinearCongruentialRandomSource {

		[Export ("seed")]
		ulong Seed { get; set; }

		[Export ("initWithSeed:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ulong seed);
	}

	/// <summary>A slow <see cref="T:GameplayKit.GKRandomSource" /> with very good randomness.</summary>
	///     <remarks>
	///       <para>The <see cref="T:GameplayKit.GKMersenneTwisterRandomSource" /> produces the highest-quality random sequences of the generators in GameplayKit, but is slower than the others.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKMersenneTwisterRandomSource_Class/index.html">Apple documentation for <c>GKMersenneTwisterRandomSource</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKRandomSource))]
	interface GKMersenneTwisterRandomSource {

		[Export ("seed")]
		ulong Seed { get; set; }

		[Export ("initWithSeed:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ulong seed);
	}

	/// <include file="../docs/api/GameplayKit/GKRuleSystem.xml" path="/Documentation/Docs[@DocId='T:GameplayKit.GKRuleSystem']/*" />
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface GKRuleSystem {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("evaluate")]
		void Evaluate ();

		[Export ("state", ArgumentSemantic.Retain)]
		NSMutableDictionary State { get; }

		[Export ("rules", ArgumentSemantic.Retain)]
		GKRule [] Rules { get; }

		[Export ("addRule:")]
		void AddRule (GKRule rule);

		[Export ("addRulesFromArray:")]
		void AddRules (GKRule [] rules);

		[Export ("removeAllRules")]
		void RemoveAllRules ();

		[Export ("agenda", ArgumentSemantic.Retain)]
		GKRule [] Agenda { get; }

		[Export ("executed", ArgumentSemantic.Retain)]
		GKRule [] Executed { get; }

		[Export ("facts", ArgumentSemantic.Retain)]
		NSObject [] Facts { get; }

		[Export ("gradeForFact:")]
		float GetGrade (NSObject fact);

		[Export ("minimumGradeForFacts:")]
		float GetMinimumGrade (NSObject [] facts);

		[Export ("maximumGradeForFacts:")]
		float GetMaximumGrade (NSObject [] facts);

		[Export ("assertFact:")]
		void AssertFact (NSObject fact);

		[Export ("assertFact:grade:")]
		void AssertFact (NSObject fact, float grade);

		[Export ("retractFact:")]
		void RetractFact (NSObject fact);

		[Export ("retractFact:grade:")]
		void RetractFact (NSObject fact, float grade);

		[Export ("reset")]
		void Reset ();
	}

	/// <summary>A single element, comprising a predicate and an action, that represents a discrete rule in a <see cref="T:GameplayKit.GKRuleSystem" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKRule_Class/index.html">Apple documentation for <c>GKRule</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKRule {

		[Export ("salience", ArgumentSemantic.Assign)]
		nint Salience { get; set; }

		[Export ("evaluatePredicateWithSystem:")]
		bool EvaluatePredicate (GKRuleSystem system);

		[Export ("performActionWithSystem:")]
		void PerformAction (GKRuleSystem system);

		[Static]
		[Export ("ruleWithPredicate:assertingFact:grade:")]
		GKRule FromPredicateAssertingFact (NSPredicate predicate, NSObject fact, float grade);

		[Static]
		[Export ("ruleWithPredicate:retractingFact:grade:")]
		GKRule FromPredicateRetractingFact (NSPredicate predicate, NSObject fact, float grade);

		[Static]
		[Export ("ruleWithBlockPredicate:action:")]
		GKRule FromPredicate (Func<GKRuleSystem, bool> predicate, Action<GKRuleSystem> action);
	}

	/// <summary>A <see cref="T:GameplayKit.GKRule" /> that uses a <see cref="T:Foundation.NSPredicate" /> to determine if it's action should be called.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKNSPredicateRule_Class/index.html">Apple documentation for <c>GKNSPredicateRule</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKRule))]
	interface GKNSPredicateRule {

		[Export ("predicate", ArgumentSemantic.Retain)]
		NSPredicate Predicate { get; }

		[Export ("initWithPredicate:")]
		NativeHandle Constructor (NSPredicate predicate);

		[Export ("evaluatePredicateWithSystem:"), New]
		bool EvaluatePredicate (GKRuleSystem system);
	}

	/// <summary>An abstract class representing a discrete state in a <see cref="T:GameplayKit.GKStateMachine" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKState_Class/index.html">Apple documentation for <c>GKState</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[DisableDefaultCtor] // designated
	interface GKState {

		[Protected]
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[NullAllowed]
		[Export ("stateMachine", ArgumentSemantic.Weak)]
		GKStateMachine StateMachine { get; }

		[Static]
		[Export ("state")]
		GKState GetState ();

		// note: need to be exposed (and overridden by user code) as it's called by the GameplayKit code
		// we can (and do) expose nicer alternatives for user code but, since they are not the one being called, they can't be virtual
		[Export ("isValidNextState:")]
		bool IsValidNextState (Class stateClass);

		[Export ("didEnterWithPreviousState:")]
		void DidEnter ([NullAllowed] GKState previousState);

		[Export ("updateWithDeltaTime:")]
		void Update (double deltaTimeInSeconds);

		[Export ("willExitWithNextState:")]
		void WillExit (GKState nextState);
	}

	/// <summary>Holds <see cref="T:GameplayKit.GKState" /> objects and manages transitions between them.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GameplayKit/Reference/GKStateMachine_Class/index.html">Apple documentation for <c>GKStateMachine</c></related>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKStateMachine {

		[NullAllowed]
		[Export ("currentState")]
		GKState CurrentState { get; }

		[Static]
		[Export ("stateMachineWithStates:")]
		GKStateMachine FromStates (GKState [] states);

		[Export ("initWithStates:")]
		[DesignatedInitializer]
		NativeHandle Constructor (GKState [] states);

		[Export ("updateWithDeltaTime:")]
		void Update (double deltaTimeInSeconds);

		[Protected] // should only be used if subclassed
		[Export ("stateForClass:")]
		[return: NullAllowed]
		GKState GetState (Class stateClass);

		[Protected] // should only be used if subclassed
		[Export ("canEnterState:")]
		bool CanEnterState (Class stateClass);

		[Protected] // should only be used if subclassed
		[Export ("enterState:")]
		bool EnterState (Class stateClass);
	}

	/// <summary>Interface for a game strategist (AI).</summary>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface GKStrategist {
		[Abstract]
		[NullAllowed, Export ("gameModel", ArgumentSemantic.Retain)]
		IGKGameModel GameModel { get; set; }

		[Abstract]
		[NullAllowed, Export ("randomSource", ArgumentSemantic.Retain)]
		IGKRandom RandomSource { get; set; }

		[Abstract]
		[Export ("bestMoveForActivePlayer")]
		IGKGameModelUpdate GetBestMoveForActivePlayer ();
	}

	/// <summary>A strategist that reaches a solution that is probably close to optimal in a deterministic amount of time.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/GameplayKit/GKMonteCarloStrategist">Apple documentation for <c>GKMonteCarloStrategist</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKMonteCarloStrategist : GKStrategist {
		[Export ("budget")]
		nuint Budget { get; set; }

		[Export ("explorationParameter")]
		nuint ExplorationParameter { get; set; }
	}

	/// <summary>Uses a <see cref="T:GameplayKit.GKNoiseSource" /> to procedurally generate an infinite three-dimensional noise field.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKNoise {

		[Export ("gradientColors", ArgumentSemantic.Copy)]
		NSDictionary<NSNumber, SKColor> GradientColors { get; set; }

		[Static]
		[Export ("noiseWithNoiseSource:")]
		GKNoise FromNoiseSource (GKNoiseSource noiseSource);

		[Static]
		[Export ("noiseWithNoiseSource:gradientColors:")]
		GKNoise FromNoiseSource (GKNoiseSource noiseSource, NSDictionary<NSNumber, SKColor> gradientColors);

		[Export ("initWithNoiseSource:")]
		NativeHandle Constructor (GKNoiseSource noiseSource);

		[Export ("initWithNoiseSource:gradientColors:")]
		[DesignatedInitializer]
		NativeHandle Constructor (GKNoiseSource noiseSource, NSDictionary<NSNumber, SKColor> gradientColors);

		[Static]
		[Export ("noiseWithComponentNoises:selectionNoise:")]
		GKNoise FromComponentNoises (GKNoise [] noises, GKNoise selectionNoise);

		[Static]
		[Export ("noiseWithComponentNoises:selectionNoise:componentBoundaries:boundaryBlendDistances:")]
		GKNoise FromComponentNoises (GKNoise [] noises, GKNoise selectionNoise, NSNumber [] componentBoundaries, NSNumber [] blendDistances);

		[Export ("valueAtPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		float GetValue (Vector2 position);

		[Export ("applyAbsoluteValue")]
		void ApplyAbsoluteValue ();

		[Export ("clampWithLowerBound:upperBound:")]
		void Clamp (double lowerBound, double upperBound);

		[Export ("raiseToPower:")]
		void RaiseToPower (double power);

		[Export ("invert")]
		void Invert ();

		[Export ("applyTurbulenceWithFrequency:power:roughness:seed:")]
		void ApplyTurbulence (double frequency, double power, int roughness, int seed);

		[Export ("remapValuesToCurveWithControlPoints:")]
		void RemapValuesToCurve (NSDictionary<NSNumber, NSNumber> controlPoints);

		[Export ("remapValuesToTerracesWithPeaks:terracesInverted:")]
		void RemapValuesToTerraces (NSNumber [] peakInputValues, bool inverted);

		[Export ("moveBy:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void Move (Vector3d delta);

		[Export ("scaleBy:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void Scale (Vector3d factor);

		[Export ("rotateBy:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void Rotate (Vector3d radians);

		[Export ("addWithNoise:")]
		void Add (GKNoise noise);

		[Export ("multiplyWithNoise:")]
		void Multiply (GKNoise noise);

		[Export ("minimumWithNoise:")]
		void GetMinimum (GKNoise noise);

		[Export ("maximumWithNoise:")]
		void GetMaximum (GKNoise noise);

		[Export ("raiseToPowerWithNoise:")]
		void RaiseToPower (GKNoise noise);

		[Export ("displaceXWithNoise:yWithNoise:zWithNoise:")]
		void Displace (GKNoise xDisplacementNoise, GKNoise yDisplacementNoise, GKNoise zDisplacementNoise);

#if !NET
		[Obsolete ("Use 'GKNoise.Displace' instead.")]
		[Wrap ("Displace (xDisplacementNoise, yDisplacementNoise, zDisplacementNoise)", isVirtual: true)]
		void DisplaceX (GKNoise xDisplacementNoise, GKNoise yDisplacementNoise, GKNoise zDisplacementNoise);
#endif
	}

	/// <summary>Slices a finite, two-dimensional rectangle from a <see cref="T:GameplayKit.GKNoise" /> object's infinite, three-dimensional noise field.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKNoiseMap {

		[Export ("size")]
		Vector2d Size {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("origin")]
		Vector2d Origin {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("sampleCount")]
		Vector2i SampleCount {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}

		[Export ("seamless")]
		bool Seamless { [Bind ("isSeamless")] get; }

		[Static]
		[Export ("noiseMapWithNoise:")]
		GKNoiseMap FromNoise (GKNoise noise);

		[Static]
		[Export ("noiseMapWithNoise:size:origin:sampleCount:seamless:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKNoiseMap FromNoise (GKNoise noise, Vector2d size, Vector2d origin, Vector2i sampleCount, bool seamless);

		[Export ("initWithNoise:")]
		NativeHandle Constructor (GKNoise noise);

		[Export ("initWithNoise:size:origin:sampleCount:seamless:")]
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (GKNoise noise, Vector2d size, Vector2d origin, Vector2i sampleCount, bool seamless);

		[Export ("valueAtPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		float GetValue (Vector2i position);

		[Export ("interpolatedValueAtPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		float GetInterpolatedValue (Vector2 position);

		[Export ("setValue:atPosition:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void SetValue (float value, Vector2i position);
	}

	/// <summary>Abstract base class for procedural noise generators.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface GKNoiseSource {

	}

	/// <summary>A <see cref="T:GameplayKit.GKNoiseSource" /> whose output varies smoothly and continuously.</summary>
	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (GKNoiseSource))]
	interface GKCoherentNoiseSource {

		[Export ("frequency")]
		double Frequency { get; set; }

		[Export ("octaveCount")]
		nint OctaveCount { get; set; }

		[Export ("lacunarity")]
		double Lacunarity { get; set; }

		[Export ("seed")]
		int Seed { get; set; }
	}

	/// <summary>A <see cref="T:GameplayKit.GKCoherentNoiseSource" /> that generates improved Perlin noise.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKCoherentNoiseSource))]
	interface GKPerlinNoiseSource {

		[Export ("persistence")]
		double Persistence { get; set; }

		[Static]
		[Export ("perlinNoiseSourceWithFrequency:octaveCount:persistence:lacunarity:seed:")]
		GKPerlinNoiseSource Create (double frequency, nint octaveCount, double persistence, double lacunarity, int seed);

		[Export ("initWithFrequency:octaveCount:persistence:lacunarity:seed:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double frequency, nint octaveCount, double persistence, double lacunarity, int seed);
	}

	/// <summary>A <see cref="T:GameplayKit.GKCoherentNoiseSource" /> whose output is similar to Perlin noise but with more rounded features.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKCoherentNoiseSource))]
	interface GKBillowNoiseSource {

		[Export ("persistence")]
		double Persistence { get; set; }

		[Static]
		[Export ("billowNoiseSourceWithFrequency:octaveCount:persistence:lacunarity:seed:")]
		GKBillowNoiseSource Create (double frequency, nint octaveCount, double persistence, double lacunarity, int seed);

		[Export ("initWithFrequency:octaveCount:persistence:lacunarity:seed:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double frequency, nint octaveCount, double persistence, double lacunarity, int seed);
	}

	/// <summary>A <see cref="T:GameplayKit.GKCoherentNoiseSource" /> whose output is similar to Perlin noise but with sharp boundaries.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKCoherentNoiseSource))]
	interface GKRidgedNoiseSource {

		[Static]
		[Export ("ridgedNoiseSourceWithFrequency:octaveCount:lacunarity:seed:")]
		GKRidgedNoiseSource Create (double frequency, nint octaveCount, double lacunarity, int seed);

		[Export ("initWithFrequency:octaveCount:lacunarity:seed:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double frequency, nint octaveCount, double lacunarity, int seed);
	}

	/// <summary>A <see cref="T:GameplayKit.GKNoiseSource" /> whose output divides space into cells surrounding seed points. Appropriate for crystalline textures.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKNoiseSource))]
	interface GKVoronoiNoiseSource {

		[Export ("frequency")]
		double Frequency { get; set; }

		[Export ("displacement")]
		double Displacement { get; set; }

		[Export ("distanceEnabled")]
		bool DistanceEnabled { [Bind ("isDistanceEnabled")] get; set; }

		[Export ("seed")]
		int Seed { get; set; }

		[Static]
		[Export ("voronoiNoiseWithFrequency:displacement:distanceEnabled:seed:")]
		GKVoronoiNoiseSource Create (double frequency, double displacement, bool distanceEnabled, int seed);

		[Export ("initWithFrequency:displacement:distanceEnabled:seed:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double frequency, double displacement, bool distanceEnabled, int seed);
	}

	/// <summary>A <see cref="T:GameplayKit.GKNoiseSource" /> whose output is a single value.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKNoiseSource))]
	interface GKConstantNoiseSource {

		[Export ("value")]
		double Value { get; set; }

		[Static]
		[Export ("constantNoiseWithValue:")]
		GKConstantNoiseSource Create (double value);

		[Export ("initWithValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double value);
	}

	/// <summary>A <see cref="T:GameplayKit.GKNoiseSource" /> whose output consists of concentric cylindrical shells. Appropriate for wood-grain textures.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKNoiseSource))]
	interface GKCylindersNoiseSource {

		[Export ("frequency")]
		double Frequency { get; set; }

		[Static]
		[Export ("cylindersNoiseWithFrequency:")]
		GKCylindersNoiseSource Create (double frequency);

		[Export ("initWithFrequency:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double frequency);
	}

	/// <summary>A <see cref="T:GameplayKit.GKNoiseSource" /> whose output consists of concentric shells. Appropriate for wood-grain textures.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKNoiseSource))]
	interface GKSpheresNoiseSource {

		[Export ("frequency")]
		double Frequency { get; set; }

		[Static]
		[Export ("spheresNoiseWithFrequency:")]
		GKSpheresNoiseSource Create (double frequency);

		[Export ("initWithFrequency:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double frequency);
	}

	/// <summary>A <see cref="T:GameplayKit.GKNoiseSource" /> whose output consists of alternating black and white squares.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKNoiseSource))]
	interface GKCheckerboardNoiseSource {

		[Export ("squareSize")]
		double SquareSize { get; set; }

		[Static]
		[Export ("checkerboardNoiseWithSquareSize:")]
		GKCheckerboardNoiseSource Create (double squareSize);

		[Export ("initWithSquareSize:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double squareSize);
	}

	/// <summary>A node in a <see cref="T:GameplayKit.GKOctree`1" />. Automatically managed by the <see cref="T:GameplayKit.GKOctree`1" /> as objects are added and removed.</summary>
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKOctreeNode {

		[Export ("box")]
		GKBox Box {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}
	}

	/// <summary>A data structure that efficiently organizes three-dimensional elements.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKOctree<ElementType> where ElementType : NSObject {

		[Static]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[Export ("octreeWithBoundingBox:minimumCellSize:")]
		GKOctree<ElementType> FromBoundingBox (GKBox box, float minCellSize);

		[Export ("initWithBoundingBox:minimumCellSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NativeHandle Constructor (GKBox box, float minCellSize);

		[Export ("addElement:withPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKOctreeNode AddElement (ElementType element, Vector3 point);

		[Export ("addElement:withBox:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKOctreeNode AddElement (ElementType element, GKBox box);

		[Export ("elementsAtPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		ElementType [] GetElements (Vector3 point);

		[Export ("elementsInBox:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		ElementType [] GetElements (GKBox box);

		[Export ("removeElement:")]
		bool RemoveElement (ElementType element);

		[Export ("removeElement:withNode:")]
		bool RemoveElement (ElementType element, GKOctreeNode node);
	}

	/// <summary>A data structure for efficiently searching objects arranged in two-dimensional space.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GKRTree<ElementType> where ElementType : NSObject {

		[Export ("queryReserve")]
		nuint QueryReserve { get; set; }

		[Static]
		[Export ("treeWithMaxNumberOfChildren:")]
		GKRTree<ElementType> FromMaxNumberOfChildren (nuint maxNumberOfChildren);

		[Export ("initWithMaxNumberOfChildren:")]
		[DesignatedInitializer]
		NativeHandle Constructor (nuint maxNumberOfChildren);

		[Export ("addElement:boundingRectMin:boundingRectMax:splitStrategy:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void AddElement (ElementType element, Vector2 boundingRectMin, Vector2 boundingRectMax, GKRTreeSplitStrategy splitStrategy);

		[Export ("removeElement:boundingRectMin:boundingRectMax:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void RemoveElement (ElementType element, Vector2 boundingRectMin, Vector2 boundingRectMax);

		[Export ("elementsInBoundingRectMin:rectMax:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[MarshalNativeExceptions]
		ElementType [] GetElements (Vector2 rectMin, Vector2 rectMax);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKComponent))]
	interface GKSKNodeComponent : GKAgentDelegate {

		[Static]
		[Export ("componentWithNode:")]
		GKSKNodeComponent FromNode (SKNode node);

		[Export ("initWithNode:")]
		NativeHandle Constructor (SKNode node);

		[Export ("node", ArgumentSemantic.Strong)]
		SKNode Node { get; set; }
	}

	interface IGKSceneRootNodeType { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface GKSceneRootNodeType {
	}

	/// <summary>Associates GameplayKit objects with a SpriteKit <see cref="T:SpriteKit.SKScene" />.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface GKScene : NSCopying, NSSecureCoding {

		[Static]
		[Export ("sceneWithFileNamed:")]
		[return: NullAllowed]
		GKScene FromFile (string filename);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("sceneWithFileNamed:rootNode:")]
		[return: NullAllowed]
		GKScene FromFile (string filename, IGKSceneRootNodeType rootNode);

		[Export ("entities")]
		GKEntity [] Entities { get; }

		[NullAllowed, Export ("rootNode", ArgumentSemantic.Assign)]
		IGKSceneRootNodeType RootNode { get; set; }

		[Export ("graphs")]
		NSDictionary<NSString, GKGraph> Graphs { get; }

		[Export ("addEntity:")]
		void AddEntity (GKEntity entity);

		[Export ("removeEntity:")]
		void RemoveEntity (GKEntity entity);

		[Export ("addGraph:name:")]
		void AddGraph (GKGraph graph, string name);

		[Export ("removeGraph:")]
		void RemoveGraph (string name);
	}

	/// <summary>A <see cref="T:GameplayKit.GKComponent" /> that operates on a <see cref="T:SceneKit.SCNNode" />.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (GKComponent))]
	interface GKSCNNodeComponent : GKAgentDelegate {
		[Static]
		[Export ("componentWithNode:")]
		GKSCNNodeComponent FromNode (SCNNode node);

		[Export ("initWithNode:")]
		NativeHandle Constructor (SCNNode node);

		[Export ("node")]
		SCNNode Node { get; }
	}

	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (SKNode))]
	interface SKNode_GameplayKit {

		// Inlined them in SKNode to avoid ugly syntax
		//[Static]
		//[Export ("obstaclesFromSpriteTextures:accuracy:")]
		//GKPolygonObstacle [] ObstaclesFromSpriteTextures (SKNode [] sprites, float accuracy);

		//[Static]
		//[Export ("obstaclesFromNodeBounds:")]
		//GKPolygonObstacle [] ObstaclesFromNodeBounds (SKNode [] nodes);

		//[Static]
		//[Export ("obstaclesFromNodePhysicsBodies:")]
		//GKPolygonObstacle [] ObstaclesFromNodePhysicsBodies (SKNode [] nodes);

		[return: NullAllowed]
		[Export ("entity")]
		GKEntity GetEntity ();

		[Export ("setEntity:")]
		void SetEntity ([NullAllowed] GKEntity entity);
	}

	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (SCNNode))]
	interface SCNNode_GameplayKit {
		[return: NullAllowed]
		[Export ("entity")]
		GKEntity GetEntity ();

		[Export ("setEntity:")]
		void SetEntity ([NullAllowed] GKEntity entity);
	}

	/// <summary>A node in a quadtree.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/GameplayKit/GKQuadTreeNode">Apple documentation for <c>GKQuadTreeNode</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "GKQuadtreeNode")] // Renamed to GKQuadtreeNode (lower case t) in Xcode8
	[DisableDefaultCtor] // <quote>Used as a hint for faster removal via [GKQuadTree removeData:WithNode:]</quote>
	interface GKQuadTreeNode {

		[Export ("quad")]
		GKQuad Quad {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}
	}

	/// <summary>A data structure that efficiently organizes objects in two-dimensional space.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/GameplayKit/GKQuadTree">Apple documentation for <c>GKQuadTree</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "GKQuadtree")] // Renamed to GKQuadtree (lower case t) in xcode8
	[DisableDefaultCtor] // crash (endless recursion)
	interface GKQuadTree {

		[Static]
		[Export ("quadtreeWithBoundingQuad:minimumCellSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKQuadTree FromBoundingQuad (GKQuad quad, float minCellSize);

		[Export ("initWithBoundingQuad:minimumCellSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[DesignatedInitializer]
		NativeHandle Constructor (GKQuad quad, float minCellSize);

		[Export ("addElement:withPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[MarshalNativeExceptions]
		GKQuadTreeNode AddElement (NSObject element, Vector2 point);

		[Export ("addElement:withQuad:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKQuadTreeNode AddElement (NSObject element, GKQuad quad);

		[Export ("elementsAtPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NSObject [] GetElements (Vector2 point);

		[Export ("elementsInQuad:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NSObject [] GetElements (GKQuad quad);

		[Export ("removeElement:")]
		bool RemoveElement (NSObject element);

		[Export ("removeElement:withNode:")]
		bool RemoveElement (NSObject data, GKQuadTreeNode node);

#if !NET && !MONOMAC // This API is removed in Xcode 8

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[NoMacCatalyst]
		[Export ("initWithMinPosition:maxPosition:minCellSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[MarshalNativeExceptions]
		NativeHandle Constructor (Vector2 min, Vector2 max, float minCellSize);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[NoMacCatalyst]
		[Export ("addDataWithPoint:point:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[MarshalNativeExceptions] // added
		GKQuadTreeNode AddData (NSObject data, Vector2 point);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[NoMacCatalyst]
		[Export ("addDataWithQuad:quadOrigin:quadSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[MarshalNativeExceptions]
		GKQuadTreeNode AddData (NSObject data, Vector2 quadOrigin, Vector2 quadSize);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[NoMacCatalyst]
		[Export ("queryDataForPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NSObject [] QueryData (Vector2 point);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[NoMacCatalyst]
		[Export ("queryDataForQuad:quadSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[MarshalNativeExceptions]
		NSObject [] QueryData (Vector2 quadOrigin, Vector2 quadSize);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[NoMacCatalyst]
		[Export ("removeData:withNode:")]
		bool RemoveData (NSObject data, GKQuadTreeNode node);
#endif
	}
}
