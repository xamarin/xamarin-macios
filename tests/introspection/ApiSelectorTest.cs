//
// Test the generated API selectors against typos or non-existing cases
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Reflection;
using NUnit.Framework;

using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Introspection {

	public abstract class ApiSelectorTest : ApiBaseTest {

		// not everything should be even tried

		protected virtual bool Skip (Type type)
		{
			if (type.ContainsGenericParameters)
				return true;

			// skip delegate (and other protocol references)
			foreach (object ca in type.GetCustomAttributes (false)) {
				if (ca is ProtocolAttribute)
					return true;
				if (ca is ModelAttribute)
					return true;
			}

			switch (type.FullName) {
			case "MetalPerformanceShaders.MPSCommandBuffer":
				// The reflectable type metadata contains no selectors.
				return true;
			}

			return SkipDueToAttribute (type);
		}

		protected virtual bool Skip (Type type, string selectorName)
		{
			// The MapKit types/selectors are optional protocol members pulled in from MKAnnotation/MKOverlay.
			// These concrete (wrapper) subclasses do not implement all of those optional members, but we
			// still need to provide a binding for them, so that user subclasses can implement those members.
			switch (type.Name) {
			case "AVAggregateAssetDownloadTask":
				switch (selectorName) {
				case "URLAsset": // added in Xcode 9 and it is present.
					return true;
				}
				break;
			case "AVAssetDownloadStorageManager":
				switch (selectorName) {
				case "sharedDownloadStorageManager": // added in Xcode 9 and it is present.
					return true;
				}
				break;
			case "MKCircle":
			case "MKPolygon":
			case "MKPolyline":
				switch (selectorName) {
				case "canReplaceMapContent":
					return true;
				}
				break;
			case "MKShape":
				switch (selectorName) {
				case "setCoordinate:":
					return true;
				}
				break;
			case "MKPlacemark":
				switch (selectorName) {
				case "setCoordinate:":
				case "subtitle":
					return true;
				}
				break;
			case "MKTileOverlay":
				switch (selectorName) {
				case "intersectsMapRect:":
					return true;
				}
				break;
			// AVAudioChannelLayout and AVAudioFormat started conforming to NSSecureCoding in OSX 10.11 and iOS 9
			case "AVAudioChannelLayout":
			case "AVAudioFormat":
			// NSSecureCoding added in iOS 10 / macOS 10.12
			case "CNContactFetchRequest":
			case "GKEntity":
			case "GKPolygonObstacle":
			case "GKComponent":
			case "GKGraphNode":
			case "WKUserContentController":
			case "WKProcessPool":
			case "WKWebViewConfiguration":
			case "WKWebsiteDataStore":
				switch (selectorName) {
				case "encodeWithCoder:":
					return true;
				}
				break;
			// SKTransition started conforming to NSCopying in OSX 10.11 and iOS 9
			case "SKTransition":
			// iOS 10 beta 2
			case "GKBehavior":
			case "MDLTransform":
			// UISceneActivationRequestOptions started conforming to NSCopying oin Xcode 13
			case "UISceneActivationRequestOptions":
				switch (selectorName) {
				case "copyWithZone:":
					return true;
				}
				break;
			case "MDLMaterialProperty":
				switch (selectorName) {
				case "copyWithZone:":
					// not working before iOS 10, macOS 10.12
					return !TestRuntime.CheckXcodeVersion (8, 0);
				}
				break;
			// Xcode 8 beta 2
			case "GKGraph":
			case "GKAgent":
			case "GKAgent2D":
			case "NEFlowMetaData":
			case "NWEndpoint":
				switch (selectorName) {
				case "copyWithZone:":
				case "encodeWithCoder:":
					return true;
				}
				break;
			// now conforms to MDLName
			case "MTKMeshBuffer":
				switch (selectorName) {
				case "name":
				case "setName:":
					return true;
				}
				break;
			// Xcode 9
			case "CIQRCodeFeature":
				switch (selectorName) {
				case "copyWithZone:":
				case "encodeWithCoder:":
					return !TestRuntime.CheckXcodeVersion (9, 0);
				}
				break;
			case "CKFetchRecordZoneChangesOptions":
				switch (selectorName) {
				case "copyWithZone:":
					return !TestRuntime.CheckXcodeVersion (9, 0);
				}
				break;
#if !NET
			case "NSUrl":
			case "ARQuickLookPreviewItem":
				switch (selectorName) {
				case "previewItemTitle":
					// 'previewItemTitle' is inlined from the QLPreviewItem protocol and should be optional (fixed in .NET)
					return true;
				}
				break;
#endif
			case "MKMapItem": // Selector not available on iOS 32-bit
				switch (selectorName) {
				case "encodeWithCoder:":
					return !TestRuntime.CheckXcodeVersion (9, 0);
				}
				break;
#if !MONOMAC
			case "MTLCaptureManager":
			case "NEHotspotEapSettings": // Wireless Accessory Configuration is not supported in the simulator.
			case "NEHotspotConfigurationManager":
			case "NEHotspotHS20Settings":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
			case "ARBodyTrackingConfiguration":
			case "ARGeoTrackingConfiguration":
				switch (selectorName) {
				case "supportsAppClipCodeTracking": // Only available on device
					return TestRuntime.IsSimulatorOrDesktop;
				}
				break;
			case "CSImportExtension":
				switch (selectorName) {
				case "beginRequestWithExtensionContext:":
				case "updateAttributes:forFileAtURL:error:":
					if (TestRuntime.IsSimulatorOrDesktop) // not available in the sim
						return true;
					break;
				}
				break;
			case "HKQuery":
				switch (selectorName) {
				case "predicateForVerifiableClinicalRecordsWithRelevantDateWithinDateInterval:": // not available in the sim
					if (TestRuntime.IsSimulatorOrDesktop) // not available in the sim
						return true;
					break;
				}
				break;
#endif
			case "WKPreferences":
				switch (selectorName) {
				case "encodeWithCoder:": // from iOS 10
					return true;
				case "textInteractionEnabled": // xcode 13 renamed this to `isTextInteractionEnabled` but does not respond to the old one
					return true;
				}
				break;
			case "CIFilterGenerator":
				switch (selectorName) {
				case "filterGenerator":
				case "filterGeneratorWithContentsOfURL:":
					if (TestRuntime.IsSimulatorOrDesktop)
						return true;
					break;
				}
				break;
			}
			// This ctors needs to be manually bound
			switch (type.Name) {
			case "AVCaptureVideoPreviewLayer":
				switch (selectorName) {
				case "initWithSession:":
				case "initWithSessionWithNoConnection:":
					return true;
				}
				break;
			case "GKPath":
				switch (selectorName) {
				case "initWithPoints:count:radius:cyclical:":
				case "initWithFloat3Points:count:radius:cyclical:":
					return true;
				}
				break;
			case "GKPolygonObstacle":
				switch (selectorName) {
				case "initWithPoints:count:":
					return true;
				}
				break;
			case "MDLMesh":
				switch (selectorName) {
				case "initCapsuleWithExtent:cylinderSegments:hemisphereSegments:inwardNormals:geometryType:allocator:":
				case "initConeWithExtent:segments:inwardNormals:cap:geometryType:allocator:":
				case "initHemisphereWithExtent:segments:inwardNormals:cap:geometryType:allocator:":
				case "initMeshBySubdividingMesh:submeshIndex:subdivisionLevels:allocator:":
				case "initSphereWithExtent:segments:inwardNormals:geometryType:allocator:":
				case "initBoxWithExtent:segments:inwardNormals:geometryType:allocator:":
				case "initCylinderWithExtent:segments:inwardNormals:topCap:bottomCap:geometryType:allocator:":
				case "initIcosahedronWithExtent:inwardNormals:geometryType:allocator:":
				case "initPlaneWithExtent:segments:geometryType:allocator:":
					return true;
				}
				break;
			case "MDLNoiseTexture":
				switch (selectorName) {
				case "initCellularNoiseWithFrequency:name:textureDimensions:channelEncoding:":
				case "initVectorNoiseWithSmoothness:name:textureDimensions:channelEncoding:":
					return true;
				}
				break;
			case "NSOperationQueue":
				switch (selectorName) {
				case "progress":
					// The "progress" property comes from the NSProgressReporting protocol, where it was introduced a long time ago.
					// Then NSOperationQueue started implementing the NSProgressReporting, but only in iOS 13, which means that
					// this selector does not exist on earlier iOS versions, even to the managed property (from the protocol) claims so.
					if (!TestRuntime.CheckXcodeVersion (11, 0))
						return true;
					break;
				}
				break;
			case "NSImage":
				switch (selectorName) {
				case "initByReferencingFile:":
					return true;
				}
				break;

			case "OSLogMessageComponent":
				switch (selectorName) {
				case "encodeWithCoder:":
					if (!TestRuntime.CheckXcodeVersion (13, 0))
						return true;
					break;
				}
				break;
			// Conform to SKWarpable
			case "SKEffectNode":
			case "SKSpriteNode":
				switch (selectorName) {
				case "setSubdivisionLevels:":
				case "setWarpGeometry:":
					return true;
				}
				break;
			case "SKAttribute":
			case "SKAttributeValue":
				switch (selectorName) {
				case "encodeWithCoder:":
					if (!TestRuntime.CheckXcodeVersion (8, 0))
						return true;
					break;
				}
				break;
			case "SKUniform":
				switch (selectorName) {
				// New selectors
				case "initWithName:vectorFloat2:":
				case "initWithName:vectorFloat3:":
				case "initWithName:vectorFloat4:":
				case "initWithName:matrixFloat2x2:":
				case "initWithName:matrixFloat3x3:":
				case "initWithName:matrixFloat4x4:":
				// Old selectors
				case "initWithName:floatVector2:":
				case "initWithName:floatVector3:":
				case "initWithName:floatVector4:":
				case "initWithName:floatMatrix2:":
				case "initWithName:floatMatrix3:":
				case "initWithName:floatMatrix4:":
					return true;
				}
				break;
			case "SKVideoNode":
				switch (selectorName) {
				case "initWithFileNamed:":
				case "initWithURL:":
				case "initWithVideoFileNamed:":
				case "initWithVideoURL:":
				case "videoNodeWithFileNamed:":
				case "videoNodeWithURL:":
					return true;
				}
				break;
			case "SKWarpGeometryGrid":
				switch (selectorName) {
				case "initWithColumns:rows:sourcePositions:destPositions:":
					return true;
				}
				break;
			case "SKNode":
				switch (selectorName) {
				case "focusItemContainer":
					if (!TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
				break;
			case "INPriceRange":
				switch (selectorName) {
				case "initWithMaximumPrice:currencyCode:":
				case "initWithMinimumPrice:currencyCode:":
					return true;
				}
				break;
			case "CKUserIdentityLookupInfo":
				switch (selectorName) {
				case "initWithEmailAddress:":
				case "initWithPhoneNumber:":
				case "lookupInfosWithRecordIDs:": // FAILs on watch yet we do have a unittest for it
				case "lookupInfosWithEmails:": // FAILs on watch yet we do have a unittest for it
				case "lookupInfosWithPhoneNumbers:": // FAILs on watch yet we do have a unittest for it
					return true;
				}
				break;
#if (__WATCHOS__ || __MACOS__ || __MACCATALYST__)
			case "AVPlayerItem":
				switch (selectorName) { // comes from AVPlayerItem+MPAdditions.h
				case "nowPlayingInfo":
				case "setNowPlayingInfo:":
					return TestRuntime.IsSimulatorOrDesktop;
				}
				break;
#endif
			case "AVPlayerItemVideoOutput":
				switch (selectorName) {
				case "initWithOutputSettings:":
				case "initWithPixelBufferAttributes:":
					return true;
				}
				break;
			case "MTLBufferLayoutDescriptor": // We do have unit tests under monotouch-tests for this properties
				switch (selectorName) {
				case "stepFunction":
				case "setStepFunction:":
				case "stepRate":
				case "setStepRate:":
				case "stride":
				case "setStride:":
					return true;
				}
				break;
			case "MTLFunctionConstant": // we do have unit tests under monotouch-tests for this properties
				switch (selectorName) {
				case "name":
				case "type":
				case "index":
				case "required":
					return true;
				}
				break;
			case "MTLStageInputOutputDescriptor": // we do have unit tests under monotouch-tests for this properties
				switch (selectorName) {
				case "attributes":
				case "indexBufferIndex":
				case "setIndexBufferIndex:":
				case "indexType":
				case "setIndexType:":
				case "layouts":
					return true;
				}
				break;
			case "MTLAttributeDescriptor": // we do have unit tests under monotouch-tests for this properties
				switch (selectorName) {
				case "bufferIndex":
				case "setBufferIndex:":
				case "format":
				case "setFormat:":
				case "offset":
				case "setOffset:":
					return true;
				}
				break;
			case "MTLAttribute": // we do have unit tests under monotouch-tests for this properties
				switch (selectorName) {
				case "isActive":
				case "attributeIndex":
				case "attributeType":
				case "isPatchControlPointData":
				case "isPatchData":
				case "name":
				case "isDepthTexture":
					return true;
				}
				break;
			case "MTLArgument": // we do have unit tests under monotouch-tests for this properties
				switch (selectorName) {
				case "isDepthTexture":
					return true;
				}
				break;
			case "MTLArgumentDescriptor":
				switch (selectorName) {
				case "access":
				case "setAccess:":
				case "arrayLength":
				case "setArrayLength:":
				case "constantBlockAlignment":
				case "setConstantBlockAlignment:":
				case "dataType":
				case "setDataType:":
				case "index":
				case "setIndex:":
				case "textureType":
				case "setTextureType:":
					return true;
				}
				break;
			case "MTLHeapDescriptor":
				switch (selectorName) {
				case "cpuCacheMode":
				case "setCpuCacheMode:":
				case "size":
				case "setSize:":
				case "storageMode":
				case "setStorageMode:":
					return true;
				}
				break;
			case "MTLIndirectCommandBufferDescriptor": // we do have unit tests under monotouch-tests for this properties
				switch (selectorName) {
				case "commandTypes":
				case "setCommandTypes:":
				case "inheritPipelineState":
				case "setInheritPipelineState:":
				case "inheritBuffers":
				case "setInheritBuffers:":
				case "maxFragmentBufferBindCount":
				case "setMaxFragmentBufferBindCount:":
				case "maxVertexBufferBindCount":
				case "setMaxVertexBufferBindCount:":
					return true;
				}
				break;
			case "MTLPipelineBufferDescriptor":
				switch (selectorName) {
				case "mutability":
				case "setMutability:":
					return true;
				}
				break;
			case "MTLPointerType":
				switch (selectorName) {
				case "access":
				case "alignment":
				case "dataSize":
				case "elementIsArgumentBuffer":
				case "elementType":
					return true;
				}
				break;
			case "MTLSharedEventListener":
				switch (selectorName) {
				case "dispatchQueue":
					return true;
				}
				break;
			case "MTLTextureReferenceType":
				switch (selectorName) {
				case "access":
				case "isDepthTexture":
				case "textureDataType":
				case "textureType":
					return true;
				}
				break;
			case "MTLType":
				switch (selectorName) {
				case "dataType":
					return true;
				}
				break;
			case "MTLTileRenderPipelineColorAttachmentDescriptor":
				switch (selectorName) {
				case "pixelFormat":
				case "setPixelFormat:":
					return true;
				}
				break;
			case "MTLTileRenderPipelineDescriptor":
				switch (selectorName) {
				case "colorAttachments":
				case "label":
				case "setLabel:":
				case "rasterSampleCount":
				case "setRasterSampleCount:":
				case "threadgroupSizeMatchesTileSize":
				case "setThreadgroupSizeMatchesTileSize:":
				case "tileBuffers":
				case "tileFunction":
				case "setTileFunction:":
				case "maxTotalThreadsPerThreadgroup":
				case "setMaxTotalThreadsPerThreadgroup:":
				case "binaryArchives":
				case "setBinaryArchives:":
					return true;
				}
				break;
			case "MTLBlitPassDescriptor":
				switch (selectorName) {
				case "sampleBufferAttachments":
					return true;
				}
				break;
			case "MTLBlitPassSampleBufferAttachmentDescriptor":
				switch (selectorName) {
				case "endOfEncoderSampleIndex":
				case "setEndOfEncoderSampleIndex:":
				case "sampleBuffer":
				case "setSampleBuffer:":
				case "startOfEncoderSampleIndex":
				case "setStartOfEncoderSampleIndex:":
					return true;
				}
				break;
			case "MTLComputePassDescriptor":
				switch (selectorName) {
				case "dispatchType":
				case "setDispatchType:":
				case "sampleBufferAttachments":
					return true;
				}
				break;
			case "MTLComputePassSampleBufferAttachmentDescriptor":
				switch (selectorName) {
				case "sampleBuffer":
				case "setSampleBuffer:":
				case "startOfEncoderSampleIndex":
				case "setStartOfEncoderSampleIndex:":
				case "endOfEncoderSampleIndex":
				case "setEndOfEncoderSampleIndex:":
					return true;
				}
				break;
			case "MTLCounterSampleBufferDescriptor":
				switch (selectorName) {
				case "counterSet":
				case "setCounterSet:":
				case "label":
				case "setLabel:":
				case "sampleCount":
				case "setSampleCount:":
				case "storageMode":
				case "setStorageMode:":
					return true;
				}
				break;
			case "MTLLinkedFunctions":
				switch (selectorName) {
				case "binaryFunctions":
				case "setBinaryFunctions:":
				case "functions":
				case "setFunctions:":
				case "groups":
				case "setGroups:":
					return true;
				}
				break;
			case "MTLRenderPassSampleBufferAttachmentDescriptor":
				switch (selectorName) {
				case "endOfFragmentSampleIndex":
				case "setEndOfFragmentSampleIndex:":
				case "endOfVertexSampleIndex":
				case "setEndOfVertexSampleIndex:":
				case "sampleBuffer":
				case "setSampleBuffer:":
				case "startOfFragmentSampleIndex":
				case "setStartOfFragmentSampleIndex:":
				case "startOfVertexSampleIndex":
				case "setStartOfVertexSampleIndex:":
					return true;
				}
				break;
			case "MTLIntersectionFunctionTableDescriptor":
				switch (selectorName) {
				case "functionCount":
				case "setFunctionCount:":
					return true;
				}
				break;
			case "MTLResourceStatePassDescriptor":
				switch (selectorName) {
				case "sampleBufferAttachments":
					return true;
				}
				break;
			case "MTLResourceStatePassSampleBufferAttachmentDescriptor":
				switch (selectorName) {
				case "endOfEncoderSampleIndex":
				case "setEndOfEncoderSampleIndex:":
				case "sampleBuffer":
				case "setSampleBuffer:":
				case "startOfEncoderSampleIndex":
				case "setStartOfEncoderSampleIndex:":
					return true;
				}
				break;
			case "MTLVisibleFunctionTableDescriptor":
				switch (selectorName) {
				case "functionCount":
				case "setFunctionCount:":
					return true;
				}
				break;
			case "AVPlayerLooper": // This API got introduced in Xcode 8.0 binding but is not currently present nor in Xcode 8.3 or Xcode 9.0 needs research
				switch (selectorName) {
				case "isLoopingEnabled":
					return true;
				}
				break;
			case "NSMenu":
				switch (selectorName) {
				case "appearance":
				case "setAppearance:":
				case "effectiveAppearance":
					if (!TestRuntime.CheckXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch))
						return true;
					break;
				}
				break;
			case "NSQueryGenerationToken": // A test was added in monotouch tests to ensure the selector works
				switch (selectorName) {
				case "encodeWithCoder:":
					return true;
				}
				break;
			case "INSpeakableString":
				switch (selectorName) {
				case "initWithVocabularyIdentifier:spokenPhrase:pronunciationHint:":
				case "initWithIdentifier:spokenPhrase:pronunciationHint:":
					return true;
				}
				break;
			case "HMCharacteristicEvent":
				switch (selectorName) {
				case "copyWithZone:":
				case "mutableCopyWithZone:":
					// Added in Xcode9 (i.e. only 64 bits) so skip 32 bits
					return !TestRuntime.CheckXcodeVersion (9, 0);
				}
				break;
			case "MPSCnnConvolution":
				switch (selectorName) {
				case "initWithDevice:convolutionDescriptor:kernelWeights:biasTerms:flags:":
					return true;
				}
				break;
			case "MPSCnnFullyConnected":
				switch (selectorName) {
				case "initWithDevice:convolutionDescriptor:kernelWeights:biasTerms:flags:":
					return true;
				}
				break;
			case "MPSImageConversion":
				switch (selectorName) {
				case "initWithDevice:srcAlpha:destAlpha:backgroundColor:conversionInfo:":
					return true;
				}
				break;
			case "MPSImageDilate":
				switch (selectorName) {
				case "initWithDevice:kernelWidth:kernelHeight:values:":
					return true;
				}
				break;
			case "MPSImageGaussianPyramid":
				switch (selectorName) {
				case "initWithDevice:kernelWidth:kernelHeight:weights:":
					return true;
				}
				break;
			case "MPSImagePyramid":
				switch (selectorName) {
				case "initWithDevice:kernelWidth:kernelHeight:weights:":
					return true;
				}
				break;
			case "MPSImageSobel":
				switch (selectorName) {
				case "initWithDevice:linearGrayColorTransform:":
					return true;
				}
				break;
			case "MPSImageThresholdBinary":
				switch (selectorName) {
				case "initWithDevice:thresholdValue:maximumValue:linearGrayColorTransform:":
					return true;
				}
				break;
			case "MPSImageThresholdBinaryInverse":
				switch (selectorName) {
				case "initWithDevice:thresholdValue:maximumValue:linearGrayColorTransform:":
					return true;
				}
				break;
			case "MPSImageThresholdToZero":
				switch (selectorName) {
				case "initWithDevice:thresholdValue:linearGrayColorTransform:":
					return true;
				}
				break;
			case "MPSImageThresholdToZeroInverse":
				switch (selectorName) {
				case "initWithDevice:thresholdValue:linearGrayColorTransform:":
					return true;
				}
				break;
			case "MPSImageThresholdTruncate":
				switch (selectorName) {
				case "initWithDevice:thresholdValue:linearGrayColorTransform:":
					return true;
				}
				break;
			case "MPSCnnBinaryKernel":
				switch (selectorName) {
				// Xcode 9.4 removed both selectors from MPSCnnBinaryKernel, reported radar https://trello.com/c/7EAM0qk1
				// but apple says this was intentional.
				case "kernelHeight":
				case "kernelWidth":
					return true;
				}
				break;
			case "MPSImageLaplacianPyramid":
			case "MPSImageLaplacianPyramidSubtract":
			case "MPSImageLaplacianPyramidAdd":
				switch (selectorName) {
				case "initWithDevice:kernelWidth:kernelHeight:weights:":
					return true;
				}
				break;
			case "CPMessageListItem":
				switch (selectorName) {
				case "initWithConversationIdentifier:text:leadingConfiguration:trailingConfiguration:detailText:trailingText:":
				case "initWithFullName:phoneOrEmailAddress:leadingConfiguration:trailingConfiguration:detailText:trailingText:":
					return true;
				}
				break;
			case "VNFaceLandmarkRegion":
			case "VNFaceLandmarks":
			case "PHLivePhoto":
				switch (selectorName) {
				case "copyWithZone:":
				case "encodeWithCoder:":
				case "requestRevision":
					// Conformance added in Xcode 11
					if (!TestRuntime.CheckXcodeVersion (11, 0))
						return true;
					break;
				case "objectWithItemProviderData:typeIdentifier:error:":
				case "readableTypeIdentifiersForItemProvider":
					// Conformance added in Xcode 12
					if (!TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
				break;
			case "MPSNNNeuronDescriptor":
			case "MLDictionaryConstraint":
			case "MLFeatureDescription":
			case "MLImageConstraint":
			case "MLImageSize":
			case "MLImageSizeConstraint":
			case "MLModelConfiguration":
			case "MLModelDescription":
			case "MLMultiArrayConstraint":
			case "MLMultiArrayShapeConstraint":
			case "MLSequenceConstraint":
				switch (selectorName) {
				case "encodeWithCoder:":
					// Conformance added in Xcode 11
					if (!TestRuntime.CheckXcodeVersion (11, 0))
						return true;
					break;
				}
				break;
			case "MLDictionaryFeatureProvider":
			case "MLMultiArray":
			case "MLFeatureValue":
			case "MLSequence":
				switch (selectorName) {
				case "encodeWithCoder:":
					if (!TestRuntime.CheckXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch))
						return true;
					break;
				}
				break;
			case "BGTaskScheduler":
				switch (selectorName) {
				case "sharedScheduler":
					return true;
				}
				break;
#if !__MACOS__
			case "ARSkeletonDefinition":
				switch (selectorName) {
				case "indexForJointName:":
				case "defaultBody2DSkeletonDefinition":
				case "defaultBody3DSkeletonDefinition":
					// This selector does not exist in the simulator
					if (TestRuntime.IsSimulatorOrDesktop)
						return true;
					break;
				}
				break;
#endif
			case "INParameter":
				switch (selectorName) {
				case "copyWithZone:":
					if (!TestRuntime.CheckXcodeVersion (10, 0))
						return true;
					break;
				}
				break;
			case "MTLCommandBufferDescriptor":
				switch (selectorName) {
				case "errorOptions":
				case "setErrorOptions:":
				case "retainedReferences":
				case "setRetainedReferences:":
					// iOS 15 sim (and macOS 12) fails, API added in 14.0
					if (TestRuntime.CheckXcodeVersion (13, 0))
						return true;
					break;
				}
				break;
			case "NSTask":
				// category, NSTask won't respond -> @interface NSTask (NSTaskConveniences)
				if (selectorName == "waitUntilExit")
					return true;
				break;
			case "NSTextStorage":
				switch (selectorName) {
				// declared in a superclass, and implemented in a concrete subclass, so it doesn't show up during inspection of NSTextStorage itself.
				case "initWithString:":
					return true;
				}
				break;
			case "MPSGraphCompilationDescriptor":
				// Runtime lookup doesn't work, but executing it works fine.
				return true;
			case "MPSImageDescriptor":
				switch (selectorName) {
				case "copyWithZone:":
					if (!TestRuntime.CheckXcodeVersion (10, 0))
						return true;
					break;
				}
				break;
			case "MPSGraphExecutableExecutionDescriptor":
				switch (selectorName) {
				case "copyWithZone:":
					if (!TestRuntime.CheckXcodeVersion (14, 0))
						return true;
					break;
				}
				break;
			case "UIControl":
#if __MACCATALYST__
				switch (selectorName) {
				case "contextMenuInteraction:configurationForMenuAtLocation:":
					if (!TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
#endif
				break;
			case "UISceneConnectionOptions":
#if __MACCATALYST__
				switch (selectorName) {
				case "shortcutItem":
					if (!TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
#endif
				break;
			case "SKAdImpression":
#if __MACCATALYST__
				switch (selectorName) {
				case "initWithSourceAppStoreItemIdentifier:advertisedAppStoreItemIdentifier:adNetworkIdentifier:adCampaignIdentifier:adImpressionIdentifier:timestamp:signature:version:":
					if (TestRuntime.CheckXcodeVersion (14, 0))
						return true;
					break;
				}
#endif
				break;
			case "EKParticipant":
#if __MACCATALYST__
				switch (selectorName) {
				case "ABRecordWithAddressBook:": // Deprecated in 13.1
					if (TestRuntime.CheckXcodeVersion (14, 0))
						return true;
					break;
				}
#endif
				break;
			case "SWRemoveParticipantAlertController":
				switch (selectorName) {
				case "initWithFrame:":
					return true;
				}
				break;
			case "CAEdrMetadata":
				switch (selectorName) {
				case "copyWithZone:":
				case "encodeWithCoder:":
					return !TestRuntime.CheckXcodeVersion (14, 3);
				}
				break;
			case "GCKeyboard":
				switch (selectorName) {
				case "encodeWithCoder:": // removed comformance
					return TestRuntime.CheckXcodeVersion (14, 3);
				}
				break;
			}

			// old binding mistake
			return (selectorName == "initWithCoder:");
		}

		protected virtual bool CheckResponse (bool value, Type actualType, MethodBase method, ref string name)
		{
			if (value)
				return true;

			var mname = method.Name;
			// properties getter and setter will be methods in the _Extensions type
			if (method.IsSpecialName)
				mname = mname.Replace ("get_", "Get").Replace ("set_", "Set");

			// it's possible that the selector was inlined for an OPTIONAL protocol member
			// we do not want those reported (too many false positives) and we have other tests to find such mistakes
			foreach (var intf in actualType.GetInterfaces ()) {
				if (intf.GetCustomAttributes<ProtocolAttribute> () is null)
					continue;
				var ext = Type.GetType (intf.Namespace + "." + intf.Name.Remove (0, 1) + "_Extensions, " + intf.Assembly.FullName);
				if (ext is null)
					continue;
				foreach (var m in ext.GetMethods ()) {
					if (mname != m.Name)
						continue;
					var parameters = method.GetParameters ();
					var ext_params = m.GetParameters ();
					// first parameters is `this XXX This`
					if (parameters.Length == ext_params.Length - 1) {
						bool match = true;
						for (int i = 1; i < ext_params.Length; i++) {
							match |= (parameters [i - 1].ParameterType == ext_params [i].ParameterType);
						}
						if (match)
							return true;
					}
				}
			}

			name = actualType.FullName + " : " + name;
			return false;
		}

		static IntPtr responds_handle = Selector.GetHandle ("instancesRespondToSelector:");

		[Test]
		public void Protocols ()
		{
			Errors = 0;
			int n = 0;

			foreach (Type t in Assembly.GetTypes ()) {
				if (t.IsNested || !NSObjectType.IsAssignableFrom (t))
					continue;

				foreach (object ca in t.GetCustomAttributes (false)) {
					if (ca is ProtocolAttribute) {
						foreach (var c in t.GetConstructors (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
							ProcessProtocolMember (t, c, ref n);
						}
						foreach (var m in t.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
							ProcessProtocolMember (t, m, ref n);
						}
					}
				}
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} protocol selectors validated", Errors, n);
		}

		void ProcessProtocolMember (Type t, MethodBase m, ref int n)
		{
			if (SkipDueToAttribute (m))
				return;

			foreach (object ca in m.GetCustomAttributes (true)) {
				ExportAttribute export = (ca as ExportAttribute);
				if (export is null)
					continue;

				string name = export.Selector;
				if (Skip (t, name))
					continue;

				CheckInit (t, m, name);
				n++;
			}
		}

		protected virtual IntPtr GetClassForType (Type type)
		{
			var fi = type.GetField ("class_ptr", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			if (fi is null)
				return IntPtr.Zero; // e.g. *Delegate
#if NET
			return (NativeHandle) fi.GetValue (null);
#else
			return (IntPtr) fi.GetValue (null);
#endif
		}

		[Test]
		public void InstanceMethods ()
		{
			Errors = 0;
			ErrorData.Clear ();
			int n = 0;

			foreach (Type t in Assembly.GetTypes ()) {
				if (t.IsNested || !NSObjectType.IsAssignableFrom (t))
					continue;

				if (Skip (t) || SkipDueToAttribute (t))
					continue;

				IntPtr class_ptr = GetClassForType (t);

				if (class_ptr == IntPtr.Zero)
					continue;

				foreach (var c in t.GetConstructors (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
					Process (class_ptr, t, c, ref n);
				}

				foreach (var m in t.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
					Process (class_ptr, t, m, ref n);
				}
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} instance selector validated{2}", Errors, n, Errors == 0 ? string.Empty : ":\n" + ErrorData.ToString () + "\n");
		}

		void Process (IntPtr class_ptr, Type t, MethodBase m, ref int n)
		{
			if (m.DeclaringType != t || SkipDueToAttribute (m))
				return;

			foreach (object ca in m.GetCustomAttributes (true)) {
				ExportAttribute export = (ca as ExportAttribute);
				if (export is null)
					continue;

				string name = export.Selector;
				if (Skip (t, name))
					continue;

				CheckInit (t, m, name);

				bool result = bool_objc_msgSend_IntPtr (class_ptr, responds_handle, Selector.GetHandle (name));
				bool response = CheckResponse (result, t, m, ref name);
				if (!response)
					ReportError ("Selector not found for {0} in {1} on {2}", name, m, t.FullName);
				n++;
			}
		}

		void CheckInit (Type t, MethodBase m, string name)
		{
			if (SkipInit (name, m))
				return;

			bool init = IsInitLike (name);
			if (m is ConstructorInfo) {
				if (!init)
					ReportError ("Selector {0} used on a constructor (not a method) on {1}", name, t.FullName);
			} else {
				if (init)
					ReportError ("Selector {0} used on a method (not a constructor) on {1}", name, t.FullName);
			}
		}

		bool IsInitLike (string selector)
		{
			if (!selector.StartsWith ("init", StringComparison.OrdinalIgnoreCase))
				return false;
			return selector.Length < 5 || Char.IsUpper (selector [4]);
		}

		protected virtual bool SkipInit (string selector, MethodBase m)
		{
			switch (selector) {
			// NSAttributedString
			case "initWithHTML:documentAttributes:":
			case "initWithRTF:documentAttributes:":
			case "initWithRTFD:documentAttributes:":
			case "initWithURL:options:documentAttributes:error:":
			case "initWithFileURL:options:documentAttributes:error:":
			// AVAudioRecorder
			case "initWithURL:settings:error:":
			case "initWithURL:format:error:":
			// NSUrlProtectionSpace
			case "initWithHost:port:protocol:realm:authenticationMethod:":
			case "initWithProxyHost:port:type:realm:authenticationMethod:":
			// NSUserDefaults
			case "initWithSuiteName:":
			case "initWithUser:":
			// GKScore
			case "initWithCategory:":
			case "initWithLeaderboardIdentifier:":
			// MCSession
			case "initWithPeer:securityIdentity:encryptionPreference:":
			// INSetProfileInCarIntent and INSaveProfileInCarIntent
			case "initWithProfileNumber:profileName:defaultProfile:":
			case "initWithProfileNumber:profileLabel:defaultProfile:":
			case "initWithProfileNumber:profileName:":
			case "initWithProfileNumber:profileLabel:":
			// MPSCnnBinaryConvolutionNode and MPSCnnBinaryFullyConnectedNode
			case "initWithSource:weights:outputBiasTerms:outputScaleTerms:inputBiasTerms:inputScaleTerms:type:flags:":
			// UISegmentedControl
			case "initWithItems:":
			// CLBeaconRegion
			case "initWithUUID:identifier:":
			case "initWithUUID:major:identifier:":
			case "initWithUUID:major:minor:identifier:":
			// Intents
			case "initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isMe:suggestionType:":
			case "initWithPersonHandle:nameComponents:displayName:image:contactIdentifier:customIdentifier:isContactSuggestion:suggestionType:":
			// NEHotspotConfiguration
			case "initWithSSID:":
			case "initWithSSID:passphrase:isWEP:":
			case "initWithSSIDPrefix:":
			case "initWithSSIDPrefix:passphrase:isWEP:":
			// MapKit
			case "initWithMaxCenterCoordinateDistance:":
			case "initWithMinCenterCoordinateDistance:":
			case "initExcludingCategories:":
			case "initIncludingCategories:":
			// Vision
			case "initWithCenter:diameter:":
			case "initWithCenter:radius:":
			case "initWithR:theta:":
			// PassKit
			case "initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:cardTemplateIdentifier:preview:":
			case "initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:cardConfigurationIdentifier:preview:":
			// NSImage
			case "initWithDataIgnoringOrientation:":
			// SCContentFilter
			case "initWithDisplay:excludingApplications:exceptingWindows:":
			case "initWithDisplay:excludingWindows:":
			case "initWithDisplay:includingApplications:exceptingWindows:":
			case "initWithDisplay:includingWindows:":
				var mi = m as MethodInfo;
				return mi is not null && !mi.IsPublic && (mi.ReturnType.Name == "IntPtr" || mi.ReturnType.Name == "NativeHandle");
			// NSAppleEventDescriptor
			case "initListDescriptor":
			case "initRecordDescriptor":
			// SharedWithYouCore
			case "initWithLocalIdentifier:":
			case "initWithCollaborationIdentifier:":
				return true;
			// CloudKit
			case "initWithExcludedZoneIDs:":
			case "initWithZoneIDs:":
				return true;
			default:
				return false;
			}
		}

		protected virtual void Dispose (NSObject obj, Type type)
		{
			obj.Dispose ();
		}

		// funny, this is how I envisioned the instance version... before hitting run :|
		protected virtual bool CheckStaticResponse (bool value, Type actualType, Type declaredType, ref string name)
		{
			if (value)
				return true;

			name = actualType.FullName + " : " + name;
			return false;
		}

		[Test]
		public void StaticMethods ()
		{
			Errors = 0;
			ErrorData.Clear ();
			int n = 0;

			IntPtr responds_handle = Selector.GetHandle ("respondsToSelector:");

			foreach (Type t in Assembly.GetTypes ()) {
				if (t.IsNested || !NSObjectType.IsAssignableFrom (t))
					continue;

				if (Skip (t) || SkipDueToAttribute (t))
					continue;

				FieldInfo fi = t.GetField ("class_ptr", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				if (fi is null)
					continue; // e.g. *Delegate
				IntPtr class_ptr = (IntPtr) (NativeHandle) fi.GetValue (null);

				foreach (var m in t.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
					if (SkipDueToAttribute (m))
						continue;

					foreach (object ca in m.GetCustomAttributes (true)) {
						if (ca is ExportAttribute) {
							string name = (ca as ExportAttribute).Selector;

							if (Skip (t, name))
								continue;

							bool result = bool_objc_msgSend_IntPtr (class_ptr, responds_handle, Selector.GetHandle (name));
							bool response = CheckStaticResponse (result, t, m.DeclaringType, ref name);
							if (!response)
								ReportError (name);
							n++;
						}
					}
				}
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} static selector validated{2}", Errors, n, Errors == 0 ? string.Empty : ":\n" + ErrorData.ToString () + "\n");
		}
	}
}
