//
// NSFileManager.cs:
// Author:
//   Miguel de Icaza
//
// Copyright 2011, Novell, Inc.
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
//
using ObjCRuntime;
using System;

namespace Foundation {

	// This is a convenience enum around a set of native strings.
	public enum NSFileType {
		Directory, Regular, SymbolicLink, Socket, CharacterSpecial, BlockSpecial, Unknown
	}

#if !MONOMAC
	public enum NSFileProtection {
		None,
		Complete,
		CompleteUnlessOpen,
		CompleteUntilFirstUserAuthentication,
	}
#endif

	public class NSFileAttributes {
		public bool? AppendOnly { get; set; }
		public bool? Busy { get; set; }
#if XAMCORE_2_0
		public bool? ExtensionHidden { get ; set; }
#else
		[Obsolete ("Use 'ExtensionHidden' instead.")]
		public bool? FileExtensionHidden { get; set; }
		public bool? ExtensionHidden { get { return FileExtensionHidden; } set { FileExtensionHidden = value; } }
#endif
		public NSDate CreationDate { get; set; }
		public string OwnerAccountName { get; set; }
		public string GroupOwnerAccountName { get; set; }
		public nint? SystemNumber { get; set; } // NSInteger
		public nuint? DeviceIdentifier { get; set; } // unsigned long
#if XAMCORE_2_0
		public nuint? GroupOwnerAccountID { get ; set; } // unsigned long
#else
		[Obsolete ("Use 'GroupOwnerAccountID' instead.")]
		public nuint? FileGroupOwnerAccountID { get; set; } // unsigned long
		public nuint? GroupOwnerAccountID { get { return FileGroupOwnerAccountID; } set { FileGroupOwnerAccountID = value; } }
#endif

		public bool? Immutable { get; set; }
		public NSDate ModificationDate { get; set; }
#if XAMCORE_2_0
		public nuint? OwnerAccountID { get; set; } // unsigned long
#else
		[Obsolete ("Use 'GroupOwnerAccountID' instead.")]
		public nuint? FileOwnerAccountID { get; set; } // unsigned long
		public nuint? OwnerAccountID { get { return FileOwnerAccountID; } set { FileOwnerAccountID = value; } }
#endif
		public nuint? HfsCreatorCode { get; set; }
		public nuint? HfsTypeCode { get; set; } // unsigned long

		// This is supposed to be a 'short' value, but compat assemblies
		// defined it as uint. Keeping it that way for compat to not break compat.
#if XAMCORE_2_0
		public short? PosixPermissions { get; set; }
#else
		public uint? PosixPermissions { get; set; }
#endif
#if XAMCORE_2_0
		public nuint? ReferenceCount { get; set; } // unsigned long
		public nuint? SystemFileNumber { get; set; } // unsigned long
		public ulong? Size { get; set; } // unsigned long long
		public NSFileType? Type { get; set; }
#else
		[Obsolete ("Use 'ReferenceCount' instead.")]
		public nuint? FileReferenceCount { get; set; } // unsigned long
		public nuint? ReferenceCount { get { return FileReferenceCount; } set { FileReferenceCount = value; } }

		[Obsolete ("Use 'SystemFileNumber' instead.")]
		public nuint? FileSystemFileNumber { get; set; } // unsigned long
		public nuint? SystemFileNumber { get { return FileSystemFileNumber; } set { FileSystemFileNumber = value; } } 

		[Obsolete ("Use 'Size' instead.")]
		public ulong? FileSize { get; set; } // unsigned long long
		public ulong? Size { get { return FileSize; } set { FileSize = value; } }

		[Obsolete ("Use 'Type' instead.")]
		public NSFileType? FileType { get; set; }
		public NSFileType? Type { get { return FileType; } set { FileType = value; } }
#endif
				
#if !MONOMAC
		public NSFileProtection? ProtectionKey { get; set; }
#endif

