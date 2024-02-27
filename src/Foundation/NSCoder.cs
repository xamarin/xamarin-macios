//
// NSCoder support
//
// Author:
//   Miguel de Icaza
//
// Copyright 2010, Novell, Inc.
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

	public partial class NSCoder {
		public void Encode (byte [] buffer, string key)
		{
			if (buffer is null)
				throw new ArgumentNullException ("buffer");

			if (key is null)
				throw new ArgumentNullException ("key");

			unsafe {
				fixed (byte* p = buffer) {
					EncodeBlock ((IntPtr) p, buffer.Length, key);
				}
			}
		}

		public void Encode (byte [] buffer, int offset, int count, string key)
		{
			if (buffer is null)
				throw new ArgumentNullException ("buffer");

			if (key is null)
				throw new ArgumentNullException ("key");

			if (offset < 0)
				throw new ArgumentException ("offset < 0");
			if (count < 0)
				throw new ArgumentException ("count < 0");

			if (offset > buffer.Length - count)
				throw new ArgumentException ("Reading would overrun buffer");

			unsafe {
				fixed (byte* p = buffer) {
					EncodeBlock ((IntPtr) p, buffer.Length, key);
				}
			}
		}

		public byte [] DecodeBytes (string key)
		{
			nuint len = 0;
			IntPtr ret = DecodeBytes (key, out len);
			if (ret == IntPtr.Zero)
				return null;

			byte [] retarray = new byte [(int) len];
			Marshal.Copy (ret, retarray, 0, (int) len);

			return retarray;
		}

		public byte [] DecodeBytes ()
		{
			nuint len = 0;
			IntPtr ret = DecodeBytes (out len);
			if (ret == IntPtr.Zero)
				return null;

			byte [] retarray = new byte [(int) len];
			Marshal.Copy (ret, retarray, 0, (int) len);

			return retarray;
		}

		public bool TryDecode (string key, out bool result)
		{
			if (ContainsKey (key)) {
				result = DecodeBool (key);
				return true;
			}
			result = false;
			return false;
		}

		public bool TryDecode (string key, out double result)
		{
			if (ContainsKey (key)) {
				result = DecodeDouble (key);
				return true;
			}
			result = 0;
			return false;
		}

		public bool TryDecode (string key, out float result)
		{
			if (ContainsKey (key)) {
				result = DecodeFloat (key);
				return true;
			}
			result = 0;
			return false;
		}

		public bool TryDecode (string key, out int result)
		{
			if (ContainsKey (key)) {
				result = DecodeInt (key);
				return true;
			}
			result = 0;
			return false;
		}

		public bool TryDecode (string key, out long result)
		{
			if (ContainsKey (key)) {
				result = DecodeLong (key);
				return true;
			}
			result = 0;
			return false;
		}

		public bool TryDecode (string key, out nint result)
		{
			if (ContainsKey (key)) {
				result = DecodeNInt (key);
				return true;
			}
			result = 0;
			return false;
		}

		public bool TryDecode (string key, out NSObject result)
		{
			if (ContainsKey (key)) {
				result = DecodeObject (key);
				return true;
			}
			result = null;
			return false;
		}

		public bool TryDecode (string key, out byte [] result)
		{
			if (ContainsKey (key)) {
				result = DecodeBytes (key);
				return true;
			}
			result = null;
			return false;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSObject DecodeTopLevelObject (Type type, string key, out NSError error)
		{
			if (type is null)
				throw new ArgumentNullException ("type");
			return DecodeTopLevelObject (new Class (type), key, out error);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSObject DecodeTopLevelObject (Type [] types, string key, out NSError error)
		{
			NSSet<Class> typeSet = null;
			if (types is not null) {
				var classes = new Class [types.Length];
				for (int i = 0; i < types.Length; i++)
					classes [i] = new Class (types [i]);
				typeSet = new NSSet<Class> (classes);
			}
			return DecodeTopLevelObject (typeSet, key, out error);
		}
	}
}
