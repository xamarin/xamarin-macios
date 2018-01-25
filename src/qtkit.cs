//
// Copyright 2010, Novell, Inc.
// Copyright 2010, Duane Wandless.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// TODO:
//   API: QTSampleBuffer.h expose a couple of AudioBufferList methods 
//   API: QTMovie needs low-level access to some methods
//   API: Expose the individual QTMovie*Attribute as C# properties
//   API: "Media" from QuickTime is not bound, so we expose as IntPtr in QTMedia
//   API: some stuff missing for QTTime.h and QTTimeRange
//  
//   QTCaptureDecompressedAudioOutput.h
//   QTCaptureVideoPreviewOutput.h
//   QTError.h -- Missing the NSString keys
//
// Need to strongly type/provide accessors in QTCaptureDevice for
//		[Field ("QTCaptureDeviceLinkedDevicesAttribute")]
//		[Field ("QTCaptureDeviceAvailableInputSourcesAttribute")]
//		[Field ("QTCaptureDeviceInputSourceIdentifierAttribute")]
//		[Field ("QTCaptureDeviceLegacySequenceGrabberAttribute")]

using System;
using Foundation;
using ObjCRuntime;
using AppKit;
using CoreAnimation;
using CoreVideo;
using CoreImage;
using CoreGraphics;

namespace QTKit
{
	[BaseType (typeof (QTCaptureOutput))]
	interface QTCaptureAudioPreviewOutput {
		[Export ("outputDeviceUniqueID")]
		string OutputDeviceUniqueID { get; set; }

		[Export ("volume")]
		float Volume { get; set; } /* float, not CGFloat */ 
	}

	[BaseType (typeof (NSObject))]
	interface QTCaptureConnection {
		[Export ("owner")]
		NSObject Owner { get; }

		[Export ("mediaType")]
		string MediaType { get; }

		[Export ("formatDescription")]
		QTFormatDescription FormatDescription { get; }

		[Export ("attributeIsReadOnly:")]
		bool IsAttributeReadOnly (string attributeKey);

		[Export ("attributeForKey:")]
		NSObject GetAttribute (NSString attributeKey);

		[Export ("setAttribute:forKey:")]
		void SetAttribute (NSObject attribute, NSString key);

		//Detected properties
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")]get; set; }

		[Export ("connectionAttributes")]
		NSDictionary ConnectionAttributes { get; set; }

		[Notification]
		[Field ("QTCaptureConnectionFormatDescriptionWillChangeNotification")]
		NSString FormatDescriptionWillChangeNotification { get; }

		[Notification]
		[Field ("QTCaptureConnectionFormatDescriptionDidChangeNotification")]
		NSString FormatDescriptionDidChangeNotification { get; }

		[Notification]
		[Field ("QTCaptureConnectionAttributeWillChangeNotification")]
		NSString AttributeWillChangeNotification { get; }

		[Notification]
		[Field ("QTCaptureConnectionAttributeDidChangeNotification")]
		NSString AttributeDidChangeNotification { get; }

		[Field ("QTCaptureConnectionChangedAttributeKey")]
		NSString ChangedAttributeKey { get; }

		[Field ("QTCaptureConnectionAudioAveragePowerLevelsAttribute")]
		NSString AudioAveragePowerLevelsAttribute { get; }

		[Field ("QTCaptureConnectionAudioPeakHoldLevelsAttribute")]
		NSString AudioPeakHoldLevelsAttribute { get; }

		[Field ("QTCaptureConnectionAudioMasterVolumeAttribute")]
		NSString AudioMasterVolumeAttribute { get; }

		[Field ("QTCaptureConnectionAudioVolumesAttribute")]
		NSString AudioVolumesAttribute { get; }

		[Field ("QTCaptureConnectionEnabledAudioChannelsAttribute")]
		NSString EnabledAudioChannelsAttribute { get; }
	}

