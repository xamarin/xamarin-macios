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
using System;
using CoreFoundation;
using ObjCRuntime;
using Foundation;
#if !XAMCORE_2_0
using MonoMac;
#endif

using System.Runtime.InteropServices;

namespace SearchKit
{
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

	public class SKSearch :IDisposable, INativeObject
	{
		IntPtr handle;
		public IntPtr Handle { get { return handle; } }

		internal SKSearch (IntPtr h)
		{
			handle = h;
		}

		~SKSearch ()
		{
			Dispose (false);
		}

		void IDisposable.Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static bool SKSearchFindMatches (IntPtr handle, nint maxCount, IntPtr ids, IntPtr scores, double time, out nint foundCount);

		public bool FindMatches (nint maxCount, ref nint [] ids, double waitTime, out nint foundCount)
		{
			if (ids == null)
				throw new ArgumentNullException ("ids");
			if (ids.Length == 0)
				throw new ArgumentException ("ids should have at least one element");
			if (ids.Length != maxCount)
				throw new ArgumentException ("ids should have as many elements as maxCount");

			unsafe {
				fixed (nint *p = &ids [0]){
					return SKSearchFindMatches (handle, maxCount, (IntPtr) p, IntPtr.Zero, waitTime, out foundCount);
				}
			} 
		}
		
		public bool FindMatches (nint maxCount, ref nint [] ids, ref float [] scores, double waitTime, out nint foundCount)
		{
			if (ids == null)
				throw new ArgumentNullException ("ids");
			if (ids.Length == 0)
				throw new ArgumentException ("ids should have at least one element");
			if (ids.Length != maxCount)
				throw new ArgumentException ("ids should have as many elements as maxCount");

			if (scores != null) {
				if (scores.Length == 0)
					throw new ArgumentException ("scores should have at least one element");
				if (scores.Length != maxCount)
					throw new ArgumentException ("scores should have as many elements as maxCount");
			}
			unsafe {
				fixed (nint *p = &ids [0]){
					if (scores == null)
						return SKSearchFindMatches (handle, maxCount, (IntPtr) p, IntPtr.Zero, waitTime, out foundCount);
					else {
						fixed (float *s = &scores [0]){
							return SKSearchFindMatches (handle, maxCount, (IntPtr) p, (IntPtr) s, waitTime, out foundCount);
						}
					}
				}
			} 
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static void SKSearchCancel (IntPtr h);
		public void Cancel ()
		{
			SKSearchCancel (handle);
		}
	}

	public class SKDocument :IDisposable, INativeObject
	{
		IntPtr handle;
		public IntPtr Handle { get { return handle; } }

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentCreate (IntPtr scheme, IntPtr docParent, IntPtr name);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentCreateWithURL (IntPtr url);

		public SKDocument (string name, SKDocument parent = null, string scheme = null)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			var ss = scheme == null ? null : new NSString (scheme);

			using (var nn = new NSString (name))
				handle = SKDocumentCreate (ss == null ? IntPtr.Zero : ss.Handle, parent == null ? IntPtr.Zero : parent.Handle, nn.Handle);
			if (ss != null)
				ss.Dispose ();
			if (handle == IntPtr.Zero)
				throw new ArgumentNullException ("Failed to create the specified document");
		}

		internal SKDocument (IntPtr h)
		{
			handle = h;
		}
		public SKDocument (NSUrl url)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			handle = SKDocumentCreateWithURL (url.Handle);
			if (handle == IntPtr.Zero)
				throw new ArgumentNullException ("Failed to create the specified document");
		}

		~SKDocument ()
		{
			Dispose (false);
		}

