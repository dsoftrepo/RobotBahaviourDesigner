using Prism.Commands;
using System.Windows.Input;
using RobotBehaviourDesigner.UI.Event;
using Prism.Events;
using RobotBehaviourDesigner.UI.ViewModel.Base;

namespace RobotBehaviourDesigner.UI.ViewModel
{
	public class NavigationItemViewModel : ViewModelBase
	{
		private string _displayMember;
		private readonly IEventAggregator _eventAggregator;
		private readonly string _detailViewModelName;

		public NavigationItemViewModel(
			string id,
			string displayMember,
			string detailViewModelName,
			IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
			Id = id;
			DisplayMember = displayMember;
			_detailViewModelName = detailViewModelName;
			OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
		}

		public string Id { get; }

		public string DisplayMember
		{
			get => _displayMember;
			set
			{
				_displayMember = value;
				OnPropertyChanged();
			}
		}

		public ICommand OpenDetailViewCommand { get; }

		private void OnOpenDetailViewExecute()
		{
			_eventAggregator.GetEvent<OpenDetailViewEvent>()
				.Publish(
					new OpenDetailViewEventArgs
					{
						Id = Id,
						ViewModelName = _detailViewModelName
					});
		}
	}
}