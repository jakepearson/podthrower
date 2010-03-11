using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace PodThrower.Model
{
	public class Feed : BaseModel
	{
		#region Member Variables
		string folder = "";
		string title = "";
		string image = "";
		string filter = "*.mp3";
		int id;

		ICommand chooseFolderCommand;
		ICommand chooseImageCommand;
		ICommand copyFeedURLCommand;
		ICommand removeFeedCommand;
		#endregion

		#region Properties
		[XmlAttribute]
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

		[XmlAttribute]
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

		[XmlAttribute]
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

		[XmlAttribute]
		public int ID
		{
			get { return id; }
			set
			{
				if (id != value)
				{
					id = value;
					LaunchChanged("ID");
					LaunchChanged("FeedURL");
				}
			}
		}

		[XmlAttribute]
		public string Filter
		{
			get { return filter; }
			set
			{
				if (filter != value)
				{
					filter = value;
					LaunchChanged("Filter");
				}
			}
		}

		public string FeedURL
		{
			get { return Constants.RootURL + "feed/" + ID; }
		}

		public string URL
		{
			get { return @"http://pod.codeplex.com/"; }
		}

		public IEnumerable<FileInfo> Files
		{
			get
			{
				foreach (var file in Directory.GetFiles(Folder, Filter, SearchOption.AllDirectories))
				{
					yield return new FileInfo(file);
				}
			}
		}

		[XmlIgnore]
		public Document Parent
		{
			get;
			set;
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

		public ICommand CopyFeedCommand
		{
			get { return copyFeedURLCommand ?? (copyFeedURLCommand = new RelayCommand(p => CopyFeedURL())); }
		}

		public ICommand RemoveFeedCommand
		{
			get { return removeFeedCommand ?? (removeFeedCommand = new RelayCommand(p => Remove())); }
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
			chooser.FileName = Image;
			if (chooser.ShowDialog() == DialogResult.OK)
			{
				Image = chooser.FileName;
			}
		}

		void CopyFeedURL()
		{
			Clipboard.SetText(FeedURL);
		}

		void Remove()
		{
			Parent.Remove(this);
		}
	}
}
