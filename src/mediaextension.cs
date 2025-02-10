#if NET

using System;

using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using ObjCRuntime;
using UniformTypeIdentifiers;

namespace MediaExtension {
	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Native]
	[ErrorDomain ("MediaExtensionErrorDomain")]
	public enum MEError : long
	{
		UnsupportedFeature = -19320,
		AllocationFailure = -19321,
		InvalidParameter = -19322,
		ParsingFailure = -19323,
		InternalFailure = -19324,
		PropertyNotSupported = -19325,
		NoSuchEdit = -19326,
		NoSamples = -19327,
		LocationNotAvailable = -19328,
		EndOfStream = -19329,
		PermissionDenied = -19330,
		ReferenceMissing = -19331,
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Native]
	public enum MEFileInfoFragmentsStatus : long
	{
		CouldNotContainFragments = 0,
		ContainsFragments = 1,
		CouldContainButDoesNotContainFragments = 2,
	}

	[Flags, NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Native]
	public enum MEFormatReaderParseAdditionalFragmentsStatus : ulong
	{
		SizeIncreased = 1uL << 0,
		FragmentAdded = 1uL << 1,
		FragmentsComplete = 1uL << 2,
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MEFormatReaderInstantiationOptions : NSCopying
	{
		[Export ("allowIncrementalFragmentParsing")]
		bool AllowIncrementalFragmentParsing { get; }
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface MEFormatReaderExtension
	{
		[Abstract]
		[Export ("formatReaderWithByteSource:options:error:")]
		[return: NullAllowed]
		IMEFormatReader CreateFormatReader (MEByteSource primaryByteSource, [NullAllowed] MEFormatReaderInstantiationOptions options, [NullAllowed] out NSError error);
	}

	delegate void MEFormatReaderLoadFileInfoCallback ([NullAllowed] MEFileInfo fileInfo, [NullAllowed] NSError error);
	delegate void MEFormatReaderLoadMetadataCallback ([NullAllowed] AVMetadataItem[] metadata, [NullAllowed] NSError error);
	delegate void MEFormatReaderLoadTrackReadersCallback ([NullAllowed] IMETrackReader[] trackReaders, [NullAllowed] NSError error);
	delegate void MEFormatReaderParseAdditionalFragmentsCallback ([NullAllowed] MEFormatReaderParseAdditionalFragmentsStatus fragmentStatus, [NullAllowed] NSError error);

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface MEFormatReader
	{
		[Abstract]
		[Export ("loadFileInfoWithCompletionHandler:")]
		void LoadFileInfo (MEFormatReaderLoadFileInfoCallback completionHandler);

		[Abstract]
		[Export ("loadMetadataWithCompletionHandler:")]
		void LoadMetadata (MEFormatReaderLoadMetadataCallback completionHandler);

		[Abstract]
		[Export ("loadTrackReadersWithCompletionHandler:")]
		void LoadTrackReaders (MEFormatReaderLoadTrackReadersCallback completionHandler);

		[Export ("parseAdditionalFragmentsWithCompletionHandler:")]
		void ParseAdditionalFragments (MEFormatReaderParseAdditionalFragmentsCallback completionHandler);
	}

	interface IMEFormatReader { }

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject))]
	interface MEFileInfo : NSCopying
	{
		[Export ("duration", ArgumentSemantic.Assign)]
		CMTime Duration { get; set; }

		[Export ("fragmentsStatus", ArgumentSemantic.Assign)]
		MEFileInfoFragmentsStatus FragmentsStatus { get; set; }
	}

	delegate void METrackReaderLoadTrackInfoCallback ([NullAllowed] METrackInfo trackInfo, [NullAllowed] NSError error);
	delegate void METrackReaderGenerateSampleCursorCallback ([NullAllowed] IMESampleCursor trackInfo, [NullAllowed] NSError error);
	delegate void METrackReaderLoadUneditedDurationCallback ([NullAllowed] CMTime uneditedDuration, [NullAllowed] NSError error);
	delegate void METrackReaderLoadTotalSampleDataLengthCallback (long totalSampleDataLength, [NullAllowed] NSError error);
	delegate void METrackReaderLoadEstimatedDataRateCallback (float estimatedDataRate, [NullAllowed] NSError error);
	delegate void METrackReaderLoadMetadataCallback ([NullAllowed] AVMetadataItem[] metadata, [NullAllowed] NSError error);

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface METrackReader
	{
		[Abstract]
		[Export ("loadTrackInfoWithCompletionHandler:")]
		void LoadTrackInfo (METrackReaderLoadTrackInfoCallback completionHandler);

		[Abstract]
		[Export ("generateSampleCursorAtPresentationTimeStamp:completionHandler:")]
		void GenerateSampleCursorAtPresentationTimeStamp (CMTime presentationTimeStamp, METrackReaderGenerateSampleCursorCallback completionHandler);

		[Abstract]
		[Export ("generateSampleCursorAtFirstSampleInDecodeOrderWithCompletionHandler:")]
		void GenerateSampleCursorAtFirstSampleInDecodeOrder (METrackReaderGenerateSampleCursorCallback completionHandler);

		[Abstract]
		[Export ("generateSampleCursorAtLastSampleInDecodeOrderWithCompletionHandler:")]
		void GenerateSampleCursorAtLastSampleInDecodeOrder (METrackReaderGenerateSampleCursorCallback completionHandler);

		[Export ("loadUneditedDurationWithCompletionHandler:")]
		void LoadUneditedDuration (METrackReaderLoadUneditedDurationCallback completionHandler);

		[Export ("loadTotalSampleDataLengthWithCompletionHandler:")]
		void LoadTotalSampleDataLength (METrackReaderLoadTotalSampleDataLengthCallback completionHandler);

		[Export ("loadEstimatedDataRateWithCompletionHandler:")]
		void LoadEstimatedDataRate (METrackReaderLoadEstimatedDataRateCallback completionHandler);

		[Export ("loadMetadataWithCompletionHandler:")]
		void LoadMetadata (METrackReaderLoadMetadataCallback completionHandler);
	}

	interface IMETrackReader { }

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface METrackInfo : NSCopying
	{
		[Export ("initWithMediaType:trackID:formatDescriptions:")]
		[DesignatedInitializer]
		// It's not clear from the documentation which type the format descriptors are, so keep as an array of NSObject for now.
		NativeHandle Constructor (CMMediaType mediaType, int trackId, [Params] NSObject[] formatDescriptions);

		[Export ("mediaType")]
		CMMediaType MediaType { get; }

		[Export ("trackID")]
		int TrackId { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		// It's not clear from the documentation which type the format descriptors are, so keep as an array of NSObject for now.
		// However, name as 'Weak' to leave the good name open for when we know.
		[Export ("formatDescriptions", ArgumentSemantic.Copy)]
		NSObject[] WeakFormatDescriptions { get; }

		// Inlined from the OptionalProperties (METrackInfo) category
		[Export ("naturalTimescale")]
		CMTimeScale NaturalTimescale { get; set; }

		// Inlined from the OptionalProperties (METrackInfo) category
		[Export ("trackEdits", ArgumentSemantic.Copy), NullAllowed]
		[BindAs (typeof (CMTimeMapping []))]
		NSValue[] TrackEdits { get; set; }

		// Inlined from the LanguageTagOptionalProperties (METrackInfo) category
		[NullAllowed, Export ("extendedLanguageTag")]
		string ExtendedLanguageTag { get; set; }

		// Inlined from the VideoSpecificOptionalProperties (METrackInfo) category
		[Export ("naturalSize", ArgumentSemantic.Assign)]
		CGSize NaturalSize { get; set; }

		// Inlined from the VideoSpecificOptionalProperties (METrackInfo) category
		[Export ("preferredTransform", ArgumentSemantic.Assign)]
		CGAffineTransform PreferredTransform { get; set; }

		// Inlined from the VideoSpecificOptionalProperties (METrackInfo) category
		[Export ("nominalFrameRate")]
		float NominalFrameRate { get; set; }

		// Inlined from the VideoSpecificOptionalProperties (METrackInfo) category
		[Export ("requiresFrameReordering")]
		bool RequiresFrameReordering { get; set; }
	}

	delegate void MESampleCursorStepInOrderCallback (long stepCount, [NullAllowed] NSError error);
	delegate void MESampleCursorStepByTimeCallback (CMTime actualDecodeTime, bool positionWasPinned, [NullAllowed] NSError error);
	delegate void MESampleCursorLoadSampleBufferCallback ([NullAllowed] CMSampleBuffer newSampleBuffer, [NullAllowed] NSError error);
	delegate void MESampleCursorLoadPostDecodeProcessingMetadataCallback ([NullAllowed] NSDictionary<NSString, NSObject> postDecodeProcessingMetadata, [NullAllowed] NSError error);

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface MESampleCursor : INSCopying
	{
		[Abstract]
		[Export ("presentationTimeStamp")]
		CMTime PresentationTimeStamp { get; }

		[Abstract]
		[Export ("decodeTimeStamp")]
		CMTime DecodeTimeStamp { get; }

		[Abstract]
		[Export ("currentSampleDuration")]
		CMTime CurrentSampleDuration { get; }

		[Abstract]
		[NullAllowed, Export ("currentSampleFormatDescription")]
		CMFormatDescription CurrentSampleFormatDescription { get; }

		[Abstract]
		[Export ("stepInDecodeOrderByCount:completionHandler:")]
		void StepInDecodeOrder (long stepCount, MESampleCursorStepInOrderCallback completionHandler);

		[Abstract]
		[Export ("stepInPresentationOrderByCount:completionHandler:")]
		void StepInPresentationOrder (long stepCount, MESampleCursorStepInOrderCallback completionHandler);

		[Abstract]
		[Export ("stepByDecodeTime:completionHandler:")]
		void StepByDecodeTime (CMTime deltaDecodeTime, MESampleCursorStepByTimeCallback completionHandler);

		[Abstract]
		[Export ("stepByPresentationTime:completionHandler:")]
		void StepByPresentationTime (CMTime deltaPresentationTime, MESampleCursorStepByTimeCallback completionHandler);

		[Export ("syncInfo")]
#if XAMCORE_5_0
		AVSampleCursorChunkInfo SyncInfo { get; }
#else
		[Internal]
		AVSampleCursorChunkInfo_Blittable SyncInfo_Blittable { get; }

		[Wrap ("SyncInfo_Blittable.ToAVSampleCursorChunkInfo ()", IsVirtual = true)]
		AVSampleCursorChunkInfo SyncInfo { get; }
#endif

		[Export ("dependencyInfo")]
#if XAMCORE_5_0
		AVSampleCursorDependencyInfo DependencyInfo { get; }
#else
		[Internal]
		AVSampleCursorDependencyInfo_Blittable DependencyInfo_Blittable { get; }

		[Wrap ("DependencyInfo_Blittable.ToAVSampleCursorDependencyInfo ()", IsVirtual = true)]
		AVSampleCursorDependencyInfo DependencyInfo { get; }
#endif
		[Export ("hevcDependencyInfo", ArgumentSemantic.Copy)]
		MEHevcDependencyInfo HevcDependencyInfo { get; }

		[Export ("decodeTimeOfLastSampleReachableByForwardSteppingThatIsAlreadyLoadedByByteSource")]
		CMTime DecodeTimeOfLastSampleReachableByForwardSteppingThatIsAlreadyLoadedByByteSource { get; }

		[Export ("samplesWithEarlierDTSsMayHaveLaterPTSsThanCursor:")]
		bool SamplesWithEarlierDtssMayHaveLaterPtssThanCursor (IMESampleCursor cursor);

		[Export ("samplesWithLaterDTSsMayHaveEarlierPTSsThanCursor:")]
		bool SamplesWithLaterDtssMayHaveEarlierPtssThanCursor (IMESampleCursor cursor);

		[Export ("chunkDetailsReturningError:")]
		[return: NullAllowed]
		MESampleCursorChunk GetChunkDetails ([NullAllowed] out NSError error);

		[Export ("sampleLocationReturningError:")]
		[return: NullAllowed]
		MESampleLocation GetSampleLocation ([NullAllowed] out NSError error);

		[Export ("estimatedSampleLocationReturningError:")]
		[return: NullAllowed]
		MEEstimatedSampleLocation GetEstimatedSampleLocation ([NullAllowed] out NSError error);

		[Export ("refineSampleLocation:refinementData:refinementDataLength:refinedLocation:error:")]
		unsafe bool RefineSampleLocation (AVSampleCursorStorageRange estimatedSampleLocation, byte* refinementData, nuint refinementDataLength, out AVSampleCursorStorageRange refinedLocation, [NullAllowed] out NSError error);

		[Export ("loadSampleBufferContainingSamplesToEndCursor:completionHandler:")]
		void LoadSampleBufferContainingSamples ([NullAllowed] IMESampleCursor endSampleCursor, MESampleCursorLoadSampleBufferCallback completionHandler);

		[Export ("loadPostDecodeProcessingMetadataWithCompletionHandler:")]
		void LoadPostDecodeProcessingMetadata (MESampleCursorLoadPostDecodeProcessingMetadataCallback completionHandler);
	}

	interface IMESampleCursor { }

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MESampleCursorChunk : NSCopying
	{
		[Export ("initWithByteSource:chunkStorageRange:chunkInfo:sampleIndexWithinChunk:")]
		[DesignatedInitializer]
#if XAMCORE_5_0
		NativeHandle Constructor (MEByteSource byteSource, AVSampleCursorStorageRange chunkStorageRange, AVSampleCursorChunkInfo chunkInfo, nint sampleIndexWithinChunk);
#else
		[Internal]
		NativeHandle Constructor (MEByteSource byteSource, AVSampleCursorStorageRange chunkStorageRange, AVSampleCursorChunkInfo_Blittable chunkInfo, nint sampleIndexWithinChunk);

		[Wrap ("this (byteSource, chunkStorageRange, chunkInfo.ToBlittable (), sampleIndexWithinChunk)")]
		NativeHandle Constructor (MEByteSource byteSource, AVSampleCursorStorageRange chunkStorageRange, AVSampleCursorChunkInfo chunkInfo, nint sampleIndexWithinChunk);
#endif


		[Export ("byteSource", ArgumentSemantic.Retain)]
		MEByteSource ByteSource { get; }

		[Export ("chunkStorageRange")]
		AVSampleCursorStorageRange ChunkStorageRange { get; }

		[Export ("chunkInfo")]
#if XAMCORE_5_0
		AVSampleCursorChunkInfo ChunkInfo { get; }
#else
		[Internal]
		AVSampleCursorChunkInfo_Blittable ChunkInfo_Blittable { get; }

		[Wrap ("ChunkInfo_Blittable.ToAVSampleCursorChunkInfo ()", IsVirtual = true)]
		AVSampleCursorChunkInfo ChunkInfo { get; }
#endif

		[Export ("sampleIndexWithinChunk")]
		nint SampleIndexWithinChunk { get; }
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MESampleLocation : NSCopying
	{
		[Export ("initWithByteSource:sampleLocation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MEByteSource byteSource, AVSampleCursorStorageRange sampleLocation);

		[Export ("sampleLocation")]
		AVSampleCursorStorageRange SampleLocation { get; }

		[Export ("byteSource", ArgumentSemantic.Retain)]
		MEByteSource ByteSource { get; }
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface MEEstimatedSampleLocation : INSCopying
	{
		[Export ("initWithByteSource:estimatedSampleLocation:refinementDataLocation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MEByteSource byteSource, AVSampleCursorStorageRange estimatedSampleLocation, AVSampleCursorStorageRange refinementDataLocation);

		[Export ("estimatedSampleLocation")]
		AVSampleCursorStorageRange EstimatedSampleLocation { get; }

		[Export ("refinementDataLocation")]
		AVSampleCursorStorageRange RefinementDataLocation { get; }

		[Export ("byteSource", ArgumentSemantic.Retain)]
		MEByteSource ByteSource { get; }
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject), Name = "MEHEVCDependencyInfo")]
	interface MEHevcDependencyInfo : INSCopying
	{
		[Export ("temporalSubLayerAccess")]
		bool TemporalSubLayerAccess { [Bind ("hasTemporalSubLayerAccess")] get; set; }

		[Export ("stepwiseTemporalSubLayerAccess")]
		bool StepwiseTemporalSubLayerAccess { [Bind ("hasStepwiseTemporalSubLayerAccess")] get; set; }

		[Export ("syncSampleNALUnitType")]
		short SyncSampleNALUnitType { get; set; }

		// Inlined from the HEVCTemporalLevelInfo (MEHEVCDependencyInfo) category
		[Export ("temporalLevel")]
		short TemporalLevel { get; set; }

		// Inlined from the HEVCTemporalLevelInfo (MEHEVCDependencyInfo) category
		[Export ("profileSpace")]
		short ProfileSpace { get; set; }

		// Inlined from the HEVCTemporalLevelInfo (MEHEVCDependencyInfo) category
		[Export ("tierFlag")]
		short TierFlag { get; set; }

		// Inlined from the HEVCTemporalLevelInfo (MEHEVCDependencyInfo) category
		[Export ("profileIndex")]
		short ProfileIndex { get; set; }

		// Inlined from the HEVCTemporalLevelInfo (MEHEVCDependencyInfo) category
		[NullAllowed, Export ("profileCompatibilityFlags", ArgumentSemantic.Copy)]
		NSData ProfileCompatibilityFlags { get; set; }

		// Inlined from the HEVCTemporalLevelInfo (MEHEVCDependencyInfo) category
		[NullAllowed, Export ("constraintIndicatorFlags", ArgumentSemantic.Copy)]
		NSData ConstraintIndicatorFlags { get; set; }

		// Inlined from the HEVCTemporalLevelInfo (MEHEVCDependencyInfo) category
		[Export ("levelIndex")]
		short LevelIndex { get; set; }
	}

	delegate void MEByteSourceReadBytesCallback (nuint bytesRead, [NullAllowed] NSError error);
	delegate void MEByteSourceReadDataCallback ([NullAllowed] NSData data, [NullAllowed] NSError error);

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MEByteSource
	{
		[Export ("fileName")]
		string FileName { get; }

		[NullAllowed, Export ("contentType")]
		UTType ContentType { get; }

		[Export ("fileLength")]
		long FileLength { get; }

		[Export ("relatedFileNamesInSameDirectory")]
		string[] RelatedFileNamesInSameDirectory { get; }

		[Export ("readDataOfLength:fromOffset:toDestination:completionHandler:")]
		unsafe void ReadData (nuint length, long offset, byte* dest, MEByteSourceReadBytesCallback completionHandler);

		[Export ("readDataOfLength:fromOffset:completionHandler:")]
		void ReadData (nuint length, long offset, MEByteSourceReadDataCallback completionHandler);

		[Export ("readDataOfLength:fromOffset:toDestination:bytesRead:error:")]
		unsafe bool ReadData (nuint length, long offset, byte* dest, out nuint bytesRead, [NullAllowed] out NSError error);

		[Export ("availableLengthAtOffset:")]
		long GetAvailableLength (long offset);

		[Export ("byteSourceForRelatedFileName:error:")]
		[return: NullAllowed]
		MEByteSource GetByteSource (string relatedFileName, [NullAllowed] out NSError error);
	}

	[Flags]
	[NoTV, NoiOS, Mac (15, 0), NoMacCatalyst]
	[Native]
	public enum MEDecodeFrameStatus : ulong
	{
		NoStatus = 0,
		FrameDropped = 1uL << 0,
	}

	delegate void MEVideoDecoderDecodeFrameCallback ([NullAllowed] CVImageBuffer imageBuffer, MEDecodeFrameStatus decodeStatus, [NullAllowed] NSError error);

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface MEVideoDecoder
	{
		[Export ("producesRAWOutput")]
		bool ProducesRawOutput { get; }

		[Export ("contentHasInterframeDependencies")]
		bool ContentHasInterframeDependencies { get; }

		[Export ("recommendedThreadCount")]
		nint RecommendedThreadCount { get; set; }

		[Export ("actualThreadCount")]
		nint ActualThreadCount { get; }

		// Can't use BindAs in a protocol. We could bind it strongly in manual code,
		// but this is a protocol developers are supposed to provide an implementation
		// of (and not call themselves), in which case the manual code would be
		// useless. Thus there's no strongly typed binding for this property.
		[Export ("supportedPixelFormatsOrderedByQuality")]
		NSNumber[] SupportedPixelFormatsOrderedByQuality { get; }

		[Export ("reducedResolution", ArgumentSemantic.Assign)]
		CGSize ReducedResolution { get; set; }

		// Can't use BindAs in a protocol. We could bind it strongly in manual code,
		// but this is a protocol developers are supposed to provide an implementation
		// of (and not call themselves), in which case the manual code would be
		// useless. Thus there's no strongly typed binding for this property.
		[Export ("pixelFormatsWithReducedResolutionDecodeSupport")]
		NSNumber[] PixelFormatsWithReducedResolutionDecodeSupport { get; }

		[Abstract]
		[Export ("readyForMoreMediaData")]
		bool ReadyForMoreMediaData { [Bind ("isReadyForMoreMediaData")] get; }

		[Abstract]
		[Export ("decodeFrameFromSampleBuffer:options:completionHandler:")]
		void DecodeFrame (CMSampleBuffer sampleBuffer, MEDecodeFrameOptions options, MEVideoDecoderDecodeFrameCallback completionHandler);

		[Export ("canAcceptFormatDescription:")]
		bool CanAcceptFormatDescription (CMFormatDescription formatDescription);
	}

	interface IMEVideoDecoder {}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Static]
	interface MEVideoDecoderFields {
		[Notification]
		[Field ("MEVideoDecoderReadyForMoreMediaDataDidChangeNotification")]
		NSString ReadyForMoreMediaDataDidChangeNotification { get; }
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Protocol (BackwardsCompatibleCodeGeneration = false, Name = "MERAWProcessorExtension")]
	interface MERawProcessorExtension
	{
		[Abstract]
		[Export ("init")]
		NativeHandle Constructor ();

		[Abstract]
		[Export ("processorWithFormatDescription:extensionPixelBufferManager:error:")]
		[return: NullAllowed]
		IMERawProcessor CreateProcessor (CMVideoFormatDescription formatDescription, MERawProcessorPixelBufferManager extensionPixelBufferManager, [NullAllowed] out NSError error);
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject), Name = "MERAWProcessorPixelBufferManager")]
	interface MERawProcessorPixelBufferManager
	{
		[Export ("pixelBufferAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> PixelBufferAttributes { get; set; }

		[Export ("createPixelBufferAndReturnError:")]
		[return: NullAllowed]
		CVPixelBuffer CreatePixelBuffer ([NullAllowed] out NSError error);
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject), Name = "MERAWProcessingParameter")]
	interface MERawProcessingParameter
	{
		[Export ("name")]
		string Name { get; }

		[Export ("key")]
		string Key { get; }

		[Export ("longDescription")]
		string LongDescription { get; }

		[Export ("enabled")]
		bool Enabled { get; set; }
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(MERawProcessingParameter), Name = "MERAWProcessingListElementParameter")]
	interface MERawProcessingListElementParameter
	{
		[Export ("initWithName:description:elementID:")]
		NativeHandle Constructor (string name, string description, nint elementId);

		[Export ("listElementID")]
		nint ListElementId { get; }
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(MERawProcessingParameter), Name = "MERAWProcessingBooleanParameter")]
	[DisableDefaultCtor]
	interface MERawProcessingBooleanParameter
	{
		[Export ("initWithName:key:description:initialValue:")]
		NativeHandle Constructor (string name, string key, string description, bool initialValue);

		[Internal]
		[Export ("initWithName:key:description:initialValue:neutralValue:")]
		NativeHandle _InitWithNeutralValue (string name, string key, string description, bool initialValue, bool neutralValue);

		[Internal]
		[Export ("initWithName:key:description:initialValue:cameraValue:")]
		NativeHandle _InitWithCameraValue (string name, string key, string description, bool initialValue, bool cameraValue);

		[Export ("initWithName:key:description:initialValue:neutralValue:cameraValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, string key, string description, bool initialValue, bool neutralValue, bool cameraValue);

		[Export ("initialValue")]
		bool InitialValue { get; }

		[Export ("currentValue")]
		bool CurrentValue { get; set; }

		[Export ("hasNeutralValue:")]
		bool HasNeutralValue ([NullAllowed] out bool neutralValue);

		[Export ("hasCameraValue:")]
		bool HasCameraValue ([NullAllowed] out bool cameraValue);
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(MERawProcessingParameter), Name = "MERAWProcessingIntegerParameter")]
	[DisableDefaultCtor]
	interface MERawProcessingIntegerParameter
	{
		[Export ("initWithName:key:description:initialValue:maximum:minimum:")]
		NativeHandle Constructor (string name, string key, string description, nint initialValue, nint maximum, nint minimum);

		[Internal]
		[Export ("initWithName:key:description:initialValue:maximum:minimum:neutralValue:")]
		NativeHandle _InitWithNeutralValue (string name, string key, string description, nint initialValue, nint maximum, nint minimum, nint neutralValue);

		[Internal]
		[Export ("initWithName:key:description:initialValue:maximum:minimum:cameraValue:")]
		NativeHandle _InitWithCameraValue (string name, string key, string description, nint initialValue, nint maximum, nint minimum, nint cameraValue);

		[Export ("initWithName:key:description:initialValue:maximum:minimum:neutralValue:cameraValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, string key, string description, nint initialValue, nint maximum, nint minimum, nint neutralValue, nint cameraValue);

		[Export ("maximumValue")]
		nint MaximumValue { get; }

		[Export ("minimumValue")]
		nint MinimumValue { get; }

		[Export ("initialValue")]
		nint InitialValue { get; }

		[Export ("currentValue")]
		nint CurrentValue { get; set; }

		[Export ("hasNeutralValue:")]
		bool HasNeutralValue ([NullAllowed] out nint neutralValue);

		[Export ("hasCameraValue:")]
		bool HasCameraValue ([NullAllowed] out nint cameraValue);
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(MERawProcessingParameter), Name = "MERAWProcessingFloatParameter")]
	[DisableDefaultCtor]
	interface MERawProcessingFloatParameter
	{
		[Export ("initWithName:key:description:initialValue:maximum:minimum:")]
		NativeHandle Constructor (string name, string key, string description, float initialValue, float maximum, float minimum);

		[Internal]
		[Export ("initWithName:key:description:initialValue:maximum:minimum:neutralValue:")]
		NativeHandle _InitWithNeutralValue (string name, string key, string description, float initialValue, float maximum, float minimum, float neutralValue);

		[Internal]
		[Export ("initWithName:key:description:initialValue:maximum:minimum:cameraValue:")]
		NativeHandle _InitWithCameraValue (string name, string key, string description, float initialValue, float maximum, float minimum, float cameraValue);

		[Export ("initWithName:key:description:initialValue:maximum:minimum:neutralValue:cameraValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, string key, string description, float initialValue, float maximum, float minimum, float neutralValue, float cameraValue);

		[Export ("maximumValue")]
		float MaximumValue { get; }

		[Export ("minimumValue")]
		float MinimumValue { get; }

		[Export ("initialValue")]
		float InitialValue { get; }

		[Export ("currentValue")]
		float CurrentValue { get; set; }

		[Export ("hasNeutralValue:")]
		bool HasNeutralValue ([NullAllowed] out float neutralValue);

		[Export ("hasCameraValue:")]
		bool HasCameraValue ([NullAllowed] out float cameraValue);
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(MERawProcessingParameter), Name = "MERAWProcessingListParameter")]
	[DisableDefaultCtor]
	interface MERawProcessingListParameter
	{
		[Export ("initWithName:key:description:list:initialValue:")]
		NativeHandle Constructor (string name, string key, string description, MERawProcessingListElementParameter[] listElements, nint initialValue);

		[Internal]
		[Export ("initWithName:key:description:list:initialValue:neutralValue:")]
		NativeHandle _InitWithNeutralValue (string name, string key, string description, MERawProcessingListElementParameter[] listElements, nint initialValue, nint neutralValue);

		[Internal]
		[Export ("initWithName:key:description:list:initialValue:cameraValue:")]
		NativeHandle _InitWithCameraValue (string name, string key, string description, MERawProcessingListElementParameter[] listElements, nint initialValue, nint cameraValue);

		[Export ("initWithName:key:description:list:initialValue:neutralValue:cameraValue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, string key, string description, MERawProcessingListElementParameter[] listElements, nint initialValue, nint neutralValue, nint cameraValue);

		[Export ("listElements")]
		MERawProcessingListElementParameter[] ListElements { get; }

		[Export ("initialValue")]
		nint InitialValue { get; }

		[Export ("currentValue")]
		nint CurrentValue { get; set; }

		[Export ("hasNeutralValue:")]
		bool HasNeutralValue ([NullAllowed] out nint neutralValue);

		[Export ("hasCameraValue:")]
		bool HasCameraValue ([NullAllowed] out nint cameraValue);
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(MERawProcessingParameter), Name = "MERAWProcessingSubGroupParameter")]
	[DisableDefaultCtor]
	interface MERawProcessingSubGroupParameter
	{
		[Export ("initWithName:description:parameters:")]
		NativeHandle Constructor (string name, string description, MERawProcessingParameter[] parameters);

		[Export ("subGroupParameters")]
		MERawProcessingParameter[] SubGroupParameters { get; }
	}

	delegate void MERawProcessorProcessFrameCallback ([NullAllowed] CVPixelBuffer pixelBuffer, [NullAllowed] NSError error);

	[NoTV, NoiOS, Mac (15,0), MacCatalyst (18, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false, Name = "MERAWProcessor")]
	interface MERawProcessor
	{
		[Export ("metalDeviceRegistryID")]
		ulong MetalDeviceRegistryId { get; set; }

		[Export ("outputColorAttachments")]
		NSDictionary<NSString, NSObject> OutputColorAttachments { get; }

		[Abstract]
		[Export ("processingParameters")]
		MERawProcessingParameter[] ProcessingParameters { get; }

		[Abstract]
		[Export ("readyForMoreMediaData")]
		bool ReadyForMoreMediaData { [Bind ("isReadyForMoreMediaData")] get; }

		[Abstract]
		[Export ("processFrameFromImageBuffer:completionHandler:")]
		void ProcessFrame (CVPixelBuffer inputFrame, MERawProcessorProcessFrameCallback completionHandler);
	}

	[NoTV, NoiOS, Mac (15,0), MacCatalyst (18, 0)]
	[Static]
	interface MERawProcessorFields
	{
		[Notification]
		[Field ("MERAWProcessorValuesDidChangeNotification")]
		NSString ValuesDidChangeNotification { get; }

		[Notification]
		[Field ("MERAWProcessorReadyForMoreMediaDataDidChangeNotification")]
		NSString ReadyForMoreMediaDataDidChangeNotification { get; }
	}

	interface IMERawProcessor {}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface MEVideoDecoderExtension
	{
		[Abstract]
		[Export ("init")]
		NativeHandle Constructor ();

		[Abstract]
		[Export ("videoDecoderWithCodecType:videoFormatDescription:videoDecoderSpecifications:extensionDecoderPixelBufferManager:error:")]
		[return: NullAllowed]
		IMEVideoDecoder CreateVideoDecoder (CMVideoCodecType codecType, CMVideoFormatDescription videoFormatDescription, NSDictionary<NSString, NSObject> videoDecoderSpecifications, MEVideoDecoderPixelBufferManager extensionDecoderPixelBufferManager, [NullAllowed] out NSError error);
	}

	[NoTV, NoiOS, Mac (15,0), NoMacCatalyst]
	[BaseType (typeof(NSObject))]
	interface MEVideoDecoderPixelBufferManager
	{
		[Export ("pixelBufferAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> PixelBufferAttributes { get; set; }

		[Export ("createPixelBufferAndReturnError:")]
		[return: NullAllowed]
		CVPixelBuffer CreatePixelBuffer ([NullAllowed] out NSError error);

		[Export ("registerCustomPixelFormat:")]
		void RegisterCustomPixelFormat (NSDictionary<NSString, NSObject> customPixelFormat);
	}

	[NoTV, NoiOS, Mac (15, 0), NoMacCatalyst]
	[BaseType (typeof(NSObject))]
	interface MEDecodeFrameOptions
	{
		[Export ("doNotOutputFrame")]
		bool DoNotOutputFrame { get; set; }

		[Export ("realTimePlayback")]
		bool RealTimePlayback { get; set; }
	}
}
#endif // NET
