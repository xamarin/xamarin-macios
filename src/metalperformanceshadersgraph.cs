using System;
using CoreGraphics;
using Foundation;
using Metal;
using MetalPerformanceShaders;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

using MPSGraphTensorDataDictionary = Foundation.NSDictionary<MetalPerformanceShadersGraph.MPSGraphTensor, MetalPerformanceShadersGraph.MPSGraphTensorData>;
using MPSGraphTensorShapedTypeDictionary = Foundation.NSDictionary<MetalPerformanceShadersGraph.MPSGraphTensor, MetalPerformanceShadersGraph.MPSGraphShapedType>;
using MPSShape = Foundation.NSArray<Foundation.NSNumber>;

namespace MetalPerformanceShadersGraph
{
	// MPSGraph.h

	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof (NSObject))]
	interface MPSGraph {
		[Static, Export ("new")]
		MPSGraph Create ();

		// @property (readwrite, atomic) MPSGraphOptions options;
		[Export ("options", ArgumentSemantic.Assign)]
		MPSGraphOptions Options { get; set; }

		// @property (readonly, nonatomic) NSArray<MPSGraphTensor *> * _Nonnull placeholderTensors;
		[Export ("placeholderTensors")]
		MPSGraphTensor[] PlaceholderTensors { get; }

		// -(MPSGraphExecutable * _Nonnull)compileWithDevice:(MPSGraphDevice * _Nullable)device feeds:(MPSGraphTensorShapedTypeDictionary * _Nonnull)feeds targetTensors:(NSArray<MPSGraphTensor *> * _Nonnull)targetTensors targetOperations:(NSArray<MPSGraphOperation *> * _Nullable)targetOperations compilationDescriptor:(MPSGraphCompilationDescriptor * _Nullable)compilationDescriptor __attribute__((availability(macos, introduced=12.0))) __attribute__((availability(ios, introduced=15.0))) __attribute__((availability(tvos, introduced=15.0)));
		[TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		[Export ("compileWithDevice:feeds:targetTensors:targetOperations:compilationDescriptor:")]
		MPSGraphExecutable CompileWithDevice ([NullAllowed] MPSGraphDevice device, NSDictionary<MPSGraphTensor, MPSGraphShapedType> feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations, [NullAllowed] MPSGraphCompilationDescriptor compilationDescriptor);

		[Export ("runWithFeeds:targetTensors:targetOperations:")]
		MPSGraphTensorDataDictionary Run (MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations);

		[Export ("runWithMTLCommandQueue:feeds:targetTensors:targetOperations:")]
		MPSGraphTensorDataDictionary Run (IMTLCommandQueue commandQueue, MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations);

		[Export ("runWithMTLCommandQueue:feeds:targetOperations:resultsDictionary:")]
		void Run (IMTLCommandQueue commandQueue, MPSGraphTensorDataDictionary feeds, [NullAllowed] MPSGraphOperation[] targetOperations, MPSGraphTensorDataDictionary resultsDictionary);

		[Export ("runAsyncWithFeeds:targetTensors:targetOperations:executionDescriptor:")]
		MPSGraphTensorDataDictionary RunAsync (MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

		[Export ("runAsyncWithMTLCommandQueue:feeds:targetTensors:targetOperations:executionDescriptor:")]
		MPSGraphTensorDataDictionary RunAsync (IMTLCommandQueue commandQueue, MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

		[Export ("runAsyncWithMTLCommandQueue:feeds:targetOperations:resultsDictionary:executionDescriptor:")]
		void RunAsync (IMTLCommandQueue commandQueue, MPSGraphTensorDataDictionary feeds, [NullAllowed] MPSGraphOperation[] targetOperations, MPSGraphTensorDataDictionary resultsDictionary, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

		[Export ("encodeToCommandBuffer:feeds:targetTensors:targetOperations:executionDescriptor:")]
		MPSGraphTensorDataDictionary Encode (MPSCommandBuffer commandBuffer, MPSGraphTensorDataDictionary feeds, MPSGraphTensor[] targetTensors, [NullAllowed] MPSGraphOperation[] targetOperations, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

		[Export ("encodeToCommandBuffer:feeds:targetOperations:resultsDictionary:executionDescriptor:")]
		void Encode (MPSCommandBuffer commandBuffer, MPSGraphTensorDataDictionary feeds, [NullAllowed] MPSGraphOperation[] targetOperations, MPSGraphTensorDataDictionary resultsDictionary, [NullAllowed] MPSGraphExecutionDescriptor executionDescriptor);

/*
		[Export("L2NormPooling4DGradientWithGradientTensor:sourceTensor:descriptor:name:")]
		MPSGraphTensor L2NormPooling4DGradient(MPSGraphTensor gradientTensor, MPSGraphTensor sourceTensor, MPSGraphPooling4DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("L2NormPooling4DWithSourceTensor:descriptor:name:")]
		MPSGraphTensor L2NormPooling4D(MPSGraphTensor sourceTensor, MPSGraphPooling4DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("absoluteWithTensor:name:")]
		MPSGraphTensor Absolute(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("acosWithTensor:name:")]
		MPSGraphTensor Acos(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("acoshWithTensor:name:")]
		MPSGraphTensor Acosh(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("additionWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Addition(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("applyStochasticGradientDescentWithLearningRateTensor:variable:gradientTensor:name:")]
		MPSGraphOperation ApplyStochasticGradientDescent(MPSGraphTensor learningRateTensor, MPSGraphVariableOp variable, MPSGraphTensor gradientTensor, [NullAllowed] string name);
		[Export("asinWithTensor:name:")]
		MPSGraphTensor Asin(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("asinhWithTensor:name:")]
		MPSGraphTensor Asinh(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("assignVariable:withValueOfTensor:name:")]
		MPSGraphOperation Assign(MPSGraphTensor assignVariable, MPSGraphTensor withValueOfTensor, [NullAllowed] string name);
		[Export("atan2WithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Atan2(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("atanWithTensor:name:")]
		MPSGraphTensor Atan(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("atanhWithTensor:name:")]
		MPSGraphTensor Atanh(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("avgPooling2DGradientWithGradientTensor:sourceTensor:descriptor:name:")]
		MPSGraphTensor AvgPooling2DGradient(MPSGraphTensor gradientTensor, MPSGraphTensor sourceTensor, MPSGraphPooling2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("avgPooling2DWithSourceTensor:descriptor:name:")]
		MPSGraphTensor AvgPooling2D(MPSGraphTensor sourceTensor, MPSGraphPooling2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("avgPooling4DGradientWithGradientTensor:sourceTensor:descriptor:name:")]
		MPSGraphTensor AvgPooling4DGradient(MPSGraphTensor gradientTensor, MPSGraphTensor sourceTensor, MPSGraphPooling4DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("avgPooling4DWithSourceTensor:descriptor:name:")]
		MPSGraphTensor AvgPooling4D(MPSGraphTensor sourceTensor, MPSGraphPooling4DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("broadcastTensor:toShape:name:")]
		MPSGraphTensor Broadcast(MPSGraphTensor tensor, MPSShape toShape, [NullAllowed] string name);
		[Export("broadcastTensor:toShapeTensor:name:")]
		MPSGraphTensor Broadcast(MPSGraphTensor tensor, MPSGraphTensor toShapeTensor, [NullAllowed] string name);
		[Export("castTensor:toType:name:")]
		MPSGraphTensor Cast(MPSGraphTensor tensor, MPSDataType toType, string name);
		[Export("ceilWithTensor:name:")]
		MPSGraphTensor Ceil(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("clampWithTensor:minValueTensor:maxValueTensor:name:")]
		MPSGraphTensor Clamp(MPSGraphTensor tensor, MPSGraphTensor minValueTensor, MPSGraphTensor maxValueTensor, [NullAllowed] string name);
		[Export("concatTensor:withTensor:dimension:name:")]
		MPSGraphTensor Concat(MPSGraphTensor tensor, MPSGraphTensor withTensor, nint dimension, [NullAllowed] string name);
		[Export("concatTensors:dimension:interleave:name:")]
		MPSGraphTensor Concat(MPSGraphTensor[] tensors, nint dimension, bool interleave, [NullAllowed] string name);
		[Export("concatTensors:dimension:name:")]
		MPSGraphTensor Concat(MPSGraphTensor[] tensors, nint dimension, [NullAllowed] string name);
		[Export("constantWithData:shape:dataType:")]
		MPSGraphTensor Constant(NSData data, MPSShape shape, MPSDataType dataType);
		[Export("constantWithScalar:dataType:")]
		MPSGraphTensor Constant(double scalar, MPSDataType dataType);
		[Export("constantWithScalar:shape:dataType:")]
		MPSGraphTensor Constant(double scalar, MPSShape shape, MPSDataType dataType);
		[Export("controlDependencyWithOperations:dependentBlock:name:")]
		MPSGraphTensor[] ControlDependency(MPSGraphOperation[] operations, MPSGraphControlFlowDependencyBlock dependentBlock, [NullAllowed] string name);
		[Export("convolution2DDataGradientWithIncomingGradientTensor:weightsTensor:outputShape:forwardConvolutionDescriptor:name:")]
		MPSGraphTensor Convolution2DDataGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor weightsTensor, MPSShape outputShape, MPSGraphConvolution2DOpDescriptor forwardConvolutionDescriptor, [NullAllowed] string name);
		[Export("convolution2DDataGradientWithIncomingGradientTensor:weightsTensor:outputShapeTensor:forwardConvolutionDescriptor:name:")]
		MPSGraphTensor Convolution2DDataGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor weightsTensor, MPSGraphTensor outputShapeTensor, MPSGraphConvolution2DOpDescriptor forwardConvolutionDescriptor, [NullAllowed] string name);
		[Export("convolution2DWeightsGradientWithIncomingGradientTensor:sourceTensor:outputShape:forwardConvolutionDescriptor:name:")]
		MPSGraphTensor Convolution2DWeightsGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSShape outputShape, MPSGraphConvolution2DOpDescriptor forwardConvolutionDescriptor, [NullAllowed] string name);
		[Export("convolution2DWeightsGradientWithIncomingGradientTensor:sourceTensor:outputShapeTensor:forwardConvolutionDescriptor:name:")]
		MPSGraphTensor Convolution2DWeightsGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSGraphTensor outputShapeTensor, MPSGraphConvolution2DOpDescriptor forwardConvolutionDescriptor, [NullAllowed] string name);
		[Export("convolution2DWithSourceTensor:weightsTensor:descriptor:name:")]
		MPSGraphTensor Convolution2D(MPSGraphTensor sourceTensor, MPSGraphTensor weightsTensor, MPSGraphConvolution2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("convolutionTranspose2DDataGradientWithIncomingGradientTensor:weightsTensor:outputShape:forwardConvolutionDescriptor:name:")]
		MPSGraphTensor ConvolutionTranspose2DDataGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor weightsTensor, MPSShape outputShape, MPSGraphConvolution2DOpDescriptor forwardConvolutionDescriptor, [NullAllowed] string name);
		[Export("convolutionTranspose2DDataGradientWithIncomingGradientTensor:weightsTensor:outputShapeTensor:forwardConvolutionDescriptor:name:")]
		MPSGraphTensor ConvolutionTranspose2DDataGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor weightsTensor, MPSGraphTensor outputShapeTensor, MPSGraphConvolution2DOpDescriptor forwardConvolutionDescriptor, [NullAllowed] string name);
		[Export("convolutionTranspose2DWeightsGradientWithIncomingGradientTensor:sourceTensor:outputShape:forwardConvolutionDescriptor:name:")]
		MPSGraphTensor ConvolutionTranspose2DWeightsGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSShape outputShape, MPSGraphConvolution2DOpDescriptor forwardConvolutionDescriptor, [NullAllowed] string name);
		[Export("convolutionTranspose2DWeightsGradientWithIncomingGradientTensor:sourceTensor:outputShapeTensor:forwardConvolutionDescriptor:name:")]
		MPSGraphTensor ConvolutionTranspose2DWeightsGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSGraphTensor outputShapeTensor, MPSGraphConvolution2DOpDescriptor forwardConvolutionDescriptor, [NullAllowed] string name);
		[Export("convolutionTranspose2DWithSourceTensor:weightsTensor:outputShape:descriptor:name:")]
		MPSGraphTensor ConvolutionTranspose2D(MPSGraphTensor sourceTensor, MPSGraphTensor weightsTensor, MPSShape outputShape, MPSGraphConvolution2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("convolutionTranspose2DWithSourceTensor:weightsTensor:outputShapeTensor:descriptor:name:")]
		MPSGraphTensor ConvolutionTranspose2D(MPSGraphTensor sourceTensor, MPSGraphTensor weightsTensor, MPSGraphTensor outputShapeTensor, MPSGraphConvolution2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("cosWithTensor:name:")]
		MPSGraphTensor Cos(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("coshWithTensor:name:")]
		MPSGraphTensor Cosh(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("depthToSpace2DTensor:widthAxis:heightAxis:depthAxis:blockSize:usePixelShuffleOrder:name:")]
		MPSGraphTensor DepthToSpace2D(MPSGraphTensor tensor, nuint widthAxis, nuint heightAxis, nuint depthAxis, nuint blockSize, bool usePixelShuffleOrder, [NullAllowed] string name);
		[Export("depthToSpace2DTensor:widthAxisTensor:heightAxisTensor:depthAxisTensor:blockSize:usePixelShuffleOrder:name:")]
		MPSGraphTensor DepthToSpace2D(MPSGraphTensor tensor, MPSGraphTensor widthAxisTensor, MPSGraphTensor heightAxisTensor, MPSGraphTensor depthAxisTensor, nuint blockSize, bool usePixelShuffleOrder, [NullAllowed] string name);
		[Export("depthwiseConvolution2DDataGradientWithIncomingGradientTensor:weightsTensor:outputShape:descriptor:name:")]
		MPSGraphTensor DepthwiseConvolution2DDataGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor weightsTensor, MPSShape outputShape, MPSGraphDepthwiseConvolution2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("depthwiseConvolution2DWeightsGradientWithIncomingGradientTensor:sourceTensor:outputShape:descriptor:name:")]
		MPSGraphTensor DepthwiseConvolution2DWeightsGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSShape outputShape, MPSGraphDepthwiseConvolution2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("depthwiseConvolution2DWithSourceTensor:weightsTensor:descriptor:name:")]
		MPSGraphTensor DepthwiseConvolution2D(MPSGraphTensor sourceTensor, MPSGraphTensor weightsTensor, MPSGraphDepthwiseConvolution2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("depthwiseConvolution3DDataGradientWithIncomingGradientTensor:weightsTensor:outputShape:descriptor:name:")]
		MPSGraphTensor DepthwiseConvolution3DDataGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor weightsTensor, [NullAllowed] MPSShape outputShape, MPSGraphDepthwiseConvolution3DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("depthwiseConvolution3DWeightsGradientWithIncomingGradientTensor:sourceTensor:outputShape:descriptor:name:")]
		MPSGraphTensor DepthwiseConvolution3DWeightsGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSShape outputShape, MPSGraphDepthwiseConvolution3DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("depthwiseConvolution3DWithSourceTensor:weightsTensor:descriptor:name:")]
		MPSGraphTensor DepthwiseConvolution3D(MPSGraphTensor sourceTensor, MPSGraphTensor weightsTensor, MPSGraphDepthwiseConvolution3DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("divisionNoNaNWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor DivisionNoNaN(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("divisionWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Division(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("dropoutTensor:rate:name:")]
		MPSGraphTensor Dropout(MPSGraphTensor tensor, double rate, [NullAllowed] string name);
		[Export("dropoutTensor:rateTensor:name:")]
		MPSGraphTensor Dropout(MPSGraphTensor tensor, MPSGraphTensor rateTensor, [NullAllowed] string name);
		[Export("equalWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Equal(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("erfWithTensor:name:")]
		MPSGraphTensor Erf(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("exponentBase10WithTensor:name:")]
		MPSGraphTensor ExponentBase10(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("exponentBase2WithTensor:name:")]
		MPSGraphTensor ExponentBase2(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("exponentWithTensor:name:")]
		MPSGraphTensor Exponent(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("flatten2DTensor:axis:name:")]
		MPSGraphTensor Flatten2D(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("flatten2DTensor:axisTensor:name:")]
		MPSGraphTensor Flatten2D(MPSGraphTensor tensor, MPSGraphTensor axisTensor, [NullAllowed] string name);
		[Export("floorModuloWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor FloorModulo(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("floorWithTensor:name:")]
		MPSGraphTensor Floor(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("forLoopWithLowerBound:upperBound:step:initialBodyArguments:body:name:")]
		MPSGraphTensor[] For(MPSGraphTensor lowerBound, MPSGraphTensor upperBound, MPSGraphTensor step, MPSGraphTensor[] initialBodyArguments, MPSGraphForLoopBodyBlock body, [NullAllowed] string name);
		[Export("forLoopWithNumberOfIterations:initialBodyArguments:body:name:")]
		MPSGraphTensor[] For(MPSGraphTensor numberOfIterations, MPSGraphTensor[] initialBodyArguments, MPSGraphForLoopBodyBlock body, [NullAllowed] string name);
		[Export("gatherNDWithUpdatesTensor:indicesTensor:batchDimensions:name:")]
		MPSGraphTensor GatherND(MPSGraphTensor updatesTensor, MPSGraphTensor indicesTensor, nuint batchDimensions, [NullAllowed] string name);
		[Export("gatherWithUpdatesTensor:indicesTensor:axis:batchDimensions:name:")]
		MPSGraphTensor Gather(MPSGraphTensor updatesTensor, MPSGraphTensor indicesTensor, nuint axis, nuint batchDimensions, [NullAllowed] string name);
		[Export("greaterThanOrEqualToWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor GreaterThanOrEqualTo(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("greaterThanWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor GreaterThan(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("identityWithTensor:name:")]
		MPSGraphTensor Identity(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("ifWithPredicateTensor:thenBlock:elseBlock:name:")]
		MPSGraphTensor[] If(MPSGraphTensor predicateTensor, MPSGraphIfThenElseBlock thenBlock, MPSGraphIfThenElseBlock elseBlock, [NullAllowed] string name);
		[Export("isFiniteWithTensor:name:")]
		MPSGraphTensor IsFinite(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("isInfiniteWithTensor:name:")]
		MPSGraphTensor IsInfinite(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("isNaNWithTensor:name:")]
		MPSGraphTensor IsNaN(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("leakyReLUGradientWithIncomingGradient:sourceTensor:alphaTensor:name:")]
		MPSGraphTensor LeakyReLUGradient(MPSGraphTensor incomingGradient, MPSGraphTensor sourceTensor, MPSGraphTensor alphaTensor, [NullAllowed] string name);
		[Export("leakyReLUWithTensor:alpha:name:")]
		MPSGraphTensor LeakyReLU(MPSGraphTensor tensor, double alpha, [NullAllowed] string name);
		[Export("leakyReLUWithTensor:alphaTensor:name:")]
		MPSGraphTensor LeakyReLU(MPSGraphTensor tensor, MPSGraphTensor alphaTensor, [NullAllowed] string name);
		[Export("lessThanOrEqualToWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor LessThanOrEqualTo(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("lessThanWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor LessThan(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("logarithmBase10WithTensor:name:")]
		MPSGraphTensor LogarithmBase10(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("logarithmBase2WithTensor:name:")]
		MPSGraphTensor LogarithmBase2(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("logarithmWithTensor:name:")]
		MPSGraphTensor Logarithm(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("logicalANDWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor LogicalAND(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("logicalNANDWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor LogicalNAND(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("logicalNORWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor LogicalNOR(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("logicalORWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor LogicalOR(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("logicalXNORWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor LogicalXNOR(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("logicalXORWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor LogicalXOR(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("matrixMultiplicationWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor MatrixMultiplication(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("maxPooling2DGradientWithGradientTensor:sourceTensor:descriptor:name:")]
		MPSGraphTensor MaxPooling2DGradient(MPSGraphTensor gradientTensor, MPSGraphTensor sourceTensor, MPSGraphPooling2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("maxPooling2DWithSourceTensor:descriptor:name:")]
		MPSGraphTensor MaxPooling2D(MPSGraphTensor sourceTensor, MPSGraphPooling2DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("maxPooling4DGradientWithGradientTensor:sourceTensor:descriptor:name:")]
		MPSGraphTensor MaxPooling4DGradient(MPSGraphTensor gradientTensor, MPSGraphTensor sourceTensor, MPSGraphPooling4DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("maxPooling4DWithSourceTensor:descriptor:name:")]
		MPSGraphTensor MaxPooling4D(MPSGraphTensor sourceTensor, MPSGraphPooling4DOpDescriptor descriptor, [NullAllowed] string name);
		[Export("maximumWithNaNPropagationWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor MaximumWithNaNPropagation(MPSGraphTensor naNPropagationWithPrimaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("maximumWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Maximum(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("meanOfTensor:axes:name:")]
		MPSGraphTensor Mean(MPSGraphTensor tensor, NSNumber[] axes, [NullAllowed] string name);
		[Export("minimumWithNaNPropagationWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor MinimumWithNaNPropagation(MPSGraphTensor naNPropagationWithPrimaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("minimumWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Minimum(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("moduloWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Modulo(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("multiplicationWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Multiplication(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("negativeWithTensor:name:")]
		MPSGraphTensor Negative(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("normalizationBetaGradientWithIncomingGradientTensor:sourceTensor:reductionAxes:name:")]
		MPSGraphTensor NormalizationBetaGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, NSNumber[] reductionAxes, [NullAllowed] string name);
		[Export("normalizationGammaGradientWithIncomingGradientTensor:sourceTensor:meanTensor:varianceTensor:reductionAxes:epsilon:name:")]
		MPSGraphTensor NormalizationGammaGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSGraphTensor meanTensor, MPSGraphTensor varianceTensor, NSNumber[] reductionAxes, float epsilon, [NullAllowed] string name);
		[Export("normalizationGradientWithIncomingGradientTensor:sourceTensor:meanTensor:varianceTensor:gammaTensor:gammaGradientTensor:betaGradientTensor:reductionAxes:epsilon:name:")]
		MPSGraphTensor NormalizationGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSGraphTensor meanTensor, MPSGraphTensor varianceTensor, [NullAllowed] MPSGraphTensor gammaTensor, [NullAllowed] MPSGraphTensor gammaGradientTensor, [NullAllowed] MPSGraphTensor betaGradientTensor, NSNumber[] reductionAxes, float epsilon, [NullAllowed] string name);
		[Export("normalizationWithTensor:meanTensor:varianceTensor:gammaTensor:betaTensor:epsilon:name:")]
		MPSGraphTensor Normalize(MPSGraphTensor tensor, MPSGraphTensor meanTensor, MPSGraphTensor varianceTensor, [NullAllowed] MPSGraphTensor gammaTensor, [NullAllowed] MPSGraphTensor betaTensor, float epsilon, [NullAllowed] string name);
		[Export("notEqualWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor NotEqual(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("notWithTensor:name:")]
		MPSGraphTensor Not(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("oneHotWithIndicesTensor:depth:axis:dataType:name:")]
		MPSGraphTensor OneHot(MPSGraphTensor indicesTensor, nuint depth, nuint axis, MPSDataType dataType, [NullAllowed] string name);
		[Export("oneHotWithIndicesTensor:depth:axis:dataType:onValue:offValue:name:")]
		MPSGraphTensor OneHot(MPSGraphTensor indicesTensor, nuint depth, nuint axis, MPSDataType dataType, double onValue, double offValue, [NullAllowed] string name);
		[Export("oneHotWithIndicesTensor:depth:axis:name:")]
		MPSGraphTensor OneHot(MPSGraphTensor indicesTensor, nuint depth, nuint axis, [NullAllowed] string name);
		[Export("oneHotWithIndicesTensor:depth:dataType:name:")]
		MPSGraphTensor OneHot(MPSGraphTensor indicesTensor, nuint depth, MPSDataType dataType, [NullAllowed] string name);
		[Export("oneHotWithIndicesTensor:depth:dataType:onValue:offValue:name:")]
		MPSGraphTensor OneHot(MPSGraphTensor indicesTensor, nuint depth, MPSDataType dataType, double onValue, double offValue, [NullAllowed] string name);
		[Export("oneHotWithIndicesTensor:depth:name:")]
		MPSGraphTensor OneHot(MPSGraphTensor indicesTensor, nuint depth, [NullAllowed] string name);
		[Export("padGradientWithIncomingGradientTensor:sourceTensor:paddingMode:leftPadding:rightPadding:name:")]
		MPSGraphTensor PadGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSGraphPaddingMode paddingMode, MPSShape leftPadding, MPSShape rightPadding, [NullAllowed] string name);
		[Export("padTensor:withPaddingMode:leftPadding:rightPadding:constantValue:name:")]
		MPSGraphTensor Pad(MPSGraphTensor tensor, MPSGraphPaddingMode withPaddingMode, MPSShape leftPadding, MPSShape rightPadding, double constantValue, [NullAllowed] string name);
		[Export("placeholderWithShape:dataType:name:")]
		MPSGraphTensor Placeholder([NullAllowed] MPSShape shape, MPSDataType dataType, [NullAllowed] string name);
		[Export("placeholderWithShape:name:")]
		MPSGraphTensor Placeholder([NullAllowed] MPSShape shape, [NullAllowed] string name);
		[Export("powerWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Power(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("randomPhiloxStateTensorWithCounterLow:counterHigh:key:name:")]
		MPSGraphTensor RandomPhiloxState(nuint counterLow, nuint counterHigh, nuint key, [NullAllowed] string name);
		[Export("randomPhiloxStateTensorWithSeed:name:")]
		MPSGraphTensor RandomPhiloxState(nuint seed, [NullAllowed] string name);
		[Export("randomTensorWithShape:descriptor:name:")]
		MPSGraphTensor Random(MPSShape shape, MPSGraphRandomOpDescriptor descriptor, [NullAllowed] string name);
		[Export("randomTensorWithShape:descriptor:seed:name:")]
		MPSGraphTensor Random(MPSShape shape, MPSGraphRandomOpDescriptor descriptor, nuint seed, [NullAllowed] string name);
		[Export("randomTensorWithShape:descriptor:stateTensor:name:")]
		MPSGraphTensor[] Random(MPSShape shape, MPSGraphRandomOpDescriptor descriptor, MPSGraphTensor stateTensor, [NullAllowed] string name);
		[Export("randomTensorWithShapeTensor:descriptor:name:")]
		MPSGraphTensor Random(MPSGraphTensor shapeTensor, MPSGraphRandomOpDescriptor descriptor, [NullAllowed] string name);
		[Export("randomTensorWithShapeTensor:descriptor:seed:name:")]
		MPSGraphTensor Random(MPSGraphTensor shapeTensor, MPSGraphRandomOpDescriptor descriptor, nuint seed, [NullAllowed] string name);
		[Export("randomTensorWithShapeTensor:descriptor:stateTensor:name:")]
		MPSGraphTensor[] Random(MPSGraphTensor shapeTensor, MPSGraphRandomOpDescriptor descriptor, MPSGraphTensor stateTensor, [NullAllowed] string name);
		[Export("randomUniformTensorWithShape:name:")]
		MPSGraphTensor RandomUniform(MPSShape shape, [NullAllowed] string name);
		[Export("randomUniformTensorWithShape:seed:name:")]
		MPSGraphTensor RandomUniform(MPSShape shape, nuint seed, [NullAllowed] string name);
		[Export("randomUniformTensorWithShape:stateTensor:name:")]
		MPSGraphTensor[] RandomUniform(MPSShape shape, MPSGraphTensor stateTensor, [NullAllowed] string name);
		[Export("randomUniformTensorWithShapeTensor:name:")]
		MPSGraphTensor RandomUniform(MPSGraphTensor shapeTensor, [NullAllowed] string name);
		[Export("randomUniformTensorWithShapeTensor:seed:name:")]
		MPSGraphTensor RandomUniform(MPSGraphTensor shapeTensor, nuint seed, [NullAllowed] string name);
		[Export("randomUniformTensorWithShapeTensor:stateTensor:name:")]
		MPSGraphTensor[] RandomUniform(MPSGraphTensor shapeTensor, MPSGraphTensor stateTensor, [NullAllowed] string name);
		[Export("reLUGradientWithIncomingGradient:sourceTensor:name:")]
		MPSGraphTensor ReLUGradient(MPSGraphTensor incomingGradient, MPSGraphTensor sourceTensor, [NullAllowed] string name);
		[Export("reLUWithTensor:name:")]
		MPSGraphTensor ReLU(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("readVariable:name:")]
		MPSGraphTensor Read(MPSGraphTensor readVariable, [NullAllowed] string name);
		[Export("reciprocalWithTensor:name:")]
		MPSGraphTensor Reciprocal(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("reductionArgMaximumWithTensor:axis:name:")]
		MPSGraphTensor ReductionArgMaximum(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("reductionArgMinimumWithTensor:axis:name:")]
		MPSGraphTensor ReductionArgMinimum(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("reductionMaximumPropagateNaNWithTensor:axes:name:")]
		MPSGraphTensor ReductionMaximumPropagateNaN(MPSGraphTensor tensor, [NullAllowed] NSNumber[] axes, [NullAllowed] string name);
		[Export("reductionMaximumPropagateNaNWithTensor:axis:name:")]
		MPSGraphTensor ReductionMaximumPropagateNaN(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("reductionMaximumWithTensor:axes:name:")]
		MPSGraphTensor ReductionMaximum(MPSGraphTensor tensor, [NullAllowed] NSNumber[] axes, [NullAllowed] string name);
		[Export("reductionMaximumWithTensor:axis:name:")]
		MPSGraphTensor ReductionMaximum(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("reductionMinimumPropagateNaNWithTensor:axes:name:")]
		MPSGraphTensor ReductionMinimumPropagateNaN(MPSGraphTensor tensor, [NullAllowed] NSNumber[] axes, [NullAllowed] string name);
		[Export("reductionMinimumPropagateNaNWithTensor:axis:name:")]
		MPSGraphTensor ReductionMinimumPropagateNaN(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("reductionMinimumWithTensor:axes:name:")]
		MPSGraphTensor ReductionMinimum(MPSGraphTensor tensor, [NullAllowed] NSNumber[] axes, [NullAllowed] string name);
		[Export("reductionMinimumWithTensor:axis:name:")]
		MPSGraphTensor ReductionMinimum(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("reductionProductWithTensor:axes:name:")]
		MPSGraphTensor ReductionProduct(MPSGraphTensor tensor, [NullAllowed] NSNumber[] axes, [NullAllowed] string name);
		[Export("reductionProductWithTensor:axis:name:")]
		MPSGraphTensor ReductionProduct(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("reductionSumWithTensor:axes:name:")]
		MPSGraphTensor ReductionSum(MPSGraphTensor tensor, [NullAllowed] NSNumber[] axes, [NullAllowed] string name);
		[Export("reductionSumWithTensor:axis:name:")]
		MPSGraphTensor ReductionSum(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("reshapeTensor:withShape:name:")]
		MPSGraphTensor Reshape(MPSGraphTensor tensor, MPSShape withShape, [NullAllowed] string name);
		[Export("reshapeTensor:withShapeTensor:name:")]
		MPSGraphTensor Reshape(MPSGraphTensor tensor, MPSGraphTensor withShapeTensor, [NullAllowed] string name);
		[Export("resizeTensor:size:mode:centerResult:alignCorners:layout:name:")]
		MPSGraphTensor Resize(MPSGraphTensor tensor, MPSShape size, MPSGraphResizeMode mode, bool centerResult, bool alignCorners, MPSGraphTensorNamedDataLayout layout, [NullAllowed] string name);
		[Export("resizeTensor:sizeTensor:mode:centerResult:alignCorners:layout:name:")]
		MPSGraphTensor Resize(MPSGraphTensor tensor, MPSGraphTensor sizeTensor, MPSGraphResizeMode mode, bool centerResult, bool alignCorners, MPSGraphTensorNamedDataLayout layout, [NullAllowed] string name);
		[Export("resizeWithGradientTensor:input:mode:centerResult:alignCorners:layout:name:")]
		MPSGraphTensor Resize(MPSGraphTensor gradientTensor, MPSGraphTensor input, MPSGraphResizeMode mode, bool centerResult, bool alignCorners, MPSGraphTensorNamedDataLayout layout, [NullAllowed] string name);
		[Export("reverseSquareRootWithTensor:name:")]
		MPSGraphTensor ReverseSquareRoot(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("reverseTensor:axes:name:")]
		MPSGraphTensor Reverse(MPSGraphTensor tensor, NSNumber[] axes, [NullAllowed] string name);
		[Export("reverseTensor:axesTensor:name:")]
		MPSGraphTensor Reverse(MPSGraphTensor tensor, MPSGraphTensor axesTensor, [NullAllowed] string name);
		[Export("reverseTensor:name:")]
		MPSGraphTensor Reverse(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("rintWithTensor:name:")]
		MPSGraphTensor Rint(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("roundWithTensor:name:")]
		MPSGraphTensor Round(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("scatterNDWithDataTensor:updatesTensor:indicesTensor:batchDimensions:mode:name:")]
		MPSGraphTensor ScatterNDWithData(MPSGraphTensor dataTensor, MPSGraphTensor updatesTensor, MPSGraphTensor indicesTensor, nuint batchDimensions, MPSGraphScatterMode mode, [NullAllowed] string name);
		[Export("scatterNDWithUpdatesTensor:indicesTensor:shape:batchDimensions:mode:name:")]
		MPSGraphTensor ScatterND(MPSGraphTensor updatesTensor, MPSGraphTensor indicesTensor, MPSShape shape, nuint batchDimensions, MPSGraphScatterMode mode, [NullAllowed] string name);
		[Export("scatterNDWithUpdatesTensor:indicesTensor:shape:batchDimensions:name:")]
		MPSGraphTensor ScatterND(MPSGraphTensor updatesTensor, MPSGraphTensor indicesTensor, MPSShape shape, nuint batchDimensions, [NullAllowed] string name);
		[Export("scatterWithDataTensor:updatesTensor:indicesTensor:axis:mode:name:")]
		MPSGraphTensor ScatterWithData(MPSGraphTensor dataTensor, MPSGraphTensor updatesTensor, MPSGraphTensor indicesTensor, nint axis, MPSGraphScatterMode mode, [NullAllowed] string name);
		[Export("scatterWithUpdatesTensor:indicesTensor:shape:axis:mode:name:")]
		MPSGraphTensor Scatter(MPSGraphTensor updatesTensor, MPSGraphTensor indicesTensor, MPSShape shape, nint axis, MPSGraphScatterMode mode, [NullAllowed] string name);
		[Export("selectWithPredicateTensor:truePredicateTensor:falsePredicateTensor:name:")]
		MPSGraphTensor Select(MPSGraphTensor predicateTensor, MPSGraphTensor truePredicateTensor, MPSGraphTensor falsePredicateTensor, [NullAllowed] string name);
		[Export("setExplicitPaddingWithPaddingLeft:paddingRight:paddingTop:paddingBottom:")]
		void SetExplicitPadding(nuint paddingLeft, nuint paddingRight, nuint paddingTop, nuint paddingBottom);
		[Export("setExplicitPaddingWithPaddingLeft:paddingRight:paddingTop:paddingBottom:")]
		void SetExplicitPadding(nuint paddingLeft, nuint paddingRight, nuint paddingTop, nuint paddingBottom);
		[Export("setExplicitPaddingWithPaddingLeft:paddingRight:paddingTop:paddingBottom:")]
		void SetExplicitPadding(nuint paddingLeft, nuint paddingRight, nuint paddingTop, nuint paddingBottom);
		[Export("shapeOfTensor:name:")]
		MPSGraphTensor ShapeOf(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("sigmoidGradientWithIncomingGradient:sourceTensor:name:")]
		MPSGraphTensor SigmoidGradient(MPSGraphTensor incomingGradient, MPSGraphTensor sourceTensor, [NullAllowed] string name);
		[Export("sigmoidWithTensor:name:")]
		MPSGraphTensor Sigmoid(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("signWithTensor:name:")]
		MPSGraphTensor Sign(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("signbitWithTensor:name:")]
		MPSGraphTensor Signbit(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("sinWithTensor:name:")]
		MPSGraphTensor Sin(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("sinhWithTensor:name:")]
		MPSGraphTensor Sinh(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("sliceGradientTensor:fwdInShapeTensor:starts:ends:strides:name:")]
		MPSGraphTensor SliceGradient(MPSGraphTensor tensor, MPSGraphTensor fwdInShapeTensor, NSNumber[] starts, NSNumber[] ends, NSNumber[] strides, [NullAllowed] string name);
		[Export("sliceGradientTensor:fwdInShapeTensor:starts:ends:strides:startMask:endMask:squeezeMask:name:")]
		MPSGraphTensor SliceGradient(MPSGraphTensor tensor, MPSGraphTensor fwdInShapeTensor, NSNumber[] starts, NSNumber[] ends, NSNumber[] strides, uint startMask, uint endMask, uint squeezeMask, [NullAllowed] string name);
		[Export("sliceTensor:dimension:start:length:name:")]
		MPSGraphTensor Slice(MPSGraphTensor tensor, nuint dimension, nint start, nint length, [NullAllowed] string name);
		[Export("sliceTensor:starts:ends:strides:name:")]
		MPSGraphTensor Slice(MPSGraphTensor tensor, NSNumber[] starts, NSNumber[] ends, NSNumber[] strides, [NullAllowed] string name);
		[Export("sliceTensor:starts:ends:strides:startMask:endMask:squeezeMask:name:")]
		MPSGraphTensor Slice(MPSGraphTensor tensor, NSNumber[] starts, NSNumber[] ends, NSNumber[] strides, uint startMask, uint endMask, uint squeezeMask, [NullAllowed] string name);
		[Export("softMaxCrossEntropyGradientWithIncomingGradientTensor:sourceTensor:labelsTensor:axis:reductionType:name:")]
		MPSGraphTensor SoftMaxCrossEntropyGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSGraphTensor labelsTensor, nint axis, MPSGraphLossReductionType reductionType, [NullAllowed] string name);
		[Export("softMaxCrossEntropyWithSourceTensor:labelsTensor:axis:reductionType:name:")]
		MPSGraphTensor SoftMaxCrossEntropy(MPSGraphTensor sourceTensor, MPSGraphTensor labelsTensor, nint axis, MPSGraphLossReductionType reductionType, [NullAllowed] string name);
		[Export("softMaxGradientWithIncomingGradient:sourceTensor:axis:name:")]
		MPSGraphTensor SoftMaxGradient(MPSGraphTensor incomingGradient, MPSGraphTensor sourceTensor, nint axis, [NullAllowed] string name);
		[Export("softMaxWithTensor:axis:name:")]
		MPSGraphTensor SoftMax(MPSGraphTensor tensor, nint axis, [NullAllowed] string name);
		[Export("spaceToDepth2DTensor:widthAxis:heightAxis:depthAxis:blockSize:usePixelShuffleOrder:name:")]
		MPSGraphTensor SpaceToDepth2D(MPSGraphTensor tensor, nuint widthAxis, nuint heightAxis, nuint depthAxis, nuint blockSize, bool usePixelShuffleOrder, [NullAllowed] string name);
		[Export("spaceToDepth2DTensor:widthAxisTensor:heightAxisTensor:depthAxisTensor:blockSize:usePixelShuffleOrder:name:")]
		MPSGraphTensor SpaceToDepth2D(MPSGraphTensor tensor, MPSGraphTensor widthAxisTensor, MPSGraphTensor heightAxisTensor, MPSGraphTensor depthAxisTensor, nuint blockSize, bool usePixelShuffleOrder, [NullAllowed] string name);
		[Export("squareRootWithTensor:name:")]
		MPSGraphTensor SquareRoot(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("squareWithTensor:name:")]
		MPSGraphTensor Square(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("stencilWithSourceTensor:weightsTensor:descriptor:name:")]
		MPSGraphTensor Stencil(MPSGraphTensor sourceTensor, MPSGraphTensor weightsTensor, MPSGraphStencilOpDescriptor descriptor, [NullAllowed] string name);
		[Export("stochasticGradientDescentWithLearningRateTensor:valuesTensor:gradientTensor:name:")]
		MPSGraphTensor StochasticGradientDescent(MPSGraphTensor learningRateTensor, MPSGraphTensor valuesTensor, MPSGraphTensor gradientTensor, [NullAllowed] string name);
		[Export("subtractionWithPrimaryTensor:secondaryTensor:name:")]
		MPSGraphTensor Subtraction(MPSGraphTensor primaryTensor, MPSGraphTensor secondaryTensor, [NullAllowed] string name);
		[Export("tanWithTensor:name:")]
		MPSGraphTensor Tan(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("tanhWithTensor:name:")]
		MPSGraphTensor Tanh(MPSGraphTensor tensor, [NullAllowed] string name);
		[Export("tileGradientWithIncomingGradientTensor:sourceTensor:withMultiplier:name:")]
		MPSGraphTensor TileGradient(MPSGraphTensor incomingGradientTensor, MPSGraphTensor sourceTensor, MPSShape withMultiplier, [NullAllowed] string name);
		[Export("tileTensor:withMultiplier:name:")]
		MPSGraphTensor Tile(MPSGraphTensor tensor, MPSShape withMultiplier, [NullAllowed] string name);
		[Export("topKWithGradientTensor:source:k:name:")]
		MPSGraphTensor TopKGradient(MPSGraphTensor gradientTensor, MPSGraphTensor source, nuint k, [NullAllowed] string name);
		[Export("topKWithGradientTensor:source:kTensor:name:")]
		MPSGraphTensor TopKGradient(MPSGraphTensor gradientTensor, MPSGraphTensor source, MPSGraphTensor kTensor, [NullAllowed] string name);
		[Export("topKWithSourceTensor:k:name:")]
		MPSGraphTensor[] TopK(MPSGraphTensor sourceTensor, nuint k, [NullAllowed] string name);
		[Export("topKWithSourceTensor:kTensor:name:")]
		MPSGraphTensor[] TopK(MPSGraphTensor sourceTensor, MPSGraphTensor kTensor, [NullAllowed] string name);
		[Export("transposeTensor:dimension:withDimension:name:")]
		MPSGraphTensor Transpose(MPSGraphTensor tensor, nuint dimension, nuint withDimension, [NullAllowed] string name);
		[Export("variableWithData:shape:dataType:name:")]
		MPSGraphTensor Variable(NSData data, MPSShape shape, MPSDataType dataType, [NullAllowed] string name);
		[Export("varianceOfTensor:axes:name:")]
		MPSGraphTensor Variance(MPSGraphTensor tensor, NSNumber[] axes, [NullAllowed] string name);
		[Export("varianceOfTensor:meanTensor:axes:name:")]
		MPSGraphTensor Variance(MPSGraphTensor tensor, MPSGraphTensor meanTensor, NSNumber[] axes, [NullAllowed] string name);
		[Export("whileWithInitialInputs:before:after:name:")]
		MPSGraphTensor[] While(MPSGraphTensor[] initialInputs, MPSGraphWhileBeforeBlock before, MPSGraphWhileAfterBlock after, [NullAllowed] string name);
*/
	}

	// @interface MPSGraphCompilationDescriptor : NSObject
	[TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof(NSObject))]
	interface MPSGraphCompilationDescriptor
	{
		// -(void)disableTypeInference;
		[Export ("disableTypeInference")]
		void DisableTypeInference ();
	}

	// @interface MPSGraphDevice : NSObject
	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof(NSObject))]
	interface MPSGraphDevice
	{
		// @property (readonly, nonatomic) MPSGraphDeviceType type;
		[Export ("type")]
		MPSGraphDeviceType Type { get; }

		// @property (readonly, nonatomic) id<MTLDevice> _Nullable metalDevice;
		[NullAllowed, Export ("metalDevice")]
		IMTLDevice MetalDevice { get; }

		// +(instancetype _Nonnull)deviceWithMTLDevice:(id<MTLDevice> _Nonnull)metalDevice;
		[Static]
		[Export ("deviceWithMTLDevice:")]
		MPSGraphDevice FromMTLDevice (IMTLDevice metalDevice);
	}

	// typedef void (^MPSGraphExecutableCompletionHandler)(NSArray<MPSGraphTensorData *> * _Nonnull, NSError * _Nullable);
	delegate void MPSGraphExecutableCompletionHandler (MPSGraphTensorData[] arg0, [NullAllowed] NSError arg1);

	// typedef void (^MPSGraphExecutableScheduledHandler)(NSArray<MPSGraphTensorData *> * _Nonnull, NSError * _Nullable);
	delegate void MPSGraphExecutableScheduledHandler (MPSGraphTensorData[] arg0, [NullAllowed] NSError arg1);

	// @interface MPSGraphExecutableExecutionDescriptor : NSObject
	[TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof(NSObject))]
	interface MPSGraphExecutableExecutionDescriptor
	{
		// @property (readwrite, atomic) MPSGraphExecutableScheduledHandler _Nonnull scheduledHandler;
		[Export ("scheduledHandler", ArgumentSemantic.Assign)]
		MPSGraphExecutableScheduledHandler ScheduledHandler { get; set; }

		// @property (readwrite, atomic) MPSGraphExecutableCompletionHandler _Nonnull completionHandler;
		[Export ("completionHandler", ArgumentSemantic.Assign)]
		MPSGraphExecutableCompletionHandler CompletionHandler { get; set; }

		// @property (readwrite, atomic) BOOL waitUntilCompleted;
		[Export ("waitUntilCompleted")]
		bool WaitUntilCompleted { get; set; }
	}

	// @interface MPSGraphExecutable : NSObject
	[TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof(NSObject))]
	interface MPSGraphExecutable
	{
		// @property (readwrite, atomic) MPSGraphOptions options;
		[Export ("options", ArgumentSemantic.Assign)]
		MPSGraphOptions Options { get; set; }

		// @property (readonly, atomic) NSArray<MPSGraphTensor *> * _Nullable feedTensors;
		[NullAllowed, Export ("feedTensors")]
		MPSGraphTensor[] FeedTensors { get; }

		// @property (readonly, atomic) NSArray<MPSGraphTensor *> * _Nullable targetTensors;
		[NullAllowed, Export ("targetTensors")]
		MPSGraphTensor[] TargetTensors { get; }

		// -(void)specializeWithDevice:(MPSGraphDevice * _Nullable)device inputTypes:(NSArray<MPSGraphType *> * _Nonnull)inputTypes compilationDescriptor:(MPSGraphCompilationDescriptor * _Nullable)compilationDescriptor;
		[Export ("specializeWithDevice:inputTypes:compilationDescriptor:")]
		void Specialize ([NullAllowed] MPSGraphDevice device, MPSGraphType[] inputTypes, [NullAllowed] MPSGraphCompilationDescriptor compilationDescriptor);

		// -(NSArray<MPSGraphTensorData *> * _Nonnull)runWithMTLCommandQueue:(id<IMTLCommandQueue> _Nonnull)commandQueue inputsArray:(NSArray<MPSGraphTensorData *> * _Nonnull)inputsArray resultsArray:(NSArray<MPSGraphTensorData *> * _Nullable)resultsArray executionDescriptor:(MPSGraphExecutableExecutionDescriptor * _Nullable)executionDescriptor __attribute__((swift_name("run(with:inputs:results:executionDescriptor:)")));
		[Export ("runWithMTLCommandQueue:inputsArray:resultsArray:executionDescriptor:")]
		MPSGraphTensorData[] Run (IMTLCommandQueue commandQueue, MPSGraphTensorData[] inputsArray, [NullAllowed] MPSGraphTensorData[] resultsArray, [NullAllowed] MPSGraphExecutableExecutionDescriptor executionDescriptor);

		// -(NSArray<MPSGraphTensorData *> * _Nonnull)runAsyncWithMTLCommandQueue:(id<IMTLCommandQueue> _Nonnull)commandQueue inputsArray:(NSArray<MPSGraphTensorData *> * _Nonnull)inputsArray resultsArray:(NSArray<MPSGraphTensorData *> * _Nullable)resultsArray executionDescriptor:(MPSGraphExecutableExecutionDescriptor * _Nullable)executionDescriptor __attribute__((swift_name("runAsync(with:inputs:results:executionDescriptor:)")));
		[Export ("runAsyncWithMTLCommandQueue:inputsArray:resultsArray:executionDescriptor:")]
		MPSGraphTensorData[] RunAsync (IMTLCommandQueue commandQueue, MPSGraphTensorData[] inputsArray, [NullAllowed] MPSGraphTensorData[] resultsArray, [NullAllowed] MPSGraphExecutableExecutionDescriptor executionDescriptor);

		// -(NSArray<MPSGraphTensorData *> * _Nonnull)encodeToCommandBuffer:(MPSCommandBuffer * _Nonnull)commandBuffer inputsArray:(NSArray<MPSGraphTensorData *> * _Nonnull)inputsArray resultsArray:(NSArray<MPSGraphTensorData *> * _Nullable)resultsArray executionDescriptor:(MPSGraphExecutableExecutionDescriptor * _Nullable)executionDescriptor __attribute__((swift_name("encode(to:inputs:results:executionDescriptor:)")));
		[Export ("encodeToCommandBuffer:inputsArray:resultsArray:executionDescriptor:")]
		MPSGraphTensorData[] Encode (MPSCommandBuffer commandBuffer, MPSGraphTensorData[] inputsArray, [NullAllowed] MPSGraphTensorData[] resultsArray, [NullAllowed] MPSGraphExecutableExecutionDescriptor executionDescriptor);
	}

	// typedef void (^MPSGraphCompletionHandler)(MPSGraphTensorDataDictionary * _Nonnull, NSError * _Nullable);
	delegate void MPSGraphCompletionHandler (MPSGraphTensorDataDictionary resultsDictionary, [NullAllowed] NSError error);

	// typedef void (^MPSGraphScheduledHandler)(MPSGraphTensorDataDictionary * _Nonnull, NSError * _Nullable);
	delegate void MPSGraphScheduledHandler (MPSGraphTensorDataDictionary resultsDictionary, [NullAllowed] NSError error);

	// @interface MPSGraphExecutionDescriptor : NSObject
	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	
	[BaseType (typeof(NSObject))]
	interface MPSGraphExecutionDescriptor
	{
		// @property (readwrite, atomic) MPSGraphScheduledHandler _Nonnull scheduledHandler;
		[Export ("scheduledHandler", ArgumentSemantic.Assign)]
		MPSGraphScheduledHandler ScheduledHandler { get; set; }

		// @property (readwrite, atomic) MPSGraphCompletionHandler _Nonnull completionHandler;
		[Export ("completionHandler", ArgumentSemantic.Assign)]
		MPSGraphCompletionHandler CompletionHandler { get; set; }

		// @property (readwrite, atomic) BOOL waitUntilCompleted;
		[Export ("waitUntilCompleted")]
		bool WaitUntilCompleted { get; set; }
	}

	// MPSGraphCore.h
	
	// @interface MPSGraphShapedType : MPSGraphType
	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof(MPSGraphType))]
	interface MPSGraphShapedType
	{
		// @property (readwrite, copy, atomic) MPSShape * _Nullable shape;
		[NullAllowed, Export ("shape", ArgumentSemantic.Copy)]
		NSNumber[] Shape { get; set; }

		// @property (readwrite, atomic) MPSDataType dataType;
		[Export ("dataType", ArgumentSemantic.Assign)]
		MPSDataType DataType { get; set; }

		// -(instancetype _Nonnull)initWithShape:(MPSShape * _Nullable)shape dataType:(MPSDataType)dataType;
		[Export ("initWithShape:dataType:")]
		IntPtr Constructor ([NullAllowed] NSNumber[] shape, MPSDataType dataType);

		// -(BOOL)isEqualTo:(MPSGraphShapedType * _Nullable)object;
		[Export ("isEqualTo:")]
		bool IsEqualTo ([NullAllowed] MPSGraphShapedType @object);
	}

	// MPSGraphType was introduced in iOS 15 (macOS 12) and became the base class for MPSGraphShapedType.
	// Prior to that, MPSGraphShapedType inherited from NSObject directly.
	// @interface MPSGraphType : NSObject
	[TV (15,0), Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof(NSObject))]
	interface MPSGraphType : NSCopying
	{
	}

	// MPSGraphOperation.h

	// @interface MPSGraphOperation : NSObject <NSCopying>
	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphOperation : NSCopying
	{
		// @property (readonly, nonatomic) NSArray<MPSGraphTensor *> * _Nonnull inputTensors;
		[Export ("inputTensors")]
		MPSGraphTensor[] InputTensors { get; }

		// @property (readonly, nonatomic) NSArray<MPSGraphTensor *> * _Nonnull outputTensors;
		[Export ("outputTensors")]
		MPSGraphTensor[] OutputTensors { get; }

		// @property (readonly, nonatomic) NSArray<MPSGraphOperation *> * _Nonnull controlDependencies;
		[Export ("controlDependencies")]
		MPSGraphOperation[] ControlDependencies { get; }

		// @property (readonly, nonatomic) MPSGraph * _Nonnull graph;
		[Export ("graph")]
		MPSGraph Graph { get; }

		// @property (readonly, nonatomic) NSString * _Nonnull name;
		[Export ("name")]
		string Name { get; }
	}

	// MPSGraphTensor.h

	// @interface MPSGraphTensor : NSObject <NSCopying>
	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MPSGraphTensor : NSCopying
	{
		// @property (readonly, copy, nonatomic) MPSShape * _Nullable shape;
		[NullAllowed, Export ("shape", ArgumentSemantic.Copy)]
		NSNumber[] Shape { get; }

		// @property (readonly, nonatomic) MPSDataType dataType;
		[Export ("dataType")]
		MPSDataType DataType { get; }

		// @property (readonly, nonatomic) MPSGraphOperation * _Nonnull operation;
		[Export ("operation")]
		MPSGraphOperation Operation { get; }
	}

	// @interface MPSGraphTensorData : NSObject
	[iOS (14,0), TV (14,0), Mac (11,0), MacCatalyst (14,0)]
	[BaseType (typeof(NSObject))]
	interface MPSGraphTensorData
	{
		// @property (readonly, copy, nonatomic) MPSShape * _Nonnull shape;
		[Export ("shape", ArgumentSemantic.Copy)]
		NSNumber[] Shape { get; }

		// @property (readonly, nonatomic) MPSDataType dataType;
		[Export ("dataType")]
		MPSDataType DataType { get; }

		// @property (readonly, nonatomic) MPSGraphDevice * _Nonnull device;
		[Export ("device")]
		MPSGraphDevice Device { get; }

		// -(instancetype _Nonnull)initWithDevice:(MPSGraphDevice * _Nonnull)device data:(NSData * _Nonnull)data shape:(MPSShape * _Nonnull)shape dataType:(MPSDataType)dataType;
		[Export ("initWithDevice:data:shape:dataType:")]
		IntPtr Constructor (MPSGraphDevice device, NSData data, NSNumber[] shape, MPSDataType dataType);

		// -(instancetype _Nonnull)initWithMTLBuffer:(id<IMTLBuffer> _Nonnull)buffer shape:(MPSShape * _Nonnull)shape dataType:(MPSDataType)dataType __attribute__((swift_name("init(_:shape:dataType:)")));
		[Export ("initWithMTLBuffer:shape:dataType:")]
		IntPtr Constructor (IMTLBuffer buffer, NSNumber[] shape, MPSDataType dataType);

		// -(instancetype _Nonnull)initWithMPSMatrix:(MPSMatrix * _Nonnull)matrix __attribute__((swift_name("init(_:)")));
		[Export ("initWithMPSMatrix:")]
		IntPtr Constructor (MPSMatrix matrix);

		// -(instancetype _Nonnull)initWithMPSMatrix:(MPSMatrix * _Nonnull)matrix rank:(NSUInteger)rank __attribute__((swift_name("init(_:rank:)")));
		[Export ("initWithMPSMatrix:rank:")]
		IntPtr Constructor (MPSMatrix matrix, nuint rank);

		// -(instancetype _Nonnull)initWithMPSVector:(MPSVector * _Nonnull)vector __attribute__((swift_name("init(_:)")));
		[Export ("initWithMPSVector:")]
		IntPtr Constructor (MPSVector vector);

		// -(instancetype _Nonnull)initWithMPSVector:(MPSVector * _Nonnull)vector rank:(NSUInteger)rank __attribute__((swift_name("init(_:rank:)")));
		[Export ("initWithMPSVector:rank:")]
		IntPtr Constructor (MPSVector vector, nuint rank);

		// -(instancetype _Nonnull)initWithMPSNDArray:(MPSNDArray * _Nonnull)ndarray __attribute__((swift_name("init(_:)")));
		[Export ("initWithMPSNDArray:")]
		IntPtr Constructor (MPSNDArray ndarray);

		// -(instancetype _Nonnull)initWithMPSImageBatch:(MPSImageBatch * _Nonnull)imageBatch __attribute__((swift_name("init(_:)")));
		[Export ("initWithMPSImageBatch:")]
		IntPtr Constructor (MPSImage[] imageBatch);

		// -(MPSNDArray * _Nonnull)mpsndarray;
		[Export ("mpsndarray")]
		MPSNDArray MPSNDArray { get; }
	}


}
