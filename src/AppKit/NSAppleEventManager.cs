
namespace Foundation {
	partial class NSAppleEventManager {
#if !XAMCORE_2_0
		public void RemoveEventHandler (AEEventClass eventClass, AEEventID eventID)
		{
			RemoveEventHandlerForEventClassandEventID (eventClass, eventID);
		}
#endif
	}
}