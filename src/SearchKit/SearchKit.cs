//
// SearchKit.cs: simple bindings for Searchkit
//
// Copyright 2015 Xamarin Inc
//
// Author:
//    Miguel de Icaza
//
// TODO:
// TODO: SKIndexDocumentIteratorCreate
// TODO: SKIndexDocumentIteratorCopyNext
// TODO: SKIndexCopyInfoForDocumentIDs
// TODO: SKIndexCopyDocumentRefsForDocumentIDs
// TODO" SKIndexCopyDocumentURLsForDocumentIDs
// TODO: SKIndexCopyDocumentIDArrayForTermID
// TODO: SKIndexCopyTermIDArrayForDocumentID
// TODO: SKIndexCopyTermStringForTermID
// TODO: SKIndexGetTermIDForTermString
//

#nullable enable

using System;
using System.Runtime.Versioning;
using CoreFoundation;
using ObjCRuntime;
using Foundation;

using System.Runtime.InteropServices;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace SearchKit {
	public enum SKIndexType {
		Unknown, Inverted, Vector, InvertedVector
	};

	[Flags]
	public enum SKSearchOptions {
		Default = 0,
		NoRelevanceScores = 1 << 0,
		SpaceMeansOr = 1 << 1,
		FindSimilar = 1 << 2
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class SKSearch : NativeObject {
		[Preserve (Conditional = true)]
		internal SKSearch (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.SearchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool SKSearchFindMatches (IntPtr handle, nint maxCount, IntPtr ids, IntPtr scores, double time, out nint foundCount);

		public bool FindMatches (nint maxCount, ref nint [] ids, double waitTime, out nint foundCount)
		{
			if (ids is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ids));
			if (ids.Length == 0)
				throw new ArgumentException ("ids should have at least one element");
			if (ids.Length != maxCount)
				throw new ArgumentException ("ids should have as many elements as maxCount");

			unsafe {
				fixed (nint* p = ids) {
					return SKSearchFindMatches (Handle, maxCount, (IntPtr) p, IntPtr.Zero, waitTime, out foundCount);
				}
			}
		}

		public bool FindMatches (nint maxCount, ref nint [] ids, ref float []? scores, double waitTime, out nint foundCount)
		{
			if (ids is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ids));
			if (ids.Length == 0)
				throw new ArgumentException ("ids should have at least one element");
			if (ids.Length != maxCount)
				throw new ArgumentException ("ids should have as many elements as maxCount");

			if (scores is not null) {
				if (scores.Length == 0)
					throw new ArgumentException ("scores should have at least one element");
				if (scores.Length != maxCount)
					throw new ArgumentException ("scores should have as many elements as maxCount");
			}
			unsafe {
				fixed (nint* p = ids) {
					if (scores is null)
						return SKSearchFindMatches (Handle, maxCount, (IntPtr) p, IntPtr.Zero, waitTime, out foundCount);
					else {
						fixed (float* s = scores) {
							return SKSearchFindMatches (Handle, maxCount, (IntPtr) p, (IntPtr) s, waitTime, out foundCount);
						}
					}
				}
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static void SKSearchCancel (IntPtr h);
		public void Cancel ()
		{
			SKSearchCancel (Handle);
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class SKDocument : NativeObject {
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentCreate (IntPtr scheme, IntPtr docParent, IntPtr name);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentCreateWithURL (IntPtr url);

		static IntPtr Create (string name, SKDocument? parent = null, string? scheme = null)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			var schemeHandle = CFString.CreateNative (scheme);
			var nameHandle = CFString.CreateNative (name);
			try {
				return SKDocumentCreate (schemeHandle, parent.GetHandle (), nameHandle);
			} finally {
				CFString.ReleaseNative (schemeHandle);
				CFString.ReleaseNative (nameHandle);
			}
		}

		public SKDocument (string name, SKDocument? parent = null, string? scheme = null)
			: base (Create (name, parent, scheme), true, true)
		{
		}

		[Preserve (Conditional = true)]
		internal SKDocument (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public SKDocument (NSUrl url)
			: base (SKDocumentCreateWithURL (Runtime.ThrowOnNull (url, nameof (url)).Handle), true, true)
		{
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentCopyURL (IntPtr h);
		public NSUrl? Url {
			get {
				var url = SKDocumentCopyURL (GetCheckedHandle ());
				return Runtime.GetNSObject<NSUrl> (url);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentGetName (IntPtr h);
		public string? Name {
			get {
				var n = SKDocumentGetName (GetCheckedHandle ());
				return CFString.FromHandle (n);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentGetParent (IntPtr h);
		public SKDocument? GetParent ()
		{
			var parent = SKDocumentGetParent (GetCheckedHandle ());
			if (parent == IntPtr.Zero)
				return null;
			return new SKDocument (parent, false);
		}
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentGetSchemeName (IntPtr h);
		public string? Scheme {
			get {
				var s = SKDocumentGetSchemeName (GetCheckedHandle ());
				return CFString.FromHandle (s);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
	public class SKIndex : DisposableObject
#else
	public class SKIndex : NativeObject
#endif
	{
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexCreateWithURL (IntPtr url, IntPtr str, SKIndexType type, IntPtr dict);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexCreateWithMutableData (IntPtr url, IntPtr str, SKIndexType type, IntPtr dict);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexOpenWithURL (IntPtr url, IntPtr str, [MarshalAs (UnmanagedType.I1)] bool writeAccess);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexOpenWithMutableData (IntPtr mutableData, IntPtr str);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexOpenWithData (IntPtr mutableData, IntPtr str);

		[DllImport (Constants.SearchKitLibrary)]
		extern static void SKIndexClose (IntPtr handle);

		[Preserve (Conditional = true)]
		SKIndex (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public static SKIndex? CreateWithUrl (NSUrl url, string indexName, SKIndexType type, SKTextAnalysis analysisProperties)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			var indexNameHandle = CFString.CreateNative (indexName);
			try {
				var handle = SKIndexCreateWithURL (url.Handle, indexNameHandle, type, analysisProperties.GetHandle ());
				if (handle == IntPtr.Zero)
					return null;
				return new SKIndex (handle, true);
			} finally {
				CFString.ReleaseNative (indexNameHandle);
			}
		}

		public static SKIndex? FromUrl (NSUrl url, string indexName, bool writeAccess)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			if (indexName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (indexName));
			var indexNameHandle = CFString.CreateNative (indexName);
			try {
				var handle = SKIndexOpenWithURL (url.Handle, indexNameHandle, writeAccess);
				if (handle == IntPtr.Zero)
					return null;
				return new SKIndex (handle, true);
			} finally {
				CFString.ReleaseNative (indexNameHandle);
			}
		}

		public static SKIndex? CreateWithMutableData (NSMutableData data, string indexName, SKIndexType type, SKTextAnalysis analysisProperties)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			if (indexName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (indexName));
			var indexNameHandle = CFString.CreateNative (indexName);
			try {
				var handle = SKIndexCreateWithMutableData (data.Handle, indexNameHandle, type, analysisProperties.GetHandle ());
				if (handle == IntPtr.Zero)
					return null;
				return new SKIndex (handle, true);
			} finally {
				CFString.ReleaseNative (indexNameHandle);
			}
		}

		public static SKIndex? FromMutableData (NSMutableData data, string indexName)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			if (indexName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (indexName));
			var indexNameHandle = CFString.CreateNative (indexName);
			try {
				var handle = SKIndexOpenWithMutableData (data.Handle, indexNameHandle);
				if (handle == IntPtr.Zero)
					return null;
				return new SKIndex (handle, true);
			} finally {
				CFString.ReleaseNative (indexNameHandle);
			}
		}

		public static SKIndex? FromData (NSData data, string indexName)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			if (indexName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (indexName));
			var indexNameHandle = CFString.CreateNative (indexName);
			try {
				var handle = SKIndexOpenWithData (data.Handle, indexNameHandle);
				if (handle == IntPtr.Zero)
					return null;
				return new SKIndex (handle, true);
			} finally {
				CFString.ReleaseNative (indexNameHandle);
			}
		}

		public void Close ()
		{
			Dispose ();
		}

#if !NET
		protected internal override void Retain ()
		{
		}

		protected internal override void Release ()
		{
		}
#endif

		protected override void Dispose (bool disposing)
		{
			if (Handle != NativeHandle.Zero) {
				SKIndexClose (Handle);
			}
			base.Dispose (disposing);
		}

		[DllImport (Constants.SearchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool SKIndexAddDocumentWithText (IntPtr h, IntPtr doc, IntPtr str, [MarshalAs (UnmanagedType.I1)] bool canreplace);

		public bool AddDocumentWithText (SKDocument document, string text, bool canReplace)
		{
			if (document is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (document));
			var textHandle = CFString.CreateNative (text);
			try {
				return SKIndexAddDocumentWithText (Handle, document.Handle, textHandle, canReplace);
			} finally {
				CFString.ReleaseNative (textHandle);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool SKIndexAddDocument (IntPtr h, IntPtr doc, IntPtr mimeHintStr, [MarshalAs (UnmanagedType.I1)] bool canReplace);

		public bool AddDocument (SKDocument document, string mimeHint, bool canReplace)
		{
			if (document is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (document));
			var mimeHintHandle = CFString.CreateNative (mimeHint);
			try {
				return SKIndexAddDocument (Handle, document.Handle, mimeHintHandle, canReplace);
			} finally {
				CFString.ReleaseNative (mimeHintHandle);
			}
		}

		[DllImport (Constants.SearchKitLibrary, EntryPoint = "SKLoadDefaultExtractorPlugIns")]
		public extern static void LoadDefaultExtractorPlugIns ();

		[DllImport (Constants.SearchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool SKIndexFlush (IntPtr h);
		public bool Flush ()
		{
			return SKIndexFlush (Handle);
		}
		[DllImport (Constants.SearchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool SKIndexCompact (IntPtr h);
		public bool Compact ()
		{
			return SKIndexCompact (Handle);
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKIndexGetDocumentCount (IntPtr handle);
		public nint DocumentCount {
			get {
				return SKIndexGetDocumentCount (Handle);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKIndexGetMaximumDocumentID (IntPtr handle);

		public nint MaximumDocumentID {
			get {
				return SKIndexGetMaximumDocumentID (Handle);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKIndexGetMaximumTermID (IntPtr handle);
		public nint MaximumTermID {
			get {
				return SKIndexGetMaximumTermID (Handle);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexGetAnalysisProperties (IntPtr h);
		public SKTextAnalysis AnalysisProperties {
			get {
				return new SKTextAnalysis (Runtime.GetNSObject<NSDictionary> (SKIndexGetAnalysisProperties (Handle)));
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool SKIndexMoveDocument (IntPtr h, IntPtr document, IntPtr newParent);
		public bool MoveDocument (SKDocument document, SKDocument newParent)
		{
			if (document is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (document));
			if (newParent is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (newParent));
			return SKIndexMoveDocument (Handle, document.Handle, newParent.Handle);
		}


		[DllImport (Constants.SearchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool SKIndexRemoveDocument (IntPtr h, IntPtr doc);

		public bool RemoveDocument (SKDocument document)
		{
			if (document is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (document));
			return SKIndexRemoveDocument (Handle, document.Handle);
		}


		[DllImport (Constants.SearchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool SKIndexRenameDocument (IntPtr h, IntPtr doc, IntPtr newName);
		public bool RenameDocument (SKDocument document, string newName)
		{
			if (document is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (document));
			if (newName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (newName));
			var newNameHandle = CFString.CreateNative (newName);
			try {
				return SKIndexRenameDocument (Handle, document.Handle, newNameHandle);
			} finally {
				CFString.ReleaseNative (newNameHandle);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static void SKIndexSetMaximumBytesBeforeFlush (IntPtr h, nint value);

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKIndexGetMaximumBytesBeforeFlush (IntPtr h);

		[Advice ("Apple recommends to use Flush instead of setting these parameters.")]
		public nint MaximumBytesBeforeFlush {
			get {
				return SKIndexGetMaximumBytesBeforeFlush (Handle);
			}
			set {
				SKIndexSetMaximumBytesBeforeFlush (Handle, value);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKSearchCreate (IntPtr h, IntPtr str, SKSearchOptions options);

		public SKSearch Search (string query, SKSearchOptions options = SKSearchOptions.Default)
		{
			if (query is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (query));
			var queryHandle = CFString.CreateNative (query);
			try {
				return new SKSearch (SKSearchCreate (Handle, queryHandle, options), true);
			} finally {
				CFString.ReleaseNative (queryHandle);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexCopyDocumentForDocumentID (IntPtr idx, /* SKDocumentID -> CFIndex */ nint docId);

		public SKDocument? GetDocument (nint documentId)
		{
			var doc = SKIndexCopyDocumentForDocumentID (Handle, documentId);
			if (doc == IntPtr.Zero)
				return null;
			return new SKDocument (doc, true);
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static void SKIndexSetDocumentProperties (IntPtr h, IntPtr doc, IntPtr dict);
		public void SetDocumentProperties (SKDocument document, NSDictionary dict)
		{
			if (document is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (document));
			if (dict is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dict));
			SKIndexSetDocumentProperties (Handle, document.Handle, dict.Handle);
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class SKSummary : NativeObject {
		[Preserve (Conditional = true)]
		internal SKSummary (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKSummaryCreateWithString (/* NSString */ IntPtr str);

		public static SKSummary? Create (string text)
		{
			if (text is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (text));
			var x = CFString.CreateNative (text);
			try {
				var handle = SKSummaryCreateWithString (x);
				if (handle == IntPtr.Zero)
					return null;
				return new SKSummary (handle, true);
			} finally {
				CFString.ReleaseNative (x);
			}
		}

		public static SKSummary? Create (NSString nsString)
		{
			if (nsString is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (nsString));

			var h = SKSummaryCreateWithString (nsString.Handle);
			if (h == IntPtr.Zero)
				return null;

			return new SKSummary (h, true);
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKSummaryGetSentenceSummaryInfo (IntPtr summary, nint maxNumSentencesInSummary, IntPtr rankOrderOfSentences, IntPtr sentenceIndexOfSentences, IntPtr paragraphIndexOfSentences);

		nint []? hack;

		public nint GetSentenceSummaryInfo (int maxNumSentencesInSummary, nint [] rankOrderOfSentences, nint [] sentenceIndexOfSentences, nint [] paragraphIndexOfSentences)
		{
			if (rankOrderOfSentences is not null && rankOrderOfSentences.Length != maxNumSentencesInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumSentencesInSummary", nameof (rankOrderOfSentences));

			if (sentenceIndexOfSentences is not null && sentenceIndexOfSentences.Length != maxNumSentencesInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumSentencesInSummary", nameof (sentenceIndexOfSentences));

			if (paragraphIndexOfSentences is not null && paragraphIndexOfSentences.Length != maxNumSentencesInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumSentencesInSummary", nameof (paragraphIndexOfSentences));

			//
			// Because of how fixed works and our incoming arguments might be null, we are going to use this
			// fake array to take the address of.   And then, before we call the method, we check if
			// we want to pass that value or not.
			//
			if (hack is null)
				hack = new nint [1];

			unsafe {
				nint [] arr = rankOrderOfSentences is null ? hack : rankOrderOfSentences;
				nint [] ars = sentenceIndexOfSentences is null ? hack : sentenceIndexOfSentences;
				nint [] arp = paragraphIndexOfSentences is null ? hack : paragraphIndexOfSentences;

				fixed (nint* r = arr) {
					fixed (nint* s = ars) {
						fixed (nint* p = arp) {
							fixed (nint* hp = hack) {
								return SKSummaryGetSentenceSummaryInfo (Handle, maxNumSentencesInSummary,
													(IntPtr) (r == hp ? null : r),
													(IntPtr) (s == hp ? null : s),
													(IntPtr) (p == hp ? null : p));
							}
						}
					}
				}
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKSummaryGetParagraphSummaryInfo (IntPtr summary, nint maxNumParagraphsInSummary, IntPtr rankOrderOfParagraphs, IntPtr paragraphIndexOfParagraphs);

		public nint GetParagraphSummaryInfo (nint maxNumParagraphsInSummary, nint [] rankOrderOfParagraphs, nint [] paragraphIndexOfParagraphs)
		{
			if (rankOrderOfParagraphs is not null && rankOrderOfParagraphs.Length != maxNumParagraphsInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumParagraphsInSummary", nameof (rankOrderOfParagraphs));
			if (paragraphIndexOfParagraphs is not null && paragraphIndexOfParagraphs.Length != maxNumParagraphsInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumParagraphsInSummary", nameof (paragraphIndexOfParagraphs));

			//
			// Because of how fixed works and our incoming arguments might be null, we are going to use this
			// fake array to take the address of.   And then, before we call the method, we check if
			// we want to pass that value or not.
			//
			if (hack is null)
				hack = new nint [1];

			unsafe {
				nint [] ar = rankOrderOfParagraphs is null ? hack : rankOrderOfParagraphs;
				nint [] ap = paragraphIndexOfParagraphs is null ? hack : paragraphIndexOfParagraphs;

				fixed (nint* r = ar) {
					fixed (nint* p = ap) {
						fixed (nint* hp = hack) {
							return SKSummaryGetParagraphSummaryInfo (Handle, maxNumParagraphsInSummary,
												 (IntPtr) (r == hp ? null : r),
												 (IntPtr) (p == hp ? null : p));
						}
					}
				}
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKSummaryGetSentenceCount (IntPtr summary);
		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKSummaryGetParagraphCount (IntPtr summary);

		public nint SentenceCount {
			get {
				return SKSummaryGetSentenceCount (GetCheckedHandle ());
			}
		}
		public nint ParagraphCount {
			get {
				return SKSummaryGetParagraphCount (GetCheckedHandle ());
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr /*NSString*/ SKSummaryCopySentenceAtIndex (IntPtr summary, nint idx);

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr /*NSString*/ SKSummaryCopyParagraphAtIndex (IntPtr summary, nint idx);

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr /*NSString*/ SKSummaryCopySentenceSummaryString (IntPtr summary, nint maxSentences);

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr /*NSString*/  SKSummaryCopyParagraphSummaryString (IntPtr summary, nint maxParagraphs);

		public string? GetSentence (nint idx)
		{
			return CFString.FromHandle (SKSummaryCopySentenceAtIndex (Handle, idx), releaseHandle: true);
		}

		public string? GetParagraph (nint idx)
		{
			return CFString.FromHandle (SKSummaryCopyParagraphAtIndex (Handle, idx), releaseHandle: true);
		}

		public string? GetSentenceSummary (nint maxSentences)
		{
			return CFString.FromHandle (SKSummaryCopySentenceSummaryString (Handle, maxSentences), releaseHandle: true);
		}

		public string? GetParagraphSummary (nint maxParagraphs)
		{
			return CFString.FromHandle (SKSummaryCopyParagraphSummaryString (Handle, maxParagraphs), releaseHandle: true);
		}

	}
}
