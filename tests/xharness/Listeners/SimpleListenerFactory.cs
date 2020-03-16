using System;
using Xharness.Logging;

namespace Xharness.Listeners {
	public enum ListenerTransport {
		Tcp,
		Http,
		File,
	}

	public interface ISimpleListenerFactory {
		(ListenerTransport transport, ISimpleListener listener, string listenerTempFile) Create (RunMode mode, ILog listenerLog, bool isSimulator);
	}

	public class SimpleListenerFactory : ISimpleListenerFactory {

		public (ListenerTransport transport, ISimpleListener listener, string listenerTempFile) Create (RunMode mode, ILog listenerLog, bool isSimulator)
		{
			string listenerTempFile = null;
			ISimpleListener listener;

			listenerTempFile = null;
			ListenerTransport transport;
			if (mode == RunMode.WatchOS) {
				transport = isSimulator ? ListenerTransport.File : ListenerTransport.Http;
			} else {
				transport = ListenerTransport.Tcp;
			}

			switch (transport) {
			case ListenerTransport.File:
				listenerTempFile = listenerLog.FullPath + ".tmp";
				listener = new SimpleFileListener (listenerTempFile);
				break;
			case ListenerTransport.Http:
				listener = new SimpleHttpListener ();
				break;
			case ListenerTransport.Tcp:
				listener = new SimpleTcpListener ();
				break;
			default:
				throw new NotImplementedException ();
			}

			return (transport, listener, listenerTempFile);
		}
	}
}
