#if !__MACCATALYST__

using System.ComponentModel;

using CoreFoundation;
using Foundation;

#nullable enable

namespace AppKit {
#if !XAMCORE_5_0
#if !COREBUILD
	public partial interface INSAccessibilityElementLoading {
		/// <summary>Use <see cref="GetAccessibilityElement2" /> instead.</summary>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'GetAccessibilityElement2' instead.")]
		[Export ("accessibilityElementWithToken:")]
		public virtual NSAccessibilityElement? GetAccessibilityElement (INSSecureCoding token)
		{
			return (NSAccessibilityElement?) _GetAccessibilityElement2 (this, token);
		}
	}
#endif // COREBUILD
#endif // XAMCORE_5_0
}
#endif // !__MACCATALYST__
