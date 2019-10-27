using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using RobotBehaviourDesigner.UI.Event;
using RobotBehaviourDesigner.UI.View.Services;
using RobotBehaviourDesigner.UI.ViewModel.Interface;

namespace RobotBehaviourDesigner.UI.ViewModel.Base
{
	public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
	{
		private bool _hasChanges;
		protected readonly IEventAggregator EventAggregator;
		protected readonly IMessageDialogService MessageDialogService;
		private string _title;

		protected DetailViewModelBase(IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
		{
			EventAggregator = eventAggregator;
			MessageDialogService = messageDialogService;
			SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
			DeleteCommand = new DelegateCommand(OnDeleteExecute);
			CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
		}

		public abstract Task LoadAsync(string id);

		public ICommand SaveCommand { get; }

		public ICommand DeleteCommand { get; }

		public ICommand CloseDetailViewCommand { get; }

		public string Id { get; protected set; }

		public string Title
		{
			get => _title;
			protected set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		public bool HasChanges
		{
			get => _hasChanges;
			set
			{
				if (_hasChanges == value) return;

				_hasChanges = value;
				OnPropertyChanged();
				((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
			}
		}

		protected abstract void OnDeleteExecute();

		protected abstract bool OnSaveCanExecute();

		protected abstract void OnSaveExecute();

		protected virtual void RaiseDetailDeletedEvent(string modelId)
		{
			EventAggregator
				.GetEvent<AfterDetailDeletedEvent>()
				.Publish(new AfterDetailDeletedEventArgs
				{
					Id = modelId,
					ViewModelName = GetType().Name
				});
		}

		protected virtual void RaiseDetailSavedEvent(string modelId, string displayMember)
		{
			EventAggregator
				.GetEvent<AfterDetailSavedEvent>()
				.Publish(new AfterDetailSavedEventArgs
				{
					Id = modelId,
					DisplayMember = displayMember,
					ViewModelName = GetType().Name
				});
		}

		protected virtual void RaiseCollectionSavedEvent()
		{
			EventAggregator
				.GetEvent<AfterCollectionSavedEvent>()
				.Publish(new AfterCollectionSavedEventArgs
				{
					ViewModelName = GetType().Name
				});
		}

		protected virtual async void OnCloseDetailViewExecute()
		{
			if (HasChanges)
			{
				MessageDialogResult result =
					await MessageDialogService.ShowOkCancelDialogAsync("You've made changes. Close this item?",
						"Question");
				if (result == MessageDialogResult.Cancel) return;
			}

			EventAggregator
				.GetEvent<AfterDetailClosedEvent>()
				.Publish(new AfterDetailClosedEventArgs
				{
					Id = Id,
					ViewModelName = GetType().Name
				});
		}

		protected async Task SaveAsync(Func<Task> saveFunc, Action afterSaveAction)
		{
			try
			{
				await saveFunc();
			}
			catch (Exception ex)
			{
				await MessageDialogService.ShowInfoDialogAsync(ex.Message);
				return;
			}

			afterSaveAction();
		}
	}
}