		internal NSDictionary ToDictionary ()
		{
			NSFileType? type;
			NSString v = null;
			var dict = new NSMutableDictionary ();
			if (AppendOnly.HasValue)
				dict.SetObject (NSNumber.FromBoolean (AppendOnly.Value), NSFileManager.AppendOnly);
			if (Busy.HasValue)
				dict.SetObject (NSNumber.FromBoolean (Busy.Value), NSFileManager.Busy);
#if XAMCORE_2_0
			if (ExtensionHidden.HasValue)
				dict.SetObject (NSNumber.FromBoolean (ExtensionHidden.Value), NSFileManager.ExtensionHidden);
#else
			if (FileExtensionHidden.HasValue)
				dict.SetObject (NSNumber.FromBoolean (FileExtensionHidden.Value), NSFileManager.ExtensionHidden);
#endif
			if (CreationDate != null)
				dict.SetObject (CreationDate, NSFileManager.CreationDate);
			if (OwnerAccountName != null)
				dict.SetObject (new NSString (OwnerAccountName), NSFileManager.OwnerAccountName);
			if (GroupOwnerAccountName != null)
				dict.SetObject (new NSString (GroupOwnerAccountName), NSFileManager.GroupOwnerAccountName);
			if (SystemNumber.HasValue)
				dict.SetObject (NSNumber.FromLong (SystemNumber.Value), NSFileManager.SystemNumber);
			if (DeviceIdentifier.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (DeviceIdentifier.Value), NSFileManager.DeviceIdentifier);
#if XAMCORE_2_0
			if (GroupOwnerAccountID.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (GroupOwnerAccountID.Value), NSFileManager.GroupOwnerAccountID);
#else
			if (FileGroupOwnerAccountID.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (FileGroupOwnerAccountID.Value), NSFileManager.GroupOwnerAccountID);
#endif
			if (Immutable.HasValue)
				dict.SetObject (NSNumber.FromBoolean (Immutable.Value), NSFileManager.Immutable);
			if (ModificationDate != null)
				dict.SetObject (ModificationDate, NSFileManager.ModificationDate);
#if XAMCORE_2_0
			if (OwnerAccountID.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (OwnerAccountID.Value), NSFileManager.OwnerAccountID);
#else
			if (FileOwnerAccountID.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (FileOwnerAccountID.Value), NSFileManager.OwnerAccountID);
#endif
			if (HfsCreatorCode.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (HfsCreatorCode.Value), NSFileManager.HfsCreatorCode);
			if (HfsTypeCode.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (HfsTypeCode.Value), NSFileManager.HfsTypeCode);
			if (PosixPermissions.HasValue)
				dict.SetObject (NSNumber.FromInt16 ((short) PosixPermissions.Value), NSFileManager.PosixPermissions);
#if XAMCORE_2_0
			if (ReferenceCount.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (ReferenceCount.Value), NSFileManager.ReferenceCount);
			if (SystemFileNumber.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (SystemFileNumber.Value), NSFileManager.SystemFileNumber);
			if (Size.HasValue)
				dict.SetObject (NSNumber.FromUInt64 (Size.Value), NSFileManager.Size);

			type = Type;
#else
			if (FileReferenceCount.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (FileReferenceCount.Value), NSFileManager.ReferenceCount);
			if (FileSystemFileNumber.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (FileSystemFileNumber.Value), NSFileManager.SystemFileNumber);
			if (FileSize.HasValue)
				dict.SetObject (NSNumber.FromUInt64 (FileSize.Value), NSFileManager.Size);

			type = FileType;
#endif

			if (type.HasValue) {
				v = null;
				switch (type.Value){
				case NSFileType.Directory:
					v = NSFileManager.TypeDirectory; break;
				case NSFileType.Regular:
					v = NSFileManager.TypeRegular; break;
				case NSFileType.SymbolicLink:
					v = NSFileManager.TypeSymbolicLink; break;
				case NSFileType.Socket:
					v = NSFileManager.TypeSocket; break;
				case NSFileType.CharacterSpecial:
					v = NSFileManager.TypeCharacterSpecial; break;
				case NSFileType.BlockSpecial:
					v = NSFileManager.TypeBlockSpecial; break;
				default:
					v = NSFileManager.TypeUnknown; break;
				}
				dict.SetObject (v, NSFileManager.NSFileType);
			}

#if !MONOMAC
			if (ProtectionKey.HasValue) {
				v = null;
				switch (ProtectionKey.Value) {
				case NSFileProtection.None:
					v = NSFileManager.FileProtectionNone; break;
				case NSFileProtection.Complete:
					v = NSFileManager.FileProtectionComplete; break;
				case NSFileProtection.CompleteUnlessOpen:
					v = NSFileManager.FileProtectionCompleteUnlessOpen; break;
				case NSFileProtection.CompleteUntilFirstUserAuthentication:
					v = NSFileManager.FileProtectionCompleteUntilFirstUserAuthentication; break;
				}
				dict.SetObject (v, NSFileManager.FileProtectionKey);
			}
#endif
			return dict;
		}

#region fetch
		internal static bool? fetch_bool (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k == null)
				return null;
			return k.BoolValue;
		}
			
