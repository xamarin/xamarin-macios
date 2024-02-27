//
// Copyright 2011, Kenneth J. Pouncey
//
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
using System;

namespace ScriptingBridge {
	// AE.framework/Headers/AEDataModel.h:typedef SInt32 AESendMode;
	[Flags]
	public enum AESendMode : int {
		NoReply = 0x00000001,
		QueueReply = 0x00000002,
		WaitReply = 0x00000003,
		DontReconnect = 0x00000080,
		WantReceipt = 0x00000200,
		NeverInteract = 0x00000010,
		CanInteract = 0x00000020,
		AlwaysInteract = 0x00000030,
		CanSwitchLayer = 0x00000040,
		DontRecord = 0x00001000,
		DontExecute = 0x00002000,
		ProcessNonReplyEvents = 0x00008000
	}

	// LaunchServices.framework/Headers/LSOpen.h:typedef OptionBits LSLaunchFlags;
	// DirectoryService.framework/Headers/DirServicesTypes.h:typedef UInt32 OptionBits;
	[Flags]
	public enum LSLaunchFlags : uint {
		LaunchDefaults = 0x00000001,
		LaunchAndPrint = 0x00000002,
		LaunchReserved2 = 0x00000004,
		LaunchReserved3 = 0x00000008,
		LaunchReserved4 = 0x00000010,
		LaunchReserved5 = 0x00000020,
		LaunchAndDisplayErrors = 0x00000040,
		LaunchInhibitBGOnly = 0x00000080,
		LaunchDontAddToRecents = 0x00000100,
		LaunchDontSwitch = 0x00000200,
		LaunchNoParams = 0x00000800,
		LaunchAsync = 0x00010000,
		LaunchStartClassic = 0x00020000,
		LaunchInClassic = 0x00040000,
		LaunchNewInstance = 0x00080000,
		LaunchAndHide = 0x00100000,
		LaunchAndHideOthers = 0x00200000,
		LaunchHasUntrustedContents = 0x00400000
	}

}
