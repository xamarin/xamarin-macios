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
		static HashSet<string> knownFailuresPInvokes = new HashSet<string> {
			"AVFoundation.AVSampleCursorAudioDependencyInfo ObjCRuntime.Messaging::AVSampleCursorAudioDependencyInfo_objc_msgSend(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorAudioDependencyInfo ObjCRuntime.Messaging::AVSampleCursorAudioDependencyInfo_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorChunkInfo ObjCRuntime.Messaging::AVSampleCursorChunkInfo_objc_msgSend_stret(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorChunkInfo ObjCRuntime.Messaging::AVSampleCursorChunkInfo_objc_msgSend(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorChunkInfo ObjCRuntime.Messaging::AVSampleCursorChunkInfo_objc_msgSendSuper_stret(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorChunkInfo ObjCRuntime.Messaging::AVSampleCursorChunkInfo_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorSyncInfo ObjCRuntime.Messaging::AVSampleCursorSyncInfo_objc_msgSend_stret(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorSyncInfo ObjCRuntime.Messaging::AVSampleCursorSyncInfo_objc_msgSend(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorSyncInfo ObjCRuntime.Messaging::AVSampleCursorSyncInfo_objc_msgSendSuper_stret(System.IntPtr,System.IntPtr)",
			"AVFoundation.AVSampleCursorSyncInfo ObjCRuntime.Messaging::AVSampleCursorSyncInfo_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSend_GCDualSenseAdaptiveTriggerPositionalAmplitudes_float(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalAmplitudes,System.Single)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSend_GCDualSenseAdaptiveTriggerPositionalResistiveStrengths(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalResistiveStrengths)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSendSuper_GCDualSenseAdaptiveTriggerPositionalAmplitudes_float(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalAmplitudes,System.Single)",
			"System.Void ObjCRuntime.Messaging::void_objc_msgSendSuper_GCDualSenseAdaptiveTriggerPositionalResistiveStrengths(System.IntPtr,System.IntPtr,GameController.GCDualSenseAdaptiveTriggerPositionalResistiveStrengths)",
		};
	}
}
