using System;
using Xharness.Logging;

namespace Xharness.Listeners {
	public enum ListenerTransport {
		Tcp,
		Http,
		File,
	}

	public interface ISimpleListenerFactory {
		ListenerTransport Create (string mode, ILog listenerLog, bool isSimulator, out SimpleListener listener, out string listenerFileTemp);
	}

	public class SimpleListenerFactory : ISimpleListenerFactory {

		public ListenerTransport Create (string mode, ILog listenerLog, bool isSimulator, out SimpleListener listener, out string listenerFileTemp)
		{
			listenerFileTemp = null;
			ListenerTransport transport;
			if (mode == "watchos") {
				transport = isSimulator ? ListenerTransport.File : ListenerTransport.Http;
			} else {
				transport = ListenerTransport.Tcp;
			}

			switch (transport) {
			case ListenerTransport.File:
				listenerFileTemp = listenerLog.FullPath + ".tmp";
				listener = new SimpleFileListener (listenerFileTemp);
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
			return transport;
		}
	}
}
