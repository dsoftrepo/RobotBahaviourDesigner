using Autofac;
using System.Configuration;
using RobotBehaviourDesigner.UI.Data.Lookups;
using RobotBehaviourDesigner.UI.View.Services;
using RobotBehaviourDesigner.UI.ViewModel;
using Prism.Events;
using RobotBehaviourDesigner.DataAccess.Database;
using RobotBehaviourDesigner.DataAccess.Repositories;
using RobotBehaviourDesigner.Model;
using RobotBehaviourDesigner.UI.ViewModel.Details;
using RobotBehaviourDesigner.UI.ViewModel.Interface;

namespace RobotBehaviourDesigner.UI.Startup
{
	public class Bootstrapper
	{
		public IContainer Bootstrap()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
			builder.RegisterType<MainWindow>().AsSelf();
			builder.RegisterType<MainViewModel>().AsSelf();
			builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
			builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
			
			builder.RegisterType<MotionDetailViewModel>().Keyed<IDetailViewModel>(nameof(MotionDetailViewModel));
			builder.RegisterType<MotionGraphDetailViewModel>().Keyed<IDetailViewModel>(nameof(MotionGraphDetailViewModel));

			string connectionString = ConfigurationManager.ConnectionStrings["RobotBehaviourDesignerDb"].ConnectionString;
			builder.Register(c => new MongoDataBase(connectionString)).As<IDatabase>();
			builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
			builder.RegisterType<MotionRepository>().As<IRepository<Motion>>();

			return builder.Build();
		}
	}
}