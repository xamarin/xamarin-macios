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
			Assert.AreNotEqual (IntPtr.Zero, new Protocol (typeof (global::ModelIO.IMDLComponent)).Handle, "IMDLComponent type");
			Assert.AreNotEqual (IntPtr.Zero, new Protocol (typeof (INSObjectProtocol)).Handle, "NSObject type");

			Assert.AreNotEqual (IntPtr.Zero, new Protocol ("MDLComponent").Handle, "MDLComponent string");
			Assert.AreNotEqual (IntPtr.Zero, new Protocol ("NSObject").Handle, "NSObject string");

			Assert.AreEqual ("MDLComponent", new Protocol ("MDLComponent").Name, "MDLComponent name");
			Assert.AreEqual ("NSObject", new Protocol ("NSObject").Name, "NSObject name");

			Assert.AreEqual ("MDLComponent", new Protocol (typeof (global::ModelIO.IMDLComponent)).Name, "MDLComponent type name");
			Assert.AreEqual ("NSObject", new Protocol (typeof (INSObjectProtocol)).Name, "NSObject type name");

			Assert.AreEqual ("MDLComponent", new Protocol (new Protocol ("MDLComponent").Handle).Name, "NSObject IntPtr name");
			Assert.AreEqual ("NSObject", new Protocol (new Protocol ("NSObject").Handle).Name, "NSObject IntPtr name");

			Assert.AreEqual ("MDLComponent", new Protocol (Protocol.GetHandle ("MDLComponent")).Name, "NSObject GetHandle name");
			Assert.AreEqual ("NSObject", new Protocol (Protocol.GetHandle ("NSObject")).Name, "NSObject GetHandle name");
		}
	}
}

#endif // XAMCORE_2_0
