#if !MONOMAC
using System;
using System.Runtime.InteropServices;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class Symbols {
		string [] symbols;

		[Test]
		public void FunctionNames ()
		{
			if (Runtime.Arch != Arch.DEVICE)
				Assert.Ignore ("This is a device-only test.");
			
			Collect ();
			bool aot = symbols [1].Contains ("MonoTouchFixtures_Symbols_Collect");
			bool interp = false;

			if (!aot) {
				for (int i = 0; i < 4 && !interp; i++) {
					/* ves_pinvoke_method (slow path) and do_icall (fast path) are
					 * MONO_NEVER_INLINE, so they should show up in the backtrace
					 * reliably */
					interp |= symbols [i].Contains ("ves_pinvoke_method") || symbols [i].Contains ("do_icall");
				}
			}

			Assert.IsTrue (aot || interp, "#1");
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
		static extern int backtrace (IntPtr[] array, int size);

		[DllImport (Constants.libcLibrary)]
		static extern IntPtr backtrace_symbols (IntPtr[] array, int size);

		[DllImport (Constants.libcLibrary)]
		static extern void free (IntPtr ptr);
	}
}
#endif