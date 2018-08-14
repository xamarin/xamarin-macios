//
// GameplayKit bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using System.ComponentModel;
using ObjCRuntime;
using Foundation;
using SpriteKit;
using SceneKit;
using Vector2 = global::OpenTK.Vector2;
using Vector2d = global::OpenTK.Vector2d;
using Vector2i = global::OpenTK.Vector2i;
using Vector3 = global::OpenTK.Vector3;
using Vector3d = global::OpenTK.Vector3d;
using Matrix3 = global::OpenTK.Matrix3;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;

#if MONOMAC
using SKColor = AppKit.NSColor;
#else
using SKColor = UIKit.UIColor;
#endif

namespace GameplayKit {

	[Native]
	[Flags]
	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	public enum GKMeshGraphTriangulationMode : ulong {
		Vertices = 1 << 0,
		Centers = 1 << 1,
		EdgeMidpoints = 1 << 2
	}

	[Native]
	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	public enum GKRTreeSplitStrategy : long {
		Halve = 0,
		Linear = 1,
		Quadratic = 2,
		ReduceOverlap = 3
	}

	interface IGKAgentDelegate { }

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[Protocol]
	[Model][BaseType (typeof (NSObject))]
	interface GKAgentDelegate {
		
		[Export ("agentWillUpdate:")]
		void AgentWillUpdate (GKAgent agent);

		[Export ("agentDidUpdate:")]
		void AgentDidUpdate (GKAgent agent);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKComponent))]
	interface GKAgent : NSSecureCoding {
		
		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		IGKAgentDelegate Delegate { get; set; }

		[NullAllowed]
		[Export ("behavior", ArgumentSemantic.Retain)]
		GKBehavior Behavior { get; set; }

		[Export ("mass")]
		float Mass { get; set; }

		[Export ("radius")]
		float Radius { get; set; }

		[Export ("speed")]
		float Speed { get; [iOS (10,0), TV(10,0), Mac (10,12, onlyOn64: true)] set; }

		[Export ("maxAcceleration")]
		float MaxAcceleration { get; set; }

		[Export ("maxSpeed")]
		float MaxSpeed { get; set; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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

#if !XAMCORE_4_0
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
#if XAMCORE_4_0
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
	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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
		[Export ("objectForKeyedSubscript:")]
		NSNumber ObjectForKeyedSubscript (GKGoal goal);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	interface GKComponent : NSCopying, NSSecureCoding {

		[NullAllowed]
		[Export ("entity", ArgumentSemantic.Weak)]
		GKEntity Entity { get; }

		[Export ("updateWithDeltaTime:")]
		void Update (double deltaTimeInSeconds);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("didAddToEntity")]
		void DidAddToEntity ();

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("willRemoveFromEntity")]
		void WillRemoveFromEntity ();
	}

#if XAMCORE_2_0 // GKComponentSystem is a generic type, which we only support in Unified (for now at least)
	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // We have a manual default ctor.
	/* using 'TComponent' for the generic argument, since we have an additional member 'ComponentType' which clashes with Objective-C's generic argument 'ComponentType' */
	interface GKComponentSystem<TComponent>
		where TComponent : GKComponent
	{

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
		IntPtr Constructor (Class cls);

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
		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("classForGenericArgumentAtIndex:")]
		Class GetClassForGenericArgument (nuint index);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Wrap ("Class.Lookup (GetClassForGenericArgument (index))")]
		Type GetTypeForGenericArgument (nuint index);
	}
