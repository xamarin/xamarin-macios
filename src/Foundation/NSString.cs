//
// Copyright 2010, Novell, Inc.
// Copyright 2011 - 2014 Xamarin Inc
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
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

#if !COREBUILD
using CoreFoundation;
using CoreGraphics;
#endif
using ObjCRuntime;

namespace Foundation {
	[Native]
	public enum NSStringEncoding : ulong {
		ASCIIStringEncoding = 1,
		NEXTSTEP = 2,
		JapaneseEUC = 3,
		UTF8 = 4,
		ISOLatin1 = 5,
		Symbol = 6,
		NonLossyASCII = 7,
		ShiftJIS = 8,
		ISOLatin2 = 9,
		Unicode = 10,
		WindowsCP1251 = 11,
		WindowsCP1252 = 12,
		WindowsCP1253 = 13,
		WindowsCP1254 = 14,
		WindowsCP1250 = 15,
		ISO2022JP = 21,
		MacOSRoman = 30,
		UTF16BigEndian = 0x90000100,
		UTF16LittleEndian = 0x94000100,
		UTF32 = 0x8c000100,
		UTF32BigEndian = 0x98000100,
		UTF32LittleEndian = 0x9c000100,
	};
	
	[Native]
	public enum NSStringCompareOptions : ulong {
		CaseInsensitiveSearch = 1,
		LiteralSearch = 2,
		BackwardsSearch = 4,
		AnchoredSearch = 8,
		NumericSearch = 64,
		DiacriticInsensitiveSearch = 128,
		WidthInsensitiveSearch = 256,
		ForcedOrderingSearch = 512,
		RegularExpressionSearch = 1024
	}

