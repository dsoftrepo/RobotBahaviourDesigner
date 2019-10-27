using Prism.Events;

namespace RobotBehaviourDesigner.UI.Event
{
	public class AfterDetailClosedEvent : PubSubEvent<AfterDetailClosedEventArgs>
	{
	}

	public class AfterDetailClosedEventArgs
	{
		public string Id { get; set; }
		public string ViewModelName { get; set; }
	}
}