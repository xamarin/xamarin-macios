// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Numerics;

using Accelerate;
using AudioToolbox;
using AudioUnit;
using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using CoreLocation;
using CoreMedia;
using CoreVideo;
using GameController;
using GameplayKit;
using MapKit;
using Metal;
using MetalPerformanceShaders;
using ModelIO;
using SceneKit;
using VideoToolbox;

#if HAS_DARWIN
using Darwin;
#endif

#if !TVOS
using CoreMidi;
using CoreMotion;
#endif

#if MONOMAC
using AppKit;
using PrintCore;
#else
using UIKit;
#endif

namespace MonoTouchFixtures {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StructEqualityTest {
		[TestCaseSource (nameof (Cases))]
		public void Equality (Action test)
		{
			test ();
		}

		static TestCaseData Case<T> (params T [] differentValues) where T : struct
		{
			return Case<T> (null, null, null, differentValues);
		}

		static TestCaseData Case<T> (Func<T, T, bool> typedEqual, params T [] differentValues) where T : struct
		{
			return Case<T> (typedEqual, null, null, differentValues);
		}

		static TestCaseData Case<T> (Func<T, T, bool> typedEqual, Func<T?, T?, bool> equal, Func<T?, T?, bool> notEqual, params T [] differentValues) where T : struct
		{
			return new TestCaseData ((Action) (() => Check (typedEqual, equal, notEqual, differentValues)))
				.SetName ($"Equality({typeof (T).FullName})");
		}

		static void Check<T> (Func<T, T, bool> typedEqual, Func<T?, T?, bool> equal, Func<T?, T?, bool> notEqual, params T [] differentValues) where T : struct
		{
			// Include the default value, then values that differ in individual fields.
			var values = new T [differentValues.Length + 1];
			differentValues.CopyTo (values, 1);
			var comparer = EqualityComparer<T>.Default;
			if (equal is not null) {
				Assert.That (equal (null, null), Is.True, "null == null");
				Assert.That (notEqual (null, null), Is.False, "null != null");
			}
			for (var i = 0; i < values.Length; i++) {
				var value = values [i];
				object boxed = value;
				object copy = value;
				Assert.That (value.Equals (null), Is.False, $"Equals(null), value {i}");
				Assert.That (value.Equals (""), Is.False, $"Equals(other type), value {i}");
				Assert.That (boxed.Equals (copy), Is.True, $"Boxed copy, value {i}");
				Assert.That (boxed.GetHashCode (), Is.EqualTo (copy.GetHashCode ()), $"Hash code, value {i}");
				if (equal is not null) {
					Assert.That (equal (value, null), Is.False, $"value {i} == null");
					Assert.That (equal (null, value), Is.False, $"null == value {i}");
					Assert.That (notEqual (value, null), Is.True, $"value {i} != null");
					Assert.That (notEqual (null, value), Is.True, $"null != value {i}");
				}

				for (var j = 0; j < values.Length; j++) {
					object other = values [j];
					var expected = i == j;
					var message = $"values {i}, {j}";
					Assert.That (value.Equals (other), Is.EqualTo (expected), $"Equals(object), {message}");
					Assert.That (object.Equals (boxed, other), Is.EqualTo (expected), $"object.Equals, {message}");
					Assert.That (comparer.Equals (value, values [j]), Is.EqualTo (expected), $"EqualityComparer, {message}");
					if (typedEqual is not null)
						Assert.That (typedEqual (value, values [j]), Is.EqualTo (expected), $"Equals(T), {message}");
					if (equal is not null) {
						Assert.That (equal (value, values [j]), Is.EqualTo (expected), $"operator ==, {message}");
						Assert.That (notEqual (value, values [j]), Is.EqualTo (!expected), $"operator !=, {message}");
					}
				}
			}
		}

