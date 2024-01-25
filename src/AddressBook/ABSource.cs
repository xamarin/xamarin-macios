// 
// ABSource.cs: Implements the managed ABSource
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright (C) 2012 Xamarin Inc.
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
//

#nullable enable

#if !MONOMAC

using System;
using System.Runtime.InteropServices;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AddressBook {

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABSource : ABRecord {
		[Preserve (Conditional = true)]
		internal ABSource (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		internal ABSource (NativeHandle handle, ABAddressBook addressbook)
			: base (handle, false)
		{
			AddressBook = addressbook;
		}

		public string? Name {
			get { return PropertyToString (ABSourcePropertyId.Name); }
			set { SetValue (ABSourcePropertyId.Name, value); }
		}

		// Type is already a property in ABRecord
		public ABSourceType SourceType {
			get { return (ABSourceType) (int) PropertyTo<NSNumber> (ABSourcePropertyId.Type); }
			set { SetValue (ABSourcePropertyId.Type, new NSNumber ((int) value)); }
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	static class ABSourcePropertyId {

		public static int Name { get; private set; }
		public static int Type { get; private set; }

		static ABSourcePropertyId ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			Name = Dlfcn.GetInt32 (handle, "kABSourceNameProperty");
			Type = Dlfcn.GetInt32 (handle, "kABSourceTypeProperty");
		}

		public static int ToId (ABSourceProperty property)
		{
			switch (property) {
			case ABSourceProperty.Name:
				return Name;
			case ABSourceProperty.Type:
				return Type;
			}
			throw new NotSupportedException ("Invalid ABSourceProperty value: " + property);
		}

		public static ABSourceProperty ToSourceProperty (int id)
		{
			if (id == Name)
				return ABSourceProperty.Name;
			if (id == Type)
				return ABSourceProperty.Type;
			throw new NotSupportedException ("Invalid ABSourcePropertyId value: " + id);
		}
	}
}

#endif // !MONOMAC
