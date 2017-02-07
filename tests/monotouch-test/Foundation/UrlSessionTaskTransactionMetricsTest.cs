//
// Unit tests for NSUrlSessionTaskTransactionMetrics
//
// Authors:
//	Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlSessionTaskTransactionMetricsTest {

		[Test]
		public void Properties ()
		{
			TestRuntime.AssertXcodeVersion (8,0);

			using (var sttm = new NSUrlSessionTaskTransactionMetrics ()) {
				// in iOS10 those selectors do not respond - but they do work (forwarded to __NSCFURLSessionTaskMetrics type ?)
				Assert.Null (sttm.ConnectEndDate, "RedirectCount");
				Assert.Null (sttm.ConnectStartDate, "TaskInterval");
				Assert.Null (sttm.DomainLookupEndDate, "TransactionMetrics");
				Assert.Null (sttm.DomainLookupStartDate, "TransactionMetrics");
				Assert.Null (sttm.FetchStartDate, "TransactionMetrics");
				Assert.Null (sttm.NetworkProtocolName, "TransactionMetrics");
				Assert.False (sttm.ProxyConnection, "TransactionMetrics");
				Assert.NotNull (sttm.Request, "TransactionMetrics");
				Assert.Null (sttm.RequestEndDate, "TransactionMetrics");
				Assert.Null (sttm.RequestStartDate, "TransactionMetrics");
				Assert.That (sttm.ResourceFetchType, Is.EqualTo (NSUrlSessionTaskMetricsResourceFetchType.Unknown),  "ResourceFetchType");
				Assert.Null (sttm.Response, "Response");
				Assert.Null (sttm.ResponseEndDate, "ResponseEndDate");
				Assert.Null (sttm.ResponseStartDate, "ResponseStartDate");
				Assert.False (sttm.ReusedConnection, "ReusedConnection");
				Assert.Null (sttm.SecureConnectionEndDate, "SecureConnectionEndDate");
				Assert.Null (sttm.SecureConnectionStartDate, "SecureConnectionStartDate");
			}
		}
	}
}