		public static IEnumerable<TestCaseData> Cases ()
		{
			yield return Case<AVAudioConverterPrimeInfo> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVAudioConverterPrimeInfo { LeadingFrames = 1 },
				new AVAudioConverterPrimeInfo { TrailingFrames = 1 });
#if !TVOS
			yield return Case<AVAudioVoiceProcessingOtherAudioDuckingConfiguration> (
				new AVAudioVoiceProcessingOtherAudioDuckingConfiguration { EnableAdvancedDucking = true });
#endif
			yield return Case<AVBeatRange> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVBeatRange { Start = 1 }, new AVBeatRange { Length = 1 });
			yield return Case<AVEdgeWidths> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVEdgeWidths { Left = 1 }, new AVEdgeWidths { Top = 1 },
				new AVEdgeWidths { Right = 1 }, new AVEdgeWidths { Bottom = 1 });
			yield return Case<AVPixelAspectRatio> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVPixelAspectRatio { HorizontalSpacing = 1 }, new AVPixelAspectRatio { VerticalSpacing = 1 });
			yield return Case<AVAudio3DVectorOrientation> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVAudio3DVectorOrientation { Forward = Vector3.UnitX },
				new AVAudio3DVectorOrientation { Forward = Vector3.UnitY },
				new AVAudio3DVectorOrientation { Forward = Vector3.UnitZ },
				new AVAudio3DVectorOrientation { Up = Vector3.UnitX },
				new AVAudio3DVectorOrientation { Up = Vector3.UnitY },
				new AVAudio3DVectorOrientation { Up = Vector3.UnitZ });
			yield return Case<AVAudio3DAngularOrientation> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVAudio3DAngularOrientation { Yaw = 1 },
				new AVAudio3DAngularOrientation { Pitch = 1 },
				new AVAudio3DAngularOrientation { Roll = 1 });
			yield return Case<AVCaptureWhiteBalanceGains> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVCaptureWhiteBalanceGains { RedGain = 1 },
				new AVCaptureWhiteBalanceGains { GreenGain = 1 },
				new AVCaptureWhiteBalanceGains { BlueGain = 1 });
			yield return Case<AVCaptureWhiteBalanceChromaticityValues> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVCaptureWhiteBalanceChromaticityValues { X = 1 },
				new AVCaptureWhiteBalanceChromaticityValues { Y = 1 });
			yield return Case<AVCaptureWhiteBalanceTemperatureAndTintValues> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVCaptureWhiteBalanceTemperatureAndTintValues { Temperature = 1 },
				new AVCaptureWhiteBalanceTemperatureAndTintValues { Tint = 1 });
			yield return Case<AVSampleCursorSyncInfo> (new AVSampleCursorSyncInfo { IsFullSync = true });
			yield return Case<AVSampleCursorDependencyInfo> (new AVSampleCursorDependencyInfo { HasDependentSamples = true });
			yield return Case<AVSampleCursorStorageRange> (new AVSampleCursorStorageRange { Offset = 1 });
			yield return Case<AVSampleCursorChunkInfo> (new AVSampleCursorChunkInfo { SampleCount = 1 });
			yield return Case<AVSampleCursorAudioDependencyInfo> (new AVSampleCursorAudioDependencyInfo { PacketRefreshCount = 1 });
#if !TVOS
			yield return Case<AVCaptionDimension> (new AVCaptionDimension { Value = 1 });
			yield return Case<AVCaptionPoint> (new AVCaptionPoint { X = new AVCaptionDimension { Value = 1 } });
			yield return Case<AVCaptionSize> (new AVCaptionSize { Width = new AVCaptionDimension { Value = 1 } });
#endif
			yield return Case<AVCaptureTimecode> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new AVCaptureTimecode { Hours = 1 },
				new AVCaptureTimecode { Minutes = 1 },
				new AVCaptureTimecode { Seconds = 1 },
				new AVCaptureTimecode { Frames = 1 },
				new AVCaptureTimecode { UserBits = 1 },
				new AVCaptureTimecode { FrameDuration = new CMTime (1, 1) },
				new AVCaptureTimecode { SourceType = (AVCaptureTimecodeSourceType) 1 });
			yield return Case<AVPlannedVideoSegmentBoundaryGuidelines> (new AVPlannedVideoSegmentBoundaryGuidelines (1, new CMTime (1, 1)));
#if !TVOS
			yield return Case<global::AVKit.AVLegibleMediaOptionsMenuState> (new global::AVKit.AVLegibleMediaOptionsMenuState { Enabled = true });
#endif
			yield return Case<vImageBuffer> (new vImageBuffer { Width = 1 });
			yield return Case<vImageAffineTransformFloat> (new vImageAffineTransformFloat { a = 1 });
			yield return Case<vImageAffineTransformDouble> (new vImageAffineTransformDouble { a = 1 });
			yield return Case<PixelFFFF> (new PixelFFFF { A = 1 });
			yield return Case<Pixel8888> (new Pixel8888 { A = 1 });
			yield return Case<PixelARGB16U> (new PixelARGB16U { A = 1 });
			yield return Case<PixelARGB16S> (new PixelARGB16S { A = 1 });

			yield return Case<AudioClassDescription> (new AudioClassDescription { SubType = AudioFormatType.LinearPCM });
			yield return Case<AudioConverterPrimeInfo> (new AudioConverterPrimeInfo { LeadingFrames = 1 });
			yield return Case<AudioFileSmpteTime> (new AudioFileSmpteTime { Hours = 1 });
			yield return Case<AudioFileMarker> (new AudioFileMarker { MarkerID = 1 });
			yield return Case<AudioPacketRangeByteCountTranslation> (new AudioPacketRangeByteCountTranslation { Packet = 1 });
			yield return Case<AudioPacketRollDistanceTranslation> (new AudioPacketRollDistanceTranslation { Packet = 1 });
			yield return Case<AudioIndependentPacketTranslation> (new AudioIndependentPacketTranslation { Packet = 1 });
			yield return Case<AudioPacketDependencyInfoTranslation> (new AudioPacketDependencyInfoTranslation { Packet = 1 });
			yield return Case<AudioFilePacketTableInfo> (new AudioFilePacketTableInfo { ValidFrames = 1 });
			yield return Case<AudioFileRegion> (new AudioFileRegion (new IntPtr (1)));
			yield return Case<AudioFormat> (new AudioFormat { AudioStreamBasicDescription = new AudioStreamBasicDescription { SampleRate = 1 } });
			yield return Case<AudioValueRange> (new AudioValueRange { Minimum = 1 });
			yield return Case<AudioQueueBuffer> (new AudioQueueBuffer { AudioDataBytesCapacity = 1 });
			yield return Case<AudioQueueParameterEvent> (new AudioQueueParameterEvent { Value = 1 });
			yield return Case<AudioQueueLevelMeterState> (new AudioQueueLevelMeterState { AveragePower = 1 });
