#nullable enable

#if HAS_METALPERFORMANCESHADERSGRAPH
// MetalPerformanceShadersGraph is not available in the tvOS simulator (the code from the static registrar won't compile)
// Unfortunately we don't have a define that tells us we're building for the simulator, so exclude all of tvOS.
#if !__TVOS__
#define ENABLE_MNIST_TESTER
#endif
#endif

#if ENABLE_MNIST_TESTER

using System;
using System.Collections.Generic;
using System.Threading;

using Foundation;
using Metal;
using MetalPerformanceShaders;
using MetalPerformanceShadersGraph;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShadersGraph {
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class MnistTester {

		[OneTimeSetUp]
		public void IsSupported ()
		{
			TestRuntime.AssertNotVirtualMachine ();
			TestRuntime.AssertXcodeVersion (13, 0); // There are known bugs in early versions of MetalPerformanceShadersGraph that makes this test crash, so require macOS 12+.
#if __TVOS__
			if (Runtime.Arch == Arch.SIMULATOR)
				Assert.Inconclusive ("Metal Performance Shaders Graph is not supported in the tvOS simulator");
#endif
#if __IOS__ && !__MACCATALYST__

			if (TestRuntime.CheckXcodeVersion (14, 0))
				TestRuntime.AssertNotSimulator ("Fails with 'Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[MTLSimHeap protectionOptions]: unrecognized selector sent to instance 0x600002a09090' - note that we don't call this selector.");
#endif
			TestRuntime.IgnoreInCI ("This test seems to make bots keel over and die.");
			TestRuntime.AssertNotX64Desktop (); // Intel Mac is not fast enough.

			var device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device is null) {
				Assert.Inconclusive ($"Metal does not exist on this device.");
			} else if (!MPSKernel.Supports (device))
				Assert.Inconclusive ($"Metal is not supported on this device: {device.Name}");
		}

		[Test]
		public void Run ()
		{
			using (var tester = new MnistTest ()) {
				tester.Run ();
			}
		}
	}

	public class MnistTest : IDisposable {
		const int batchSize = 3;

		readonly int numTrainingIterations = 5;

		readonly IMTLCommandQueue commandQueue = MTLDevice.SystemDefault!.CreateCommandQueue ()!;

		readonly MnistGraph graph = new (batchSize);

		readonly MnistData data = new ();

		public void Run ()
		{
			MPSCommandBuffer? latestCommandBuffer = null;
			for (var i = 0; i < numTrainingIterations; i++) {
				latestCommandBuffer = RunTrainingIterationBatch ((i + 1) / (float) numTrainingIterations);
			}
		}

		MPSCommandBuffer RunTrainingIterationBatch (float progress)
		{
			var commandBuffer = MPSCommandBuffer.Create (commandQueue);

			var xInput = data.GetRandomTrainingBatch (commandQueue.Device, batchSize, out var yLabels);
			var completed = false;
			graph.EncodeTrainingBatch (commandBuffer, xInput, yLabels, loss => {
				commandBuffer.Dispose ();
				completed = true;
			});

			Assert.IsTrue (TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
			}, () => completed), "Completion");

			// Don't need to commit since EncodeTrainingBatch oddly does that
			return commandBuffer;
		}

		void IDisposable.Dispose ()
		{
			graph.Dispose ();
		}
	}

	public class MnistData {
		public const int ImageSize = 28;
		public const int NumClasses = 10;

		readonly Random random = new Random ();

		const int ImageMetadataPrefixSize = 16;

		readonly int totalNumberOfTrainImages;
		readonly byte [] dataTrainLabel;
		readonly byte [] dataTrainImage;

		public MnistData ()
		{
			dataTrainImage = new byte [ImageMetadataPrefixSize + ImageSize * ImageSize];
			dataTrainLabel = new byte [ImageMetadataPrefixSize + 1];

			totalNumberOfTrainImages = dataTrainLabel.Length - ImageMetadataPrefixSize;
		}

		public MPSGraphTensorData GetRandomTrainingBatch (IMTLDevice device, int batchSize, out MPSGraphTensorData labels)
		{
			var inputVals = new float [batchSize * ImageSize * ImageSize];
			var labelVals = new float [batchSize * NumClasses];

			for (var batchInd = 0; batchInd < batchSize; batchInd++) {
				var randomImageIdx = random.Next (totalNumberOfTrainImages);

				var valueOffset = ImageMetadataPrefixSize + randomImageIdx * ImageSize * ImageSize;
				for (var ind = 0; ind < ImageSize * ImageSize; ind++) {
					inputVals [batchInd * ImageSize * ImageSize + ind] = dataTrainImage [valueOffset + ind] / 255.0f;
				}

				var labelOffset = ImageMetadataPrefixSize + randomImageIdx;
				for (int classIdx = 0; classIdx < NumClasses; classIdx++) {
					labelVals [batchInd * NumClasses + classIdx] = classIdx == dataTrainLabel [labelOffset] ? 1.0f : 0.0f;
				}
			}

			labels = MPSGraphTensorData.Create (device, labelVals, batchSize, NumClasses);
			return MPSGraphTensorData.Create (device, inputVals, batchSize, ImageSize * ImageSize);
		}
	}

	public class MnistGraph : MPSGraph {
		const float lambda = 0.01f;

		readonly int imageSize;
		readonly int numClasses;

		readonly MPSGraphConvolution2DOpDescriptor convDesc = MPSGraphConvolution2DOpDescriptor.Create (
			strideInX: 1,
			strideInY: 1,
			dilationRateInX: 1,
			dilationRateInY: 1,
			groups: 1,
			paddingStyle: MPSGraphPaddingStyle.Same,
			dataLayout: MPSGraphTensorNamedDataLayout.Nhwc,
			weightsLayout: MPSGraphTensorNamedDataLayout.Hwio)!;

		readonly MPSGraphPooling2DOpDescriptor poolDesc = MPSGraphPooling2DOpDescriptor.Create (
			kernelWidth: 2,
			kernelHeight: 2,
			strideInX: 2,
			strideInY: 2,
			paddingStyle: MPSGraphPaddingStyle.Same,
			dataLayout: MPSGraphTensorNamedDataLayout.Nhwc)!;

		readonly MPSGraphTensor [] trainingTensors;
		readonly MPSGraphOperation [] trainingOps;

		readonly MPSGraphTensor sourcePlaceholder;
		readonly MPSGraphTensor labelsPlaceholder;

		readonly Random random = new Random ();
		readonly Semaphore doubleBufferSemaphore = new Semaphore (2, 2);

		public MnistGraph (int batchSize)
		{
			this.imageSize = MnistData.ImageSize;
			this.numClasses = MnistData.NumClasses;

			Options = MPSGraphOptions.SynchronizeResults;// | MPSGraphOptions.Verbose;

			Console.WriteLine (Options);

			sourcePlaceholder = this.Placeholder (new [] { batchSize, imageSize * imageSize }, null);
			labelsPlaceholder = this.Placeholder (new [] { batchSize, numClasses }, null);

			var variables = new List<MPSGraphTensor> ();
			var reshapedInput = this.Reshape (sourcePlaceholder, shape: new [] { batchSize, imageSize, imageSize, 1 }, null);

			var conv0 = AddConvLayer (reshapedInput, weightsShape: new int [4] { 5, 5, 1, 32 }, convDesc, variables);
			var pool0 = this.MaxPooling2D (conv0, poolDesc, null);

			var conv1Tensor = AddConvLayer (pool0, weightsShape: new int [4] { 5, 5, 32, 64 }, convDesc, variables);
			var pool1Tensor = this.MaxPooling2D (conv1Tensor, poolDesc, null);

			var reshape = this.Reshape (pool1Tensor, new [] { -1, 64 * 7 * 7 }, null);

			var fc0 = AddFullyConnectedLayer (reshape, weightsShape: new int [2] { 7 * 7 * 64, 1024 }, hasActivation: true, variables);
			var fc1 = AddFullyConnectedLayer (fc0, weightsShape: new int [2] { 1024, numClasses }, hasActivation: false, variables);

			var softmax = this.SoftMax (fc1, axis: -1, null);
			var loss = this.SoftMaxCrossEntropy (fc1, labels: labelsPlaceholder, axis: -1, MPSGraphLossReductionType.Sum, null);

			var batchSizeT = this.Constant ((float) batchSize);
			var lossMean = this.Division (loss, batchSizeT, null);

			trainingTensors = new [] { lossMean };
			trainingOps = GetAssignOperations (lossMean, variables);
		}

		MPSGraphOperation [] GetAssignOperations (MPSGraphTensor loss, List<MPSGraphTensor> variables)
		{
			var grads = this.Gradients (loss, variables.ToArray (), null);
			var lambdaT = this.Constant (lambda);
			var updateOps = new List<MPSGraphOperation> ();
			foreach (var (k, value) in grads) {
				var key = (MPSGraphTensor) k;
				var update = this.StochasticGradientDescent (lambdaT, key, (MPSGraphTensor) value, null);
				var assign = this.Assign (key, update, null);
				updateOps.Add (assign);
			}
			return updateOps.ToArray ();
		}

		MPSGraphTensor AddFullyConnectedLayer (MPSGraphTensor source, int [] weightsShape, bool hasActivation, List<MPSGraphTensor> variables)
		{
			var weightCount = 1;
			foreach (var length in weightsShape) {
				weightCount *= length;
			}
			var biasCount = weightsShape [1];

			var weightsValues = GetRandomData (weightCount, -0.2f, 0.2f);
			var biasesValues = new float [biasCount];
			Array.Fill (biasesValues, 0.1f);

			var weights = this.Variable (weightsValues, weightsShape);
			var biases = this.Variable (biasesValues, new [] { biasCount });

			var fc = this.MatrixMultiplication (source, weights, null);
			var fcBias = this.Addition (fc, biases, null);

			variables.Add (weights);
			variables.Add (biases);

			if (!hasActivation)
				return fcBias;

			var activation = this.ReLU (fcBias, null);
			return activation;
		}

		MPSGraphTensor AddConvLayer (MPSGraphTensor source, int [] weightsShape, MPSGraphConvolution2DOpDescriptor desc, List<MPSGraphTensor> variables)
		{
			var weightCount = 1;
			foreach (var length in weightsShape) {
				weightCount *= length;
			}
			var biasCount = weightsShape [3];

			var convWeightsValues = GetRandomData (weightCount, -0.2f, 0.2f);
			var weights = this.Variable (convWeightsValues, weightsShape);

			var biases = this.Variable (0.1f, new [] { biasCount });

			var conv = this.Convolution2D (source, weights, desc, null);
			var convBias = this.Addition (conv, biases, null);
			var activation = this.ReLU (convBias, null);

			variables.Add (weights);
			variables.Add (biases);

			return activation;
		}

		float [] GetRandomData (int length, float min, float max)
		{
			var d = max - min;
			var r = new float [length];
			for (var i = 0; i < length; i++) {
				r [i] = ((float) random.NextDouble () * d) + min;
			}
			return r;
		}

		public MPSGraphTensorData EncodeTrainingBatch (MPSCommandBuffer commandBuffer, MPSGraphTensorData sourceTensorData, MPSGraphTensorData labelsTensorData, Action<float>? completion)
		{
			doubleBufferSemaphore.WaitOne ();

			var executionDesc = new MPSGraphExecutionDescriptor {
				CompletionHandler = (results, error) => {
					// This is necessary because there's a weird synchronization issue with
					// this callback. I have requested support from Apple about it.
					// Same things happens in Swift, so just some bug or mistake in the sample.
					Thread.Sleep (5);

					var lossTensorData = results [trainingTensors [0]];
					var loss = new [] { 0.0f };
					lossTensorData.Read (loss);

					doubleBufferSemaphore.Release ();

					if (completion is { } c) {
						BeginInvokeOnMainThread (() => c (loss [0]));
					}
				}
			};

			var feed = NSDictionary<MPSGraphTensor, MPSGraphTensorData>.FromObjectsAndKeys (
				new [] { sourceTensorData, labelsTensorData },
				new [] { sourcePlaceholder, labelsPlaceholder },
				2);

			var fetch = this.Encode (commandBuffer, feed, trainingTensors, trainingOps, executionDesc);

			return fetch [trainingTensors [0]];
		}
	}
}
#endif // HAS_METALPERFORMANCESHADERSGRAPH
