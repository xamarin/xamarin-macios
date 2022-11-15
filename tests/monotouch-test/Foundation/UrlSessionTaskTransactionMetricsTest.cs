//
// Unit tests for NSUrlSessionTaskTransactionMetrics
//
// Authors:
//	Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlSessionTaskTransactionMetricsTest {

		[Test]
		public void Properties ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			using (var sttm = new NSUrlSessionTaskTransactionMetrics ()) {
				// in iOS10 those selectors do not respond - but they do work (forwarded to __NSCFURLSessionTaskMetrics type ?)
				Assert.Null (sttm.ConnectEndDate, "RedirectCount");
				Assert.Null (sttm.ConnectStartDate, "TaskInterval");
				Assert.Null (sttm.DomainLookupEndDate, "TransactionMetrics");
				Assert.Null (sttm.DomainLookupStartDate, "TransactionMetrics");
				if (TestRuntime.CheckXcodeVersion (11, 0)) {
					Assert.NotNull (sttm.FetchStartDate, "TransactionMetrics");
				} else {
					Assert.Null (sttm.FetchStartDate, "TransactionMetrics");
				}
				Assert.Null (sttm.NetworkProtocolName, "TransactionMetrics");
				Assert.False (sttm.ProxyConnection, "TransactionMetrics");
				Assert.NotNull (sttm.Request, "TransactionMetrics");
				if (TestRuntime.CheckXcodeVersion (11, 0)) {
					Assert.NotNull (sttm.RequestEndDate, "TransactionMetrics");
					Assert.NotNull (sttm.RequestStartDate, "TransactionMetrics");
				} else {
					Assert.Null (sttm.RequestEndDate, "TransactionMetrics");
					Assert.Null (sttm.RequestStartDate, "TransactionMetrics");
				}
				Assert.That (sttm.ResourceFetchType, Is.EqualTo (NSUrlSessionTaskMetricsResourceFetchType.Unknown), "ResourceFetchType");
				Assert.Null (sttm.Response, "Response");
				if (TestRuntime.CheckXcodeVersion (11, 0)) {
					Assert.NotNull (sttm.ResponseEndDate, "ResponseEndDate");
					Assert.NotNull (sttm.ResponseStartDate, "ResponseStartDate");
				} else {
					Assert.Null (sttm.ResponseEndDate, "ResponseEndDate");
					Assert.Null (sttm.ResponseStartDate, "ResponseStartDate");
				}
				Assert.That (sttm.ReusedConnection, Is.EqualTo (true).Or.EqualTo (false), "ReusedConnection");
				Assert.Null (sttm.SecureConnectionEndDate, "SecureConnectionEndDate");
				Assert.Null (sttm.SecureConnectionStartDate, "SecureConnectionStartDate");
			}
		}
	}
}