		internal static uint? fetch_uint (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k == null)
				return null;
			return k.UInt32Value;
		}

		internal static nuint? fetch_nuint (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k == null)
				return null;
			return k.UnsignedLongValue;
		}

		internal static nint? fetch_nint (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k == null)
				return null;
			return k.LongValue;
		}

		internal static ulong? fetch_ulong (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k == null)
				return null;
			return k.UInt64Value;
		}

		internal static long? fetch_long (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k == null)
				return null;
			return k.Int64Value;
		}

		internal static short? fetch_short (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k == null)
				return null;
			return k.Int16Value;
		}
#endregion

#if !XAMCORE_2_0
		[Obsolete ("Use FromDictionary instead.")]
		public static NSFileAttributes FromDict (NSDictionary dict)
		{
			return FromDictionary (dict);
		}
#endif

		public static NSFileAttributes FromDictionary (NSDictionary dict)
		{
			if (dict == null)
				return null;
			var ret = new NSFileAttributes ();

			ret.AppendOnly = fetch_bool (dict, NSFileManager.AppendOnly);
			ret.Busy = fetch_bool (dict, NSFileManager.Busy);
#if XAMCORE_2_0
			ret.ExtensionHidden = fetch_bool (dict, NSFileManager.ExtensionHidden);
#else
			ret.FileExtensionHidden = fetch_bool (dict, NSFileManager.ExtensionHidden);
#endif
			ret.CreationDate = dict.ObjectForKey (NSFileManager.CreationDate) as NSDate;
			ret.OwnerAccountName = dict.ObjectForKey (NSFileManager.OwnerAccountName) as NSString;
			ret.GroupOwnerAccountName = dict.ObjectForKey (NSFileManager.GroupOwnerAccountName) as NSString;
			ret.SystemNumber = fetch_nint (dict, NSFileManager.SystemNumber);
			ret.DeviceIdentifier = fetch_nuint (dict, NSFileManager.DeviceIdentifier);
#if XAMCORE_2_0
			ret.GroupOwnerAccountID = fetch_nuint (dict, NSFileManager.GroupOwnerAccountID);
#else
			ret.FileGroupOwnerAccountID = fetch_nuint (dict, NSFileManager.GroupOwnerAccountID);
#endif
			ret.Immutable = fetch_bool (dict, NSFileManager.Immutable);
			ret.ModificationDate = dict.ObjectForKey (NSFileManager.ModificationDate) as NSDate;
#if XAMCORE_2_0
			ret.OwnerAccountID = fetch_nuint (dict, NSFileManager.OwnerAccountID);
#else
			ret.FileOwnerAccountID = fetch_nuint (dict, NSFileManager.OwnerAccountID);
#endif
			ret.HfsCreatorCode = fetch_nuint (dict, NSFileManager.HfsCreatorCode);
			ret.HfsTypeCode = fetch_nuint (dict, NSFileManager.HfsTypeCode);
#if XAMCORE_2_0
			ret.PosixPermissions = fetch_short (dict, NSFileManager.PosixPermissions);
#else
			ret.PosixPermissions = (uint?) fetch_short (dict, NSFileManager.PosixPermissions);
#endif
#if XAMCORE_2_0
			ret.ReferenceCount = fetch_nuint (dict, NSFileManager.ReferenceCount);
			ret.SystemFileNumber = fetch_nuint (dict, NSFileManager.SystemFileNumber);
			ret.Size = fetch_ulong (dict, NSFileManager.Size);
#else
			ret.FileReferenceCount = fetch_nuint (dict, NSFileManager.ReferenceCount);
			ret.FileSystemFileNumber = fetch_nuint (dict, NSFileManager.SystemFileNumber);
			ret.FileSize = fetch_ulong (dict, NSFileManager.Size);
#endif

			NSString name;

			name = dict.ObjectForKey (NSFileManager.NSFileType) as NSString;
			if (name != null) {
				NSFileType? type = null;

				if (name == NSFileManager.TypeDirectory)
					type = NSFileType.Directory;
				else if (name == NSFileManager.TypeRegular)
					type = NSFileType.Regular;
				else if (name == NSFileManager.TypeSymbolicLink)
					type = NSFileType.SymbolicLink;
				else if (name == NSFileManager.TypeSocket)
					type = NSFileType.Socket;
				else if (name == NSFileManager.TypeCharacterSpecial)
					type = NSFileType.CharacterSpecial;
				else if (name == NSFileManager.TypeBlockSpecial)
					type = NSFileType.BlockSpecial;
				else if (name == NSFileManager.TypeUnknown)
					type = NSFileType.Unknown;
					
#if XAMCORE_2_0
				ret.Type = type;
#else
				ret.FileType = type;
#endif
			}
				
#if !MONOMAC
			name = dict.ObjectForKey (NSFileManager.FileProtectionKey) as NSString;
			if (name != null) {
				NSFileProtection? protection = null;

				if (name == NSFileManager.FileProtectionNone)
					protection = NSFileProtection.None;
				else if (name == NSFileManager.FileProtectionComplete)
					protection = NSFileProtection.Complete;
				else if (name == NSFileManager.FileProtectionCompleteUnlessOpen)
					protection = NSFileProtection.CompleteUnlessOpen;
				else if (name == NSFileManager.FileProtectionCompleteUntilFirstUserAuthentication)
					protection = NSFileProtection.CompleteUntilFirstUserAuthentication;

				ret.ProtectionKey = protection;
			}
#endif
			return ret;
		}
	}

	public class NSFileSystemAttributes {
		NSDictionary dict;
		
		internal NSFileSystemAttributes (NSDictionary dict)
		{
			this.dict = dict;
		}

		// The documentation only says these are NSNumbers, it doesn't say which type of number.
		public ulong Size { get; internal set; }
		public ulong FreeSize { get; internal set; }
		public long Nodes { get; internal set; }
		public long FreeNodes { get; internal set; }
		// "The value corresponds to the value of st_dev, as returned by stat(2)" => st_dev is defined to be int32_t in all architectures.
		public uint Number { get; internal set; }

		internal static NSFileSystemAttributes FromDictionary (NSDictionary dict)
		{
			if (dict == null)
				return null;
			var ret = new NSFileSystemAttributes (dict);
			ulong l = 0;
			uint i = 0;
			ret.Size      = NSFileAttributes.fetch_ulong (dict, NSFileManager.SystemSize) ?? 0;
			ret.FreeSize  = NSFileAttributes.fetch_ulong (dict, NSFileManager.SystemFreeSize) ?? 0;
			ret.Nodes     = NSFileAttributes.fetch_long (dict, NSFileManager.SystemNodes) ?? 0;
			ret.FreeNodes = NSFileAttributes.fetch_long (dict, NSFileManager.SystemFreeNodes) ?? 0;
			ret.Number    = NSFileAttributes.fetch_uint (dict, NSFileManager.SystemFreeNodes) ?? 0;

			return ret;
		}

		// For source code compatibility with users that had done manual NSDictionary lookups before
		public static implicit operator NSDictionary (NSFileSystemAttributes attr)
		{
			return attr.dict;
		}
		
	}		
	
	public partial class NSFileManager {
		public bool SetAttributes (NSFileAttributes attributes, string path, out NSError error)
		{
			if (attributes == null)
				throw new ArgumentNullException ("attributes");
			return SetAttributes (attributes.ToDictionary (), path, out error);
		}

		public bool SetAttributes (NSFileAttributes attributes, string path)
		{
			NSError ignore;
			if (attributes == null)
				throw new ArgumentNullException ("attributes");

			return SetAttributes (attributes.ToDictionary (), path, out ignore);
		}

		public bool CreateDirectory (string path, bool createIntermediates, NSFileAttributes attributes, out NSError error)
		{
			var dict = attributes == null ? null : attributes.ToDictionary ();
			return CreateDirectory (path, createIntermediates, dict, out error);
		}

		public bool CreateDirectory (string path, bool createIntermediates, NSFileAttributes attributes)
		{
			NSError error;
			var dict = attributes == null ? null : attributes.ToDictionary ();
			return CreateDirectory (path, createIntermediates, dict, out error);
		}

		public bool CreateFile (string path, NSData data, NSFileAttributes attributes)
		{
			var dict = attributes == null ? null : attributes.ToDictionary ();
			return CreateFile (path, data, dict);
		}
		
		public NSFileAttributes GetAttributes (string path, out NSError error)
		{
			return NSFileAttributes.FromDictionary (_GetAttributes (path, out error));
		}

		public NSFileAttributes GetAttributes (string path)
		{
			NSError error;
			return NSFileAttributes.FromDictionary (_GetAttributes (path, out error));
		}

		public NSFileSystemAttributes GetFileSystemAttributes (string path)
		{
			NSError error;
			return NSFileSystemAttributes.FromDictionary (_GetFileSystemAttributes (path, out error));
		}

		public NSFileSystemAttributes GetFileSystemAttributes (string path, out NSError error)
		{
			return NSFileSystemAttributes.FromDictionary (_GetFileSystemAttributes (path, out error));
		}

		public NSUrl[] GetMountedVolumes (NSString [] properties, NSVolumeEnumerationOptions options)
		{
			return GetMountedVolumes (NSArray.FromNSObjects (properties), options);
		}

		public string CurrentDirectory {
			get { return GetCurrentDirectory (); }
			// ignore boolean return value
			set { ChangeCurrentDirectory (value); }
		}
	}
}