using System.Threading.Tasks;

namespace RobotBehaviourDesigner.UI.ViewModel.Interface
{
	public interface IDetailViewModel
	{
		Task LoadAsync(string id);
		bool HasChanges { get; }
		string Id { get; }
	}
}