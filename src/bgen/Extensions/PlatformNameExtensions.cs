using System.Diagnostics.CodeAnalysis;

public static class PlatformNameExtensions {

	public static bool TryGetApplicationClassName (this PlatformName currentPlatform, [NotNullWhen (true)] out string? className)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
			className = "UIApplication";
			return true;
		case PlatformName.MacOSX:
			className = "NSApplication";
			return true;
		default:
			className = null;
			return false;
		}
	}

	public static bool TryGetCoreImageMap (this PlatformName currentPlatform, [NotNullWhen (true)] out string? coreImageMap)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
			coreImageMap = "CoreImage";
			return true;
		case PlatformName.MacOSX:
			coreImageMap = "Quartz";
			return true;
		default:
			coreImageMap = null;
			return false;
		}
	}

	public static bool TryGetCoreServicesMap (this PlatformName currentPlatform, [NotNullWhen (true)] out string? coreServicesMap)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
			coreServicesMap = "MobileCoreServices";
			return true;
		case PlatformName.MacOSX:
			coreServicesMap = "CoreServices";
			return true;
		default:
			coreServicesMap = null;
			return false;
		}
	}

	public static bool TryGetPDFKitMap (this PlatformName currentPlatform, [NotNullWhen (true)] out string? pdfKitMap)
	{
		switch (currentPlatform) {
		case PlatformName.iOS:
		case PlatformName.MacCatalyst:
			pdfKitMap = "PDFKit";
			return true;
		case PlatformName.MacOSX:
			pdfKitMap = "Quartz";
			return true;
		default:
			pdfKitMap = null;
			return false;
		}
	}
}
