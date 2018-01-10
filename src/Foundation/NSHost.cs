//
// NSHost.cs: augment NSHost with C#isms and
// System.Net's IPAddress and IPHostEntry.
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc. All rights reserved.
//

#if MONOMAC

using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace Foundation
{
	public partial class NSHost : IEquatable<NSHost>, IEnumerable<IPAddress>
	{
		static NSHost CheckNull (NSHost host)
		{
			if (host == null)
				return null;

			var addrs = host._Addresses;
			if (addrs == null || addrs.Length == 0)
				return null;

			return host;
		}

		public static NSHost Current {
			get { return CheckNull (_Current); }
		}

		public static NSHost FromAddress (string address)
		{
			if (address == null)
				return null;
			return CheckNull (_FromAddress (address));
		}

		public static NSHost FromName (string name)
		{
			if (name == null)
				return null;
			return CheckNull (_FromName (name));
		}

		public static explicit operator IPAddress (NSHost host)
		{
			return host.Address;
		}

		public static explicit operator NSHost (IPAddress address)
		{
			return FromAddress (address);
		}

		public static explicit operator IPHostEntry (NSHost host)
		{
			return host.ToIPHostEntry ();
		}

		public static explicit operator NSHost (IPHostEntry hostEntry)
		{
			return FromIPHostEntry (hostEntry);
		}

		public static NSHost FromIPHostEntry (IPHostEntry hostEntry)
		{
			if (hostEntry == null)
				return null;

			if (hostEntry.AddressList != null) {
				foreach (var addr in hostEntry.AddressList) {
					var host = FromAddress (addr);
					if (host != null)
						return host;
				}
			}

			if (hostEntry.HostName != null) {
				var host = FromName (hostEntry.HostName);
				if (host != null)
					return host;
			}

			if (hostEntry.Aliases != null) {
				foreach (var name in hostEntry.Aliases) {
					var host = FromName (name);
					if (host != null)
						return host;
				}
			}

			return null;
		}

		public IPHostEntry ToIPHostEntry ()
		{
			return new IPHostEntry {
				HostName = Name,
				AddressList = Addresses,
				Aliases = Names
			};
		}

		public static NSHost FromAddress (IPAddress address)
		{
			if (address == null)
				return null;
			return FromAddress (address.ToString ());
		}

		public IPAddress Address {
			get { return IPAddress.Parse (_Address); }
		}

		public IPAddress [] Addresses {
			get {
				var addrs = new IPAddress [_Addresses.Length];
				for (int i = 0; i < addrs.Length; i++)
					addrs [i] = IPAddress.Parse (_Addresses [i]);
				return addrs;
			}
		}

		public override int GetHashCode ()
		{
			return (int)_Hash;
		}

		public override bool Equals (object obj)
		{
			if (obj == this)
				return true;

			var host = obj as NSHost;
			if (host != null)
				return Equals (host);

			return false;
		}

		public IEnumerator<IPAddress> GetEnumerator ()
		{
			foreach (var address in Addresses)
				yield return address;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}

#endif // MONOMAC
