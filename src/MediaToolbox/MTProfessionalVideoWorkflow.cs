#if MONOMAC

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace MediaToolbox {

	static public class MTProfessionalVideoWorkflow {

#if !NET
		[Mac (10,10)]
#endif
		[DllImport (Constants.MediaToolboxLibrary, EntryPoint = "MTRegisterProfessionalVideoWorkflowFormatReaders")]
		public static extern void RegisterFormatReaders ();
	}
}

#endif
