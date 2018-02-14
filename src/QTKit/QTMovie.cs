//
// Copyright 2010, Novell, Inc.
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
using AppKit;

namespace QTKit {

	// Untyped and anonymous enum in ObjC
	public enum QTCodecQuality {
		Lossless = 0x00000400,
		Max = 0x000003FF,
		Min = 0x00000000,
		Low = 0x00000100,
		Normal = 0x00000200,
		High = 0x00000300
	}
	
	public class QTImageAttributes {
		public QTImageAttributes ()
		{
		}
		
		public string CodecType { get; set; }
		public QTCodecQuality? Quality { get; set; }
		public int? TimeScale { get; set; }

		public NSDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();
			if (CodecType != null)
				dict.SetObject (new NSString (CodecType), QTMovie.ImageCodecType);
			if (Quality.HasValue)
				dict.SetObject (NSNumber.FromInt32 ((int) Quality.Value), QTMovie.ImageCodecQuality);
			if (TimeScale.HasValue)
				dict.SetObject (NSNumber.FromInt32 (TimeScale.Value), QTTrack.TimeScaleAttribute);
			return dict;
		}
	}

	public class QTMovieSaveOptions {
		public QTMovieSaveOptions ()
		{
		}

		public bool Flatten { get; set; }
		public QTFileType? ExportType { get; set; }
		public NSData ExportSettings { get; set; }
		public int? ManufacturerCode { get; set; }

		public NSDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();
			if (Flatten)
				dict.SetObject (NSNumber.FromInt32 (1), QTMovie.KeyFlatten);
			if (ExportType.HasValue){
				dict.SetObject (NSNumber.FromInt32 (1), QTMovie.KeyExport);
				dict.SetObject (NSNumber.FromInt32 ((int) ExportType.Value), QTMovie.KeyExportType);
			}
			if (ExportSettings != null)
				dict.SetObject (ExportSettings, QTMovie.KeyExportSettings);
			if (ManufacturerCode.HasValue)
				dict.SetObject (NSNumber.FromInt32 ((int) ManufacturerCode.Value), QTMovie.KeyExportManufacturer);

			return dict;
		}
	}
	
	public partial class QTTrack {
		public void AddImage (NSImage image, QTTime forDuration, QTImageAttributes attributes)
		{
			if (attributes == null)
				throw new ArgumentNullException ("attributes");
			AddImage (image, forDuration, attributes.ToDictionary ());
		}
	}
	
	public partial class QTMovie {
		public bool SaveTo (string fileName, QTMovieSaveOptions options, out NSError error)
		{
			return SaveTo (fileName, options == null ? null : options.ToDictionary (), out error);
		}

		public bool SaveTo (string fileName, QTMovieSaveOptions options)
		{
			return SaveTo (fileName, options == null ? null : options.ToDictionary ());
		}
		
		public void AddImage (NSImage image, QTTime forDuration, QTImageAttributes attributes)
		{
			if (attributes == null)
				throw new ArgumentNullException ("attributes");
			AddImage (image, forDuration, attributes.ToDictionary ());
		}

		public QTTrack[] TracksOfMediaType (QTMediaType mediaType)
		{
			return TracksOfMediaType (QTMedia.NSStringFromQTMediaType (mediaType));
		}
	}
}