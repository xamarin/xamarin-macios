using System;
using Xharness.Logging;

namespace Xharness.Listeners {
	public enum ListenerTransport {
		Tcp,
		Http,
		File,
	}

	public interface ISimpleListenerFactory {
		IMetro Metro { get; }
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
		public IMetro Metro { get; private set; }

		public SimpleListenerFactory (IMetro metro)
		{
			Metro = metro ?? throw new ArgumentNullException (nameof (metro));
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
				// if there is a tunnel, and we want to use it, re-use the port
				if (Metro.HasTunnel (device, out var tunnel)) {
					listener = new SimpleTcpListener (tunnel.Port, log, listenerLog, autoExit, xmlOutput, useTcpTunnel);
				} else { 
					listener = new SimpleTcpListener (log, listenerLog, autoExit, xmlOutput, useTcpTunnel);
				}
				break;
			default:
				throw new NotImplementedException ("Unknown type of listener");
			}

			return (transport, listener, listenerTempFile);
		}
	}
}
