using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;

using ObjCRuntime;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class BlittablePInvokes {
		struct MethodBlitResult {
			public MethodBlitResult (bool isBlittable)
			{
				IsBlittable = isBlittable;
				Result = new StringBuilder ();
			}
			public bool IsBlittable;
			public StringBuilder Result;
		}

		struct TypeAndIndex {
			public TypeAndIndex (TypeReference type, int index)
			{
				Type = type;
				Index = index;
			}
			public TypeReference Type;
			public int Index;
		}
		struct BlitAndReason {
			public BlitAndReason (bool isBlittable, string reason)
			{
				IsBlittable = isBlittable;
				Reason = reason;
			}
			public bool IsBlittable;
			public string Reason;
		}

		[Ignore ("work in progress - there are 6 failures, mostly due to delegates")]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformImplementationAssemblyDefinitions))]
		public void CheckForNonBlittablePInvokes (AssemblyInfo info)
		{
			var assembly = info.Assembly;
			var pinvokes = AllPInvokes (assembly).Where (IsPInvokeOK);
			Assert.IsTrue (pinvokes.Count () > 0);

			var blitCache = new Dictionary<string, BlitAndReason> ();
			var results = pinvokes.Select (pi => IsMethodBlittable (assembly, pi, blitCache)).Where (r => !r.IsBlittable);
			if (results.Count () > 0) {
				var failString = new StringBuilder ();
				failString.Append ($"There is an issue with {results.Count ()} pinvokes in {assembly.Name} ({info.Path}):\n");
				foreach (var sb in results.Select (r => r.Result)) {
					failString.Append (sb.ToString ());
				}
				Assert.Fail (failString.ToString ());
			}

		}

		MethodBlitResult IsMethodBlittable (AssemblyDefinition assembly, MethodReference method, Dictionary<string, BlitAndReason> blitCache)
		{
			var result = new MethodBlitResult (true);
			var localResult = new StringBuilder ();
			var types = TypesFromMethod (method);
			foreach (var typeIndex in types) {
				if (!IsTypeBlittable (assembly, typeIndex.Type, localResult, blitCache)) {
					if (result.IsBlittable) {
						result.IsBlittable = false;
						result.Result.Append ($"    The P/Invoke {method.FullName} has been marked as non-blittable for the following reasons:\n");
					}
					if (typeIndex.Index < 0) {
						result.Result.Append ($"        The return type is");
					} else {
						result.Result.Append ($"        Parameter index {typeIndex.Index} is");
					}
					result.Result.Append ($" {typeIndex.Type}: {localResult.ToString ()}\n");
				}
			}
			return result;
		}

		IEnumerable<TypeAndIndex> TypesFromMethod (MethodReference method)
		{
			if (method.ReturnType is not null)
				yield return new TypeAndIndex (method.ReturnType, -1);
			var i = 0;
			foreach (var parameter in method.Parameters)
				yield return new TypeAndIndex (parameter.ParameterType, i++);
		}

		bool IsTypeBlittable (AssemblyDefinition assembly, TypeReference type, StringBuilder result, Dictionary<string, BlitAndReason> blitCache)
		{
			if (blitCache.TryGetValue (type.Name, out var cachedResult)) {
				if (!cachedResult.IsBlittable)
					result.Append ($" {cachedResult.Reason}");
				return cachedResult.IsBlittable;
			}
			if (IsBlittableTypesWeLike (type)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			if (IsBlittablePointer (type)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			var localResult = new StringBuilder ();
			if (IsBlittableValueType (assembly, type, localResult, blitCache)) {
				blitCache [type.Name] = new BlitAndReason (true, "");
				return true;
			}
			result.Append (localResult);
			blitCache [type.Name] = new BlitAndReason (false, result.ToString ());
			return false;
		}


		static HashSet<string> typesWeLike = new HashSet<string> () {
			"System.Void",
			"System.IntPtr",
			"System.UIntPtr",
			"ObjCRuntime.NativeHandle",
			"System.Byte",
			"System.SByte",
			"System.Int16",
			"System.UInt16",
			"System.Int32",
			"System.UInt32",
			"System.Int64",
			"System.UInt64",
			"System.Single",
			"System.Double",
			"System.Runtime.InteropServices.NFloat",
			"System.Runtime.InteropServices.NFloat&",
		};

		bool IsBlittableTypesWeLike (TypeReference t)
		{
			return typesWeLike.Contains (t.ToString ());
		}

		bool IsBlittablePointer (TypeReference type)
		{
			return type.IsPointer || type.IsFunctionPointer;
		}


		bool IsBlittableValueType (AssemblyDefinition assembly, TypeReference type, StringBuilder result, Dictionary<string, BlitAndReason> blitCache)
		{
			TypeDefinition? typeDefinition = type.Resolve ();
			if (typeDefinition is null) {
				result.Append ($" {type.FullName}: Unable to load type.");
				return false;
			}
			if (!typeDefinition.IsValueType) {
				// handy for debugging
				// change the true to false to get more information
				// than you'll probably need about the typeDefinition
				var other = true ? "" : $"IsByReference {typeDefinition.IsByReference} IsPointer {typeDefinition.IsPointer} IsSentinel {typeDefinition.IsSentinel} IsArray {typeDefinition.IsArray} IsGenericParameter {typeDefinition.IsGenericParameter} IsRequiredModifier {typeDefinition.IsRequiredModifier} IsOptionalModifier {typeDefinition.IsOptionalModifier} IsPinned {typeDefinition.IsPinned} IsFunctionPointer {typeDefinition.IsFunctionPointer} IsPrimitive {typeDefinition.IsPrimitive}";
				result.Append ($" {type.Name}: Type is not a value type.\n{other}\n");
				return false;
			}
			if (typeDefinition.IsEnum) {
				return true;
			}
			var allBlittable = true;
			// if we get here then this is a struct. We can presume
			// that a struct will be blittable until we know otherwise
			// this will prevent infinite recursion
			blitCache [type.Name] = new BlitAndReason (true, "");
			var fieldsResult = new StringBuilder ();

			// if we're here, this is a struct
			// a struct is blittable if and only if all the
			// non-static fields are blittable.
			foreach (var f in typeDefinition.Fields) {
				if (f.IsStatic)
					continue;
				var localResult = new StringBuilder ();
				if (!IsTypeBlittable (assembly, f.FieldType, localResult, blitCache)) {
					if (!allBlittable)
						fieldsResult.Append ($" {type.Name}:");
					fieldsResult.Append ($" ({f.Name}: {localResult})");
					allBlittable = false;
				}
			}
			if (!allBlittable) {
				result.Append (fieldsResult);
				blitCache [type.Name] = new BlitAndReason (false, fieldsResult.ToString ());
			}
			return allBlittable;
		}

		IEnumerable<MethodDefinition> AllPInvokes (AssemblyDefinition assembly)
		{
			return assembly.EnumerateMethods (method =>
				(method.Attributes & MethodAttributes.PInvokeImpl) != 0);
		}

		static bool IsPInvokeOK (MethodDefinition method)
		{
			var fullName = method.FullName;
			switch (fullName) {
			default:
				return true;
			}
		}

		[Test]
		public void CheckForBlockLiterals ()
		{
			var failures = new HashSet<(string Message, string Location)> ();

			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				var assembly = info.Assembly;
				foreach (var type in assembly.EnumerateTypes ()) {
					foreach (var method in type.EnumerateMethods (m => m.HasBody)) {
						if (Skip_CheckForBlockLiterals (method))
							continue;

						var body = method.Body;
						foreach (var instr in body.Instructions) {
							switch (instr.OpCode.Code) {
							case Code.Call:
							case Code.Calli:
							case Code.Callvirt:
								break;
							default:
								continue;
							}

							var targetMethod = (MethodReference) instr.Operand;
							if (targetMethod is null) {
								Console.WriteLine ("HUH");
								continue;
							}

							if (!targetMethod.DeclaringType.Is ("ObjCRuntime", "BlockLiteral"))
								continue;

							switch (targetMethod.Name) {
							case "SetupBlock":
							case "SetupBlockUnsafe":
								break;
							default:
								continue;
							}

							var location = method.RenderLocation (instr);
							var message = $"The call to {targetMethod.Name} in {method.AsFullName ()} must be converted to new Block syntax.";
							failures.Add (new (message, location));
						}
					}
				}
			}

			var newFailures = failures.Where (v => !knownFailuresBlockLiterals.Contains (v.Message)).ToArray ();
			var fixedFailures = knownFailuresBlockLiterals.Except (failures.Select (v => v.Message).ToHashSet ());

			var printKnownFailures = newFailures.Any () || fixedFailures.Any ();
			if (printKnownFailures) {
				Console.WriteLine ("Printing all failures as known failures because they seem out of date:");
				Console.WriteLine ("\t\tstatic HashSet<string> knownFailuresBlockLiterals = new HashSet<string> {");
				foreach (var failure in failures.OrderBy (v => v))
					Console.WriteLine ($"\t\t\t\"{failure.Message}\",");
				Console.WriteLine ("\t\t};");
			}

			if (newFailures.Any ()) {
				// Print any new failures with the local path for easy navigation (depending on the terminal and/or IDE you might just click on the path to open the corresponding file).
				Console.WriteLine ($"Printing {newFailures.Count ()} new failures with local paths for easy navigation:");
				foreach (var failure in newFailures.OrderBy (v => v))
					Console.WriteLine ($"    {failure.Location}: {failure.Message}");
			}

			Assert.IsEmpty (newFailures, "Failures");
			Assert.IsEmpty (fixedFailures, "Known failures that aren't failing anymore - remove these from the list of known failures");
		}

		static HashSet<string> knownFailuresBlockLiterals = new HashSet<string> {
			"The call to SetupBlock in ObjCRuntime.BlockLiteral.GetBlockForDelegate(System.Reflection.MethodInfo, System.Object, System.UInt32, System.String) must be converted to new Block syntax.",
			"The call to SetupBlock in ObjCRuntime.BlockLiteral.SetupBlock(System.Delegate, System.Delegate) must be converted to new Block syntax.",
			"The call to SetupBlock in ObjCRuntime.BlockLiteral.SetupBlockUnsafe(System.Delegate, System.Delegate) must be converted to new Block syntax.",
			"The call to SetupBlock in UIKit.UIAccessibility.RequestGuidedAccessSession(System.Boolean, System.Action`1<System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in AddressBook.ABAddressBook.RequestAccess(System.Action`2<System.Boolean,Foundation.NSError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in AudioToolbox.SystemSound.PlayAlertSound(System.Action) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in AudioToolbox.SystemSound.PlaySystemSound(System.Action) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in AudioUnit.AudioComponent.ValidateAsync(Foundation.NSDictionary, System.Action`2<AudioUnit.AudioComponentValidationResult,Foundation.NSDictionary>, out System.Int32&) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreFoundation.DispatchBlock.create(CoreFoundation.DispatchBlockFlags, CoreFoundation.DispatchQualityOfService, System.Int32, System.Action) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreFoundation.DispatchBlock.create(System.Action, CoreFoundation.DispatchBlockFlags) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreFoundation.DispatchBlock.Invoke(System.Action, System.Action`1<System.IntPtr>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreFoundation.DispatchIO.Read(System.Int32, System.UIntPtr, CoreFoundation.DispatchQueue, CoreFoundation.DispatchIOHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreFoundation.DispatchIO.Write(System.Int32, CoreFoundation.DispatchData, CoreFoundation.DispatchQueue, CoreFoundation.DispatchIOHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreGraphics.CGPDFArray.Apply(CoreGraphics.CGPDFArray/ApplyCallback, System.Object) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreText.CTFontManager.RegisterFontDescriptors(CoreText.CTFontDescriptor[], CoreText.CTFontManagerScope, System.Boolean, CoreText.CTFontManager/CTFontRegistrationHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreText.CTFontManager.RegisterFonts(Foundation.NSUrl[], CoreText.CTFontManagerScope, System.Boolean, CoreText.CTFontManager/CTFontRegistrationHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreText.CTFontManager.RegisterFonts(System.String[], CoreFoundation.CFBundle, CoreText.CTFontManagerScope, System.Boolean, CoreText.CTFontManager/CTFontRegistrationHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreText.CTFontManager.RequestFonts(CoreText.CTFontDescriptor[], CoreText.CTFontManager/CTFontManagerRequestFontsHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreText.CTFontManager.UnregisterFontDescriptors(CoreText.CTFontDescriptor[], CoreText.CTFontManagerScope, CoreText.CTFontManager/CTFontRegistrationHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreText.CTFontManager.UnregisterFonts(Foundation.NSUrl[], CoreText.CTFontManagerScope, CoreText.CTFontManager/CTFontRegistrationHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in CoreText.CTLine.EnumerateCaretOffsets(CoreText.CTLine/CaretEdgeEnumerator) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in ImageIO.CGImageAnimation.AnimateImage(Foundation.NSData, ImageIO.CGImageAnimationOptions, ImageIO.CGImageAnimation/CGImageSourceAnimationHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in ImageIO.CGImageAnimation.AnimateImage(Foundation.NSUrl, ImageIO.CGImageAnimationOptions, ImageIO.CGImageAnimation/CGImageSourceAnimationHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Metal.MTLDevice.GetAllDevices(Metal.MTLDeviceNotificationHandler, out Foundation.NSObject&) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWBrowser.SetChangesHandler(Network.NWBrowserChangesDelegate) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWBrowser.SetStateChangesHandler(System.Action`2<Network.NWBrowserState,Network.NWError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWBrowseResult.EnumerateInterfaces(System.Action`1<Network.NWInterface>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.GetEstablishmentReport(CoreFoundation.DispatchQueue, System.Action`1<Network.NWEstablishmentReport>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.Receive(System.UInt32, System.UInt32, Network.NWConnectionReceiveCompletion) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.ReceiveData(System.UInt32, System.UInt32, Network.NWConnectionReceiveDispatchDataCompletion) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.ReceiveMessage(Network.NWConnectionReceiveCompletion) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.ReceiveMessageData(Network.NWConnectionReceiveDispatchDataCompletion) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.ReceiveMessageReadOnlyData(Network.NWConnectionReceiveReadOnlySpanCompletion) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.ReceiveReadOnlyData(System.UInt32, System.UInt32, Network.NWConnectionReceiveReadOnlySpanCompletion) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.Send(CoreFoundation.DispatchData, Network.NWContentContext, System.Boolean, System.Action`1<Network.NWError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.SetBetterPathAvailableHandler(System.Action`1<System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.SetBooleanChangeHandler(System.Action`1<System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.SetPathChangedHandler(System.Action`1<Network.NWPath>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnection.SetStateChangeHandler(System.Action`2<Network.NWConnectionState,Network.NWError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnectionGroup.Send(CoreFoundation.DispatchData, Network.NWEndpoint, Network.NWContentContext, System.Action`1<Network.NWError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnectionGroup.SetNewConnectionHandler(System.Action`1<Network.NWConnection>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnectionGroup.SetReceiveHandler(System.UInt32, System.Boolean, Network.NWConnectionGroupReceiveDelegate) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWConnectionGroup.SetStateChangedHandler(Network.NWConnectionGroupStateChangedDelegate) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWContentContext.IterateProtocolMetadata(System.Action`2<Network.NWProtocolDefinition,Network.NWProtocolMetadata>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWDataTransferReport.Collect(CoreFoundation.DispatchQueue, System.Action`1<Network.NWDataTransferReport>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWEstablishmentReport.EnumerateProtocols(System.Action`3<Network.NWProtocolDefinition,System.TimeSpan,System.TimeSpan>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWEstablishmentReport.EnumerateResolutionReports(System.Action`1<Network.NWResolutionReport>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWEstablishmentReport.EnumerateResolutions(System.Action`5<Network.NWReportResolutionSource,System.TimeSpan,System.Int32,Network.NWEndpoint,Network.NWEndpoint>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWEthernetChannel.Send(System.ReadOnlySpan`1<System.Byte>, System.UInt16, System.String, System.Action`1<Network.NWError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWEthernetChannel.SetReceiveHandler(Network.NWEthernetChannelReceiveDelegate) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWEthernetChannel.SetStateChangesHandler(System.Action`2<Network.NWBrowserState,Network.NWError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramer.ParseInput(System.UIntPtr, System.UIntPtr, System.Memory`1<System.Byte>, Network.NWFramerParseCompletionDelegate) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramer.ParseOutput(System.UIntPtr, System.UIntPtr, System.Memory`1<System.Byte>, System.Action`2<System.Memory`1<System.Byte>,System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramer.ScheduleAsync(System.Action) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramer.set_CleanupHandler(System.Action`1<Network.NWFramer>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramer.set_InputHandler(Network.NWFramerInputDelegate) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramer.set_OutputHandler(System.Action`4<Network.NWFramer,Network.NWFramerMessage,System.UIntPtr,System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramer.set_StopHandler(System.Action`1<Network.NWFramer>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramer.set_WakeupHandler(System.Action`1<Network.NWFramer>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramerMessage.GetData(System.String, System.Int32, out System.ReadOnlySpan`1<System.Byte>&) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWFramerMessage.SetData(System.String, System.Byte[]) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWListener.SetAdvertisedEndpointChangedHandler(Network.NWListener/AdvertisedEndpointChanged) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWListener.SetNewConnectionGroupHandler(System.Action`1<Network.NWConnectionGroup>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWListener.SetNewConnectionHandler(System.Action`1<Network.NWConnection>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWListener.SetStateChangedHandler(System.Action`2<Network.NWListenerState,Network.NWError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWMulticastGroup.EnumerateEndpoints(System.Func`2<Network.NWEndpoint,System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.CreateCustomIP(System.Byte, System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.CreateQuic(System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.CreateSecureTcp(System.Action`1<Network.NWProtocolOptions>, System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.CreateSecureTcp(System.Action`1<Network.NWProtocolOptions>, System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.CreateSecureUdp(System.Action`1<Network.NWProtocolOptions>, System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.CreateSecureUdp(System.Action`1<Network.NWProtocolOptions>, System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.CreateTcp(System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.CreateUdp(System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.IterateProhibitedInterfaces(System.Func`2<Network.NWInterface,System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWParameters.IterateProhibitedInterfaces(System.Func`2<Network.NWInterfaceType,System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWPath.EnumerateGateways(System.Func`2<Network.NWEndpoint,System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWPath.EnumerateInterfaces(System.Func`2<Network.NWInterface,System.Boolean>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWPathMonitor._SetUpdatedSnapshotHandler(System.Action`1<Network.NWPath>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWPathMonitor.SetMonitorCanceledHandler(System.Action) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWProtocolDefinition.CreateFramerDefinition(System.String, Network.NWFramerCreateFlags, System.Func`2<Network.NWFramer,Network.NWFramerStartResult>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWProtocolStack.IterateProtocols(System.Action`1<Network.NWProtocolOptions>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWTxtRecord.Apply(Network.NWTxtRecord/NWTxtRecordApplyDelegate) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWTxtRecord.GetRawBytes(Network.NWTxtRecord/NWTxtRecordGetRawByteDelegate) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWTxtRecord.GetValue(System.String, Network.NWTxtRecord/NWTxtRecordGetValueDelegete) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWWebSocketMetadata.SetPongHandler(CoreFoundation.DispatchQueue, System.Action`1<Network.NWError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWWebSocketOptions.SetClientRequestHandler(CoreFoundation.DispatchQueue, System.Action`1<Network.NWWebSocketRequest>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWWebSocketRequest.EnumerateAdditionalHeaders(System.Action`2<System.String,System.String>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWWebSocketRequest.EnumerateSubprotocols(System.Action`1<System.String>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Network.NWWebSocketResponse.EnumerateAdditionalHeaders(System.Action`2<System.String,System.String>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in ObjCRuntime.BlockLiteral.SimpleCall(System.Action, System.Action`1<System.IntPtr>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecIdentity2.AccessCertificates(System.Action`1<Security.SecCertificate2>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecProtocolMetadata.AccessPreSharedKeys(Security.SecProtocolMetadata/SecAccessPreSharedKeysHandler) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecProtocolMetadata.SetCertificateChainForPeerHandler(System.Action`1<Security.SecCertificate>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecProtocolMetadata.SetDistinguishedNamesForPeerHandler(System.Action`1<CoreFoundation.DispatchData>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecProtocolMetadata.SetOcspResponseForPeerHandler(System.Action`1<CoreFoundation.DispatchData>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecProtocolMetadata.SetSignatureAlgorithmsForPeerHandler(System.Action`1<System.UInt16>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecProtocolOptions.SetKeyUpdateCallback(Security.SecProtocolKeyUpdate, CoreFoundation.DispatchQueue) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecSharedCredential.AddSharedWebCredential(System.String, System.String, System.String, System.Action`1<Foundation.NSError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecSharedCredential.RequestSharedWebCredential(System.String, System.String, System.Action`2<Security.SecSharedCredentialInfo[],Foundation.NSError>) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecTrust.Evaluate(CoreFoundation.DispatchQueue, Security.SecTrustCallback) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in Security.SecTrust.Evaluate(CoreFoundation.DispatchQueue, Security.SecTrustWithErrorCallback) must be converted to new Block syntax.",
			"The call to SetupBlockUnsafe in UIKit.UIGuidedAccessRestriction.ConfigureAccessibilityFeatures(UIKit.UIGuidedAccessAccessibilityFeature, System.Boolean, UIKit.UIGuidedAccessRestriction/UIGuidedAccessConfigureAccessibilityFeaturesCompletionHandler) must be converted to new Block syntax.",
		};

		bool Skip_CheckForBlockLiterals (MethodDefinition method)
		{
			// Skipping generated files, the generator needs to be fixed first.
			if (method.HasBindingImplAttribute (out var implOptions) && (implOptions & BindingImplOptions.GeneratedCode) == BindingImplOptions.GeneratedCode)
				return true;

			return false;
		}


		[Test]
		public void CheckForMonoPInvokeCallback ()
		{
			var failures = new HashSet<(string Message, string Location)> ();

			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				var assembly = info.Assembly;
				foreach (var type in assembly.EnumerateTypes ()) {
					foreach (var method in type.EnumerateMethods (m => m.HasCustomAttributes)) {
						if (Skip_CheckForMonoPInvokeCallback (method))
							continue;

						foreach (var ca in method.CustomAttributes) {
							if (ca.AttributeType.Name != "MonoPInvokeCallbackAttribute")
								continue;

							var location = method.RenderLocation ();
							var message = $"The method {method.AsFullName ()} has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.";
							failures.Add (new (message, location));

							break;
						}
					}
				}
			}

			var newFailures = failures.Where (v => !knownFailuresMonoPInvokeCallback.Contains (v.Message)).ToArray ();
			var fixedFailures = knownFailuresMonoPInvokeCallback.Except (failures.Select (v => v.Message).ToHashSet ());

			var printKnownFailures = newFailures.Any () || fixedFailures.Any ();
			if (printKnownFailures) {
				Console.WriteLine ("Printing all failures as known failures because they seem out of date:");
				Console.WriteLine ("\t\tstatic HashSet<string> knownFailuresMonoPInvokeCallback = new HashSet<string> {");
				foreach (var failure in failures.OrderBy (v => v))
					Console.WriteLine ($"\t\t\t\"{failure.Message}\",");
				Console.WriteLine ("\t\t};");
			}

			if (newFailures.Any ()) {
				// Print any new failures with the local path for easy navigation (depending on the terminal and/or IDE you might just click on the path to open the corresponding file).
				Console.WriteLine ($"Printing {newFailures.Count ()} new failures with local paths for easy navigation:");
				foreach (var failure in newFailures.OrderBy (v => v))
					Console.WriteLine ($"    {failure.Location}: {failure.Message}");
			}

			Assert.IsEmpty (newFailures, "Failures");
			Assert.IsEmpty (fixedFailures, "Known failures that aren't failing anymore - remove these from the list of known failures");
		}

		static HashSet<string> knownFailuresMonoPInvokeCallback = new HashSet<string> {
			"The method AddressBook.ABAddressBook.TrampolineCompletionHandler(System.IntPtr, System.Boolean, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method AudioToolbox.SystemSound.TrampolineAction(System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method AudioUnit.AudioComponent.TrampolineAction(System.IntPtr, AudioUnit.AudioComponentValidationResult, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method CoreFoundation.CFStream.OnCallback(System.IntPtr, System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method CoreFoundation.DispatchIO.Trampoline_DispatchReadWriteHandler(System.IntPtr, System.IntPtr, System.Int32) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method CoreGraphics.CGPDFArray.ApplyBlockHandler(System.IntPtr, System.IntPtr, System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method CoreText.CTFontManager.TrampolineRegistrationHandler(System.IntPtr, System.IntPtr, System.Boolean) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method CoreText.CTFontManager.TrampolineRequestFonts(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method CoreText.CTLine.TrampolineEnumerate(System.IntPtr, System.Double, System.IntPtr, System.Boolean, System.Boolean&) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method ImageIO.CGImageAnimation/SDCGImageSourceAnimationBlock.Invoke(System.IntPtr, System.IntPtr, System.IntPtr, out System.Boolean&) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Metal.MTLDevice.TrampolineNotificationHandler(System.IntPtr, System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWBrowser.TrampolineChangesHandler(System.IntPtr, System.IntPtr, System.IntPtr, System.Boolean) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWBrowser.TrampolineStateChangesHandler(System.IntPtr, Network.NWBrowserState, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWBrowseResult.TrampolineEnumerateInterfacesHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnection.Trampoline_StateChangeCallback(System.IntPtr, Network.NWConnectionState, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnection.TrampolineBooleanChangeHandler(System.IntPtr, System.Boolean) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnection.TrampolineGetEstablishmentReportHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnection.TrampolinePathChanged(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnection.TrampolineReceiveCompletion(System.IntPtr, System.IntPtr, System.IntPtr, System.Boolean, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnection.TrampolineReceiveCompletionData(System.IntPtr, System.IntPtr, System.IntPtr, System.Boolean, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnection.TrampolineReceiveCompletionReadOnlyData(System.IntPtr, System.IntPtr, System.IntPtr, System.Boolean, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnection.TrampolineSendCompletion(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnectionGroup.TrampolineReceiveHandler(System.IntPtr, System.IntPtr, System.IntPtr, System.Boolean) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnectionGroup.TrampolineSendCompletion(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnectionGroup.TrampolineSetNewConnectionHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWConnectionGroup.TrampolineStateChangedHandler(System.IntPtr, Network.NWConnectionGroupState, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWContentContext.TrampolineProtocolIterator(System.IntPtr, System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWDataTransferReport.TrampolineCollectHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWEstablishmentReport.TrampolineEnumerateProtocolsHandler(System.IntPtr, System.IntPtr, System.UIntPtr, System.UIntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWEstablishmentReport.TrampolineEnumerateResolutionReport(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWEstablishmentReport.TrampolineResolutionEnumeratorHandler(System.IntPtr, Network.NWReportResolutionSource, System.UIntPtr, System.Int32, System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWEthernetChannel.TrampolineReceiveHandler(System.IntPtr, System.IntPtr, System.UInt16, System.Byte[], System.Byte[]) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWEthernetChannel.TrampolineSendCompletion(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWEthernetChannel.TrampolineStateChangesHandler(System.IntPtr, Network.NWEthernetChannelState, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramer.TrampolineCleanupHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramer.TrampolineInputHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramer.TrampolineOutputHandler(System.IntPtr, System.IntPtr, System.IntPtr, System.UIntPtr, System.Boolean) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramer.TrampolineParseInputHandler(System.IntPtr, System.IntPtr, System.UIntPtr, System.Boolean) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramer.TrampolineParseOutputHandler(System.IntPtr, System.IntPtr, System.UIntPtr, System.Boolean) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramer.TrampolineScheduleHandler(System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramer.TrampolineStopHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramer.TrampolineWakeupHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramerMessage.TrampolineAccessValueHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWFramerMessage.TrampolineSetDataHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWListener.TrampolineAdvertisedEndpointChangedHandler(System.IntPtr, System.IntPtr, System.Byte) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWListener.TrampolineListenerStateChanged(System.IntPtr, Network.NWListenerState, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWListener.TrampolineNewConnection(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWListener.TrampolineNewConnectionGroup(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWMulticastGroup.TrampolineEnumerateEndpointsHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWParameters.TrampolineConfigureHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWParameters.TrampolineIterateProhibitedHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWParameters.TrampolineIterateProhibitedTypeHandler(System.IntPtr, Network.NWInterfaceType) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWPath.TrampolineEnumerator(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWPath.TrampolineGatewaysHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWPathMonitor.TrampolineMonitorCanceled(System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWPathMonitor.TrampolineUpdatedSnapshot(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWProtocolDefinition.TrampolineCreateFramerHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWProtocolStack.TrampolineIterateHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWTxtRecord.TrampolineAccessKeyHandler(System.IntPtr, System.String, Network.NWTxtRecordFindKey, System.IntPtr, System.UIntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWTxtRecord.TrampolineApplyHandler(System.IntPtr, System.String, Network.NWTxtRecordFindKey, System.IntPtr, System.UIntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWTxtRecord.TrampolineRawBytesHandler(System.IntPtr, System.IntPtr, System.UIntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWWebSocketMetadata.TrampolinePongHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWWebSocketOptions.TrampolineClientRequestHandler(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWWebSocketRequest.TrampolineEnumerateHeaderHandler(System.IntPtr, System.String, System.String) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWWebSocketRequest.TrampolineEnumerateSubprotocolHandler(System.IntPtr, System.String) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Network.NWWebSocketResponse.TrampolineEnumerateHeadersHandler(System.IntPtr, System.String, System.String) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method ObjCRuntime.BlockStaticDispatchClass.TrampolineDispatchBlock(System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecIdentity2.TrampolineAccessCertificates(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecProtocolMetadata.TrampolineAccessPreSharedKeys(System.IntPtr, System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecProtocolMetadata.TrampolineCertificateChainForPeer(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecProtocolMetadata.TrampolineDistinguishedNamesForPeer(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecProtocolMetadata.TrampolineOcspReposeForPeer(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecProtocolMetadata.TrampolineSignatureAlgorithmsForPeer(System.IntPtr, System.UInt16) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecSharedCredential/ActionTrampoline.Invoke(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecSharedCredential/ArrayErrorActionTrampoline.Invoke(System.IntPtr, System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecTrust.TrampolineEvaluate(System.IntPtr, System.IntPtr, Security.SecTrustResult) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method Security.SecTrust.TrampolineEvaluateError(System.IntPtr, System.IntPtr, System.Boolean, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method UIKit.SDUICellAccessoryPosition.Invoke(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method UIKit.SDUIConfigurationColorTransformerHandler.Invoke(System.IntPtr, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method UIKit.UIAccessibility.TrampolineRequestGuidedAccessSession(System.IntPtr, System.Boolean) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
			"The method UIKit.UIGuidedAccessRestriction/UIGuidedAccessConfigureAccessibilityFeaturesTrampoline.Invoke(System.IntPtr, System.Boolean, System.IntPtr) has a MonoPInvokeCallback attribute and must be converted to a UnmanagedCallersOnly method.",
		};

		bool Skip_CheckForMonoPInvokeCallback (MethodDefinition method)
		{
			// Skipping generated files, the generator needs to be fixed first.
			if (method.HasBindingImplAttribute (out var implOptions) && (implOptions & BindingImplOptions.GeneratedCode) == BindingImplOptions.GeneratedCode)
				return true;
			// ObjCRuntime.Trampolines is a generated type, even though it doesn't have a BindingImpl attribute.
			if (method.DeclaringType.IsNested && method.DeclaringType.DeclaringType.Is ("ObjCRuntime", "Trampolines"))
				return true;

			return false;
		}
	}
}
