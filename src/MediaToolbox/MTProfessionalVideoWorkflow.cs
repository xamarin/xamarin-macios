#if MONOMAC

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace MediaToolbox {

	static public class MTProfessionalVideoWorkflow {

#if NET
		[SupportedOSPlatform ("macos10.10")]
#else
		[Mac (10,10)]
#endif
		[DllImport (Constants.MediaToolboxLibrary, EntryPoint = "MTRegisterProfessionalVideoWorkflowFormatReaders")]
		public static extern void RegisterFormatReaders ();
	}
}

#endif
