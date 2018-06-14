//
// GLK/GLTextureLoader.cs: extenions for GLTextureLoader
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011-2013 Xamarin, Inc.
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
#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using ObjCRuntime;

namespace GLKit {
	public partial class GLKTextureLoader {
		public static GLKTextureInfo CubeMapFromFiles (string [] files, NSDictionary textureOperations, out NSError error)
		{
			using (var array = NSArray.FromStrings (files))
				return CubeMapFromFiles (array, textureOperations, out error);
		}

		public static GLKTextureInfo CubeMapFromUrls (NSUrl [] urls, NSDictionary textureOperations, out NSError error)
		{
			using (var array = NSArray.FromNSObjects (urls))
				return CubeMapFromFiles (array, textureOperations, out error);
		}

		public void BeginLoadCubeMap (string [] files, NSDictionary textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			using (var array = NSArray.FromStrings (files))
				BeginLoadCubeMap (array, textureOperations, queue, onComplete);
		}

		public void BeginLoadCubeMap (NSUrl [] urls, NSDictionary textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			using (var array = NSArray.FromNSObjects (urls))
				BeginLoadCubeMap (array, textureOperations, queue, onComplete);
		}

		//
		// New, strongly typed
		//
		public static GLKTextureInfo FromFile (string path, GLKTextureOperations textureOperations, out NSError error)
		{
			return FromFile (path, textureOperations == null ? null : textureOperations.Dictionary, out error);
		}

		public static GLKTextureInfo FromUrl (NSUrl url, GLKTextureOperations textureOperations, out NSError error)
		{
			return FromUrl (url, textureOperations == null ? null : textureOperations.Dictionary, out error);
		}

		public static GLKTextureInfo FromData (NSData data, GLKTextureOperations textureOperations, out NSError error)
		{
			return FromData (data, textureOperations == null ? null : textureOperations.Dictionary, out error);
		}

		public static GLKTextureInfo FromImage (CGImage cgImage, GLKTextureOperations textureOperations, out NSError error)
		{
			return FromImage (cgImage, textureOperations == null ? null : textureOperations.Dictionary, out error);
		}

		public static GLKTextureInfo CubeMapFromFiles (string [] files, GLKTextureOperations textureOperations, out NSError error)
		{
			using (var array = NSArray.FromStrings (files))
				return CubeMapFromFiles (files, textureOperations == null ? null : textureOperations.Dictionary, out error);
		}

		public static GLKTextureInfo CubeMapFromUrls (NSUrl [] urls, GLKTextureOperations textureOperations, out NSError error)
		{
			using (var array = NSArray.FromNSObjects (urls))
				return CubeMapFromFiles (array, textureOperations == null ? null : textureOperations.Dictionary, out error);
		}
		
		public static GLKTextureInfo CubeMapFromFile (string path, GLKTextureOperations textureOperations, out NSError error)
		{
			return CubeMapFromFile (path, textureOperations == null ? null : textureOperations.Dictionary, out error);
		}

		public static GLKTextureInfo CubeMapFromUrl (NSUrl url, GLKTextureOperations textureOperations, out NSError error)
		{
			return CubeMapFromUrl (url, textureOperations == null ? null : textureOperations.Dictionary, out error);
		}

		public void BeginTextureLoad (string file, GLKTextureOperations textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			BeginTextureLoad (file, textureOperations == null ? null : textureOperations.Dictionary, queue, onComplete);
		}

		public void BeginTextureLoad (NSUrl filePath, GLKTextureOperations textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			BeginTextureLoad (filePath, textureOperations == null ? null : textureOperations.Dictionary, queue, onComplete);
		}

		public void BeginTextureLoad (NSData data, GLKTextureOperations textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			BeginTextureLoad (data, textureOperations == null ? null : textureOperations.Dictionary, queue, onComplete);
		}

		public void BeginTextureLoad (CGImage image, GLKTextureOperations textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			BeginTextureLoad (image, textureOperations == null ? null : textureOperations.Dictionary, queue, onComplete);
		}

		public void BeginLoadCubeMap (string [] files, GLKTextureOperations textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			using (var array = NSArray.FromStrings (files))
				BeginLoadCubeMap (array, textureOperations == null ? null : textureOperations.Dictionary, queue, onComplete);
		}

		public void BeginLoadCubeMap (NSUrl [] urls, GLKTextureOperations textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			using (var array = NSArray.FromNSObjects (urls))
				BeginLoadCubeMap (array, textureOperations == null ? null : textureOperations.Dictionary, queue, onComplete);
		}

		public void BeginLoadCubeMap (string fileName, GLKTextureOperations textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			BeginLoadCubeMap (fileName, textureOperations == null ? null : textureOperations.Dictionary, queue, onComplete);
		}

		public void BeginLoadCubeMap (NSUrl filePath, GLKTextureOperations textureOperations, DispatchQueue queue, GLKTextureLoaderCallback onComplete)
		{
			BeginLoadCubeMap (filePath, textureOperations == null ? null : textureOperations.Dictionary, queue, onComplete);
		}
		
	}

	[Deprecated (PlatformName.iOS, 12,0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12,0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10,14, message: "Use 'Metal' instead.")]
	public class GLKTextureOperations : DictionaryContainer {
		public GLKTextureOperations () : base (new NSMutableDictionary ()) {}

		public GLKTextureOperations (NSDictionary options) : base (options) {}

		public bool? ApplyPremultiplication {
			get {
				return GetBoolValue (GLKTextureLoader.ApplyPremultiplication);
			}
			set {
				SetBooleanValue (GLKTextureLoader.ApplyPremultiplication, value);
			}
		}

		public bool? OriginBottomLeft {
			get {
				return GetBoolValue (GLKTextureLoader.OriginBottomLeft);
			}
			set {
				SetBooleanValue (GLKTextureLoader.OriginBottomLeft, value);
			}
		}

		public bool? GenerateMipmaps {
			get {
				return GetBoolValue (GLKTextureLoader.GenerateMipmaps);
			}
			set {
				SetBooleanValue (GLKTextureLoader.GenerateMipmaps, value);
			}
		}

		public bool? GrayscaleAsAlpha {
			get {
				return GetBoolValue (GLKTextureLoader.GrayscaleAsAlpha);
			}
			set {
				SetBooleanValue (GLKTextureLoader.GrayscaleAsAlpha, value);
			}
		}

		[iOS (7,0)]
		public bool? SRGB {
			get {
				return GetBoolValue (GLKTextureLoader.SRGB);
			}
			set {
				SetBooleanValue (GLKTextureLoader.SRGB, value);
			}
		}
	}
}
#endif
