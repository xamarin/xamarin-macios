using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;

using ObjCRuntime;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {
	public partial class BlittablePInvokes {
#if XAMCORE_5_0
		static HashSet<string> knownFailuresMarshalAs = new (); // shouldn't have any failures here in XAMCORE_5_0.
#else
		static HashSet<string> knownFailuresMarshalAs = new HashSet<string> {
			"For the method ImageIO.CGImageMetadataTagBlock::EndInvoke(System.IAsyncResult) the return type has a [MarshalAs] attribute",
			"For the method ImageIO.CGImageMetadataTagBlock::Invoke(Foundation.NSString,ImageIO.CGImageMetadataTag) the return type has a [MarshalAs] attribute",
			"For the method Network.NWFramerParseCompletionDelegate::BeginInvoke(System.Memory`1<System.Byte>,System.Boolean,System.AsyncCallback,System.Object) the parameter #1 (isCompleted) has a [MarshalAs] attribute",
			"For the method Network.NWFramerParseCompletionDelegate::Invoke(System.Memory`1<System.Byte>,System.Boolean) the parameter #1 (isCompleted) has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorAudioDependencyInfo.IsIndependentlyDecodable has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorChunkInfo.HasUniformFormatDescriptions has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorChunkInfo.HasUniformSampleDurations has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorChunkInfo.HasUniformSampleSizes has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorDependencyInfo.DependsOnOthers has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorDependencyInfo.HasDependentSamples has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorDependencyInfo.HasRedundantCoding has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorDependencyInfo.IndicatesWhetherItDependsOnOthers has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorDependencyInfo.IndicatesWhetherItHasDependentSamples has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorDependencyInfo.IndicatesWhetherItHasRedundantCoding has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorSyncInfo.IsDroppable has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorSyncInfo.IsFullSync has a [MarshalAs] attribute",
			"The field AVFoundation.AVSampleCursorSyncInfo.IsPartialSync has a [MarshalAs] attribute",
			"The field CoreMidi.MidiCIDeviceIdentification.Family has a [MarshalAs] attribute",
			"The field CoreMidi.MidiCIDeviceIdentification.Manufacturer has a [MarshalAs] attribute",
			"The field CoreMidi.MidiCIDeviceIdentification.ModelNumber has a [MarshalAs] attribute",
			"The field CoreMidi.MidiCIDeviceIdentification.Reserved has a [MarshalAs] attribute",
			"The field CoreMidi.MidiCIDeviceIdentification.RevisionLevel has a [MarshalAs] attribute",
			"The field GameController.GCDualSenseAdaptiveTriggerPositionalAmplitudes.Values has a [MarshalAs] attribute",
			"The field GameController.GCDualSenseAdaptiveTriggerPositionalResistiveStrengths.Values has a [MarshalAs] attribute",
			"The field GameController.GCExtendedGamepadSnapshotData.LeftThumbstickButton has a [MarshalAs] attribute",
			"The field GameController.GCExtendedGamepadSnapshotData.RightThumbstickButton has a [MarshalAs] attribute",
			"The field GameController.GCExtendedGamepadSnapshotData.SupportsClickableThumbsticks has a [MarshalAs] attribute",
			"The field GLKit.GLKVertexAttributeParameters.Normalized has a [MarshalAs] attribute",
			"The field Metal.MTLQuadTessellationFactorsHalf.EdgeTessellationFactor has a [MarshalAs] attribute",
			"The field Metal.MTLQuadTessellationFactorsHalf.InsideTessellationFactor has a [MarshalAs] attribute",
			"The field Metal.MTLTriangleTessellationFactorsHalf.EdgeTessellationFactor has a [MarshalAs] attribute",
			"The field ObjCRuntime.Class/objc_attribute_prop.name has a [MarshalAs] attribute",
			"The field ObjCRuntime.Class/objc_attribute_prop.value has a [MarshalAs] attribute",
		};
#endif
		static HashSet<string> knownFailuresPInvokes = new HashSet<string> {
			"System.Byte GameController.GCExtendedGamepadSnapshot::GCExtendedGamepadSnapshotDataFromNSData(GameController.GCExtendedGamepadSnapshotData*,System.IntPtr)",
			"System.IntPtr GameController.GCExtendedGamepadSnapshotData::NSDataFromGCExtendedGamepadSnapshotData(GameController.GCExtendedGamepadSnapshotData*)",
		};
	}
}
