//
// Unit tests for NSUrlSessionConfiguration
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using Security;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.Security;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UrlSessionConfigurationTest {

		[Test]
		public void BackgroundSessionConfiguration ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertMacSystemVersion (10, 9, throwIfOtherPlatform: false);

			// https://trello.com/c/F6cyUBFU/70-simple-background-transfer-bo-pang-block-by-an-system-invalidcastexception-in-nsurlsessionconfiguration-backgroundsessionconfigu
			using (var session = NSUrlSessionConfiguration.BackgroundSessionConfiguration ("id")) {
				Assert.That (session.Identifier, Is.EqualTo ("id"), "Identifier");
			}
		}

		[Test]
		public void Default_Properties ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);
			TestRuntime.AssertMacSystemVersion (10, 9, throwIfOtherPlatform: false);

			var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;

			// in iOS9 those selectors do not respond - but they do work (forwarded to __NSCFURLSessionConfiguration type ?)

			Assert.True (config.AllowsCellularAccess, "allowsCellularAccess");
			config.AllowsCellularAccess = config.AllowsCellularAccess; // setAllowsCellularAccess:

			Assert.Null (config.ConnectionProxyDictionary, "connectionProxyDictionary");
			config.ConnectionProxyDictionary = null; // setConnectionProxyDictionary:

			Assert.False (config.Discretionary, "isDiscretionary");
			config.Discretionary = config.Discretionary; // setDiscretionary:

			Assert.Null (config.HttpAdditionalHeaders, "HTTPAdditionalHeaders");
			config.HttpAdditionalHeaders = config.HttpAdditionalHeaders; // setHTTPAdditionalHeaders:

			Assert.That (config.HttpCookieAcceptPolicy, Is.EqualTo (NSHttpCookieAcceptPolicy.OnlyFromMainDocumentDomain), "HTTPCookieAcceptPolicy");
			config.HttpCookieAcceptPolicy = config.HttpCookieAcceptPolicy; // setHTTPCookieAcceptPolicy:

			Assert.NotNull (config.HttpCookieStorage, "HTTPCookieStorage");
			config.HttpCookieStorage = config.HttpCookieStorage; // setHTTPCookieStorage:

			// iOS 7.x returned 6 (instead of 4)
			Assert.That (config.HttpMaximumConnectionsPerHost, Is.GreaterThanOrEqualTo (4), "HTTPMaximumConnectionsPerHost");
			config.HttpMaximumConnectionsPerHost = config.HttpMaximumConnectionsPerHost; // setHTTPMaximumConnectionsPerHost:

			Assert.True (config.HttpShouldSetCookies, "HTTPShouldSetCookies");
			config.HttpShouldSetCookies = config.HttpShouldSetCookies; // setHTTPShouldSetCookies:

			Assert.False (config.HttpShouldUsePipelining, "HTTPShouldUsePipelining");
			config.HttpShouldUsePipelining = config.HttpShouldUsePipelining; // setHTTPShouldUsePipelining:

			Assert.Null (config.Identifier, "identifier");

			Assert.That (config.NetworkServiceType, Is.EqualTo (NSUrlRequestNetworkServiceType.Default), "networkServiceType");
			config.NetworkServiceType = config.NetworkServiceType; // setNetworkServiceType:

			Assert.That (config.RequestCachePolicy, Is.EqualTo (NSUrlRequestCachePolicy.UseProtocolCachePolicy), "requestCachePolicy");
			config.RequestCachePolicy = config.RequestCachePolicy; // setRequestCachePolicy:

			Assert.False (config.SessionSendsLaunchEvents, "sessionSendsLaunchEvents");
			config.SessionSendsLaunchEvents = config.SessionSendsLaunchEvents; // setSessionSendsLaunchEvents:

			var hasSharedContainerIdentifier = true;
#if __MACOS__
			hasSharedContainerIdentifier = TestRuntime.CheckMacSystemVersion (10, 10);
#else
			hasSharedContainerIdentifier = TestRuntime.CheckXcodeVersion (6, 0);
#endif
			if (hasSharedContainerIdentifier) {
				Assert.Null (config.SharedContainerIdentifier, "sharedContainerIdentifier");
				config.SharedContainerIdentifier = config.SharedContainerIdentifier; // setSharedContainerIdentifier:
			}

			Assert.That (config.TimeoutIntervalForRequest, Is.GreaterThan (0), "timeoutIntervalForRequest");
			config.TimeoutIntervalForRequest = config.TimeoutIntervalForRequest; // setTimeoutIntervalForRequest:

			Assert.That (config.TimeoutIntervalForResource, Is.GreaterThan (0), "timeoutIntervalForResource");
			config.TimeoutIntervalForResource = config.TimeoutIntervalForResource; // setTimeoutIntervalForResource:

			var max = TestRuntime.CheckXcodeVersion (8,0) ? SslProtocol.Unknown : SslProtocol.Tls_1_2; // Unknown also means default
			Assert.That (config.TLSMaximumSupportedProtocol, Is.EqualTo (max), "TLSMaximumSupportedProtocol");
			config.TLSMaximumSupportedProtocol = config.TLSMaximumSupportedProtocol; // setTLSMaximumSupportedProtocol:

			Assert.That (config.TLSMinimumSupportedProtocol, Is.GreaterThanOrEqualTo (SslProtocol.Ssl_3_0), "TLSMinimumSupportedProtocol");
			config.TLSMinimumSupportedProtocol = config.TLSMinimumSupportedProtocol; // setTLSMinimumSupportedProtocol:

			Assert.NotNull (config.URLCache, "URLCache");
			config.URLCache = config.URLCache; // setURLCache:

			Assert.NotNull (config.URLCredentialStorage, "URLCredentialStorage");
			config.URLCredentialStorage = config.URLCredentialStorage; // setURLCredentialStorage:

			var hasProtocolClasses = true;
#if __MACOS__
			hasProtocolClasses = TestRuntime.CheckMacSystemVersion (10, 10);
#else
			hasProtocolClasses = TestRuntime.CheckXcodeVersion (6, 0);
#endif
			if (hasProtocolClasses) {
				Assert.NotNull (config.WeakProtocolClasses, "protocolClasses");
			} else {
				Assert.Null (config.WeakProtocolClasses, "protocolClasses");
			}
			config.WeakProtocolClasses = config.WeakProtocolClasses; // setProtocolClasses:
		}
	}
}
