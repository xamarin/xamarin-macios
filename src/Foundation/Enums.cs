// FIXME: EnumDesktop.cs should be merged into this file.

#if !MONOMAC

using XamCore.ObjCRuntime;

namespace XamCore.Foundation {

	// Utility enum, ObjC uses NSString
	[iOS (7,0)]
	public enum NSDocumentType {
		Unknown = -1,
		PlainText,
		RTF,
		RTFD,
		HTML
	}

	// Utility enum, ObjC uses NSString
	[iOS (7,0)]
	public enum NSDocumentViewMode {
		Normal,
		PageLayout
			
	}
}
#endif // !MONOMAC
