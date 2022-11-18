//
// MessageUI.cs: This file describes the API that the generator will produce for MessageUI
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2014 Xamarin, Inc.
//

using ObjCRuntime;

namespace MessageUI {

#if NET
	[Native]
	public enum MFMailComposeResult : long {
#else
	// Before iOS 10 beta 3, this was an untyped enum -> MFMailComposeViewController.h
	// Note: now used as a NSInteger in the API.
	public enum MFMailComposeResult {
#endif
		Cancelled,
		Saved,
		Sent,
		Failed
	}

	[ErrorDomain ("MFMailComposeErrorDomain")]
#if NET
	[Native]
	public enum MFMailComposeErrorCode : long {
#else
	// Before iOS 10 beta 3, this was an untyped enum -> MFMailComposeViewController.h
	// Note: now used as a NSInteger in the API.
	public enum MFMailComposeErrorCode {
#endif
		SaveFailed,
		SendFailed
	}

#if NET
	[Native]
	public enum MessageComposeResult : long {
#else
	// Before iOS 10 beta 3, this was an untyped enum -> MFMessageComposeViewController.h
	// Note: now used as a NSInteger in the API.
	public enum MessageComposeResult {
#endif
		Cancelled, Sent, Failed
	}
}
