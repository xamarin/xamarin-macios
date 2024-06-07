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
		};
#endif
		static HashSet<string> knownFailuresPInvokes = new HashSet<string> {
			"System.Byte GameController.GCExtendedGamepadSnapshot::GCExtendedGamepadSnapshotDataFromNSData(GameController.GCExtendedGamepadSnapshotData*,System.IntPtr)",
			"System.IntPtr GameController.GCExtendedGamepadSnapshotData::NSDataFromGCExtendedGamepadSnapshotData(GameController.GCExtendedGamepadSnapshotData*)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSend_GCDualSenseAdaptiveTriggerPositionalAmplitudes_float(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalAmplitudes,System.Single)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSend_GCDualSenseAdaptiveTriggerPositionalResistiveStrengths(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalResistiveStrengths)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSendSuper_GCDualSenseAdaptiveTriggerPositionalAmplitudes_float(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalAmplitudes,System.Single)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSendSuper_GCDualSenseAdaptiveTriggerPositionalResistiveStrengths(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalResistiveStrengths)",
		};
	}
}
