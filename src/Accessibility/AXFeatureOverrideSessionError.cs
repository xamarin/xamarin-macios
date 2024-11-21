using System;
using System.Runtime.Versioning;
using System.Diagnostics.CodeAnalysis;

using Foundation;
using ObjCRuntime;

#if __IOS__ && !__MACCATALYST__

namespace Accessibility {
	/// <summary>Extension methods for the <see cref="global::Accessibility.AXFeatureOverrideSessionError" /> enumeration.</summary>
	/// <remarks>
	///   <para>The extension method for the <see cref="global::Accessibility.AXFeatureOverrideSessionError" /> enumeration can be used to fetch the error domain associated with these error codes.</para>
	/// </remarks>
	[SupportedOSPlatform ("ios18.2")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	public static partial class AXFeatureOverrideSessionErrorExtensions {
		/// <summary>Returns the error domain associated with the <a cref="global::Accessibility.AXFeatureOverrideSessionError" /> value.</summary>
		/// <param name="self">The enumeration value</param>
		/// <remarks>
		///   <para>See the <see cref="global::Foundation.NSError" /> for information on how to use the error domains when reporting errors.</para>
		/// </remarks>
		public static NSString? GetDomain (this AXFeatureOverrideSessionError self)
		{
			// This is defined as a constant in the headers.
			return (NSString) "AXFeatureOverrideSessionErrorDomain";
		}
	}
}
#endif // __IOS__ && !__MACCATALYST__
