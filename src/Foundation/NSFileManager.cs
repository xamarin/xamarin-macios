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
using CoreFoundation;

using ObjCRuntime;

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#nullable enable

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

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class NSFileAttributes {
		public bool? AppendOnly { get; set; }
		public bool? Busy { get; set; }
		public bool? ExtensionHidden { get; set; }
		public NSDate? CreationDate { get; set; }
		public string? OwnerAccountName { get; set; }
		public string? GroupOwnerAccountName { get; set; }
		public nint? SystemNumber { get; set; } // NSInteger
		public nuint? DeviceIdentifier { get; set; } // unsigned long
		public nuint? GroupOwnerAccountID { get; set; } // unsigned long

		public bool? Immutable { get; set; }
		public NSDate? ModificationDate { get; set; }
		public nuint? OwnerAccountID { get; set; } // unsigned long
		public nuint? HfsCreatorCode { get; set; }
		public nuint? HfsTypeCode { get; set; } // unsigned long

		public short? PosixPermissions { get; set; }
		public nuint? ReferenceCount { get; set; } // unsigned long
		public nuint? SystemFileNumber { get; set; } // unsigned long
		public ulong? Size { get; set; } // unsigned long long
		public NSFileType? Type { get; set; }

#if !MONOMAC
		public NSFileProtection? ProtectionKey { get; set; }
#endif

		internal NSDictionary ToDictionary ()
		{
			NSFileType? type;
			NSString? v = null;
			var dict = new NSMutableDictionary ();
			if (AppendOnly.HasValue)
				dict.SetObject (NSNumber.FromBoolean (AppendOnly.Value), NSFileManager.AppendOnly);
			if (Busy.HasValue)
				dict.SetObject (NSNumber.FromBoolean (Busy.Value), NSFileManager.Busy);
			if (ExtensionHidden.HasValue)
				dict.SetObject (NSNumber.FromBoolean (ExtensionHidden.Value), NSFileManager.ExtensionHidden);
			if (CreationDate is not null)
				dict.SetObject (CreationDate, NSFileManager.CreationDate);
			if (OwnerAccountName is not null)
				dict.SetObject (new NSString (OwnerAccountName), NSFileManager.OwnerAccountName);
			if (GroupOwnerAccountName is not null)
				dict.SetObject (new NSString (GroupOwnerAccountName), NSFileManager.GroupOwnerAccountName);
			if (SystemNumber.HasValue)
				dict.SetObject (NSNumber.FromLong (SystemNumber.Value), NSFileManager.SystemNumber);
			if (DeviceIdentifier.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (DeviceIdentifier.Value), NSFileManager.DeviceIdentifier);
			if (GroupOwnerAccountID.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (GroupOwnerAccountID.Value), NSFileManager.GroupOwnerAccountID);
			if (Immutable.HasValue)
				dict.SetObject (NSNumber.FromBoolean (Immutable.Value), NSFileManager.Immutable);
			if (ModificationDate is not null)
				dict.SetObject (ModificationDate, NSFileManager.ModificationDate);
			if (OwnerAccountID.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (OwnerAccountID.Value), NSFileManager.OwnerAccountID);
			if (HfsCreatorCode.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (HfsCreatorCode.Value), NSFileManager.HfsCreatorCode);
			if (HfsTypeCode.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (HfsTypeCode.Value), NSFileManager.HfsTypeCode);
			if (PosixPermissions.HasValue)
				dict.SetObject (NSNumber.FromInt16 ((short) PosixPermissions.Value), NSFileManager.PosixPermissions);
			if (ReferenceCount.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (ReferenceCount.Value), NSFileManager.ReferenceCount);
			if (SystemFileNumber.HasValue)
				dict.SetObject (NSNumber.FromUnsignedLong (SystemFileNumber.Value), NSFileManager.SystemFileNumber);
			if (Size.HasValue)
				dict.SetObject (NSNumber.FromUInt64 (Size.Value), NSFileManager.Size);

			type = Type;

			if (type.HasValue) {
				v = null;
				switch (type.Value) {
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
				if (v is not null)
					dict.SetObject (v, NSFileManager.FileProtectionKey);
			}
#endif
			return dict;
		}

		#region fetch
		internal static bool? fetch_bool (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k is null)
				return null;
			return k.BoolValue;
		}

		internal static uint? fetch_uint (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k is null)
				return null;
			return k.UInt32Value;
		}

		internal static nuint? fetch_nuint (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k is null)
				return null;
			return k.UnsignedLongValue;
		}

		internal static nint? fetch_nint (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k is null)
				return null;
			return k.LongValue;
		}

		internal static ulong? fetch_ulong (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k is null)
				return null;
			return k.UInt64Value;
		}

		internal static long? fetch_long (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k is null)
				return null;
			return k.Int64Value;
		}

		internal static short? fetch_short (NSDictionary dict, NSString key)
		{
			var k = dict.ObjectForKey (key) as NSNumber;
			if (k is null)
				return null;
			return k.Int16Value;
		}
		#endregion

		public static NSFileAttributes? FromDictionary (NSDictionary dict)
		{
			if (dict is null)
				return null;
			var ret = new NSFileAttributes ();

			ret.AppendOnly = fetch_bool (dict, NSFileManager.AppendOnly);
			ret.Busy = fetch_bool (dict, NSFileManager.Busy);
			ret.ExtensionHidden = fetch_bool (dict, NSFileManager.ExtensionHidden);
			ret.CreationDate = dict.ObjectForKey (NSFileManager.CreationDate) as NSDate;
			ret.OwnerAccountName = dict.ObjectForKey (NSFileManager.OwnerAccountName) as NSString;
			ret.GroupOwnerAccountName = dict.ObjectForKey (NSFileManager.GroupOwnerAccountName) as NSString;
			ret.SystemNumber = fetch_nint (dict, NSFileManager.SystemNumber);
			ret.DeviceIdentifier = fetch_nuint (dict, NSFileManager.DeviceIdentifier);
			ret.GroupOwnerAccountID = fetch_nuint (dict, NSFileManager.GroupOwnerAccountID);
			ret.Immutable = fetch_bool (dict, NSFileManager.Immutable);
			ret.ModificationDate = dict.ObjectForKey (NSFileManager.ModificationDate) as NSDate;
			ret.OwnerAccountID = fetch_nuint (dict, NSFileManager.OwnerAccountID);
			ret.HfsCreatorCode = fetch_nuint (dict, NSFileManager.HfsCreatorCode);
			ret.HfsTypeCode = fetch_nuint (dict, NSFileManager.HfsTypeCode);
			ret.PosixPermissions = fetch_short (dict, NSFileManager.PosixPermissions);
			ret.ReferenceCount = fetch_nuint (dict, NSFileManager.ReferenceCount);
			ret.SystemFileNumber = fetch_nuint (dict, NSFileManager.SystemFileNumber);
			ret.Size = fetch_ulong (dict, NSFileManager.Size);

			var name = dict.ObjectForKey (NSFileManager.NSFileType) as NSString;
			if (name is not null) {
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

				ret.Type = type;
			}

#if !MONOMAC
			name = dict.ObjectForKey (NSFileManager.FileProtectionKey) as NSString;
			if (name is not null) {
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

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
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

		internal static NSFileSystemAttributes? FromDictionary (NSDictionary dict)
		{
			if (dict is null)
				return null;
			var ret = new NSFileSystemAttributes (dict);
			ret.Size = NSFileAttributes.fetch_ulong (dict, NSFileManager.SystemSize) ?? 0;
			ret.FreeSize = NSFileAttributes.fetch_ulong (dict, NSFileManager.SystemFreeSize) ?? 0;
			ret.Nodes = NSFileAttributes.fetch_long (dict, NSFileManager.SystemNodes) ?? 0;
			ret.FreeNodes = NSFileAttributes.fetch_long (dict, NSFileManager.SystemFreeNodes) ?? 0;
			ret.Number = NSFileAttributes.fetch_uint (dict, NSFileManager.SystemFreeNodes) ?? 0;

			return ret;
		}

		// For source code compatibility with users that had done manual NSDictionary lookups before
		public static implicit operator NSDictionary (NSFileSystemAttributes attr)
		{
			return attr.dict;
		}

	}

	public partial class NSFileManager {

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr NSUserName ();

		public static string? UserName {
			get {
				return CFString.FromHandle (NSUserName ());
			}
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr NSFullUserName ();

		public static string? FullUserName {
			get {
				return CFString.FromHandle (NSFullUserName ());
			}
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr NSHomeDirectory ();

		public static string? HomeDirectory {
			get {
				return CFString.FromHandle (NSHomeDirectory ());
			}
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr NSHomeDirectoryForUser (/* NSString */IntPtr userName);

		public static string? GetHomeDirectory (string userName)
		{
			if (userName is null)
				throw new ArgumentNullException (nameof (userName));

			var userNamePtr = CFString.CreateNative (userName);
			var rv = CFString.FromHandle (NSHomeDirectoryForUser (userNamePtr));
			CFString.ReleaseNative (userNamePtr);
			return rv;
		}

		[DllImport (Constants.FoundationLibrary)]
		static extern IntPtr NSTemporaryDirectory ();

		public static string? TemporaryDirectory {
			get {
				return CFString.FromHandle (NSTemporaryDirectory ());
			}
		}

		public bool SetAttributes (NSFileAttributes attributes, string path, out NSError error)
		{
			if (attributes is null)
				throw new ArgumentNullException (nameof (attributes));
			return SetAttributes (attributes.ToDictionary (), path, out error);
		}

		public bool SetAttributes (NSFileAttributes attributes, string path)
		{
			if (attributes is null)
				throw new ArgumentNullException (nameof (attributes));

			return SetAttributes (attributes.ToDictionary (), path, out _);
		}

		public bool CreateDirectory (string path, bool createIntermediates, NSFileAttributes? attributes, out NSError error)
		{
			return CreateDirectory (path, createIntermediates, attributes?.ToDictionary (), out error);
		}

		public bool CreateDirectory (string path, bool createIntermediates, NSFileAttributes? attributes)
		{
			return CreateDirectory (path, createIntermediates, attributes?.ToDictionary (), out var _);
		}

		public bool CreateFile (string path, NSData data, NSFileAttributes? attributes)
		{
			return CreateFile (path, data, attributes?.ToDictionary ());
		}

		public NSFileAttributes? GetAttributes (string path, out NSError error)
		{
			return NSFileAttributes.FromDictionary (_GetAttributes (path, out error));
		}

		public NSFileAttributes? GetAttributes (string path)
		{
			return NSFileAttributes.FromDictionary (_GetAttributes (path, out var _));
		}

		public NSFileSystemAttributes? GetFileSystemAttributes (string path)
		{
			return NSFileSystemAttributes.FromDictionary (_GetFileSystemAttributes (path, out var _));
		}

		public NSFileSystemAttributes? GetFileSystemAttributes (string path, out NSError error)
		{
			return NSFileSystemAttributes.FromDictionary (_GetFileSystemAttributes (path, out error));
		}

		public NSUrl [] GetMountedVolumes (NSString [] properties, NSVolumeEnumerationOptions options)
		{
			using var array = NSArray.FromNSObjects (properties);
			return GetMountedVolumes (array, options);
		}

		public string CurrentDirectory {
			get { return GetCurrentDirectory (); }
			// ignore boolean return value
			set { ChangeCurrentDirectory (value); }
		}

		public static NSError SetSkipBackupAttribute (string filename, bool skipBackup)
		{
			if (filename is null)
				throw new ArgumentNullException (nameof (filename));

			using (var url = NSUrl.FromFilename (filename)) {
				url.SetResource (NSUrl.IsExcludedFromBackupKey, (NSNumber) (skipBackup ? 1 : 0), out var error);
				return error;
			}
		}

		public static bool GetSkipBackupAttribute (string filename)
		{
			return GetSkipBackupAttribute (filename, out var _);
		}

		public static bool GetSkipBackupAttribute (string filename, out NSError error)
		{
			if (filename is null)
				throw new ArgumentNullException (nameof (filename));

			using (var url = NSUrl.FromFilename (filename)) {
				url.TryGetResource (NSUrl.IsExcludedFromBackupKey, out var value, out error);
				return (error is null) && ((long) ((NSNumber) value) == 1);
			}
		}
	}
}
