using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime {
	static partial class Messaging {
		internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

#if !COREBUILD
		internal delegate IntPtr VariadicFunction19 (IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10, IntPtr arg11, IntPtr arg12, IntPtr arg13, IntPtr arg14, IntPtr arg15, IntPtr arg16, IntPtr arg17, IntPtr arg18, IntPtr arg19);

		/* Generic variadic function */

		// Call the specified variadic function with 0 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction0 (VariadicFunction19 function, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call the specified variadic function with 1 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction1 (VariadicFunction19 function, IntPtr arg1, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (1, arg1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call the specified variadic function with 2 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction2 (VariadicFunction19 function, IntPtr arg1, IntPtr arg2, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (2, arg1, arg2, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call the specified variadic function with 3 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction3 (VariadicFunction19 function, IntPtr arg1, IntPtr arg2, IntPtr arg3, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (3, arg1, arg2, arg3, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call the specified variadic function with 4 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction4 (VariadicFunction19 function, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (4, arg1, arg2, arg3, arg4, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call the specified variadic function with 5 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction5 (VariadicFunction19 function, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (5, arg1, arg2, arg3, arg4, arg5, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call the specified variadic function with 6 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction6 (VariadicFunction19 function, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (6, arg1, arg2, arg3, arg4, arg5, arg6, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call the specified variadic function with 7 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction7 (VariadicFunction19 function, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (7, arg1, arg2, arg3, arg4, arg5, arg6, arg7, IntPtr.Zero, varArguments));
		}

		// Call the specified variadic function with 8 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr CallVariadicFunction8 (VariadicFunction19 function, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, params IntPtr [] varArguments)
		{
			return CallVariadicFunction (function, ShuffleVarArgs (8, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, varArguments));
		}

		/* Variadic versions of objc_msgSend */

		// Call objc_msgSend with 2 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr objc_msgSend_2_vargs (IntPtr arg1, IntPtr arg2, params IntPtr [] varArguments)
		{
			return CallObjCMsgSendVarArgs (ShuffleVarArgs (2, arg1, arg2, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call objc_msgSend with 3 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr objc_msgSend_3_vargs (IntPtr arg1, IntPtr arg2, IntPtr arg3, params IntPtr [] varArguments)
		{
			return CallObjCMsgSendVarArgs (ShuffleVarArgs (3, arg1, arg2, arg3, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call objc_msgSend with 4 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr objc_msgSend_4_vargs (IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, params IntPtr [] varArguments)
		{
			return CallObjCMsgSendVarArgs (ShuffleVarArgs (4, arg1, arg2, arg3, arg4, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call objc_msgSend with 5 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr objc_msgSend_5_vargs (IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, params IntPtr [] varArguments)
		{
			return CallObjCMsgSendVarArgs (ShuffleVarArgs (5, arg1, arg2, arg3, arg4, arg5, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call objc_msgSend with 6 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr objc_msgSend_6_vargs (IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, params IntPtr [] varArguments)
		{
			return CallObjCMsgSendVarArgs (ShuffleVarArgs (6, arg1, arg2, arg3, arg4, arg5, arg6, IntPtr.Zero, IntPtr.Zero, varArguments));
		}

		// Call objc_msgSend with 7 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr objc_msgSend_7_vargs (IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, params IntPtr [] varArguments)
		{
			return CallObjCMsgSendVarArgs (ShuffleVarArgs (7, arg1, arg2, arg3, arg4, arg5, arg6, arg7, IntPtr.Zero, varArguments));
		}

		// Call objc_msgSend with 8 non-variadic arguments, and up to 10 (MaxVarArgs) variadic arguments.
		// An additional IntPtr.Zero is always passed at the end to terminate any varargs arrays.
		internal static IntPtr objc_msgSend_8_vargs (IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, params IntPtr [] varArguments)
		{
			return CallObjCMsgSendVarArgs (ShuffleVarArgs (8, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, varArguments));
		}

		static internal int MaxVarArgs = 10;

		static IntPtr CallObjCMsgSendVarArgs (IntPtr [] shuffledArguments)
		{
			return CallVariadicFunction (objc_msgSend_variadic, shuffledArguments);
		}

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static IntPtr objc_msgSend_variadic (IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10, IntPtr arg11, IntPtr arg12, IntPtr arg13, IntPtr arg14, IntPtr arg15, IntPtr arg16, IntPtr arg17, IntPtr arg18, IntPtr arg19);

		// This method creates an array of normal arguments to pass to objc_msgSend given the specified normal arguments + varargs arguments.
		static IntPtr [] ShuffleVarArgs (int argCount, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, params IntPtr [] varArguments)
		{
			if (varArguments.Length > MaxVarArgs)
				throw new ArgumentOutOfRangeException ($"A maximum of {MaxVarArgs} variadic arguments is supported.");

			var rv = new IntPtr [8 + MaxVarArgs];

			rv [0] = arg1;
			rv [1] = arg2;
			rv [2] = arg3;
			rv [3] = arg4;
			rv [4] = arg5;
			rv [5] = arg6;
			rv [6] = arg7;
			rv [7] = arg8;

			// ARM64: 8 arguments in registers, the rest is on the stack. This is where iOS/ARM64 expects the first varargs arguments.
			var varArgsStartIndex = Runtime.IsARM64CallingConvention ? 8 : argCount;

			Array.Copy (varArguments, 0, rv, varArgsStartIndex, varArguments.Length);

			return rv;
		}

		static IntPtr CallVariadicFunction (VariadicFunction19 function, IntPtr [] shuffledArguments)
		{
			return function (
				shuffledArguments [0],
				shuffledArguments [1],
				shuffledArguments [2],
				shuffledArguments [3],
				shuffledArguments [4],
				shuffledArguments [5],
				shuffledArguments [6],
				shuffledArguments [7],
				shuffledArguments [8],
				shuffledArguments [9],
				shuffledArguments [10],
				shuffledArguments [11],
				shuffledArguments [12],
				shuffledArguments [13],
				shuffledArguments [14],
				shuffledArguments [15],
				shuffledArguments [16],
				shuffledArguments [17],
				IntPtr.Zero // An additional null pointer since many variadic lists arrays end with a null pointer.
			);
		}
#endif // !COREBUILD
	}
}
