using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodThrower.Model
{
	public class Feed : BaseModel
	{
		#region Member Variables
		string folder;
		string title;
		#endregion

		#region Properties
		public string Folder
		{
			get { return folder; }
			set
			{
				if (folder != value)
				{
					folder = value;
					LaunchChanged("Folder");
				}
			}
		}

		public string Title
		{
			get { return title; }
			set
			{
				if (title != value)
				{
					title = value;
					LaunchChanged("Title");
				}
			}
		}
		#endregion
	}
}
