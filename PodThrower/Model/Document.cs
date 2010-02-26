﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PodThrower.View;

namespace PodThrower.Model
{
	public class Document : BaseModel
	{
		#region Member Variables
		int port;
		bool connected;
		ObservableCollection<Feed> feeds;
		ICommand editCommand;
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
		#endregion

		void ShowEditor()
		{
		}
	}
}
