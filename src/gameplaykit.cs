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
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using Vector2 = global::OpenTK.Vector2;
using Vector2i = global::OpenTK.Vector2i;

namespace XamCore.GameplayKit {

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
	interface GKAgent {
		
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
		float Speed { get; }

		[Export ("maxAcceleration")]
		float MaxAcceleration { get; set; }

		[Export ("maxSpeed")]
		float MaxSpeed { get; set; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKAgent))]
	interface GKAgent2D {
		
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
		void Update (double seconds);
	}

	// FIXME: @interface GKBehavior : NSObject <NSFastEnumeration>
	// Fix when we have NSFastEnumerator to IEnumerable support
	// https://bugzilla.xamarin.com/show_bug.cgi?id=4391
	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface GKBehavior {
		
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
	interface GKComponent : NSCopying {

		[NullAllowed]
		[Export ("entity", ArgumentSemantic.Weak)]
		GKEntity Entity { get; }

		[Export ("updateWithDeltaTime:")]
		void Update (double seconds);
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
		void Update (double seconds);
	}
#endif // XAMCORE_2_0

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface GKEntity : NSCopying {
		
		[Static]
		[Export ("entity")]
		GKEntity GetEntity ();

		[Export ("updateWithDeltaTime:")]
		void Update (double seconds);

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

		[NoMac] // not yet
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
	interface GKGraph {
		
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

		[Export ("nodesForObstacle:")]
		GKGraphNode2D [] GetNodes (GKPolygonObstacle obstacle);

		[Export ("lockConnectionFromNode:toNode:")]
		void LockConnection (GKGraphNode2D startNode, GKGraphNode2D endNode);

		[Export ("unlockConnectionFromNode:toNode:")]
		void UnlockConnection (GKGraphNode2D startNode, GKGraphNode2D endNode);

		[Export ("isConnectionLockedFromNode:toNode:")]
		bool IsConnectionLocked (GKGraphNode2D startNode, GKGraphNode2D endNode);
	}

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

		[Export ("nodeAtGridPosition:")]
		[return: NullAllowed]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKGridGraphNode GetNodeAt (Vector2i position);

		[Export ("connectNodeToAdjacentNodes:")]
		void ConnectNodeToAdjacentNodes (GKGridGraphNode node);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface GKGraphNode {
		
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
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (Vector2 point);
	}

	[DisableDefaultCtor]
	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (GKGraphNode))]
	interface GKGridGraphNode {

		[Export ("gridPosition", ArgumentSemantic.Assign)]
		Vector2i GridPosition { 
			[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
			get; 
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
		[DesignatedInitializer]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		IntPtr Constructor (Vector2i gridPosition);
	}

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject), Name = "GKMinmaxStrategist")]
#if !MONOMAC
	interface GKMinMaxStrategist : GKStrategist {
#else
	interface GKMinMaxStrategist {
		// GKStrategist protocol is not yet available
		[NullAllowed]
		[Export ("gameModel", ArgumentSemantic.Retain)]
		IGKGameModel GameModel { get; set; }

		[NullAllowed]
		[Export ("randomSource", ArgumentSemantic.Retain)]
		IGKRandom RandomSource { get; set; }
#endif		
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
	interface GKPolygonObstacle {

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

	[iOS (9,0), Mac (10,11, onlyOn64 : true)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface GKPath {
		
		[Export ("radius")]
		float Radius { get; set; }

		[Export ("cyclical")]
		bool Cyclical { [Bind ("isCyclical")] get; set; }

		[Export ("numPoints")]
		nuint NumPoints { get; }

		[Static, Internal]
		[Export ("pathWithPoints:count:radius:cyclical:")]
		GKPath FromPoints (IntPtr points, nuint count, float radius, bool cyclical);

		[Internal]
		[Export ("initWithPoints:count:radius:cyclical:")]
		[DesignatedInitializer]
		IntPtr InitWithPoints (IntPtr points, nuint count, float radius, bool cyclical);

		[Static]
		[Export ("pathWithGraphNodes:radius:")]
		GKPath FromGraphNodes (GKGraphNode2D [] graphNodes, float radius);

		[Export ("initWithGraphNodes:radius:")]
		IntPtr Constructor (GKGraphNode2D [] graphNodes, float radius);

		[Export ("pointAtIndex:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		Vector2 GetPoint (nuint index);
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
	interface GKRandomSource : GKRandom, NSSecureCoding, NSCopying {
		
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
	interface GKRuleSystem {
		
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
	interface GKState {

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
		void Update (double seconds);

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
		void Update (double seconds);

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

	[NoMac]
	[iOS (9,1)][TV (9,0)]
	[BaseType (typeof (NSObject))]
	interface GKMonteCarloStrategist : GKStrategist {
		[Export ("budget")]
		nuint Budget { get; set; }

		[Export ("explorationParameter")]
		nuint ExplorationParameter { get; set; }
	}

	[NoMac]
	[iOS (9,1)][TV (9,0)]
	[BaseType (typeof (NSObject))]
	// [DisableDefaultCtor] // <quote>Used as a hint for faster removal via [GKQuadTree removeData:WithNode:]</quote>
	interface GKQuadTreeNode {
	}

	[NoMac]
	[iOS (9,1)][TV (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // crash (endless recursion)
	interface GKQuadTree {

		[Export ("initWithMinPosition:maxPosition:minCellSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		[DesignatedInitializer]
		IntPtr Constructor (Vector2 min, Vector2 max, float minCellSize);

		[Export ("addDataWithPoint:point:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKQuadTreeNode AddData (NSObject data, Vector2 point);

		[Export ("addDataWithQuad:quadOrigin:quadSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		GKQuadTreeNode AddData (NSObject data, Vector2 quadOrigin, Vector2 quadSize);

		[Export ("queryDataForPoint:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NSObject[] QueryData (Vector2 point);

		[Export ("queryDataForQuad:quadSize:")]
		[MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
		NSObject[] QueryData (Vector2 quadOrigin, Vector2 quadSize);

		[Export ("removeData:")]
		bool RemoveData (NSObject data);

		[Export ("removeData:withNode:")]
		bool RemoveData (NSObject data, GKQuadTreeNode node);
	}
}
#endif
