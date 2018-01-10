// 
// CTFontNameKey.cs: CTFont name specifier constants
//
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2009 Novell, Inc
// Copyright 2014 Xamarin Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreText {

	// Utility enum for constant strings in ObjC
	public enum CTFontNameKey {
		Copyright,
		Family,
		SubFamily,
		Style,
		Unique,
		Full,
		Version,
		PostScript,
		Trademark,
		Manufacturer,
		Designer,
		Description,
		VendorUrl,
		DesignerUrl,
		License,
		LicenseUrl,
		SampleText,
		PostscriptCid,
	}

	static class CTFontNameKeyId {
		
		public static readonly NSString Copyright;
		public static readonly NSString Family;
		public static readonly NSString SubFamily;
		public static readonly NSString Style;
		public static readonly NSString Unique;
		public static readonly NSString Full;
		public static readonly NSString Version;
		public static readonly NSString PostScript;
		public static readonly NSString Trademark;
		public static readonly NSString Manufacturer;
		public static readonly NSString Designer;
		public static readonly NSString Description;
		public static readonly NSString VendorUrl;
		public static readonly NSString DesignerUrl;
		public static readonly NSString License;
		public static readonly NSString LicenseUrl;
		public static readonly NSString SampleText;
		public static readonly NSString PostscriptCid;

		static CTFontNameKeyId ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreTextLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				Copyright     = Dlfcn.GetStringConstant (handle, "kCTFontCopyrightNameKey");
				Family        = Dlfcn.GetStringConstant (handle, "kCTFontFamilyNameKey");
				SubFamily     = Dlfcn.GetStringConstant (handle, "kCTFontSubFamilyNameKey");
				Style         = Dlfcn.GetStringConstant (handle, "kCTFontStyleNameKey");
				Unique        = Dlfcn.GetStringConstant (handle, "kCTFontUniqueNameKey");
				Full          = Dlfcn.GetStringConstant (handle, "kCTFontFullNameKey");
				Version       = Dlfcn.GetStringConstant (handle, "kCTFontVersionNameKey");
				PostScript    = Dlfcn.GetStringConstant (handle, "kCTFontPostScriptNameKey");
				Trademark     = Dlfcn.GetStringConstant (handle, "kCTFontTrademarkNameKey");
				Manufacturer  = Dlfcn.GetStringConstant (handle, "kCTFontManufacturerNameKey");
				Designer      = Dlfcn.GetStringConstant (handle, "kCTFontDesignerNameKey");
				Description   = Dlfcn.GetStringConstant (handle, "kCTFontDescriptionNameKey");
				VendorUrl     = Dlfcn.GetStringConstant (handle, "kCTFontVendorURLNameKey");
				DesignerUrl   = Dlfcn.GetStringConstant (handle, "kCTFontDesignerURLNameKey");
				License       = Dlfcn.GetStringConstant (handle, "kCTFontLicenseNameKey");
				LicenseUrl    = Dlfcn.GetStringConstant (handle, "kCTFontLicenseURLNameKey");
				SampleText    = Dlfcn.GetStringConstant (handle, "kCTFontSampleTextNameKey");
				PostscriptCid = Dlfcn.GetStringConstant (handle, "kCTFontPostScriptCIDNameKey");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}

		public static NSString ToId (CTFontNameKey key)
		{
			switch (key) {
				case CTFontNameKey.Copyright:     return Copyright;
				case CTFontNameKey.Family:        return Family;
				case CTFontNameKey.SubFamily:     return SubFamily;
				case CTFontNameKey.Style:         return Style;
				case CTFontNameKey.Unique:        return Unique;
				case CTFontNameKey.Full:          return Full;
				case CTFontNameKey.Version:       return Version;
				case CTFontNameKey.PostScript:    return PostScript;
				case CTFontNameKey.Trademark:     return Trademark;
				case CTFontNameKey.Manufacturer:  return Manufacturer;
				case CTFontNameKey.Designer:      return Designer;
				case CTFontNameKey.Description:   return Description;
				case CTFontNameKey.VendorUrl:     return VendorUrl;
				case CTFontNameKey.DesignerUrl:   return DesignerUrl;
				case CTFontNameKey.License:       return License;
				case CTFontNameKey.LicenseUrl:    return LicenseUrl;
				case CTFontNameKey.SampleText:    return SampleText;
				case CTFontNameKey.PostscriptCid: return PostscriptCid;
			}
			throw new NotSupportedException ("Invalid CTFontNameKey value: " + key);
		}

		public static CTFontNameKey ToFontNameKey (NSString key)
		{
			if (key == Copyright)    	return CTFontNameKey.Copyright;
			if (key == Family)        return CTFontNameKey.Family;
			if (key == SubFamily)     return CTFontNameKey.SubFamily;
			if (key == Style)         return CTFontNameKey.Style;
			if (key == Unique)        return CTFontNameKey.Unique;
			if (key == Full)          return CTFontNameKey.Full;
			if (key == Version)       return CTFontNameKey.Version;
			if (key == PostScript)    return CTFontNameKey.PostScript;
			if (key == Trademark)     return CTFontNameKey.Trademark;
			if (key == Manufacturer)  return CTFontNameKey.Manufacturer;
			if (key == Designer)      return CTFontNameKey.Designer;
			if (key == Description)   return CTFontNameKey.Description;
			if (key == VendorUrl)     return CTFontNameKey.VendorUrl;
			if (key == DesignerUrl)   return CTFontNameKey.DesignerUrl;
			if (key == License)       return CTFontNameKey.License;
			if (key == LicenseUrl)    return CTFontNameKey.LicenseUrl;
			if (key == SampleText)    return CTFontNameKey.SampleText;
			if (key == PostscriptCid) return CTFontNameKey.PostscriptCid;
			throw new NotSupportedException ("Invalid CTFontNameKeyId value: " + key);
		}
	}
}

