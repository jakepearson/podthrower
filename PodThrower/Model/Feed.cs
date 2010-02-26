using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PodThrower.Model
{
	public class Feed : BaseModel
	{
		#region Member Variables
		string folder;
		string title;

		ICommand chooseFolderCommand;
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

		#region Commands
		public ICommand ChooseFolderCommand
		{
			get { return chooseFolderCommand ?? (chooseFolderCommand = new RelayCommand(p => ChooseFolder())); }
		}
		#endregion

		void ChooseFolder()
		{

		}
	}
}
