using Prism.Events;

namespace RobotBehaviourDesigner.UI.Event
{
	public class AfterDetailDeletedEvent : PubSubEvent<AfterDetailDeletedEventArgs>
	{
	}

	public class AfterDetailDeletedEventArgs
	{
		public string Id { get; set; }
		public string ViewModelName { get; set; }
	}
}