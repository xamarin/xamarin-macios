using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.MobileCoreServices {

	[Partial]
	interface UTType {
		[Field ("kUTTypeItem", "+CoreServices")]
		NSString Item { get; }
		
		[Field ("kUTTypeContent", "+CoreServices")]
		NSString Content { get; }
		
		[Field ("kUTTypeCompositeContent", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString CompositeContent { get; }
		 
		[Field ("kUTTypeMessage", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Message { get; }
		
		[Field ("kUTTypeContact", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Contact { get; }
		
		[Field ("kUTTypeArchive", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Archive { get; }
		
		[Field ("kUTTypeDiskImage", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString DiskImage { get; }
		
		[Field ("kUTTypeData", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Data { get; }
		
		[Field ("kUTTypeDirectory", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Directory { get; }
		
		[Field ("kUTTypeResolvable", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Resolvable { get; }
		
		[Field ("kUTTypeSymLink", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString SymLink { get; }
		
		[Field ("kUTTypeExecutable", "+CoreServices")]
		[Mac (10,10), iOS(8,0)] // Symbol not found: _kUTTypeExecutable in 10.9
		NSString Executable { get; }
		
		[Field ("kUTTypeMountPoint", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString MountPoint { get; }
		
		[Field ("kUTTypeAliasFile", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString AliasFile { get; }
		
		[Field ("kUTTypeAliasRecord", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString AliasRecord { get; }
		
		[Field ("kUTTypeURLBookmarkData", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString URLBookmarkData { get; }
		
		[Field ("kUTTypeURL", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString URL { get; }
		
		[Field ("kUTTypeFileURL", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString FileURL { get; }
		
		[Field ("kUTTypeText", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Text { get; }
		
		[Field ("kUTTypePlainText", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString PlainText { get; }
		
		[Field ("kUTTypeUTF8PlainText", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString UTF8PlainText { get; }
		
		[Field ("kUTTypeUTF16ExternalPlainText", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString UTF16ExternalPlainText { get; }
		
		[Field ("kUTTypeUTF16PlainText", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString UTF16PlainText { get; }
		
		[Field ("kUTTypeDelimitedText", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString DelimitedText { get; }
		
		[Field ("kUTTypeCommaSeparatedText", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString CommaSeparatedText { get; }
		
		[Field ("kUTTypeTabSeparatedText", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString TabSeparatedText { get; }
		
		[Field ("kUTTypeUTF8TabSeparatedText", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString UTF8TabSeparatedText { get; }
		
		[Field ("kUTTypeRTF", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString RTF { get; }
		
		[Field ("kUTTypeHTML", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString HTML { get; }
		
		[Field ("kUTTypeXML", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString XML { get; }
		
		[Field ("kUTTypeSourceCode", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString SourceCode { get; }
		
		[Field ("kUTTypeAssemblyLanguageSource", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString AssemblyLanguageSource { get; }
		
		[Field ("kUTTypeCSource", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString CSource { get; }
		
		[Field ("kUTTypeObjectiveCSource", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString ObjectiveCSource { get; }
		
		[Field ("kUTTypeCPlusPlusSource", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString CPlusPlusSource { get; }
		
		[Field ("kUTTypeObjectiveCPlusPlusSource", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString ObjectiveCPlusPlusSource { get; }
		
		[Field ("kUTTypeCHeader", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString CHeader { get; }
		
		[Field ("kUTTypeCPlusPlusHeader", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString CPlusPlusHeader { get; }
		
		[Field ("kUTTypeJavaSource", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString JavaSource { get; }
		
		[Field ("kUTTypeScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString Script { get; }
		
		[Field ("kUTTypeAppleScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString AppleScript { get; }
		
		[Field ("kUTTypeOSAScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString OSAScript { get; }
		
		[Field ("kUTTypeOSAScriptBundle", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString OSAScriptBundle { get; }
		
		[Field ("kUTTypeJavaScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString JavaScript { get; }
		
		[Field ("kUTTypeShellScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString ShellScript { get; }
		
		[Field ("kUTTypePerlScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString PerlScript { get; }
		
		[Field ("kUTTypePythonScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString PythonScript { get; }
		
		[Field ("kUTTypeRubyScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString RubyScript { get; }
		
		[Field ("kUTTypePHPScript", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString PHPScript { get; }
		
		[Field ("kUTTypeJSON", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString JSON { get; }
		
		[Field ("kUTTypePropertyList", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString PropertyList { get; }
		
		[Field ("kUTTypeXMLPropertyList", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString XMLPropertyList { get; }
		
		[Field ("kUTTypeBinaryPropertyList", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString BinaryPropertyList { get; }
		
		[Field ("kUTTypePDF", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString PDF { get; }
		
		[Field ("kUTTypeRTFD", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString RTFD { get; }
		
		[Field ("kUTTypeFlatRTFD", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString FlatRTFD { get; }
		
		[Field ("kUTTypeTXNTextAndMultimediaData", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString TXNTextAndMultimediaData { get; }
		
		[Field ("kUTTypeWebArchive", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString WebArchive { get; }
		
		[Field ("kUTTypeImage", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Image { get; }
		
		[Field ("kUTTypeJPEG", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString JPEG { get; }
		
		[Field ("kUTTypeJPEG2000", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString JPEG2000 { get; }
		
		[Field ("kUTTypeTIFF", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString TIFF { get; }
		
		[Field ("kUTTypePICT", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString PICT { get; }
		
		[Field ("kUTTypeGIF", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString GIF { get; }
		
		[Field ("kUTTypePNG", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString PNG { get; }
		
		[Field ("kUTTypeQuickTimeImage", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString QuickTimeImage { get; }
		
		[Field ("kUTTypeAppleICNS", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString AppleICNS { get; }
		
		[Field ("kUTTypeBMP", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString BMP { get; }
		
		[Field ("kUTTypeICO", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString ICO { get; }
		
		[Field ("kUTTypeRawImage", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString RawImage { get; }
		
		[Field ("kUTTypeScalableVectorGraphics", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString ScalableVectorGraphics { get; }
		
		[Field ("kUTTypeAudiovisualContent", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString AudiovisualContent { get; }
		
		[Field ("kUTTypeMovie", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Movie { get; }
		
		[Field ("kUTTypeVideo", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Video { get; }
		
		[Field ("kUTTypeAudio", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Audio { get; }
		
		[Field ("kUTTypeQuickTimeMovie", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString QuickTimeMovie { get; }
		
		[Field ("kUTTypeMPEG", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString MPEG { get; }
		
		[Field ("kUTTypeMPEG2Video", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString MPEG2Video { get; }
		
		[Field ("kUTTypeMPEG2TransportStream", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString MPEG2TransportStream { get; }
		
		[Field ("kUTTypeMP3", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString MP3 { get; }
		
		[Field ("kUTTypeMPEG4", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString MPEG4 { get; }
		
		[Field ("kUTTypeMPEG4Audio", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString MPEG4Audio { get; }
		
		[Field ("kUTTypeAppleProtectedMPEG4Audio", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString AppleProtectedMPEG4Audio { get; }
		
		[Field ("kUTTypeAppleProtectedMPEG4Video", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString AppleProtectedMPEG4Video { get; }
		
		[Field ("kUTTypeAVIMovie", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString AVIMovie { get; }
		
		[Field ("kUTTypeAudioInterchangeFileFormat", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString AudioInterchangeFileFormat { get; }
		
		[Field ("kUTTypeWaveformAudio", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString WaveformAudio { get; }
		
		[Field ("kUTTypeMIDIAudio", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString MIDIAudio { get; }
		
		[Field ("kUTTypePlaylist", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString Playlist { get; }
		
		[Field ("kUTTypeM3UPlaylist", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString M3UPlaylist { get; }
		
		[Field ("kUTTypeFolder", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Folder { get; }
		
		[Field ("kUTTypeVolume", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Volume { get; }
		
		[Field ("kUTTypePackage", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Package { get; }
		
		[Field ("kUTTypeBundle", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Bundle { get; }
		
		[Field ("kUTTypePluginBundle", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString PluginBundle { get; }
		
		[Field ("kUTTypeSpotlightImporter", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString SpotlightImporter { get; }
		
		[Field ("kUTTypeQuickLookGenerator", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString QuickLookGenerator { get; }
		
		[Field ("kUTTypeXPCService", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString XPCService { get; }
		
		[Field ("kUTTypeFramework", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Framework { get; }
		
		[Field ("kUTTypeApplication", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString Application { get; }
		
		[Field ("kUTTypeApplicationBundle", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString ApplicationBundle { get; }
		
		[Field ("kUTTypeApplicationFile", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString ApplicationFile { get; }
		
		[Field ("kUTTypeUnixExecutable", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString UnixExecutable { get; }
		
		[Field ("kUTTypeWindowsExecutable", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString WindowsExecutable { get; }
		
		[Field ("kUTTypeJavaClass", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString JavaClass { get; }
		
		[Field ("kUTTypeJavaArchive", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString JavaArchive { get; }
		
		[Field ("kUTTypeSystemPreferencesPane", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString SystemPreferencesPane { get; }
		
		[Field ("kUTTypeGNUZipArchive", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString GNUZipArchive { get; }
		
		[Field ("kUTTypeBzip2Archive", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString Bzip2Archive { get; }
		
		[Field ("kUTTypeZipArchive", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString ZipArchive { get; }
		
		[Field ("kUTTypeSpreadsheet", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString Spreadsheet { get; }
		
		[Field ("kUTTypePresentation", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString Presentation { get; }
		
		[Field ("kUTTypeDatabase", "+CoreServices")]
		[Mac (10,4), iOS(8,0)]
		NSString Database { get; }
		
		[Field ("kUTTypeVCard", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString VCard { get; }
		
		[Field ("kUTTypeToDoItem", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString ToDoItem { get; }
		
		[Field ("kUTTypeCalendarEvent", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString CalendarEvent { get; }
		
		[Field ("kUTTypeEmailMessage", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString EmailMessage { get; }
		
		[Field ("kUTTypeInternetLocation", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString InternetLocation { get; }
		
		[Field ("kUTTypeInkText", "+CoreServices")]
		[Mac (10,4), iOS(3,0)]
		NSString InkText { get; }
		
		[Field ("kUTTypeFont", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString Font { get; }
		
		[Field ("kUTTypeBookmark", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString Bookmark { get; }
		
		[Field ("kUTType3DContent", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString ThreeDContent { get; }
		
		[Field ("kUTTypePKCS12", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString PKCS12 { get; }
		
		[Field ("kUTTypeX509Certificate", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString X509Certificate { get; }
		
		[Field ("kUTTypeElectronicPublication", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
		NSString ElectronicPublication { get; }
		
		[Field ("kUTTypeLog", "+CoreServices")]
		[Mac (10,10), iOS(8,0)]
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
		
#if MONOMAC
		[Field ("kUTTagClassNSPboardType", "+CoreServices")]
		NSString TagClassNSPboardType { get; }
		
		[Field ("kUTTagClassOSType", "+CoreServices")]
		NSString TagClassOSType { get; }
#endif

		[Mac(10,11), iOS (9,0)]
		[Field ("kUTTypeSwiftSource", "+CoreServices")]
		NSString SwiftSource { get; }

// exclude from MonoMac classic
#if (XAMCORE_2_0 || !MONOMAC)
		[NoWatch]
		[iOS (9,0)][Mac(10,11, onlyOn64 : true)]
		[Field ("kUTTypeAlembic", "ModelIO")]
		NSString Alembic { get; }

		[NoWatch]
		[iOS (9,0)][Mac(10,11, onlyOn64 : true)]
		[Field ("kUTType3dObject", "ModelIO")]
		NSString k3dObject { get; }

		[NoWatch]
		[iOS (9,0)][Mac(10,11, onlyOn64 : true)]
		[Field ("kUTTypePolygon", "ModelIO")]
		NSString Polygon { get; }

		[NoWatch]
		[iOS (9,0)][Mac(10,11, onlyOn64 : true)]
		[Field ("kUTTypeStereolithography", "ModelIO")]
		NSString Stereolithography { get; }

		[NoWatch]
		[iOS (10,0)][Mac(10,12, onlyOn64 : true)]
		[TV (10,0)]
		[Field ("kUTTypeUniversalSceneDescription", "ModelIO")]
		NSString UniversalSceneDescription { get; }

		[Watch (2,2)]
		[iOS (9,1)][TV (9,0)]
		[NoMac]
		[Field ("kUTTypeLivePhoto", "+CoreServices")]
		NSString LivePhoto { get; }
#endif
	}
}
