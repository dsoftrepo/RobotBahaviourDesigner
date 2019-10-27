using Prism.Events;

namespace RobotBehaviourDesigner.UI.Event
{
	public class OpenDetailViewEvent : PubSubEvent<OpenDetailViewEventArgs>
	{
	}

	public class OpenDetailViewEventArgs
	{
		public string Id { get; set; }
		public string ViewModelName { get; set; }
	}
}