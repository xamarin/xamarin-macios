using System;
using System.Collections.Generic;
#if NET
using System.Diagnostics.CodeAnalysis;
#endif

#nullable enable

public class MarshalTypeList : List<MarshalType> {

	public void Load (TypeManager typeManager, Frameworks frameworks)
	{
		Add (new MarshalType (typeManager.NSObject, create: "Runtime.GetNSObject (", closingCreate: ")!"));
		Add (new MarshalType (typeManager.Selector, create: "Selector.FromHandle (", closingCreate: ")!"));
		Add (new MarshalType (typeManager.BlockLiteral, "BlockLiteral", "{0}", "THIS_IS_BROKEN"));
		if (typeManager.MusicSequence is not null)
			Add (new MarshalType (typeManager.MusicSequence, create: "global::AudioToolbox.MusicSequence.Lookup ("));
		Add (typeManager.CGColor);
		Add (typeManager.CGPath);
		Add (typeManager.CGGradient);
		Add (typeManager.CGContext);
		Add (typeManager.CGPDFDocument);
		Add (typeManager.CGPDFPage);
		Add (typeManager.CGImage);
		Add (typeManager.Class);
		Add (typeManager.CFRunLoop);
		Add (typeManager.CGColorSpace);
		Add (typeManager.CGImageSource);
		Add (typeManager.DispatchData);
		Add (typeManager.DispatchQueue);
		Add (typeManager.Protocol);
		if (frameworks.HaveCoreMidi)
			Add (typeManager.MidiEndpoint);
		if (frameworks.HaveCoreMedia) {
			Add (typeManager.CMTimebase);
			Add (typeManager.CMClock);
		}
		Add (typeManager.NSZone);
		if (frameworks.HaveOpenGL) {
			Add (typeManager.CGLContext);
			Add (typeManager.CGLPixelFormat);
			Add (typeManager.CVImageBuffer);
		}
		if (frameworks.HaveMediaToolbox)
			Add (new MarshalType (typeManager.MTAudioProcessingTap!, create: "MediaToolbox.MTAudioProcessingTap.FromHandle("));
		if (frameworks.HaveAddressBook) {
			Add (typeManager.ABAddressBook);
			Add (new MarshalType (typeManager.ABPerson!, create: "(ABPerson) ABRecord.FromHandle (", closingCreate: ")!"));
			Add (new MarshalType (typeManager.ABRecord!, create: "ABRecord.FromHandle (", closingCreate: ")!"));
		}
		if (frameworks.HaveCoreVideo) {
			// owns `false` like ptr ctor https://github.com/xamarin/xamarin-macios/blob/6f68ab6f79c5f1d96d2cbb1e697330623164e46d/src/CoreVideo/CVBuffer.cs#L74-L90
			Add (new MarshalType (typeManager.CVPixelBuffer!, create: "Runtime.GetINativeObject<CVPixelBuffer> (", closingCreate: ", false)!"));
		}
		Add (typeManager.CGLayer);
		if (frameworks.HaveCoreMedia)
			Add (typeManager.CMSampleBuffer);

		if (frameworks.HaveCoreVideo) {
			Add (typeManager.CVImageBuffer);
			Add (typeManager.CVPixelBufferPool);
		}
		if (frameworks.HaveAudioUnit)
			Add (typeManager.AudioComponent);
		if (frameworks.HaveCoreMedia) {
			Add (new MarshalType (typeManager.CMFormatDescription!, create: "CMFormatDescription.Create (", closingCreate: ")!"));
			Add (typeManager.CMAudioFormatDescription);
			Add (typeManager.CMVideoFormatDescription);
		}
		if (frameworks.HaveAudioUnit)
			Add (typeManager.AudioUnit);
		Add (typeManager.SecIdentity);
		Add (typeManager.SecIdentity2);
		Add (typeManager.SecTrust);
		Add (typeManager.SecTrust2);
		Add (typeManager.SecProtocolOptions);
		Add (typeManager.SecProtocolMetadata);
		Add (typeManager.SecAccessControl);
		Add (typeManager.AudioBuffers);
		if (frameworks.HaveAudioUnit) {
			Add (typeManager.AURenderEventEnumerator);
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
