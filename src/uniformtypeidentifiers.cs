using System;
using System.ComponentModel;
using Foundation;
using ObjCRuntime;

namespace UniformTypeIdentifiers {

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UTType : NSCopying, NSSecureCoding {

		[Static]
		[Export ("typeWithIdentifier:")]
		[return: NullAllowed]
		UTType CreateFromIdentifier (string identifier);

		[Static]
		[Export ("typeWithFilenameExtension:")]
		[return: NullAllowed]
		UTType CreateFromExtension (string filenameExtension);

		[Static]
		[Export ("typeWithFilenameExtension:conformingToType:")]
		[return: NullAllowed]
		UTType CreateFromExtension (string filenameExtension, UTType supertype);

		[Static]
		[Export ("typeWithMIMEType:")]
		[return: NullAllowed]
		UTType CreateFromMimeType (string mimeType);

		[Static]
		[Export ("typeWithMIMEType:conformingToType:")]
		[return: NullAllowed]
		UTType CreateFromMimeType (string mimeType, UTType supertype);

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("preferredFilenameExtension")]
		string PreferredFilenameExtension { get; }

		[NullAllowed, Export ("preferredMIMEType")]
		string PreferredMimeType { get; }

		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; }

		[NullAllowed, Export ("version")]
		NSNumber Version { get; }

		[NullAllowed, Export ("referenceURL")]
		NSUrl ReferenceUrl { get; }

		[Export ("dynamic")]
		bool Dynamic { [Bind ("isDynamic")] get; }

		[Export ("declared")]
		bool Declared { [Bind ("isDeclared")] get; }

		[Export ("publicType")]
		bool PublicType { [Bind ("isPublicType")] get; }

		// @interface LocalConstants (UTType)

		[Static]
		[Export ("exportedTypeWithIdentifier:")]
		UTType CreateExportedType (string identifier);

		[Static]
		[Export ("exportedTypeWithIdentifier:conformingToType:")]
		UTType CreateExportedType (string identifier, UTType parentType);

		[Static]
		[Export ("importedTypeWithIdentifier:")]
		UTType CreateImportedType (string identifier);

		[Static]
		[Export ("importedTypeWithIdentifier:conformingToType:")]
		UTType CreateImportedType (string identifier, UTType parentType);

		// @interface UTTagSpecification (UTType)

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("typeWithTag:tagClass:conformingToType:")]
		[return: NullAllowed]
		UTType GetType (string tag, NSString tagClass, [NullAllowed] UTType supertype);

		[Static]
		[Wrap ("GetType (tag, tagClass.GetConstant ()!, supertype)")]
		[return: NullAllowed]
		UTType GetType (string tag, UTTagClass tagClass, [NullAllowed] UTType supertype);

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("typesWithTag:tagClass:conformingToType:")]
		UTType [] GetTypes (string tag, NSString tagClass, [NullAllowed] UTType supertype);

		[Static]
		[Wrap ("GetTypes (tag, tagClass.GetConstant ()!, supertype)")]
		UTType [] GetTypes (string tag, UTTagClass tagClass, [NullAllowed] UTType supertype);

		[Export ("tags")]
		NSDictionary<NSString, NSArray<NSString>> Tags { get; }

		// @interface Conformance (UTType)

		[Export ("conformsToType:")]
		bool ConformsTo (UTType type);

		[Export ("isSupertypeOfType:")]
		bool IsSupertypeOf (UTType type);

		[Export ("isSubtypeOfType:")]
		bool IsSubtypeOf (UTType type);

		[Export ("supertypes")]
		NSSet<UTType> Supertypes { get; }

		// extension methods used in ShazamKit

		[Static]
		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("SHCustomCatalogContentType", ArgumentSemantic.Strong)]
		UTType SHCustomCatalogContentType { get; }

		[Static]
		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("SHSignatureContentType", ArgumentSemantic.Strong)]
		UTType SHSignatureContentType { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	enum UTTagClass {
		[Field ("UTTagClassFilenameExtension")]
		FilenameExtension,

		[Field ("UTTagClassMIMEType")]
		MimeType,
	}

	// split from UTType for clarity between members (selectors) and constants (fields)
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[Static]
	interface UTTypes {

		[Field ("UTTypeItem")]
		UTType Item { get; }

		[Field ("UTTypeContent")]
		UTType Content { get; }

		[Field ("UTTypeCompositeContent")]
		UTType CompositeContent { get; }

		[Field ("UTTypeDiskImage")]
		UTType DiskImage { get; }

		[Field ("UTTypeData")]
		UTType Data { get; }

		[Field ("UTTypeDirectory")]
		UTType Directory { get; }

		[Field ("UTTypeResolvable")]
		UTType Resolvable { get; }

		[Field ("UTTypeSymbolicLink")]
		UTType SymbolicLink { get; }

		[Field ("UTTypeExecutable")]
		UTType Executable { get; }

		[Field ("UTTypeMountPoint")]
		UTType MountPoint { get; }

		[Field ("UTTypeAliasFile")]
		UTType AliasFile { get; }

		[Field ("UTTypeURLBookmarkData")]
		UTType UrlBookmarkData { get; }

		[Field ("UTTypeURL")]
		UTType Url { get; }

		[Field ("UTTypeFileURL")]
		UTType FileUrl { get; }

		[Field ("UTTypeText")]
		UTType Text { get; }

		[Field ("UTTypePlainText")]
		UTType PlainText { get; }

		[Field ("UTTypeUTF8PlainText")]
		UTType Utf8PlainText { get; }

		[Field ("UTTypeUTF16ExternalPlainText")]
		UTType Utf16ExternalPlainText { get; }

		[Field ("UTTypeUTF16PlainText")]
		UTType Utf16PlainText { get; }

		[Field ("UTTypeDelimitedText")]
		UTType DelimitedText { get; }

		[Field ("UTTypeCommaSeparatedText")]
		UTType CommaSeparatedText { get; }

		[Field ("UTTypeTabSeparatedText")]
		UTType TabSeparatedText { get; }

		[Field ("UTTypeUTF8TabSeparatedText")]
		UTType Utf8TabSeparatedText { get; }

		[Field ("UTTypeRTF")]
		UTType Rtf { get; }

		[Field ("UTTypeHTML")]
		UTType Html { get; }

		[Field ("UTTypeXML")]
		UTType Xml { get; }

		[Field ("UTTypeYAML")]
		UTType Yaml { get; }

		[Field ("UTTypeSourceCode")]
		UTType SourceCode { get; }

		[Field ("UTTypeAssemblyLanguageSource")]
		UTType AssemblyLanguageSource { get; }

		[Field ("UTTypeCSource")]
		UTType CSource { get; }

		[Field ("UTTypeObjectiveCSource")]
		UTType ObjectiveCSource { get; }

		[Field ("UTTypeSwiftSource")]
		UTType SwiftSource { get; }

		[Field ("UTTypeCPlusPlusSource")]
		UTType CPlusPlusSource { get; }

		[Field ("UTTypeObjectiveCPlusPlusSource")]
		UTType ObjectiveCPlusPlusSource { get; }

		[Field ("UTTypeCHeader")]
		UTType CHeader { get; }

		[Field ("UTTypeCPlusPlusHeader")]
		UTType CPlusPlusHeader { get; }

		[Field ("UTTypeScript")]
		UTType Script { get; }

		[Field ("UTTypeAppleScript")]
		UTType AppleScript { get; }

		[Field ("UTTypeOSAScript")]
		UTType OsaScript { get; }

		[Field ("UTTypeOSAScriptBundle")]
		UTType OsaScriptBundle { get; }

		[Field ("UTTypeJavaScript")]
		UTType JavaScript { get; }

		[Field ("UTTypeShellScript")]
		UTType ShellScript { get; }

		[Field ("UTTypePerlScript")]
		UTType PerlScript { get; }

		[Field ("UTTypePythonScript")]
		UTType PythonScript { get; }

		[Field ("UTTypeRubyScript")]
		UTType RubyScript { get; }

		[Field ("UTTypePHPScript")]
		UTType PhpScript { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("UTTypeMakefile")]
		UTType Makefile { get; }

		[Field ("UTTypeJSON")]
		UTType Json { get; }

		[Field ("UTTypePropertyList")]
		UTType PropertyList { get; }

		[Field ("UTTypeXMLPropertyList")]
		UTType XmlPropertyList { get; }

		[Field ("UTTypeBinaryPropertyList")]
		UTType BinaryPropertyList { get; }

		[Field ("UTTypePDF")]
		UTType Pdf { get; }

		[Field ("UTTypeRTFD")]
		UTType Rtfd { get; }

		[Field ("UTTypeFlatRTFD")]
		UTType FlatRtfd { get; }

		[Field ("UTTypeWebArchive")]
		UTType WebArchive { get; }

		[Field ("UTTypeImage")]
		UTType Image { get; }

		[Field ("UTTypeJPEG")]
		UTType Jpeg { get; }

		[Field ("UTTypeTIFF")]
		UTType Tiff { get; }

		[Field ("UTTypeGIF")]
		UTType Gif { get; }

		[Field ("UTTypePNG")]
		UTType Png { get; }

		[Field ("UTTypeICNS")]
		UTType Icns { get; }

		[Field ("UTTypeBMP")]
		UTType Bmp { get; }

		[Field ("UTTypeICO")]
		UTType Ico { get; }

		[Field ("UTTypeRAWImage")]
		UTType RawImage { get; }

		[Field ("UTTypeSVG")]
		UTType Svg { get; }

		[Field ("UTTypeLivePhoto")]
		UTType LivePhoto { get; }

		[Field ("UTTypeHEIF")]
		UTType Heif { get; }

		[Field ("UTTypeHEIC")]
		UTType Heic { get; }

		[Field ("UTTypeWebP")]
		UTType WebP { get; }

		[Field ("UTType3DContent")]
		UTType ThreeDContent { get; }

		[Field ("UTTypeUSD")]
		UTType Usd { get; }

		[Field ("UTTypeUSDZ")]
		UTType Usdz { get; }

		[Field ("UTTypeRealityFile")]
		UTType RealityFile { get; }

		[Field ("UTTypeSceneKitScene")]
		UTType SceneKitScene { get; }

		[Field ("UTTypeARReferenceObject")]
		UTType ARReferenceObject { get; }

		[Field ("UTTypeAudiovisualContent")]
		UTType AudiovisualContent { get; }

		[Field ("UTTypeMovie")]
		UTType Movie { get; }

		[Field ("UTTypeVideo")]
		UTType Video { get; }

		[Field ("UTTypeAudio")]
		UTType Audio { get; }

		[Field ("UTTypeQuickTimeMovie")]
		UTType QuickTimeMovie { get; }

		[Field ("UTTypeMPEG")]
		UTType Mpeg { get; }

		[Field ("UTTypeMPEG2Video")]
		UTType Mpeg2Video { get; }

		[Field ("UTTypeMPEG2TransportStream")]
		UTType Mpeg2TransportStream { get; }

		[Field ("UTTypeMP3")]
		UTType MP3 { get; }

		[Field ("UTTypeMPEG4Movie")]
		UTType Mpeg4Movie { get; }

		[Field ("UTTypeMPEG4Audio")]
		UTType Mpeg4Audio { get; }

		[Field ("UTTypeAppleProtectedMPEG4Audio")]
		UTType AppleProtectedMpeg4Audio { get; }

		[Field ("UTTypeAppleProtectedMPEG4Video")]
		UTType AppleProtectedMpeg4Video { get; }

		[Field ("UTTypeAVI")]
		UTType Avi { get; }

		[Field ("UTTypeAIFF")]
		UTType Aiff { get; }

		[Field ("UTTypeWAV")]
		UTType Wav { get; }

		[Field ("UTTypeMIDI")]
		UTType Midi { get; }

		[Field ("UTTypePlaylist")]
		UTType Playlist { get; }

		[Field ("UTTypeM3UPlaylist")]
		UTType M3uPlaylist { get; }

		[Field ("UTTypeFolder")]
		UTType Folder { get; }

		[Field ("UTTypeVolume")]
		UTType Volume { get; }

		[Field ("UTTypePackage")]
		UTType Package { get; }

		[Field ("UTTypeBundle")]
		UTType Bundle { get; }

		[Field ("UTTypePluginBundle")]
		UTType PluginBundle { get; }

		[Field ("UTTypeSpotlightImporter")]
		UTType SpotlightImporter { get; }

		[Field ("UTTypeQuickLookGenerator")]
		UTType QuickLookGenerator { get; }

		[Field ("UTTypeXPCService")]
		UTType XpcService { get; }

		[Field ("UTTypeFramework")]
		UTType Framework { get; }

		[Field ("UTTypeApplication")]
		UTType Application { get; }

		[Field ("UTTypeApplicationBundle")]
		UTType ApplicationBundle { get; }

		[Field ("UTTypeApplicationExtension")]
		UTType ApplicationExtension { get; }

		[Field ("UTTypeUnixExecutable")]
		UTType UnixExecutable { get; }

		[Field ("UTTypeEXE")]
		UTType Exe { get; }

		[Field ("UTTypeSystemPreferencesPane")]
		UTType SystemPreferencesPane { get; }

		[Field ("UTTypeArchive")]
		UTType Archive { get; }

		[Field ("UTTypeGZIP")]
		UTType Gzip { get; }

		[Field ("UTTypeBZ2")]
		UTType BZ2 { get; }

		[Field ("UTTypeZIP")]
		UTType Zip { get; }

		[Field ("UTTypeAppleArchive")]
		UTType AppleArchive { get; }

		[Field ("UTTypeSpreadsheet")]
		UTType Spreadsheet { get; }

		[Field ("UTTypePresentation")]
		UTType Presentation { get; }

		[Field ("UTTypeDatabase")]
		UTType Database { get; }

		[Field ("UTTypeMessage")]
		UTType Message { get; }

		[Field ("UTTypeContact")]
		UTType Contact { get; }

		[Field ("UTTypeVCard")]
		UTType VCard { get; }

		[Field ("UTTypeToDoItem")]
		UTType ToDoItem { get; }

		[Field ("UTTypeCalendarEvent")]
		UTType CalendarEvent { get; }

		[Field ("UTTypeEmailMessage")]
		UTType EmailMessage { get; }

		[Field ("UTTypeInternetLocation")]
		UTType InternetLocation { get; }

		[Field ("UTTypeInternetShortcut")]
		UTType Shortcut { get; }

		[Field ("UTTypeFont")]
		UTType Font { get; }

		[Field ("UTTypeBookmark")]
		UTType Bookmark { get; }

		[Field ("UTTypePKCS12")]
		UTType Pkcs12 { get; }

		[Field ("UTTypeX509Certificate")]
		UTType X509Certificate { get; }

		[Field ("UTTypeEPUB")]
		UTType Epub { get; }

		[Field ("UTTypeLog")]
		UTType Log { get; }

		[Watch (10, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("UTTypeAHAP")]
		UTType Ahap { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[Category]
	[BaseType (typeof (NSString))]
	interface NSString_UTAdditions {

		[Export ("stringByAppendingPathComponent:conformingToType:")]
		NSString AppendPathComponent (string partialName, UTType contentType);

		[Export ("stringByAppendingPathExtensionForType:")]
		NSString AppendPathExtension (UTType contentType);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[Category]
	[BaseType (typeof (NSUrl))]
	interface NSUrl_UTAdditions {

		[Export ("URLByAppendingPathComponent:conformingToType:")]
		NSUrl AppendPathComponent (string partialName, UTType contentType);

		[Export ("URLByAppendingPathExtensionForType:")]
		NSUrl AppendPathExtension (UTType contentType);
	}
}
