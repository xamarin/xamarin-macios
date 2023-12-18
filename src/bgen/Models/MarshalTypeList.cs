using System;
using System.Collections.Generic;
#if NET
using System.Diagnostics.CodeAnalysis;
#endif

#nullable enable

public class MarshalTypeList : List<MarshalType> {

	public void Load (TypeCache typeCache, Frameworks frameworks)
	{
		Add (new MarshalType (typeCache.NSObject, create: "Runtime.GetNSObject (", closingCreate: ")!"));
		Add (new MarshalType (typeCache.Selector, create: "Selector.FromHandle (", closingCreate: ")!"));
		Add (new MarshalType (typeCache.BlockLiteral, "BlockLiteral", "{0}", "THIS_IS_BROKEN"));
		if (typeCache.MusicSequence is not null)
			Add (new MarshalType (typeCache.MusicSequence, create: "global::AudioToolbox.MusicSequence.Lookup ("));
		Add (typeCache.CGColor);
		Add (typeCache.CGPath);
		Add (typeCache.CGGradient);
		Add (typeCache.CGContext);
		Add (typeCache.CGPDFDocument);
		Add (typeCache.CGPDFPage);
		Add (typeCache.CGImage);
		Add (typeCache.Class);
		Add (typeCache.CFRunLoop);
		Add (typeCache.CGColorSpace);
		Add (typeCache.CGImageSource);
		Add (typeCache.DispatchData);
		Add (typeCache.DispatchQueue);
		Add (typeCache.Protocol);
		if (frameworks.HaveCoreMidi)
			Add (typeCache.MidiEndpoint);
		if (frameworks.HaveCoreMedia) {
			Add (typeCache.CMTimebase);
			Add (typeCache.CMClock);
		}
		Add (typeCache.NSZone);
		if (frameworks.HaveOpenGL) {
			Add (typeCache.CGLContext);
			Add (typeCache.CGLPixelFormat);
			Add (typeCache.CVImageBuffer);
		}
		if (frameworks.HaveMediaToolbox)
			Add (new MarshalType (typeCache.MTAudioProcessingTap!, create: "MediaToolbox.MTAudioProcessingTap.FromHandle("));
		if (frameworks.HaveAddressBook) {
			Add (typeCache.ABAddressBook);
			Add (new MarshalType (typeCache.ABPerson!, create: "(ABPerson) ABRecord.FromHandle (", closingCreate: ")!"));
			Add (new MarshalType (typeCache.ABRecord!, create: "ABRecord.FromHandle (", closingCreate: ")!"));
		}
		if (frameworks.HaveCoreVideo) {
			// owns `false` like ptr ctor https://github.com/xamarin/xamarin-macios/blob/6f68ab6f79c5f1d96d2cbb1e697330623164e46d/src/CoreVideo/CVBuffer.cs#L74-L90
			Add (new MarshalType (typeCache.CVPixelBuffer!, create: "Runtime.GetINativeObject<CVPixelBuffer> (", closingCreate: ", false)!"));
		}
		Add (typeCache.CGLayer);
		if (frameworks.HaveCoreMedia)
			Add (typeCache.CMSampleBuffer);

		if (frameworks.HaveCoreVideo) {
			Add (typeCache.CVImageBuffer);
			Add (typeCache.CVPixelBufferPool);
		}
		if (frameworks.HaveAudioUnit)
			Add (typeCache.AudioComponent);
		if (frameworks.HaveCoreMedia) {
			Add (new MarshalType (typeCache.CMFormatDescription!, create: "CMFormatDescription.Create (", closingCreate: ")!"));
			Add (typeCache.CMAudioFormatDescription);
			Add (typeCache.CMVideoFormatDescription);
		}
		if (frameworks.HaveAudioUnit)
			Add (typeCache.AudioUnit);
		Add (typeCache.SecIdentity);
		Add (typeCache.SecIdentity2);
		Add (typeCache.SecTrust);
		Add (typeCache.SecTrust2);
		Add (typeCache.SecProtocolOptions);
		Add (typeCache.SecProtocolMetadata);
		Add (typeCache.SecAccessControl);
		Add (typeCache.AudioBuffers);
		if (frameworks.HaveAudioUnit) {
			Add (typeCache.AURenderEventEnumerator);
		}
	}

	void Add (Type? type)
	{
		if (type is not null)
			base.Add (type);
	}

#if NET
	public bool TryGetMarshalType (Type t, [NotNullWhen (true)] out MarshalType? res)
#else
	public bool TryGetMarshalType (Type t, out MarshalType? res)
#endif
	{
		res = null;
		// quick out for common (and easy to detect) cases
		if (t.IsArray || t.IsByRef || t.IsPrimitive)
			return false;
		foreach (var mt in this) {
			// full name is required because some types (e.g. CVPixelBuffer) are now also in core.dll
			if (mt.Type.FullName == t.FullName) {
				res = mt;
				return true;
			}
		}
		return false;
	}
}
