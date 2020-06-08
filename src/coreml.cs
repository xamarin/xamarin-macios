//
// CoreML C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using ImageIO;

#if !WATCH
using Metal;
using Vision;
using CoreImage;
#else
using IMTLCommandBuffer = global::Foundation.NSObject; // Won't be used just to make compilation happy.
using IMTLTexture = global::Foundation.NSObject; // Won't be used just to make compilation happy.
using IMTLDevice = global::Foundation.NSObject;
using VNImageCropAndScaleOption = global::System.nuint;
#endif

namespace CoreML {

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum MLFeatureType : long {
		Invalid = 0,
		Int64 = 1,
		Double = 2,
		String = 3,
		Image = 4,
		MultiArray = 5,
		Dictionary = 6,
		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		Sequence = 7,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[ErrorDomain ("MLModelErrorDomain")]
	[Native]
	public enum MLModelError : long {
		Generic = 0,
		FeatureType = 1,
		IO = 3,
		[Watch (4,2), TV (11,2), Mac (10,13,2), iOS (11,2)]
		CustomLayer = 4,
		CustomModel = 5,
		Update = 6,
		Parameters = 7,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum MLMultiArrayDataType : long {
		Double = 65536 | 64,
		Float32 = 65536 | 32,
		Int32 = 131072 | 32,
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[Native]
	public enum MLImageSizeConstraintType : long {
		Unspecified = 0,
		Enumerated = 2,
		Range = 3,
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[Native]
	public enum MLMultiArrayShapeConstraintType : long {
		Unspecified = 1,
		Enumerated = 2,
		Range = 3,
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[Native]
	public enum MLComputeUnits : long {
		CpuOnly = 0,
		CpuAndGpu = 1,
		All = 2,
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Native]
	public enum MLTaskState : long {
		Suspended = 1,
		Running = 2,
		Cancelling = 3,
		Completed = 4,
		Failed = 5,
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Flags]
	[Native]
	public enum MLUpdateProgressEvent : ulong {
		TrainingBegin = 1L << 0,
		EpochEnd = 1L << 1,
		MiniBatchEnd = 1L << 2,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLDictionaryFeatureProvider : MLFeatureProvider {

		[Export ("dictionary")]
		NSDictionary<NSString, MLFeatureValue> Dictionary { get; }

		[Export ("initWithDictionary:error:")]
		IntPtr Constructor (NSDictionary<NSString, NSObject> dictionary, out NSError error);
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLFeatureDescription : NSCopying, NSSecureCoding {

		[Export ("name")]
		string Name { get; }

		[Export ("type")]
		MLFeatureType Type { get; }

		[Export ("optional")]
		bool Optional { [Bind ("isOptional")] get; }

		[Export ("isAllowedValue:")]
		bool IsAllowed (MLFeatureValue value);

		// Category MLFeatureDescription (MLFeatureValueConstraints)

		[NullAllowed, Export ("multiArrayConstraint", ArgumentSemantic.Assign)]
		MLMultiArrayConstraint MultiArrayConstraint { get; }

		[NullAllowed, Export ("imageConstraint", ArgumentSemantic.Assign)]
		MLImageConstraint ImageConstraint { get; }

		[NullAllowed, Export ("dictionaryConstraint", ArgumentSemantic.Assign)]
		MLDictionaryConstraint DictionaryConstraint { get; }

		[Watch (5,0),TV (12,0), Mac (10,14), iOS (12,0)]
		[NullAllowed, Export ("sequenceConstraint")]
		MLSequenceConstraint SequenceConstraint { get; }
	}

	interface IMLFeatureProvider { }

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Protocol]
	interface MLFeatureProvider {

		[Abstract]
		[Export ("featureNames")]
		NSSet<NSString> FeatureNames { get; }

		[Abstract]
		[Export ("featureValueForName:")]
		[return: NullAllowed]
		MLFeatureValue GetFeatureValue (string featureName);
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLFeatureValue : NSCopying {

		[Export ("type")]
		MLFeatureType Type { get; }

		[Export ("undefined")]
		bool Undefined { [Bind ("isUndefined")] get; }

		[Export ("int64Value")]
		long Int64Value { get; }

		[Export ("doubleValue")]
		double DoubleValue { get; }

		[Export ("stringValue")]
		string StringValue { get; }

		[NullAllowed, Export ("multiArrayValue")]
		MLMultiArray MultiArrayValue { get; }

		[Export ("dictionaryValue")]
		NSDictionary<NSObject, NSNumber> DictionaryValue { get; }

		[NullAllowed, Export ("imageBufferValue")]
		CVPixelBuffer ImageBufferValue { get; }

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[NullAllowed, Export ("sequenceValue")]
		MLSequence SequenceValue { get; }

		[Static]
		[Export ("featureValueWithPixelBuffer:")]
		MLFeatureValue Create (CVPixelBuffer value);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[Static]
		[Export ("featureValueWithSequence:")]
		MLFeatureValue Create (MLSequence sequence);

		[Static]
		[Export ("featureValueWithInt64:")]
		MLFeatureValue Create (long value);

		[Static]
		[Export ("featureValueWithDouble:")]
		MLFeatureValue Create (double value);

		[Static]
		[Export ("featureValueWithString:")]
		MLFeatureValue Create (string value);

		[Static]
		[Export ("featureValueWithMultiArray:")]
		MLFeatureValue Create (MLMultiArray value);

		[Static]
		[Export ("undefinedFeatureValueWithType:")]
		MLFeatureValue CreateUndefined (MLFeatureType type);

		[Static]
		[Export ("featureValueWithDictionary:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (NSDictionary<NSObject, NSNumber> value, out NSError error);

		[Export ("isEqualToFeatureValue:")]
		bool IsEqual (MLFeatureValue value);

		// From MLFeatureValue (MLImageConversion)

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("featureValueWithImageAtURL:pixelsWide:pixelsHigh:pixelFormatType:options:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (NSUrl url, nint pixelsWide, nint pixelsHigh, CVPixelFormatType pixelFormatType, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Wrap ("Create (url, pixelsWide, pixelsHigh, pixelFormatType, imageOptions.GetDictionary (), out error)")]
		[return: NullAllowed]
		MLFeatureValue Create (NSUrl url, nint pixelsWide, nint pixelsHigh, CVPixelFormatType pixelFormatType, [NullAllowed] MLFeatureValueImageOption imageOptions, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("featureValueWithImageAtURL:constraint:options:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (NSUrl url, MLImageConstraint constraint, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Wrap ("Create (url, constraint, imageOptions.GetDictionary (), out error)")]
		[return: NullAllowed]
		MLFeatureValue Create (NSUrl url, MLImageConstraint constraint, [NullAllowed] MLFeatureValueImageOption imageOptions, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("featureValueWithCGImage:pixelsWide:pixelsHigh:pixelFormatType:options:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (CGImage image, nint pixelsWide, nint pixelsHigh, CVPixelFormatType pixelFormatType, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Wrap ("Create (image, pixelsWide, pixelsHigh, pixelFormatType, imageOptions.GetDictionary (), out error)")]
		[return: NullAllowed]
		MLFeatureValue Create (CGImage image, nint pixelsWide, nint pixelsHigh, CVPixelFormatType pixelFormatType, [NullAllowed] MLFeatureValueImageOption imageOptions, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("featureValueWithCGImage:constraint:options:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (CGImage image, MLImageConstraint constraint, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Wrap ("Create (image, constraint, imageOptions.GetDictionary (), out error)")]
		[return: NullAllowed]
		MLFeatureValue Create (CGImage image, MLImageConstraint constraint, [NullAllowed] MLFeatureValueImageOption imageOptions, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("featureValueWithImageAtURL:orientation:pixelsWide:pixelsHigh:pixelFormatType:options:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (NSUrl url, CGImagePropertyOrientation orientation, nint pixelsWide, nint pixelsHigh, CVPixelFormatType pixelFormatType, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Wrap ("Create (url, orientation, pixelsWide, pixelsHigh, pixelFormatType, imageOptions.GetDictionary (), out error)")]
		[return: NullAllowed]
		MLFeatureValue Create (NSUrl url, CGImagePropertyOrientation orientation, nint pixelsWide, nint pixelsHigh, CVPixelFormatType pixelFormatType, [NullAllowed] MLFeatureValueImageOption imageOptions, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("featureValueWithImageAtURL:orientation:constraint:options:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (NSUrl url, CGImagePropertyOrientation orientation, MLImageConstraint constraint, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Wrap ("Create (url, orientation, constraint, imageOptions.GetDictionary (), out error)")]
		[return: NullAllowed]
		MLFeatureValue Create (NSUrl url, CGImagePropertyOrientation orientation, MLImageConstraint constraint, [NullAllowed] MLFeatureValueImageOption imageOptions, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("featureValueWithCGImage:orientation:pixelsWide:pixelsHigh:pixelFormatType:options:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (CGImage image, CGImagePropertyOrientation orientation, nint pixelsWide, nint pixelsHigh, CVPixelFormatType pixelFormatType, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Wrap ("Create (image, orientation, pixelsWide, pixelsHigh, pixelFormatType, imageOptions.GetDictionary (), out error)")]
		[return: NullAllowed]
		MLFeatureValue Create (CGImage image, CGImagePropertyOrientation orientation, nint pixelsWide, nint pixelsHigh, CVPixelFormatType pixelFormatType, [NullAllowed] MLFeatureValueImageOption imageOptions, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("featureValueWithCGImage:orientation:constraint:options:error:")]
		[return: NullAllowed]
		MLFeatureValue Create (CGImage image, CGImagePropertyOrientation orientation, MLImageConstraint constraint, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Static]
		[Wrap ("Create (image, orientation, constraint, imageOptions.GetDictionary (), out error)")]
		[return: NullAllowed]
		MLFeatureValue Create (CGImage image, CGImagePropertyOrientation orientation, MLImageConstraint constraint, [NullAllowed] MLFeatureValueImageOption imageOptions, [NullAllowed] out NSError error);
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Internal]
	[Static]
	interface MLFeatureValueImageOptionKeys {

		[Field ("MLFeatureValueImageOptionCropRect")]
		NSString CropRectKey { get; }

		[Field ("MLFeatureValueImageOptionCropAndScale")]
		NSString CropAndScaleKey { get; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[StrongDictionary ("MLFeatureValueImageOptionKeys")]
	interface MLFeatureValueImageOption {
		CGRect CropRect { get; set; }
		VNImageCropAndScaleOption CropAndScale { get; set; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLModel {

		[Export ("modelDescription")]
		MLModelDescription ModelDescription { get; }

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[Export ("configuration")]
		MLModelConfiguration Configuration { get; }

		[Static]
		[Export ("modelWithContentsOfURL:error:")]
		[return: NullAllowed]
		MLModel Create (NSUrl url, out NSError error);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[Static]
		[Export ("modelWithContentsOfURL:configuration:error:")]
		[return: NullAllowed]
		MLModel Create (NSUrl url, MLModelConfiguration configuration, out NSError error);

		[Export ("predictionFromFeatures:error:")]
		[return: NullAllowed]
		IMLFeatureProvider GetPrediction (IMLFeatureProvider input, out NSError error);

		[Export ("predictionFromFeatures:options:error:")]
		[return: NullAllowed]
		IMLFeatureProvider GetPrediction (IMLFeatureProvider input, MLPredictionOptions options, out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("predictionsFromBatch:error:")]
		[return: NullAllowed]
		IMLBatchProvider GetPredictions (IMLBatchProvider inputBatch, [NullAllowed] out NSError error);

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[Export ("predictionsFromBatch:options:error:")]
		[return: NullAllowed]
		IMLBatchProvider GetPredictions (IMLBatchProvider inputBatch, MLPredictionOptions options, out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("parameterValueForKey:error:")]
		[return: NullAllowed]
		NSObject GetParameterValue (MLParameterKey key, [NullAllowed] out NSError error);

		// Category MLModel (MLModelCompilation)

		[Static]
		[Export ("compileModelAtURL:error:")]
		[return: NullAllowed]
		NSUrl CompileModel (NSUrl modelUrl, out NSError error);
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLModelDescription : NSSecureCoding {

		[Export ("inputDescriptionsByName")]
		NSDictionary<NSString, MLFeatureDescription> InputDescriptionsByName { get; }

		[Export ("outputDescriptionsByName")]
		NSDictionary<NSString, MLFeatureDescription> OutputDescriptionsByName { get; }

		[NullAllowed, Export ("predictedFeatureName")]
		string PredictedFeatureName { get; }

		[NullAllowed, Export ("predictedProbabilitiesName")]
		string PredictedProbabilitiesName { get; }

		[Export ("metadata")]
		[Internal]
		NSDictionary _Metadata { get; }

		[Wrap ("_Metadata")]
		MLModelMetadata Metadata { get; }

		// From MLModelDescription (MLUpdateAdditions)

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("isUpdatable")]
		bool IsUpdatable { get; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("trainingInputDescriptionsByName")]
		NSDictionary<NSString, MLFeatureDescription> TrainingInputDescriptionsByName { get; }

		// From MLModelDescription (MLParameters)

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("parameterDescriptionsByKey")]
		NSDictionary<MLParameterKey, MLParameterDescription> ParameterDescriptionsByKey { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Internal]
	[Static]
	interface MLModelMetadataKeys {

		[Field ("MLModelDescriptionKey")]
		NSString DescriptionKey { get; }

		[Field ("MLModelVersionStringKey")]
		NSString VersionStringKey { get; }

		[Field ("MLModelAuthorKey")]
		NSString AuthorKey { get; }

		[Field ("MLModelLicenseKey")]
		NSString LicenseKey { get; }

		[Field ("MLModelCreatorDefinedKey")]
		NSString CreatorDefinedKey { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[StrongDictionary ("MLModelMetadataKeys")]
	interface MLModelMetadata {
		string Description { get; }
		string VersionString { get; }
		string Author { get; }
		string License { get; }
		string CreatorDefined { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MLMultiArray {

		[Export ("dataPointer")]
		IntPtr DataPointer { get; }

		[Export ("dataType")]
		MLMultiArrayDataType DataType { get; }

		[Internal]
		[Export ("shape")]
		IntPtr _Shape { get; }

		[Internal]
		[Export ("strides")]
		IntPtr _Strides { get; }

		[Export ("count")]
		nint Count { get; }

		// From MLMultiArray (Creation) Category

		[Export ("initWithShape:dataType:error:")]
		IntPtr Constructor (NSNumber [] shape, MLMultiArrayDataType dataType, out NSError error);

		[Export ("initWithDataPointer:shape:dataType:strides:deallocator:error:")]
		IntPtr Constructor (IntPtr dataPointer, NSNumber [] shape, MLMultiArrayDataType dataType, NSNumber [] strides, Action<IntPtr> deallocator, out NSError error);

		// From MLMultiArray (NSNumberDataAccess) Category

		[Export ("objectAtIndexedSubscript:")]
		NSNumber GetObject (nint idx);

		[Export ("objectForKeyedSubscript:")]
		NSNumber GetObject (NSNumber [] key);

		[Sealed]
		[Export ("objectForKeyedSubscript:")]
		[Internal]
		// Bind 'key' as IntPtr to avoid multiple conversions (nint[] -> NSNumber[] -> NSArray)
		NSNumber GetObject (IntPtr key);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject (NSNumber obj, nint idx);

		[Export ("setObject:forKeyedSubscript:")]
		void SetObject (NSNumber obj, NSNumber [] key);

		[Sealed]
		[Export ("setObject:forKeyedSubscript:")]
		[Internal]
		// Bind 'key' as IntPtr to avoid multiple conversions (nint[] -> NSNumber[] -> NSArray)
		void SetObject (NSNumber obj, IntPtr key);
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MLDictionaryConstraint : NSSecureCoding {

		[Export ("keyType")]
		MLFeatureType KeyType { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MLImageConstraint : NSSecureCoding {

		[Export ("pixelsHigh")]
		nint PixelsHigh { get; }

		[Export ("pixelsWide")]
		nint PixelsWide { get; }

		[Export ("pixelFormatType")]
		uint PixelFormatType { get; }

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[Export ("sizeConstraint")]
		MLImageSizeConstraint SizeConstraint { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MLMultiArrayConstraint : NSSecureCoding {

		[Internal]
		[Export ("shape")]
		IntPtr _Shape { get; }

		[Export ("dataType")]
		MLMultiArrayDataType DataType { get; }

		[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
		[Export ("shapeConstraint")]
		MLMultiArrayShapeConstraint ShapeConstraint { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLPredictionOptions {

		[Export ("usesCPUOnly")]
		bool UsesCpuOnly { get; set; }
	}

	[NoWatch, TV (11,2), Mac (10,13,2), iOS (11,2)]
	[Protocol]
	interface MLCustomLayer {

		// Must be manually inlined in classes implementing this protocol
		//[Abstract]
		//[Export ("initWithParameterDictionary:error:")]
		//IntPtr Constructor (NSDictionary<NSString, NSObject> parameters, [NullAllowed] out NSError error);

		[Abstract]
		[Export ("setWeightData:error:")]
		bool SetWeightData (NSData[] weights, [NullAllowed] out NSError error);

		[Abstract]
		[Export ("outputShapesForInputShapes:error:")]
		[return: NullAllowed]
		NSArray[] GetOutputShapes (NSArray[] inputShapes, [NullAllowed] out NSError error);

		[Abstract]
		[Export ("evaluateOnCPUWithInputs:outputs:error:")]
		bool EvaluateOnCpu (MLMultiArray[] inputs, MLMultiArray[] outputs, [NullAllowed] out NSError error);

		[Export ("encodeToCommandBuffer:inputs:outputs:error:")]
		bool Encode (IMTLCommandBuffer commandBuffer, IMTLTexture[] inputs, IMTLTexture[] outputs, [NullAllowed] out NSError error);
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MLArrayBatchProvider : MLBatchProvider {

		[Export ("array")]
		IMLFeatureProvider[] Array { get; }

		[Export ("initWithFeatureProviderArray:")]
		IntPtr Constructor (IMLFeatureProvider[] array);

		[Export ("initWithDictionary:error:")]
		IntPtr Constructor (NSDictionary<NSString, NSArray> dictionary, out NSError error);
	}

	interface IMLBatchProvider {}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[Protocol]
	interface MLBatchProvider {

		[Abstract]
		[Export ("count")]
		nint Count { get; }

		[Abstract]
		[Export ("featuresAtIndex:")]
		IMLFeatureProvider GetFeatures (nint index);
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[Protocol, Model (AutoGeneratedName = true)]
	interface MLCustomModel {

		// [Abstract]
		[Export ("initWithModelDescription:parameterDictionary:error:")]
		IntPtr Constructor (MLModelDescription modelDescription, NSDictionary<NSString, NSObject> parameters, out NSError error);

		[Abstract]
		[Export ("predictionFromFeatures:options:error:")]
		[return: NullAllowed]
		IMLFeatureProvider GetPrediction (IMLFeatureProvider inputFeatures, MLPredictionOptions options, out NSError error);

		[Export ("predictionsFromBatch:options:error:")]
		[return: NullAllowed]
		IMLBatchProvider GetPredictions (IMLBatchProvider inputBatch, MLPredictionOptions options, out NSError error);
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MLImageSize : NSSecureCoding {

		[Export ("pixelsWide")]
		nint PixelsWide { get; }

		[Export ("pixelsHigh")]
		nint PixelsHigh { get; }
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MLImageSizeConstraint : NSSecureCoding {

		[Export ("type")]
		MLImageSizeConstraintType Type { get; }

		[Export ("pixelsWideRange")]
		NSRange PixelsWideRange { get; }

		[Export ("pixelsHighRange")]
		NSRange PixelsHighRange { get; }

		[Export ("enumeratedImageSizes")]
		MLImageSize[] EnumeratedImageSizes { get; }
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MLMultiArrayShapeConstraint : NSSecureCoding {

		[Export ("type")]
		MLMultiArrayShapeConstraintType Type { get; }

		[Export ("sizeRangeForDimension")]
		NSValue[] SizeRangeForDimension { get; }

		[Export ("enumeratedShapes")]
		NSArray<NSNumber>[] EnumeratedShapes { get; }
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MLSequence {

		[Export ("type")]
		MLFeatureType Type { get; }

		[Static]
		[Export ("emptySequenceWithType:")]
		MLSequence CreateEmpty (MLFeatureType type);

		[Static]
		[Export ("sequenceWithStringArray:")]
		MLSequence Create (string[] stringValues);

		[Export ("stringValues")]
		string[] StringValues { get; }

		[Static]
		[Export ("sequenceWithInt64Array:")]
		MLSequence Create (NSNumber[] int64Values);

		[Export ("int64Values")]
		NSNumber[] Int64Values { get; }
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MLSequenceConstraint : NSCopying, NSSecureCoding {

		[Export ("valueDescription")]
		MLFeatureDescription ValueDescription { get; }

		[Export ("countRange")]
		NSRange CountRange { get; }
	}

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	interface MLModelConfiguration : NSCopying, NSSecureCoding {

		[Export ("computeUnits", ArgumentSemantic.Assign)]
		MLComputeUnits ComputeUnits { get; set; }

		// From MLModelConfiguration (MLGPUConfigurationOptions)

		[NoWatch, TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("allowLowPrecisionAccumulationOnGPU")]
		bool AllowLowPrecisionAccumulationOnGpu { get; set; }

		[NoWatch, TV (13,0), Mac (10,15), iOS (13,0)]
		[NullAllowed, Export ("preferredMetalDevice", ArgumentSemantic.Assign)]
		IMTLDevice PreferredMetalDevice { get; set; }

		// From MLModelConfiguration (MLModelParameterAdditions)

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[NullAllowed, Export ("parameters", ArgumentSemantic.Assign)]
		NSDictionary<MLParameterKey, NSObject> Parameters { get; set; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLKey : NSCopying, NSSecureCoding {

		[Export ("name")]
		string Name { get; }

		[NullAllowed, Export ("scope")]
		string Scope { get; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (MLKey))]
	[DisableDefaultCtor]
	interface MLMetricKey {

		[Static]
		[Export ("lossValue")]
		MLMetricKey LossValue { get; }

		[Static]
		[Export ("epochIndex")]
		MLMetricKey EpochIndex { get; }

		[Static]
		[Export ("miniBatchIndex")]
		MLMetricKey MiniBatchIndex { get; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLNumericConstraint : NSSecureCoding {

		[Export ("minNumber")]
		NSNumber MinNumber { get; } // no better type found on docs nor swift

		[Export ("maxNumber")]
		NSNumber MaxNumber { get; } // no better type found on docs nor swift

		[NullAllowed, Export ("enumeratedNumbers")]
		NSSet<NSNumber> EnumeratedNumbers { get; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLParameterDescription : NSSecureCoding {

		[Export ("key")]
		MLParameterKey Key { get; }

		[Export ("defaultValue")]
		NSObject DefaultValue { get; }

		[NullAllowed, Export ("numericConstraint")]
		MLNumericConstraint NumericConstraint { get; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (MLKey))]
	[DisableDefaultCtor]
	interface MLParameterKey {

		[Static]
		[Export ("learningRate")]
		MLParameterKey LearningRate { get; }

		[Static]
		[Export ("momentum")]
		MLParameterKey Momentum { get; }

		[Static]
		[Export ("miniBatchSize")]
		MLParameterKey MiniBatchSize { get; }

		[Static]
		[Export ("beta1")]
		MLParameterKey Beta1 { get; }

		[Static]
		[Export ("beta2")]
		MLParameterKey Beta2 { get; }

		[Static]
		[Export ("eps")]
		MLParameterKey Eps { get; }

		[Static]
		[Export ("epochs")]
		MLParameterKey Epochs { get; }

		[Static]
		[Export ("shuffle")]
		MLParameterKey Shuffle { get; }

		[Static]
		[Export ("seed")]
		MLParameterKey Seed { get; }

		[Static]
		[Export ("numberOfNeighbors")]
		MLParameterKey NumberOfNeighbors { get; }

		// From MLParameterKey (MLLinkedModelParameters)

		[Static]
		[Export ("linkedModelFileName")]
		MLParameterKey LinkedModelFileName { get; }

		[Static]
		[Export ("linkedModelSearchPath")]
		MLParameterKey LinkedModelSearchPath { get; }

		// From MLParameterKey (MLNeuralNetworkParameters)

		[Static]
		[Export ("weights")]
		MLParameterKey Weights { get; }

		[Static]
		[Export ("biases")]
		MLParameterKey Biases { get; }

		// From MLParameterKey (MLScopedParameters)

		[Export ("scopedTo:")]
		MLParameterKey GetScopedParameter (string scope);

	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLTask {

		[Export ("taskIdentifier")]
		string TaskIdentifier { get; }

		[Export ("state", ArgumentSemantic.Assign)]
		MLTaskState State { get; }

		[NullAllowed, Export ("error", ArgumentSemantic.Copy)]
		NSError Error { get; }

		[Export ("resume")]
		void Resume ();

		[Export ("cancel")]
		void Cancel ();
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLUpdateContext {

		[Export ("task")]
		MLUpdateTask Task { get; }

		[Export ("model")]
		IMLWritable Model { get; }

		[Export ("event")]
		MLUpdateProgressEvent Event { get; }

		[Export ("metrics")]
		NSDictionary<MLMetricKey, NSObject> Metrics { get; }

		[Export ("parameters")]
		NSDictionary<MLParameterKey, NSObject> Parameters { get; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MLUpdateProgressHandlers {

		[Export ("initForEvents:progressHandler:completionHandler:")]
		IntPtr Constructor (MLUpdateProgressEvent interestedEvents, [NullAllowed] Action<MLUpdateContext> progressHandler, Action<MLUpdateContext> completionHandler);
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (MLTask))]
	[DisableDefaultCtor]
	interface MLUpdateTask {

		[Static]
		[Export ("updateTaskForModelAtURL:trainingData:configuration:completionHandler:error:")]
		[return: NullAllowed]
		MLUpdateTask Create (NSUrl modelUrl, IMLBatchProvider trainingData, [NullAllowed] MLModelConfiguration configuration, Action<MLUpdateContext> completionHandler, [NullAllowed] out NSError error);

		[Static]
		[Export ("updateTaskForModelAtURL:trainingData:configuration:progressHandlers:error:")]
		[return: NullAllowed]
		MLUpdateTask Create (NSUrl modelUrl, IMLBatchProvider trainingData, [NullAllowed] MLModelConfiguration configuration, MLUpdateProgressHandlers progressHandlers, [NullAllowed] out NSError error);

		[Export ("resumeWithParameters:")]
		void Resume (NSDictionary<MLParameterKey, NSObject> updateParameters);
	}

	interface IMLWritable { }

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Protocol]
	interface MLWritable {

		[Abstract]
		[Export ("writeToURL:error:")]
		bool Write (NSUrl url, [NullAllowed] out NSError error);
	}
}
