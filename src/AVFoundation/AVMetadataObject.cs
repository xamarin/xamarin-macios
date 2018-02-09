// 
// AVMetadataObject.cs:
//     
// Copyright 2014 Xamarin Inc.
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

#if XAMCORE_2_0 && IOS
using Foundation;
using ObjCRuntime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AVFoundation {

	public partial class AVMetadataObject {
		public AVMetadataObjectType Type {
			get {
				return ObjectToEnum (WeakType);
			}
		}

		internal static AVMetadataObjectType ArrayToEnum (NSString[] arr)
		{
			AVMetadataObjectType rv = AVMetadataObjectType.None;

			if (arr == null || arr.Length == 0)
				return rv;

			foreach (var str in arr) {
				rv |= ObjectToEnum (str);
			}

			return rv;
		}

		internal static NSString[] EnumToArray (AVMetadataObjectType value)
		{
			if (value == AVMetadataObjectType.None)
				return null;

			var rv = new List<NSString> ();
			var val = (ulong) value;
			var shifts = 0;

			while (val != 0) {
				if ((val & 0x1) == 0x1)
					rv.Add (EnumToObject ((AVMetadataObjectType) (0x1UL << shifts)));
				val >>= 1;
				shifts++;
			}

			return rv.ToArray ();
		}

		internal static AVMetadataObjectType ObjectToEnum (NSString obj)
		{
			if (obj == null)
				return AVMetadataObjectType.None;
			else if (obj == AVMetadataObject.TypeFace)
				return AVMetadataObjectType.Face;
			else if (obj == AVMetadataObject.TypeAztecCode)
				return AVMetadataObjectType.AztecCode;
			else if (obj == AVMetadataObject.TypeCode128Code)
				return AVMetadataObjectType.Code128Code;
			else if (obj == AVMetadataObject.TypeCode39Code)
				return AVMetadataObjectType.Code39Code;
			else if (obj == AVMetadataObject.TypeCode39Mod43Code)
				return AVMetadataObjectType.Code39Mod43Code;
			else if (obj == AVMetadataObject.TypeCode93Code)
				return AVMetadataObjectType.Code93Code;
			else if (obj == AVMetadataObject.TypeEAN13Code)
				return AVMetadataObjectType.EAN13Code;
			else if (obj == AVMetadataObject.TypeEAN8Code)
				return AVMetadataObjectType.EAN8Code;
			else if (obj == AVMetadataObject.TypePDF417Code)
				return AVMetadataObjectType.PDF417Code;
			else if (obj == AVMetadataObject.TypeQRCode)
				return AVMetadataObjectType.QRCode;
			else if (obj == AVMetadataObject.TypeUPCECode)
				return AVMetadataObjectType.UPCECode;
			else if (obj == AVMetadataObject.TypeInterleaved2of5Code)
				return AVMetadataObjectType.Interleaved2of5Code;
			else if (obj == AVMetadataObject.TypeITF14Code)
				return AVMetadataObjectType.ITF14Code;
			else if (obj == AVMetadataObject.TypeDataMatrixCode)
				return AVMetadataObjectType.DataMatrixCode;
			else
				throw new ArgumentOutOfRangeException (string.Format ("Unexpected AVMetadataObjectType: {0}", obj));
		}

		internal static NSString EnumToObject (AVMetadataObjectType val)
		{
			switch (val) {
			case AVMetadataObjectType.None:
				return null;
			case AVMetadataObjectType.Face:
				return AVMetadataObject.TypeFace;
			case AVMetadataObjectType.AztecCode:
				return AVMetadataObject.TypeAztecCode;
			case AVMetadataObjectType.Code128Code:
				return AVMetadataObject.TypeCode128Code;
			case AVMetadataObjectType.Code39Code:
				return AVMetadataObject.TypeCode39Code;
			case AVMetadataObjectType.Code39Mod43Code:
				return AVMetadataObject.TypeCode39Mod43Code;
			case AVMetadataObjectType.Code93Code:
				return AVMetadataObject.TypeCode93Code;
			case AVMetadataObjectType.EAN13Code:
				return AVMetadataObject.TypeEAN13Code;
			case AVMetadataObjectType.EAN8Code:
				return AVMetadataObject.TypeEAN8Code;
			case AVMetadataObjectType.PDF417Code:
				return AVMetadataObject.TypePDF417Code;
			case AVMetadataObjectType.QRCode:
				return AVMetadataObject.TypeQRCode;
			case AVMetadataObjectType.UPCECode:
				return AVMetadataObject.TypeUPCECode;
			case AVMetadataObjectType.Interleaved2of5Code:
				return AVMetadataObject.TypeInterleaved2of5Code;
			case AVMetadataObjectType.ITF14Code:
				return AVMetadataObject.TypeITF14Code;
			case AVMetadataObjectType.DataMatrixCode:
				return AVMetadataObject.TypeDataMatrixCode;
			default:
				throw new ArgumentOutOfRangeException (string.Format ("Unexpected AVMetadataObjectType: {0}", val));
			}
		}
	}
}
#endif
