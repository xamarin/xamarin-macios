//
// CoreML C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using XamCore.ObjCRuntime;
using XamCore.CoreFoundation;
using XamCore.Foundation;

#if !WATCH
using XamCore.CoreVideo;
#endif

namespace XamCore.CoreML {

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Native]
	public enum MLFeatureType : nint {
		Invalid = 0,
		Int64 = 1,
		Double = 2,
		String = 3,
		Image = 4,
		MultiArray = 5,
		Dictionary = 6
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[ErrorDomain ("MLModelErrorDomain")]
	[Native]
	public enum MLModelError : nint {
		Generic = 0,
		FeatureType = 1,
		DescriptionMismatch = 2,
		Io = 3
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Native]
	public enum MLMultiArrayDataType : nint {
		Double = 65536 | 64,
		Float32 = 65536 | 32,
		Int32 = 131072 | 32
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLDictionaryFeatureProvider : MLFeatureProvider {

		[Export ("dictionary")]
		NSDictionary<NSString, MLFeatureValue> Dictionary { get; }

		[Export ("initWithDictionary:error:")]
		IntPtr Constructor (NSDictionary<NSString, NSObject> dictionary, out NSError error);
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLFeatureDescription : NSCopying {

		[Export ("name")]
		string Name { get; }

		[Export ("type")]
		MLFeatureType Type { get; }

		[Export ("optional")]
		bool Optional { [Bind ("isOptional")] get; }

		[Export ("isAllowedValue:")]
		bool IsAllowed (MLFeatureValue value);
	}

	interface IMLFeatureProvider { }

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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

#if !WATCH
		[NullAllowed, Export ("imageBufferValue")]
		CVPixelBuffer ImageBufferValue { get; }

		[Static]
		[Export ("featureValueWithPixelBuffer:")]
		MLFeatureValue FromPixelBuffer (CVPixelBuffer value);
#endif
		[Static]
		[Export ("featureValueWithInt64:")]
		MLFeatureValue FromInt64 (long value);

		[Static]
		[Export ("featureValueWithDouble:")]
		MLFeatureValue FromDouble (double value);

		[Static]
		[Export ("featureValueWithString:")]
		MLFeatureValue FromString (string value);

		[Static]
		[Export ("featureValueWithMultiArray:")]
		MLFeatureValue FromMultiArray (MLMultiArray value);

		[Static]
		[Export ("undefinedFeatureValueWithType:")]
		MLFeatureValue CreateUndefined (MLFeatureType type);

		[Static]
		[Export ("featureValueWithDictionary:error:")]
		[return: NullAllowed]
		MLFeatureValue FromDictionary (NSDictionary<NSObject, NSNumber> value, out NSError error);

		[Export ("isEqualToFeatureValue:")]
		bool IsEqual (MLFeatureValue value);
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLModel {

		[Export ("modelDescription")]
		MLModelDescription ModelDescription { get; }

		[Static]
		[Export ("modelWithContentsOfURL:error:")]
		[return: NullAllowed]
		MLModel FromUrl (NSUrl url, [NullAllowed] out NSError error);

		[Export ("predictionFromFeatures:error:")]
		[return: NullAllowed]
		IMLFeatureProvider GetPrediction (IMLFeatureProvider input, out NSError error);
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface MLModelDescription {

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
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
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
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[StrongDictionary ("MLModelMetadataKeys")]
	interface MLModelMetadata {
		string Description { get; }
		string VersionString { get; }
		string Author { get; }
		string License { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface MLMultiArray {

		[Export ("dataPointer")]
		IntPtr DataPointer { get; }

		[Export ("dataType")]
		MLMultiArrayDataType DataType { get; }

		[Export ("shape")]
		NSNumber [] Shape { get; }

		[Export ("strides")]
		NSNumber [] Strides { get; }

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

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject (NSNumber obj, nint idx);

		[Export ("setObject:forKeyedSubscript:")]
		void SetObject (NSNumber obj, NSNumber [] key);
	}
}
#endif // XAMCORE_2_0
