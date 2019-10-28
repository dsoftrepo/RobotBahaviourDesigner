using Autofac.Features.Indexed;
using RobotBehaviourDesigner.UI.Event;
using RobotBehaviourDesigner.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RobotBehaviourDesigner.UI.ViewModel.Base;
using RobotBehaviourDesigner.UI.ViewModel.Interface;

namespace RobotBehaviourDesigner.UI.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private IDetailViewModel _selectedDetailViewModel;
		private readonly IMessageDialogService _messageDialogService;
		private readonly IIndex<string, IDetailViewModel> _detailViewModelCreator;

		public MainViewModel(
			INavigationViewModel navigationViewModel,
			IIndex<string, IDetailViewModel> detailViewModelCreator,
			IEventAggregator eventAggregator,
			IMessageDialogService messageDialogService,
			string dataSourceInfo)
		{
			DataSourceInfo = dataSourceInfo;
			_detailViewModelCreator = detailViewModelCreator;
			_messageDialogService = messageDialogService;

			DetailViewModels = new ObservableCollection<IDetailViewModel>();

			eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailView);
			eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
			eventAggregator.GetEvent<AfterDetailClosedEvent>().Subscribe(AfterDetailClosed);

			CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
			OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleDetailViewExecute);

			NavigationViewModel = navigationViewModel;
		}

		public async Task LoadAsync()
		{
			await NavigationViewModel.LoadAsync();
		}

		public string DataSourceInfo { get; }

		public ICommand CreateNewDetailCommand { get; }

		public ICommand OpenSingleDetailViewCommand { get; }

		public INavigationViewModel NavigationViewModel { get; }

		public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

		public IDetailViewModel SelectedDetailViewModel
		{
			get => _selectedDetailViewModel;
			set
			{
				_selectedDetailViewModel = value;
				OnPropertyChanged();
			}
		}

		private async void OnOpenDetailView(OpenDetailViewEventArgs args)
		{
			IDetailViewModel detailViewModel = DetailViewModels.SingleOrDefault(vm => vm.Id == args.Id && vm.GetType().Name == args.ViewModelName);

			if (detailViewModel == null)
			{
				detailViewModel = _detailViewModelCreator[args.ViewModelName];
				try
				{
					await detailViewModel.LoadAsync(args.Id);
				}
				catch(Exception ex)
				{
					await _messageDialogService.ShowInfoDialogAsync($"Could not load the entity {ex.Message}");
					await NavigationViewModel.LoadAsync();
					return;
				}

				DetailViewModels.Add(detailViewModel);
			}

			SelectedDetailViewModel = detailViewModel;
		}
		
		private void OnCreateNewDetailExecute(Type viewModelType)
		{
			OnOpenDetailView(
				new OpenDetailViewEventArgs
				{
					Id = string.Empty,
					ViewModelName = viewModelType.Name
				});
		}

		private void OnOpenSingleDetailViewExecute(Type viewModelType)
		{
			OnOpenDetailView(
				new OpenDetailViewEventArgs
				{
					Id = string.Empty,
					ViewModelName = viewModelType.Name
				});
		}

		private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
		{
			RemoveDetailViewModel(args.Id, args.ViewModelName);
		}

		private void AfterDetailClosed(AfterDetailClosedEventArgs args)
		{
			RemoveDetailViewModel(args.Id, args.ViewModelName);
		}

		private void RemoveDetailViewModel(string id, string viewModelName)
		{
			IDetailViewModel detailViewModel = DetailViewModels.SingleOrDefault(vm => vm.Id == id && vm.GetType().Name == viewModelName);
			
			if (detailViewModel != null) DetailViewModels.Remove(detailViewModel);
		}
	}
}