	public partial class NSString : NSObject
#if COREBUILD
		, INSCopying, INSSecureCoding
#endif
	 {
#if !COREBUILD
		const string selUTF8String = "UTF8String";
		const string selInitWithCharactersLength = "initWithCharacters:length:";

#if MONOMAC
		static IntPtr selUTF8StringHandle = Selector.GetHandle (selUTF8String);
		static IntPtr selInitWithCharactersLengthHandle = Selector.GetHandle (selInitWithCharactersLength);
#endif

		public static readonly NSString Empty = new NSString (String.Empty);

		internal NSString (IntPtr handle, bool alloced) : base (handle, alloced)
		{
		}

		static IntPtr CreateWithCharacters (IntPtr handle, string str, bool autorelease = false)
		{
			unsafe {
				fixed (char *ptrFirstChar = str) {
	#if MONOMAC
					handle = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (handle, selInitWithCharactersLengthHandle, (IntPtr) ptrFirstChar, (IntPtr) str.Length);
	#else
					handle = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (handle, Selector.GetHandle (selInitWithCharactersLength), (IntPtr) ptrFirstChar, (IntPtr) str.Length);
	#endif

					if (autorelease)
						NSObject.DangerousAutorelease (handle);

					return handle;
				}
			}
		}

		public static IntPtr CreateNative (string str)
		{
			return CreateNative (str, false);
		}

		public static IntPtr CreateNative (string str, bool autorelease)
		{
			if (str == null)
				return IntPtr.Zero;

#if MONOMAC
			var handle = Messaging.IntPtr_objc_msgSend (class_ptr, Selector.AllocHandle);
#else
			var handle = Messaging.IntPtr_objc_msgSend (class_ptr, Selector.GetHandle (Selector.Alloc));
#endif

			return CreateWithCharacters (handle, str, autorelease);
		}

		public static void ReleaseNative (IntPtr handle)
		{
			NSObject.DangerousRelease (handle);
		}
	
		public NSString (string str) {
			if (str == null)
				throw new ArgumentNullException ("str");

			Handle = CreateWithCharacters (Handle, str);
		}
	
		public unsafe override string ToString ()
		{
			return FromHandle (Handle);
		}

		public static implicit operator string (NSString str)
		{
			if (((object) str) == null)
				return null;
			return str.ToString ();
		}

		public static explicit operator NSString (string str)
		{
			if (str == null)
				return null;
			return new NSString (str);
		}

		public unsafe static string FromHandle (IntPtr usrhandle)
		{
			if (usrhandle == IntPtr.Zero)
				return null;

#if MONOMAC
			return Marshal.PtrToStringAuto (Messaging.IntPtr_objc_msgSend (usrhandle, selUTF8StringHandle));
#else
			return Marshal.PtrToStringAuto (Messaging.IntPtr_objc_msgSend (usrhandle, Selector.GetHandle (selUTF8String)));
#endif
		}

		public static bool Equals (NSString a, NSString b)
		{
			if ((a as object) == (b as object))
				return true;

			if (((object) a) == null || ((object) b) == null)
				return false;

			if (a.Handle == b.Handle)
				return true;
			return a.IsEqualTo (b.Handle);
		}

		public static bool operator == (NSString a, NSString b)
		{
			return Equals (a, b);
		}
		
		public static bool operator != (NSString a, NSString b)
		{
			return !Equals (a, b);
		}

		public override bool Equals (Object obj)
		{
			return Equals (this, obj as NSString);
		}

		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format (IntPtr fmt);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_1 (IntPtr fmt, IntPtr arg1);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_2 (IntPtr fmt, IntPtr arg1, IntPtr arg2);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_3 (IntPtr fmt, IntPtr arg1, IntPtr arg2, IntPtr arg3);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_4 (IntPtr fmt, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_5 (IntPtr fmt, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_6 (IntPtr fmt, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_7 (IntPtr fmt, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_8 (IntPtr fmt, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8);
		[DllImport ("__Internal")]
		extern static IntPtr xamarin_localized_string_format_9 (IntPtr fmt, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9);
		
		public static NSString LocalizedFormat (string format, params object [] args)
		{
			using (var ns = new NSString (format))
				return LocalizedFormat (ns, args);
		}
		
		public static NSString LocalizedFormat (NSString format, params object [] args)
		{
			int argc = args.Length;
			var nso = new NSObject [argc];
			for (int i = 0; i < argc; i++)
				nso [i] = NSObject.FromObject (args [i]);
				
			return LocalizedFormat (format, nso);
		}
		       
		public static NSString LocalizedFormat (NSString format, NSObject [] args)
		{
			switch (args.Length){
			case 0:
				return new NSString (xamarin_localized_string_format (format.Handle));
			case 1:
				return new NSString (xamarin_localized_string_format_1 (format.Handle, args[0].Handle));
			case 2:
				return new NSString (xamarin_localized_string_format_2 (format.Handle, args[0].Handle, args[1].Handle));
			case 3:
				return new NSString (xamarin_localized_string_format_3 (format.Handle, args[0].Handle, args[1].Handle, args[2].Handle));
			case 4:
				return new NSString (xamarin_localized_string_format_4 (format.Handle, args[0].Handle, args[1].Handle, args[2].Handle, args[3].Handle));
			case 5:
				return new NSString (xamarin_localized_string_format_5 (format.Handle, args[0].Handle, args[1].Handle, args[2].Handle, args[3].Handle, args[4].Handle));
			case 6:
				return new NSString (xamarin_localized_string_format_6 (format.Handle, args[0].Handle, args[1].Handle, args[2].Handle, args[3].Handle, args[4].Handle, args[5].Handle));
			case 7:
				return new NSString (xamarin_localized_string_format_7 (format.Handle, args[0].Handle, args[1].Handle, args[2].Handle, args[3].Handle, args[4].Handle, args[5].Handle, args[6].Handle));
			case 8:
				return new NSString (xamarin_localized_string_format_8 (format.Handle, args[0].Handle, args[1].Handle, args[2].Handle, args[3].Handle, args[4].Handle, args[5].Handle, args[6].Handle, args[7].Handle));
			case 9:
				return new NSString (xamarin_localized_string_format_9 (format.Handle, args[0].Handle, args[1].Handle, args[2].Handle, args[3].Handle, args[4].Handle, args[5].Handle, args[6].Handle, args[7].Handle, args[8].Handle));
			default:
				throw new Exception ("Unsupported number of arguments, maximum number is 9");
			}
		}

		public NSString TransliterateString (NSStringTransform transform, bool reverse)
		{
			return TransliterateString (transform.GetConstant (), reverse);
		}
		
		public override int GetHashCode ()
		{
#if XAMCORE_2_0
			// for unified NSObject already deals with `hash` for both 32/64 bits
			return base.GetHashCode ();
#else
			// for classic there's no GetHashCode override in NSObject, so Hash makes sense
			return (int) GetNativeHash ();
#endif
		}

#if !MONOMAC && !XAMCORE_2_0
		[Advice ("Use the version with a `ref float actualFontSize`")]
		public CGSize DrawString (CGPoint point, nfloat width, UIKit.UIFont font, nfloat minFontSize, nfloat actualFontSize, UIKit.UILineBreakMode breakMode, UIKit.UIBaselineAdjustment adjustment)
		{
			nfloat temp = actualFontSize;
			return DrawString (point, width, font, minFontSize, ref temp, breakMode, adjustment);
		}
#endif
#endif // !COREBUILD
	}
}
