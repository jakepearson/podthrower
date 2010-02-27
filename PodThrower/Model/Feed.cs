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
		string image;

		ICommand chooseFolderCommand;
		ICommand chooseImageCommand;
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

		public string Image
		{
			get { return image; }
			set
			{
				if (image != value)
				{
					image = value;
					LaunchChanged("Image");
				}
			}
		}

		public string URL
		{
			get { return @"http://pod.codeplex.com/"; }
		}
		#endregion

		#region Commands
		public ICommand ChooseFolderCommand
		{
			get { return chooseFolderCommand ?? (chooseFolderCommand = new RelayCommand(p => ChooseFolder())); }
		}

		public ICommand ChooseImageCommand
		{
			get { return chooseImageCommand ?? (chooseImageCommand = new RelayCommand(p => ChooseImage())); }
		}
		#endregion

		void ChooseFolder()
		{
		}

		void ChooseImage()
		{
		}
	}
}
