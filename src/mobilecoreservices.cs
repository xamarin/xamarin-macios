using Foundation;
using ObjCRuntime;

namespace MobileCoreServices {

	[Deprecated (PlatformName.iOS, 14, 0, message: "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
	[Deprecated (PlatformName.TvOS, 14, 0, message: "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
	[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'UniformTypeIdentifiers.UTType' API instead.")]
	[Partial]
	interface UTType {
		[Field ("kUTTypeItem", "+CoreServices")]
		NSString Item { get; }

		[Field ("kUTTypeContent", "+CoreServices")]
		NSString Content { get; }

		[Field ("kUTTypeCompositeContent", "+CoreServices")]
		NSString CompositeContent { get; }

		[Field ("kUTTypeMessage", "+CoreServices")]
		NSString Message { get; }

		[Field ("kUTTypeContact", "+CoreServices")]
		NSString Contact { get; }

		[Field ("kUTTypeArchive", "+CoreServices")]
		NSString Archive { get; }

		[Field ("kUTTypeDiskImage", "+CoreServices")]
		NSString DiskImage { get; }

		[Field ("kUTTypeData", "+CoreServices")]
		NSString Data { get; }

		[Field ("kUTTypeDirectory", "+CoreServices")]
		NSString Directory { get; }

		[Field ("kUTTypeResolvable", "+CoreServices")]
		NSString Resolvable { get; }

		[Field ("kUTTypeSymLink", "+CoreServices")]
		NSString SymLink { get; }

		[Field ("kUTTypeExecutable", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Executable { get; }

		[Field ("kUTTypeMountPoint", "+CoreServices")]
		NSString MountPoint { get; }

		[Field ("kUTTypeAliasFile", "+CoreServices")]
		NSString AliasFile { get; }

		[Field ("kUTTypeAliasRecord", "+CoreServices")]
		NSString AliasRecord { get; }

		[Field ("kUTTypeURLBookmarkData", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString URLBookmarkData { get; }

		[Field ("kUTTypeURL", "+CoreServices")]
		NSString URL { get; }

		[Field ("kUTTypeFileURL", "+CoreServices")]
		NSString FileURL { get; }

		[Field ("kUTTypeText", "+CoreServices")]
		NSString Text { get; }

		[Field ("kUTTypePlainText", "+CoreServices")]
		NSString PlainText { get; }

		[Field ("kUTTypeUTF8PlainText", "+CoreServices")]
		NSString UTF8PlainText { get; }

		[Field ("kUTTypeUTF16ExternalPlainText", "+CoreServices")]
		NSString UTF16ExternalPlainText { get; }

		[Field ("kUTTypeUTF16PlainText", "+CoreServices")]
		NSString UTF16PlainText { get; }

		[Field ("kUTTypeDelimitedText", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString DelimitedText { get; }

		[Field ("kUTTypeCommaSeparatedText", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString CommaSeparatedText { get; }

		[Field ("kUTTypeTabSeparatedText", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString TabSeparatedText { get; }

		[Field ("kUTTypeUTF8TabSeparatedText", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString UTF8TabSeparatedText { get; }

		[Field ("kUTTypeRTF", "+CoreServices")]
		NSString RTF { get; }

		[Field ("kUTTypeHTML", "+CoreServices")]
		NSString HTML { get; }

		[Field ("kUTTypeXML", "+CoreServices")]
		NSString XML { get; }

		[Field ("kUTTypeSourceCode", "+CoreServices")]
		NSString SourceCode { get; }

		[Field ("kUTTypeAssemblyLanguageSource", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString AssemblyLanguageSource { get; }

		[Field ("kUTTypeCSource", "+CoreServices")]
		NSString CSource { get; }

		[Field ("kUTTypeObjectiveCSource", "+CoreServices")]
		NSString ObjectiveCSource { get; }

		[Field ("kUTTypeCPlusPlusSource", "+CoreServices")]
		NSString CPlusPlusSource { get; }

		[Field ("kUTTypeObjectiveCPlusPlusSource", "+CoreServices")]
		NSString ObjectiveCPlusPlusSource { get; }

		[Field ("kUTTypeCHeader", "+CoreServices")]
		NSString CHeader { get; }

		[Field ("kUTTypeCPlusPlusHeader", "+CoreServices")]
		NSString CPlusPlusHeader { get; }

		[Field ("kUTTypeJavaSource", "+CoreServices")]
		NSString JavaSource { get; }

		[Field ("kUTTypeScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Script { get; }

		[Field ("kUTTypeAppleScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString AppleScript { get; }

		[Field ("kUTTypeOSAScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString OSAScript { get; }

		[Field ("kUTTypeOSAScriptBundle", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString OSAScriptBundle { get; }

		[Field ("kUTTypeJavaScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString JavaScript { get; }

		[Field ("kUTTypeShellScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString ShellScript { get; }

		[Field ("kUTTypePerlScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString PerlScript { get; }

		[Field ("kUTTypePythonScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString PythonScript { get; }

		[Field ("kUTTypeRubyScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString RubyScript { get; }

		[Field ("kUTTypePHPScript", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString PHPScript { get; }

		[Field ("kUTTypeJSON", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString JSON { get; }

		[Field ("kUTTypePropertyList", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString PropertyList { get; }

		[Field ("kUTTypeXMLPropertyList", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString XMLPropertyList { get; }

		[Field ("kUTTypeBinaryPropertyList", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString BinaryPropertyList { get; }

		[Field ("kUTTypePDF", "+CoreServices")]
		NSString PDF { get; }

		[Field ("kUTTypeRTFD", "+CoreServices")]
		NSString RTFD { get; }

		[Field ("kUTTypeFlatRTFD", "+CoreServices")]
		NSString FlatRTFD { get; }

		[Field ("kUTTypeTXNTextAndMultimediaData", "+CoreServices")]
		NSString TXNTextAndMultimediaData { get; }

		[Field ("kUTTypeWebArchive", "+CoreServices")]
		NSString WebArchive { get; }

		[Field ("kUTTypeImage", "+CoreServices")]
		NSString Image { get; }

		[Field ("kUTTypeJPEG", "+CoreServices")]
		NSString JPEG { get; }

		[Field ("kUTTypeJPEG2000", "+CoreServices")]
		NSString JPEG2000 { get; }

		[Field ("kUTTypeTIFF", "+CoreServices")]
		NSString TIFF { get; }

		[Field ("kUTTypePICT", "+CoreServices")]
		NSString PICT { get; }

		[Field ("kUTTypeGIF", "+CoreServices")]
		NSString GIF { get; }

		[Field ("kUTTypePNG", "+CoreServices")]
		NSString PNG { get; }

		[Field ("kUTTypeQuickTimeImage", "+CoreServices")]
		NSString QuickTimeImage { get; }

		[Field ("kUTTypeAppleICNS", "+CoreServices")]
		NSString AppleICNS { get; }

		[Field ("kUTTypeBMP", "+CoreServices")]
		NSString BMP { get; }

		[Field ("kUTTypeICO", "+CoreServices")]
		NSString ICO { get; }

		[Field ("kUTTypeRawImage", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString RawImage { get; }

		[Field ("kUTTypeScalableVectorGraphics", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString ScalableVectorGraphics { get; }

		[Field ("kUTTypeAudiovisualContent", "+CoreServices")]
		NSString AudiovisualContent { get; }

		[Field ("kUTTypeMovie", "+CoreServices")]
		NSString Movie { get; }

		[Field ("kUTTypeVideo", "+CoreServices")]
		NSString Video { get; }

		[Field ("kUTTypeAudio", "+CoreServices")]
		NSString Audio { get; }

		[Field ("kUTTypeQuickTimeMovie", "+CoreServices")]
		NSString QuickTimeMovie { get; }

		[Field ("kUTTypeMPEG", "+CoreServices")]
		NSString MPEG { get; }

		[Field ("kUTTypeMPEG2Video", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString MPEG2Video { get; }

		[Field ("kUTTypeMPEG2TransportStream", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString MPEG2TransportStream { get; }

		[Field ("kUTTypeMP3", "+CoreServices")]
		NSString MP3 { get; }

		[Field ("kUTTypeMPEG4", "+CoreServices")]
		NSString MPEG4 { get; }

		[Field ("kUTTypeMPEG4Audio", "+CoreServices")]
		NSString MPEG4Audio { get; }

		[Field ("kUTTypeAppleProtectedMPEG4Audio", "+CoreServices")]
		NSString AppleProtectedMPEG4Audio { get; }

		[Field ("kUTTypeAppleProtectedMPEG4Video", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString AppleProtectedMPEG4Video { get; }

		[Field ("kUTTypeAVIMovie", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString AVIMovie { get; }

		[Field ("kUTTypeAudioInterchangeFileFormat", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString AudioInterchangeFileFormat { get; }

		[Field ("kUTTypeWaveformAudio", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString WaveformAudio { get; }

		[Field ("kUTTypeMIDIAudio", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString MIDIAudio { get; }

		[Field ("kUTTypePlaylist", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Playlist { get; }

		[Field ("kUTTypeM3UPlaylist", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString M3UPlaylist { get; }

		[Field ("kUTTypeFolder", "+CoreServices")]
		NSString Folder { get; }

		[Field ("kUTTypeVolume", "+CoreServices")]
		NSString Volume { get; }

		[Field ("kUTTypePackage", "+CoreServices")]
		NSString Package { get; }

		[Field ("kUTTypeBundle", "+CoreServices")]
		NSString Bundle { get; }

		[Field ("kUTTypePluginBundle", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString PluginBundle { get; }

		[Field ("kUTTypeSpotlightImporter", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString SpotlightImporter { get; }

		[Field ("kUTTypeQuickLookGenerator", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString QuickLookGenerator { get; }

		[Field ("kUTTypeXPCService", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString XPCService { get; }

		[Field ("kUTTypeFramework", "+CoreServices")]
		NSString Framework { get; }

		[Field ("kUTTypeApplication", "+CoreServices")]
		NSString Application { get; }

		[Field ("kUTTypeApplicationBundle", "+CoreServices")]
		NSString ApplicationBundle { get; }

		[Field ("kUTTypeApplicationFile", "+CoreServices")]
		NSString ApplicationFile { get; }

		[Field ("kUTTypeUnixExecutable", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString UnixExecutable { get; }

		[Field ("kUTTypeWindowsExecutable", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString WindowsExecutable { get; }

		[Field ("kUTTypeJavaClass", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString JavaClass { get; }

		[Field ("kUTTypeJavaArchive", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString JavaArchive { get; }

		[Field ("kUTTypeSystemPreferencesPane", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString SystemPreferencesPane { get; }

		[Field ("kUTTypeGNUZipArchive", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString GNUZipArchive { get; }

		[Field ("kUTTypeBzip2Archive", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Bzip2Archive { get; }

		[Field ("kUTTypeZipArchive", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString ZipArchive { get; }

		[Field ("kUTTypeSpreadsheet", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Spreadsheet { get; }

		[Field ("kUTTypePresentation", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Presentation { get; }

		[Field ("kUTTypeDatabase", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Database { get; }

		[Field ("kUTTypeVCard", "+CoreServices")]
		NSString VCard { get; }

		[Field ("kUTTypeToDoItem", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString ToDoItem { get; }

		[Field ("kUTTypeCalendarEvent", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString CalendarEvent { get; }

		[Field ("kUTTypeEmailMessage", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString EmailMessage { get; }

		[Field ("kUTTypeInternetLocation", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString InternetLocation { get; }

		[Field ("kUTTypeInkText", "+CoreServices")]
		NSString InkText { get; }

		[Field ("kUTTypeFont", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Font { get; }

		[Field ("kUTTypeBookmark", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Bookmark { get; }

		[Field ("kUTType3DContent", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString ThreeDContent { get; }

		[Field ("kUTTypePKCS12", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString PKCS12 { get; }

		[Field ("kUTTypeX509Certificate", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString X509Certificate { get; }

		[Field ("kUTTypeElectronicPublication", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString ElectronicPublication { get; }

		[Field ("kUTTypeLog", "+CoreServices")]
		[MacCatalyst (13, 1)]
		NSString Log { get; }

		[Field ("kUTExportedTypeDeclarationsKey", "+CoreServices")]
		NSString ExportedTypeDeclarationsKey { get; }

		[Field ("kUTImportedTypeDeclarationsKey", "+CoreServices")]
		NSString ImportedTypeDeclarationsKey { get; }

		[Field ("kUTTypeIdentifierKey", "+CoreServices")]
		NSString IdentifierKey { get; }

		[Field ("kUTTypeTagSpecificationKey", "+CoreServices")]
		NSString TagSpecificationKey { get; }

		[Field ("kUTTypeConformsToKey", "+CoreServices")]
		NSString ConformsToKey { get; }

		[Field ("kUTTypeDescriptionKey", "+CoreServices")]
		NSString DescriptionKey { get; }

		[Field ("kUTTypeIconFileKey", "+CoreServices")]
		NSString IconFileKey { get; }

		[Field ("kUTTypeReferenceURLKey", "+CoreServices")]
		NSString ReferenceURLKey { get; }

		[Field ("kUTTypeVersionKey", "+CoreServices")]
		NSString VersionKey { get; }

		[Field ("kUTTagClassFilenameExtension", "+CoreServices")]
		NSString TagClassFilenameExtension { get; }

		[Field ("kUTTagClassMIMEType", "+CoreServices")]
		NSString TagClassMIMEType { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("kUTTagClassNSPboardType", "+CoreServices")]
		NSString TagClassNSPboardType { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("kUTTagClassOSType", "+CoreServices")]
		NSString TagClassOSType { get; }

		[MacCatalyst (13, 1)]
		[Field ("kUTTypeSwiftSource", "+CoreServices")]
		NSString SwiftSource { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("kUTTypeAlembic", "ModelIO")]
		NSString Alembic { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("kUTType3dObject", "ModelIO")]
		NSString k3dObject { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("kUTTypePolygon", "ModelIO")]
		NSString Polygon { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("kUTTypeStereolithography", "ModelIO")]
		NSString Stereolithography { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("kUTTypeUniversalSceneDescription", "ModelIO")]
		NSString UniversalSceneDescription { get; }

		[NoWatch]
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("kUTTypeUniversalSceneDescriptionMobile", "ModelIO")]
		NSString UniversalSceneDescriptionMobile { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kUTTypeLivePhoto", "+CoreServices")]
		NSString LivePhoto { get; }
	}
}
