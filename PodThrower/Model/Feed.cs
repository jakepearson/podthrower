using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;

namespace PodThrower.Model
{
	public class Feed : BaseModel
	{
		#region Member Variables
		string folder;
		string title;
		string image;
		int id;

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

		public int ID
		{
			get { return id; }
			set
			{
				if (id != value)
				{
					id = value;
					LaunchChanged("ID");
				}
			}
		}

		public string URL
		{
			get { return @"http://pod.codeplex.com/"; }
		}

		public IEnumerable<FileInfo> Files
		{
			get
			{
				foreach (var file in Directory.GetFiles(Folder, "*.mp3", SearchOption.AllDirectories))
				{
					yield return new FileInfo(file);
				}
			}
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
			var chooser = new FolderBrowserDialog();
			chooser.SelectedPath = Folder;
			if (chooser.ShowDialog() == DialogResult.OK)
			{
				Folder = chooser.SelectedPath;
			}
		}

		void ChooseImage()
		{
			var chooser = new OpenFileDialog();
		}
	}
}
