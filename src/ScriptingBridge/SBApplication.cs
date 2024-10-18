#nullable enable

using System;
using System.Reflection;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace ScriptingBridge {
	/// <summary>This class provides API to send Apple events to scriptable applications and get Apple events back.</summary>
	/// <remarks>
	///   <para>There are multiple steps to use this API successfully.</para>
	///   <para>First step is to create a binding project, which will define a protocol for the specific application to interact with, and the corresponding API. The code in the API definition for the binding project should look something like this:</para>
	///   <example>
	///     <code lang="csharp lang-csharp"><![CDATA[
	/// namespace Finder {
	///     [Protocol]
	///     interface FinderApplication {
	///         [Export ("version")]
	///         string Version { get; }
	///     }
	/// }]]></code>
	///   </example>
	///   <para>The second step is to create an executable project, which references the binding project. Additionally, the executable project defines a class that mirrors the protocol from the binding project:</para>
	///   <example>
	///     <code lang="csharp lang-csharp"><![CDATA[
	/// namespace Finder {
	///     [Protocol]
	///     class FinderApplication : SBApplication, IFinderApplication {
	///         // This constructor so that the runtime can create an instance of this class from the
	///         // Objetive-C object handle in the call to SBApplication.GetApplication.
	///         protected FinderApplication (NativeHandle handle) { }
	///         // Otherwise this class is empty, all the API will be provided through the protocol interface.
	///     }
	/// }]]></code>
	///   </example>
	///   <para>Note that the FinderApplication class subclasses this class (<see cref="SBApplication" />), in addition to implementing the FinderApplication protocol (which has been generated as an interface, with an <c>I</c> prefix, in the binding project).</para>
	///   <para>The final step is to call GetApplication, and cast the return value to the protocol interface.</para>
	///   <example>
	///     <code lang="csharp lang-csharp"><![CDATA[
	/// public static string GetFinderVersion () {
	///     var finder = (IFinderApplication) SBApplication.GetApplication&lt;FinderApplication&gt; ("com.apple.finder");
	///     return finder.Version;
	/// }]]></code>
	///   </example>
	/// </remarks>
	public partial class SBApplication {
		public static SBApplication? GetApplication (string ident) => Runtime.GetNSObject<SBApplication> (_FromBundleIdentifier (ident));

		public static T? GetApplication<T> (string ident) where T : SBApplication => Runtime.GetINativeObject<T> (_FromBundleIdentifier (ident), forced_type: true, owns: false);

		public static SBApplication? GetApplication (NSUrl url) => Runtime.GetNSObject<SBApplication> (_FromURL (url));

		public static T? GetApplication<T> (NSUrl url) where T : SBApplication => Runtime.GetINativeObject<T> (_FromURL (url), forced_type: true, owns: false);

		public static SBApplication? GetApplication (int pid) => Runtime.GetNSObject<SBApplication> (_FromProcessIdentifier (pid));

		public static T? GetApplication<T> (int pid) where T : SBApplication => Runtime.GetINativeObject<T> (_FromProcessIdentifier (pid), forced_type: true, owns: false);
	}
}
