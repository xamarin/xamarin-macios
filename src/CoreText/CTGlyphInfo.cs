// 
// CTGlyphInfo.cs: Implements the managed CTGlyphInfo
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
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
using CoreFoundation;
using CoreGraphics;

using CGGlyph     = System.UInt16;
using CGFontIndex = System.UInt16;

namespace CoreText {

#region Glyph Info Values
	public enum CTCharacterCollection : ushort {
		IdentityMapping = 0,
		AdobeCNS1       = 1,
		AdobeGB1        = 2,
		AdobeJapan1     = 3,
		AdobeJapan2     = 4,
		AdobeKorea1     = 5,
	}
#endregion

	public class CTGlyphInfo : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CTGlyphInfo (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw ConstructorError.ArgumentNull (this, "handle");

			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}
		
		public IntPtr Handle {
			get {return handle;}
		}

		~CTGlyphInfo ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

#region Glyph Info Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTGlyphInfoCreateWithGlyphName (IntPtr glyphName, IntPtr font, IntPtr baseString);
		public CTGlyphInfo (string glyphName, CTFont font, string baseString)
		{
			if (glyphName == null)
				throw ConstructorError.ArgumentNull (this, "glyphName");
			if (font == null)
				throw ConstructorError.ArgumentNull (this, "font");
			if (baseString == null)
				throw ConstructorError.ArgumentNull (this, "baseString");

			using (var gn = new NSString (glyphName))
			using (var bs = new NSString (baseString))
				handle = CTGlyphInfoCreateWithGlyphName (gn.Handle, font.Handle, bs.Handle);

			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTGlyphInfoCreateWithGlyph (CGGlyph glyph, IntPtr font, IntPtr baseString);
		public CTGlyphInfo (CGGlyph glyph, CTFont font, string baseString)
		{
			if (font == null)
				throw ConstructorError.ArgumentNull (this, "font");
			if (baseString == null)
				throw ConstructorError.ArgumentNull (this, "baseString");

			using (var bs = new NSString (baseString))
				handle = CTGlyphInfoCreateWithGlyph (glyph, font.Handle, bs.Handle);

			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTGlyphInfoCreateWithCharacterIdentifier (CGFontIndex cid, CTCharacterCollection collection, IntPtr baseString);
		public CTGlyphInfo (CGFontIndex cid, CTCharacterCollection collection, string baseString)
		{
			if (baseString == null)
				throw ConstructorError.ArgumentNull (this, "baseString");

			using (var bs = new NSString (baseString))
				handle = CTGlyphInfoCreateWithCharacterIdentifier (cid, collection, bs.Handle);

			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}
#endregion

#region Glyph Info Access
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTGlyphInfoGetGlyphName (IntPtr glyphInfo);
		public string GlyphName {
			get {
				var cfStringRef = CTGlyphInfoGetGlyphName (handle);
				return CFString.FetchString (cfStringRef);
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CGFontIndex CTGlyphInfoGetCharacterIdentifier (IntPtr glyphInfo);
		public CGFontIndex CharacterIdentifier {
			get {return CTGlyphInfoGetCharacterIdentifier (handle);}
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern CTCharacterCollection CTGlyphInfoGetCharacterCollection (IntPtr glyphInfo);
		public CTCharacterCollection CharacterCollection {
			get {return CTGlyphInfoGetCharacterCollection (handle);}
		}
#endregion

		public override string ToString ()
		{
			return GlyphName;
		}
	}
}