#if !(XAMCORE_5_0 && MONOMAC)
			yield return Case<AudioQueueChannelAssignment> ();
#endif
			yield return Case<AudioStreamBasicDescription> (new AudioStreamBasicDescription { SampleRate = 1 });
			yield return Case<AudioStreamPacketDescription> (new AudioStreamPacketDescription { StartOffset = 1 });
			yield return Case<AudioStreamPacketDependencyDescription> (new AudioStreamPacketDependencyDescription { PreRollCount = 1 });
			yield return Case<AudioChannelDescription> (new AudioChannelDescription { Label = AudioChannelLabel.Left });
			yield return Case<SmpteTime> (new SmpteTime { Hours = 1 });
			yield return Case<AudioTimeStamp> (new AudioTimeStamp { SampleTime = 1 });
			yield return Case<AudioBuffer> (new AudioBuffer { NumberChannels = 1 });
			yield return Case<CABarBeatTime> (new CABarBeatTime { Bar = 1 });
			yield return Case<MidiNoteMessage> (new MidiNoteMessage { Note = 1 });
			yield return Case<MidiChannelMessage> (new MidiChannelMessage { Status = 1 });
			yield return Case<ExtendedNoteOnEvent> (new ExtendedNoteOnEvent { Pitch = 1 });
			yield return Case<AudioComponentDescription> (new AudioComponentDescription { ComponentFlagsMask = 1 });
			yield return Case<AudioUnitParameterEvent> (new AudioUnitParameterEvent { Scope = 1 });
			yield return Case<AudioUnitParameterEvent.EventValuesStruct> (
				new AudioUnitParameterEvent.EventValuesStruct { Immediate = new AudioUnitParameterEvent.EventValuesStruct.ImmediateStruct { Value = 1 } });
			yield return Case<AudioUnitParameterEvent.EventValuesStruct.RampStruct> (
				new AudioUnitParameterEvent.EventValuesStruct.RampStruct { DurationInFrames = 1 });
			yield return Case<AudioUnitParameterEvent.EventValuesStruct.ImmediateStruct> (
				new AudioUnitParameterEvent.EventValuesStruct.ImmediateStruct { Value = 1 });
			yield return Case<AURenderEventHeader> (new AURenderEventHeader { EventSampleTime = 1 });
			yield return Case<AUParameterObserverToken> (new AUParameterObserverToken (new IntPtr (1)));
			yield return Case<AUParameterEvent> (new AUParameterEvent { Value = 1 });
			yield return Case<AURenderEvent> (new AURenderEvent { Head = new AURenderEventHeader { EventSampleTime = 1 } });
			yield return Case<AURecordedParameterEvent> (new AURecordedParameterEvent { Value = 1 });
			yield return Case<AUParameterAutomationEvent> (new AUParameterAutomationEvent { Value = 1 });

			yield return Case<CAFrameRateRange> (new CAFrameRateRange { Minimum = 1 });
			yield return Case<CATransform3D> ((a, b) => a.Equals (b),
				new CATransform3D { M11 = 1 }, new CATransform3D { M12 = 1 },
				new CATransform3D { M13 = 1 }, new CATransform3D { M14 = 1 },
				new CATransform3D { M21 = 1 }, new CATransform3D { M22 = 1 },
				new CATransform3D { M23 = 1 }, new CATransform3D { M24 = 1 },
				new CATransform3D { M31 = 1 }, new CATransform3D { M32 = 1 },
				new CATransform3D { M33 = 1 }, new CATransform3D { M34 = 1 },
				new CATransform3D { M41 = 1 }, new CATransform3D { M42 = 1 },
				new CATransform3D { M43 = 1 }, new CATransform3D { M44 = 1 });
			yield return Case<CFBundle.PackageInfo> (new CFBundle.PackageInfo (default (CFBundle.PackageType), "TEST"));
			yield return Case<CFSocketNativeHandle> ();
			yield return Case<CFStreamClientContext> (new CFStreamClientContext { Version = 1 });
			yield return Case<CFRange> (new CFRange (1, 0), new CFRange (0, 1));
			yield return Case<CGAffineTransformComponents> (new CGAffineTransformComponents { HorizontalShear = 1 });
			yield return Case<DispatchTime> (new DispatchTime (1));
			yield return Case<CGAffineTransform> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new CGAffineTransform { A = 1 }, new CGAffineTransform { B = 1 },
				new CGAffineTransform { C = 1 }, new CGAffineTransform { D = 1 },
				new CGAffineTransform { Tx = 1 }, new CGAffineTransform { Ty = 1 });
			yield return Case<CGBitmapParameters> (new CGBitmapParameters { Width = 1 });
			yield return Case<CGColorConversionInfoTriple> (new CGColorConversionInfoTriple { Transform = (CGColorConversionInfoTransformType) 1 });
			yield return Case<CGContentInfo> (new CGContentInfo { HasTransparency = true });
			yield return Case<CGContentToneMappingInfo> (new CGContentToneMappingInfo { Method = (CGToneMapping) 1 });
			yield return Case<CGPathElement> (new CGPathElement { Point1 = new CGPoint (1, 0) });
			yield return Case<CGPoint> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new CGPoint (1, 0), new CGPoint (0, 1));
			yield return Case<CGRect> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new CGRect (1, 0, 0, 0), new CGRect (0, 1, 0, 0),
				new CGRect (0, 0, 1, 0), new CGRect (0, 0, 0, 1));
			yield return Case<CGSize> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new CGSize (1, 0), new CGSize (0, 1));
			yield return Case<CGVector> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new CGVector (1, 0), new CGVector (0, 1));
			yield return Case<CLLocationCoordinate2D> (new CLLocationCoordinate2D (1, 0), new CLLocationCoordinate2D (0, 1));
			yield return Case<TextMarkupColor> (new TextMarkupColor (1, 0, 0, 0));
			yield return Case<CMTime> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new CMTime (1, 1), new CMTime (2, 1), new CMTime (1, 1, 1));
			yield return Case<CMSampleTimingInfo> (new CMSampleTimingInfo { Duration = new CMTime (1, 1) });
			yield return Case<CMTimeRange> (new CMTimeRange { Start = new CMTime (1, 1) });
			yield return Case<CMTimeMapping> (new CMTimeMapping { Source = new CMTimeRange { Start = new CMTime (1, 1) } });
			yield return Case<CMTimeScale> (new CMTimeScale (1));
			yield return Case<CMVideoDimensions> (new CMVideoDimensions (1, 0));
			yield return Case<CVTime> (
				new CVTime { TimeValue = 1 }, new CVTime { TimeScale = 1 },
				new CVTime { TimeFlags = CVTimeFlags.IsIndefinite });
			yield return Case<CVPlanarComponentInfo> (new CVPlanarComponentInfo { Offset = 1 });
			yield return Case<CVPlanarPixelBufferInfo> (new CVPlanarPixelBufferInfo { ComponentInfo = new CVPlanarComponentInfo [1] });
			yield return Case<CVPlanarPixelBufferInfo_YCbCrPlanar> (
				new CVPlanarPixelBufferInfo_YCbCrPlanar { ComponentInfoY = new CVPlanarComponentInfo { Offset = 1 } });
			yield return Case<CVPlanarPixelBufferInfo_YCbCrBiPlanar> (
				new CVPlanarPixelBufferInfo_YCbCrBiPlanar { ComponentInfoY = new CVPlanarComponentInfo { Offset = 1 } });
			yield return Case<CVTimeStamp> (new CVTimeStamp { VideoTime = 1 });
			yield return Case<CVSMPTETime> (new CVSMPTETime { Hours = 1 });
			yield return Case<CVFillExtendedPixelsCallBackData> (new CVFillExtendedPixelsCallBackData { Version = 1 });
