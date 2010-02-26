using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PodThrower.View;
using System.Windows;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace PodThrower.Model
{
	public class Document : BaseModel
	{
		#region Member Variables
		int port = 8000;
		bool connected;
		ObservableCollection<Feed> feeds;
		WebServiceHost host;

		ICommand editCommand;
		ICommand connectCommand;
		#endregion

		#region Properties
		public int Port
		{
			get { return port; }
			set
			{
				if (port != value)
				{
					port = value;
					LaunchChanged("Port");
				}
			}
		}

		public bool Connected
		{
			get { return connected; }
			set
			{
				if (connected != value)
				{
					connected = value;
					LaunchChanged("Connected");
					LaunchChanged("Message");
				}
			}
		}

		public string Message
		{
			get { return "Pod Thrower: " + (Connected ? "Connected" : "Not Connected"); }
		}

		public ObservableCollection<Feed> Feeds
		{
			get { return feeds ?? (feeds = new ObservableCollection<Feed>()); }
		}
		#endregion

		#region Commands
		public ICommand EditCommand
		{
			get { return editCommand ?? (editCommand = new RelayCommand(p => ShowEditor())); }
		}

		public ICommand ConnectCommand
		{
			get { return connectCommand ?? (connectCommand = new RelayCommand(p => Connect())); }
		}
		#endregion

		void ShowEditor()
		{
		}

		public void Connect()
		{
			Uri address = new Uri("http://localhost:" + Port + "/PodThrower/");
			host = new WebServiceHost(typeof(NewsFeedService), address);

			try
			{
				host.Open();
			}
			catch (CommunicationException ce)
			{
				MessageBox.Show(ce.ToString());
				host.Abort();
				host = null;
			}
		}
	}
}