	[BaseType (typeof (QTCaptureOutput), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (QTCaptureDecompressedVideoOutputDelegate)})]
	interface QTCaptureDecompressedVideoOutput {
		[Export ("outputVideoFrame:withSampleBuffer:fromConnection:")]
		void OutputVideoFrame (CVImageBuffer videoFrame, QTSampleBuffer sampleBuffer, QTCaptureConnection connection);

		//Detected properties
		[Export ("pixelBufferAttributes")]
		NSDictionary PixelBufferAttributes { get; set; }

		[Export ("minimumVideoFrameInterval")]
		double MinimumVideoFrameInterval { get; set; }

		[Export ("automaticallyDropsLateVideoFrames")]
		bool AutomaticallyDropsLateVideoFrames { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		QTCaptureDecompressedVideoOutputDelegate Delegate { get; set; }
	}

	[BaseType (typeof (NSObject), Name="QTCaptureDecompressedVideoOutput_Delegate")]
	[Model]
	[Protocol (IsInformal = true)]
	interface QTCaptureDecompressedVideoOutputDelegate {
		[Export ("captureOutput:didOutputVideoFrame:withSampleBuffer:fromConnection:"), EventArgs ("QTCaptureVideoFrame")]
		void DidOutputVideoFrame (QTCaptureOutput captureOutput, CVImageBuffer videoFrame, QTSampleBuffer sampleBuffer, QTCaptureConnection connection);

		[Export ("captureOutput:didDropVideoFrameWithSampleBuffer:fromConnection:"), EventArgs ("QTCaptureVideoDropped")]
		void DidDropVideoFrame (QTCaptureOutput captureOutput, QTSampleBuffer sampleBuffer, QTCaptureConnection connection);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // An uncaught exception was raised: Cannot instantiate a QTCaptureDevice directly.
	interface QTCaptureDevice : NSCoding {
		[Static]
		[Export ("inputDevices")]
		QTCaptureDevice [] InputDevices { get; }

		[Static]
		[Internal, Export ("inputDevicesWithMediaType:")]
		QTCaptureDevice [] _GetInputDevices (NSString forMediaType);

		[Static]
		[Internal,Export ("defaultInputDeviceWithMediaType:")]
		QTCaptureDevice _GetDefaultInputDevice (NSString forMediaType);

		[Static]
		[Export ("deviceWithUniqueID:")]
		QTCaptureDevice FromUniqueID (string deviceUniqueID);

		[Export ("uniqueID")]
		string UniqueID { get; }

		[Export ("modelUniqueID")]
		string ModelUniqueID { get; }

		[Export ("localizedDisplayName")]
		string LocalizedDisplayName { get; }

		[Export ("formatDescriptions")]
		QTFormatDescription [] FormatDescriptions { get; }

		[Export ("hasMediaType:"), Internal]
		bool _HasMediaType (string mediaType);

		[Export ("attributeIsReadOnly:")]
		bool IsAttributeReadOnly (string attributeKey);

		[Export ("attributeForKey:")]
		NSObject GetAttribute (string attributeKey);

		[Export ("setAttribute:forKey:")]
		void SetAttribute (NSObject attribute, string attributeKey);

		[Export ("isConnected")]
		bool IsConnected { get; }

		[Export ("isInUseByAnotherApplication")]
		bool IsInUseByAnotherApplication { get; }

		[Export ("isOpen")]
		bool IsOpen { get; }

		[Export ("open:")]
		bool Open (out NSError error);

		[Export ("close")]
		void Close ();

		//Detected properties
		[Export ("deviceAttributes")]
		NSDictionary DeviceAttributes { get; set; }

		[Notification]
		[Field ("QTCaptureDeviceWasConnectedNotification")]
		NSString WasConnectedNotification { get; }

		[Notification]
		[Field ("QTCaptureDeviceWasDisconnectedNotification")]
		NSString WasDisconnectedNotification { get; }

		[Notification]
		[Field ("QTCaptureDeviceFormatDescriptionsWillChangeNotification")]
		NSString FormatDescriptionsWillChangeNotification { get; }

		[Notification]
		[Field ("QTCaptureDeviceFormatDescriptionsDidChangeNotification")]
		NSString FormatDescriptionsDidChangeNotification { get; }

		[Notification]
		[Field ("QTCaptureDeviceAttributeWillChangeNotification")]
		NSString AttributeWillChangeNotification { get; }

		[Notification]
		[Field ("QTCaptureDeviceAttributeDidChangeNotification")]
		NSString AttributeDidChangeNotification { get; }

		[Field ("QTCaptureDeviceChangedAttributeKey")]
		NSString ChangedAttributeKey { get; }

		[Field ("QTCaptureDeviceLinkedDevicesAttribute")]
		NSString LinkedDevicesAttribute { get; }
		
		[Field ("QTCaptureDeviceAvailableInputSourcesAttribute")]
		NSString AvailableInputSourcesAttribute { get; }
		
		[Field ("QTCaptureDeviceInputSourceIdentifierAttribute")]
		NSString InputSourceIdentifierAttribute { get; }
		
		[Field ("QTCaptureDeviceInputSourceIdentifierKey")]
		NSString InputSourceIdentifierKey { get; }
		
		[Field ("QTCaptureDeviceInputSourceLocalizedDisplayNameKey")]
		NSString InputSourceLocalizedDisplayNameKey { get; }
		
		[Mac (10, 5, 0, PlatformArchitecture.Arch32)] 
		[Field ("QTCaptureDeviceLegacySequenceGrabberAttribute")]
		NSString LegacySequenceGrabberAttribute { get; }

		[Field ("QTCaptureDeviceAVCTransportControlsAttribute")]
		NSString AVCTransportControlsAttribute { get; }
		
		[Field ("QTCaptureDeviceAVCTransportControlsPlaybackModeKey")]
		NSString AVCTransportControlsPlaybackModeKey { get; }
		
		[Field ("QTCaptureDeviceAVCTransportControlsSpeedKey")]
		NSString AVCTransportControlsSpeedKey { get; }

		[Field ("QTCaptureDeviceSuspendedAttribute")]
		NSString SuspendedAttribute { get; }
	}

	[BaseType (typeof (QTCaptureInput))]
	[DisableDefaultCtor] // crash without warning
	interface QTCaptureDeviceInput {
		[Static]
		[Export ("deviceInputWithDevice:")]
		QTCaptureDeviceInput FromDevice (QTCaptureDevice device);

		[Export ("initWithDevice:")]
		IntPtr Constructor (QTCaptureDevice device);
		
		[Export ("device")]
		QTCaptureDevice Device { get; }
	}

	[BaseType (typeof (QTCaptureOutput), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (QTCaptureFileOutputDelegate)})]
	[DisableDefaultCtor] // crash without warning
	interface QTCaptureFileOutput {
		[Export ("outputFileURL")]
		NSUrl OutputFileUrl { get; }

		[Export ("recordToOutputFileURL:")]
		void RecordToOutputFile ([NullAllowed] NSUrl url);

		[Export ("recordToOutputFileURL:bufferDestination:")]
		void RecordToOutputFile ([NullAllowed] NSUrl url, QTCaptureDestination bufferDestination);

		[Export ("isRecordingPaused")]
		bool IsRecordingPaused { get; }

		[Export ("pauseRecording")]
		void PauseRecording ();

		[Export ("resumeRecording")]
		void ResumeRecording ();

		[Export ("compressionOptionsForConnection:")]
		QTCompressionOptions GetCompressionOptions (QTCaptureConnection forConnection);

		[Export ("setCompressionOptions:forConnection:")]
		void SetCompressionOptions (QTCompressionOptions compressionOptions, QTCaptureConnection forConnection);

		[Export ("recordedDuration")]
		QTTime RecordedDuration { get; }

		[Export ("recordedFileSize")]
		UInt64 RecordedFileSize { get; }

		//Detected properties
		[Export ("maximumVideoSize")]
		CGSize MaximumVideoSize { get; set; }

		[Export ("minimumVideoFrameInterval")]
		double MinimumVideoFrameInterval { get; set; }

		[Export ("maximumRecordedDuration")]
		QTTime MaximumRecordedDuration { get; set; }

		[Export ("maximumRecordedFileSize")]
		UInt64 MaximumRecordedFileSize { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		QTCaptureFileOutputDelegate Delegate { get; set; }

	}
		
	[BaseType (typeof (NSObject), Name="QTCaptureFileOutput_Delegate")]
	[Model]
	[Protocol (IsInformal = true)]
	interface QTCaptureFileOutputDelegate {
		[Export ("captureOutput:didOutputSampleBuffer:fromConnection:"), EventArgs ("QTCaptureFileSample")]
		void DidOutputSampleBuffer (QTCaptureFileOutput captureOutput, QTSampleBuffer sampleBuffer, QTCaptureConnection connection);

		[Export ("captureOutput:willStartRecordingToOutputFileAtURL:forConnections:"), EventArgs ("QTCaptureFileUrl")]
		void WillStartRecording (QTCaptureFileOutput captureOutput, NSUrl fileUrl, QTCaptureConnection [] connections);

		[Export ("captureOutput:didStartRecordingToOutputFileAtURL:forConnections:"), EventArgs ("QTCaptureFileUrl")]
		void DidStartRecording (QTCaptureFileOutput captureOutput, NSUrl fileUrl, QTCaptureConnection [] connections);

		[Export ("captureOutput:shouldChangeOutputFileAtURL:forConnections:dueToError:"), DelegateName ("QTCaptureFileError"), DefaultValue (true)]
		bool ShouldChangeOutputFile (QTCaptureFileOutput captureOutput, NSUrl outputFileURL, QTCaptureConnection [] connections, NSError reason);

		[Export ("captureOutput:mustChangeOutputFileAtURL:forConnections:dueToError:"), EventArgs ("QTCaptureFileError")]
		void MustChangeOutputFile (QTCaptureFileOutput captureOutput, NSUrl outputFileURL, QTCaptureConnection [] connections, NSError reason);

		[Export ("captureOutput:willFinishRecordingToOutputFileAtURL:forConnections:dueToError:"), EventArgs ("QTCaptureFileError")]
		void WillFinishRecording (QTCaptureFileOutput captureOutput, NSUrl outputFileURL, QTCaptureConnection [] connections, NSError reason);

		[Export ("captureOutput:didFinishRecordingToOutputFileAtURL:forConnections:dueToError:"), EventArgs ("QTCaptureFileError")]
		void DidFinishRecording (QTCaptureFileOutput captureOutput, NSUrl outputFileURL, QTCaptureConnection [] connections, NSError reason);

		[Export ("captureOutput:didPauseRecordingToOutputFileAtURL:forConnections:"), EventArgs ("QTCaptureFileUrl")]
		void DidPauseRecording (QTCaptureFileOutput captureOutput, NSUrl fileUrl, QTCaptureConnection [] connections);

		[Export ("captureOutput:didResumeRecordingToOutputFileAtURL:forConnections:"), EventArgs ("QTCaptureFileUrl")]
		void DidResumeRecording (QTCaptureFileOutput captureOutput, NSUrl fileUrl, QTCaptureConnection [] connections);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // An uncaught exception was raised: Cannot instantiate QTCaptureInput because it is an abstract superclass.
	interface QTCaptureInput {
		[Export ("connections")]
		QTCaptureConnection [] Connections { get; }
	}

	[BaseType (typeof (CALayer))]
	interface QTCaptureLayer {
		[Static, Export ("layerWithSession:")]
		NSObject FromSession (QTCaptureSession session);

		[Export ("initWithSession:")]
		IntPtr Constructor (QTCaptureSession session);

		//Detected properties
		[Export ("session")]
		QTCaptureSession Session { get; set; }
	}

	[BaseType (typeof (QTCaptureFileOutput))]
	interface QTCaptureMovieFileOutput {
		// Empty
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // An uncaught exception was raised: Cannot instantiate QTCaptureOutput because it is an abstract superclass.
	interface QTCaptureOutput {
		[Export ("connections")]
		QTCaptureConnection [] Connections { get; }
	}
	
	[BaseType (typeof (NSObject))]
	interface QTCaptureSession {
		[Export ("inputs")]
		QTCaptureInput [] Inputs { get; }

		[Export ("addInput:error:")]
		bool AddInput (QTCaptureInput input, out NSError error);

		[Export ("removeInput:")]
		void RemoveInput (QTCaptureInput input);

		[Export ("outputs")]
		QTCaptureOutput [] Outputs { get; }

		[Export ("addOutput:error:")]
		bool AddOutput (QTCaptureOutput output, out NSError error);

		[Export ("removeOutput:")]
		void RemoveOutput (QTCaptureOutput output);

		[Export ("isRunning")]
		bool IsRunning { get; }

		[Export ("startRunning")]
		void StartRunning ();

		[Export ("stopRunning")]
		void StopRunning ();

		[Notification]
		[Field ("QTCaptureSessionRuntimeErrorNotification")]
		NSString RuntimeErrorNotification { get; }

		[Field ("QTCaptureSessionErrorKey")]
		NSString ErrorKey { get; }
	}

	[BaseType (typeof (NSView), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (QTCaptureViewDelegate)})]
	interface QTCaptureView {
		[Export ("availableVideoPreviewConnections")]
		QTCaptureConnection [] AvailableVideoPreviewConnections { get; }

		[Export ("previewBounds")]
		CGRect PreviewBounds { get; }

		//Detected properties
		[Export ("captureSession")]
		QTCaptureSession CaptureSession { get; set; }

		[Export ("videoPreviewConnection")]
		QTCaptureConnection VideoPreviewConnection { get; set; }

		[Export ("fillColor")]
		NSColor FillColor { get; set; }

		[Export ("preservesAspectRatio")]
		bool PreservesAspectRatio { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		QTCaptureViewDelegate Delegate { get; set; }
	}

	[BaseType (typeof (NSObject), Name="QTCaptureView_Delegate")]
	[Model]
	[Protocol (IsInformal = true)]
	interface QTCaptureViewDelegate {
		[Export ("view:willDisplayImage:"), DelegateName ("QTCaptureImageEvent"), DefaultValueFromArgument ("image")]
		CIImage WillDisplayImage (QTCaptureView view, CIImage image);
	}

	[BaseType (typeof (NSObject))]
	interface QTCompressionOptions {
		[Static]
		[Export ("compressionOptionsIdentifiersForMediaType:")]
		string [] GetCompressionOptionsIdentifiers (string forMediaType);

		[Static]
		[Export ("compressionOptionsWithIdentifier:")]
		NSObject FromIdentifier (string identifier);

		[Export ("mediaType")]
		string MediaType { get; }

		[Export ("localizedDisplayName")]
		string LocalizedDisplayName { get; }

		[Export ("localizedCompressionOptionsSummary")]
		string LocalizedCompressionOptionsSummary { get; }

		[Export ("isEqualToCompressionOptions:")]
		bool IsEqualToCompressionOptions (QTCompressionOptions compressionOptions);
	}

	[BaseType (typeof (NSObject))]
	interface QTDataReference : NSCoding {
		[Static]
		[Export ("dataReferenceWithDataRefData:type:")]
		NSObject FromDataRefData (NSData dataRefData, string type);

		[Static]
		[Export ("dataReferenceWithReferenceToFile:")]
		NSObject FromReference (string fileName);

		[Static]
		[Export ("dataReferenceWithReferenceToURL:")]
		NSObject FromReference (NSUrl url);

		[Static]
		[Export ("dataReferenceWithReferenceToData:")]
		NSObject FromDataReference (NSData data);

		[Static]
		[Export ("dataReferenceWithReferenceToData:name:MIMEType:")]
		NSObject FromReference (NSData data, string name, string mimeType);

		[Export ("dataRefData")]
		NSData DataRefData { get; }

		[Export ("referenceFile")]
		string ReferenceFile { get; }

		[Export ("referenceURL")]
		NSUrl ReferenceUrl { get; }

		[Export ("referenceData")]
		NSData ReferenceData { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("MIMEType")]
		string MimeType { get; }

		//Detected properties
		//[Export ("dataRef")]
		//IntPtr DataRef { get; set; }

		[Export ("dataRefType")]
		string DataRefType { get; set; }
	}

	[Static]
	interface QTErrorKey {
		[Field ("QTKitErrorDomain")]
		NSString Domain { get; }

		[Field ("QTErrorCaptureInputKey")]
		NSString CaptureInput { get; }

		[Field ("QTErrorCaptureOutputKey")]
		NSString CaptureOutput { get; }

		[Field ("QTErrorDeviceKey")]
		NSString Device { get; }

		[Field ("QTErrorExcludingDeviceKey")]
		NSString ExcludingDevice { get; }

		[Field ("QTErrorTimeKey")]
		NSString Time { get; }

		[Field ("QTErrorFileSizeKey")]
		NSString FileSize { get; }

		[Field ("QTErrorRecordingSuccesfullyFinishedKey")]
		NSString RecordingSuccesfullyFinished { get; }

//		[Field ("QTErrorRecordingSuccessfullyFinishedKey")]
//		NSString RecordingSuccessfullyFinished { get; }
	}
	
	[BaseType (typeof (NSObject))]
	interface QTFormatDescription {
		[Export ("mediaType")]
		string MediaType { get; }

		[Export ("formatType")]
		UInt32 FormatType { get; }

		[Export ("localizedFormatSummary")]
		string LocalizedFormatSummary { get; }

		[Export ("quickTimeSampleDescription")]
		NSData QuickTimeSampleDescription { get; }

		[Export ("formatDescriptionAttributes")]
		NSDictionary FormatDescriptionAttributes { get; }

		[Export ("attributeForKey:")]
		NSObject AttributeForKey (string key);

		[Export ("isEqualToFormatDescription:")]
		bool IsEqualToFormatDescription (QTFormatDescription formatDescription);

	}

	[BaseType (typeof (NSObject))]
	interface QTMedia {
		[Mac (10, 3, 0, PlatformArchitecture.Arch32)] 
		[Deprecated (PlatformName.MacOSX, 10, 9)] 
		[Static, Export ("mediaWithQuickTimeMedia:error:")]
		NSObject FromQuickTimeMedia (IntPtr quicktimeMedia, out NSError error);

#if !XAMCORE_3_0
		[Mac (10, 3, 0, PlatformArchitecture.Arch32)] 
		[Deprecated (PlatformName.MacOSX, 10, 9)] 
		[Export ("initWithQuickTimeMedia:error:")]
		IntPtr Conditions (IntPtr quicktimeMedia, out NSError error);
#endif
		[Sealed] // For the duplicate selector error
		[Mac (10, 3, 0, PlatformArchitecture.Arch32)] 
		[Deprecated (PlatformName.MacOSX, 10, 9)] 
		[Export ("initWithQuickTimeMedia:error:")]
		IntPtr Constructors (IntPtr quicktimeMedia, out NSError error);

		[Export ("track")]
		QTTrack Track { get; }

		[Export ("attributeForKey:")]
		NSObject GetAttribute (string attributeKey);

		[Export ("setAttribute:forKey:")]
		void SetAttribute (NSObject value, string attributeKey);

		[Export ("hasCharacteristic:")]
		bool HasCharacteristic (string characteristic);

		[Mac (10, 3, 0, PlatformArchitecture.Arch32)] 
		[Deprecated (PlatformName.MacOSX, 10, 9)] 
		[Export ("quickTimeMedia")]
		IntPtr QuickTimeMedia { get; }

		//Detected properties
		[Export ("mediaAttributes")]
		NSDictionary MediaAttributes { get; set; }

		// Constants
		[Internal, Field ("QTMediaTypeVideo")]
		NSString TypeVideo { get; }

		[Internal, Field ("QTMediaTypeSound")]
		NSString TypeSound { get; }

		[Internal, Field ("QTMediaTypeText")]
		NSString TypeText { get; }

		[Internal, Field ("QTMediaTypeBase")]
		NSString TypeBase { get; }

		[Internal, Field ("QTMediaTypeMPEG")]
		NSString TypeMpeg { get; }

		[Internal, Field ("QTMediaTypeMusic")]
		NSString TypeMusic { get; }

		[Internal, Field ("QTMediaTypeTimeCode")]
		NSString TypeTimeCode { get; }

		[Internal, Field ("QTMediaTypeSprite")]
		NSString TypeSprite { get; }

		[Internal, Field ("QTMediaTypeFlash")]
		NSString TypeFlash { get; }

		[Internal, Field ("QTMediaTypeMovie")]
		NSString TypeMovie { get; }

		[Internal, Field ("QTMediaTypeTween")]
		NSString TypeTween { get; }

		[Internal, Field ("QTMediaType3D")]
		NSString Type3D { get; }

		[Internal, Field ("QTMediaTypeSkin")]
		NSString TypeSkin { get; }

		[Internal, Field ("QTMediaTypeQTVR")]
		NSString TypeQTVR { get; }

		[Internal, Field ("QTMediaTypeHint")]
		NSString TypeHint { get; }

		[Internal, Field ("QTMediaTypeStream")]
		NSString TypeStream { get; }

		[Internal, Field ("QTMediaTypeMuxed")]
		NSString TypeMuxed { get; }

		[Internal, Field ("QTMediaTypeQuartzComposer")]
		NSString TypeQuartzComposer { get; }

		[Field ("QTMediaCharacteristicVisual")]
		NSString CharacteristicVisual { get; }

		[Field ("QTMediaCharacteristicAudio")]
		NSString CharacteristicAudio { get; }

		[Field ("QTMediaCharacteristicCanSendVideo")]
		NSString CharacteristicCanSendVideo { get; }

		[Field ("QTMediaCharacteristicProvidesActions")]
		NSString CharacteristicProvidesActions { get; }

		[Field ("QTMediaCharacteristicNonLinear")]
		NSString CharacteristicNonLinear { get; }

		[Field ("QTMediaCharacteristicCanStep")]
		NSString CharacteristicCanStep { get; }

		[Field ("QTMediaCharacteristicHasNoDuration")]
		NSString CharacteristicHasNoDuration { get; }

		[Field ("QTMediaCharacteristicHasSkinData")]
		NSString CharacteristicHasSkinData { get; }

		[Field ("QTMediaCharacteristicProvidesKeyFocus")]
		NSString CharacteristicProvidesKeyFocus { get; }

		[Field ("QTMediaCharacteristicHasVideoFrameRate")]
		NSString CharacteristicHasVideoFrameRate { get; }

		[Field ("QTMediaCreationTimeAttribute")]
		NSString CreationTimeAttribute { get; }

		[Field ("QTMediaDurationAttribute")]
		NSString DurationAttribute { get; }

		[Field ("QTMediaModificationTimeAttribute")]
		NSString ModificationTimeAttribute { get; }

		[Field ("QTMediaSampleCountAttribute")]
		NSString SampleCountAttribute { get; }

		[Field ("QTMediaQualityAttribute")]
		NSString QualityAttribute { get; }

		[Field ("QTMediaTimeScaleAttribute")]
		NSString TimeScaleAttribute { get; }

		[Field ("QTMediaTypeAttribute")]
		NSString TypeAttribute { get; }
	}

	[BaseType (typeof (CALayer))]
	interface QTMovieLayer {
		[Static, Export ("layerWithMovie:")]
		QTMovieLayer FromMovie (QTMovie movie);

		[Export ("initWithMovie:")]
		IntPtr Constructor (QTMovie movie);

		//Detected properties
		[Export ("movie")]
		QTMovie Movie { get; set; }
	}

	[BaseType (typeof (NSView))]
	interface QTMovieView {

		[Export ("movie")]
		QTMovie Movie { get; set; }
		
		[Export ("isControllerVisible")]
		bool IsControllerVisible { get; [Bind("setControllerVisible:")] set; }
		
		[Export ("isEditable")]
		bool Editable { get; [Bind("setEditable:")] set; }

		[Export ("controllerBarHeight")]
		float ControllerBarHeight { get; } /* float, not CGFloat */
		
		[Export ("preservesAspectRatio")]
		bool PreservesAspectRatio { get; set; }
		
		[Export ("fillColor")]
		NSColor FillColor { get; set; }
			
		[Export ("movieBounds")]
		CGRect MovieBounds { get; }

		[Export ("movieControllerBounds")]
		CGRect MovieControllerBounds { get; }
		
		[Export ("setShowsResizeIndicator:")]
		void SetShowsResizeIndicator (bool show);
		
		[Export ("play:")]
		void Play (NSObject sender);

		[Export ("pause:")]
		void Pause (NSObject sender);

		[Export ("gotoBeginning:")]
		void GotoBeginning (NSObject sender);

		[Export ("gotoEnd:")]
		void GotoEnd (NSObject sender);

		[Export ("gotoNextSelectionPoint:")]
		void GotoNextSelectionPoint (NSObject sender);

		[Export ("gotoPreviousSelectionPoint:")]
		void GotoPreviousSelectionPoint (NSObject sender);

		[Export ("gotoPosterFrame:")]
		void GotoPosterFrame (NSObject sender);

		[Export ("stepForward:")]
		void StepForward (NSObject sender);

		[Export ("stepBackward:")]
		void StepBackward (NSObject sender);

		[Export ("cut:")]
		void Cut (NSObject sender);

		[Export ("copy:")]
		void Copy (NSObject sender);

		[Export ("paste:")]
		void Paste (NSObject sender);

		[Export ("selectAll:")]
		void SelectAll (NSObject sender);

		[Export ("selectNone:")]
		void SelectNone (NSObject sender);

		[Export ("delete:")]
		void Delete (NSObject sender);

		[Export ("add:")]
		void Add (NSObject sender);

		[Export ("addScaled:")]
		void AddScaled (NSObject sender);

		[Export ("replace:")]
		void Replace (NSObject sender);

		[Export ("trim:")]
		void Trim (NSObject sender);
		
		[Export ("backButtonVisible")]
		bool BackButtonVisible { [Bind ("isBackButtonVisible")] get; set; }

		[Export ("customButtonVisible")]
		bool CustomButtonVisible { [Bind ("isCustomButtonVisible")] get; set; }

		[Export ("hotSpotButtonVisible")]
		bool HotSpotButtonVisible { [Bind ("isHotSpotButtonVisible")] get; set; }

		[Export ("stepButtonsVisible")]
		bool SetStepButtonsVisible { [Bind ("areStepButtonsVisible")] get; set; }

		[Export ("translateButtonVisible")]
		bool TranslateButtonVisible { [Bind ("isTranslateButtonVisible")] get; set; }

		[Export ("volumeButtonVisible")]
		bool VolumeButtonVisible { [Bind ("isVolumeButtonVisible")] get; set; }

		[Export ("zoomButtonsVisible")]
		bool ZoomButtonsVisible { [Bind ("areZoomButtonsVisible")] get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")]
		[Protocolize]
		QTMovieViewDelegate Delegate { get; set; }
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface QTMovieViewDelegate {
		[Export ("view:willDisplayImage:")]
		CIImage ViewWillDisplayImage (QTMovieView view, CIImage image);
	}
	
	[BaseType (typeof (NSObject))]
	interface QTMovie : NSCoding, NSCopying {
		[Export ("duration")]
		QTTime Duration { get; }

		[Static, Export ("canInitWithPasteboard:")]
		bool CanInitWithPasteboard (NSPasteboard pasteboard);

		[Static, Export ("canInitWithFile:")]
		bool CanInitWithFile (string fileName);

		[Static, Export ("canInitWithURL:")]
		bool CanInitWithUrl (NSUrl url);

		//[Static, Export ("canInitWithDataReference:")]
		//bool CanInitWithDataReference (QTDataReference dataReference);

		[Static, Export ("movieFileTypes:")]
		string[] MovieFileTypes (QTMovieFileTypeOptions types);

		[Static, Export ("movieUnfilteredFileTypes")]
		string[] MovieUnfilteredFileTypes ();

		//+ (NSArray *)movieUnfilteredPasteboardTypes;
		[Static, Export ("movieUnfilteredPasteboardTypes")]
		string[] MovieUnfilteredPasteboardTypes ();

		[Static, Export ("movieTypesWithOptions:")]
		string[] MovieTypesWithOptions (QTMovieFileTypeOptions types);

		[Static, Export ("movie")]
		QTMovie Movie { get; }

		[Static, Export ("movieWithFile:error:")]
		QTMovie FromFile (string fileName, out NSError error);

		[Static, Export ("movieWithURL:error:")]
		QTMovie FromUrl (NSUrl url, out NSError error);

		//[Static, Export ("movieWithDataReference:error:")]
		//QTMovie MovieWithDataReferenceError (QTDataReference dataReference, out NSError error);

		[Static, Export ("movieWithPasteboard:error:")]
		QTMovie FromPasteboard (NSPasteboard pasteboard, out NSError error);

		[Static, Export ("movieWithData:error:")]
		QTMovie FromData (NSData data, out NSError error);

//		[Static, Export ("movieWithQuickTimeMovie:disposeWhenDone:error:")]
//		QTMovie MovieWithQuickTimeMovieDisposeWhenDone (Movie movie, bool dispose, out NSError error);

		[Static, Export ("movieWithAttributes:error:")]
		QTMovie FromAttributes (NSDictionary attributes, out NSError error);

		[Static, Export ("movieNamed:error:")]
		QTMovie FromMovieNamed (string name, out NSError error);

		[Export ("initWithFile:error:")]
		IntPtr Constructor (string fileName, out NSError error);

		[Export ("initWithURL:error:")]
		IntPtr Constructor (NSUrl url, out NSError error);

		[Export ("initWithDataReference:error:")]
		IntPtr Constructor (QTDataReference dataReference, out NSError error);

		[Export ("initWithPasteboard:error:")]
		IntPtr Constructor (NSPasteboard pasteboard, out NSError error);

		[Export ("initWithData:error:")]
		IntPtr Constructor (NSData data, out NSError error);

		[Export ("initWithMovie:timeRange:error:")]
		IntPtr Constructor (QTMovie movie, QTTimeRange range, out NSError error);

		//- (id)initWithQuickTimeMovie:(Movie)movie disposeWhenDone:(BOOL)dispose error:(NSError **)errorPtr;
//		[Export ("initWithQuickTimeMovie:disposeWhenDone:error:")]
//		IntPtr Constructor ([Movie movie, bool dispose, out NSError error);

		[Export ("initWithAttributes:error:")]
		IntPtr Constructor (NSDictionary attributes, out NSError error);

#if !XAMCORE_3_0
		[Obsolete ("Use the 'MoveWithTimeRange' method instead.")]
		[Export ("movieWithTimeRange:error:")]
		IntPtr Constructor (QTTimeRange range, out NSError error);
#endif

		[Sealed] // This is required because otherwise we'll have two methods for 'movieWithTimeRange:error:'
		[Export ("movieWithTimeRange:error:")]
		QTMovie MovieWithTimeRange (QTTimeRange range, out NSError error);

//		[Export ("initToWritableFile:error:")]
//		IntPtr Constructor (string filename, out NSError error);

		[Export ("initToWritableData:error:")]
		IntPtr Constructor (NSMutableData data, out NSError error);

		//- (id)initToWritableDataReference:(QTDataReference *)dataReference error:(NSError **)errorPtr;
//		[Export ("initToWritableDataReference:error:")]
//		IntPtr Constructor (QTDataReference dataReference, out NSError error);

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("currentTime")]
		QTTime CurrentTime { get; set; }

		[Export ("rate")]
		float Rate { get; set; } /* float, not CGFloat */

		[Export ("volume")]
		float Volume { get; set; } /* float, not CGFloat */

		[Export ("muted")]
		bool Muted { get; set; }
		
		[Export ("movieAttributes")]
		NSDictionary MovieAttributes { get; set; }

		[Export ("attributeForKey:")]
		NSObject GetAttribute (string attributeKey);

		[Export ("setAttribute:forKey:")]
		void SetAttribute (NSObject value, string attributeKey);

		[Export ("tracks")]
		QTTrack[] Tracks { get; }

		[Export ("tracksOfMediaType:")]
		QTTrack[] TracksOfMediaType (string type);

		[Export ("posterImage")]
		NSImage PosterImage { get; }

		[Export ("currentFrameImage")]
		NSImage CurrentFrameImage { get; }

		[Export ("frameImageAtTime:")]
		NSImage FrameImageAtTime (QTTime time);

		[Export ("frameImageAtTime:withAttributes:error:")]
		IntPtr FrameImageAtTime (QTTime time, NSDictionary attributes, out NSError error);

		[Export ("movieFormatRepresentation")]
		NSData MovieFormatRepresentation ();

		[Export ("writeToFile:withAttributes:")]
		bool SaveTo (string fileName, NSDictionary attributes);
		
		[Export ("writeToFile:withAttributes:error:")]
		bool SaveTo (string fileName, NSDictionary attributes, out NSError error);

		[Export ("canUpdateMovieFile")]
		bool CanUpdateMovieFile { get; }

		[Export ("updateMovieFile")]
		bool UpdateMovieFile ();

		[Export ("autoplay")]
		void Autoplay ();

		[Export ("play")]
		void Play ();

		[Export ("stop")]
		void Stop ();

		[Export ("gotoBeginning")]
		void GotoBeginning ();

		[Export ("gotoEnd")]
		void GotoEnd ();

		[Export ("gotoNextSelectionPoint")]
		void GotoNextSelectionPoint ();

		[Export ("gotoPreviousSelectionPoint")]
		void GotoPreviousSelectionPoint ();

		[Export ("gotoPosterTime")]
		void GotoPosterTime ();

		[Export ("stepForward")]
		void StepForward ();

		[Export ("stepBackward")]
		void StepBackward ();

		[Export ("setSelection:")]
		void SetSelection (QTTimeRange selection);

		[Export ("selectionStart")]
		QTTime SelectionStart ();

		[Export ("selectionEnd")]
		QTTime SelectionEnd ();

		[Export ("selectionDuration")]
		QTTime SelectionDuration ();

		[Export ("replaceSelectionWithSelectionFromMovie:")]
		void ReplaceSelectionWithSelectionFromMovie (QTMovie movie);

		[Export ("appendSelectionFromMovie:")]
		void AppendSelectionFromMovie (QTMovie movie);

		[Export ("insertSegmentOfMovie:timeRange:atTime:")]
		void InsertSegmentOfMovieTimeRange (QTMovie movie, QTTimeRange range, QTTime time);

		[Export ("insertSegmentOfMovie:fromRange:scaledToRange:")]
		void InsertSegmentOfMovieFromRange (QTMovie movie, QTTimeRange srcRange, QTTimeRange dstRange);

		[Export ("insertEmptySegmentAt:")]
		void InsertEmptySegmentAt (QTTimeRange range);

		[Export ("deleteSegment:")]
		void DeleteSegment (QTTimeRange segment);

		[Export ("scaleSegment:newDuration:")]
		void ScaleSegmentNewDuration (QTTimeRange segment, QTTime newDuration);

		[Export ("addImage:forDuration:withAttributes:")]
		void AddImage (NSImage image, QTTime duration, NSDictionary attributes);

		[Export ("insertSegmentOfTrack:timeRange:atTime:")]
		QTTrack InsertSegmentOfTrackTimeRange (QTTrack track, QTTimeRange range, QTTime time);

		[Export ("insertSegmentOfTrack:fromRange:scaledToRange:")]
		QTTrack InsertSegmentOfTrackFromRange (QTTrack track, QTTimeRange srcRange, QTTimeRange dstRange);

		[Export ("removeTrack:")]
		void RemoveTrack (QTTrack track);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		//[Wrap ("WeakDelegate")]
		//QTMovieDelegate Delegate { get; set; }

		//- (Movie)quickTimeMovie;
//		[Export ("quickTimeMovie")]
//		Movie QuickTimeMovie ();

		//- (MovieController)quickTimeMovieController;
//		[Export ("quickTimeMovieController")]
//		MovieController QuickTimeMovieController ();

		//- (void)generateApertureModeDimensions;
		[Export ("generateApertureModeDimensions")]
		void GenerateApertureModeDimensions ();

		//- (void)removeApertureModeDimensions;
		[Export ("removeApertureModeDimensions")]
		void RemoveApertureModeDimensions ();

		//- (QTVisualContextRef)visualContext;
//		[Export ("visualContext")]
//		QTVisualContextRef VisualContext ();

		[Static, Export ("enterQTKitOnThread")]
		void EnterQTKitOnThread ();

		[Static, Export ("enterQTKitOnThreadDisablingThreadSafetyProtection")]
		void EnterQTKitOnThreadDisablingThreadSafetyProtection ();

		[Static, Export ("exitQTKitOnThread")]
		void ExitQTKitOnThread ();

		[Export ("attachToCurrentThread")]
		bool AttachToCurrentThread ();

		[Export ("detachFromCurrentThread")]
		bool DetachFromCurrentThread ();

		[Export ("isIdling")]
		bool Idling { get; }

		[Export ("hasChapters")]
		bool HasChapters { get; }

		[Export ("chapterCount")]
		nint ChapterCount { get; }

		[Export ("chapters")]
		NSDictionary[] Chapters ();

//		[Export ("addChapters:withAttributes:error:")]
//		void AddChaptersWithAttributes (NSArray chapters, NSDictionary attributes, out NSError error);

		[Export ("removeChapters")]
		bool RemoveChapters ();

		[Export ("startTimeOfChapter:")]
		QTTime StartTimeOfChapter (nint chapterIndex);

		[Export ("chapterIndexForTime:")]
		nint ChapterIndexForTime (QTTime time);

		//
		// Pasteboard type
		//
		[Field ("QTMoviePasteboardType")]
		NSString PasteboardType { get; }

		//
		// Notifications
		//


		[Notification]
		[Field ("QTMovieEditabilityDidChangeNotification")]
		NSString EditabilityDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieEditedNotification")]
		NSString EditedNotification { get; }

		[Notification]
		[Field ("QTMovieLoadStateDidChangeNotification")]
		NSString LoadStateDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieLoopModeDidChangeNotification")]
		NSString LoopModeDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieMessageStringPostedNotification")]
		NSString MessageStringPostedNotification { get; }

		[Notification]
		[Field ("QTMovieRateDidChangeNotification")]
		NSString RateDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieSelectionDidChangeNotification")]
		NSString SelectionDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieSizeDidChangeNotification")]
		NSString SizeDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieStatusStringPostedNotification")]
		NSString StatusStringPostedNotification { get; }


		[Notification]
		[Field ("QTMovieTimeDidChangeNotification")]
		NSString TimeDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieVolumeDidChangeNotification")]
		NSString VolumeDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieDidEndNotification")]
		NSString DidEndNotification { get; }

		[Notification]
		[Field ("QTMovieChapterDidChangeNotification")]
		NSString ChapterDidChangeNotification { get; }

		[Notification]
		[Field ("QTMovieChapterListDidChangeNotification")]
		NSString ChapterListDidChangeNotification { get; }


		[Notification]
		[Field ("QTMovieEnterFullScreenRequestNotification")]
		NSString EnterFullScreenRequestNotification { get; }

		[Notification]
		[Field ("QTMovieExitFullScreenRequestNotification")]
		NSString ExitFullScreenRequestNotification { get; }

		[Notification]
		[Field ("QTMovieCloseWindowRequestNotification")]
		NSString CloseWindowRequestNotification { get; }

		[Notification]
		[Field ("QTMovieApertureModeDidChangeNotification")]
		NSString ApertureModeDidChangeNotification { get; }

		// Notification parameters
		[Field ("QTMovieMessageNotificationParameter")]
		NSString MessageNotificationParameter { get; }
		[Field ("QTMovieRateDidChangeNotificationParameter")]
		NSString RateDidChangeNotificationParameter { get; }
		[Field ("QTMovieStatusFlagsNotificationParameter")]
		NSString StatusFlagsNotificationParameter { get; }
		[Field ("QTMovieStatusCodeNotificationParameter")]
		NSString StatusCodeNotificationParameter { get; }
		[Field ("QTMovieStatusStringNotificationParameter")]
		NSString StatusStringNotificationParameter { get; }

		[Field ("QTMovieTargetIDNotificationParameter")]
		NSString TargetIDNotificationParameter { get; }
		[Field ("QTMovieTargetNameNotificationParameter")]
		NSString TargetNameNotificationParameter { get; }

		// WriteToFile parameters
		[Internal, Field ("QTMovieExport")]      		// NSNumber Bool
		NSString KeyExport { get; }
		[Internal, Field ("QTMovieExportType")]			// NSNumber long
		NSString KeyExportType { get; }
		[Internal, Field ("QTMovieFlatten")]			// NSNumber bool
		NSString KeyFlatten { get; }
		[Internal, Field ("QTMovieExportSettings")]		// NSData (QTAtomContainer)
		NSString KeyExportSettings { get; }
		[Internal, Field ("QTMovieExportManufacturer")]		// NSNumber (long)
		NSString KeyExportManufacturer { get; }

		//
		// Add Image
		//
		[Internal, Field ("QTAddImageCodecType")]		 // nsstring
		NSString ImageCodecType { get; }
		[Internal, Field ("QTAddImageCodecQuality")]	// nsnumber
		NSString ImageCodecQuality { get; }

		// data locators for FromAttributes
		[Field ("QTMovieDataReferenceAttribute")]
		NSString DataReferenceAttribute { get; }
		[Field ("QTMoviePasteboardAttribute")]
		NSString PasteboardAttribute { get; }
		[Field ("QTMovieDataAttribute")]
		NSString DataAttribute { get; }

		// Instantiation options
		[Field ("QTMovieFileOffsetAttribute")]
		NSString FileOffsetAttribute { get; }
		[Field ("QTMovieResolveDataRefsAttribute")]
		NSString ResolveDataRefsAttribute { get; }
		[Field ("QTMovieAskUnresolvedDataRefsAttribute")]
		NSString AskUnresolvedDataRefsAttribute { get; }
		[Field ("QTMovieOpenAsyncOKAttribute")]
		NSString OpenAsyncOKAttribute { get; }

		// movie attributes
		[Field ("QTMovieApertureModeAttribute")]
		NSString ApertureModeAttribute { get; }
		[Field ("QTMovieActiveSegmentAttribute")]
		NSString ActiveSegmentAttribute { get; }
		[Field ("QTMovieAutoAlternatesAttribute")]
		NSString AutoAlternatesAttribute { get; }
		[Field ("QTMovieCopyrightAttribute")]
		NSString CopyrightAttribute { get; }
		[Field ("QTMovieCreationTimeAttribute")]
		NSString CreationTimeAttribute { get; }
		[Field ("QTMovieCurrentSizeAttribute")]
		NSString CurrentSizeAttribute { get; }
		[Field ("QTMovieCurrentTimeAttribute")]
		NSString CurrentTimeAttribute { get; }
		[Field ("QTMovieDataSizeAttribute")]
		NSString DataSizeAttribute { get; }
		[Field ("QTMovieDelegateAttribute")]
		NSString DelegateAttribute { get; }
		[Field ("QTMovieDisplayNameAttribute")]
		NSString DisplayNameAttribute { get; }
		[Field ("QTMovieDontInteractWithUserAttribute")]
		NSString DontInteractWithUserAttribute { get; }
		[Field ("QTMovieDurationAttribute")]
		NSString DurationAttribute { get; }
		[Field ("QTMovieEditableAttribute")]
		NSString EditableAttribute { get; }
		[Field ("QTMovieFileNameAttribute")]
		NSString FileNameAttribute { get; }
		[Field ("QTMovieHasApertureModeDimensionsAttribute")]
		NSString HasApertureModeDimensionsAttribute { get; }
		[Field ("QTMovieHasAudioAttribute")]
		NSString HasAudioAttribute { get; }
		[Field ("QTMovieHasDurationAttribute")]
		NSString HasDurationAttribute { get; }
		[Field ("QTMovieHasVideoAttribute")]
		NSString HasVideoAttribute { get; }
		[Field ("QTMovieIsActiveAttribute")]
		NSString IsActiveAttribute { get; }
		[Field ("QTMovieIsInteractiveAttribute")]
		NSString IsInteractiveAttribute { get; }
		[Field ("QTMovieIsLinearAttribute")]
		NSString IsLinearAttribute { get; }
		[Field ("QTMovieIsSteppableAttribute")]
		NSString IsSteppableAttribute { get; }
		[Field ("QTMovieLoadStateAttribute")]
		NSString LoadStateAttribute { get; }
		[Field ("QTMovieLoopsAttribute")]
		NSString LoopsAttribute { get; }
		[Field ("QTMovieLoopsBackAndForthAttribute")]
		NSString LoopsBackAndForthAttribute { get; }
		[Field ("QTMovieModificationTimeAttribute")]
		NSString ModificationTimeAttribute { get; }
		[Field ("QTMovieMutedAttribute")]
		NSString MutedAttribute { get; }
		[Field ("QTMovieNaturalSizeAttribute")]
		NSString NaturalSizeAttribute { get; }
		[Field ("QTMoviePlaysAllFramesAttribute")]
		NSString PlaysAllFramesAttribute { get; }
		[Field ("QTMoviePlaysSelectionOnlyAttribute")]
		NSString PlaysSelectionOnlyAttribute { get; }
		[Field ("QTMoviePosterTimeAttribute")]
		NSString PosterTimeAttribute { get; }
		[Field ("QTMoviePreferredMutedAttribute")]
		NSString PreferredMutedAttribute { get; }
		[Field ("QTMoviePreferredRateAttribute")]
		NSString PreferredRateAttribute { get; }
		[Field ("QTMoviePreferredVolumeAttribute")]
		NSString PreferredVolumeAttribute { get; }
		[Field ("QTMoviePreviewModeAttribute")]
		NSString PreviewModeAttribute { get; }
		[Field ("QTMoviePreviewRangeAttribute")]
		NSString PreviewRangeAttribute { get; }
		[Field ("QTMovieRateAttribute")]
		NSString RateAttribute { get; }
		[Field ("QTMovieSelectionAttribute")]
		NSString SelectionAttribute { get; }
		[Field ("QTMovieTimeScaleAttribute")]
		NSString TimeScaleAttribute { get; }
		[Field ("QTMovieURLAttribute")]
		NSString URLAttribute { get; }
		[Field ("QTMovieVolumeAttribute")]
		NSString VolumeAttribute { get; }
		[Field ("QTMovieRateChangesPreservePitchAttribute")]
		NSString RateChangesPreservePitchAttribute { get; }


		[Field ("QTMovieApertureModeClassic")]
		NSString ApertureModeClassic { get; }
		[Field ("QTMovieApertureModeClean")]
		NSString ApertureModeClean { get; }
		[Field ("QTMovieApertureModeProduction")]
		NSString ApertureModeProduction { get; }
		[Field ("QTMovieApertureModeEncodedPixels")]
		NSString ApertureModeEncodedPixels { get; }

		[Field ("QTMovieFrameImageSize")]
		NSString FrameImageSize { get; }
		[Field ("QTMovieFrameImageType")]
		NSString FrameImageType { get; }
		[Field ("QTMovieFrameImageTypeNSImage")]
		NSString FrameImageTypeNSImage { get; }
		[Field ("QTMovieFrameImageTypeCGImageRef")]
		NSString FrameImageTypeCGImageRef { get; }
		[Field ("QTMovieFrameImageTypeCIImage")]
		NSString FrameImageTypeCIImage { get; }
		[Field ("QTMovieFrameImageTypeCVPixelBufferRef")]
		NSString FrameImageTypeCVPixelBufferRef { get; }
		[Field ("QTMovieFrameImageTypeCVOpenGLTextureRef")]
		NSString FrameImageTypeCVOpenGLTextureRef { get; }
		[Field ("QTMovieFrameImageOpenGLContext")]
		NSString FrameImageOpenGLContext { get; }
		[Field ("QTMovieFrameImagePixelFormat")]
		NSString FrameImagePixelFormat { get; }
		[Field ("QTMovieFrameImageRepresentationsType")]
		NSString FrameImageRepresentationsType { get; }
		[Field ("QTMovieFrameImageDeinterlaceFields")]
		NSString FrameImageDeinterlaceFields { get; }
		[Field ("QTMovieFrameImageHighQuality")]
		NSString FrameImageHighQuality { get; }
		[Field ("QTMovieFrameImageSingleField")]
		NSString FrameImageSingleField { get; }


		[Field ("QTMovieUneditableException")]
		NSString UneditableException { get; }

		[Field ("QTMovieChapterName")]
		NSString ChapterName { get; }
		[Field ("QTMovieChapterStartTime")]
		NSString ChapterStartTime { get; }

		[Field ("QTMovieChapterTargetTrackAttribute")]
		NSString ChapterTargetTrackAttribute { get; }
	}

	
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // invalid handle returned
	interface QTSampleBuffer {
		[Export ("bytesForAllSamples")]
		IntPtr BytesForAllSamples { get; }

		[Export ("lengthForAllSamples")]
		nuint LengthForAllSamples { get; }

		[Export ("formatDescription")]
		QTFormatDescription FormatDescription { get; }

		[Export ("duration")]
		QTTime Duration { get; }

		[Export ("decodeTime")]
		QTTime DecodeTime { get; }

		[Export ("presentationTime")]
		QTTime PresentationTime { get; }

		[Export ("numberOfSamples")]
		nint SampleCount { get; }

		[Export ("sampleBufferAttributes")]
		NSDictionary SampleBufferAttributes { get; }

		[Export ("attributeForKey:")]
		NSObject GetAttribute (string key);

		[Export ("sampleUseCount")]
		nint SampleUseCount { get; }
		
		[Export ("incrementSampleUseCount")]
		void IncrementSampleUseCount ();

		[Export ("decrementSampleUseCount")]
		void DecrementSampleUseCount ();

		//[Export ("audioBufferListWithOptions:")]
		//AudioBufferList AudioBufferListWithOptions (QTSampleBufferAudioBufferListOptions options);

		//[Export ("getAudioStreamPacketDescriptions:inRange:")]
		//bool GetAudioStreamPacketDescriptionsinRange (AudioStreamPacketDescription audioStreamPacketDescriptions, NSRange range);
	}

	[BaseType (typeof (NSObject))]
	interface QTTrack {
		[Mac (10, 3, 0, PlatformArchitecture.Arch32)] 
		[Deprecated (PlatformName.MacOSX, 10, 9)] 
		[Static, Export ("trackWithQuickTimeTrack:error:")]
		NSObject FromQuickTimeTrack (IntPtr quicktimeTrack, out NSError error);

		[Mac (10, 3, 0, PlatformArchitecture.Arch32)] 
		[Deprecated (PlatformName.MacOSX, 10, 9)] 
		[Export ("initWithQuickTimeTrack:error:")]
		IntPtr Constructor (IntPtr quicktimeTrack, out NSError error);

		[Export ("movie")]
		QTMovie Movie { get; }

		[Export ("media")]
		QTMedia Media { get; }

		[Export ("attributeForKey:")]
		NSObject GetAttribute (string attributeKey);

		[Export ("setAttribute:forKey:")]
		void SetAttribute (NSObject value, string attributeKey);

		[Mac (10, 3, 0, PlatformArchitecture.Arch32)] 
		[Deprecated (PlatformName.MacOSX, 10, 9)] 
		[Export ("quickTimeTrack")]
		IntPtr QuickTimeTrack { get; }

		[Export ("insertSegmentOfTrack:timeRange:atTime:")]
		void InsertSegmentOfTrack (QTTrack track, QTTimeRange timeRange, QTTime atTime);

		[Export ("insertSegmentOfTrack:fromRange:scaledToRange:")]
		void InsertSegmentOfTrack (QTTrack track, QTTimeRange fromRange, QTTimeRange scaledToRange);

		[Export ("insertEmptySegmentAt:")]
		void InsertEmptySegment (QTTimeRange range);

		[Export ("deleteSegment:")]
		void DeleteSegment (QTTimeRange segment);

		[Export ("scaleSegment:newDuration:")]
		void ScaleSegmentnewDuration (QTTimeRange segment, QTTime newDuration);

		[Export ("addImage:forDuration:withAttributes:")]
		void AddImage (NSImage image, QTTime forDuration, NSDictionary attributes);

		//Detected properties
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")]get; set; }

		[Export ("volume")]
		float Volume { get; set; } /* float, not CGFloat */

		[Export ("trackAttributes")]
		NSDictionary TrackAttributes { get; set; }

		[Export ("apertureModeDimensionsForMode:")]
		CGSize ApertureModeDimensionsForMode (string mode);

		[Export ("setApertureModeDimensions:forMode:")]
		void SetApertureModeDimensionsforMode (CGSize dimensions, string mode);

		[Export ("generateApertureModeDimensions")]
		void GenerateApertureModeDimensions ();

		[Export ("removeApertureModeDimensions")]
		void RemoveApertureModeDimensions ();

		[Field ("QTTrackBoundsAttribute")]
		NSString BoundsAttribute { get; }

		[Field ("QTTrackCreationTimeAttribute")]
		NSString CreationTimeAttribute { get; }

		[Field ("QTTrackDimensionsAttribute")]
		NSString DimensionsAttribute { get; }

		[Field ("QTTrackDisplayNameAttribute")]
		NSString DisplayNameAttribute { get; }

		[Field ("QTTrackEnabledAttribute")]
		NSString EnabledAttribute { get; }

		[Field ("QTTrackFormatSummaryAttribute")]
		NSString FormatSummaryAttribute { get; }

		[Field ("QTTrackIsChapterTrackAttribute")]
		NSString IsChapterTrackAttribute { get; }

		[Field ("QTTrackHasApertureModeDimensionsAttribute")]
		NSString HasApertureModeDimensionsAttribute { get; }

		[Field ("QTTrackIDAttribute")]
		NSString IDAttribute { get; }

		[Field ("QTTrackLayerAttribute")]
		NSString LayerAttribute { get; }

		[Field ("QTTrackMediaTypeAttribute")]
		NSString MediaTypeAttribute { get; }

		[Field ("QTTrackModificationTimeAttribute")]
		NSString ModificationTimeAttribute { get; }

		[Field ("QTTrackRangeAttribute")]
		NSString RangeAttribute { get; }

		[Field ("QTTrackTimeScaleAttribute")]
		NSString TimeScaleAttribute { get; }

		[Field ("QTTrackUsageInMovieAttribute")]
		NSString UsageInMovieAttribute { get; }

		[Field ("QTTrackUsageInPosterAttribute")]
		NSString UsageInPosterAttribute { get; }

		[Field ("QTTrackUsageInPreviewAttribute")]
		NSString UsageInPreviewAttribute { get; }

		[Field ("QTTrackVolumeAttribute")]
		NSString VolumeAttribute { get; }
	}
}
