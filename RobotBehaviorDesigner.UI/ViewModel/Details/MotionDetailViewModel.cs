using System.Threading.Tasks;
using MongoDB.Bson;
using Prism.Commands;
using Prism.Events;
using RobotBehaviourDesigner.DataAccess.Repositories;
using RobotBehaviourDesigner.Model;
using RobotBehaviourDesigner.UI.Event;
using RobotBehaviourDesigner.UI.View.Services;
using RobotBehaviourDesigner.UI.ViewModel.Base;
using RobotBehaviourDesigner.UI.ViewModel.Interface;
using RobotBehaviourDesigner.UI.Wrapper;

namespace RobotBehaviourDesigner.UI.ViewModel.Details
{
	public class MotionDetailViewModel : DetailViewModelBase, IMotionDetailViewModel
	{
		private MotionWrapper _motion;
		private readonly IRepository<Motion> _motionRepository;

		public MotionDetailViewModel(
			IRepository<Motion> motionRepository,
			IEventAggregator eventAggregator,
			IMessageDialogService messageDialogService)
			: base(eventAggregator, messageDialogService)
		{
			_motionRepository = motionRepository;
			eventAggregator.GetEvent<AfterCollectionSavedEvent>().Subscribe(AfterCollectionSaved);
		}

		public override async Task LoadAsync(string motionId)
		{
			Motion motion = !string.IsNullOrEmpty(motionId)
				? await _motionRepository.FindAsync(x=>x.Id == motionId)
				: CreateNewMotion();
			Id = motionId;
			InitializeMotion(motion);
		}

		private void InitializeMotion(Motion motion)
		{
			Motion = new MotionWrapper(motion);
			Motion.PropertyChanged += (s, e) =>
			{
				if (!HasChanges) HasChanges = _motionRepository.HasChanges();
				if (e.PropertyName == nameof(Motion.HasErrors)) ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
				if (e.PropertyName == nameof(Motion.Name)) SetTitle();
			};

			((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();

			// Little trick to trigger the validation
			if (string.IsNullOrEmpty(Motion.Id)) Motion.Name = "";

			SetTitle();
		}

		private void SetTitle()
		{
			Title = Motion.Name;
		}

		public MotionWrapper Motion
		{
			get => _motion;
			private set
			{
				_motion = value;
				OnPropertyChanged();
			}
		}

		protected override async void OnSaveExecute()
		{
			await SaveAsync(_motionRepository.SaveChangesAsync,
				() =>
				{
					HasChanges = _motionRepository.HasChanges();
					Id = Motion.Id;
					RaiseDetailSavedEvent(Motion.Id, Motion.Name);
				});
		}

		protected override bool OnSaveCanExecute()
		{
			return Motion != null
			       && !Motion.HasErrors
			       && HasChanges;
		}

		protected override async void OnDeleteExecute()
		{
			MessageDialogResult result = await MessageDialogService.ShowOkCancelDialogAsync(
				$"Do you really want to delete the friend {Motion.Name} ?",
				"Question");

			if (result != MessageDialogResult.Ok) return;

			_motionRepository.Remove(Motion.Model);
			await _motionRepository.SaveChangesAsync();
			RaiseDetailDeletedEvent(Motion.Id);
		}

		private Motion CreateNewMotion()
		{
			var motion = new Motion();
			_motionRepository.Add(motion);
			return motion;
		}

		private void AfterCollectionSaved(AfterCollectionSavedEventArgs obj)
		{
		}
	}
}