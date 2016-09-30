#if XAMARIN_APPLETLS || XAMARIN_NO_TLS
//
// OldTlsProvider.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2016 Xamarin, Inc.
//
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;
using MNS = Mono.Net.Security;

#if XAMARIN_NO_TLS
namespace XamCore.Security.NoTls
#else
namespace XamCore.Security.Tls
#endif
{
	class OldTlsProvider : MNS.LegacyTlsProvider
	{
	}
}
#endif

