﻿using Autofac;
using RobotBehaviourDesigner.UI.Startup;
using System;
using System.Windows;

namespace RobotBehaviourDesigner.UI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			var bootstrapper = new Bootstrapper();
			IContainer container = bootstrapper.Bootstrap();

			var mainWindow = container.Resolve<MainWindow>();
			mainWindow.Show();
		}

		private void Application_DispatcherUnhandledException(object sender,
			System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show("Unexpected error occured. Please inform the admin."
			                + Environment.NewLine + e.Exception.Message, "Unexpected error");

			e.Handled = true;
		}
	}
}