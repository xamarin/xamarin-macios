//
// Copyright 2015 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace CoreFoundation {

	public class CFBundle : INativeObject, IDisposable {

		public enum PackageType {
			Application,
			Framework,
			Bundle
		}

		// from machine.h
		// #define CPU_ARCH_ABI64       0x01000000
		// #define CPU_TYPE_X86        ((cpu_type_t) 7)
		// #define CPU_TYPE_X86_64     (CPU_TYPE_X86 | CPU_ARCH_ABI64)
		// #define CPU_TYPE_ARM        ((cpu_type_t) 12)
		// #define CPU_TYPE_ARM64      (CPU_TYPE_ARM | CPU_ARCH_ABI64)
		// #define CPU_TYPE_POWERPC    ((cpu_type_t) 18)
		// #define CPU_TYPE_POWERPC64  (CPU_TYPE_POWERPC | CPU_ARCH_ABI64)
		public enum Architecture {
			I386     = 0x00000007,
			X86_64   = 0x01000007,
			ARM      = 0x00000012,
			ARM64    = 0x01000012,
			PPC      = 0x00000018,
			PPC64    = 0x01000018,
		}

		public struct PackageInfo {
			public PackageInfo (CFBundle.PackageType type, string creator)
			{
				this.Type = type;
				this.Creator = creator;
			}

			public PackageType Type { get; private set; }
			public string Creator { get; private set; }
		}

		IntPtr handle;

		public IntPtr Handle {
			get { return handle; }
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		~CFBundle ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize(this);
		}

		internal CFBundle (IntPtr handle) : this (handle, false)
		{
		}

		internal CFBundle (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw new ArgumentNullException ("handle");
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (this.handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFBundleRef */ IntPtr CFBundleCreate ( /* CFAllocatorRef can be null */ IntPtr allocator, /* CFUrlRef */ IntPtr bundleURL);

		public CFBundle (NSUrl bundleUrl) 
		{
			if (bundleUrl == null)
				throw new ArgumentNullException ("bundleUrl");
				
			this.handle = CFBundleCreate (IntPtr.Zero, bundleUrl.Handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArrayRef */ IntPtr CFBundleCreateBundlesFromDirectory (/* CFAllocatorRef can be null */ IntPtr allocator, /* CFUrlRef */ IntPtr directoryURL, /* CFStringRef */ IntPtr bundleType);

		public static CFBundle[] GetBundlesFromDirectory (NSUrl directoryUrl, string bundleType)
		{
			if (directoryUrl == null) // NSUrl cannot be "" by definition
				throw new ArgumentNullException ("directoryUrl");
			if (String.IsNullOrEmpty (bundleType))
				throw new ArgumentException ("bundleType");
			using (var bundleTypeCFSting = new CFString (bundleType))
			using (var cfBundles = new CFArray (CFBundleCreateBundlesFromDirectory (IntPtr.Zero, directoryUrl.Handle, bundleTypeCFSting.Handle), true)) {
				var managedBundles = new CFBundle [cfBundles.Count];
				for (int index = 0; index < cfBundles.Count; index++) {
					// follow the create rules, therefore we do have ownership of each of the cfbundles
					managedBundles [index] = new CFBundle (cfBundles.GetValue (index), true);
				}
				return managedBundles;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFBundleGetAllBundles ();
		
		public static CFBundle[] GetAll ()
		{
			using (var cfBundles = new CFArray (CFBundleGetAllBundles ())) {
				var managedBundles = new CFBundle [cfBundles.Count];
				for (int index = 0; index < cfBundles.Count; index++) {
					// follow the get rule, we do not own the object
					managedBundles [index] = new CFBundle (cfBundles.GetValue (index), false);
				}
				return managedBundles;
			}
		}


		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFBundleGetBundleWithIdentifier (/* CFStringRef */ IntPtr bundleID);
		
		public static CFBundle Get (string bundleID)
		{
			if (String.IsNullOrEmpty (bundleID))
				throw new ArgumentException ("bundleID");
			using (var cfBundleId = new CFString (bundleID)) {
				var cfBundle = CFBundleGetBundleWithIdentifier (cfBundleId.Handle);
				if (cfBundle == IntPtr.Zero)
					return null;
				// follow the Get rule and retain the obj
				return new CFBundle (cfBundle, false);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFBundleGetMainBundle ();

		public static CFBundle GetMain ()
		{
			var cfBundle = CFBundleGetMainBundle ();
			if (cfBundle == IntPtr.Zero)
				return null;
			// follow the get rule and retain
			return new CFBundle (CFBundleGetMainBundle (), false);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static bool CFBundleIsExecutableLoaded (IntPtr bundle);
		
		public bool HasLoadedExecutable {
			get { return CFBundleIsExecutableLoaded (handle); }
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static bool CFBundlePreflightExecutable (IntPtr bundle, out IntPtr error);
		
		public bool PreflightExecutable (out NSError error)
		{
			IntPtr errorPtr = IntPtr.Zero;
			// follow the create rule, no need to retain
			var loaded = CFBundlePreflightExecutable (handle, out errorPtr);
			error = Runtime.GetNSObject<NSError> (errorPtr);
			return loaded;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static bool CFBundleLoadExecutableAndReturnError (IntPtr bundle, out IntPtr error);
		
		public bool LoadExecutable (out NSError error)
		{
			IntPtr errorPtr = IntPtr.Zero;
			// follows the create rule, no need to retain
			var loaded = CFBundleLoadExecutableAndReturnError (handle, out errorPtr);
			error = Runtime.GetNSObject<NSError> (errorPtr);
			return loaded;
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFBundleUnloadExecutable (IntPtr bundle);
		
		public void UnloadExecutable ()
		{
			CFBundleUnloadExecutable (handle);
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyAuxiliaryExecutableURL (IntPtr bundle, /* CFStringRef */ IntPtr executableName);
		
		public NSUrl GetAuxiliaryExecutableUrl (string executableName)
		{
			if (String.IsNullOrEmpty (executableName))
				throw new ArgumentException ("executableName");
			using (var cfExecutableName = new CFString (executableName)) {
				// follows the create rule no need to retain
				var urlHandle = CFBundleCopyAuxiliaryExecutableURL (handle, cfExecutableName.Handle);
				if (urlHandle == IntPtr.Zero)
					return null;
				return Runtime.GetNSObject<NSUrl> (urlHandle, true);
			}
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyBuiltInPlugInsURL (IntPtr bundle);
		
		public NSUrl BuiltInPlugInsUrl {
			get {
				return Runtime.GetNSObject<NSUrl> (CFBundleCopyBuiltInPlugInsURL (handle), true);
			}
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyExecutableURL (IntPtr bundle);
		
		public NSUrl ExecutableUrl {
			get {
				return Runtime.GetNSObject<NSUrl> (CFBundleCopyExecutableURL (handle), true);
			}
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyPrivateFrameworksURL (IntPtr bundle);
		
		public NSUrl PrivateFrameworksUrl {
			get {
				return Runtime.GetNSObject<NSUrl> (CFBundleCopyPrivateFrameworksURL (handle), true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyResourcesDirectoryURL (IntPtr bundle);
		
		public NSUrl ResourcesDirectoryUrl {
			get {
				return Runtime.GetNSObject<NSUrl> (CFBundleCopyResourcesDirectoryURL (handle), true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopySharedFrameworksURL (IntPtr bundle);
		
		public NSUrl SharedFrameworksUrl {
			get {
				return Runtime.GetNSObject<NSUrl> (CFBundleCopySharedFrameworksURL (handle), true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopySharedSupportURL (IntPtr bundle);

		public NSUrl SharedSupportUrl {
			get {
				return Runtime.GetNSObject<NSUrl> (CFBundleCopySharedSupportURL (handle), true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopySupportFilesDirectoryURL (IntPtr bundle);
		
		public NSUrl SupportFilesDirectoryUrl {
			get {
				return Runtime.GetNSObject<NSUrl> (CFBundleCopySupportFilesDirectoryURL (handle), true);
			}
		}

		// the parameters do not take CFString because we want to be able to pass null (IntPtr.Zero) to the resource type and subdir names
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyResourceURL (IntPtr bundle, /* CFStringRef */ IntPtr resourceName, /* CFString */ IntPtr resourceType, /* CFString */ IntPtr subDirName);
		
		public NSUrl GetResourceUrl (string resourceName, string resourceType, string subDirName)
		{
			if (String.IsNullOrEmpty (resourceName))
				throw new ArgumentException ("resourceName");

			if (String.IsNullOrEmpty (resourceType))
				throw new ArgumentException ("resourceType");

			using (CFString cfResourceName = new CFString (resourceName),
					cfResourceType = new CFString (resourceType),
					cfDirName = (subDirName == null)? new CFString ("") : new CFString (subDirName)) {
				// follows the create rules and therefore we do not need to retain
				var urlHandle = CFBundleCopyResourceURL (handle, cfResourceName.Handle, cfResourceType.Handle,
								   	 String.IsNullOrEmpty (subDirName) ? IntPtr.Zero : cfDirName.Handle);
				return Runtime.GetNSObject<NSUrl> (urlHandle, true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyResourceURLInDirectory (/* CFUrlRef */ IntPtr bundleURL, /* CFStringRef */ IntPtr resourceName, /* CFStringRef */ IntPtr resourceType, /* CFStringRef */ IntPtr subDirName);
		
		public static NSUrl GetResourceUrl (NSUrl bundleUrl, string resourceName, string resourceType, string subDirName)
		{
			if (bundleUrl == null)
				throw new ArgumentNullException ("bundleUrl");

			if (String.IsNullOrEmpty (resourceName))
				throw new ArgumentException ("resourceName");

			if (String.IsNullOrEmpty (resourceType))
				throw new ArgumentException ("resourceType");

			// follows the create rules and therefore we do not need to retain
			using (CFString cfResourceName = new CFString (resourceName),
					cfResourceType = new CFString (resourceType),
					cfSubDirName = new CFString (subDirName ?? string.Empty)) {
				var urlHandle = CFBundleCopyResourceURLInDirectory (bundleUrl.Handle, cfResourceName.Handle, cfResourceType.Handle,
										      String.IsNullOrEmpty (subDirName) ? IntPtr.Zero : cfSubDirName.Handle);
				return Runtime.GetNSObject<NSUrl> (urlHandle, true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArray */ IntPtr CFBundleCopyResourceURLsOfType (IntPtr bundle, /* CFStringRef */ IntPtr resourceType, /* CFStringRef */ IntPtr subDirName);
		
		public NSUrl[] GetResourceUrls (string resourceType, string subDirName)
		{
			if (String.IsNullOrEmpty (resourceType))
				throw new ArgumentException ("resourceName");
			
			using (CFString cfResourceType = new CFString (resourceType),
					cfSubDir = new CFString (subDirName ?? string.Empty))
			using (var cfArray = new CFArray (CFBundleCopyResourceURLsOfType (handle, cfResourceType.Handle,
										     String.IsNullOrEmpty (subDirName) ? IntPtr.Zero : cfSubDir.Handle), true)) {
				var result = new NSUrl [cfArray.Count];
				for (int index = 0; index < cfArray.Count; index++) {
					result [index] = Runtime.GetNSObject<NSUrl> (cfArray.GetValue (index), true);
				}
				return result;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArray */ IntPtr CFBundleCopyResourceURLsOfTypeInDirectory (/* CFUrlRef */ IntPtr bundleURL, /* CFStringRef */ IntPtr resourceType, /* CFStringRef */ IntPtr subDirName);

		public static NSUrl[] GetResourceUrls (NSUrl bundleUrl, string resourceType, string subDirName)
		{
			if (bundleUrl == null)
				throw new ArgumentNullException ("bundleUrl");

			if (String.IsNullOrEmpty (resourceType))
				throw new ArgumentException ("resourceType");
			
			using (CFString cfResourceType = new CFString (resourceType),
			                cfSubDir = new CFString (subDirName ?? string.Empty))
			using (var cfArray = new CFArray (CFBundleCopyResourceURLsOfTypeInDirectory (bundleUrl.Handle, cfResourceType.Handle,
					                                                        String.IsNullOrEmpty (subDirName) ? IntPtr.Zero : cfSubDir.Handle), true)) {
				var result = new NSUrl [cfArray.Count];
				for (int index = 0; index < cfArray.Count; index++) {
					result [index] = Runtime.GetNSObject<NSUrl> (cfArray.GetValue (index), true);
				}
				return result;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyResourceURLForLocalization (IntPtr bundle, /* CFStringRef */ IntPtr resourceName, /* CFStringRef */ IntPtr resourceType, /* CFStringRef */ IntPtr subDirName,
		                                                                            /* CFStringRef */ IntPtr localizationName);
		
		public NSUrl GetResourceUrl (string resourceName, string resourceType, string subDirName, string localizationName)
		{
			if (String.IsNullOrEmpty (resourceName))
				throw new ArgumentException ("resourceName");

			if (String.IsNullOrEmpty (resourceType))
				throw new ArgumentException ("resourceType");
			
			if (String.IsNullOrEmpty (localizationName))
				throw new ArgumentException ("localizationName");

			using (CFString cfResourceName = new CFString (resourceName),
			                cfResourceType = new CFString (resourceType),
					cfSubDir = new CFString (subDirName ?? string.Empty),
					cfLocalization = new CFString (localizationName)) {
				var urlHandle = CFBundleCopyResourceURLForLocalization (handle, cfResourceName.Handle, cfResourceType.Handle,
											String.IsNullOrEmpty (subDirName) ? IntPtr.Zero : cfSubDir.Handle, cfLocalization.Handle);
				return Runtime.GetNSObject<NSUrl> (urlHandle, true);
			}
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArray */ IntPtr CFBundleCopyResourceURLsOfTypeForLocalization (IntPtr bundle, /* CFStringRef */ IntPtr resourceType, /* CFStringRef */ IntPtr subDirName,
		                                                                                  /* CFStringRef */ IntPtr localizationName);
		
		public NSUrl[] GetResourceUrls (string resourceType, string subDirName, string localizationName)
		{
			if (String.IsNullOrEmpty (resourceType))
				throw new ArgumentException ("resourceType");
			
			if (String.IsNullOrEmpty (localizationName))
				throw new ArgumentException ("localizationName");
			
			using (CFString cfType = new CFString (resourceType),
			                cfDirName = new CFString (subDirName ?? string.Empty),
					cfLocalization = new CFString (localizationName))
			using (var cfArray = new CFArray (CFBundleCopyResourceURLsOfTypeForLocalization (handle, cfType.Handle,
													 String.IsNullOrEmpty (subDirName) ? IntPtr.Zero : cfDirName.Handle,
													 cfLocalization.Handle), true)) {
				var urls = new NSUrl [cfArray.Count];
				for (int index = 0; index < cfArray.Count; index++) {
					urls [index] = Runtime.GetNSObject<NSUrl> (cfArray.GetValue (index), true);
				}
				return urls;
			}
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFString */ IntPtr CFBundleCopyLocalizedString (IntPtr bundle, /* CFStringRef */ IntPtr key, /* CFStringRef */ IntPtr value, /* CFStringRef */ IntPtr tableName);
		
		public string GetLocalizedString (string key, string defaultValue, string tableName)
		{
			if (String.IsNullOrEmpty (key))
				throw new ArgumentException ("key");

			if (String.IsNullOrEmpty (tableName))
				throw new ArgumentException ("tableName");

			// we do allow null and simply use an empty string to avoid the extra check
			if (defaultValue == null)
				defaultValue = string.Empty;

			using (CFString cfKey = new CFString (key),
					cfValue = new CFString (defaultValue),
					cfTable = new CFString (tableName)) {
				return CFString.FetchString (CFBundleCopyLocalizedString (handle, cfKey.Handle, cfValue.Handle, cfTable.Handle), releaseHandle: true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArray */ IntPtr CFBundleCopyLocalizationsForPreferences (/* CFArrayRef */ IntPtr locArray, /* CFArrayRef */ IntPtr prefArray);

		public static string[] GetLocalizationsForPreferences (string[] locArray, string[] prefArray)
		{
			if (locArray == null)
				throw new ArgumentNullException ("locArray");
			if (prefArray == null)
				throw new ArgumentNullException ("prefArray");

			var cfLocal = new CFString [locArray.Length];
			for (int index = 0; index < locArray.Length; index++) {
				cfLocal [index] = new CFString (locArray [index]);
			}
			
			var cfPref = new CFString [prefArray.Length];
			for (int index = 0; index < prefArray.Length; index++) {
				cfPref [index] = new CFString (prefArray [index]);
			}
			
			using (CFArray cfLocalArray = CFArray.FromNativeObjects (cfLocal),
				       cfPrefArray = CFArray.FromNativeObjects (cfPref))
			using (var cfArray = new CFArray (CFBundleCopyLocalizationsForPreferences (cfLocalArray.Handle, cfPrefArray.Handle), true)) {
				var cultureInfo = new string [cfArray.Count];
				for (int index = 0; index < cfArray.Count; index ++) {
					cultureInfo [index] = CFString.FetchString (cfArray.GetValue (index));
				}
				return cultureInfo;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArray */ IntPtr CFBundleCopyLocalizationsForURL (/* CFUrlRef */ IntPtr url);
		
		public static string[] GetLocalizations (NSUrl bundle)
		{
			if (bundle == null)
				throw new ArgumentNullException ("bundle");
			using (var cfArray = new CFArray (CFBundleCopyLocalizationsForURL (bundle.Handle), true)) {
				var cultureInfo = new string [cfArray.Count];
				for (int index = 0; index < cfArray.Count; index++) {
					cultureInfo [index] = CFString.FetchString (cfArray.GetValue (index));
				}
				return cultureInfo;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArray */ IntPtr CFBundleCopyPreferredLocalizationsFromArray (/* CFArrayRef */ IntPtr locArray);
		
		public static string[] GetPreferredLocalizations (string[] locArray)
		{
			if (locArray == null)
				throw new ArgumentNullException ("locArray");

			var cfString = new CFString [locArray.Length];
			for (int index = 0; index < locArray.Length; index++) {
				cfString [index] = new CFString (locArray [index]);
			}
			using (var cfLocArray = CFArray.FromNativeObjects (cfString))
			using (var cfArray = new CFArray (CFBundleCopyPreferredLocalizationsFromArray (cfLocArray.Handle), true)) {
				var cultureInfo = new string [cfArray.Count];
				for (int index = 0; index < cfArray.Count; index++) {
					cultureInfo [index] = CFString.FetchString (cfArray.GetValue (index));
				}
				return cultureInfo;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFUrlRef */ IntPtr CFBundleCopyBundleURL (IntPtr bundle);
		
		public NSUrl Url {
			get { 
				return Runtime.GetNSObject<NSUrl> (CFBundleCopyBundleURL (handle), true);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFString */ IntPtr CFBundleGetDevelopmentRegion (IntPtr bundle );
		
		public string DevelopmentRegion {
			get { return CFString.FetchString (CFBundleGetDevelopmentRegion (handle)); }
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFString */ IntPtr CFBundleGetIdentifier (IntPtr bundle);
		
		public string Identifier {
			get { return CFString.FetchString (CFBundleGetIdentifier (handle)); }
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFDictionary */ IntPtr CFBundleGetInfoDictionary (IntPtr bundle ); 
		
		public NSDictionary InfoDictionary {
			get {
				// follows the Get rule, we need to retain
				return Runtime.GetNSObject<NSDictionary> (CFBundleGetInfoDictionary (handle));
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* NSDictionary */ IntPtr CFBundleGetLocalInfoDictionary (IntPtr bundle );	
		
		public NSDictionary LocalInfoDictionary {
			get {
				// follows the Get rule, we need to retain
				return Runtime.GetNSObject<NSDictionary> (CFBundleGetLocalInfoDictionary (handle));
			}
		}

		// We do not bind CFDictionaryRef CFBundleCopyInfoDictionaryInDirectory because we will use CFBundleCopyInfoDictionaryForURL. As per the apple documentation
		// For a directory URL, this is equivalent to CFBundleCopyInfoDictionaryInDirectory. For a plain file URL representing an unbundled application, this function
		// will attempt to read an information dictionary either from the (__TEXT, __info_plist) section of the file (for a Mach-O file) or from a plst resource. 

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* NSDictionary */ IntPtr CFBundleCopyInfoDictionaryForURL (/* CFUrlRef */ IntPtr url);
		
		public static NSDictionary GetInfoDictionary (NSUrl url)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			// follow the create rule, no need to retain
			return Runtime.GetNSObject<NSDictionary> (CFBundleCopyInfoDictionaryForURL (url.Handle));
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static void CFBundleGetPackageInfo (IntPtr bundle, out uint packageType, out uint packageCreator);
		
		static string FourCCToString (uint code)
		{
			return new string (new char [] { 
				(char) (byte) (code >> 24), 
				(char) (byte) (code >> 16), 
				(char) (byte) (code >> 8), 
				(char) (byte) code});
		}

		public PackageInfo Info {
			get {
				uint type = 0;
				uint creator = 0;
				
				CFBundleGetPackageInfo (handle, out type, out creator);
				var creatorStr = FourCCToString (creator);
				switch (type) {
				case 1095782476: // ""APPL
					return new PackageInfo (CFBundle.PackageType.Application, creatorStr);
				case 1179473739: // "FMWK"
					return new PackageInfo (CFBundle.PackageType.Framework, creatorStr);
				case 1112425548: // "BNDL" so that we know we did not forget about this value
				default:
					return new PackageInfo (CFBundle.PackageType.Bundle, creatorStr);
				}
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArray */ IntPtr CFBundleCopyExecutableArchitectures (IntPtr bundle);

		public CFBundle.Architecture[] Architectures {
			get {
				using (var cfArray = new CFArray(CFBundleCopyExecutableArchitectures (handle), true)) {
					var archs = new CFBundle.Architecture [cfArray.Count];
					for (int index = 0; index < cfArray.Count; index++) {
						int value = 0;
						if (CFDictionary.CFNumberGetValue (cfArray.GetValue (index), /* kCFNumberSInt32Type */ 3, out value)) {
							archs [index] = (CFBundle.Architecture) value;
						} 
					}
					return archs;
				}
			}
		}
	}
}