#endif // XAMCORE_2_0

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKDecisionTree : NSSecureCoding {

		[NullAllowed, Export ("rootNode")]
		GKDecisionNode RootNode { get; }

		[Export ("randomSource", ArgumentSemantic.Copy)]
		GKRandomSource RandomSource { get; set; }

		[Export ("initWithAttribute:")]
		IntPtr Constructor (NSObject attribute);

		[Export ("initWithExamples:actions:attributes:")]
#if XAMCORE_2_0
		IntPtr Constructor (NSArray<NSObject> [] examples, NSObject [] actions, NSObject [] attributes);
#else
		IntPtr Constructor (NSArray [] examples, NSObject [] actions, NSObject [] attributes);
#endif
		[Export ("findActionForAnswers:")]
		[return: NullAllowed]
		NSObject FindAction (NSDictionary<NSObject, NSObject> answers);

		[iOS (11,0), TV (11,0)]
		[Mac (10,13, onlyOn64: true)]
		[Export ("initWithURL:error:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSError error);

		[iOS (11,0), TV (11,0)]
		[Mac (10,13, onlyOn64: true)]
		[Export ("exportToURL:error:")]
		bool Export (NSUrl url, [NullAllowed] NSError error);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface GKEntity : NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[Protocol]
	interface GKGameModelUpdate {

		[Abstract]
		[Export ("value", ArgumentSemantic.Assign)]
		nint Value { get; set; }
	}

	interface IGKGameModelPlayer { }

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[Protocol]
	interface GKGameModelPlayer {

#if XAMCORE_4_0
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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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

		[Mac (10,11,2, onlyOn64 : true)]
		[iOS (9,1)][TV (9,0)]
		[Export ("unapplyGameModelUpdate:")]
		void UnapplyGameModelUpdate (IGKGameModelUpdate gameModelUpdate);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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
		GKGoal GetGoalToAvoidObstacles (GKObstacle[] obstacles, double maxPredictionTime);

		[Static]
		[Export ("goalToAvoidAgents:maxPredictionTime:")]
		GKGoal GetGoalToAvoidAgents (GKAgent[] agents, double maxPredictionTime);

		[Static]
		[Export ("goalToSeparateFromAgents:maxDistance:maxAngle:")]
		GKGoal GetGoalToSeparate (GKAgent[] agents, float maxDistance, float maxAngle);

		[Static]
		[Export ("goalToAlignWithAgents:maxDistance:maxAngle:")]
		GKGoal GetGoalToAlign (GKAgent[] agents, float maxDistance, float maxAngle);

		[Static]
		[Export ("goalToCohereWithAgents:maxDistance:maxAngle:")]
		GKGoal GetGoalToCohere (GKAgent[] agents, float maxDistance, float maxAngle);

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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface GKGraph : NSCopying, NSSecureCoding {
		
		[NullAllowed]
		[Export ("nodes")]
		GKGraphNode [] Nodes { get; }

		[Static]
		[Export ("graphWithNodes:")]
		GKGraph FromNodes (GKGraphNode [] nodes);

		[Export ("initWithNodes:")]
		IntPtr Constructor (GKGraphNode [] nodes);

		[Export ("connectNodeToLowestCostNode:bidirectional:")]
		void ConnectNodeToLowestCostNode (GKGraphNode node, bool bidirectional);

		[Export ("removeNodes:")]
		void RemoveNodes (GKGraphNode[] nodes);

		[Export ("addNodes:")]
		void AddNodes (GKGraphNode[] nodes);

		[Export ("findPathFromNode:toNode:")]
		GKGraphNode[] FindPath (GKGraphNode startNode, GKGraphNode endNode);
	}

	interface GKObstacleGraph<NodeType> : GKObstacleGraph { }

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKGraph))]
	interface GKObstacleGraph {
		
		[Export ("obstacles")]
		GKPolygonObstacle [] Obstacles { get; }

		[Export ("bufferRadius")]
		float BufferRadius { get; }

		[Static]
		[Export ("graphWithObstacles:bufferRadius:")]
		GKObstacleGraph FromObstacles (GKPolygonObstacle[] obstacles, float bufferRadius);

		[Export ("initWithObstacles:bufferRadius:")]
		IntPtr Constructor (GKPolygonObstacle [] obstacles, float bufferRadius);

		[Internal]
		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Static]
		[Export ("graphWithObstacles:bufferRadius:nodeClass:")]
		IntPtr GraphWithObstacles (GKPolygonObstacle [] obstacles, float bufferRadius, Class nodeClass);

		[Internal]
		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("initWithObstacles:bufferRadius:nodeClass:")]
		IntPtr Constructor (GKPolygonObstacle [] obstacles, float bufferRadius, Class nodeClass);

		[Export ("connectNodeUsingObstacles:")]
		void ConnectNodeUsingObstacles (GKGraphNode2D node);

		[Export ("connectNodeUsingObstacles:ignoringObstacles:")]
		void ConnectNodeUsingObstacles (GKGraphNode2D node, GKPolygonObstacle[] obstaclesToIgnore);

		[Export ("connectNodeUsingObstacles:ignoringBufferRadiusOfObstacles:")]
		void ConnectNodeUsingObstaclesIgnoringBufferRadius (GKGraphNode2D node, GKPolygonObstacle[] obstaclesBufferRadiusToIgnore);

		[Export ("addObstacles:")]
		void AddObstacles (GKPolygonObstacle[] obstacles);

		[Export ("removeObstacles:")]
		void RemoveObstacles (GKPolygonObstacle[] obstacles);

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
		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("classForGenericArgumentAtIndex:")]
		Class GetClassForGenericArgument (nuint index);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Wrap ("Class.Lookup (GetClassForGenericArgument (index))")]
		Type GetTypeForGenericArgument (nuint index);
	}

	// Apple turned GKGridGraph into a generic type in Xcode8 update
	// 	interface GKGridGraph<NodeType : GKGridGraphNode *>  : GKGraph
	// but we are not doing it since there is not much value to do it right now
	// due to it is only used in the return type of GetNodeAt which in docs says
	// it returns a GKGridGraphNode and we avoid a breaking change. Added a generic GetNodeAt.
	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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
		IntPtr Constructor (Vector2i position, int width, int height, bool diagonalsAllowed);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Static]
		[Export ("graphFromGridStartingAt:width:height:diagonalsAllowed:nodeClass:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKGridGraph FromGridStartingAt (Vector2i position, int width, int height, bool diagonalsAllowed, Class aClass);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Static]
		[Wrap ("FromGridStartingAt (position, width, height, diagonalsAllowed, new Class (type))")]
		GKGridGraph FromGridStartingAt (Vector2i position, int width, int height, bool diagonalsAllowed, Type type);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("initFromGridStartingAt:width:height:diagonalsAllowed:nodeClass:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (Vector2i position, int width, int height, bool diagonalsAllowed, Class aClass);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Wrap ("this (position, width, height, diagonalsAllowed, new Class (nodeType))")]
		IntPtr Constructor (Vector2i position, int width, int height, bool diagonalsAllowed, Type nodeType);

		[Internal]
		[Export ("nodeAtGridPosition:")]
		[return: NullAllowed]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr _GetNodeAt (Vector2i position);

		[Export ("connectNodeToAdjacentNodes:")]
		void ConnectNodeToAdjacentNodes (GKGridGraphNode node);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("classForGenericArgumentAtIndex:")]
		Class GetClassForGenericArgument (nuint index);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Wrap ("Class.Lookup (GetClassForGenericArgument (index))")]
		Type GetTypeForGenericArgument (nuint index);
	}

