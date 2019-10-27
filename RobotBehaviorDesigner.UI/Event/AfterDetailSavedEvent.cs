using Prism.Events;

namespace RobotBehaviourDesigner.UI.Event
{
	public class AfterDetailSavedEvent : PubSubEvent<AfterDetailSavedEventArgs>
	{
	}

	public class AfterDetailSavedEventArgs
	{
		public string Id { get; set; }
		public string DisplayMember { get; set; }
		public string ViewModelName { get; set; }
	}
}