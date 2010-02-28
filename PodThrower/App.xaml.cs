using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using PodThrower.Model;

namespace PodThrower
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			/*var notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
			var document = new Document();
			document.Open();
			notifyIcon.DataContext = document;*/
		}
	}
}
