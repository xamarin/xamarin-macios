//
// Unit tests for Protocol
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2018 Microsoft Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime
{

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ProtocolTest
	{
		[Test]
		public void Ctors ()
		{
			var data = new [] {
				new { Type = typeof (INSObjectProtocol), Name = "NSObject" }, // protocol name doesn't match at all
				new { Type = typeof (INSUrlSessionDownloadDelegate), Name = "NSURLSessionDownloadDelegate" }, // different casing in native
				new { Type = typeof (global::CloudKit.ICKRecordValue), Name = "CKRecordValue" }, // protocol name is the same in native and managed
			};

			foreach (var d in data) {
				Assert.AreNotEqual (IntPtr.Zero, new Protocol (d.Type).Handle, $"{d.Name} type");
				Assert.AreNotEqual (IntPtr.Zero, new Protocol (d.Name).Handle, $"{d.Name} string");
				Assert.AreEqual (d.Name, new Protocol (d.Name).Name, $"{d.Name} name");
				Assert.AreEqual (d.Name, new Protocol (d.Type).Name, $"{d.Name} type name");
				Assert.AreEqual (d.Name, new Protocol (new Protocol (d.Name).Handle).Name, $"{d.Name} IntPtr name");
				Assert.AreEqual (d.Name, new Protocol (Protocol.GetHandle (d.Name)).Name, $"{d.Name} GetHandle name");
			}
		}
	}
}

#endif // XAMCORE_2_0
