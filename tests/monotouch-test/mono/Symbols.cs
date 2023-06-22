#if !MONOMAC
using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class Symbols {
		string [] symbols;

		[Test]
		public void FunctionNames ()
		{
			TestRuntime.AssertDevice ();

			Collect ();
			bool aot = symbols [1].Contains ("MonoTouchFixtures_Symbols_Collect");
			bool nativeaot = symbols [1].Contains ("MonoTouchFixtures_Symbols__Collect");
			bool llvmonly = symbols [1].Contains ("mono_llvmonly_runtime_invoke"); // LLVM inlines the Collect function, so 'Collect' doesn't show up in the stack trace :/
			bool interp = false;

			if (!aot) {
				for (int i = 0; i < 5 && !interp; i++) {
					/* ves_pinvoke_method (slow path) and do_icall (fast path) are
					 * MONO_NEVER_INLINE, so they should show up in the backtrace
					 * reliably */
					interp |= symbols [i].Contains ("ves_pinvoke_method") || symbols [i].Contains ("do_icall");
				}
			}

			Assert.IsTrue (aot || interp || llvmonly || nativeaot, $"#1\n\t{string.Join ("\n\t", symbols)}");
		}

		void Collect ()
		{
			var array = new IntPtr [50];
			var size = backtrace (array, array.Length);
			var symbols = backtrace_symbols (array, size);

			this.symbols = new string [size];
			for (int i = 0; i < size; i++) {
				this.symbols [i] = Marshal.PtrToStringAuto (Marshal.ReadIntPtr (symbols, i * IntPtr.Size));
				//				Console.WriteLine (" #{0}: {1}", i, this.symbols [i]);
			}

			free (symbols);
		}

		[DllImport (Constants.libcLibrary)]
		static extern int backtrace (IntPtr [] array, int size);

		[DllImport (Constants.libcLibrary)]
		static extern IntPtr backtrace_symbols (IntPtr [] array, int size);

		[DllImport (Constants.libcLibrary)]
		static extern void free (IntPtr ptr);
	}
}
#endif
