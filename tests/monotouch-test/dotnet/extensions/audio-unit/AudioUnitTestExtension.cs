// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

using AudioToolbox;
using AudioUnit;
using AVFoundation;
using Foundation;
using ObjCRuntime;

namespace MonotouchTest.AudioUnitExtensionHost {
	[Register ("MonotouchTestAudioUnitFactory")]
	public class MonotouchTestAudioUnitFactory : NSObject, IAUAudioUnitFactory {
		public MonotouchTestAudioUnitFactory (NativeHandle handle) : base (handle)
		{
		}

		public AUAudioUnit CreateAudioUnit (AudioComponentDescription desc, out NSError error)
		{
			ExtensionTestHost.InstallDebugHooks ();
			error = null;
			var audioUnit = new MonotouchTestAudioUnit (desc, out error);
			if (error is null)
				Task.Run (async () => {
					await ExtensionTestHost.RunOnce ();
				});
			return audioUnit;
		}

		[Export ("beginRequestWithExtensionContext:")]
		public void BeginRequestWithExtensionContext (NSExtensionContext context)
		{
			ExtensionTestHost.InstallDebugHooks ();
			Task.Run (async () => await ExtensionTestHost.RunOnce ());
		}
	}

	[Register ("MonotouchTestAudioUnit")]
	public class MonotouchTestAudioUnit : AUAudioUnit {
		AUAudioUnitBusArray inputBusArray;
		AUAudioUnitBusArray outputBusArray;

		public MonotouchTestAudioUnit (AudioComponentDescription componentDescription, out NSError error)
			: base (componentDescription, AudioComponentInstantiationOptions.OutOfProcess, out error)
		{
			var format = new AVAudioFormat (44100, 2);
			var inputBus = new AUAudioUnitBus (format, out error);
			var outputBus = new AUAudioUnitBus (format, out error);
			inputBusArray = new AUAudioUnitBusArray (this, AUAudioUnitBusType.Input, new [] { inputBus });
			outputBusArray = new AUAudioUnitBusArray (this, AUAudioUnitBusType.Output, new [] { outputBus });
		}

		public MonotouchTestAudioUnit (NativeHandle handle) : base (handle)
		{
		}

		public override AUAudioUnitBusArray InputBusses => inputBusArray;

		public override AUAudioUnitBusArray OutputBusses => outputBusArray;

		public override AUInternalRenderBlock InternalRenderBlock {
			get {
				return (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp,
						uint frameCount, nint outputBusNumber, AudioBuffers outputData,
						AURenderEventEnumerator realtimeEventListHead, AURenderPullInputBlock pullInputBlock) => {
							if (pullInputBlock is null)
								return AudioUnitStatus.NoError;
							pullInputBlock (ref actionFlags, ref timestamp, frameCount, 0, outputData);
							return AudioUnitStatus.NoError;
						};
			}
		}
	}
}
