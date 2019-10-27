using System.Collections.Generic;
using RobotBehaviourDesigner.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using RobotBehaviourDesigner.Model;
using RobotBehaviourDesigner.UI.Data.Lookups;
using RobotBehaviourDesigner.UI.ViewModel.Base;
using RobotBehaviourDesigner.UI.ViewModel.Details;
using RobotBehaviourDesigner.UI.ViewModel.Interface;

namespace RobotBehaviourDesigner.UI.ViewModel
{
	public class NavigationViewModel : ViewModelBase, INavigationViewModel
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly IMotionLookupDataService _motionLookupService;

		public NavigationViewModel(
			IMotionLookupDataService motionLookupService,
			IEventAggregator eventAggregator)
		{
			Motions = new ObservableCollection<NavigationItemViewModel>();
			
			_motionLookupService = motionLookupService;
			_eventAggregator = eventAggregator;
			_eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
			_eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
		}

		public async Task LoadAsync()
		{
			IEnumerable<LookupItem> lookup = await _motionLookupService.GetMotionLookupAsync();
			Motions.Clear();
			foreach (LookupItem item in lookup)
			{
				Motions.Add(new NavigationItemViewModel(item.Id, item.DisplayMember, nameof(MotionDetailViewModel), _eventAggregator));
			}
		}

		public ObservableCollection<NavigationItemViewModel> Motions { get; }

		private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
		{
			switch (args.ViewModelName)
			{
				case nameof(MotionDetailViewModel):
					AfterDetailDeleted(Motions, args);
					break;
			}
		}

		private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
		{
			NavigationItemViewModel item = items.SingleOrDefault(f => f.Id == args.Id);
			if (item != null) items.Remove(item);
		}

		private void AfterDetailSaved(AfterDetailSavedEventArgs args)
		{
			switch (args.ViewModelName)
			{
				case nameof(MotionDetailViewModel):
					AfterDetailSaved(Motions, args);
					break;
			}
		}

		private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> items, AfterDetailSavedEventArgs args)
		{
			NavigationItemViewModel lookupItem = items.SingleOrDefault(l => l.Id == args.Id);
			if (lookupItem == null)
				items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember, args.ViewModelName, _eventAggregator));
			else
				lookupItem.DisplayMember = args.DisplayMember;
		}
	}
}