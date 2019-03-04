#if !COREBUILD

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace CoreFoundation {
	
	public class CFMutableString : CFString {

		[DllImport (Constants.CoreFoundationLibrary, CharSet=CharSet.Unicode)]
		static extern unsafe /* CFMutableStringRef* */ IntPtr CFStringCreateMutableWithExternalCharactersNoCopy (/* CFAllocatorRef* */ IntPtr alloc, string chars, nint numChars, nint capacity, /* CFAllocatorRef* */ IntPtr externalCharactersAllocator);

		public CFMutableString (string theString, nint capacity)
		{
			if (theString == null)
				throw new ArgumentNullException (nameof (theString));
			
			handle = CFStringCreateMutableWithExternalCharactersNoCopy (IntPtr.Zero, theString, theString.Length, capacity, IntPtr.Zero);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern unsafe /* CFMutableStringRef* */ IntPtr CFStringCreateMutable (/* CFAllocatorRef* */ IntPtr alloc, nint maxLength);

		public CFMutableString (nint maxLength)
		{
			handle = CFStringCreateMutable (IntPtr.Zero, maxLength);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern unsafe /* CFMutableStringRef* */ IntPtr CFStringCreateMutableCopy (/* CFAllocatorRef* */ IntPtr alloc, nint maxLength, /* CFStringRef* */ IntPtr theString);

		public CFMutableString (CFString theString, nint maxLength)
		{
			handle = CFStringCreateMutableCopy (IntPtr.Zero, maxLength, theString.GetHandle ());
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern internal bool /* Boolean */ CFStringTransform (/* CFMutableStringRef* */ IntPtr @string, /* CFRange* */ ref CFRange range, /* CFStringRef* */ IntPtr transform, [MarshalAs (UnmanagedType.I1)] /* Boolean */ bool reverse);

		public bool Transform (ref CFRange range, CFStringTransform transform, bool reverse)
		{
			str = null; // destroy any cached value
			return Transform (Handle, ref range, transform.GetConstant ().GetHandle (), reverse);
		}

		// constant documentation mention it also accept any ICT transform
		public bool Transform (ref CFRange range, CFString transform, bool reverse)
		{
			str = null; // destroy any cached value
			return CFStringTransform (Handle, ref range, transform.GetHandle (), reverse);
		}

		internal bool Transform (IntPtr @string, ref CFRange range, IntPtr transform, bool reverse)
		{
			return CFStringTransform (@string, ref range, transform, reverse);
		}
	}
}

#endif // !COREBUILD