		void IDisposable.Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentCopyURL (IntPtr h);
		public NSUrl Url {
			get {
				if (handle == IntPtr.Zero)
					throw new ObjectDisposedException ("disposed");
				var url = SKDocumentCopyURL (handle);
				if (url == IntPtr.Zero)
					return null;
				return new NSUrl (url);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentGetName (IntPtr h);
		public string Name {
			get {
				if (handle == IntPtr.Zero)
					throw new ObjectDisposedException ("disposed");
				
				var n = SKDocumentGetName (handle);
				if (n == IntPtr.Zero)
					return null;
				return NSString.FromHandle (n);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentGetParent (IntPtr h);
		public SKDocument GetParent ()
		{
			if (handle == IntPtr.Zero)
				throw new ObjectDisposedException ("disposed");
			var parent = SKDocumentGetParent (handle);
			if (parent == IntPtr.Zero)
				return null;
			return new SKDocument (parent);
		}
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKDocumentGetSchemeName (IntPtr h);
		public string Scheme {
			get {
				if (handle == IntPtr.Zero)
					throw new ObjectDisposedException ("disposed");
				var s = SKDocumentGetSchemeName (handle);
				if (s == IntPtr.Zero)
					return null;
				return NSString.FromHandle (s);
			}
		}
	}

	public class SKIndex :IDisposable, INativeObject
	{
		IntPtr handle;
		public IntPtr Handle { get { return handle; } }

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexCreateWithURL (IntPtr url, IntPtr str, SKIndexType type, IntPtr dict);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexCreateWithMutableData (IntPtr url, IntPtr str, SKIndexType type, IntPtr dict);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexOpenWithURL (IntPtr url, IntPtr str, bool writeAccess);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexOpenWithMutableData (IntPtr mutableData, IntPtr str);
		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexOpenWithData (IntPtr mutableData, IntPtr str);

		[DllImport (Constants.SearchKitLibrary)]
		extern static void SKIndexClose (IntPtr handle);

		internal SKIndex (IntPtr handle)
		{
			this.handle = handle;
		}

		public static SKIndex CreateWithUrl (NSUrl url, string indexName, SKIndexType type, SKTextAnalysis analysisProperties)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			var cfstr = indexName == null ? null : new NSString (indexName);
			
			var h = SKIndexCreateWithURL (url.Handle, cfstr == null ? IntPtr.Zero : cfstr.Handle, type, analysisProperties == null ? IntPtr.Zero : analysisProperties.Dictionary.Handle);
			cfstr.Dispose ();
			if (h == IntPtr.Zero)
				return null;
			return new SKIndex (h);
		}

		public static SKIndex FromUrl (NSUrl url, string indexName, bool writeAccess)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			if (indexName == null)
				throw new ArgumentNullException ("indexName");
			using (var cfstr = new NSString (indexName)) {
				var h = SKIndexOpenWithURL (url.Handle, cfstr.Handle, writeAccess);
				if (h == IntPtr.Zero)
					return null;
				return new SKIndex (h);
			}
		}

		public static SKIndex CreateWithMutableData (NSMutableData data, string indexName, SKIndexType type, SKTextAnalysis analysisProperties)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			if (indexName == null)
				throw new ArgumentNullException ("indexName");
			using (var cfstr = new NSString (indexName)) {
				var h = SKIndexCreateWithMutableData (data.Handle, cfstr.Handle, type, analysisProperties == null ? IntPtr.Zero : analysisProperties.Dictionary.Handle);
				if (h == IntPtr.Zero)
					return null;
				return new SKIndex (h);
			}
		}

		public static SKIndex FromMutableData (NSMutableData data, string indexName)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			if (indexName == null)
				throw new ArgumentNullException ("indexName");
			using (var cfstr = new NSString (indexName)) {
				var h = SKIndexOpenWithMutableData (data.Handle, cfstr.Handle);
				if (h == IntPtr.Zero)
					return null;
				return new SKIndex (h);
			}
		}

