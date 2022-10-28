//
// CXCompat.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

using System;
using System.Threading.Tasks;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

#if !NET && MONOMAC
#nullable enable
namespace CallKit {

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXCallController", SkipRegistration = true)]
	public class CXCallController : NSObject {

		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		
		public CXCallController () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXCallController (NSObjectFlag t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXCallController (IntPtr handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXCallController (DispatchQueue queue) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual void RequestTransaction (CXTransaction transaction, Action<NSError> completion) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual Task RequestTransactionAsync (CXTransaction transaction) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void RequestTransaction (CXAction[] actions, Action<NSError> completion) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual Task RequestTransactionAsync (CXAction[] actions) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void RequestTransaction (CXAction action, Action<NSError> completion) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual Task RequestTransactionAsync (CXAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual CXCallObserver CallObserver => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

	} /* class CXCallController */

	[Obsolete (Constants.UnavailableOnMacOS)]
	public interface ICXCallObserverDelegate : INativeObject, IDisposable {
		void CallChanged (CXCallObserver callObserver, CXCall call) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* interface ICXCallObserverDelegate */

	[Obsolete (Constants.UnavailableOnMacOS)]
	internal sealed class CXCallObserverDelegateWrapper : BaseWrapper, ICXCallObserverDelegate {
		public CXCallObserverDelegateWrapper (IntPtr handle, bool owns) : base (handle, owns) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public void CallChanged (CXCallObserver callObserver, CXCall call) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXCallObserverDelegateWrapper */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXCallObserverDelegate", SkipRegistration = true)]
	public abstract class CXCallObserverDelegate : NSObject, ICXCallObserverDelegate {

		protected CXCallObserverDelegate () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXCallObserverDelegate (NSObjectFlag t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXCallObserverDelegate (IntPtr handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public abstract void CallChanged (CXCallObserver callObserver, CXCall call);
	} /* class CXCallObserverDelegate */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXCallObserver", SkipRegistration = true)]
	public class CXCallObserver : NSObject {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public CXCallObserver () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXCallObserver (NSObjectFlag t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXCallObserver (IntPtr handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual void SetDelegate (ICXCallObserverDelegate? aDelegate, DispatchQueue? queue) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual CXCall[] Calls => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

	} /* class CXCallObserver */

	[Obsolete (Constants.UnavailableOnMacOS)]
	public interface ICXProviderDelegate : INativeObject, IDisposable {
		void DidReset (CXProvider provider) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* interface ICXProviderDelegate */

	[Obsolete (Constants.UnavailableOnMacOS)]
	public static class CXProviderDelegate_Extensions {
		public static void DidBegin (this ICXProviderDelegate This, CXProvider provider) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static bool ExecuteTransaction (this ICXProviderDelegate This, CXProvider provider, CXTransaction transaction) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static void PerformStartCallAction (this ICXProviderDelegate This, CXProvider provider, CXStartCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static void PerformAnswerCallAction (this ICXProviderDelegate This, CXProvider provider, CXAnswerCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static void PerformEndCallAction (this ICXProviderDelegate This, CXProvider provider, CXEndCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static void PerformSetHeldCallAction (this ICXProviderDelegate This, CXProvider provider, CXSetHeldCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static void PerformSetMutedCallAction (this ICXProviderDelegate This, CXProvider provider, CXSetMutedCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static void PerformSetGroupCallAction (this ICXProviderDelegate This, CXProvider provider, CXSetGroupCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static void PerformPlayDtmfCallAction (this ICXProviderDelegate This, CXProvider provider, CXPlayDtmfCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public static void TimedOutPerformingAction (this ICXProviderDelegate This, CXProvider provider, CXAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXProviderDelegate_Extensions */

	[Obsolete (Constants.UnavailableOnMacOS)]
	internal sealed class CXProviderDelegateWrapper : BaseWrapper, ICXProviderDelegate {
		public CXProviderDelegateWrapper (IntPtr handle, bool owns) : base (handle, owns) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public void DidReset (CXProvider provider) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXProviderDelegateWrapper */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXProviderDelegate", SkipRegistration = true)]
	public abstract class CXProviderDelegate : NSObject, ICXProviderDelegate {
		protected CXProviderDelegate () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXProviderDelegate (NSObjectFlag t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXProviderDelegate (IntPtr handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual void DidBegin (CXProvider provider) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public abstract void DidReset (CXProvider provider);
		public virtual bool ExecuteTransaction (CXProvider provider, CXTransaction transaction) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void PerformAnswerCallAction (CXProvider provider, CXAnswerCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void PerformEndCallAction (CXProvider provider, CXEndCallAction action)=> throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void PerformPlayDtmfCallAction (CXProvider provider, CXPlayDtmfCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void PerformSetGroupCallAction (CXProvider provider, CXSetGroupCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void PerformSetHeldCallAction (CXProvider provider, CXSetHeldCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void PerformSetMutedCallAction (CXProvider provider, CXSetMutedCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void PerformStartCallAction (CXProvider provider, CXStartCallAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void TimedOutPerformingAction (CXProvider provider, CXAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXProviderDelegate */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXProvider", SkipRegistration = true)]
	public class CXProvider : NSObject {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXProvider (NSObjectFlag t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXProvider (IntPtr handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXProvider (CXProviderConfiguration configuration) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual CXCallAction[] GetPendingCallActions (Class callActionClass, NSUuid callUuid) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual CXCallAction[] GetPendingCallActions<T> (NSUuid callUuid) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void Invalidate () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void ReportCall (NSUuid uuid, CXCallUpdate update) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void ReportCall (NSUuid uuid, NSDate? dateEnded, CXCallEndedReason endedReason) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void ReportConnectedOutgoingCall (NSUuid uuid, NSDate? dateConnected) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void ReportConnectingOutgoingCall (NSUuid uuid, NSDate? dateStartedConnecting) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void ReportNewIncomingCall (NSUuid uuid, CXCallUpdate update, Action<NSError> completion) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual Task ReportNewIncomingCallAsync (NSUuid uuid, CXCallUpdate update) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void SetDelegate (ICXProviderDelegate? aDelegate, DispatchQueue? queue) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual CXTransaction[] PendingTransactions => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual CXProviderConfiguration Configuration {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
	} /* class CXProvider */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXAction", SkipRegistration = true)]
	public class CXAction : NSObject, INSCoding, INSCopying, INSSecureCoding {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXAction (NSObjectFlag t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXAction (IntPtr handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXAction () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual NSObject Copy (NSZone? zone) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void EncodeTo (NSCoder encoder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void Fail () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void Fulfill () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual bool Complete => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual NSDate TimeoutDate => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual NSUuid Uuid => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXAnswerCallAction", SkipRegistration = true)]
	public class CXAnswerCallAction : CXCallAction {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXAnswerCallAction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXAnswerCallAction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXAnswerCallAction (NSUuid callUuid) : base (callUuid) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXAnswerCallAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual void Fulfill (NSDate dateConnected) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXAnswerCallAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXCall", SkipRegistration = true)]
	public class CXCall : NSObject {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXCall (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXCall (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual bool IsEqual (CXCall call) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual bool HasConnected => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual bool HasEnded => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual bool OnHold => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual bool Outgoing => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual NSUuid Uuid => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXCall */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXCallAction", SkipRegistration = true)]
	public class CXCallAction : CXAction {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXCallAction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXCallAction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXCallAction () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXCallAction (NSUuid callUuid) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXCallAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual NSUuid CallUuid => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXCallAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXCallUpdate", SkipRegistration = true)]
	public class CXCallUpdate : NSObject, INSCopying {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXCallUpdate (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXCallUpdate (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXCallUpdate (NSUuid callUuid) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXCallUpdate () => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual NSObject Copy (NSZone? zone) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual bool HasVideo {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual string? LocalizedCallerName {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual CXHandle? RemoteHandle {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual bool SupportsDtmf {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual bool SupportsGrouping {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual bool SupportsHolding {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual bool SupportsUngrouping {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
	} /* class CXCallUpdate */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXEndCallAction", SkipRegistration = true)]
	public class CXEndCallAction : CXCallAction {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXEndCallAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXEndCallAction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXEndCallAction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXEndCallAction (NSUuid callUuid) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual void Fulfill (NSDate dateEnded) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXEndCallAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXHandle", SkipRegistration = true)]
	public class CXHandle : NSObject, INSCoding, INSCopying, INSSecureCoding {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXHandle (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXHandle (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXHandle (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXHandle (CXHandleType type, string value) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual NSObject Copy (NSZone? zone) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void EncodeTo (NSCoder encoder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual bool IsEqual (CXHandle handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual CXHandleType Type => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual string Value => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXHandle */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXPlayDTMFCallAction", SkipRegistration = true)]
	public class CXPlayDtmfCallAction : CXCallAction {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXPlayDtmfCallAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXPlayDtmfCallAction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXPlayDtmfCallAction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXPlayDtmfCallAction (NSUuid callUuid, string digits, CXPlayDtmfCallActionType type) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual string Digits {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual CXPlayDtmfCallActionType Type {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
	} /* class CXPlayDtmfCallAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXSetGroupCallAction", SkipRegistration = true)]
	public class CXSetGroupCallAction : CXCallAction {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXSetGroupCallAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXSetGroupCallAction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXSetGroupCallAction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXSetGroupCallAction (NSUuid callUuid, NSUuid? callUuidToGroupWith) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual NSUuid? CallUuidToGroupWith {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
	} /* class CXSetGroupCallAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXSetHeldCallAction", SkipRegistration = true)]
	public class CXSetHeldCallAction : CXCallAction {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXSetHeldCallAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXSetHeldCallAction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXSetHeldCallAction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXSetHeldCallAction (NSUuid callUuid, bool onHold) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual bool OnHold {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
	} /* class CXSetHeldCallAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXSetMutedCallAction", SkipRegistration = true)]
	public class CXSetMutedCallAction : CXCallAction {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXSetMutedCallAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXSetMutedCallAction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXSetMutedCallAction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXSetMutedCallAction (NSUuid callUuid, bool muted) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual bool Muted {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
	} /* class CXSetMutedCallAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXStartCallAction", SkipRegistration = true)]
	public class CXStartCallAction : CXCallAction {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXStartCallAction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXStartCallAction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXStartCallAction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXStartCallAction (NSUuid callUuid, CXHandle callHandle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual void Fulfill (NSDate dateStarted) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual CXHandle CallHandle {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual string? ContactIdentifier {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
		public virtual bool Video {
			get => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
			set => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		}
	} /* class CXStartCallAction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Register ("CXTransaction", SkipRegistration = true)]
	public class CXTransaction : NSObject, INSCoding, INSCopying, INSSecureCoding {
		public override IntPtr ClassHandle => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXTransaction (NSCoder coder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected CXTransaction (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		protected internal CXTransaction (IntPtr handle) : base (handle) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXTransaction (CXAction[] actions) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public CXTransaction (CXAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);

		public virtual void AddAction (CXAction action) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual NSObject Copy (NSZone? zone) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual void EncodeTo (NSCoder encoder) => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual CXAction[] Actions => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual bool Complete => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
		public virtual NSUuid Uuid => throw new PlatformNotSupportedException (Constants.UnavailableOnMacOS);
	} /* class CXTransaction */

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Native]
	public enum CXErrorCode : long {
		Unknown = 0,
		Unentitled = 1,
		InvalidArgument = 2,
		MissingVoIPBackgroundMode = 3,
	}

	[Obsolete (Constants.UnavailableOnMacOS)]
	public static class CXErrorCodeExtensions {
		public static NSString? GetDomain (this CXErrorCode self) => throw new PlatformNotSupportedException (Constants.UnavailableOnThisPlatform);
	}

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Native]
	public enum CXErrorCodeIncomingCallError : long {
		Unknown = 0,
		Unentitled = 1,
		CallUuidAlreadyExists = 2,
		FilteredByDoNotDisturb = 3,
		FilteredByBlockList = 4
	}

	[Obsolete (Constants.UnavailableOnMacOS)]
	public static class CXErrorCodeIncomingCallErrorExtensions {
		public static NSString? GetDomain (this CXErrorCodeIncomingCallError self) => throw new PlatformNotSupportedException (Constants.UnavailableOnThisPlatform);
	}

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Native]
	public enum CXErrorCodeRequestTransactionError : long {
		Unknown = 0,
		Unentitled = 1,
		UnknownCallProvider = 2,
		EmptyTransaction = 3,
		UnknownCallUuid = 4,
		CallUuidAlreadyExists = 5,
		InvalidAction = 6,
		MaximumCallGroupsReached = 7,
	}

	[Obsolete (Constants.UnavailableOnMacOS)]
	public static class CXErrorCodeRequestTransactionErrorExtensions {
		public static NSString? GetDomain (this CXErrorCodeRequestTransactionError self) => throw new PlatformNotSupportedException (Constants.UnavailableOnThisPlatform);
	}

	[Obsolete (Constants.UnavailableOnMacOS)]
	[Native]
	public enum CXErrorCodeCallDirectoryManagerError : long {
		Unknown = 0,
		NoExtensionFound = 1,
		LoadingInterrupted = 2,
		EntriesOutOfOrder = 3,
		DuplicateEntries = 4,
		MaximumEntriesExceeded = 5,
		ExtensionDisabled = 6,
		CurrentlyLoading = 7,
		UnexpectedIncrementalRemoval = 8,
	}

	[Obsolete (Constants.UnavailableOnMacOS)]
	public static class CXErrorCodeCallDirectoryManagerErrorExtensions {
		public static NSString? GetDomain (this CXErrorCodeCallDirectoryManagerError self) => throw new PlatformNotSupportedException (Constants.UnavailableOnThisPlatform);
	} 
}

#endif // !NET && MONOMAC