#if !XAMCORE_5_0
			yield return Case<CVFillExtendedPixelsCallBackDataStruct> (new CVFillExtendedPixelsCallBackDataStruct { Version = 1 });
			yield return Case<BlockLiteral> ();
#endif
			// AudioBufferList, ObjCSuper, and the XAMCORE_5_0 BlockLiteral are ref structs and cannot be boxed or compared with Equals.
#if HAS_DARWIN
			yield return Case<KernelEvent> (new KernelEvent { Ident = new IntPtr (1) });
			yield return Case<TimeSpec> (new TimeSpec { Seconds = 1 });
#endif
			yield return Case<NSDecimal> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b, (NSDecimal) 1, (NSDecimal) 2);
			yield return Case<NSOperatingSystemVersion> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NSOperatingSystemVersion (1, 0, 0),
				new NSOperatingSystemVersion (0, 1, 0),
				new NSOperatingSystemVersion (0, 0, 1));
			yield return Case<NSRange> ((a, b) => a.Equals (b),
				new NSRange (1, 0), new NSRange (0, 1));

			yield return Case<GCExtendedGamepadSnapShotDataV100> (new GCExtendedGamepadSnapShotDataV100 { ButtonA = 1 });
			yield return Case<GCExtendedGamepadSnapshotData> (new GCExtendedGamepadSnapshotData { ButtonA = 1 });
			yield return Case<GCGamepadSnapShotDataV100> (new GCGamepadSnapShotDataV100 { ButtonA = 1 });
			yield return Case<GCMicroGamepadSnapShotDataV100> (new GCMicroGamepadSnapShotDataV100 { ButtonA = 1 });
			yield return Case<GCMicroGamepadSnapshotData> (new GCMicroGamepadSnapshotData { ButtonA = 1 });
			yield return Case<GCAcceleration> (new GCAcceleration { X = 1 });
			yield return Case<GCRotationRate> (new GCRotationRate { X = 1 });
			yield return Case<GCQuaternion> (new GCQuaternion { W = 1 });
			yield return Case<GCDualSenseAdaptiveTriggerPositionalAmplitudes> (new GCDualSenseAdaptiveTriggerPositionalAmplitudes (new [] { 1f }));
			yield return Case<GCDualSenseAdaptiveTriggerPositionalResistiveStrengths> (new GCDualSenseAdaptiveTriggerPositionalResistiveStrengths (new [] { 1f }));
			yield return Case<GCPoint2> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new GCPoint2 (1, 0), new GCPoint2 (0, 1));
			yield return Case<GKBox> (new GKBox { Min = Vector3.UnitX });
			yield return Case<GKQuad> (new GKQuad { Min = Vector2.UnitX });
			yield return Case<GKTriangle> (new GKTriangle { Points = new [] { Vector3.UnitX, Vector3.Zero, Vector3.Zero } });
			yield return Case<MKTileOverlayPath> (new MKTileOverlayPath { X = 1 });
			yield return Case<MKCoordinateSpan> (new MKCoordinateSpan (1, 0));
			yield return Case<MKCoordinateRegion> (new MKCoordinateRegion (new CLLocationCoordinate2D (1, 0), default (MKCoordinateSpan)));
			yield return Case<MKMapPoint> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new MKMapPoint (1, 0), new MKMapPoint (0, 1));
			yield return Case<MKMapSize> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new MKMapSize (1, 0), new MKMapSize (0, 1));
			yield return Case<MKMapRect> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new MKMapRect (1, 0, 0, 0), new MKMapRect (0, 1, 0, 0),
				new MKMapRect (0, 0, 1, 0), new MKMapRect (0, 0, 0, 1));

			yield return Case<MTLOrigin> (new MTLOrigin (1, 0, 0));
			yield return Case<MTLSize> (new MTLSize (1, 0, 0));
			yield return Case<MTLScissorRect> (new MTLScissorRect (1, 0, 0, 0));
			yield return Case<MTLViewport> (new MTLViewport (1, 0, 0, 0, 0, 0));
			yield return Case<MTLSamplePosition> (new MTLSamplePosition { X = 1 });
			yield return Case<MTLClearColor> (new MTLClearColor (1, 0, 0, 0));
			yield return Case<MTLRegion> (new MTLRegion (new MTLOrigin (1, 0, 0), default (MTLSize)));
			yield return Case<MTLClearValue> (new MTLClearValue (1d));
			yield return Case<MTLDispatchThreadgroupsIndirectArguments> (new MTLDispatchThreadgroupsIndirectArguments { ThreadGroupsPerGrid1 = 1 });
			yield return Case<MTLStageInRegionIndirectArguments> (new MTLStageInRegionIndirectArguments { StageInOrigin1 = 1 });
			yield return Case<MTLDrawPrimitivesIndirectArguments> (new MTLDrawPrimitivesIndirectArguments { VertexCount = 1 });
			yield return Case<MTLDrawIndexedPrimitivesIndirectArguments> (new MTLDrawIndexedPrimitivesIndirectArguments { IndexCount = 1 });
			yield return Case<MTLSizeAndAlign> (new MTLSizeAndAlign (1, 0));
			yield return Case<MTLDrawPatchIndirectArguments> (new MTLDrawPatchIndirectArguments (1, 0, 0, 0));
			yield return Case<MTLQuadTessellationFactorsHalf> (new MTLQuadTessellationFactorsHalf (new ushort [] { 1, 0, 0, 0 }, new ushort [2]));
			yield return Case<MTLTriangleTessellationFactorsHalf> (new MTLTriangleTessellationFactorsHalf (new ushort [] { 1, 0, 0 }, 0));
			yield return Case<MTLTextureSwizzleChannels> (new MTLTextureSwizzleChannels { Red = MTLTextureSwizzle.One });
			yield return Case<MTLVertexAmplificationViewMapping> (new MTLVertexAmplificationViewMapping { ViewportArrayIndexOffset = 1 });
			yield return Case<MTLCoordinate2D> (new MTLCoordinate2D { X = 1 });
			yield return Case<MTLAccelerationStructureSizes> (new MTLAccelerationStructureSizes { AccelerationStructureSize = 1 });
			yield return Case<MTLResourceId> (new MTLResourceId { Impl = 1 });
			yield return Case<MTL4BufferRange> (new MTL4BufferRange (1, 1));
			yield return Case<MTL4CopySparseBufferMappingOperation> (new MTL4CopySparseBufferMappingOperation { DestinationOffset = 1 });
			yield return Case<MTL4CopySparseTextureMappingOperation> (new MTL4CopySparseTextureMappingOperation { SourceLevel = 1 });
			yield return Case<MTL4UpdateSparseBufferMappingOperation> (new MTL4UpdateSparseBufferMappingOperation { HeapOffset = 1 });
			yield return Case<MTL4UpdateSparseTextureMappingOperation> (new MTL4UpdateSparseTextureMappingOperation { HeapOffset = 1 });
			yield return Case<MPSOffset> (new MPSOffset { X = 1 });
			yield return Case<MPSOrigin> (new MPSOrigin { X = 1 });
			yield return Case<MPSSize> (new MPSSize { Width = 1 });
			yield return Case<MPSDimensionSlice> (new MPSDimensionSlice { Start = 1 });
			yield return Case<MPSRegion> (new MPSRegion { Origin = new MPSOrigin { X = 1 } });
			yield return Case<MPSScaleTransform> (new MPSScaleTransform { ScaleX = 1 });
			yield return Case<MPSImageCoordinate> (new MPSImageCoordinate { X = 1 });
			yield return Case<MPSImageRegion> (new MPSImageRegion { Offset = new MPSImageCoordinate { X = 1 } });
			yield return Case<MPSImageHistogramInfo> (new MPSImageHistogramInfo { NumberOfHistogramEntries = 1 });
			yield return Case<MPSMatrixCopyOffsets> (new MPSMatrixCopyOffsets { SourceRowOffset = 1 });
			yield return Case<MPSImageReadWriteParams> (new MPSImageReadWriteParams { FeatureChannelOffset = 1 });
			yield return Case<MPSImageKeypointRangeInfo> (new MPSImageKeypointRangeInfo { MaximumKeypoints = 1 });
			yield return Case<MPSStateTextureInfo> (new MPSStateTextureInfo { Width = 1 });
			yield return Case<MPSAxisAlignedBoundingBox> (new MPSAxisAlignedBoundingBox { Min = Vector3.UnitX });
			yield return Case<MPSFunctionsAxisAlignedBoundingBox> (new MPSFunctionsAxisAlignedBoundingBox { Min = Vector4.UnitX });
			yield return Case<MPSMatrixOffset> (new MPSMatrixOffset { RowOffset = 1 });
			var offsets = new nint [16];
			offsets [0] = 1;
			yield return Case<MPSNDArrayOffsets> (new MPSNDArrayOffsets { Dimensions = offsets });
			var sizes = new nuint [16];
			sizes [0] = 1;
			yield return Case<MPSNDArraySizes> (new MPSNDArraySizes { Dimensions = sizes });
			yield return Case<MDLAxisAlignedBoundingBox> (new MDLAxisAlignedBoundingBox (Vector3.UnitX, Vector3.Zero));
			yield return Case<MDLVoxelIndexExtent> (new MDLVoxelIndexExtent (new NVector4i (1, 0, 0, 0), default (NVector4i)));
			yield return Case<NativeHandle> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b, new NativeHandle (new IntPtr (1)));
