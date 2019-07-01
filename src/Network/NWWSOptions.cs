//
// NWWSOptions.cs: Bindings the Network WebSockets API
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2019 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace Network {

	public enum NWWSOpcode {
		Invalid = -1,
		Count = 0,
		Text = 1,
		Binary = 2,
		Close = 8
		Ping = 9,
		Pong = 0xa
	}

	public enum NWWSCloseCode {
		NormalClosure = 1000,
		GoingAway = 1001,
		ProtocolError = 1002,
		UnsupportedData = 1003,
		NoStatusReceived = 1005,
		AbnormalClosure = 1006,
		InvalidFramePayloadData = 1007,
		PolicyViolation = 1008,
		MessageTooBig = 1009,
		MandatoryExtension = 1010,
		InternalServerError = 1011,
		TlsHandshake = 1015
	}
}
