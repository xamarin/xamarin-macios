#if !WATCH

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using MapKit;

#nullable enable

namespace MapKit {
	/// <summary>This enum is used to select how to initialize a new instance of a <see cref="MKAddressFilter" />.</summary>
	public enum MKAddressFilterConstructorOption {
		/// <summary>The <c>options</c> parameter passed to the constructor are inclusive.</summary>
		Include,
		/// <summary>The <c>options</c> parameter passed to the constructor are exclusive.</summary>
		Exclude,
	}

	public partial class MKAddressFilter {
		/// <summary>Create a new <see cref="MKAddressFilter" /> with the specified address filter options.</summary>
		/// <param name="options">The address filter options to use.</param>
		/// <param name="constructorOption">Specify whether the <paramref name="options" /> argument is including or excluding the given options.</param>
		/// <returns>A new <see cref="MKAddressFilter" /> instance with the specified address filter options.</returns>
		public MKAddressFilter (MKAddressFilterOption options, MKAddressFilterConstructorOption constructorOption)
			: base (NSObjectFlag.Empty)
		{
			switch (constructorOption) {
			case MKAddressFilterConstructorOption.Include:
				InitializeHandle (_InitIncludingOptions (options));
				break;
			case MKAddressFilterConstructorOption.Exclude:
				InitializeHandle (_InitExcludingOptions (options));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (constructorOption), constructorOption, "Invalid enum value.");
			}
		}
	}
}

#endif
