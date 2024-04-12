// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Based on https://github.com/dotnet/runtime/blob/c2848c582f5d6ae42c89f5bfe0818687ab3345f0/src/libraries/Common/src/System/Net/Security/CertificateHelper.cs
// with the NetEventSource code removed and namespace changed.

using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Foundation
{
    internal static partial class CertificateHelper
    {
        private const string ClientAuthenticationOID = "1.3.6.1.5.5.7.3.2";

        internal static X509Certificate2? GetEligibleClientCertificate(X509CertificateCollection? candidateCerts)
        {
            if (candidateCerts == null || candidateCerts.Count == 0)
            {
                return null;
            }

            var certs = new X509Certificate2Collection();
            certs.AddRange(candidateCerts);

            return GetEligibleClientCertificate(certs);
        }

        internal static X509Certificate2? GetEligibleClientCertificate(X509Certificate2Collection? candidateCerts)
        {
            if (candidateCerts == null || candidateCerts.Count == 0)
            {
                return null;
            }

            foreach (X509Certificate2 cert in candidateCerts)
            {
                if (!cert.HasPrivateKey)
                {
                    continue;
                }

                if (IsValidClientCertificate(cert))
                {
                    return cert;
                }
            }
            return null;
        }

        private static bool IsValidClientCertificate(X509Certificate2 cert)
        {
            foreach (X509Extension extension in cert.Extensions)
            {
                if ((extension is X509EnhancedKeyUsageExtension eku) && !IsValidForClientAuthenticationEKU(eku))
                {
                    return false;
                }
                else if ((extension is X509KeyUsageExtension ku) && !IsValidForDigitalSignatureUsage(ku))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsValidForClientAuthenticationEKU(X509EnhancedKeyUsageExtension eku)
        {
            foreach (Oid oid in eku.EnhancedKeyUsages)
            {
                if (oid.Value == ClientAuthenticationOID)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsValidForDigitalSignatureUsage(X509KeyUsageExtension ku)
        {
            const X509KeyUsageFlags RequiredUsages = X509KeyUsageFlags.DigitalSignature;
            return (ku.KeyUsages & RequiredUsages) == RequiredUsages;
        }
    }
}