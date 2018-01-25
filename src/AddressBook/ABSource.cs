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

#if !MONOMAC

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace AddressBook {
	
	// note: not a true flag
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	public enum ABSourceType : int /* typedef int */ {
		Local		= 0x0,
		Exchange	= 0x1,
		ExchangeGAL	= Exchange | SearchableMask,
		MobileMe	= 0x2,
		LDAP		= 0x3 | SearchableMask,
		CardDAV		= 0x4,
		DAVSearch	= CardDAV | SearchableMask,

		SearchableMask = 0x01000000
	};	
	
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	public class ABSource : ABRecord {
#if !XAMCORE_2_0
		[Advice ("Use ABSourceType.SearchableMask")]
		public const int SearchableMask = 0x01000000;
#endif

		internal ABSource (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}
		
		internal ABSource (IntPtr handle, ABAddressBook addressbook)
			: base (handle, false)
		{
			AddressBook = addressbook;
		}
		
		public string Name {
			get { return PropertyToString (ABSourcePropertyId.Name); }
			set { SetValue (ABSourcePropertyId.Name, value); }
		}
		
		// Type is already a property in ABRecord
		public ABSourceType SourceType {
			get { return (ABSourceType) (int) PropertyTo<NSNumber> (ABSourcePropertyId.Type); }
			set { SetValue (ABSourcePropertyId.Type, new NSNumber ((int) value)); }
		}
	}

	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	public enum ABSourceProperty {
		Name,
		Type,
	}
	
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	static class ABSourcePropertyId {

		public static int Name { get; private set;}
		public static int Type { get; private set;}
		
		static ABSourcePropertyId ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Dlfcn.dlopen (Constants.AddressBookLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			
			try {
				Name = Dlfcn.GetInt32 (handle, "kABSourceNameProperty");
				Type = Dlfcn.GetInt32 (handle, "kABSourceTypeProperty");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
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
