#if MONOMAC

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace MediaToolbox {

	static public class MTProfessionalVideoWorkflow {

		[Mac (10,10)]
		[DllImport (Constants.MediaToolboxLibrary, EntryPoint = "MTRegisterProfessionalVideoWorkflowFormatReaders")]
		public static extern void RegisterFormatReaders ();
	}
}

#endif
