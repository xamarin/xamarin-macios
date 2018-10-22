using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class IntentDefinitionCodegenTaskBase : Task
	{
		static readonly string[] DefaultResponseCodes = {
			"Unspecified", "Ready", "ContinueInApp", "InProgress", "Success", "Failure", "FailureRequiringAppLaunch"
		};

		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public ITaskItem IntentDefinition { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public string RootNamespace { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] GeneratedSources { get; set; }

		#endregion

		static string GetCSharpName (string name)
		{
			if (name.Length > 1)
				return char.ToUpperInvariant (name[0]) + name.Substring (1);

			return name.ToUpperInvariant ();
		}

		static string GetCSharpTypeName (string type)
		{
			switch (type) {
			case "CurrencyAmount": return "INCurrencyAmount";
			case "Decimal":        return "NSNumber";
			case "Distance":       return "NSUnitLength";
			case "Integer":        return "NSNumber";
			case "Object":         return "INObject";
			case "PaymentMethod":  return "INPaymentMethod";
			case "Person":         return "INPerson";
			case "Placemark":      return "CLPlacemark";
			case "String":         return "NSString";
			case "Temperature":    return "NSUnitTemperature";
			case "URL":            return "NSUrl";
			default: return type;
			}
		}

		void WriteEnumDefinition (StreamWriter writer, string prefix, PDictionary enumDefinition)
		{
			PArray values;
			PNumber index;
			PString name;

			if (!enumDefinition.TryGetValue (SiriIntentsKeys.INEnumName, out name))
				return;

			if (!enumDefinition.TryGetValue (SiriIntentsKeys.INEnumValues, out values))
				return;

			writer.WriteLine ("\t[Native]");
			writer.WriteLine ("\tenum {0}{1}", prefix, name.Value);
			writer.WriteLine ("\t{");
			for (int i = 0; i < values.Count; i++) {
				var valueDefinition = values[i] as PDictionary;

				if (valueDefinition == null)
					continue;

				// TODO: Should this use INEnumValueDisplayName?
				if (!valueDefinition.TryGetValue (SiriIntentsKeys.INEnumValueName, out name))
					continue;

				if (valueDefinition.TryGetValue (SiriIntentsKeys.INEnumValueIndex, out index))
					writer.WriteLine ("\t\t{0} = {1},", GetCSharpName (name.Value), index.Value);
				else
					writer.WriteLine ("\t\t{0},", GetCSharpName (name.Value));
			}
			writer.WriteLine ("\t}");
			writer.WriteLine ();
		}

		void WriteIntentDefinition (StreamWriter writer, string prefix, PDictionary intentDefinition)
		{
			writer.WriteLine ("\t[Register (\"{0}Intent\")]", prefix);
			writer.WriteLine ("\tclass {0}Intent : INIntent", prefix);
			writer.WriteLine ("\t{");

			// write out the .ctors
			writer.WriteLine ("\t\tpublic {0}Intent () : base (NSObjectFlag.Empty) {{ }}", prefix);
			writer.WriteLine ();
			writer.WriteLine ("\t\tprotected {0}Intent (IntPtr handle) : base (handle) {{ }}", prefix);

			// write out the properties
			if (intentDefinition.TryGetValue (SiriIntentsKeys.INIntentParameters, out PArray parameters)) {
				foreach (var parameterDefinition in parameters.OfType<PDictionary> ()) {
					bool isMultiDimensional = false;
					string propertyName;
					string typeName;

					if (!parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentParameterName, out PString parameterName))
						continue;

					if (parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentParameterSupportsMultipleValues, out PBoolean multi))
						isMultiDimensional = multi.Value;

					writer.WriteLine ();

					if (parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentParameterEnumType, out PString enumType)) {
						writer.WriteLine ("\t\t[Export (\"{0}\"{1})]", parameterName.Value, isMultiDimensional ? ", ArgumentSemantic.Copy" : string.Empty);
						typeName = prefix + enumType.Value;
					} else if (parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentParameterType, out PString parameterType)) {
						writer.WriteLine ("\t\t[Export (\"{0}\", ArgumentSemantic.Copy)]", parameterName.Value);
						typeName = GetCSharpTypeName (parameterType.Value);
					} else {
						continue;
					}

					propertyName = GetCSharpName (parameterName.Value);

					if (isMultiDimensional)
						writer.WriteLine ("\t\tpublic NSArray<{0}> {1} {{ get; set; }}", typeName, propertyName);
					else
						writer.WriteLine ("\t\tpublic {0} {1} {{ get; set; }}", typeName, propertyName);
				}
			}

			writer.WriteLine ("\t}");
		}

		void WriteIntentResponseCodeEnumDefinition (StreamWriter writer, string prefix, PDictionary responseDefinition)
		{
			writer.WriteLine ("\t[Native]");
			writer.WriteLine ("\tenum {0}IntentResponseCode : long", prefix);
			writer.WriteLine ("\t{");

			// write out the default response code enum values
			for (int i = 0; i < DefaultResponseCodes.Length; i++)
				writer.WriteLine ("\t\t{0},", DefaultResponseCodes[i]);

			// write out any custom response code enum values
			if (responseDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseCodes, out PArray responseCodes) && responseCodes.Count > 0) {
				var known = new HashSet<string> (DefaultResponseCodes, StringComparer.Ordinal);
				var first = true;

				foreach (var responseCode in responseCodes.OfType<PDictionary> ()) {
					if (!responseCode.TryGetValue (SiriIntentsKeys.INIntentResponseCodeName, out PString name))
						continue;

					var csharp = GetCSharpName (name.Value);

					if (!known.Add (csharp))
						continue;

					if (first) {
						writer.WriteLine ("\t\t{0} = 100,", csharp);
						first = false;
					} else {
						writer.WriteLine ("\t\t{0},", csharp);
					}
				}
			}

			writer.WriteLine ("\t}");
			writer.WriteLine ();
		}

		void WriteResponseCodeCreationMethods (StreamWriter writer, string prefix, PDictionary responseDefinition, Dictionary<string, string> parameterTypes)
		{
			if (!responseDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseCodes, out PArray responseCodes))
				return;

			foreach (var responseCode in responseCodes.OfType<PDictionary> ()) {
				if (!responseCode.TryGetValue (SiriIntentsKeys.INIntentResponseCodeFormatString, out PString format))
					continue;

				int startIndex = format.Value.IndexOf ("${", StringComparison.Ordinal);

				if (startIndex == -1)
					continue;

				int endIndex = format.Value.IndexOf ('}', startIndex += 2);

				if (endIndex == -1)
					continue;

				if (!responseCode.TryGetValue (SiriIntentsKeys.INIntentResponseCodeName, out PString name))
					continue;

				var methodName = new StringBuilder (GetCSharpName (name.Value) + "IntentResponseWith");
				var symbolName = new StringBuilder (name.Value + "IntentResponseWith");
				var parameters = new List<string> ();
				bool first = true;

				do {
					var parameterName = format.Value.Substring (startIndex, endIndex - startIndex);

					parameters.Add (parameterName);

					if (first) {
						var csharp = GetCSharpName (parameterName);

						methodName.Append (csharp);
						symbolName.Append (csharp);
						first = false;
					} else {
						symbolName.Append (parameterName);
					}

					symbolName.Append (':');

					if ((startIndex = format.Value.IndexOf ("${", endIndex + 1, StringComparison.Ordinal)) == -1)
						break;

					endIndex = format.Value.IndexOf ('}', startIndex += 2);
				} while (endIndex != -1);

				writer.WriteLine ("\t\t[Export (\"{0}\")]", symbolName);
				writer.Write ("\t\tpublic static {0}IntentResponse {1} (", prefix, methodName);
				for (int i = 0; i < parameters.Count; i++) {
					if (!parameterTypes.TryGetValue (parameters[i], out string parameterType)) {
						Log.LogError (7069, IntentDefinition.ItemSpec, "Unknown response parameter: {0}", parameters[i]);
						continue;
					}

					writer.Write ("{0} {1}{2}", parameterType, parameters[i], i + 1 < parameters.Count ? ", " : string.Empty);
				}

				writer.WriteLine (")");
				writer.WriteLine ("\t\t{");
				writer.WriteLine ("\t\t\treturn new {0}IntentResponse ({0}IntentResponseCode.{1}, null) {{", prefix, GetCSharpName (name.Value));
				for (int i = 0; i < parameters.Count; i++)
					writer.WriteLine ("\t\t\t\t{0} = {1},", GetCSharpName (parameters[i]), parameters[i]);
				writer.WriteLine ("\t\t\t};");
				writer.WriteLine ("\t\t}");
				writer.WriteLine ();
			}
		}

		void WriteIntentResponseDefinition (StreamWriter writer, string prefix, PDictionary responseDefinition)
		{
			WriteIntentResponseCodeEnumDefinition (writer, prefix, responseDefinition);

			writer.WriteLine ("\t[Register (\"{0}IntentResponse\")]", prefix);
			writer.WriteLine ("\tclass {0}IntentResponse : INIntentResponse", prefix);
			writer.WriteLine ("\t{");

			// write out the .ctors
			writer.WriteLine ("\t\t[Export (\"initWithCode:userActivity:\")]");
			writer.WriteLine ("\t\tpublic {0}IntentResponse ({0}IntentResponseCode code, NSUserActivity userActivity)", prefix);
			writer.WriteLine ("\t\t{");
			writer.WriteLine ("\t\t\tCode = code;");
			writer.WriteLine ("\t\t\tUserActivity = userActivity;");
			writer.WriteLine ("\t\t}");
			writer.WriteLine ();

			if (responseDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseParameters, out PArray responseParameters)) {
				var parameterTypes = new Dictionary<string, string> (StringComparer.Ordinal);
				var isEnumType = new Dictionary<string, bool> (StringComparer.Ordinal);
				var parameters = new List<string> ();

				foreach (var parameterDefinition in responseParameters.OfType<PDictionary> ()) {
					var isMultiDimensional = false;
					string typeName;

					if (!parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseParameterName, out PString parameterName))
						continue;

					if (parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseParameterSupportsMultipleValues, out PBoolean multi))
						isMultiDimensional = multi.Value;

					if (parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseParameterEnumType, out PString enumType)) {
						typeName = prefix + enumType.Value;
					} else if (parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseParameterType, out PString parameterType)) {
						typeName = GetCSharpTypeName (parameterType.Value);
					} else {
						continue;
					}

					if (isMultiDimensional)
						typeName = "NSArray<" + typeName + ">";

					if (!parameterTypes.ContainsKey (parameterName.Value)) {
						isEnumType.Add (parameterName.Value, enumType != null && !isMultiDimensional);
						parameterTypes.Add (parameterName.Value, typeName);
						parameters.Add (parameterName.Value);
					}
				}

				// write out the static create methods
				WriteResponseCodeCreationMethods (writer, prefix, responseDefinition, parameterTypes);

				// write out properties based on the parameters
				foreach (var parameter in parameters) {
					var semantic = isEnumType[parameter] ? ", ArgumentSemantic.Copy" : string.Empty;

					writer.WriteLine ("\t\tpublic {0} {1} {{", parameterTypes[parameter], GetCSharpName (parameter));
					writer.WriteLine ("\t\t\t[Export (\"{0}\"{1})]", parameter, semantic);
					writer.WriteLine ("\t\t\tget;");
					writer.WriteLine ("\t\t\t[Export (\"set{0}:\"{1})]", GetCSharpName (parameter), semantic);
					writer.WriteLine ("\t\t\tset;");
					writer.WriteLine ("\t\t}");
					writer.WriteLine ();
				}
			}

			writer.WriteLine ("\t\t[Export (\"code\")]");
			writer.WriteLine ("\t\tpublic {0}IntentResponseCode Code {{ get; }}", prefix);

			writer.WriteLine ("\t}");
		}

		void WriteIntentHandlingTrampolines (TextWriter writer, string prefix)
		{
			writer.WriteLine ("\t[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
			writer.WriteLine ("\tstatic class {0}IntentHandlingTrampolines", prefix);
			writer.WriteLine ("\t{");
			writer.WriteLine ("\t\tconst string LIBOBJC_DYLIB = \"/usr/lib/libobjc.dylib\";");
			writer.WriteLine ();
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			writer.WriteLine ("\t\tpublic extern static IntPtr IntPtr_objc_msgSend (IntPtr receiever, IntPtr selector);");
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
			writer.WriteLine ("\t\tpublic extern static IntPtr IntPtr_objc_msgSendSuper (IntPtr receiever, IntPtr selector);");
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			writer.WriteLine ("\t\tpublic extern static IntPtr IntPtr_objc_msgSend_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);");
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
			writer.WriteLine ("\t\tpublic extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);");
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			writer.WriteLine ("\t\tpublic extern static void void_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1);");
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
			writer.WriteLine ("\t\tpublic extern static void void_objc_msgSendSuper_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1);");
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			writer.WriteLine ("\t\tpublic extern static void void_objc_msgSend_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);");
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
			writer.WriteLine ("\t\tpublic extern static void void_objc_msgSendSuper_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);");
			writer.WriteLine ("\t\t[DllImport (LIBOBJC_DYLIB)]");
			writer.WriteLine ("\t\tstatic extern IntPtr _Block_copy (IntPtr ptr);");
			writer.WriteLine ();
			writer.WriteLine ("\t\t[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]");
			writer.WriteLine ("\t\t[UserDelegateType (typeof ({0}ResponseCallback))]", prefix);
			writer.WriteLine ("\t\tinternal delegate void D{0}ResponseCallback (IntPtr block, IntPtr response);", prefix);
			writer.WriteLine ();
			writer.WriteLine ("\t\tstatic internal class SD{0}ResponseCallback", prefix);
			writer.WriteLine ("\t\t{");
			writer.WriteLine ("\t\t\tstatic internal readonly D{0}ResponseCallback Handler = Invoke;", prefix);
			writer.WriteLine ();
			writer.WriteLine ("\t\t\t[MonoPInvokeCallback (typeof (D{0}ResponseCallback))]", prefix);
			writer.WriteLine ("\t\t\tstatic unsafe void Invoke (IntPtr block, IntPtr response)");
			writer.WriteLine ("\t\t\t{");
			writer.WriteLine ("\t\t\t\tvar descriptor = (BlockLiteral*) block;");
			writer.WriteLine ("\t\t\t\tvar del = ({0}ResponseCallback) (descriptor->Target);", prefix);
			writer.WriteLine ("\t\t\t\tif (del != null)");
			writer.WriteLine ("\t\t\t\t\tdel (Runtime.GetNSObject<{0}IntentResponse> (response));", prefix);
			writer.WriteLine ("\t\t\t}");
			writer.WriteLine ("\t\t}");
			writer.WriteLine ();
			writer.WriteLine ("\t\tinternal class NID{0}ResponseCallback", prefix);
			writer.WriteLine ("\t\t{");
			writer.WriteLine ("\t\t\tIntPtr blockPtr;");
			writer.WriteLine ("\t\t\tD{0}ResponseCallback invoker;", prefix);
			writer.WriteLine ();
			writer.WriteLine ("\t\t\t[Preserve (Conditional=true)]");
			writer.WriteLine ("\t\t\t[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
			writer.WriteLine ("\t\t\tpublic unsafe NID{0}ResponseCallback (BlockLiteral *block)", prefix);
			writer.WriteLine ("\t\t\t{");
			writer.WriteLine ("\t\t\t\tblockPtr = _Block_copy ((IntPtr) block);");
			writer.WriteLine ("\t\t\t\tinvoker = block->GetDelegateForBlock<D{0}ResponseCallback> ();", prefix);
			writer.WriteLine ("\t\t\t}");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\t[Preserve (Conditional=true)]");
			writer.WriteLine ("\t\t\t[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
			writer.WriteLine ("\t\t\t~NID{0}ResponseCallback ()", prefix);
			writer.WriteLine ("\t\t\t{");
			writer.WriteLine ("\t\t\t\tRuntime.ReleaseBlockOnMainThread (blockPtr);");
			writer.WriteLine ("\t\t\t}");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\t[Preserve (Conditional=true)]");
			writer.WriteLine ("\t\t\t[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
			writer.WriteLine ("\t\t\tpublic unsafe static {0}ResponseCallback Create (IntPtr block)", prefix);
			writer.WriteLine ("\t\t\t{");
			writer.WriteLine ("\t\t\t\tif (block == IntPtr.Zero)");
			writer.WriteLine ("\t\t\t\t\treturn null;");
			writer.WriteLine ("\t\t\t\tif (BlockLiteral.IsManagedBlock (block)) {");
			writer.WriteLine ("\t\t\t\t\tvar existing_delegate = ((BlockLiteral *) block)->Target as {0}ResponseCallback;", prefix);
			writer.WriteLine ("\t\t\t\t\tif (existing_delegate != null)");
			writer.WriteLine ("\t\t\t\t\t\treturn existing_delegate;");
			writer.WriteLine ("\t\t\t\t}");
			writer.WriteLine ("\t\t\t\treturn new NID{0}ResponseCallback ((BlockLiteral *) block).Invoke;", prefix);
			writer.WriteLine ("\t\t\t}");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\t[Preserve (Conditional=true)]");
			writer.WriteLine ("\t\t\t[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
			writer.WriteLine ("\t\t\tunsafe void Invoke ({0}IntentResponse response)", prefix);
			writer.WriteLine ("\t\t\t{");
			writer.WriteLine ("\t\t\t\tinvoker (blockPtr, response == null ? IntPtr.Zero : response.Handle);");
			writer.WriteLine ("\t\t\t}");
			writer.WriteLine ("\t\t}");
			writer.WriteLine ("\t}");
		}

		void WriteIntentHandlingInterface (TextWriter writer, string prefix)
		{
			writer.WriteLine ("\t[Protocol (Name = \"{0}IntentHandling\", WrapperType = typeof ({0}IntentHandlingWrapper))]", prefix);
			writer.WriteLine ("\t[ProtocolMember (IsRequired = true, IsProperty = false, IsStatic = false, Name = \"Handle{0}\", Selector = \"handle{0}:completion:\", ParameterType = new Type [] {{ typeof ({0}Intent), typeof ({0}ResponseCallback) }}, ParameterByRef = new bool [] {{ false, false }}, ParameterBlockProxy = new Type [] {{ null, typeof ({0}IntentHandlingTrampolines.NID{0}ResponseCallback) }})]", prefix);
			writer.WriteLine ("\t[ProtocolMember (IsRequired = false, IsProperty = false, IsStatic = false, Name = \"Confirm{0}\", Selector = \"confirm{0}:completion:\", ParameterType = new Type [] {{ typeof ({0}Intent), typeof ({0}ResponseCallback) }}, ParameterByRef = new bool [] {{ false, false }}, ParameterBlockProxy = new Type [] {{ null, typeof ({0}IntentHandlingTrampolines.NID{0}ResponseCallback) }})]", prefix);
			writer.WriteLine ("\tinterface I{0}IntentHandling : INativeObject, IDisposable", prefix);
			writer.WriteLine ("\t{");
			writer.WriteLine ("\t\t[Export (\"handle{0}:completion:\")]", prefix);
			writer.WriteLine ("\t\t[Preserve (Conditional = true)]");
			writer.WriteLine ("\t\tvoid Handle{0} ({0}Intent intent, [BlockProxy (typeof ({0}IntentHandlingTrampolines.NID{0}ResponseCallback))]{0}ResponseCallback response);", prefix);
			writer.WriteLine ("\t}");
		}

		void WriteIntentHandlingExtensions (TextWriter writer, string prefix)
		{
			writer.WriteLine ("\tstatic partial class {0}IntentHandlingExtensions", prefix);
			writer.WriteLine ("\t{");
			writer.WriteLine ("\t\t[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
			writer.WriteLine ("\t\tpublic unsafe static void Confirm{0} (this I{0}IntentHandling This, {0}Intent intent, [BlockProxy (typeof ({0}IntentHandlingTrampolines.NID{0}ResponseCallback))]{0}ResponseCallback response)", prefix);
			writer.WriteLine ("\t\t{");
			writer.WriteLine ("\t\t\tif (intent == null)");
			writer.WriteLine ("\t\t\t\tthrow new ArgumentNullException (\"intent\");");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\tif (response == null)");
			writer.WriteLine ("\t\t\t\tthrow new ArgumentNullException (\"response\");");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\tBlockLiteral *block_ptr_response;");
			writer.WriteLine ("\t\t\tBlockLiteral block_response;");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\tblock_response = new BlockLiteral ();");
			writer.WriteLine ("\t\t\tblock_ptr_response = &block_response;");
			writer.WriteLine ("\t\t\tblock_response.SetupBlockUnsafe ({0}IntentHandlingTrampolines.SD{0}ResponseCallback.Handler, response);", prefix);
			writer.WriteLine ();
			writer.WriteLine ("\t\t\t{0}IntentHandlingTrampolines.void_objc_msgSend_IntPtr_IntPtr (This.Handle, Selector.GetHandle (\"confirm{0}:completion:\"), intent.Handle, (IntPtr) block_ptr_response);", prefix);
			writer.WriteLine ("\t\t\tblock_ptr_response->CleanupBlock ();");
			writer.WriteLine ("\t\t}");
			writer.WriteLine ("\t}");
		}

		void WriteIntentHandlingWrapper (TextWriter writer, string prefix)
		{
			writer.WriteLine ("\tsealed class {0}IntentHandlingWrapper : BaseWrapper, I{0}IntentHandling", prefix);
			writer.WriteLine ("\t{");
			writer.WriteLine ("\t\t[Preserve (Conditional = true)]");
			writer.WriteLine ("\t\tpublic {0}IntentHandlingWrapper (IntPtr handle, bool owns) : base (handle, owns)", prefix);
			writer.WriteLine ("\t\t{");
			writer.WriteLine ("\t\t}");
			writer.WriteLine ();
			writer.WriteLine ("\t\t[Export (\"handle{0}:completion:\")]", prefix);
			writer.WriteLine ("\t\t[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
			writer.WriteLine ("\t\tpublic unsafe void Handle{0} ({0}Intent intent, [BlockProxy (typeof ({0}IntentHandlingTrampolines.NID{0}ResponseCallback))]{0}ResponseCallback response)", prefix);
			writer.WriteLine ("\t\t{");
			writer.WriteLine ("\t\t\tif (intent == null)");
			writer.WriteLine ("\t\t\t\tthrow new ArgumentNullException (\"intent\");");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\tif (response == null)");
			writer.WriteLine ("\t\t\t\tthrow new ArgumentNullException (\"response\");");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\tBlockLiteral *block_ptr_response;");
			writer.WriteLine ("\t\t\tBlockLiteral block_response;");
			writer.WriteLine ();
			writer.WriteLine ("\t\t\tblock_response = new BlockLiteral ();");
			writer.WriteLine ("\t\t\tblock_ptr_response = &block_response;");
			writer.WriteLine ("\t\t\tblock_response.SetupBlockUnsafe ({0}IntentHandlingTrampolines.SD{0}ResponseCallback.Handler, response);", prefix);
			writer.WriteLine ();
			writer.WriteLine ("\t\t\t{0}IntentHandlingTrampolines.void_objc_msgSend_IntPtr_IntPtr (intent.Handle, Selector.GetHandle (\"handle{0}:completion:\"), intent.Handle, (IntPtr) block_ptr_response);", prefix);
			writer.WriteLine ("\t\t\tblock_ptr_response->CleanupBlock ();");
			writer.WriteLine ("\t\t}");
			writer.WriteLine ("\t}");
		}

		void WriteIntentHandlingDefinition (TextWriter writer, string prefix)
		{
			writer.WriteLine ("\tdelegate void {0}ResponseCallback ({0}IntentResponse response);", prefix);
			writer.WriteLine ();
			WriteIntentHandlingTrampolines (writer, prefix);
			writer.WriteLine ();
			WriteIntentHandlingInterface (writer, prefix);
			writer.WriteLine ();
			WriteIntentHandlingExtensions (writer, prefix);
			writer.WriteLine ();
			WriteIntentHandlingWrapper (writer, prefix);
		}

		bool TryGenerateIntentBinding (string intermediateDir, PDictionary intentDefinition, Dictionary<string, PDictionary> enumDefinitions, out string path)
		{
			PString value;

			path = null;

			if (!intentDefinition.TryGetValue (SiriIntentsKeys.INIntentName, out value))
				return false;

			if (!intentDefinition.TryGetValue (SiriIntentsKeys.INIntentResponse, out PDictionary responseDefinition))
				return false;

			var name = value.Value;

			path = Path.Combine (intermediateDir, name + "Intent.cs");

			using (var writer = File.CreateText (path)) {
				var enums = new HashSet<string> (StringComparer.Ordinal);
				PArray parameters;

				writer.WriteLine ("using System;");
				writer.WriteLine ("using System.Runtime.InteropServices;");
				writer.WriteLine ();
				writer.WriteLine ("using Intents;");
				writer.WriteLine ("using Foundation;");
				writer.WriteLine ("using ObjCRuntime;");
				writer.WriteLine ("using CoreLocation;");
				writer.WriteLine ();
				writer.WriteLine ("namespace {0} {{", RootNamespace);

				// collect a list of enums that this class uses...
				if (intentDefinition.TryGetValue (SiriIntentsKeys.INIntentParameters, out parameters)) {
					foreach (var parameterDefinition in parameters.OfType<PDictionary> ()) {
						if (parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentParameterEnumType, out PString enumType))
							enums.Add (enumType.Value);
					}
				}

				if (responseDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseParameters, out parameters)) {
					foreach (var parameterDefinition in parameters.OfType<PDictionary> ()) {
						if (parameterDefinition.TryGetValue (SiriIntentsKeys.INIntentResponseParameterEnumType, out PString enumType))
							enums.Add (enumType.Value);
					}
				}

				// write out the enum definitions
				foreach (var enumName in enums) {
					if (enumDefinitions.TryGetValue (enumName, out PDictionary enumDefinition))
						WriteEnumDefinition (writer, name, enumDefinition);
				}

				WriteIntentDefinition (writer, name, intentDefinition);
				writer.WriteLine ();
				WriteIntentResponseDefinition (writer, name, responseDefinition);
				writer.WriteLine ();
				WriteIntentHandlingDefinition (writer, name);

				// end of namespace
				writer.WriteLine ("}");
			}

			return true;
		}

		public override bool Execute ()
		{
			var enumDefinitions = new Dictionary<string, PDictionary> (StringComparer.Ordinal);
			PDictionary plist;
			PArray intents;
			PArray enums;

			try {
				plist = PDictionary.FromFile (IntentDefinition.ItemSpec);
			} catch (Exception ex) {
				Log.LogError (7068, IntentDefinition.ItemSpec, $"Error loading '{IntentDefinition.ItemSpec}': {ex.Message}", IntentDefinition.ItemSpec);
				return false;
			}

			if (!plist.TryGetValue (SiriIntentsKeys.INIntents, out intents) || intents.Count == 0)
				return true;

			if (!plist.TryGetValue (SiriIntentsKeys.INEnums, out enums))
				enums = new PArray ();

			if (!Directory.Exists (IntermediateOutputPath))
				Directory.CreateDirectory (IntermediateOutputPath);

			var generated = new List<ITaskItem> ();

			foreach (var enumDefinition in enums.OfType<PDictionary> ()) {
				if (!enumDefinition.TryGetValue (SiriIntentsKeys.INEnumName, out PString name))
					continue;

				enumDefinitions.Add (name.Value, enumDefinition);
			}

			foreach (var intent in intents.OfType<PDictionary> ()) {
				PString category;

				if (!intent.TryGetValue (SiriIntentsKeys.INIntentCategory, out category))
					continue;

				if (category.Value == "system")
					continue;

				if (!TryGenerateIntentBinding (IntermediateOutputPath, intent, enumDefinitions, out string fileName))
					continue;

				generated.Add (new TaskItem (fileName));
			}

			GeneratedSources = generated.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