#if HAS_PASSKIT
			yield return Case<global::PassKit.PKMerchantCategoryCode> (new global::PassKit.PKMerchantCategoryCode (1));
#endif
			yield return Case<VTDataRateLimit> (new VTDataRateLimit (1, 1));

			yield return Case<SCNVector3> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new SCNVector3 { X = 1 }, new SCNVector3 { Y = 1 }, new SCNVector3 { Z = 1 });
			yield return Case<SCNVector4> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new SCNVector4 { X = 1 }, new SCNVector4 { Y = 1 },
				new SCNVector4 { Z = 1 }, new SCNVector4 { W = 1 });
			yield return Case<SCNQuaternion> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new SCNQuaternion { X = 1 }, new SCNQuaternion { Y = 1 },
				new SCNQuaternion { Z = 1 }, new SCNQuaternion { W = 1 });
			yield return Case<SCNMatrix4> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new SCNMatrix4 { M11 = 1 }, new SCNMatrix4 { M12 = 1 },
				new SCNMatrix4 { M13 = 1 }, new SCNMatrix4 { M14 = 1 },
				new SCNMatrix4 { M21 = 1 }, new SCNMatrix4 { M22 = 1 },
				new SCNMatrix4 { M23 = 1 }, new SCNMatrix4 { M24 = 1 },
				new SCNMatrix4 { M31 = 1 }, new SCNMatrix4 { M32 = 1 },
				new SCNMatrix4 { M33 = 1 }, new SCNMatrix4 { M34 = 1 },
				new SCNMatrix4 { M41 = 1 }, new SCNMatrix4 { M42 = 1 },
				new SCNMatrix4 { M43 = 1 }, new SCNMatrix4 { M44 = 1 });
			yield return Case<NMatrix2> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NMatrix2 { M11 = 1 }, new NMatrix2 { M12 = 1 },
				new NMatrix2 { M21 = 1 }, new NMatrix2 { M22 = 1 });
			yield return Case<NMatrix3> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NMatrix3 { M11 = 1 }, new NMatrix3 { M12 = 1 }, new NMatrix3 { M13 = 1 },
				new NMatrix3 { M21 = 1 }, new NMatrix3 { M22 = 1 }, new NMatrix3 { M23 = 1 },
				new NMatrix3 { M31 = 1 }, new NMatrix3 { M32 = 1 }, new NMatrix3 { M33 = 1 });
			yield return Case<RMatrix3> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new RMatrix3 { M11 = 1 }, new RMatrix3 { M12 = 1 }, new RMatrix3 { M13 = 1 },
				new RMatrix3 { M21 = 1 }, new RMatrix3 { M22 = 1 }, new RMatrix3 { M23 = 1 },
				new RMatrix3 { M31 = 1 }, new RMatrix3 { M32 = 1 }, new RMatrix3 { M33 = 1 });
			yield return Case<NMatrix4x3> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NMatrix4x3 { M11 = 1 }, new NMatrix4x3 { M12 = 1 }, new NMatrix4x3 { M13 = 1 }, new NMatrix4x3 { M14 = 1 },
				new NMatrix4x3 { M21 = 1 }, new NMatrix4x3 { M22 = 1 }, new NMatrix4x3 { M23 = 1 }, new NMatrix4x3 { M24 = 1 },
				new NMatrix4x3 { M31 = 1 }, new NMatrix4x3 { M32 = 1 }, new NMatrix4x3 { M33 = 1 }, new NMatrix4x3 { M34 = 1 });
			yield return Case<NMatrix4> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NMatrix4 { M11 = 1 }, new NMatrix4 { M12 = 1 },
				new NMatrix4 { M13 = 1 }, new NMatrix4 { M14 = 1 },
				new NMatrix4 { M21 = 1 }, new NMatrix4 { M22 = 1 },
				new NMatrix4 { M23 = 1 }, new NMatrix4 { M24 = 1 },
				new NMatrix4 { M31 = 1 }, new NMatrix4 { M32 = 1 },
				new NMatrix4 { M33 = 1 }, new NMatrix4 { M34 = 1 },
				new NMatrix4 { M41 = 1 }, new NMatrix4 { M42 = 1 },
				new NMatrix4 { M43 = 1 }, new NMatrix4 { M44 = 1 });
			yield return Case<NMatrix4d> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NMatrix4d { M11 = 1 }, new NMatrix4d { M12 = 1 },
				new NMatrix4d { M13 = 1 }, new NMatrix4d { M14 = 1 },
				new NMatrix4d { M21 = 1 }, new NMatrix4d { M22 = 1 },
				new NMatrix4d { M23 = 1 }, new NMatrix4d { M24 = 1 },
				new NMatrix4d { M31 = 1 }, new NMatrix4d { M32 = 1 },
				new NMatrix4d { M33 = 1 }, new NMatrix4d { M34 = 1 },
				new NMatrix4d { M41 = 1 }, new NMatrix4d { M42 = 1 },
				new NMatrix4d { M43 = 1 }, new NMatrix4d { M44 = 1 });
			yield return Case<NQuaterniond> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NQuaterniond { X = 1 }, new NQuaterniond { Y = 1 },
				new NQuaterniond { Z = 1 }, new NQuaterniond { W = 1 });
			yield return Case<NVector16b> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b, ByteVectors ());
			yield return Case<NVector2d> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NVector2d { X = 1 }, new NVector2d { Y = 1 });
			yield return Case<NVector3d> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NVector3d { X = 1 }, new NVector3d { Y = 1 }, new NVector3d { Z = 1 });
			yield return Case<NVector4d> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NVector4d { X = 1 }, new NVector4d { Y = 1 },
				new NVector4d { Z = 1 }, new NVector4d { W = 1 });
			yield return Case<NVector3> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NVector3 { X = 1 }, new NVector3 { Y = 1 }, new NVector3 { Z = 1 });
			yield return Case<NVector2i> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NVector2i { X = 1 }, new NVector2i { Y = 1 });
			yield return Case<NVector3i> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NVector3i { X = 1 }, new NVector3i { Y = 1 }, new NVector3i { Z = 1 });
			yield return Case<NVector4i> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NVector4i { X = 1 }, new NVector4i { Y = 1 },
				new NVector4i { Z = 1 }, new NVector4i { W = 1 });