#if XAMCORE_2_0
	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (float bufferRadius, Vector2 min, Vector2 max, Class nodeClass);

		[Wrap ("this (bufferRadius, min, max, new Class (nodeType))")]
		IntPtr Constructor (float bufferRadius, Vector2 min, Vector2 max, Type nodeType);

		[Static]
		[Export ("graphWithBufferRadius:minCoordinate:maxCoordinate:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKMeshGraph<NodeType> FromBufferRadius (float bufferRadius, Vector2 min, Vector2 max);

		[Export ("initWithBufferRadius:minCoordinate:maxCoordinate:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (float bufferRadius, Vector2 min, Vector2 max);

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

		[Wrap ("Class.Lookup (GetClassForGenericArgument (index))")]
		Type GetTypeForGenericArgument (nuint index);
	}
#endif

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface GKGraphNode : NSSecureCoding {
		
		[Export ("connectedNodes")]
		GKGraphNode [] ConnectedNodes { get; }

		[Export ("addConnectionsToNodes:bidirectional:")]
		void AddConnections (GKGraphNode [] nodes, bool bidirectional);

		[Export ("removeConnectionsToNodes:bidirectional:")]
		void RemoveConnections (GKGraphNode[] nodes, bool bidirectional);

		[Export ("estimatedCostToNode:")]
		float GetEstimatedCost (GKGraphNode node);

		[Export ("costToNode:")]
		float GetCost (GKGraphNode node);

		[Export ("findPathToNode:")]
		GKGraphNode [] FindPathTo (GKGraphNode goalNode);

		[Export ("findPathFromNode:")]
		GKGraphNode [] FindPathFrom (GKGraphNode startNode);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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
		IntPtr Constructor (Vector2 point);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (Vector3 point);
	}

	[DisableDefaultCtor]
	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKGraphNode))]
	interface GKGridGraphNode {

		[Export ("gridPosition", ArgumentSemantic.Assign)]
		Vector2i GridPosition { 
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
#if !XAMCORE_2_0
			// classic expose the xamarin_simd__void_objc_msgSend[Super]_Vector2i so it's a breaking change not to include the attribute
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
#endif
#if !XAMCORE_4_0
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
		IntPtr Constructor (Vector2i gridPosition);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	interface GKObstacle {
	}
		
	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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
		IntPtr Constructor (float radius);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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
		IntPtr Constructor (IntPtr points, nuint numPoints);

		[Export ("vertexAtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetVertex (nuint index);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (float radius);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Static, Internal]
		[Export ("pathWithFloat3Points:count:radius:cyclical:")]
		GKPath FromFloat3Points (IntPtr points, nuint count, float radius, bool cyclical);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (GKGraphNode [] nodes, float radius);

		[Wrap ("this (nodes: graphNodes, radius: radius)")] // Avoid breaking change
		IntPtr Constructor (GKGraphNode2D [] graphNodes, float radius);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'GetVector2Point' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'GetVector2Point' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'GetVector2Point' instead.")]
		[Export ("pointAtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetPoint (nuint index);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("float2AtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetVector2Point (nuint index);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
		[Export ("float3AtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector3 GetVector3Point (nuint index);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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
		IntPtr Constructor (IGKRandom source, nint lowestInclusive, nint highestInclusive);

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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKRandomDistribution))]
	interface GKGaussianDistribution {
		
		[Export ("mean")]
		float Mean { get; }

		[Export ("deviation")]
		float Deviation { get; }

		[Export ("initWithRandomSource:lowestValue:highestValue:")]
		[DesignatedInitializer]
		IntPtr Constructor (IGKRandom source, nint lowestInclusive, nint highestInclusive);

		[Export ("initWithRandomSource:mean:deviation:")]
		[DesignatedInitializer]
		IntPtr Constructor (IGKRandom source, float mean, float deviation);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKRandomDistribution))]
	interface GKShuffledDistribution {

		// inlined from base type
		[Export ("initWithRandomSource:lowestValue:highestValue:")]
		[DesignatedInitializer]
		IntPtr Constructor (IGKRandom source, nint lowestInclusive, nint highestInclusive);
	}

	interface IGKRandom { }

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface GKRandomSource : GKRandom, NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Static]
		[Export ("sharedRandom")]
		GKRandomSource SharedRandom { get; }

		[Export ("arrayByShufflingObjectsInArray:")]
		NSObject [] ShuffleObjects (NSObject [] array);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKRandomSource))]
	interface GKARC4RandomSource {
		
		[Export ("seed", ArgumentSemantic.Copy)]
		NSData Seed { get; set; }

		[Export ("initWithSeed:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData seed);

		[Export ("dropValuesWithCount:")]
		void DropValues (nuint count);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKRandomSource))]
	interface GKLinearCongruentialRandomSource {
		
		[Export ("seed")]
		ulong Seed { get; set; }

		[Export ("initWithSeed:")]
		[DesignatedInitializer]
		IntPtr Constructor (ulong seed);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKRandomSource))]
	interface GKMersenneTwisterRandomSource {
		
		[Export ("seed")]
		ulong Seed { get; set; }

		[Export ("initWithSeed:")]
		[DesignatedInitializer]
		IntPtr Constructor (ulong seed);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface GKRuleSystem {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("evaluate")]
		void Evaluate ();

		[Export ("state", ArgumentSemantic.Retain)]
		NSMutableDictionary State { get; }

		[Export ("rules", ArgumentSemantic.Retain)]
		GKRule [] Rules { get; }

		[Export ("addRule:")]
		void AddRule (GKRule rule);

		[Export ("addRulesFromArray:")]
		void AddRules (GKRule[] rules);

		[Export ("removeAllRules")]
		void RemoveAllRules ();

		[Export ("agenda", ArgumentSemantic.Retain)]
		GKRule [] Agenda { get; }

		[Export ("executed", ArgumentSemantic.Retain)]
		GKRule[] Executed { get; }

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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKRule))]
	interface GKNSPredicateRule {
		
		[Export ("predicate", ArgumentSemantic.Retain)]
		NSPredicate Predicate { get; }

		[Export ("initWithPredicate:")]
		IntPtr Constructor (NSPredicate predicate);

		[Export ("evaluatePredicateWithSystem:"), New]
		bool EvaluatePredicate (GKRuleSystem system);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[DisableDefaultCtor] // designated
	interface GKState {

		[Protected]
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
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
		IntPtr Constructor (GKState[] states);

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

	[NoMac]
	[iOS (9,1)][TV (9,0)]
	[BaseType (typeof (NSObject))]
	interface GKHybridStrategist : GKStrategist {
		[Export ("budget")]
		nuint Budget { get; set; }

		[Export ("explorationParameter")]
		nuint ExplorationParameter { get; set; }

		[Export ("maxLookAheadDepth")]
		nuint MaxLookAheadDepth { get; set; }
	}

	[iOS (9,1)][TV (9,0)]
	[Protocol]
	interface GKStrategist {
		[Abstract]
		[NullAllowed, Export ("gameModel", ArgumentSemantic.Retain)]
		IGKGameModel GameModel { get; set; }

		[Abstract]
		[NullAllowed, Export ("randomSource", ArgumentSemantic.Retain)]
		IGKRandom RandomSource { get; set; }

		[Abstract]
		[NullAllowed, Export ("bestMoveForActivePlayer")]
		IGKGameModelUpdate GetBestMoveForActivePlayer ();
	}

	[iOS (9,1), TV (9,0), Mac (10,12, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface GKMonteCarloStrategist : GKStrategist {
		[Export ("budget")]
		nuint Budget { get; set; }

		[Export ("explorationParameter")]
		nuint ExplorationParameter { get; set; }
	}

	[iOS (10,0), TV (10, 0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (GKNoiseSource noiseSource);

		[Export ("initWithNoiseSource:gradientColors:")]
		[DesignatedInitializer]
		IntPtr Constructor (GKNoiseSource noiseSource, NSDictionary<NSNumber, SKColor> gradientColors);

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

#if !XAMCORE_4_0
		[Obsolete ("Use 'GKNoise.Displace' instead.")]
		[Wrap ("Displace (xDisplacementNoise, yDisplacementNoise, zDisplacementNoise)", isVirtual: true)]
		void DisplaceX (GKNoise xDisplacementNoise, GKNoise yDisplacementNoise, GKNoise zDisplacementNoise);
#endif
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (GKNoise noise);

		[Export ("initWithNoise:size:origin:sampleCount:seamless:")]
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (GKNoise noise, Vector2d size, Vector2d origin, Vector2i sampleCount, bool seamless);

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

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[DisableDefaultCtor]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface GKNoiseSource {

	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (double frequency, nint octaveCount, double persistence, double lacunarity, int seed);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (double frequency, nint octaveCount, double persistence, double lacunarity, int seed);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[DisableDefaultCtor]
	[BaseType (typeof (GKCoherentNoiseSource))]
	interface GKRidgedNoiseSource {

		[Static]
		[Export ("ridgedNoiseSourceWithFrequency:octaveCount:lacunarity:seed:")]
		GKRidgedNoiseSource Create (double frequency, nint octaveCount, double lacunarity, int seed);

		[Export ("initWithFrequency:octaveCount:lacunarity:seed:")]
		[DesignatedInitializer]
		IntPtr Constructor (double frequency, nint octaveCount, double lacunarity, int seed);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (double frequency, double displacement, bool distanceEnabled, int seed);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (double value);
	}

	[iOS (10, 0), TV (10, 0), Mac (10, 12, onlyOn64: true)]
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
		IntPtr Constructor (double frequency);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (double frequency);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (double squareSize);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKOctreeNode {

		[Export ("box")]
		GKBox Box {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}
	}

#if XAMCORE_2_0
	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface GKOctree <ElementType> where ElementType : NSObject {

		[Static]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[Export ("octreeWithBoundingBox:minimumCellSize:")]
		GKOctree<ElementType> FromBoundingBox (GKBox box, float minCellSize);

		[Export ("initWithBoundingBox:minimumCellSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (GKBox box, float minCellSize);

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

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (nuint maxNumberOfChildren);

		[Export ("addElement:boundingRectMin:boundingRectMax:splitStrategy:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void AddElement (ElementType element, Vector2 boundingRectMin, Vector2 boundingRectMax, GKRTreeSplitStrategy splitStrategy);

		[Export ("removeElement:boundingRectMin:boundingRectMax:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		void RemoveElement (ElementType element, Vector2 boundingRectMin, Vector2 boundingRectMax);

		[Export ("elementsInBoundingRectMin:rectMax:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		ElementType [] GetElements (Vector2 rectMin, Vector2 rectMax);
	}
#endif

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[BaseType (typeof (GKComponent))]
	interface GKSKNodeComponent : GKAgentDelegate {

		[Static]
		[Export ("componentWithNode:")]
		GKSKNodeComponent FromNode (SKNode node);

		[Export ("initWithNode:")]
		IntPtr Constructor (SKNode node);

		[Export ("node", ArgumentSemantic.Strong)]
		SKNode Node { get; set; }
	}

	interface IGKSceneRootNodeType { }

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[Protocol]
	interface GKSceneRootNodeType {
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface GKScene : NSCopying, NSSecureCoding {

		[Static]
		[Export ("sceneWithFileNamed:")]
		[return: NullAllowed]
		GKScene FromFile (string filename);

		[iOS (11,0)][TV (11,0)][Mac (10,13, onlyOn64: true)]
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

	[iOS (11,0)]
	[TV (11,0)]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof(GKComponent))]
	interface GKSCNNodeComponent : GKAgentDelegate
	{
		[Static]
		[Export ("componentWithNode:")]
		GKSCNNodeComponent FromNode (SCNNode node);

		[Export ("initWithNode:")]
		IntPtr Constructor (SCNNode node);

		[Export ("node")]
		SCNNode Node { get; }
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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

		[Export ("entity")]
		GKEntity GetEntity ();

		[Export ("setEntity:")]
		void SetEntity (GKEntity entity);
	}

	[iOS (11,0), TV (11,0), Mac (10,13, onlyOn64: true)]
	[Category]
	[BaseType (typeof (SCNNode))]
	interface SCNNode_GameplayKit {
		[Export ("entity")]
		GKEntity GetEntity ();

		[Export ("setEntity:")]
		void SetEntity (GKEntity entity);
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
	[BaseType (typeof (NSObject), Name = "GKQuadtreeNode")] // Renamed to GKQuadtreeNode (lower case t) in Xcode8
	[DisableDefaultCtor] // <quote>Used as a hint for faster removal via [GKQuadTree removeData:WithNode:]</quote>
	interface GKQuadTreeNode {

		[Export ("quad")]
		GKQuad Quad {
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get;
		}
	}

	[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64: true)]
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
		IntPtr Constructor (GKQuad quad, float minCellSize);

		[Export ("addElement:withPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
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

#if !XAMCORE_4_0 && !MONOMAC // This API is removed in Xcode 8

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Export ("initWithMinPosition:maxPosition:minCellSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (Vector2 min, Vector2 max, float minCellSize);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Export ("addDataWithPoint:point:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKQuadTreeNode AddData (NSObject data, Vector2 point);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Export ("addDataWithQuad:quadOrigin:quadSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKQuadTreeNode AddData (NSObject data, Vector2 quadOrigin, Vector2 quadSize);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Export ("queryDataForPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NSObject[] QueryData (Vector2 point);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Export ("queryDataForQuad:quadSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NSObject[] QueryData (Vector2 quadOrigin, Vector2 quadSize);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Export ("removeData:")]
		bool RemoveData (NSObject data);

		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.TvOS, 10, 0)]
		[Export ("removeData:withNode:")]
		bool RemoveData (NSObject data, GKQuadTreeNode node);
#endif
	}
}
#endif
