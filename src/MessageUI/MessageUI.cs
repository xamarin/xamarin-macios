//
// MessageUI.cs: This file describes the API that the generator will produce for MessageUI
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2014 Xamarin, Inc.
//

using XamCore.ObjCRuntime;

namespace XamCore.MessageUI {

	// untyped enum -> MFMailComposeViewController.h
	public enum MFMailComposeResult {
		Cancelled,
		Saved,
		Sent,
		Failed
	}

	// untyped enum -> MFMailComposeViewController.h
	[ErrorDomain ("MFMailComposeErrorDomain")]
	public enum MFMailComposeErrorCode {
	    SaveFailed,
	    SendFailed
	}

	// untype enum -> MFMessageComposeViewController.h
	public enum MessageComposeResult {
		Cancelled, Sent, Failed
	}
}