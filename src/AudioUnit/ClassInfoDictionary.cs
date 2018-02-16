// 
// AudioUnitPreset.cs: Audio unit preset (ClassInfo) dictionary
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2013, Xamarin Inc.
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

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using CoreImage;

namespace AudioUnit
{
	public class ClassInfoDictionary : DictionaryContainer
	{
		const string VersionKey  		= "version";
		const string TypeKey 			= "type";
		const string SubtypeKey 		= "subtype";
		const string ManufacturerKey	= "manufacturer";
		const string DataKey			= "data";
		const string NameKey			= "name";
		const string RenderQualityKey	= "render-quality";
		const string CPULoadKey			= "cpu-load";
		const string ElementNameKey		= "element-name";
		const string ExternalFileRefs	= "file-references";

		public ClassInfoDictionary ()
			: base (new NSMutableDictionary ())
		{
		}

		public ClassInfoDictionary (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public AudioComponentManufacturerType? Manufacturer {
			get {
				using (var key = new NSString (ManufacturerKey))
					return (AudioComponentManufacturerType?) GetInt32Value (key);
			}
		}

		public string Name {
			get {
				return GetStringValue (NameKey);					
			}
		}

		public AudioComponentType? Type {
			get {
				using (var key = new NSString (TypeKey))
					return (AudioComponentType?) GetInt32Value (key);
			}
		}
	}
}
