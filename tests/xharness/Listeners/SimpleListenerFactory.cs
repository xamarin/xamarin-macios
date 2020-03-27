using System;
using Xharness.Logging;

namespace Xharness.Listeners {
	public enum ListenerTransport {
		Tcp,
		Http,
		File,
	}

	public interface ISimpleListenerFactory {
		ITunnelBore TunnelBore { get; }
		(ListenerTransport transport, ISimpleListener listener, string listenerTempFile) Create (string device,
																								 RunMode mode,
																								 ILog log,
																								 ILog listenerLog,
																								 bool isSimulator,
																								 bool autoExit,
																								 bool xmlOutput,
																								 bool useTcpTunnel);
	}

	public class SimpleListenerFactory : ISimpleListenerFactory {
		public ITunnelBore TunnelBore { get; private set; }

		public SimpleListenerFactory (ITunnelBore tunnelBore)
		{
			TunnelBore = tunnelBore ?? throw new ArgumentNullException (nameof (tunnelBore));
		}

		public (ListenerTransport transport, ISimpleListener listener, string listenerTempFile) Create (string device,
																									    RunMode mode,
																									    ILog log,
																									    ILog listenerLog,
																									    bool isSimulator,
																									    bool autoExit,
																									    bool xmlOutput,
																										bool useTcpTunnel)
		{
			string listenerTempFile = null;
			ISimpleListener listener;
			ListenerTransport transport;

			if (mode == RunMode.WatchOS) {
				transport = isSimulator ? ListenerTransport.File : ListenerTransport.Http;
			} else {
				transport = ListenerTransport.Tcp;
			}

			switch (transport) {
			case ListenerTransport.File:
				listenerTempFile = listenerLog.FullPath + ".tmp";
				listener = new SimpleFileListener (listenerTempFile, log, listenerLog, xmlOutput);
				break;
			case ListenerTransport.Http:
				listener = new SimpleHttpListener (log, listenerLog, autoExit, xmlOutput);
				break;
			case ListenerTransport.Tcp:
				listener = new SimpleTcpListener (log, listenerLog, autoExit, xmlOutput, useTcpTunnel);
				break;
			default:
				throw new NotImplementedException ("Unknown type of listener");
			}

			return (transport, listener, listenerTempFile);
		}
	}
}
