using Prism.Events;

namespace RobotBehaviourDesigner.UI.Event
{
	public class AfterCollectionSavedEvent : PubSubEvent<AfterCollectionSavedEventArgs>
	{
	}

	public class AfterCollectionSavedEventArgs
	{
		public string ViewModelName { get; set; }
	}
}