#if !TVOS
			yield return Case<MidiCIDeviceIdentification> (new MidiCIDeviceIdentification { Manufacturer = new byte [] { 1, 0, 0 } });
			yield return Case<Midi2DeviceManufacturer> (new Midi2DeviceManufacturer { SysExIdByte = new byte [] { 1, 0, 0 } });
			yield return Case<Midi2DeviceRevisionLevel> (new Midi2DeviceRevisionLevel { RevisionLevel = new byte [] { 1, 0, 0, 0 } });
			yield return Case<MidiCIProfileIdStandard> (new MidiCIProfileIdStandard { ProfileIdByte1 = 1 });
			yield return Case<MidiCIProfileIdManufacturerSpecific> (new MidiCIProfileIdManufacturerSpecific { SysExId1 = 1 });
			yield return Case<MidiCIProfileId> (new MidiCIProfileId { Standard = new MidiCIProfileIdStandard { ProfileIdByte1 = 1 } });
			yield return Case<MidiTransform> (new MidiTransform { Param = 1 });
			yield return Case<MidiValueMap> (new MidiValueMap { Value = new byte [128] });
			yield return Case<MidiControlTransform> (new MidiControlTransform { Param = 1 });
			yield return Case<MidiThruConnectionEndpoint> (new MidiThruConnectionEndpoint (1, 1));
			yield return Case<CMCalibratedMagneticField> (new CMCalibratedMagneticField { Field = new CMMagneticField { X = 1 } });
			yield return Case<CMMagneticField> (new CMMagneticField { X = 1 });
			yield return Case<CMAcceleration> (new CMAcceleration (1, 0, 0));
			yield return Case<CMRotationMatrix> (new CMRotationMatrix { m11 = 1 });
			yield return Case<CMQuaternion> (new CMQuaternion (1, 0, 0, 0));
			yield return Case<CMRotationRate> (new CMRotationRate (1, 0, 0));
