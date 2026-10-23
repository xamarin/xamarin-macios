// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

using ObjCRuntime;
using TVServices;

namespace MonotouchTest.AudioUnitExtensionHost {
	[Register ("MonotouchTestTopShelfProvider")]
	public class MonotouchTestTopShelfProvider : TVTopShelfContentProvider {
		public MonotouchTestTopShelfProvider (NativeHandle handle) : base (handle)
		{
		}

		public override void LoadTopShelfContent (Action<ITVTopShelfContent> completionHandler)
		{
			ExtensionTestHost.InstallDebugHooks ();
			Task.Run (async () => {
				await ExtensionTestHost.RunOnce ();
				completionHandler (null);
			});
		}
	}
}
