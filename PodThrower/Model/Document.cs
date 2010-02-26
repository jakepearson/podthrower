using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PodThrower.Model
{
	public class Document : BaseModel
	{
		#region Member Variables
		int port;
		ObservableCollection<Feed> feeds;
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

		public ObservableCollection<Feed> Feeds
		{
			get { return feeds ?? (feeds = new ObservableCollection<Feed>()); }
		}
		#endregion
	}
}