#endif
#if HAS_GLKIT
			yield return Case<global::GLKit.GLKVertexAttributeParameters> (new global::GLKit.GLKVertexAttributeParameters { Size = 1 });
#endif

#if MONOMAC || __MACCATALYST__
			yield return Case<global::AppKit.NSEdgeInsets> (new global::AppKit.NSEdgeInsets (1, 0, 0, 0));
			yield return Case<CGEventTapInformation> (new CGEventTapInformation { EventTapID = 1 });
#endif
#if MONOMAC
			yield return Case<MTLIndirectCommandBufferExecutionRange> (new MTLIndirectCommandBufferExecutionRange (1, 0));
			yield return Case<global::CoreServices.FSEvent> ();
			yield return Case<PMResolution> (new PMResolution (1, 0));
			yield return Case<PMRect> (new PMRect (1, 0, 0, 0));
			yield return Case<PMPaperMargins> (new PMPaperMargins (1, 0, 0, 0));
			yield return Case<global::Security.AuthorizationCallbacks> ();
			yield return Case<global::Security.SecKeychainSettings> (new global::Security.SecKeychainSettings { LockOnSleep = true });
			yield return Case<NSDirectionalEdgeInsets> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NSDirectionalEdgeInsets { Top = 1 }, new NSDirectionalEdgeInsets { Leading = 1 },
				new NSDirectionalEdgeInsets { Bottom = 1 }, new NSDirectionalEdgeInsets { Trailing = 1 });