		public static SKIndex FromData (NSData data, string indexName)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			if (indexName == null)
				throw new ArgumentNullException ("indexName");
			using (var cfstr = new NSString (indexName)) {
				var h = SKIndexOpenWithData (data.Handle, cfstr.Handle);
				if (h == IntPtr.Zero)
					return null;
				return new SKIndex (h);
			}
		}

		~SKIndex ()
		{
			Dispose (false);
		}

		public void Close ()
		{
			Dispose ();
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				SKIndexClose (handle);
				handle = IntPtr.Zero;
			}
		}


		[DllImport (Constants.SearchKitLibrary)]
		extern static bool SKIndexAddDocumentWithText (IntPtr h, IntPtr doc, IntPtr str, bool canreplace);

		public bool AddDocumentWithText (SKDocument document, string text, bool canReplace)
		{
			if (document == null)
				throw new ArgumentNullException ("document");
			var ns = text == null ? null : new NSString (text);
			try {
				return SKIndexAddDocumentWithText (handle, document.Handle, ns == null ? IntPtr.Zero : ns.Handle, canReplace);
			} finally {
				if (ns != null)
					ns.Dispose ();
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static bool SKIndexAddDocument (IntPtr h, IntPtr doc, IntPtr mimeHintStr, bool canReplace);

		public bool AddDocument (SKDocument document, string mimeHint, bool canReplace)
		{
			if (document == null)
				throw new ArgumentNullException ("document");
			var ns = mimeHint == null ? null : new NSString (mimeHint);
			return SKIndexAddDocument (handle, document.Handle, ns == null ? IntPtr.Zero : ns.Handle, canReplace);
		}

		[DllImport (Constants.SearchKitLibrary, EntryPoint="SKLoadDefaultExtractorPlugIns")]
		public extern static void LoadDefaultExtractorPlugIns ();

		[DllImport (Constants.SearchKitLibrary)]
		extern static bool SKIndexFlush (IntPtr h);
		public bool Flush ()
		{
			return SKIndexFlush (handle);
		}
		[DllImport (Constants.SearchKitLibrary)]
		extern static bool SKIndexCompact (IntPtr h);
		public bool Compact ()
		{
			return SKIndexCompact (handle);
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKIndexGetDocumentCount (IntPtr handle);
		public nint DocumentCount {
			get {
				return SKIndexGetDocumentCount (handle);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKIndexGetMaximumDocumentID (IntPtr handle);

		public nint MaximumDocumentID {
			get {
				return SKIndexGetMaximumDocumentID (handle);
			}
		}
			
		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKIndexGetMaximumTermID (IntPtr handle);
		public nint MaximumTermID {
			get {
				return SKIndexGetMaximumTermID (handle);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexGetAnalysisProperties (IntPtr h);
		public SKTextAnalysis AnalysisProperties {
			get {
				return new SKTextAnalysis (Runtime.GetNSObject<NSDictionary> (SKIndexGetAnalysisProperties (handle)));
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static bool SKIndexMoveDocument (IntPtr h, IntPtr document, IntPtr newParent);
		public bool MoveDocument (SKDocument document, SKDocument newParent)
		{
			if (document == null)
				throw new ArgumentNullException ("document");
			if (newParent == null)
				throw new ArgumentNullException ("newParent");
			return SKIndexMoveDocument (handle, document.Handle, newParent.Handle);
		}


		[DllImport (Constants.SearchKitLibrary)]
		extern static bool SKIndexRemoveDocument (IntPtr h, IntPtr doc);

		public bool RemoveDocument (SKDocument document)
		{
			if (document == null)
				throw new ArgumentNullException ("document");
			return SKIndexRemoveDocument (handle, document.Handle);	
		}


		[DllImport (Constants.SearchKitLibrary)]
		extern static bool SKIndexRenameDocument (IntPtr h, IntPtr doc, IntPtr newName);
		public bool RenameDocument (SKDocument document, string newName)
		{
			if (document == null)
				throw new ArgumentNullException ("document");
			if (newName == null)
				throw new ArgumentNullException ("newName");
			using (var ns = new NSString (newName))
				return SKIndexRenameDocument (handle, document.Handle, ns.Handle);
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static void SKIndexSetMaximumBytesBeforeFlush (IntPtr h, nint value);

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKIndexGetMaximumBytesBeforeFlush (IntPtr h);

		[Advice ("Apple recommends to use Flush instead of setting these parameters")]
		public nint MaximumBytesBeforeFlush {
			get {
				return SKIndexGetMaximumBytesBeforeFlush (handle);
			}
			set {
				SKIndexSetMaximumBytesBeforeFlush (handle, value);
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKSearchCreate (IntPtr h, IntPtr str, SKSearchOptions options);
	
		public SKSearch Search (string query, SKSearchOptions options = SKSearchOptions.Default)
		{
			if (query == null)
				throw new ArgumentNullException ("query");
			using (var nsq = new NSString (query)){
				return new SKSearch (SKSearchCreate (handle, nsq.Handle, options));
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKIndexCopyDocumentForDocumentID (IntPtr idx, /* SKDocumentID -> CFIndex */ nint docId);

		public SKDocument GetDocument (nint documentId)
		{
			var doc = SKIndexCopyDocumentForDocumentID (handle, documentId);
			if (doc == IntPtr.Zero)
				return null;
			return new SKDocument (doc);
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static void SKIndexSetDocumentProperties (IntPtr h, IntPtr doc, IntPtr dict);
		public void SetDocumentProperties (SKDocument document, NSDictionary dict)
		{
			if (document == null)
				throw new ArgumentNullException ("document");
			if (dict == null)
				throw new ArgumentNullException ("dict");
			SKIndexSetDocumentProperties (handle, document.Handle, dict.Handle);
		}
	}

	public class SKSummary :IDisposable, INativeObject
	{
		IntPtr handle;
		public IntPtr Handle { get { return handle; } }

		internal SKSummary (IntPtr handle)
		{
			this.handle = handle;
		}

		~SKSummary ()
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
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static IntPtr SKSummaryCreateWithString (/* NSString */ IntPtr str);

		public static SKSummary Create (string text)
		{
			if (text == null)
				throw new ArgumentNullException ("text");
			var x = NSString.CreateNative (text);
			var h = SKSummaryCreateWithString (x);
			NSString.ReleaseNative (x);
			if (h == IntPtr.Zero)
				return null;
			
			return new SKSummary (h);
		}

		public static SKSummary Create (NSString nsString)
		{
			if (nsString == null)
				throw new ArgumentNullException ("nsString");
			
			var h = SKSummaryCreateWithString (nsString.Handle);
			if (h == IntPtr.Zero)
				return null;
			
			return new SKSummary (h);
		}

		[DllImport (Constants.SearchKitLibrary)]
		extern static nint SKSummaryGetSentenceSummaryInfo (IntPtr summary, nint maxNumSentencesInSummary, IntPtr rankOrderOfSentences, IntPtr sentenceIndexOfSentences, IntPtr paragraphIndexOfSentences );

		nint [] hack;
		
		public nint GetSentenceSummaryInfo (int maxNumSentencesInSummary, nint [] rankOrderOfSentences, nint [] sentenceIndexOfSentences, nint [] paragraphIndexOfSentences)
		{
			if (rankOrderOfSentences != null && rankOrderOfSentences.Length != maxNumSentencesInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumSentencesInSummary", nameof (rankOrderOfSentences));
			
			if (sentenceIndexOfSentences != null && sentenceIndexOfSentences.Length != maxNumSentencesInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumSentencesInSummary", nameof (sentenceIndexOfSentences));

			if (paragraphIndexOfSentences != null && paragraphIndexOfSentences.Length != maxNumSentencesInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumSentencesInSummary", nameof (paragraphIndexOfSentences));

			//
			// Because of how fixed works and our incoming arguments might be null, we are going to use this
			// fake array to take the address of.   And then, before we call the method, we check if
			// we want to pass that value or not.
			//
			if (hack == null)
				hack = new nint [1];

			unsafe {
				nint [] arr = rankOrderOfSentences == null ? hack : rankOrderOfSentences;
				nint [] ars = sentenceIndexOfSentences == null ? hack : sentenceIndexOfSentences;
				nint [] arp = paragraphIndexOfSentences == null ? hack : paragraphIndexOfSentences;
				
				fixed (nint *r = &arr [0]){
					fixed (nint *s = &ars [0]){
						fixed (nint *p = &arp [0]){
							fixed (nint *hp = &hack [0]){
								return SKSummaryGetSentenceSummaryInfo (handle, maxNumSentencesInSummary,
													(IntPtr)(r == hp ? null : r),
													(IntPtr)(s == hp ? null : s),
													(IntPtr)(p == hp ? null : p));
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
			if (rankOrderOfParagraphs != null && rankOrderOfParagraphs.Length != maxNumParagraphsInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumParagraphsInSummary", nameof (rankOrderOfParagraphs));
			if (paragraphIndexOfParagraphs != null && paragraphIndexOfParagraphs.Length != maxNumParagraphsInSummary)
				throw new ArgumentException ("array must contain as many element as specified in maxNumParagraphsInSummary", nameof (paragraphIndexOfParagraphs));

			//
			// Because of how fixed works and our incoming arguments might be null, we are going to use this
			// fake array to take the address of.   And then, before we call the method, we check if
			// we want to pass that value or not.
			//
			if (hack == null)
				hack = new nint [1];

			unsafe {
				nint [] ar = rankOrderOfParagraphs == null ? hack : rankOrderOfParagraphs;
				nint [] ap = paragraphIndexOfParagraphs == null ? hack : paragraphIndexOfParagraphs;
				
				fixed (nint *r = &ar [0]){
					fixed (nint *p = &ap [0]){
						fixed (nint *hp = &hack [0]){
							return SKSummaryGetParagraphSummaryInfo (handle, maxNumParagraphsInSummary,
												 (IntPtr)(r == hp ? null : r),
												 (IntPtr)(p == hp ? null : p));
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
				if (handle == IntPtr.Zero)
					throw new ObjectDisposedException ("disposed");
				return SKSummaryGetSentenceCount (handle);
			}
		}
		public nint ParagraphCount {
			get {
				if (handle == IntPtr.Zero)
					throw new ObjectDisposedException ("disposed");
				return SKSummaryGetParagraphCount (handle);
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

		public string GetSentence (nint idx)
		{
			return CFString.FetchString (SKSummaryCopySentenceAtIndex (handle, idx), releaseHandle: true);
		}

		public string GetParagraph (nint idx)
		{
			return CFString.FetchString (SKSummaryCopyParagraphAtIndex (handle, idx), releaseHandle: true);
		}

		public string GetSentenceSummary (nint maxSentences)
		{
			return CFString.FetchString (SKSummaryCopySentenceSummaryString (handle, maxSentences), releaseHandle: true);
		}

		public string GetParagraphSummary (nint maxParagraphs)
		{
			return CFString.FetchString (SKSummaryCopyParagraphSummaryString (handle, maxParagraphs), releaseHandle: true);
		}
		
	}
}

