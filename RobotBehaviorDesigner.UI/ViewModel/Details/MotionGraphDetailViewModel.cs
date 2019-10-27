using System.Threading.Tasks;
using Prism.Events;
using RobotBehaviourDesigner.UI.View.Services;
using RobotBehaviourDesigner.UI.ViewModel.Base;

namespace RobotBehaviourDesigner.UI.ViewModel.Details
{
	public class MotionGraphDetailViewModel : DetailViewModelBase
	{
		public MotionGraphDetailViewModel(
			IEventAggregator eventAggregator,
			IMessageDialogService messageDialogService)
			: base(eventAggregator, messageDialogService)
		{
		}

		public override Task LoadAsync(string id)
		{
			throw new System.NotImplementedException();
		}

		protected override void OnDeleteExecute()
		{
			throw new System.NotImplementedException();
		}

		protected override bool OnSaveCanExecute()
		{
			throw new System.NotImplementedException();
		}

		protected override void OnSaveExecute()
		{
			throw new System.NotImplementedException();
		}
	}
}