#else
			yield return Case<UIOffset> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new UIOffset { Horizontal = 1 }, new UIOffset { Vertical = 1 });
			yield return Case<UIEdgeInsets> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new UIEdgeInsets { Top = 1 }, new UIEdgeInsets { Left = 1 },
				new UIEdgeInsets { Bottom = 1 }, new UIEdgeInsets { Right = 1 });
			yield return Case<UIFloatRange> ((a, b) => a.Equals (b), new UIFloatRange (1, 0), new UIFloatRange (0, 1));
			yield return Case<NSDirectionalEdgeInsets> ((a, b) => a.Equals (b), (a, b) => a == b, (a, b) => a != b,
				new NSDirectionalEdgeInsets { Top = 1 }, new NSDirectionalEdgeInsets { Leading = 1 },
				new NSDirectionalEdgeInsets { Bottom = 1 }, new NSDirectionalEdgeInsets { Trailing = 1 });
#if HAS_BROWSERENGINEKIT
			yield return Case<global::BrowserEngineKit.BEDirectionalTextRange> (new global::BrowserEngineKit.BEDirectionalTextRange { Offset = 1 });
#endif
#if !TVOS
			yield return Case<UIPointerAccessoryPosition> (new UIPointerAccessoryPosition { Offset = 1 });
			yield return Case<global::SensorKit.SRAmbientLightChromaticity> (new global::SensorKit.SRAmbientLightChromaticity { X = 1 });
#if HAS_ADDRESSBOOK
			yield return Case<global::AddressBook.ABMultiValueEntry<NSString>> ();
#endif
#if HAS_CARPLAY
			yield return Case<global::CarPlay.CPLocationCoordinate3D> (new global::CarPlay.CPLocationCoordinate3D { Latitude = 1 });
#endif
#endif
#endif

#if MONOMAC || (__IOS__ && !__MACCATALYST__) || (__MACCATALYST__ && !XAMCORE_5_0)
			yield return Case<global::FileProvider.NSFileProviderTypeAndCreator> (new global::FileProvider.NSFileProviderTypeAndCreator { Type = 1 });
#endif
		}

		static NVector16b [] ByteVectors ()
		{
			var values = new NVector16b [16];
			for (var i = 0; i < values.Length; i++)
				values [i] [i] = 1;
			return values;
		}

		[Test]
		public void Tag ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);
			Check<CMTag> (CMTag.Equals, null, null, CMTag.MediaTypeVideo, CMTag.MediaTypeAudio);
		}

#if !(XAMCORE_5_0 && MONOMAC)
		[Test]
		public void AudioQueueChannelAssignmentValues ()
		{
			using var device = new CFString ("test-device");
			Check<AudioQueueChannelAssignment> (null, null, null,
				new AudioQueueChannelAssignment (device, 1),
				new AudioQueueChannelAssignment (device, 2));
		}
#endif

		[Test]
		public void NativeHandleIntPtrOperators ()
		{
			var values = new [] { IntPtr.Zero, new IntPtr (1), new IntPtr (2) };
			foreach (var left in values) {
				var handle = new NativeHandle (left);
				foreach (var right in values) {
					var expected = left == right;
					Assert.That (handle == right, Is.EqualTo (expected), $"NativeHandle == IntPtr: {left}, {right}");
					Assert.That (right == handle, Is.EqualTo (expected), $"IntPtr == NativeHandle: {right}, {left}");
					Assert.That (handle != right, Is.EqualTo (!expected), $"NativeHandle != IntPtr: {left}, {right}");
					Assert.That (right != handle, Is.EqualTo (!expected), $"IntPtr != NativeHandle: {right}, {left}");
				}
			}
		}

		[Test]
		public void NativeHandleNullableIntPtrOperators ()
		{
			var values = new IntPtr? [] { null, IntPtr.Zero, new IntPtr (1), new IntPtr (2) };
			foreach (var left in values) {
				NativeHandle? handle = left.HasValue ? new NativeHandle (left.Value) : null;
				foreach (var right in values) {
					var expected = left == right;
					Assert.That (handle == right, Is.EqualTo (expected), $"NativeHandle? == IntPtr?: {left}, {right}");
					Assert.That (right == handle, Is.EqualTo (expected), $"IntPtr? == NativeHandle?: {right}, {left}");
					Assert.That (handle != right, Is.EqualTo (!expected), $"NativeHandle? != IntPtr?: {left}, {right}");
					Assert.That (right != handle, Is.EqualTo (!expected), $"IntPtr? != NativeHandle?: {right}, {left}");
				}
			}
		}
	}
}
