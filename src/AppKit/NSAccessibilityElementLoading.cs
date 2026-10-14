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
		public virtual NSAccessibilityElement? GetAccessibilityElement (INSSecureCoding token)
		{
			throw new You_Should_Not_Call_base_In_This_Method ();
		}
	}
#endif // COREBUILD
#endif // XAMCORE_5_0
#endif // !__MACCATALYST__
}
